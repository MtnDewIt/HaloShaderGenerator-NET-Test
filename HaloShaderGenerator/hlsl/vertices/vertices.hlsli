#ifndef _VERTICES_HLSLI
#define _VERTICES_HLSLI

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


struct VS_INPUT_RIGID_VERTEX_LINEAR_PRT
{
	float4 position : POSITION;
	float4 texcoord : TEXCOORD;
	float4 normal : NORMAL;
	float4 tangent : TANGENT;
	float4 binormal : BINORMAL;
	float4 coefficients : BLENDWEIGHT1;
};

struct VS_INPUT_RIGID_VERTEX_QUADRATIC_PRT
{
	float4 position : POSITION;
	float4 texcoord : TEXCOORD;
	float4 normal : NORMAL;
	float4 tangent : TANGENT;
	float4 binormal : BINORMAL;
	float3 coefficients1 : BLENDWEIGHT1;
	float3 coefficients2 : BLENDWEIGHT2;
	float3 coefficients3 : BLENDWEIGHT3;
};
#endif