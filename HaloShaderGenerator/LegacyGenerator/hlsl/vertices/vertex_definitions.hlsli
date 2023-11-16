#ifndef _VERTEX_DEFINITIONS_HLSLI
#define _VERTEX_DEFINITIONS_HLSLI

#include "../registers/vertex_shader.hlsli"
#include "../helpers/transform_math.hlsli"

/* 
*  Vertex definition for vertex shader input (use POSITION, not SV_position)
*/

struct WORLD_VERTEX
{
	float4 position : POSITION;
	float4 texcoord : TEXCOORD;
	float4 normal : NORMAL;
	float4 tangent : TANGENT;
	float4 binormal : BINORMAL;
};

struct SKINNED_VERTEX
{
	float4 position : POSITION;
	float4 texcoord : TEXCOORD;
	float4 normal : NORMAL;
	float4 tangent : TANGENT;
	float4 binormal : BINORMAL;
	float4 node_indices : BLENDINDICES;
	float4 node_weights : BLENDWEIGHT;
};

struct RIGID_VERTEX
{
	float4 position : POSITION;
	float4 texcoord : TEXCOORD;
	float4 normal : NORMAL;
	float4 tangent : TANGENT;
	float4 binormal : BINORMAL;
};

struct FLATWORLD_VERTEX
{
    float4 position : POSITION;
    float4 texcoord : TEXCOORD;
};

struct FLATSKINNED_VERTEX
{
    float4 position : POSITION;
    float4 texcoord : TEXCOORD;
    float4 node_indices : BLENDINDICES;
    float4 node_weights : BLENDWEIGHT;
};

struct FLATRIGID_VERTEX
{
    float4 position : POSITION;
    float4 texcoord : TEXCOORD;
};

// Per vertex lighting data, sh coefficients
struct STATIC_PER_VERTEX_DATA
{
	float4 color_1 : TEXCOORD3;
	float4 color_2 : TEXCOORD4;
	float4 color_3 : TEXCOORD5;
	float4 color_4 : TEXCOORD6;
	float4 color_5 : TEXCOORD7;
};

// per pixel lighting data stored in the lightmap, texcoord to access the lightmap
struct STATIC_PER_PIXEL_DATA
{
	float2 lightmap_texcoord : TEXCOORD1;
};

// geometry color to replace GI
struct STATIC_PER_VERTEX_COLOR_DATA
{
	float4 color : TEXCOORD3;
};

// order 1 SH coefficient for PRT
struct AMBIENT_PRT
{
	float coefficient : BLENDWEIGHT1;
};

// order 2 SH coefficients for PRT
struct LINEAR_PRT
{
	float4 coefficients : BLENDWEIGHT1;
};

// order 3 SH coefficients for PRT
struct QUADRATIC_PRT
{
	float3 coefficients1 : BLENDWEIGHT1;
	float3 coefficients2 : BLENDWEIGHT2;
	float3 coefficients3 : BLENDWEIGHT3;
};

#endif