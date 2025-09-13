Shader "Custom/CoolToon"
{
    // Refined CoolToon Shader with lilToon-inspired outline system
    // Features: Distance-based outline scaling, vertex color modulation, 
    //          texture masking, improved toon shading, organized properties
    Properties
    {
        [Header(Base)]
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _BaseMap ("Base Map", 2D) = "white" {}
        
        [Header(Transparency)]
        [Enum(Opaque,0,Cutout,1,Fade,2,Transparent,3)] _BlendMode ("Blend Mode", Float) = 0
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        
        [Header(Toon Shading)]
        _ShadowColor ("Shadow Color", Color) = (0.5,0.5,0.5,1)
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _ShadowSmoothness ("Shadow Smoothness", Range(0,0.1)) = 0.01
        _ShadingStrength ("Shading Strength", Range(0,1)) = 1
        _AmbientStrength ("Ambient Strength", Range(0,2)) = 1
        
        [Header(Rim Light)]
        [Toggle(_RIM)] _EnableRim ("Enable Rim Light", Float) = 1
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.1,16)) = 4
        _RimIntensity ("Rim Intensity", Range(0,2)) = 0.5
        
        [Header(Outline)]
        [Toggle(_OUTLINE)] _UseOutline ("Enable Outline", Float) = 1
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0,2)) = 0.1
        [NoScaleOffset]_OutlineWidthMask ("Outline Width Mask", 2D) = "white" {}
        [Toggle(_OUTLINE_WIDTH_MASK)] _UseOutlineWidthMask ("Use Width Mask", Float) = 0
        _OutlineFixWidth ("Fix Width (Distance Scale)", Range(0,1)) = 0.8
        [Enum(None,0,Red Channel,1,Alpha Channel,2)]_OutlineVertexR2Width ("Vertex Color Usage", Int) = 0
        _OutlineZBias ("Z Bias", Range(-1,1)) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _OutlineCull ("Outline Cull", Float) = 1
        
        [Header(Other)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2
        [HideInInspector] _SrcBlend ("Src Blend", Float) = 1
        [HideInInspector] _DstBlend ("Dst Blend", Float) = 0
        [HideInInspector] _ZWrite ("Z Write", Float) = 1
        [HideInInspector] _Surface ("Surface", Float) = 0
        
        [Header(Stencil)]
        [IntRange] _StencilRef ("Stencil Reference", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Compare", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilPass ("Stencil Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFail ("Stencil Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail ("Stencil ZFail", Float) = 0
        [IntRange] _StencilWriteMask ("Stencil Write Mask", Range(0, 255)) = 255
        [IntRange] _StencilReadMask ("Stencil Read Mask", Range(0, 255)) = 255
        
        [Header(Render Queue)]
        [IntRange] _RenderQueue ("Render Queue", Range(-1, 5000)) = -1
    }

    SubShader
    {
        Tags{ "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }

        HLSLINCLUDE
        #pragma target 3.0
        #pragma multi_compile _ _RIM

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            float4 _ShadowColor;
            float4 _RimColor;
            float4 _OutlineColor;
            float  _RimPower;
            float  _RimIntensity;
            float  _ShadowThreshold;
            float  _ShadowSmoothness;
            float  _ShadingStrength;
            float  _AmbientStrength;
            float  _OutlineWidth;
            float  _OutlineFixWidth;
            float  _OutlineZBias;
            int    _OutlineVertexR2Width;
            float  _Cutoff;
            float  _BlendMode;
            float  _Cull;
            float  _OutlineCull;
            int    _StencilRef;
            float  _StencilComp;
            float  _StencilPass;
            float  _StencilFail;
            float  _StencilZFail;
            int    _StencilWriteMask;
            int    _StencilReadMask;
            int    _RenderQueue;
        CBUFFER_END

        TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
        TEXTURE2D(_OutlineWidthMask); SAMPLER(sampler_OutlineWidthMask);

        // Helper functions for outline calculation (lilToon-inspired)
        float GetOutlineWidth(float2 uv, float4 color, float outlineWidth)
        {
            outlineWidth *= 0.01; // Proper lilToon scaling
            #ifdef _OUTLINE_WIDTH_MASK
                outlineWidth *= SAMPLE_TEXTURE2D_LOD(_OutlineWidthMask, sampler_OutlineWidthMask, uv, 0).r;
            #endif
            // Vertex color modulation
            if(_OutlineVertexR2Width == 1) outlineWidth *= color.r;
            if(_OutlineVertexR2Width == 2) outlineWidth *= color.a;
            return outlineWidth;
        }

        float GetDistanceScale(float3 positionWS)
        {
            // lilToon-style distance calculation for consistent outline thickness
            float3 headDirection = _WorldSpaceCameraPos - positionWS;
            float distance = length(headDirection);
            return lerp(1.0, saturate(distance * 0.1), _OutlineFixWidth);
        }

        struct Attributes {
            float4 positionOS : POSITION;
            float3 normalOS   : NORMAL;
            float2 uv         : TEXCOORD0;
            float4 color      : COLOR;
        };

        struct Varyings {
            float4 positionCS : SV_POSITION;
            float3 positionWS : TEXCOORD0;
            float3 normalWS   : TEXCOORD1;
            float2 uv         : TEXCOORD2;
            float3 viewDirWS  : TEXCOORD3;
        };

        Varyings ToonVert(Attributes IN)
        {
            Varyings OUT;
            float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
            float3 normalWS   = TransformObjectToWorldNormal(IN.normalOS);
            OUT.positionCS = TransformWorldToHClip(positionWS);
            OUT.positionWS = positionWS;
            OUT.normalWS   = normalWS;
            OUT.uv         = IN.uv;
            OUT.viewDirWS  = GetWorldSpaceViewDir(positionWS);
            return OUT;
        }

        float3 ShadeToon(float3 baseRGB, float3 N, float3 V, float3 positionWS)
        {
            float3 col = baseRGB;
            N = normalize(N);
            V = normalize(V);

            // Start with ambient light
            float3 ambientLight = SampleSH(N) * _AmbientStrength;
            col = baseRGB * ambientLight;

            // Main light two-tone shading
            float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
            Light mainLight = GetMainLight(shadowCoord);
            float NdotL = saturate(dot(N, mainLight.direction));
            float litTerm = NdotL * mainLight.shadowAttenuation;
            
            // Two-tone threshold with smoothstep for anti-aliasing
            float toonShade = smoothstep(_ShadowThreshold - _ShadowSmoothness, 
                                       _ShadowThreshold + _ShadowSmoothness, litTerm);
            
            // Apply shadow color with proper energy conservation
            float3 lightColor = lerp(_ShadowColor.rgb * baseRGB, baseRGB, toonShade);
            // Blend between flat lighting and two-tone shading based on shading strength
            lightColor = lerp(baseRGB, lightColor, _ShadingStrength);
            col += lightColor * mainLight.color;

            // Additional lights with two-tone shading
            uint count = GetAdditionalLightsCount();
            LIGHT_LOOP_BEGIN(count)
                Light l = GetAdditionalLight(lightIndex, positionWS);
                float nl = saturate(dot(N, l.direction));
                float additionalToon = smoothstep(_ShadowThreshold - _ShadowSmoothness, 
                                                _ShadowThreshold + _ShadowSmoothness, nl);
                float3 additionalLight = lerp(_ShadowColor.rgb * baseRGB, baseRGB, additionalToon);
                // Blend between flat lighting and two-tone shading for additional lights
                additionalLight = lerp(baseRGB, additionalLight, _ShadingStrength);
                col += additionalLight * l.color * l.distanceAttenuation * l.shadowAttenuation;
            LIGHT_LOOP_END

            #if _RIM
            // Rim lighting (Fresnel-based)
            float fresnel = 1.0 - saturate(dot(N, V));
            float rim = pow(fresnel, _RimPower) * _RimIntensity;
            col += _RimColor.rgb * rim;
            #endif
            
            return col;
        }
        ENDHLSL

        // ---------- Outline ----------
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "Outline" }
            Cull [_OutlineCull]
            ZWrite [_ZWrite]
            ZTest LEqual
            Blend [_SrcBlend] [_DstBlend]
            
            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
                WriteMask [_StencilWriteMask]
                ReadMask [_StencilReadMask]
            }

            HLSLPROGRAM
            #pragma vertex OutlineVert
            #pragma fragment OutlineFrag
            #pragma multi_compile _ _OUTLINE
            #pragma multi_compile _ _OUTLINE_WIDTH_MASK
            #pragma shader_feature_local _ALPHATEST_ON

            struct OutlineAttributes { 
                float4 positionOS : POSITION; 
                float3 normalOS   : NORMAL; 
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };
            struct OutlineVaryings { 
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            OutlineVaryings OutlineVert(OutlineAttributes IN)
            {
                OutlineVaryings OUT;
                
                #ifndef _OUTLINE
                    // If outline is disabled, return an invalid position to cull the vertex
                    OUT.positionCS = float4(0, 0, 0, 0);
                    OUT.uv = IN.uv;
                    return OUT;
                #endif
                
                // Transform to world space
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                normalWS = normalize(normalWS);
                
                // Calculate outline width with all modulations
                float width = GetOutlineWidth(IN.uv, IN.color, _OutlineWidth);
                width *= GetDistanceScale(positionWS);
                
                // Apply outline expansion in world space
                positionWS += normalWS * width;
                
                // Apply Z-bias if needed (for depth fighting prevention)
                if(_OutlineZBias != 0)
                {
                    float3 viewDirWS = normalize(_WorldSpaceCameraPos - positionWS);
                    positionWS -= viewDirWS * _OutlineZBias * 0.001;
                }
                
                OUT.positionCS = TransformWorldToHClip(positionWS);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 OutlineFrag(OutlineVaryings IN) : SV_Target
            {
                #ifndef _OUTLINE
                    // If outline is disabled, discard the fragment
                    discard;
                #endif
                
                float4 outlineColor = _OutlineColor;
                
                // Sample base texture for alpha in transparency modes
                float alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv).a * _BaseColor.a;
                
                // #ifdef _ALPHATEST_ON
                //     clip(alpha - _Cutoff);
                // #endif
                
                #ifdef _OUTLINE_WIDTH_MASK
                    // Optional: modulate outline color with width mask
                    float mask = SAMPLE_TEXTURE2D(_OutlineWidthMask, sampler_OutlineWidthMask, IN.uv).r;
                    outlineColor.rgb *= mask;
                #endif
                
                outlineColor.a *= alpha;
                return outlineColor;
            }
            ENDHLSL
        }

        // ---------- Forward Lit ----------
        Pass
        {
            Name "ForwardLit"
            Tags{ "LightMode"="UniversalForward" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            Cull [_Cull]
            
            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
                WriteMask [_StencilWriteMask]
                ReadMask [_StencilReadMask]
            }

            HLSLPROGRAM
            #pragma vertex ToonVert
            #pragma fragment ToonFrag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local _ALPHAPREMULTIPLY_ON

            float4 ToonFrag(Varyings IN) : SV_Target
            {
                float4 baseSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                
                // // Alpha cutoff for cutout mode
                // #ifdef _ALPHATEST_ON
                //     clip(baseSample.a - _Cutoff);
                // #endif

                float3 col = ShadeToon(baseSample.rgb, IN.normalWS, IN.viewDirWS, IN.positionWS);
                col = MixFog(col, ComputeFogFactor(IN.positionCS.z));
                
                // Handle alpha premultiply for transparency
                #ifdef _ALPHAPREMULTIPLY_ON
                    col *= baseSample.a;
                #endif
                
                return float4(col, baseSample.a);
            }
            ENDHLSL
        }

        // ---------- ShadowCaster (URP only) ----------
        Pass
        {
            Name "ShadowCaster"
            Tags{ "LightMode"="ShadowCaster" }
            ZWrite On
            ZTest LEqual
            Cull [_Cull]
            
            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
                WriteMask [_StencilWriteMask]
                ReadMask [_StencilReadMask]
            }
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            #pragma shader_feature_local _ALPHATEST_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            // Shadow casting variables
            float3 _LightDirection;
            float3 _LightPosition;

            struct ShadowAttributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 texcoord     : TEXCOORD0;
            };

            struct ShadowVaryings
            {
                float2 uv           : TEXCOORD0;
                float4 positionCS   : SV_POSITION;
            };

            float4 GetShadowPositionHClip(ShadowAttributes input)
            {
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                // Get main light for shadow bias
                Light mainLight = GetMainLight();
                float3 lightDirectionWS = mainLight.direction;

            #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                // For punctual lights, use the light position if available
                if (length(_LightPosition) > 0.001)
                {
                    lightDirectionWS = normalize(_LightPosition - positionWS);
                }
            #endif

                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

            #if UNITY_REVERSED_Z
                positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
            #else
                positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
            #endif

                return positionCS;
            }

            ShadowVaryings ShadowPassVertex(ShadowAttributes input)
            {
                ShadowVaryings output;
                output.uv = input.texcoord;
                output.positionCS = GetShadowPositionHClip(input);
                return output;
            }

            half4 ShadowPassFragment(ShadowVaryings input) : SV_TARGET
            {
                // #ifdef _ALPHATEST_ON
                //     float alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a * _BaseColor.a;
                //     clip(alpha - _Cutoff);
                // #endif
                return 0;
            }
            ENDHLSL
        }

        // ---------- DepthOnly (URP only) ----------
        Pass
        {
            Name "DepthOnly"
            Tags{ "LightMode"="DepthOnly" }
            ZWrite On
            ColorMask 0
            Cull [_Cull]
            
            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
                WriteMask [_StencilWriteMask]
                ReadMask [_StencilReadMask]
            }
            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            struct DepthOnlyAttributes
            {
                float4 position     : POSITION;
                float2 texcoord     : TEXCOORD0;
            };

            struct DepthOnlyVaryings
            {
                float2 uv           : TEXCOORD0;
                float4 positionCS   : SV_POSITION;
            };

            DepthOnlyVaryings DepthOnlyVertex(DepthOnlyAttributes input)
            {
                DepthOnlyVaryings output = (DepthOnlyVaryings)0;
                output.uv = input.texcoord;
                output.positionCS = TransformObjectToHClip(input.position.xyz);
                return output;
            }

            half4 DepthOnlyFragment(DepthOnlyVaryings input) : SV_TARGET
            {
                // // Always test alpha for stencil operations
                // float alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a * _BaseColor.a;
                // clip(alpha - _Cutoff);
                return 0;
            }
            ENDHLSL
        }
    }

    CustomEditor "CoolToonEditor"
    Fallback Off
}
