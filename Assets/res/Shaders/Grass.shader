Shader "Terrain/Grass"
{
	Properties 
	{
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff ("Cutoff", float) = 0.5
	}

	SubShader 
	{
		Tags 
		{
			"Queue" = "Geometry+200"
			"IgnoreProjector"="True"
			"RenderType"="Grass"
		}
		Cull Off
		LOD 200
		ColorMask RGB
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:GrassVert addshadow
		#pragma exclude_renderers flash

		sampler2D _MainTex;
		fixed _Cutoff;
		half _WaveSpeed;
		half _WaveScale;
		half _FrameUpdate;
	
		void GrassVert (inout appdata_full v)
		{
			half2 disturb = v.texcoord2;
			disturb.y += _FrameUpdate;
			disturb.y *= 0.025f;
			half s = sin(disturb.y);
			half c = cos(disturb.y);
			half3 waveMove = float3(s, 0, c) * disturb.x * 0.08f;
			v.vertex.xz += waveMove.xz;
		}
					
		struct Input
		{
			float2 uv_MainTex;
			fixed4 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			clip (o.Alpha - _Cutoff);
			o.Alpha *= IN.color.a;
		}
ENDCG
	}
	
	Fallback "Legacy Shaders/Transparent/Cutout/Diffuse"
}
