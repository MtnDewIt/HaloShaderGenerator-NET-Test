#ifndef _VERTICES_HLSLI
#define _VERTICES_HLSLI

#include "../registers/vertex_shader.hlsli"
#include "../helpers/transform_math.hlsli"
#include "vertex_definitions.hlsli"

void calc_vertex_transform_rigid(RIGID_VERTEX input, out float4 world_position, out float4 screen_position, out float3 normal, out float3 tangent, out float3 binormal, out float2 texcoord)
{
	float3x3 node_transformation = float3x3(Nodes[0].xyz, Nodes[1].xyz, Nodes[2].xyz);
	float4x4 v_node_transformation = float4x4(Nodes[0], Nodes[1], Nodes[2], float4(0, 0, 0, 0));
	texcoord = calculate_texcoord(input.texcoord);
	normal = transform_vector(input.normal.xyz, node_transformation);
	binormal = transform_vector(transform_binormal(input.normal.xyz, input.tangent.xyz, input.binormal.xyz), node_transformation);
	tangent = transform_vector(input.tangent.xyz, node_transformation);
	world_position = float4(decompress_vertex_position(input.position.xyz), 1.0);
	world_position.xyz = mul(world_position.xyzw, transpose(v_node_transformation)).xyz;
	screen_position = calculate_screenspace_position(world_position);
}

void calc_vertex_transform_skinned(SKINNED_VERTEX input, out float4 world_position, out float4 screen_position, out float3 normal, out float3 tangent, out float3 binormal, out float2 texcoord)
{
	texcoord.xy = calculate_texcoord(input.texcoord);
	float4 weights = input.node_weights * (1.0 / dot(input.node_weights, 1)); // make sure weights sum to 1
	int4 indices = int4(3 * floor(input.node_indices)); // offset into the matrix by 3
	// compute transformation matrix for weighted vertices
	float4 basis1 = Nodes[indices.x + 0] * weights.x + Nodes[indices.y + 0] * weights.y + Nodes[indices.z + 0] * weights.z + Nodes[indices.w + 0] * weights.w;
	float4 basis2 = Nodes[indices.x + 1] * weights.x + Nodes[indices.y + 1] * weights.y + Nodes[indices.z + 1] * weights.z + Nodes[indices.w + 1] * weights.w;
	float4 basis3 = Nodes[indices.x + 2] * weights.x + Nodes[indices.y + 2] * weights.y + Nodes[indices.z + 2] * weights.z + Nodes[indices.w + 2] * weights.w;
	
	float3x3 node_transformation = float3x3(basis1.xyz, basis2.xyz, basis3.xyz);
	float4x4 v_node_transformation = float4x4(basis1, basis2, basis3, float4(0, 0, 0, 0));

	normal.xyz = transform_vector(input.normal.xyz, node_transformation);
	binormal.xyz = transform_vector(input.binormal.xyz, node_transformation);
	tangent.xyz = transform_vector(input.tangent.xyz, node_transformation);
	world_position = float4(decompress_vertex_position(input.position.xyz), 1.0);
	world_position.xyz = mul(world_position.xyzw, transpose(v_node_transformation)).xyz;
	screen_position = calculate_screenspace_position(world_position);
}

void calc_vertex_transform_world(WORLD_VERTEX input, out float4 world_position, out float4 screen_position, out float3 normal, out float3 tangent, out float3 binormal, out float2 texcoord)
{
	binormal.xyz = transform_binormal(input.normal.xyz, input.tangent.xyz, input.binormal.xyz);
	world_position = float4(input.position.xyz, 1.0);
	screen_position = calculate_screenspace_position(world_position);
	texcoord.xy = input.texcoord.xy;
	normal.xyz = input.normal.xyz;
	tangent.xyz = input.tangent.xyz;
}

void calc_vertex_transform_flatrigid(FLATRIGID_VERTEX input, out float4 world_position, out float4 screen_position, out float3 normal, out float3 tangent, out float3 binormal, out float2 texcoord)
{
    float3x3 node_transformation = float3x3(Nodes[0].xyz, Nodes[1].xyz, Nodes[2].xyz);
    float4x4 v_node_transformation = float4x4(Nodes[0], Nodes[1], Nodes[2], float4(0, 0, 0, 0));
    texcoord = calculate_texcoord(input.texcoord);
    world_position = float4(decompress_vertex_position(input.position.xyz), 1.0);
    world_position.xyz = mul(world_position.xyzw, transpose(v_node_transformation)).xyz;
    screen_position = calculate_screenspace_position(world_position);
	
    normal = 0;
    tangent = 0;
    binormal = 0;
}

void calc_vertex_transform_flatskinned(FLATSKINNED_VERTEX input, out float4 world_position, out float4 screen_position, out float3 normal, out float3 tangent, out float3 binormal, out float2 texcoord)
{
    texcoord.xy = calculate_texcoord(input.texcoord);
    float4 weights = input.node_weights * (1.0 / dot(input.node_weights, 1)); // make sure weights sum to 1
    int4 indices = int4(3 * floor(input.node_indices)); // offset into the matrix by 3
	// compute transformation matrix for weighted vertices
    float4 basis1 = Nodes[indices.x + 0] * weights.x + Nodes[indices.y + 0] * weights.y + Nodes[indices.z + 0] * weights.z + Nodes[indices.w + 0] * weights.w;
    float4 basis2 = Nodes[indices.x + 1] * weights.x + Nodes[indices.y + 1] * weights.y + Nodes[indices.z + 1] * weights.z + Nodes[indices.w + 1] * weights.w;
    float4 basis3 = Nodes[indices.x + 2] * weights.x + Nodes[indices.y + 2] * weights.y + Nodes[indices.z + 2] * weights.z + Nodes[indices.w + 2] * weights.w;
	
    float3x3 node_transformation = float3x3(basis1.xyz, basis2.xyz, basis3.xyz);
    float4x4 v_node_transformation = float4x4(basis1, basis2, basis3, float4(0, 0, 0, 0));

    world_position = float4(decompress_vertex_position(input.position.xyz), 1.0);
    world_position.xyz = mul(world_position.xyzw, transpose(v_node_transformation)).xyz;
    screen_position = calculate_screenspace_position(world_position);
	
    normal = 0;
    tangent = 0;
    binormal = 0;
}

void calc_vertex_transform_flatworld(FLATWORLD_VERTEX input, out float4 world_position, out float4 screen_position, out float3 normal, out float3 tangent, out float3 binormal, out float2 texcoord)
{
    world_position = float4(input.position.xyz, 1.0);
    screen_position = calculate_screenspace_position(world_position);
    texcoord.xy = input.texcoord.xy;
	
    normal = 0;
    tangent = 0;
    binormal = 0;
}

#ifndef calc_vertex_transform
#define calc_vertex_transform calc_vertex_transform_rigid
#endif

#ifndef input_vertex_format
#define input_vertex_format RIGID_VERTEX
#endif

#endif