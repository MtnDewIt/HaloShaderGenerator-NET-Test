#ifndef _PRT_HLSL
#define _PRT_HLSL

#include "../registers/vertex_shader.hlsli"
#include "math.hlsli"
#include "../vertices/vertices.hlsli"


#ifndef transform_unknown_vector
#define transform_unknown_vector transform_unknown_vector_rigid
#endif

// normalization factor for ambient and directional light
#define ambient_light_norm 3.54490770
#define dir_light_norm 2.956793086
// cosine lobe zonal harmonic coefficients
#define h_0_0 0.282094806
#define h_1_0 0.325735007
#define h_2_0 0.157695782
// convolution constant per band
#define conv_0 3.54490770
#define conv_1 2.04665342
#define conv_2 1.58533092

float4 calculate_ambient_radiance_vector(float coefficient, float3 normal)
{
	float4 result;
	float tempx, tempy;
	
	
	tempy = 0.333333333;
	tempx = dot(v_lighting_constant_0.xyz, tempy);
	tempy = tempx * coefficient;
	tempx = tempx * 0.282094806;
	tempy = max(tempy, 0.01);
	tempx = max(tempx, 0.01);
	tempx = 1.0 / tempx;
	
	result.x = tempx * tempy;
	float3 unknown_sh = normalize(v_lighting_constant_3.xyz + v_lighting_constant_1.xyz + v_lighting_constant_2.xyz);
	tempx = dot(normal, -unknown_sh);
	
	result.w = min(tempy, tempx);
	result.y = tempy;
	result.z = conv_0 * coefficient;
	
	return result;
}

float3 transform_unknown_vector_rigid(RIGID_VERTEX vert, float3 input)
{
	return input.x * nodes[0] + input.y * nodes[1] + input.z * nodes[2];
}

float3 transform_unknown_vector_world(WORLD_VERTEX vert, float3 input)
{
	return input;
}

float transform_unknown_vector_skinned(SKINNED_VERTEX vert, float3 input)
{
	int4 indices = int4(3 * floor(vert.node_indices)); // offset into the matrix by 3
	float4 weights = vert.node_weights * (1.0 / dot(vert.node_weights, 1)); // make sure weights sum to 1
	// compute transformation matrix for weighted vertices
	float4 basis1 = weights.x * nodes[indices.x + 0] + weights.y * nodes[indices.y + 0] + weights.z * nodes[indices.z + 0] + weights.w * nodes[indices.w + 0];
	float4 basis2 = weights.x * nodes[indices.x + 1] + weights.y * nodes[indices.y + 1] + weights.z * nodes[indices.z + 1] + weights.w * nodes[indices.w + 1];
	float4 basis3 = weights.x * nodes[indices.x + 2] + weights.y * nodes[indices.y + 2] + weights.z * nodes[indices.z + 2] + weights.w * nodes[indices.w + 2];
	return input.x * basis1 + input.y * basis2 + input.z * basis3;
}

float4 calculate_linear_radiance_vector(input_vertex_format vert, float4 coefficients, float3 normal)
{
	float4 result = float4(0, 0, 0, 0);

	float4 r0 = float4(0, 0, 0, 0);
	float4 r1 = float4(0, 0, 0, 0);
	float4 r2 = float4(0, 0, 0, 0);
	float3 r3 = float4(0, 0, 0, 0);

	result.z = 3.54490733 * (2.0 * coefficients.x - 1.0);
	r0.w = 0.33333333333;
	r0.x = dot(v_lighting_constant_0.xyz, 0.333333333);
	r2.xyz = v_lighting_constant_1.xyz + v_lighting_constant_3.xyz + v_lighting_constant_2.xyz;
	r3.xyz = normalize(r2);
	r1.w = dot(normal, -r3.xyz);
	r2.xyz = 0.333333333 * r2.xyz;
	r1.x = dot(normal, r2.xyz);
	r1.x = r1.x * -1.02332795;
	r1.x = r0.x * 0.886227012 + r1.x;
	r2.xyz = transform_unknown_vector(vert, r2.xyz);
	r0.yzw = r2.xyz * float3(1, -1, 1);
	r2 = coefficients * 2 - 1;
	r0.x = dot(r0, r2);
	r0.x = max(r0.x, 0.01);
	result.w = min(r0.x, r1.w);
	r0.y = r1.x * 0.318309873;
	r0.y = max(r0.y, 0.01);
	r0.y = 1.0 / r0.y;
	result.x = r0.x * r0.y;
	result.y = r0.x;
	
	return result;
}

#endif
