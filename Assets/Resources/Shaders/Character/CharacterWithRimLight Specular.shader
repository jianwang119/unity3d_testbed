Shader "Custom/CharacterWithRimLight Specular"
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		_Glossiness("Smoothness", Range(0.0, 1.5)) = 1
		_SpecGlossMap("Specular", 2D) = "white" {}

		_ShineValue("Shine Value", Range(0,2)) = 0.0

		_RimColor("Rim Color", Color) = (0.2,0.2,0.2,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue" = "Geometry+500" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf StandardSpecular fullforwardshadows

		#pragma target 3.0

		struct Input 
		{
			float2 uv_MainTex;
			float3 viewDir;
		};

		fixed4 _Color;
		sampler2D _MainTex;
		half _Glossiness;
		sampler2D _SpecGlossMap;
		half _ShineValue;
		fixed4 _RimColor;
		half _RimPower;

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) 
		{
			fixed4 c = _Color * tex2D(_MainTex, IN.uv_MainTex);
			fixed4 s = tex2D(_SpecGlossMap, IN.uv_MainTex);

			o.Albedo = c.rgb + _ShineValue;
			o.Specular = s.rgb;
			o.Smoothness = s.a * _Glossiness;
			o.Alpha = c.a;

			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
