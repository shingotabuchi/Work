Shader "Custom/CoolToon"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _BaseMap ("Base Map", 2D) = "white" {}
        _ShadowColor ("Shadow Color", Color) = (0.5,0.5,0.5,1)
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _ShadowSmoothness ("Shadow Smoothness", Range(0,0.1)) = 0.01
        _ShadingStrength ("Shading Strength", Range(0,1)) = 1
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.1,16)) = 4
        _RimIntensity ("Rim Intensity", Range(0,2)) = 0.5
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width (screen)", Range(0,5)) = 1.2
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags{ "RenderType"="Opaque" "Queue"="Geometry" }

        HLSLINCLUDE
        #pragma target 3.0

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
            float  _OutlineWidth;
            float  _Cutoff;
        CBUFFER_END

        TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);

        struct Attributes {
            float4 positionOS : POSITION;
            float3 normalOS   : NORMAL;
            float2 uv         : TEXCOORD0;
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

            // Main light two-tone shading
            float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
            Light mainLight = GetMainLight(shadowCoord);
            float NdotL = saturate(dot(normalize(N), mainLight.direction));
            float litTerm = NdotL * mainLight.shadowAttenuation;
            
            // Two-tone threshold with smoothstep for anti-aliasing
            float toonShade = smoothstep(_ShadowThreshold - _ShadowSmoothness, _ShadowThreshold + _ShadowSmoothness, litTerm);
            float3 lightColor = lerp(_ShadowColor.rgb, baseRGB, toonShade);
            // Blend between flat lighting and two-tone shading based on shading strength
            lightColor = lerp(baseRGB, lightColor, _ShadingStrength);
            col = lightColor * mainLight.color;

            // Additional lights with two-tone shading
            uint count = GetAdditionalLightsCount();
            [loop] for (uint i = 0; i < count; i++) {
                Light l = GetAdditionalLight(i, positionWS);
                float nl = saturate(dot(normalize(N), l.direction));
                float additionalToon = smoothstep(_ShadowThreshold - _ShadowSmoothness, _ShadowThreshold + _ShadowSmoothness, nl);
                float3 additionalLight = lerp(_ShadowColor.rgb, baseRGB, additionalToon);
                // Blend between flat lighting and two-tone shading for additional lights
                additionalLight = lerp(baseRGB, additionalLight, _ShadingStrength);
                col += additionalLight * l.color * l.distanceAttenuation * l.shadowAttenuation;
            }

            // Rim lighting
            float rim = pow(saturate(1.0 - dot(normalize(N), normalize(V))), _RimPower) * _RimIntensity;
            col += _RimColor.rgb * rim;
            return col;
        }
        ENDHLSL

        // ---------- Forward Lit ----------
        Pass
        {
            Name "ForwardLit"
            Tags{ "LightMode"="UniversalForward" }

            Blend One Zero
            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma vertex ToonVert
            #pragma fragment ToonFrag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            float4 ToonFrag(Varyings IN) : SV_Target
            {
                float4 baseSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                // if (baseSample.a < _Cutoff) discard;

                float3 col = ShadeToon(baseSample.rgb, IN.normalWS, IN.viewDirWS, IN.positionWS);
                col = MixFog(col, IN.positionCS.z);
                return float4(col, 1);
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
            Cull Back
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW

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
                return 0;
            }
            ENDHLSL
        }

        // ---------- Outline ----------
        Pass
        {
            Name "Outline"
            Tags{ "LightMode"="UniversalForward" }
            Cull Front
            ZWrite On

            HLSLPROGRAM
            #pragma vertex OutlineVert
            #pragma fragment OutlineFrag

            struct OutlineAttributes { float4 positionOS:POSITION; float3 normalOS:NORMAL; };
            struct OutlineVaryings { float4 positionCS:SV_POSITION; };

            OutlineVaryings OutlineVert(OutlineAttributes IN)
            {
                OutlineVaryings OUT;
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS   = TransformObjectToWorldNormal(IN.normalOS);
                float4 posVS = mul(UNITY_MATRIX_V, float4(positionWS, 1.0));
                float3 nVS = normalize(mul((float3x3)UNITY_MATRIX_V, normalWS));

                float width = _OutlineWidth * 0.001;
                posVS.xyz += nVS * width * -posVS.z;
                OUT.positionCS = mul(UNITY_MATRIX_P, posVS);
                return OUT;
            }

            float4 OutlineFrag() : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }

    Fallback Off
}
