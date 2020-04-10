#ifndef _V_PRT_HLSLI
#define _V_PRT_HLSLI

struct AMBIENT_PRT
{
	float coefficient : BLENDWEIGHT1;
};

struct LINEAR_PRT
{
	float4 coefficients : BLENDWEIGHT1;
};

struct QUADRATIC_PRT
{
	float3 coefficients1 : BLENDWEIGHT1;
	float3 coefficients2 : BLENDWEIGHT2;
	float3 coefficients3 : BLENDWEIGHT3;
};
#endif