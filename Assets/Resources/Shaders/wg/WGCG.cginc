#ifndef CUSTOM_CG_INCLUDED
#define CUSTOM_CG_INCLUDED

#include "UnityShaderVariables.cginc"
#include "Lighting.cginc"

/*****************************************************************************************************
 * Function : Lambert 
 *****************************************************************************************************/

float4 CustomLightingLambert (SurfaceOutput s, float3 lightDir, float atten)
{
	float diff = max (0, dot (s.Normal, lightDir));
    // diff = diff * 0.5 + 0.5;	
	float4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * diff * atten * 2;
	c.a = s.Alpha;
	return c;
}


// ------------------------------------------------------------------
//  Fog helpers
//
//	multi_compile_fog Will compile fog variants.
//	UNITY_FOG_COORDS(texcoordindex) Declares the fog data interpolator.
//	UNITY_TRANSFER_FOG(outputStruct,clipspacePos) Outputs fog data from the vertex shader.
//	UNITY_APPLY_FOG(fogData,col) Applies fog to color "col". Automatically applies black fog when in forward-additive pass.
//	Can also use CUSTOM_APPLY_FOG_COLOR to supply your own fog color.


// In case someone by accident tries to compile fog code in one of teh g-buffer or shadow passes:
// treat it as fog is off.
#if defined(UNITY_PASS_PREPASSBASE) || defined(UNITY_PASS_DEFERRED) || defined(UNITY_PASS_SHADOWCASTER)
#undef CUSTOM_FOG_LINEAR
#undef CUSTOM_FOG_EXP
#undef CUSTOM_FOG_EXP2
#undef CUSTOM_FOG_HEIGHT
#endif

float4 custom_FogParams;
float4 custom_HeightFogParams;

float linearFogFactor = 1;
float expFogFactor = 1;
float heightFogFactor = 1;
float _LinearFogHeightFactor = 0;

float4 linearFogColor;
float4 expFogColor;
float4 heightFogColor;


float UNITY_FOG_LINEAR(float coord, float height) 
{

	float factor = 0;
	if (_LinearFogHeightFactor > 0)
		factor = height * _LinearFogHeightFactor * 0.001 - 1; 
	else
		factor = coord * custom_FogParams.z + custom_FogParams.w;
		
	return factor;
}

float UNITY_FOG_EXP(float coord)
{
	float factor = 0;
	if (_LinearFogHeightFactor > 0)
		factor = 1;
	else
	{
		factor = custom_FogParams.y * (coord);
		factor = exp2(-factor);
	}
	return factor;
}

float UNITY_FOG_EXP2(float coord)
{
	float factor = 0;
	if (_LinearFogHeightFactor > 0)
		factor = 1;
	else
	{
		factor = custom_FogParams.x * (coord);
		factor = exp2(-factor*factor);
	}
	return factor;
}

float UNITY_FOG_HEIGHT(float coord)
{
	return (coord) * custom_HeightFogParams.z + custom_HeightFogParams.w;
}

/**********************************************************************************************************
 * Function : Calculate Fog Factor
 * linearFogFactor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
 * expFogFactor = exp(-density*z)
 * exp2FogFactor = exp(-(density*z)^2)
 **********************************************************************************************************/

void CUSTOM_CALC_FOG_FACTOR(float4 coord, float3 wPos)
{
	linearFogFactor = 1;
	expFogFactor = 1;	
	heightFogFactor = 1;

	float d = distance(wPos, _WorldSpaceCameraPos);

	#ifdef CUSTOM_FOG_EXP
		expFogFactor = UNITY_FOG_EXP(d);
	#endif


	#ifdef CUSTOM_FOG_LINEAR
		linearFogFactor = UNITY_FOG_LINEAR(d, wPos.y);
	#endif


	#ifdef CUSTOM_FOG_HEIGHT
		heightFogFactor = UNITY_FOG_HEIGHT(wPos.y);
	#endif

	
}


#define CUSTOM_FOG_COORDS_PACKED(idx, vectype) vectype fogCoord : TEXCOORD##idx;

