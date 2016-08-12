Shader "Custom/Character"
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		//_MetallicMap("Metallic (R) Smoothness(A)", 2D) = "white" {}
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_ShineValue("Shine Value", Range(0,2)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue" = "Transparent-1" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		struct Input 
		{
			float2 uv_MainTex;
		};

		fixed4 _Color;
		sampler2D _MainTex;
		sampler2D _MetallicMap;
		half _Metallic;
		half _Glossiness;
		half _ShineValue;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c  = _Color * tex2D(_MainTex, IN.uv_MainTex);
			fixed4 m = 0;
			//m = tex2D(_MetallicMap, uv);
			m.r = _Metallic;
			m.a = _Glossiness;

			o.Albedo = c.rgb + _ShineValue;
			o.Metallic = m.r;
			o.Smoothness = m.a;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
