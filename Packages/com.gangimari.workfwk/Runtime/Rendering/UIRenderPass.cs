// ref. https://media.colorfulpalette.co.jp/n/nffc0ece136be

// TODO: RenderGraph対応
// https://forum.unity.com/threads/introduction-of-render-graph-in-the-universal-render-pipeline-urp.1500833/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#pragma warning disable CS0618
#pragma warning disable CS0672

public class UIRenderPass : ScriptableRenderPass
{
    private readonly ProfilingSampler _uiBlurBelowSampler = new("UIBlurBelow");
    private readonly ProfilingSampler _uiDefaultSampler = new("Default");
    private readonly ProfilingSampler _uiBlurAboveSampler = new("UIBlurAbove");
    private readonly ProfilingSampler _captureAndBlurSampler = new("CaptureAndBlur");
    private readonly ProfilingSampler _blurSampler = new("Blur");

    private static class ShaderID
    {
        public static readonly int SimpleBlurParams = Shader.PropertyToID("_SimpleBlurParams");
        public static readonly int SourceTex = Shader.PropertyToID("_SourceTex");
        public static readonly int BlurCaptureTex = Shader.PropertyToID("_BlurCaptureTex");

        public static readonly string BlurCaptureRTName = "BlurCaptureRT";
        public static readonly string TemporaryBlurRT1Name = "TemporaryBlurRT1";
        public static readonly string TemporaryBlurRT2Name = "TemporaryBlurRT2";
        public static readonly string TemporaryBlurRT3Name = "TemporaryBlurRT3";
    }

    private readonly Material _blurMaterial;
    private RenderStateBlock _stateBlock;
    private FilteringSettings _belowFilteringSettings;
    private FilteringSettings _defaultFilteringSettings;
    private FilteringSettings _aboveFilteringSettings;
    private readonly List<ShaderTagId> _shaderTagIds;

    private RTHandle _blurCaptureRT;
    private RTHandle _blurTemporaryRT1;
    private RTHandle _blurTemporaryRT2;
    private RTHandle _blurTemporaryRT3;

    private float _blurBlendRate;
    private float _blurSize;
    private float _blurRenderScale;
    private bool _isExecBlur;
    private bool _isExecGlassMorphism;

    public UIRenderPass(Material blurMaterial, LayerMask layerMask)
    {
        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        _blurMaterial = blurMaterial;

        _belowFilteringSettings = new FilteringSettings(RenderQueueRange.transparent, layerMask);
        var belowSortingLayer = (short)SortingLayer.GetLayerValueFromName("UIBlurBelow");
        _belowFilteringSettings.sortingLayerRange = new SortingLayerRange(belowSortingLayer, belowSortingLayer);

        _defaultFilteringSettings = new FilteringSettings(RenderQueueRange.transparent, layerMask);
        var defaultSortingLayer = (short)SortingLayer.GetLayerValueFromName("Default");
        _defaultFilteringSettings.sortingLayerRange =
            new SortingLayerRange(defaultSortingLayer, defaultSortingLayer);

        _aboveFilteringSettings = new FilteringSettings(RenderQueueRange.transparent, layerMask);
        var aboveSortingLayer = (short)SortingLayer.GetLayerValueFromName("UIBlurAbove");
        _aboveFilteringSettings.sortingLayerRange = new SortingLayerRange(aboveSortingLayer, aboveSortingLayer);

        _stateBlock = new RenderStateBlock();

        _shaderTagIds = new List<ShaderTagId>()
            {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("SRPDefaultUnlit"),
            };
    }

