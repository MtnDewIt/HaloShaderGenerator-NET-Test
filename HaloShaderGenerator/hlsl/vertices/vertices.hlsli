#ifndef _VERTICES_HLSLI
#define _VERTICES_HLSLI

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

void calc_vertex_transform_rigid(RIGID_VERTEX input, out float4 world_position, out float4 screen_position, out float3 normal, out float3 tangent, out float3 binormal, out float2 texcoord, out float3 camera_dir)
{
	float3x3 node_transformation = float3x3(nodes[0].xyz, nodes[1].xyz, nodes[2].xyz);
	float4x4 v_node_transformation = float4x4(nodes[0], nodes[1], nodes[2], float4(0, 0, 0, 0));
	texcoord = calculate_texcoord(input.texcoord);
	normal = transform_vector(input.normal.xyz, node_transformation);
	binormal = transform_vector(transform_binormal(input.normal.xyz, input.tangent.xyz, input.binormal.xyz), node_transformation);
	tangent = transform_vector(input.tangent.xyz, node_transformation);
	world_position = float4(decompress_vertex_position(input.position.xyz), 1.0);
	world_position.xyz = mul(v_node_transformation, world_position.xyzw).xyz;
	screen_position = calculate_screenspace_position(world_position);
	camera_dir = camera_position - world_position.xyz;
}

void calc_vertex_transform_skinned(SKINNED_VERTEX input, out float4 world_position, out float4 screen_position, out float3 normal, out float3 tangent, out float3 binormal, out float2 texcoord, out float3 camera_dir)
{
	texcoord.xy = calculate_texcoord(input.texcoord);
	int4 indices = int4(3 * floor(input.node_indices)); // offset into the matrix by 3
	float4 weights = input.node_weights * (1.0 / dot(input.node_weights, 1)); // make sure weights sum to 1
	// compute transformation matrix for weighted vertices
	float4 basis1 = weights.x * nodes[indices.x + 0] + weights.y * nodes[indices.y + 0] + weights.z * nodes[indices.z + 0] + weights.w * nodes[indices.w + 0];
	float4 basis2 = weights.x * nodes[indices.x + 1] + weights.y * nodes[indices.y + 1] + weights.z * nodes[indices.z + 1] + weights.w * nodes[indices.w + 1];
	float4 basis3 = weights.x * nodes[indices.x + 2] + weights.y * nodes[indices.y + 2] + weights.z * nodes[indices.z + 2] + weights.w * nodes[indices.w + 2];
	
	float3x3 node_transformation = float3x3(basis1.xyz, basis2.xyz, basis3.xyz);
	float4x4 v_node_transformation = float4x4(basis1, basis2, basis3, float4(0, 0, 0, 0));

	normal.xyz = transform_vector(input.normal.xyz, node_transformation);
	binormal.xyz = transform_vector(input.binormal.xyz, node_transformation);
	tangent.xyz = transform_vector(input.tangent.xyz, node_transformation);
	world_position = float4(decompress_vertex_position(input.position.xyz), 1.0);
	world_position.xyz = mul(v_node_transformation, world_position.xyzw).xyz;
	screen_position = calculate_screenspace_position(world_position);
	camera_dir = camera_position - world_position.xyz;
}

void calc_vertex_transform_world(WORLD_VERTEX input, out float4 world_position, out float4 screen_position, out float3 normal, out float3 tangent, out float3 binormal, out float2 texcoord, out float3 camera_dir)
{
	binormal.xyz = transform_binormal(input.normal.xyz, input.tangent.xyz, input.binormal.xyz);
	world_position = float4(input.position.xyz, 1.0);
	screen_position = calculate_screenspace_position(world_position);
	camera_dir = camera_position - world_position.xyz;
	texcoord.xy = input.texcoord.xy;
	normal.xyz = input.normal.xyz;
	tangent.xyz = input.tangent.xyz;
}

#ifndef calc_vertex_transform
#define calc_vertex_transform calc_vertex_transform_rigid
#endif

#ifndef input_vertex_format
#define input_vertex_format RIGID_VERTEX
#endif

#endif