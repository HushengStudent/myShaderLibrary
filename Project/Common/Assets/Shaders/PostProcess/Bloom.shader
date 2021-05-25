Shader "myShaderLibrary/PostProcess/Bloom"
{
    Properties
    {
        _BloomLimit ("Bloom Limit", Range(0,1)) = 0.1
        _BloomStrength ("Bloom Strength", Range(0,10)) = 5
    }

    CGINCLUDE
        #include "UnityCG.cginc"
        #include "Assets/Shaders/CGIncludes/Common.cginc"

        #pragma vertex vert

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            //UNITY_FOG_COORDS(1)
            float4 vertex : SV_POSITION;
        };

        sampler2D _MainTex,_BlomTex;
        float4 _MainTex_TexelSize;
        float _BloomLimit;
        float _BloomStrength;

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = float4(v.vertex.xy, 0.0, 1.0);
            o.uv = TransformTriangleVertexToUV(v.vertex.xy);
            o.uv = UVStartAtTop(o.uv);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            return tex2D(_MainTex, i.uv);
        }

        fixed4 Comb (v2f i) : SV_Target
        {
            return tex2D(_MainTex, i.uv) + tex2D(_BlomTex, i.uv);
        }

        fixed4 ExtractBright (v2f i) : SV_Target
        {
            fixed4 col = tex2D(_MainTex, i.uv);
            fixed bright = max(col.r, max(col.g, col.b)); 
            fixed val = clamp(bright - _BloomLimit, 0, 1);
            return fixed4(col.r * val,col.g * val,col.b * val, 1);
        }

        fixed4 Bloom (v2f i) : SV_Target
        {
            return Blur(i.uv, _MainTex, _BloomStrength);
        }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma fragment frag
            ENDCG
        }

        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma fragment ExtractBright
            ENDCG
        }

        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma fragment Bloom
            ENDCG
        }

        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma fragment Comb
            ENDCG
        }
    }
}