    public void Setup(
        bool isExecBlur,
        bool isExecGlassMorphism,
        float blurBlendRate,
        float blurSize,
        float blurRenderScale)
    {
        _isExecBlur = isExecBlur;
        _isExecGlassMorphism = isExecGlassMorphism;
        _blurBlendRate = blurBlendRate;
        _blurSize = blurSize;
        _blurRenderScale = blurRenderScale;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        base.OnCameraSetup(cmd, ref renderingData);

        // ブラー適用のための一時バッファ確保
        var rtDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        rtDescriptor.depthBufferBits = 0;
        RenderingUtils.ReAllocateIfNeeded(ref _blurTemporaryRT3, rtDescriptor, FilterMode.Bilinear,
            TextureWrapMode.Clamp, name: ShaderID.TemporaryBlurRT3Name);

        rtDescriptor.width = (int)(rtDescriptor.width * _blurRenderScale);
        rtDescriptor.height = (int)(rtDescriptor.height * _blurRenderScale);
        RenderingUtils.ReAllocateIfNeeded(ref _blurTemporaryRT1, rtDescriptor, FilterMode.Bilinear,
            TextureWrapMode.Clamp, name: ShaderID.TemporaryBlurRT1Name);
        RenderingUtils.ReAllocateIfNeeded(ref _blurTemporaryRT2, rtDescriptor, FilterMode.Bilinear,
            TextureWrapMode.Clamp, name: ShaderID.TemporaryBlurRT2Name);
        RenderingUtils.ReAllocateIfNeeded(ref _blurCaptureRT, rtDescriptor, FilterMode.Bilinear,
            TextureWrapMode.Clamp,
            name: ShaderID.BlurCaptureRTName);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        /*
         描画フローとしては、以下を想定
         ・カメラカラーバッファ
            ・UIBlurBelowのUI描画
         ・ぼかしキャプチャバッファ（グラスモーフィズム（すりガラス）的なUIが存在する場合に実行）
            ・この時点のカメラカラーバッファをコピー → ブラー適用
         ・カメラカラーバッファ
            ・DefaultのUI描画 → ブラー適用（全体ブラー有効時に実行） → UIBlurAboveのUI描画

         全てのUIはこのパス上で上記のように描画される想定
        */

        var cameraColorRT = renderingData.cameraData.renderer.cameraColorTargetHandle;
        if (cameraColorRT == null || cameraColorRT.rt == null)
        {
            return;
        }

        const SortingCriteria sortFlags = SortingCriteria.CommonTransparent;
        DrawingSettings drawingSettings = CreateDrawingSettings(_shaderTagIds, ref renderingData, sortFlags);
        CommandBuffer cmd = CommandBufferPool.Get();

        // グラスモーフィズム（すりガラス）効果有効時、カメラカラーバッファをコピー → 縦横ブラー適用
        if (_isExecGlassMorphism)
        {
            // UIBlurBelowのUI描画
            // このレイヤーはグラスモーフィズムが有効の時しか使わない想定
            using (new ProfilingScope(cmd, _uiBlurBelowSampler))
            {
                // ProfilingScope内でコマンド追加されているのでまず実行、コマンドを空にしておく
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _belowFilteringSettings,
                    ref _stateBlock);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            using (new ProfilingScope(cmd, _captureAndBlurSampler))
            {
                // ProfilingScope内でコマンド追加されているのでまず実行、コマンドを空にしておく
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                ApplyBlur(
                    cmd,
                    cameraColorRT,
                    _blurCaptureRT,
                    _blurSize,
                    _blurBlendRate);
            }

            CoreUtils.SetRenderTarget(cmd, cameraColorRT);
            cmd.SetGlobalTexture(ShaderID.BlurCaptureTex, _blurCaptureRT);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }

        // DefaultのUI描画
        using (new ProfilingScope(cmd, _uiDefaultSampler))
        {
            // ProfilingScope内でコマンド追加されているのでまず実行、コマンドを空にしておく
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _defaultFilteringSettings,
                ref _stateBlock);
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        // 全体ブラー有効時、カメラカラーバッファに縦横ブラー適用
        if (_isExecBlur)
        {
            using (new ProfilingScope(cmd, _blurSampler))
            {
                // ProfilingScope内でコマンド追加されているのでまず実行、コマンドを空にしておく
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                ApplyBlur(
                    cmd,
                    cameraColorRT,
                    cameraColorRT,
                    _blurSize,
                    _blurBlendRate);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }

        // UIBlurAboveのUI描画
        using (new ProfilingScope(cmd, _uiBlurAboveSampler))
        {
            // ProfilingScope内でコマンド追加されているのでまず実行、コマンドを空にしておく
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _aboveFilteringSettings,
                ref _stateBlock);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
        ReleaseTemporaryRT();
    }

    private void ApplyBlur(
        CommandBuffer cmd,
        RTHandle source,
        RTHandle destination,
        float blurSize,
        float blendRate)
    {
        if (_blurMaterial == null)
        {
            return;
        }

        Vector4 blurParams = Vector4.zero;
        blurParams.x = blurSize;
        blurParams.y = blendRate;
        const int blurPass = 0;
        const int blurFinalPass = 1;

        // ブラーを適用し一時バッファに描画
        _blurMaterial.SetVector(ShaderID.SimpleBlurParams, blurParams);
        Blitter.BlitCameraTexture(cmd, source, _blurTemporaryRT1, _blurMaterial, blurPass);
        Blitter.BlitCameraTexture(cmd, _blurTemporaryRT1, _blurTemporaryRT2, _blurMaterial, blurFinalPass);

        // 描画元と描画先が同じ場合、一旦別バッファに逃がして使用
        if (source == destination)
        {
            Blitter.BlitCameraTexture(cmd, source, _blurTemporaryRT3);
            cmd.SetGlobalTexture(ShaderID.SourceTex, _blurTemporaryRT3);
        }
        else
        {
            cmd.SetGlobalTexture(ShaderID.SourceTex, source);
        }

        // 描画先に反映
        Blitter.BlitCameraTexture(cmd, _blurTemporaryRT2, destination, _blurMaterial, blurFinalPass);
    }

    private void ReleaseTemporaryRT()
    {
        _blurCaptureRT?.Release();
        _blurTemporaryRT1?.Release();
        _blurTemporaryRT2?.Release();
        _blurTemporaryRT3?.Release();
        _blurCaptureRT = null;
        _blurTemporaryRT1 = null;
        _blurTemporaryRT2 = null;
        _blurTemporaryRT3 = null;
    }
}

#pragma warning restore CS0672
#pragma warning restore CS0618
