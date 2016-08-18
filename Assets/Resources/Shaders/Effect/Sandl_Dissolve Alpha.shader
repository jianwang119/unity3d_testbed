// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33571,y:32815,varname:node_4013,prsc:2|emission-1903-OUT,alpha-7364-OUT;n:type:ShaderForge.SFN_Tex2d,id:956,x:31879,y:32509,ptovrint:True,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:65e5021f53db7f54998f51796a50237a,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3008,x:31473,y:33079,ptovrint:True,ptlb:Dissolve,ptin:_Dissolve,varname:_Dissolve,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c8c15e32c9902ba43993867aaa4fc094,ntxv:3,isnm:False;n:type:ShaderForge.SFN_Color,id:5963,x:31938,y:32193,ptovrint:True,ptlb:MainColor,ptin:_MainColor,varname:_MainColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:1967,x:32238,y:32406,varname:node_1967,prsc:2|A-5963-RGB,B-956-RGB;n:type:ShaderForge.SFN_VertexColor,id:3550,x:31324,y:32714,varname:node_3550,prsc:2;n:type:ShaderForge.SFN_Add,id:9825,x:31621,y:32637,varname:node_9825,prsc:2|A-1006-OUT,B-3550-R;n:type:ShaderForge.SFN_If,id:5613,x:31964,y:33071,varname:Vetex_Color01,prsc:2|A-9825-OUT,B-3008-R,GT-3908-OUT,EQ-3908-OUT,LT-765-OUT;n:type:ShaderForge.SFN_If,id:9837,x:31954,y:33339,varname:node_9837,prsc:2|A-3550-R,B-3008-R,GT-3908-OUT,EQ-3908-OUT,LT-765-OUT;n:type:ShaderForge.SFN_Vector1,id:3908,x:31486,y:33318,varname:Value01,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:765,x:31486,y:33427,varname:Value02,prsc:2,v1:0;n:type:ShaderForge.SFN_Subtract,id:2066,x:32138,y:33211,varname:node_2066,prsc:2|A-5613-OUT,B-9837-OUT;n:type:ShaderForge.SFN_Multiply,id:8191,x:32328,y:33339,varname:node_8191,prsc:2|A-2066-OUT,B-5679-OUT;n:type:ShaderForge.SFN_Add,id:9472,x:32502,y:33188,varname:node_9472,prsc:2|A-5613-OUT,B-8191-OUT;n:type:ShaderForge.SFN_Multiply,id:7848,x:32793,y:33151,varname:node_7848,prsc:2|A-956-A,B-9472-OUT;n:type:ShaderForge.SFN_Multiply,id:7364,x:32904,y:32983,varname:node_7364,prsc:2|A-3550-A,B-7848-OUT;n:type:ShaderForge.SFN_Multiply,id:405,x:32512,y:32307,varname:node_405,prsc:2|A-8074-OUT,B-1967-OUT;n:type:ShaderForge.SFN_Multiply,id:1903,x:32780,y:32452,varname:node_1903,prsc:2|A-405-OUT,B-956-R;n:type:ShaderForge.SFN_ValueProperty,id:8074,x:32196,y:32100,ptovrint:True,ptlb:Glow,ptin:_Glow,varname:_Glow,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_ValueProperty,id:5679,x:32107,y:33572,ptovrint:True,ptlb:Rim_Shine,ptin:_Rim_Shine,varname:_Rim_Shine,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:100;n:type:ShaderForge.SFN_ValueProperty,id:1006,x:31289,y:32437,ptovrint:True,ptlb:Rim_Size,ptin:_Rim_Size,varname:_Rim_Size,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;proporder:5963-956-3008-8074-5679-1006;pass:END;sub:END;*/

Shader "Shader Forge/Sandl_Dissolve Alpha" {
    Properties {
        _MainColor ("MainColor", Color) = (0.5,0.5,0.5,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Dissolve ("Dissolve", 2D) = "bump" {}
        _Glow ("Glow", Float ) = 0.1
        _Rim_Shine ("Rim_Shine", Float ) = 100
        _Rim_Size ("Rim_Size", Float ) = 0.1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Dissolve; uniform float4 _Dissolve_ST;
            uniform float4 _MainColor;
            uniform float _Glow;
            uniform float _Rim_Shine;
            uniform float _Rim_Size;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = ((_Glow*(_MainColor.rgb*_MainTex_var.rgb))*_MainTex_var.r);
                float3 finalColor = emissive;
                float4 _Dissolve_var = tex2D(_Dissolve,TRANSFORM_TEX(i.uv0, _Dissolve));
                float Vetex_Color01_if_leA = step((_Rim_Size+i.vertexColor.r),_Dissolve_var.r);
                float Vetex_Color01_if_leB = step(_Dissolve_var.r,(_Rim_Size+i.vertexColor.r));
                float Value02 = 0.0;
                float Value01 = 1.0;
                float Vetex_Color01 = lerp((Vetex_Color01_if_leA*Value02)+(Vetex_Color01_if_leB*Value01),Value01,Vetex_Color01_if_leA*Vetex_Color01_if_leB);
                float node_9837_if_leA = step(i.vertexColor.r,_Dissolve_var.r);
                float node_9837_if_leB = step(_Dissolve_var.r,i.vertexColor.r);
                fixed4 finalRGBA = fixed4(finalColor,(i.vertexColor.a*(_MainTex_var.a*(Vetex_Color01+((Vetex_Color01-lerp((node_9837_if_leA*Value02)+(node_9837_if_leB*Value01),Value01,node_9837_if_leA*node_9837_if_leB))*_Rim_Shine)))));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
