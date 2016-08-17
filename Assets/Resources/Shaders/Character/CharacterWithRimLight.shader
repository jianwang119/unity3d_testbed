Shader "Custom/CharacterWithRimLight"
{
	//https://blogs.unity3d.com/cn/2015/02/18/working-with-physically-based-shading-a-practical-approach/

	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MetallicMap("Metallic (R) Smoothness(A)", 2D) = "white" {}
		_ShineValue("Shine Value", Range(0,2)) = 0.0

		_RimColor("Rim Color", Color) = (0.2,0.2,0.2,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
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
			float3 viewDir;
		};

		fixed4 _Color;
		sampler2D _MainTex;
		sampler2D _MetallicMap;
		half _ShineValue;
		fixed4 _RimColor;
		half _RimPower;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = _Color * tex2D(_MainTex, IN.uv_MainTex);
			fixed4 m = tex2D(_MetallicMap, IN.uv_MainTex);

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
