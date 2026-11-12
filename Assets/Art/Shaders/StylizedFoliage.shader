Shader "MedievalChinaARPG/StylizedFoliage"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.28, 0.5, 0.26, 1)
        _TipColor ("Tip Color", Color) = (0.55, 0.75, 0.35, 1)
        _RimColor ("Rim Color", Color) = (0.75, 0.95, 0.55, 1)
        _SwayStrength ("Sway Strength", Range(0, 0.4)) = 0.12
        _SwaySpeed ("Sway Speed", Range(0.2, 5)) = 2.1
        _Translucency ("Translucency", Range(0, 1)) = 0.45
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _TipColor;
                float4 _RimColor;
                float _SwayStrength;
                float _SwaySpeed;
                float _Translucency;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float height : TEXCOORD2;
                float fogFactor : TEXCOORD3;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 pos = input.positionOS.xyz;
                float h = saturate(pos.y * 0.5 + 0.5);
                float phase = pos.x * 2.7 + pos.z * 1.9;
                float sway = sin(_Time.y * _SwaySpeed + phase) * _SwayStrength * h;
                float sway2 = cos(_Time.y * (_SwaySpeed * 0.83) + phase * 1.3) * _SwayStrength * 0.6 * h;
                pos.x += sway;
                pos.z += sway2;

                VertexPositionInputs posInputs = GetVertexPositionInputs(pos);
                VertexNormalInputs nInputs = GetVertexNormalInputs(input.normalOS);
                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS = nInputs.normalWS;
                output.height = h;
                output.fogFactor = ComputeFogFactor(posInputs.positionCS.z);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 n = normalize(input.normalWS);
                float3 v = GetWorldSpaceNormalizeViewDir(input.positionWS);
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));

                float wrap = saturate(dot(n, mainLight.direction) * 0.5 + 0.5);
                float bands = floor(wrap * 3.0) / 3.0;
                float3 albedo = lerp(_BaseColor.rgb, _TipColor.rgb, input.height);
                float3 lit = albedo * mainLight.color * lerp(0.35, 1.0, bands * mainLight.shadowAttenuation);

                float back = saturate(dot(-n, mainLight.direction));
                lit += albedo * mainLight.color * back * _Translucency;

                float rim = pow(1.0 - saturate(dot(n, v)), 2.4);
                lit += _RimColor.rgb * rim * 0.35;

                lit = MixFog(lit, input.fogFactor);
                return half4(lit, 1);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
