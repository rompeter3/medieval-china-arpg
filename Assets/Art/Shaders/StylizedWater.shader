Shader "MedievalChinaARPG/StylizedWater"
{
    Properties
    {
        _ShallowColor ("Shallow", Color) = (0.25, 0.55, 0.55, 0.85)
        _DeepColor ("Deep", Color) = (0.08, 0.22, 0.35, 0.95)
        _FoamColor ("Foam", Color) = (0.85, 0.95, 0.95, 1)
        _WaveStrength ("Wave Strength", Range(0, 0.3)) = 0.06
        _WaveSpeed ("Wave Speed", Range(0.1, 3)) = 1.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _ShallowColor;
                float4 _DeepColor;
                float4 _FoamColor;
                float _WaveStrength;
                float _WaveSpeed;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float fogFactor : TEXCOORD2;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 pos = input.positionOS.xyz;
                float wave = sin(_Time.y * _WaveSpeed + pos.x * 2.0 + pos.z * 1.5) * _WaveStrength;
                pos.y += wave;
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
                Light mainLight = GetMainLight();

                float fresnel = pow(1.0 - saturate(dot(n, v)), 2.2);
                float3 col = lerp(_DeepColor.rgb, _ShallowColor.rgb, fresnel);
                float foam = saturate(sin(input.positionWS.x * 4.0 + _Time.y) * sin(input.positionWS.z * 3.5 + _Time.y * 0.7));
                foam = step(0.82, foam);
                col = lerp(col, _FoamColor.rgb, foam * 0.55);
                col *= mainLight.color * (0.65 + 0.35 * saturate(dot(n, mainLight.direction)));
                col = MixFog(col, input.fogFactor);
                float alpha = lerp(_DeepColor.a, _ShallowColor.a, fresnel);
                return half4(col, alpha);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Unlit"
}