#if defined(CUSTOM_FOG_LINEAR) || defined(CUSTOM_FOG_EXP) || defined(CUSTOM_FOG_EXP2) || defined(CUSTOM_FOG_HEIGHT)

	#define CUSTOM_UNITY_FOG_COORDS(idx) CUSTOM_FOG_COORDS_PACKED(idx, float4)

	#if (SHADER_TARGET < 30) || defined(SHADER_API_MOBILE)
		// mobile or SM2.0: calculate fog factor per-vertex
		#define CUSTOM_UNITY_TRANSFER_FOG(o,outpos,wPos) CUSTOM_CALC_FOG_FACTOR(outpos, wPos); o.fogCoord.y = linearFogFactor; o.fogCoord.z = expFogFactor; o.fogCoord.w = heightFogFactor;
	#else
		// SM3.0 and PC/console: calculate fog distance per-vertex, and fog factor per-pixel
		// #define CUSTOM_UNITY_TRANSFER_FOG(o,outpos,wPos) o.fogCoord = outpos;
		#define CUSTOM_UNITY_TRANSFER_FOG(o,outpos,wPos) CUSTOM_CALC_FOG_FACTOR(outpos, wPos); o.fogCoord.y = linearFogFactor; o.fogCoord.z = expFogFactor; o.fogCoord.w = heightFogFactor;
	#endif
#else
	#define CUSTOM_UNITY_FOG_COORDS(idx)
	#define CUSTOM_UNITY_TRANSFER_FOG(o,outpos,v)
#endif

/**********************************************************************************************************
 * Function : Lerp Fog Color
 **********************************************************************************************************/

float4 CUSTOM_FOG_LERP_COLOR(float4 col, float4 fogFac, float fogDensity) 
{
	fogFac.y = saturate(fogFac.y);	
	fogFac.z = saturate(fogFac.z);	
	fogFac.w = saturate(fogFac.w);	

	float4 source = col;


	#ifdef CUSTOM_FOG_EXP
		col.rgb = lerp(expFogColor.rgb, col.rgb, fogFac.z);	
	#endif

	#ifdef CUSTOM_FOG_LINEAR
		col.rgb = lerp(linearFogColor.rgb, col.rgb, fogFac.y);	
	#endif


	#ifdef CUSTOM_FOG_HEIGHT
		col.rgb = lerp(heightFogColor.rgb, col.rgb, fogFac.w);	
	#endif

	col.rgb = lerp(source.rgb, col.rgb, fogDensity);

	return col;
}

/**********************************************************************************************************
 * Function : Apply Fog Color
 **********************************************************************************************************/

#if defined(CUSTOM_FOG_LINEAR) || defined(CUSTOM_FOG_EXP) || defined(CUSTOM_FOG_EXP2) || defined(CUSTOM_FOG_HEIGHT)
	#if (SHADER_TARGET < 30) || defined(SHADER_API_MOBILE)
		// mobile or SM2.0: fog factor was already calculated per-vertex, so just lerp the color
		#define CUSTOM_APPLY_FOG_COLOR(coord,col,fogDensity) col = CUSTOM_FOG_LERP_COLOR(col, coord,fogDensity);
	#else
		// SM3.0 and PC/console: calculate fog factor and lerp fog color
		// #define CUSTOM_APPLY_FOG_COLOR(coord,col,fogDensity) 
		#define CUSTOM_APPLY_FOG_COLOR(coord,col,fogDensity) col = CUSTOM_FOG_LERP_COLOR(col, coord,fogDensity);
	#endif
#else
	#define CUSTOM_APPLY_FOG_COLOR(coord,col,fogDensity)
#endif


#ifdef UNITY_PASS_FORWARDADD
	#define CUSTOM_UNITY_APPLY_FOG(coord,col,fogDensity) CUSTOM_APPLY_FOG_COLOR(coord,col,fogDensity)
#else
	#define CUSTOM_UNITY_APPLY_FOG(coord,col,fogDensity) CUSTOM_APPLY_FOG_COLOR(coord,col,fogDensity)
#endif



#endif // UNITY_CG_INCLUDED
