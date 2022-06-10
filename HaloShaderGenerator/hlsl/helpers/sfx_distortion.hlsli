#ifndef _SFX_DISTORTION_HLSLI
#define _SFX_DISTORTION_HLSLI

#ifndef calc_distortion
#define calc_distortion calc_distortion_rigid
#endif

#include "../vertices/vertices.hlsli"
#include "input_output.hlsli"
#include "..\helpers\vertex_shader_helper.hlsli"

// TODO: clean this up with more generic functions, skinned uses the boolean squished thing

#define UPDATED_SFX_DISTORT 1

uniform float distort_fadeoff;

void calc_distortion_world(WORLD_VERTEX input, out VS_OUTPUT_SFX_DISTORT output)
{
	float4 world_position = float4(input.position.xyz, 1.0);
    output.position = mul(world_position, View_Projection);
	
	if (vertextype == k_vertextype_skinned)
		calculate_z_squish_2(output.position);
	else
		calculate_z_squish(output.position);
	
	float3 camera_dir = normalize(world_position.xyz - Camera_Position);
	float n_dot_c = dot(input.normal.xyz, camera_dir);
	float distortion_factor = min(abs(n_dot_c), 1.0);
	
#ifdef UPDATED_SFX_DISTORT
    output.distortion = saturate(output.position.w / distort_fadeoff) + 1.0f;
#else
    output.distortion = distortion_factor * distortion_factor * (3.0 - 2.0 * distortion_factor);
#endif
	
    output.texcoord.z = output.position.w;
	output.texcoord.xy = input.texcoord.xy;
	output.texcoord.w = 1.0;
}

void calc_distortion_rigid(RIGID_VERTEX input, out VS_OUTPUT_SFX_DISTORT output)
{
	float3x3 node_transformation = float3x3(Nodes[0].xyz, Nodes[1].xyz, Nodes[2].xyz);
	float4x4 v_node_transformation = float4x4(Nodes[0], Nodes[1], Nodes[2], float4(0, 0, 0, 0));
	
	float2 texcoord = calculate_texcoord(input.texcoord);
	float3 normal = transform_vector(input.normal.xyz, node_transformation);
	
	float4 world_position = float4(decompress_vertex_position(input.position.xyz), 1.0);
	world_position.xyz = mul(world_position.xyzw, transpose(v_node_transformation)).xyz;
	
    output.position = mul(world_position, View_Projection);
    calculate_z_squish(output.position);
	
	float3 camera_dir = normalize(world_position.xyz - Camera_Position);
	float n_dot_c = dot(normal, camera_dir);
	float distortion_factor = min(abs(n_dot_c), 1.0);
	
#ifdef UPDATED_SFX_DISTORT
    output.distortion = saturate(output.position.w / distort_fadeoff) + 1.0f;
#else
    output.distortion = distortion_factor * distortion_factor * (3.0 - 2.0 * distortion_factor);
#endif
	
	output.texcoord.xy = texcoord;
    output.texcoord.z = output.position.w;
	output.texcoord.w = 1.0;
}

void calc_distortion_skinned(SKINNED_VERTEX input, out VS_OUTPUT_SFX_DISTORT output)
{
	int4 indices = int4(3 * floor(input.node_indices)); // offset into the matrix by 3
	float4 weights = input.node_weights * (1.0 / dot(input.node_weights, 1)); // make sure weights sum to 1
	// compute transformation matrix for weighted vertices
	float4 basis1 = weights.x * Nodes[indices.x + 0] + weights.y * Nodes[indices.y + 0] + weights.z * Nodes[indices.z + 0] + weights.w * Nodes[indices.w + 0];
	float4 basis2 = weights.x * Nodes[indices.x + 1] + weights.y * Nodes[indices.y + 1] + weights.z * Nodes[indices.z + 1] + weights.w * Nodes[indices.w + 1];
	float4 basis3 = weights.x * Nodes[indices.x + 2] + weights.y * Nodes[indices.y + 2] + weights.z * Nodes[indices.z + 2] + weights.w * Nodes[indices.w + 2];
	
	float3x3 node_transformation = float3x3(basis1.xyz, basis2.xyz, basis3.xyz);
	float4x4 v_node_transformation = float4x4(basis1, basis2, basis3, float4(0, 0, 0, 0));
	
	float2 texcoord = calculate_texcoord(input.texcoord);
	float3 normal = transform_vector(input.normal.xyz, node_transformation);
	
	float4 world_position = float4(decompress_vertex_position(input.position.xyz), 1.0);
	world_position.xyz = mul(world_position.xyzw, transpose(v_node_transformation)).xyz;
	
    output.position = mul(world_position, View_Projection);
    calculate_z_squish(output.position);
	
	float3 camera_dir = normalize(world_position.xyz - Camera_Position);
	float n_dot_c = dot(normal, camera_dir);
	float distortion_factor = min(abs(n_dot_c), 1.0);
	
#ifdef UPDATED_SFX_DISTORT
    output.distortion = saturate(output.position.w / distort_fadeoff) + 1.0f;
#else
    output.distortion = distortion_factor * distortion_factor * (3.0 - 2.0 * distortion_factor);
#endif
	
	output.texcoord.xy = texcoord;
    output.texcoord.z = output.position.w;
	output.texcoord.w = 1.0;
}

#endif
