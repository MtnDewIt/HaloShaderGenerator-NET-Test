#ifndef _CHUD_INPUT_OUTPUT_HLSLI
#define _CHUD_INPUT_OUTPUT_HLSLI

struct VS_OUTPUT_CORTANA_COMPOSITE
{
    float4 position : SV_Position;
};

struct PS_OUTPUT_CORTANA_COMPOSITE
{
	float4 diffuse;
	float4 normal;
	float4 unknown;
};


#endif