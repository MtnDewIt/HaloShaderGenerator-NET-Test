#ifndef _CHUD_INPUT_OUTPUT_HLSLI
#define _CHUD_INPUT_OUTPUT_HLSLI

struct VS_OUTPUT_CORTANA_COMPOSITE
{
    float4 position : SV_Position;
	float2 v0 : TEXCOORD0;
	float2 v1 : TEXCOORD1;
	float2 v2 : TEXCOORD2;
	float v3 : TEXCOORD3;
};

struct PS_OUTPUT_CORTANA_COMPOSITE
{
	float4 diffuse;
	float4 normal;
	float4 unknown;
};


#endif