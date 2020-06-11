Shader "Glitch/Jitter"
{
    Properties
    {
        _MainTex ("-", 2D) = "" {}
    }
    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float2 _MainTex_TexelSize;

    float2 _Jitter; 

    float nrand(float x, float y)
    {
        return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
    }

    half4 frag(v2f_img i) : SV_Target
    {
        float u = i.uv.x;
        float v = i.uv.y;

        // Scan line jitter
        float jitterV = nrand(v, _Time.x) * 2 - 1;
        jitterV *= step(_Jitter.y, abs(jitterV)) * _Jitter.x;  
        
        float jitterU = nrand(u, _Time.x) * 2 - 1;
        jitterU *= step(_Jitter.y, abs(jitterU)) * _Jitter.x; 

        half4 src1 = tex2D(_MainTex, float2(u + jitterV, v + jitterU ));        

        return half4(src1.r, src1.g, src1.b, 1);
    }

    ENDCG
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma target 3.0
            ENDCG
        }
    }
}
