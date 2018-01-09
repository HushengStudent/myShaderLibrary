Shader "myShaderLibrary/Common/002Vertex&FragmentShader" {
	SubShader {
		Pass{
			CGPROGRAM
			//顶点函数声明顶点函数的函数名
			//基本作用是 完成顶点坐标从模型空间到剪裁空间的转换
			//（从游戏环境转换到视野相机屏幕上）
			#pragma vertex vert
			//片元函数声明了片元函数的函数名
			//基本作用返回模型对应的屏幕上的每一个像素的颜色值
			#pragma fragment frag
			float4 vert(float4 v : POSITION) :SV_POSITION {
			//POSITION:顶点坐标
			//SV_POSITION:返回值是剪裁空间下的顶点坐标
			//UNITY_MATRIX_MVP:Unity内置矩阵：模型观察投影矩阵
			return mul(UNITY_MATRIX_MVP,v);
			}
			//SV_Target：将输出的颜色输出到默认的帧缓冲
			fixed4 frag() :SV_Target 
			{
				return fixed4(0.5,0.5,1,1);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
