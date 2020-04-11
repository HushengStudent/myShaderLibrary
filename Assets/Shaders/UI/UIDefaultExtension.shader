﻿// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "myShaderLibrary/UI/UIDefaultExtension"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        //[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        _GrayScaleLuminosity ("Gray Scale Luminosity", Range(0,1)) = 0.2

        _BlurStrength ("Blur Strength", Range(0,10)) = 3

        _ScanLineJitter("Scan Line Jitter", Range(0,1)) = 0.2
        _VerticalJumpRange("Vertical Jump Range", Range(0,1)) = 0.02
        _VerticalJumpSpeed("Vertical Jump Speed", Range(-10,10)) = -5
        _HorizontalShake("Horizontal Shake", Range(0,1)) = 0.02
        _ColorDrift("Color Drift", Range(0,1)) = 0.1

        _GlowColor("Glow Color", Color) = (1,1,1,1)
		_GlowIntensity("Glow Intensity", Range(0,100)) = 10

        _MeltNoiseTex("Melt Noise Texture", 2D) = "white" {}
        _MeltStrength ("Melt Strength", Range(0,1)) = 0
        _MeltAddColor ("Melt Add Color", Color) = (1,1,1,1)
        _MeltAddColorStrength ("Melt Add Color Strength", Range(1,20)) = 1
        _MeltAddColorLength ("Melt Add Color Length", Range(0,1)) = 0
        _MeltAdditionalTex("Melt Additional Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "Assets/Shaders/CGIncludes/Common.cginc"

            #pragma shader_feature UI_CLIP_RECT_ON

            //#pragma multi_compile __ UNITY_UI_CLIP_RECT
            //#pragma multi_compile __ UNITY_UI_ALPHACLIP
            
            //置灰
            #pragma shader_feature GREYSCALE_ON
            //模糊
            #pragma shader_feature BLUR_ON
            //故障
            #pragma shader_feature Glitch_ON
            //发光
            #pragma shader_feature GLOW_ON
            //消融
            #pragma shader_feature MELT_ON
            #pragma shader_feature MELT_TEX_ON

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;

                #ifdef MELT_ON
                float2 meltNoiseUV  : TEXCOORD2;
                float2 additionalUV  : TEXCOORD3;
                #endif

                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;


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

            #ifdef GLOW_ON
            float4 _GlowColor;
            float _GlowIntensity;
            #endif

            #ifdef MELT_ON
            sampler2D _MeltNoiseTex;
            float4 _MeltNoiseTex_ST;
            float _MeltStrength;
            fixed4 _MeltAddColor;
            float _MeltAddColorStrength;
            float _MeltAddColorLength;
            sampler2D _MeltAdditionalTex;
            float4 _MeltAdditionalTex_ST;
            #endif

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;

                #ifdef MELT_ON
                OUT.meltNoiseUV = TRANSFORM_TEX(v.texcoord, _MeltNoiseTex);
                OUT.additionalUV = TRANSFORM_TEX(v.texcoord, _MeltAdditionalTex);
                #endif

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                #ifdef UI_CLIP_RECT_ON
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                /*
                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif
                */
                
                #ifdef Glitch_ON
                float u = IN.texcoord.x;
                float v = IN.texcoord.y;

                float jitter_thresh = saturate(1.0f - _ScanLineJitter * 1.2f);
                float jitter_disp = 0.002f + pow(_ScanLineJitter, 3) * 0.05f;
                float jitter = rand(fixed2(v, _Time.x)) * 2 - 1;
                jitter *= step(jitter_thresh, abs(jitter)) * jitter_disp;

                float jump = lerp(v, frac(v + _Time.x * _VerticalJumpSpeed), _VerticalJumpRange);
                float shake = (rand(fixed2(_Time.x, 2)) - 0.5) * _HorizontalShake* 0.2f;
                float drift = sin(jump + _Time.y* 606.11f) * _ColorDrift* 0.04f;

                half4 src1 = tex2D(_MainTex, frac(float2(u + jitter + shake, jump)));
                half4 src2 = tex2D(_MainTex, frac(float2(u + jitter + shake + drift, jump)));
                color = half4(src1.r, src2.g, src1.b, color.a);
                color.a *= step(0.1, src1.a)*step(0.1, src2.a);
                #endif

                #ifdef GLOW_ON
                color.rgb += color.a * _GlowIntensity * _GlowColor;
                #endif

                #ifdef BLUR_ON
                color = Blur(IN.texcoord, _MainTex, _BlurStrength);
                #endif

                #ifdef GREYSCALE_ON
                color.rgb = saturate((color.r + color.g + color.b) * _GrayScaleLuminosity);
                #endif

                #ifdef MELT_ON
                float melt = tex2D(_MeltNoiseTex, IN.meltNoiseUV).a;
                float value = melt - _MeltStrength;
                float clip = step(0.01, value);
                color.rgb *= clip;
                float length = saturate(value + _MeltAddColorLength);
                color.a *= step(0.01, length);
                float degrees = saturate(_MeltAddColorStrength * length * (1 -clip));
                half4 additional = tex2D(_MeltAdditionalTex, IN.additionalUV);
                #ifdef MELT_TEX_ON
				color.rgb = lerp(color.rgb, additional , degrees);
                #else
				color.rgb = lerp(color.rgb, _MeltAddColor , degrees);
                #endif
                #endif
                return color;
            }
        ENDCG
        }
    }
	CustomEditor "UIDefaultExtensionShaderGUI"
}
