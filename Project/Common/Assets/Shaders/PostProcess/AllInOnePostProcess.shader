﻿Shader "myShaderLibrary/PostProcess/AllInOnePostProcess"
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

        _GlowColor("Glow Color", Color) = (1,1,1,1)
		_GlowIntensity("Glow Intensity", Range(0,100)) = 10

/*
According to the reference, Shader.setGlobal functions would only work on variables 
that are not exposed as a property in the property block. So what you have to do is 
remove the _test("_test",Float) = 0.0 line to make it work. Be sure to remake a new 
material, since unity will save the properties you have set on a material even when 
you are using a shader that doesn't have that property.
*/
        _MeltNoiseTex("Melt Noise Texture", 2D) = "white" {}
        //_MeltStrength ("Melt Strength", Range(0,1)) = 0
        _MeltAddColor ("Melt Add Color", Color) = (1,1,1,1)
        _MeltAddColorStrength ("Melt Add Color Strength", Range(1,20)) = 1
        _MeltAddColorLength ("Melt Add Color Length", Range(0,1)) = 0
        _MeltAdditionalTex("Melt Additional Texture", 2D) = "white" {}

        _PixelateIntensity("Pixelate Intensity", Range(1,200)) = 10

        _AberrationColor("Aberration Color", Color) = (1,1,1,1)
        _AberrationAmount("Aberration Amount", Range(0, 1)) = 1
		_AberrationAlpha("Aberration Alpha", Range(0, 1)) = 0.4

        _DistortNoiseTex ("Distort Noise Tex", 2D) = "white" {}
        _DistortSpeedX ("Distort SpeedX", Range(1,10)) = 1
        _DistortSpeedY ("Distort SpeedY", Range(1,10)) = 1
        _DistortStrength ("Distort Strength", Range(0,1)) = 1
    }

    CGINCLUDE
        // make fog work
        //#pragma multi_compile_fog

        #include "UnityCG.cginc"
        #include "Assets/Shaders/CGIncludes/Common.cginc"

        //置灰
        #pragma shader_feature GREYSCALE_ON
        //模糊
        #pragma shader_feature BLUR_ON
        //故障
        #pragma shader_feature GLITCH_ON
        //发光
        #pragma shader_feature GLOW_ON
        //消融
        #pragma shader_feature MELT_ON
        #pragma shader_feature MELT_TEX_ON
        //负片
        #pragma shader_feature NEGATIVE_ON
        //像素
        #pragma shader_feature PIXELATE_ON
        //色差
        #pragma shader_feature ABERRATION_ON
        //变形
        #pragma shader_feature DISTORT_ON

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
            #ifdef MELT_ON
            float2 meltNoiseUV  : TEXCOORD1;
            float2 additionalUV  : TEXCOORD2;
            #endif

            #ifdef DISTORT_ON
            float2 distortNoiseUV  : TEXCOORD1;
            #endif
        };

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;

        #ifdef GREYSCALE_ON
        float _GrayScaleLuminosity;
        #endif

        #ifdef BLUR_ON
        float _BlurStrength;
        #endif

        #ifdef GLITCH_ON
        float _ScanLineJitter;
        float _VerticalJumpRange;
        float _VerticalJumpSpeed;
        float _HorizontalShake;
        float _ColorDrift;
        #endif

        #ifdef GLOW_ON
        float4 _GlowColor;
        float _GlowIntensity;
        #endif

        #ifdef MELT_ON
        sampler2D _MeltNoiseTex;
        float4 _MeltNoiseTex_TexelSize;
        float _MeltStrength;
        fixed4 _MeltAddColor;
        float _MeltAddColorStrength;
        float _MeltAddColorLength;
        sampler2D _MeltAdditionalTex;
        float4 _MeltAdditionalTex_TexelSize;;
        #endif

        #ifdef PIXELATE_ON
        float _PixelateIntensity;
        #endif

        #if ABERRATION_ON
        float4 _AberrationColor;
        fixed _AberrationAmount, _AberrationAlpha;
        #endif

        #if DISTORT_ON
        sampler2D _DistortNoiseTex;
        float4 _DistortNoiseTex_TexelSize;
        fixed _DistortSpeedX, _DistortSpeedY,_DistortStrength;
        #endif

        v2f vert (appdata v)
        {
            v2f o;
            /*
            o.vertex = v.vertex;
            o.uv = ComputeScreenPos(o.vertex);
            //UNITY_TRANSFER_FOG(o,o.vertex);
                
            o.uv = UVStartAtTop(o.uv);
                
            #ifdef MELT_ON
            o.meltNoiseUV = ComputeScreenPos(o.vertex);
            o.additionalUV = ComputeScreenPos(o.vertex);

            o.meltNoiseUV = UVStartAtTop(o.meltNoiseUV);
            o.additionalUV = UVStartAtTop(o.additionalUV);
            #endif
                
            #ifdef DISTORT_ON
            o.distortNoiseUV = ComputeScreenPos(o.vertex);
            o.distortNoiseUV = UVStartAtTop(o.distortNoiseUV);
            #endif
            */

            o.vertex = float4(v.vertex.xy, 0.0, 1.0);
            o.uv = TransformTriangleVertexToUV(v.vertex.xy);
            //UNITY_TRANSFER_FOG(o,o.vertex);
                
            o.uv = UVStartAtTop(o.uv);
                
            #ifdef MELT_ON
            o.meltNoiseUV = TransformTriangleVertexToUV(v.vertex.xy);
            o.additionalUV = TransformTriangleVertexToUV(v.vertex.xy);

            o.meltNoiseUV = UVStartAtTop(o.meltNoiseUV);
            o.additionalUV = UVStartAtTop(o.additionalUV);
            #endif
                
            #ifdef DISTORT_ON
            o.distortNoiseUV = TransformTriangleVertexToUV(v.vertex.xy);
            o.distortNoiseUV = UVStartAtTop(o.distortNoiseUV);
            #endif

            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            #ifdef PIXELATE_ON
            i.uv = floor(i.uv * _PixelateIntensity) / _PixelateIntensity;
            #endif
                
            #ifdef DISTORT_ON
            i.distortNoiseUV.x += (_Time * _DistortSpeedX) % 1;
            i.distortNoiseUV.y += (_Time * _DistortSpeedY) % 1;
            fixed distortNoise = tex2D(_DistortNoiseTex, i.distortNoiseUV).a * _DistortStrength;
            fixed distortOffset = distortNoise - (_DistortStrength/2);          
            i.uv.x += distortOffset;
            i.uv.y += distortOffset;
            #endif

            // sample the texture
            fixed4 col = tex2D(_MainTex, i.uv);
            // apply fog
            //UNITY_APPLY_FOG(i.fogCoord, col);

            #ifdef GLITCH_ON
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
            col.a *= step(0.1, src1.a)*step(0.1, src2.a);
            #endif
                
            #ifdef GLOW_ON
            col.rgb += col.a * _GlowIntensity * _GlowColor;
            #endif

            #ifdef BLUR_ON
            col = Blur(i.uv, _MainTex, _BlurStrength);
            #endif

            #ifdef GREYSCALE_ON
            col.rgb = saturate((col.r + col.g + col.b) * _GrayScaleLuminosity);
            #endif

            #ifdef MELT_ON
            float melt = tex2D(_MeltNoiseTex, i.meltNoiseUV).a;
            float value = melt - _MeltStrength;
            float clip = step(0.01, value);
            col.rgb *= clip;
            float length = saturate(value + _MeltAddColorLength);
            col.a *= step(0.01, length);
            float degrees = saturate(_MeltAddColorStrength * length * (1 -clip));
            half4 additional = tex2D(_MeltAdditionalTex, i.additionalUV);
            #ifdef MELT_TEX_ON
            col.rgb = lerp(col.rgb, additional , degrees);
            #else
            col.rgb = lerp(col.rgb, _MeltAddColor , degrees);
            #endif
            #endif

            #ifdef NEGATIVE_ON
            col.rgb = 1 - col.rgb;
            #endif

            #if ABERRATION_ON
            fixed4 r = tex2D(_MainTex, i.uv + fixed2(_AberrationAmount/10, 0)) * _AberrationColor;
            fixed4 g = tex2D(_MainTex, i.uv + fixed2(-_AberrationAmount/10, 0)) * _AberrationColor;
            fixed4 b = tex2D(_MainTex, i.uv + fixed2(0,-_AberrationAmount/10)) * _AberrationColor;
            fixed4 a = tex2D(_MainTex, i.uv + fixed2(0,_AberrationAmount/10)) * _AberrationColor;
            fixed alpha = (r.a + g.a + b.a + a.a)/4;
            col = fixed4(r.r , g.g, b.b , max(alpha * _AberrationAlpha, col.a));
            #endif

            return col;
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
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }

    CustomEditor "AllInOnePostProcessShaderGUI"
}
