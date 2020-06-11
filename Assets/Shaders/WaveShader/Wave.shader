Shader "Hidden/Custom/Wave"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float _Divisor;
        float _Speed;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float2 uv = i.texcoord;
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, sin(i.vertex.x / 100 + _Time[1] * _Speed)/_Divisor));            
            return color;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}