// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

/******************************************************************************************* 
 * Shader : Bumped Specular
 ********************************************************************************************/ 

Shader "WG/Bumped Specular"
{
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1) 
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}
 
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Geometry"  "SHADOWSUPPORT"="true"}

		LOD 200 
		Cull Back

		Pass {
 
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			
			ZWrite On
			
			CGPROGRAM
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase noforwardadd

			#pragma multi_compile __ CUSTOM_FOG_LINEAR
			#pragma multi_compile __ CUSTOM_FOG_EXP
			#pragma multi_compile __ CUSTOM_FOG_EXP2
			#pragma multi_compile __ CUSTOM_FOG_HEIGHT
			#include "HLSLSupport.cginc"
			#include "UnityShaderVariables.cginc"
			#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "WGCG.cginc"
			#define INTERNAL_DATA
			#define WorldReflectionVector(data,normal) data.worldRefl
			#define WorldNormalVector(data,normal) normal
			#line 1
			#line 12

			sampler2D _MainTex;
			sampler2D _BumpMap;
			fixed4 _Color;
			half _Shininess;

			struct Input {
				float2 uv_MainTex;
				float2 uv_BumpMap;
			};
 
			void surf (Input IN, inout SurfaceOutput o) {
				fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = tex.rgb * _Color.rgb;
				o.Gloss = tex.a;
				o.Alpha = tex.a * _Color.a;
				o.Specular = _Shininess;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			}

			#ifdef LIGHTMAP_OFF
				struct v2f_surf {
				  float4 pos : SV_POSITION;
				  float4 pack0 : TEXCOORD0;
				  fixed3 lightDir : TEXCOORD1;
				  fixed3 vlight : TEXCOORD2;
				  float3 viewDir : TEXCOORD3;
				  CUSTOM_UNITY_FOG_COORDS(4)
				  LIGHTING_COORDS(5,6)
				};
			#endif

			#ifndef LIGHTMAP_OFF
				struct v2f_surf {
				  float4 pos : SV_POSITION;
				  float2 pack4 : TEXCOORD0;
				  float2 lmap : TEXCOORD1;
				  CUSTOM_UNITY_FOG_COORDS(2)
				  LIGHTING_COORDS(3,4)
				};
			#endif


			float4 _MainTex_ST;
			float4 _BumpMap_ST;

			v2f_surf vert_surf (appdata_full v) 
			{
				v2f_surf o;
				UNITY_INITIALIZE_OUTPUT(v2f_surf, o);
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.pack0.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

				#ifndef LIGHTMAP_OFF
					o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				// World2Object unity_Scale.w is removed
				float3 worldN = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));

				TANGENT_SPACE_ROTATION;
				float3 lightDir = mul (rotation, ObjSpaceLightDir(v.vertex));
				#ifdef LIGHTMAP_OFF
					o.lightDir = lightDir;
				#endif

				#if defined (LIGHTMAP_OFF) || !defined (DIRLIGHTMAP_OFF)
					float3 viewDirForLight = mul (rotation, ObjSpaceViewDir(v.vertex));
					o.viewDir = viewDirForLight;
				#endif

				#ifdef LIGHTMAP_OFF
					float3 shlight = ShadeSH9 (float4(worldN,1.0));
					o.vlight = shlight;

					#ifdef VERTEXLIGHT_ON
						float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
						o.vlight += Shade4PointLights (
						unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
						unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
						unity_4LightAtten0, worldPos, worldN );
					#endif // VERTEXLIGHT_ON
				#endif // LIGHTMAP_OFF

				CUSTOM_UNITY_TRANSFER_FOG(o, o.pos, mul(unity_ObjectToWorld, v.vertex).xyz);	
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}


			fixed4 frag_surf (v2f_surf IN) : COLOR
			{
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT(Input, surfIN);

				surfIN.uv_MainTex = IN.pack0.xy;
				surfIN.uv_BumpMap = IN.pack0.zw;

				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT(SurfaceOutput, o);

				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Specular = 0.0;
				o.Alpha = 0.0;
				o.Gloss = 0.0;

				surf (surfIN, o);

				fixed atten = LIGHT_ATTENUATION(IN);
				fixed4 c = 0;

				#ifdef LIGHTMAP_OFF
					c = CustomLightingBlinnPhong (o, IN.lightDir, normalize(half3(IN.viewDir)), atten);
				#endif // LIGHTMAP_OFF

				#ifdef LIGHTMAP_OFF
					c.rgb += o.Albedo * IN.vlight;
				#endif // LIGHTMAP_OFF
			
				#ifndef LIGHTMAP_OFF
					fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy);
					fixed3 lm = DecodeLightmap (lmtex);

					#ifdef SHADOWS_SCREEN

						#if defined(SHADER_API_GLES) && defined(SHADER_API_MOBILE)
							c.rgb += o.Albedo * min(lm, atten);
						#else
							c.rgb += o.Albedo * max(min(lm,(atten)*lmtex.rgb), lm*atten);
						#endif
							
					#else // SHADOWS_SCREEN	
						c.rgb += o.Albedo * lm;
					#endif // SHADOWS_SCREEN

					c.a = o.Alpha;
				#endif // LIGHTMAP_OFF

				c.a = o.Alpha;

				CUSTOM_UNITY_APPLY_FOG(IN.fogCoord, c, 1);
				return c;
			}
		
			ENDCG
		} 

		// --------------------------------------------------------------------------------------------------------------------
		//  Shadow caster rendering pass

		Pass
        {
            Tags {"LightMode"="ShadowCaster"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f { 
                V2F_SHADOW_CASTER;
            };

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
	}
	Fallback "VertexLit"
}
