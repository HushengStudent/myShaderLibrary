Shader "myShaderLibrary/PostProcess/AllInOnePostProcess"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}

        _GrayScaleLuminosity ("Gray Scale Luminosity", Range(0,1)) = 0.2

        _BlurStrength ("Blur Strength", Range(0,10)) = 3

        _ScanLineJitter("Scan Line Jitter", Range(0,1)) = 0.2
        _VerticalJumpRange("Vertical Jump Range", Range(0,1)) = 0.02
        _VerticalJumpSpeed("Vertical Jump Speed", Range(-10,10)) = -5
        _HorizontalShake("Horizontal Shake", Range(0,1)) = 0.02
        _ColorDrift("Color Drift", Range(0,1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {

            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Assets/Shaders/CGIncludes/Common.cginc"

            //置灰
            #pragma shader_feature GREYSCALE_ON
            //模糊
            #pragma shader_feature BLUR_ON
            //故障
            #pragma shader_feature Glitch_ON

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

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            #ifdef GREYSCALE_ON
            float _GrayScaleLuminosity;
            #endif

            #ifdef BLUR_ON
            float _BlurStrength;
            #endif

            #ifdef Glitch_ON
            float _ScanLineJitter;
            float _VerticalJumpRange;
            float _VerticalJumpSpeed;
            float _HorizontalShake;
            float _ColorDrift;
            #endif

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.uv = ComputeScreenPos(o.vertex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                
                #if UNITY_UV_STARTS_AT_TOP
                o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
                #endif

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);

                #ifdef Glitch_ON
                float u = i.uv.x;
                float v = i.uv.y;

                float jitter_thresh = saturate(1.0f - _ScanLineJitter * 1.2f);
                float jitter_disp = 0.002f + pow(_ScanLineJitter, 3) * 0.05f;
                float jitter = rand(fixed2(v, _Time.x)) * 2 - 1;
                jitter *= step(jitter_thresh, abs(jitter)) * jitter_disp;

                float jump = lerp(v, frac(v + _Time.x * _VerticalJumpSpeed), _VerticalJumpRange);
                float shake = (rand(fixed2(_Time.x, 2)) - 0.5) * _HorizontalShake* 0.2f;
                float drift = sin(jump + _Time.y* 606.11f) * _ColorDrift* 0.04f;

                half4 src1 = tex2D(_MainTex, frac(float2(u + jitter + shake, jump)));
                half4 src2 = tex2D(_MainTex, frac(float2(u + jitter + shake + drift, jump)));
                col = half4(src1.r, src2.g, src1.b, col.a);
                #endif

                #ifdef BLUR_ON
                col = Blur(i.uv, _MainTex, _BlurStrength);
                #endif

                #ifdef GREYSCALE_ON
                col.rgb = saturate((col.r + col.g + col.b) * _GrayScaleLuminosity);
                #endif

                return col;
            }

            ENDCG
        }
    }
    CustomEditor "AllInOnePostProcessShaderGUI"
}
