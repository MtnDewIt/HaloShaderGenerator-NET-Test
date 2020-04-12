#ifndef _PRT_HLSL
#define _PRT_HLSL

#include "../registers/vertex_shader.hlsli"
#include "math.hlsli"
#include "../vertices/vertices.hlsli"
#include "../vertices/prt.hlsli"

#ifndef transform_unknown_vector
#define transform_unknown_vector transform_unknown_vector_rigid
#endif

// cosine lobe 0 ? = 0.886226925 = sqrt(pi/4)
// cosine lobe 1 ? = 1.023326708 = sqrt(pi/3)
// cosine lobe 2 ? = 0.495415912 = sqrt(5pi/64)

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
	tempx = tempx * h_0_0;
	tempy = max(tempy, 0.01);
	tempx = max(tempx, 0.01);
	tempx = 1.0 / tempx;
	
	result.x = tempx * tempy;
	float3 unknown_sh = normalize(v_lighting_constant_3.xyz + v_lighting_constant_1.xyz + v_lighting_constant_2.xyz);
	tempx = dot(normal, -unknown_sh);
	
	result.w = min(tempy, tempx);
	result.y = tempy;
	result.z = conv_0 * coefficient; // 
	
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
	r2.xyz = v_lighting_constant_1.xyz + v_lighting_constant_3.xyz + v_lighting_constant_2.xyz; // dominant light direction perhaps? doesn't have the luminance scaling
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

float4 calculate_quadratic_radiance_vector(input_vertex_format vert, QUADRATIC_PRT prt, float3 normal)
{
	float4 result = float4(0, 0, 0, 0);
	float4 r0 = float4(0, 0, 0, 0);
	float4 r1 = float4(0, 0, 0, 0);
	float4 r2 = float4(0, 0, 0, 0);

	float4 r4 = float4(0, 0, 0, 0); // normal(x,yz) temp w
	float4 r5 = float4(0, 0, 0, 0);
	float4 r6 = float4(0, 0, 0, 0);
	float4 r7 = float4(0, 0, 0, 0);
	float4 r8 = float4(0, 0, 0, 0);
	float4 r9 = float4(0, 0, 0, 0);
	float4 r10 = float4(0, 0, 0, 0);
	float4 r11 = float4(0, 0, 0, 0);
	float3x3 node_transformation = float3x3(nodes[0].xyz, nodes[1].xyz, nodes[2].xyz);
	
	r0.xyz = float3(nodes[0].y, nodes[1].y, nodes[2].y);
	r2.xyz = float3(nodes[0].x, nodes[1].x, nodes[2].x);
	
	r7.xyz = 0.33333333333 * (v_lighting_constant_3.xyz + v_lighting_constant_2.xyz + v_lighting_constant_1.xyz);
	r0.w = 0.886227012 * dot(0.33333333333, v_atmosphere_constant_0.xyz) - dot(normal, r7.xyz) * 1.02332795;
	r5.yzw = transform_vector(r7.xyz, node_transformation) * float3(1, -1, 1);
	
	r7.xyz = prt.coefficients1.xyz;
	r7.w = prt.coefficients2.x;
	r1.w = prt.coefficients1.x * dot(0.33333333333, v_atmosphere_constant_0.xyz) + dot(r5.yzw, r7.yzw);
	
	r5.xyz = 0.33333333333 * (v_lighting_constant_4.xyz + v_lighting_constant_5.xyz + v_lighting_constant_6.xyz);
	r7.xyz = r0.xyz * r2.yzx;
	r7.xyz = r2.xyz * r0.yzx + r7.xyz;
	r2.w = dot(r7.xyz, r5.xyz);
	
	r7.xyz = 0.33333333333 * (v_lighting_constant_7.xyz + v_lighting_constant_8.xyz + v_lighting_constant_9.xyz);
	r8.xyz = r0.xyz * r2.xyz;
	r4.w = dot(r8.xyz, r7.xyz);
	
	r8.w = 0.33333333333;
	r8.xyz = normal * normal;
	r5.w = dot(r8, r7); //dot4
	r8.xyz = float3(nodes[0].z, nodes[1].z, nodes[2].z);
	r9.xyz = r0.yzx * r8.xyz;
	r9.xyz = r0.xyz * r8.yzx + r9.xyz;
	r6.w = dot(r9.xyz, r5.xyz);
	r9.xyz = normal.yzx * normal.xyz;
	r8.w = dot(r9.xyz, r5.xyz);
	r9.x = -r2.w -r4.w;
	r10.w = -0.288675129;
	r11.xyz = r8.xyz * r8.xyz;
	r10.xyz = r11.xyz * -0.866025388;
	r2.w = dot(r10.xyzw, r7.xyzw);
	r10.xyz = r2.xyz * r8.xyz;
	r4.w = dot(r10.xyz, r7.xyz);
	r10.xyz = r0.xyz * r8.xyz;
	r7.w = dot(r10.xyz, r7.xyz);
	r9.y = r6.w + r7.w;
	r10.xyz = r8.xyz * r8.yzx;
	r10.xyz = r10.xyz * -1.73205078;
	r6.w = dot(r10.xyz, r5.xyz);
	r9.z = r2.w + r6.w;
	r10.xyz = r2.yzx * r8.xyz;
	r8.xyz = r2.xyz * r8.yzx + r10.xyz;
	r2.w = dot(r8.xyz, r5.xyz);
	r9.w = r4.w + r2.w;
	r10.xy = prt.coefficients2.yz;
	r10.zw = prt.coefficients3.xy;
	r2.w = dot(r9, r10);
	r1.x = dot(r1.xyz, r7.xyz);
	r7.xyz = normalize(r6);
	r1.y = dot(normal, -r7.xyz);
	r0.w = r0.w - r8.w * 0.85808599;
	r0.xyz = r0 * r0.yzxw;
	r0.xyz = r2 * r2.yzxw - r0;
	r0.x = dot(r0.xyz, r5.xyz);
	r0.x = -r1.x - r0.x;
	r0.y = r1.w + r2.w;
	r0.x = r0.x * prt.coefficients3.z + r0.y;
	r0.y = r0.w - r5.w * 0.429042995;
	r0.x = max(r0.x, 0.01);
	result.w = min(r0.x, r1.y);
	r0.y = r1.x * 0.318309873;
	r0.y = max(r0.y, 0.01);
	r0.y = 1.0 / r0.y;
	result.x = r0.x * r0.y;
	result.y = r0.x;
	result.z = 3.54490733 * prt.coefficients1.x;
	return result;
}

#endif
