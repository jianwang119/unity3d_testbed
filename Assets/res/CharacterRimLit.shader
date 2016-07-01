Shader "Custom/CharacterRimLit"
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		//_MetallicMap("Metallic (R) Smoothness(A)", 2D) = "white" {}
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		
		_BumpMap("Normal Map", 2D) = "bump" {}
		_RimColor("Rim Color", Color) = (0.2,0.2,0.2,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
	}
	SubShader {
		Tags{ "RenderType" = "Opaque" "Queue" = "Transparent-1" }
		LOD 200

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows

			#pragma target 3.0

			struct Input
			{
				float2 uv_MainTex;
				float2 uv_BumpMap;
				float3 viewDir;
			};

			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _MetallicMap;
			half _Metallic;
			half _Glossiness;

			sampler2D _BumpMap;
			float4 _RimColor;
			float _RimPower;

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = _Color * tex2D(_MainTex, IN.uv_MainTex);
				fixed4 m = 0;
				//m = tex2D(_MetallicMap, uv);
				m.r = _Metallic;
				m.a = _Glossiness;

				o.Albedo = c.rgb;
				o.Metallic = m.r;
				o.Smoothness = m.a;
				o.Alpha = c.a;

				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
				o.Emission = _RimColor.rgb * pow(rim, _RimPower);
			}
			ENDCG
	}
	FallBack "Diffuse"
}
