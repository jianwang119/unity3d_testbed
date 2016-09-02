Shader "Custom/CharacterCombineRimLight"
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MetallicMap("Metallic (R) Smoothness(A)", 2D) = "white" {}

		_Color1("P1:Color", Color) = (1,1,1,1)
		_MainTex1("P1:Albedo (RGB)", 2D) = "white" {}
		_MetallicMap1("P1:Metallic (R) Smoothness(A)", 2D) = "white" {}

		_Color2("P2:Color", Color) = (1,1,1,1)
		_MainTex2("P2:Albedo (RGB)", 2D) = "white" {}
		_MetallicMap2("P2:Metallic (R) Smoothness(A)", 2D) = "white" {}

		_ShineValue("Shine Value", Range(0,2)) = 0.0
		_RimColor("Rim Color", Color) = (0.2,0.2,0.2,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0

	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" "Queue" = "Transparent-2" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		struct Input 
		{
			float2 uv_MainTex;
			float3 viewDir;
		};

		fixed4 _Color;
		sampler2D _MainTex;
		sampler2D _MetallicMap;

		fixed4 _Color1;
		sampler2D _MainTex1;
		sampler2D _MetallicMap1;

		fixed4 _Color2;
		sampler2D _MainTex2;
		sampler2D _MetallicMap2;

		half _ShineValue;
		fixed4 _RimColor;
		half _RimPower;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = _Color;
			fixed4 m = 0;
			half2 uv = IN.uv_MainTex;
			if (uv.x > 2)
			{
				c = _Color2 * tex2D(_MainTex2, uv);
				m = tex2D(_MetallicMap2, uv);
			}
			else if (uv.x > 1)
			{
				c = _Color1 * tex2D(_MainTex1, uv);
				m = tex2D(_MetallicMap1, uv);
			}
			else
			{
				c = _Color * tex2D(_MainTex, uv);
				m = tex2D(_MetallicMap, uv);
			}
			o.Albedo = c.rgb + _ShineValue;
			o.Metallic = m.r;
			o.Smoothness = m.a;
			o.Alpha = c.a;

			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
