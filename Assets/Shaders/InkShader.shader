Shader "Custom/InkShader" {
	Properties {
		_Color("Color", Color) = (0, 0, 0, 1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { 
			"Queue"="Transparent-1"
			"RenderType"="Opaque" 			
		}
		LOD 200

		//Todo : zTest alwaysだとオブジェクトが増えてきたときにRenderer Queueの設定が複雑になるので、追々、not staticなオブジェクトよりqueueを小さくするなど方法を考える
		ZTest Always

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf NoLighting

		half4 _Color;
		sampler2D _MainTex;

		
		struct Input {
			half2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			half4 c =  _Color;
			o.Albedo = c.rgb;
		}

		half4 LightingNoLighting (SurfaceOutput s, half3 lightDir, half atten)
		{	
			half4 finalColor;
			finalColor.rgb = s.Albedo;
			finalColor.a = s.Alpha;
			return finalColor;
		}
		ENDCG
	}	
	FallBack "Diffuse"
}
