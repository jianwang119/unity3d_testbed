Shader "Custom/CharacterStatue"
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		//_MetallicMap("Metallic (R) Smoothness(A)", 2D) = "white" {}
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_ShineValue("Shine Value", Range(0,2)) = 0.0

		_StatueTex("Statue Texture (RGB)", 2D) = "white" {}
		_StatueRatio("StatueRatio", Range(1, 0)) = 1
		_MainTexRatio("_MainTexRatio", Range(1, 0)) = 1
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
		};

		fixed4 _Color;
		sampler2D _MainTex;
		//sampler2D _MetallicMap;
		half _Metallic;
		half _Glossiness;
		half _ShineValue;

		sampler2D _StatueTex;
		half _StatueRatio;
		half _MainTexRatio;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 greyTex = Luminance(tex);
			fixed4 statueTex = tex2D(_StatueTex, IN.uv_StatueTex);
			fixed4 c1 = lerp(greyTex, tex, _MainTexRatio);
			fixed4 c = lerp(c1, statueTex, _StatueRatio) * _Color;
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
