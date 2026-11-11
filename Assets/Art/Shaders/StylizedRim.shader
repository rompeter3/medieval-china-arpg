Shader "MedievalChinaARPG/StylizedRim"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.7, 0.55, 0.4, 1)
        _RimColor ("Rim Color", Color) = (0.85, 0.9, 1, 1)
        _RimPower ("Rim Power", Range(0.5, 8)) = 3.2
        _ShadowSoftness ("Shadow Softness", Range(0.01, 1)) = 0.35
        _LeafSway ("Leaf Sway", Range(0, 1)) = 0
        _SwayStrength ("Sway Strength", Range(0, 0.2)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _RimColor;
                float _RimPower;
                float _ShadowSoftness;
                float _LeafSway;
                float _SwayStrength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 pos = input.positionOS.xyz;
                if (_LeafSway > 0.01)
                {
                    float sway = sin(_Time.y * 1.7 + pos.x * 2.5 + pos.z) * _SwayStrength * _LeafSway;
                    pos.x += sway;
                    pos.z += sway * 0.5;
                }
                VertexPositionInputs posInputs = GetVertexPositionInputs(pos);
                VertexNormalInputs nInputs = GetVertexNormalInputs(input.normalOS);
                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS = nInputs.normalWS;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 n = normalize(input.normalWS);
                float3 v = GetWorldSpaceNormalizeViewDir(input.positionWS);
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));

                float ndotl = saturate(dot(n, mainLight.direction));
                float soft = smoothstep(0.0, _ShadowSoftness, ndotl);
                float shadow = mainLight.shadowAttenuation;
                float lighting = lerp(0.35, 1.0, soft * shadow);

                float rim = pow(1.0 - saturate(dot(n, v)), _RimPower);
                float3 color = _BaseColor.rgb * mainLight.color * lighting + _RimColor.rgb * rim * 0.55;
                return half4(color, _BaseColor.a);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
            ZWrite On
            ZTest LEqual
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
    FallBack Off
}
