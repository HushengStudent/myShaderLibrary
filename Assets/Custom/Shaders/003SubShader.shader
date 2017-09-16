/*
VS2015对Shader有部分支持（只有关键字高亮）
VS插件：ShaderLabVS
*/
Shader "myShaderLibrary/003SubShader" { //Shader的路径名称
    //资源属性代码块
	Properties {   
		_MainTex ("Base (RGB)", 2D) = "white" {}

		//_MainTex ("Base (RGB)", 2D) = "white" {}
		// 变量名  Inspector显示名字 变量类型  默认值
		/* 
         类型: 
         Color  一种颜色 
         2D     2的阶数大小的贴图 
         Rect   非2阶数大小的贴图 
         Cube   立方体纹理 
         Range(min, max) 介于最小值最大值的浮点数 
         Float 任意一个浮点数 
         Vector 一个四维数 
         */  
//Properties example===================================
		_Color("Color",Color)=(1,1,1,1)
		_Vector("Vector",Vector)=(1,2,3,4)
		_Int("Int",Int)= 34234
		_Float("Float",Float) = 4.5
		_Range("Range",Range(1,11))=6
		_2D("Texture",2D) = "red"{}
		_Cube("Cube",Cube) = "white"{}
		_3D("Texure",3D) = "black"{}
//====================================================
	}
	/*
	SubShader可以写很多个 显卡运行效果的时候，从第一个SubShader开始
	如果第一个SubShader里面的效果都可以实现，那么就使用第一个SubShader
	如果显卡这个SubShader里面某些效果它实现不了，它会自动去运行下一个SubShader
	*/
	SubShader {
		//标签：控制以怎样的方式？何时渲染模型?
		/* 
        重要的Tags: 
        "RenderType"="Transparent"     透明/半透明像素渲染 
        "ForceNoShadowCasting"="True"  从不产生阴影 
        "Queue"="xxx"                  
		渲染顺序队列，主要参数： 
		"Backgroung"  最早调用，用来渲染天空盒或者背景 1000 
        "Geometry"    渲染非透明物体，默认值 2000 
        "AlphaText"    "Transparent" 从后往前的顺序渲染透明物体 3000 
        "Overlay" 渲染叠加效果，渲染的最后阶段，镜头光晕之类 4000 
        */  
		Tags { "RenderType"="Opaque" }
		LOD 200 //Level of Detail
		//Shader Level of Detail (LOD) works by only using shaders or
		//subshaders that have their LOD value less than a given number
		
		CGPROGRAM
		/*
		告诉着色器使用哪个光照模型，此处surf函数使用Lambert光照模型
		surface指明这是一个表面着色器
		*/
		#pragma surface surf Lambert 

		sampler2D _MainTex;

//在CG语言中调用=====================================
			float4 _Color; 
			float4 _Vector;
			float _Int;
			float _Float;
			float _Range; 
			sampler2D _2D;
			samplerCUBE _Cube;
			sampler3D _3D;
			//float  32位来存储
			//half  16  -6万 ~ +6万
			//fixed 11 -2 到 +2
//====================================================
		struct Input {
			float2 uv_MainTex;
		};

		//处理函数，输入输出必须按规定写
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 

	//没有匹配的SubShader
	//FallBack "Diffuse"
}

/*
     Surface Shader 结构
     Shader "Custom/myshader01" {
         Properties {
     
         }
         SubShader {
         
         }
         FallBack "Diffuse"
     }
*/
