Shader "UIEx/Default-Mask"
{
    Properties
    {
         _MainTex ("Sprite Texture", 2D) = "white" {}
        _Mask ("Mask Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [Toggle] _isGray("is Gray", Float) = 0

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		
		_ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

        Pass
        {
    	CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
                half2 texcoordMask : TEXCOORD1;
				float4 worldPosition : TEXCOORD2;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Mask;
            float4 _Mask_ST;
			float4 _ClipRect;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
				OUT.worldPosition = IN.vertex;
                OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord,_MainTex);
                OUT.texcoordMask = TRANSFORM_TEX(IN.texcoord,_Mask);
#ifdef UNITY_HALF_TEXEL_OFFSET
                OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
                OUT.color = IN.color * _Color;
                return OUT;
            }

            float _isGray;
            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;
                // clip (color.a - 0.01);

                half4 color2 = tex2D(_Mask, IN.texcoordMask);
                color.a = color.a * color2.a;

				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

                if(_isGray == 1)
				{
					float grey = dot(color.rgb, fixed3(0.22, 0.707, 0.071));
					return fixed4(grey, grey, grey, color.a);
				}

                return color;
//              return fixed4(color.r, color.g, color.b, color2.r);
            }
        ENDCG
        }
    }
}
