Shader "Custom/CharacterStatue"
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		//_MetallicMap("Metallic (R) Smoothness(A)", 2D) = "white" {}
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Glossiness("Smoothness", Range(0,1)) = 0.5

		
		_EnvTex("Cube env tex", CUBE) = "black" {}
		_EnvTexMul("env tex Mul", Range(0, 10)) = 1
		_MainTexMul("MainTex Mul", Range(0,10)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue" = "Geometry+500" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_StatueTex;
			float3 worldRefl;
		};

		fixed4 _Color;
		sampler2D _MainTex;
		//sampler2D _MetallicMap;
		half _Metallic;
		half _Glossiness;

		samplerCUBE _EnvTex;
		half _EnvTexMul;
		half _MainTexMul;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 greyTex = tex;// Luminance(tex);
			fixed4 ref = texCUBE(_EnvTex, WorldReflectionVector(IN, o.Normal));

			fixed4 m = 0;

			//m = tex2D(_MetallicMap, uv);
			m.r = _Metallic;
			m.a = _Glossiness;

			o.Albedo = (greyTex * _MainTexMul + ref * _EnvTexMul)* _Color;
			o.Metallic = m.r;
			o.Smoothness = m.a;
			o.Alpha = tex.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
