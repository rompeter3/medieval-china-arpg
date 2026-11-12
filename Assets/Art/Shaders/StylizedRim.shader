Shader "MedievalChinaARPG/StylizedRim"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.7, 0.55, 0.4, 1)
        _ShadeColor ("Shade Color", Color) = (0.35, 0.28, 0.32, 1)
        _RimColor ("Rim Color", Color) = (0.95, 0.85, 0.65, 1)
        _RimPower ("Rim Power", Range(0.5, 8)) = 2.6
        _RimIntensity ("Rim Intensity", Range(0, 2)) = 0.85
        _Bands ("Color Bands", Range(1, 6)) = 3
        _ShadowSoftness ("Shadow Softness", Range(0.01, 1)) = 0.22
        _Specular ("Specular", Range(0, 1)) = 0.15
        _LeafSway ("Leaf Sway", Range(0, 1)) = 0
        _SwayStrength ("Sway Strength", Range(0, 0.35)) = 0.08
        _SwaySpeed ("Sway Speed", Range(0.2, 4)) = 1.8
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        LOD 250
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _ShadeColor;
                float4 _RimColor;
                float _RimPower;
                float _RimIntensity;
                float _Bands;
                float _ShadowSoftness;
                float _Specular;
                float _LeafSway;
                float _SwayStrength;
                float _SwaySpeed;
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
                float fogFactor : TEXCOORD2;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 pos = input.positionOS.xyz;
                if (_LeafSway > 0.01)
                {
                    float h = saturate(pos.y);
                    float sway = sin(_Time.y * _SwaySpeed + pos.x * 3.1 + pos.z * 2.2) * _SwayStrength * _LeafSway * h;
                    float sway2 = cos(_Time.y * (_SwaySpeed * 0.7) + pos.z * 2.8) * _SwayStrength * 0.55 * _LeafSway * h;
                    pos.x += sway;
                    pos.z += sway2;
                }

                VertexPositionInputs posInputs = GetVertexPositionInputs(pos);
                VertexNormalInputs nInputs = GetVertexNormalInputs(input.normalOS);
                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS = nInputs.normalWS;
                output.fogFactor = ComputeFogFactor(posInputs.positionCS.z);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 n = normalize(input.normalWS);
                float3 v = GetWorldSpaceNormalizeViewDir(input.positionWS);
                // Avoid shadow-coord path — keeps this pass robust on Metal.
                Light mainLight = GetMainLight();

                float ndotl = saturate(dot(n, mainLight.direction));
                float soft = smoothstep(0.0, _ShadowSoftness, ndotl);
                float bands = max(1.0, _Bands);
                float stepped = floor(soft * bands + 1e-4) / bands;
                float lighting = saturate(stepped);

                float3 lit = lerp(_ShadeColor.rgb, _BaseColor.rgb, lighting) * mainLight.color;
                float3 halfDir = normalize(mainLight.direction + v);
                float spec = pow(saturate(dot(n, halfDir)), 48.0) * _Specular * lighting;
                lit += spec * mainLight.color;

                float rim = pow(1.0 - saturate(dot(n, v)), _RimPower) * _RimIntensity;
                lit += _RimColor.rgb * rim * (0.45 + 0.55 * lighting);

                lit = MixFog(lit, input.fogFactor);
                return half4(lit, _BaseColor.a);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
