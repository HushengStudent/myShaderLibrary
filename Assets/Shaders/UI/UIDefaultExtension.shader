// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

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

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
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
                
                #ifdef BLUR_ON
                color = Blur(IN.texcoord, _MainTex, _BlurStrength);
                #endif

                #ifdef GREYSCALE_ON
                color.rgb = saturate((color.r + color.g + color.b) * _GrayScaleLuminosity);
                #endif

                return color;
            }
        ENDCG
        }
    }
	CustomEditor "UIDefaultExtensionShaderGUI"
}
