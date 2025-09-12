using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed class OutlineRenderPass : ScriptableRenderPass
{
    private const string ProfilerTag = nameof(OutlineRenderPass);
    private readonly ProfilingSampler _profilingSampler = new ProfilingSampler(ProfilerTag);

    private readonly RenderQueueRange _renderQueueRange;

    public OutlineRenderPass(RenderPassEvent renderPassEvent, RenderQueueRange renderQueueRange)
    {
        _renderQueueRange = renderQueueRange;
        this.renderPassEvent = renderPassEvent;
    }

    [Obsolete]
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, _profilingSampler))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            var camera = renderingData.cameraData.camera;
            var shaderTagId = new ShaderTagId("MToonOutline");
            var sortingSettings = new SortingSettings(camera);
            var drawingSettings = new DrawingSettings(shaderTagId, sortingSettings)
            {
                perObjectData = PerObjectData.ReflectionProbes | PerObjectData.Lightmaps |
                                PerObjectData.LightProbe | PerObjectData.LightData | PerObjectData.OcclusionProbe |
                                PerObjectData.ShadowMask
            };
            var filteringSettings = FilteringSettings.defaultValue;
            filteringSettings.renderQueueRange = _renderQueueRange;
#if UNITY_2022_2_OR_NEWER
            var rendererListParams = new RendererListParams
            {
                cullingResults = renderingData.cullResults,
                drawSettings = drawingSettings,
                filteringSettings = filteringSettings,
            };
            var rendererList = context.CreateRendererList(ref rendererListParams);
            cmd.DrawRendererList(rendererList);
#else
                var renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings,
                    ref renderStateBlock);
#endif
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }
}
