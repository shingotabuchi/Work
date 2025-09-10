// ref. https://media.colorfulpalette.co.jp/n/nffc0ece136be

Shader "SimpleBlur"
{
    HLSLINCLUDE
    #pragma target 3.0
    #pragma exclude_renderers gles
    #pragma multi_compile _ _USE_DRAW_PROCEDURAL

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    TEXTURE2D_X(_SourceTex);

    float4 _SimpleBlurParams;
    #define TEXEL_OFFSET (_SimpleBlurParams.x)
    #define BLUR_BLEND_RATE (_SimpleBlurParams.y)

    static const int BLUR_SAMPLE_COUNT = 8;
    static const float2 BLUR_KERNEL[BLUR_SAMPLE_COUNT] =
    {
        float2(-1.0f, -1.0f),
        float2(-1.0f, 1.0f),
        float2(1.0f, -1.0f),
        float2(1.0f, 1.0f),
        float2(-0.70711f, 0.0f),
        float2(0.0f, 0.70711f),
        float2(0.70711f, 0.0f),
        float2(0.0f, -0.70711f),
    };
    static const float BLUR_KERNEL_WEIGHT[BLUR_SAMPLE_COUNT] =
    {
        0.100f,
        0.100f,
        0.100f,
        0.100f,
        0.150f,
        0.150f,
        0.150f,
        0.150f,
    };

    // ブラーサンプリング
    half4 Blur(half2 uv, half2 targetTexelSize)
    {
        float2 scale = (TEXEL_OFFSET * BLUR_BLEND_RATE) * 0.002f;
        scale.y *= targetTexelSize.y / targetTexelSize.x;
        half4 sumColor = 0.0h;
        half sumWeight = 0.0h;

        UNITY_UNROLL for (int i = 0; i < BLUR_SAMPLE_COUNT; i++)
        {
            const half4 sampledColor = SAMPLE_TEXTURE2D_X(
                _BlitTexture,
                sampler_LinearClamp,
                uv + BLUR_KERNEL[i] * scale
            );

            sumColor += sampledColor * BLUR_KERNEL_WEIGHT[i];
            sumWeight += BLUR_KERNEL_WEIGHT[i];
        }

        return sumColor / sumWeight;
    }

    half4 FragBlur(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        const half2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);
        const half2 res = _BlitTexture_TexelSize.xy;

        return Blur(uv, res);
    }

    half4 FragBlurBlend(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        const half2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);
        half4 blurColor = Blur(uv, _BlitTexture_TexelSize.xy);
        half4 sourceColor = SAMPLE_TEXTURE2D_X(_SourceTex, sampler_LinearClamp, uv);

        half4 finalColor;
        finalColor.rgb = blurColor.rgb;
        finalColor.a = sourceColor.a; // 元のアルファを保持

        return finalColor;
    }
    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "Simple Blur"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragBlur
            ENDHLSL
        }

        Pass
        {
            Name "Simple Blur Blend"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragBlurBlend
            ENDHLSL
        }
    }
}
