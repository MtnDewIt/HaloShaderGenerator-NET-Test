#ifndef _INPUT_OUTPUT_TERRAIN_HLSLI
#define _INPUT_OUTPUT_TERRAIN_HLSLI



struct VS_OUTPUT_ALBEDO_TERRAIN
{
    float4 position : SV_Position;
    float4 texcoord : TEXCOORD;
    float4 normal : TEXCOORD1;
    float3 binormal : TEXCOORD2;
    float3 tangent : TEXCOORD3;
};

struct VS_OUTPUT_STATIC_PRT_TERRAIN
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

struct VS_OUTPUT_ACTIVE_CAMO_TERRAIN
{
	float4 position : SV_Position;
	float4 texcoord : TEXCOORD1;
	float4 camo_param : TEXCOORD;
};

struct VS_OUTPUT_DYNAMIC_LIGHT_TERRAIN
{
	float4 position : SV_Position;
	float2 texcoord : TEXCOORD0;
	float3 normal : TEXCOORD1;
	float3 binormal : TEXCOORD2;
	float3 tangent : TEXCOORD3;
	float3 camera_dir : TEXCOORD4;
	float4 shadowmap_texcoord : TEXCOORD5;
};


struct VS_OUTPUT_LIGHTMAP_DEBUG_MODE_TERRAIN
{
	float4 position : SV_Position;
	float2 lightmap_texcoord : TEXCOORD;
	float3 normal : TEXCOORD1;
	float2 texcoord : TEXCOORD2;
	float3 tangent : TEXCOORD3;
	float3 binormal : TEXCOORD4;
	float3 camera_dir : TEXCOORD5;
};


struct VS_OUTPUT_STATIC_SH_TERRAIN
{
	float4 position : SV_Position;
	float3 texcoord : TEXCOORD; //z is used for the angle of the dominant light dir?
	float3 normal : TEXCOORD3; 
	float3 binormal : TEXCOORD4;
	float3 tangent : TEXCOORD5;
	float3 camera_dir : TEXCOORD6;
	float3 extinction_factor : COLOR;
	float3 sky_radiance : COLOR1;
};

struct VS_OUTPUT_PER_PIXEL_TERRAIN
{
	float4 position : SV_Position;
	float2 texcoord : TEXCOORD;
	float3 normal : TEXCOORD3;
	float3 binormal : TEXCOORD4;
	float3 tangent : TEXCOORD5;
	float2 lightmap_texcoord : TEXCOORD6_CENTROID;
	float3 camera_dir : TEXCOORD7;
	float3 extinction_factor : COLOR;
	float3 sky_radiance : COLOR1;
};

struct VS_OUTPUT_PER_VERTEX_TERRAIN
{
	float4 position : SV_Position;
	float4 texcoord : TEXCOORD;
	float3 camera_dir : TEXCOORD1;
	float3 tangent : TEXCOORD2;
	float3 normal : TEXCOORD3;
	float3 binormal : TEXCOORD4;
	
	float4 color1 : TEXCOORD5;
	float4 color2 : TEXCOORD6;
	float4 color3 : TEXCOORD7;
	float3 color4 : TEXCOORD8;
	
	float4 extinction_factor : COLOR;
};

#endif