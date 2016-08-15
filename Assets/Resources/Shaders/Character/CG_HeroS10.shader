Shader "CodeGame/CG_HeroS10"
{
	Properties 
	{
		//基础属性：颜色、贴图、材质区分
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)							
		_MainTex ("Tex(RGB)&Spec(A)", 2D) = "white" {}
		_PathTex("PathTex(RGB)", 2D) = "white" {}		
		//进阶属性：法线、背光、环境反射
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_Ramp ("ToonRamp", 2D) = "black" {}
		//_RefTex ("RefTex", 2D) = "gray" {}
		_RefTex ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
	}
	
	SubShader 
	{		
		//Cull Off
		Tags { "RenderType" = "Opaque"}

		CGPROGRAM
		#pragma surface surf Spec
		#pragma target 3.0
		
		fixed4 _Color;
		sampler2D _MainTex, _PathTex, _BumpMap, _Ramp;//, _RefTex;
		samplerCUBE _RefTex;
		float SpecSmooth;

		//光照渲染
		half4 LightingSpec (SurfaceOutput s, half3 lightDir, fixed3 viewDir, half atten) 
		{	
			s.Normal = normalize(s.Normal);

			//明暗渲染
			half diff = max(0, dot (s.Normal, lightDir)* 0.9 + 0.1);//*(1 - _Params[0].b) + _Params[0].b;
			half difframp = max(0, dot (s.Normal, lightDir)* 0.6 + 0.4);

			//高光渲染
			half3 h = normalize (lightDir + viewDir);
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, s.Specular * 128.0) * s.Gloss * 2;
			spec = smoothstep(0.5 - SpecSmooth * 0.5, 0.5 + SpecSmooth * 0.5, spec);
			
			//背光渲染
			half3 ramp = tex2D (_Ramp, float2(difframp, diff)).rgb;
			
			half4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * (diff + ramp * 3 * float3(0,1,2))  + _LightColor0.rgb * spec) * atten;
			//c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
			c.a = s.Alpha;
			//clip( c.a - 0.01 );
			return c;
		}
    	
		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_Ramp;
			float3 viewDir;
			float3 worldRefl;
			INTERNAL_DATA
		};

		//材质区分计算
		float4x4 GetParams(half4 Path)
		{
			//高光参数Params[0]:_diffPath, _SpecPower, SpecSmooth, _Shininess, 
			//法线参数Params[1]:UV_noiseX,UV_noiseY,UV_noisePower
			//轮廓光参数Params[2]:_RimColor.rgb, _RimPower
			//反射参数Params[3]:_RefColor.rgb, _RefPower
			float v = Path.r + Path.g * 2 + Path.b * 4;
			if(v < 0.01)//透明
			{
				return float4x4(//0.2,0.5,0.3,0.05,
								0.1,0.25,0.7,0.8,
								0,0,0,0,
								0.2,0.2,0.2,8,
								0,0,0,0.15);
			}
			else if(v < 1.5)//木材
			{
				return float4x4(//0.4,0.15,0.3,0.05,
								0.3,0.2,1,0.15,
								0,0,0,0,
								0.2,0.2,0.2,8,
								0,0,0,0.05);
			}
			else if(v < 2.5)//头发
			{
				return float4x4(//0.2,0.5,0.3,0.05,
								0.3,0.2,0.5,0.5,
								0.1,100,3,0,
								0.2,0.2,0.2,8,
								0,0,0,0.05);
			}
			else if(v < 3.5)//金属
			{
				return float4x4(//1,1,0.3,0.5,
								0.3,.7,0,1,
								0,0,0,0,
								0.2,0.2,0.2,8,
								0,0,0,0.5);
			}
			else if(v < 4.5)//布料
			{
				//float3 c = main.rgb / max(main.r, max(main.g, main.b)) * 0.5;
				return float4x4(//0.1,0.14,0.3,0,
								0.3,0.1,1,0.14,
								4,64,1,0,
								//c.r,c.g,c.b,8,
								0.2,0.2,0.2,8,
								0,0,0,0);
			}
			else if(v < 5.5)//宝石
			{
				return float4x4(//1,0.17,0.5,0.9,
								0.1,0.4,0.1,0.8,
								0,0,0,0,
								0.1,0.1,0.1,8,
								0,0,0,0.9);
			}
			else if(v < 6.5)//未知
			{
				return float4x4(0,0,0,0,
								0,0,0,0,
								0,0,0,0,
								0,0,0,0);
			}
			else//皮肤
			{
				return float4x4(//0.2,0.37,0.3,0,
								0.3,0.18,0.5,0.5,
								0,0,0,0,
								0.2,0.2,0.2,8,
								0,0,0,0);
			}
		}

		float2 R_To_UV(float3 r)
        {
            float interim = 2.0 * sqrt(r.x * r.x + r.y * r.y + (r.z + 1) * (r.z + 1)); 
            return float2(r.x/interim+0.5,r.y/interim+0.5);
        }

		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 main = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			half4 path = tex2D (_PathTex, IN.uv_MainTex);
			float4x4 _Params	= GetParams(path);

			//噪点
			//half noise = (tex2D (_Ramp, IN.uv_MainTex * float2(_Params[1].r, _Params[1].g)).a - 0.5) * _Params[1].b;
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_MainTex));// + float4(0, noise, 0, noise));

			//反射
			fixed4 ref = texCUBE (_RefTex, WorldReflectionVector (IN, o.Normal));
//			float3 n = normalize(o.Normal);
//			float3 reflectedDir = reflect(IN.viewDir, n);// / 5 + 0.5;
//			half4 ref = tex2D(_RefTex, R_To_UV(reflectedDir));
			
			//轮廓
			half angel = dot (normalize(IN.viewDir), o.Normal);
			//half rim = 0;
			//if( angel > -0.3)
			half rim = 1 - saturate(abs(angel));

			o.Gloss = _Params[0].g;
			SpecSmooth = _Params[0].b;
			o.Specular = _Params[0].a;

			o.Emission = _Params[2].rgb * pow (rim, _Params[2].a) + ref.rgb * _Params[3].w;
			//o.Emission = ref.rgb * _Params[0].a;

			o.Albedo = main.rgb;
			o.Alpha = main.a;
		}
		ENDCG		
	}	
	Fallback "Diffuse"
}