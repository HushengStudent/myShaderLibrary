Shader "myShaderLibrary/UI/ZTestMask" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" }
		LOD 200

		ZWrite On
		ColorMask 0

		Pass{}
	}

	FallBack "Diffuse"
}
