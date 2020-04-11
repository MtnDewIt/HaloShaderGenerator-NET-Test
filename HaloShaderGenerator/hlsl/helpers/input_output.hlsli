#ifndef _INPUT_OUTPUT_HLSLI
#define _INPUT_OUTPUT_HLSLI

struct VS_OUTPUT_ALBEDO
{
    float4 position : SV_Position;
    float2 texcoord : TEXCOORD;
    float4 normal : TEXCOORD1;
    float3 binormal : TEXCOORD2;
    float3 tangent : TEXCOORD3;
    float3 camera_dir : TEXCOORD4;
};

struct VS_OUTPUT_STATIC_PRT
{
	float4 position : SV_Position;
	float2 texcoord : TEXCOORD;
	float3 normal : TEXCOORD3;
	float3 binormal : TEXCOORD4;
	float3 tangent : TEXCOORD5;
	float3 camera_dir : TEXCOORD6;
	float4 prt_radiance_vector : TEXCOORD7;
	float3 extinction_factor : COLOR;
	float3 sky_radiance : COLOR1;
};

struct VS_OUTPUT_ACTIVE_CAMO
{
	float4 vPos : SV_Position;
    float4 TexCoord : TEXCOORD0;
    float4 TexCoord1 : TEXCOORD1;
    float4 TexCoord2 : TEXCOORD2;
    float4 TexCoord3 : TEXCOORD3;
    float4 TexCoord4 : TEXCOORD4;
    float4 TexCoord5 : TEXCOORD5;
    float4 TexCoord6 : TEXCOORD6;
};

struct VS_OUTPUT_SFX_DISTORT
{
	float4 position : SV_Position;
	float4 texcoord : TEXCOORD;
	float distortion : TEXCOORD1;
};

struct PS_OUTPUT_ALBEDO
{
	float4 Diffuse;
	float4 Normal;
	float4 Unknown;
};

struct PS_OUTPUT_DEFAULT
{
    float4 LowFrequency;
    float4 HighFrequency;
    float4 Unknown;
};

#endif