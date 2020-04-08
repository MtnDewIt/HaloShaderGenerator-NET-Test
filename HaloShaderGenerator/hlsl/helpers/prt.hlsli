#ifndef _PRT_HLSL
#define _PRT_HLSL

#include "../registers/vertex_shader.hlsli"
#include "math.hlsli"

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

/*
float4 calculate_linear_radiance_vector(float4 coefficients, float3 normal)
{
	float4 result;
	
	float tempx, tempy;
	float4 r0;
	float4 r1;
	float4 r2;
	float3 r3;
	
	result.z = 3.54490733 * (2 * coefficients.x - 1.0);
	r0.x = dot(v_lighting_constant_0.xyz, 0.333333333);
	float3 sum = v_lighting_constant_3.xyz + v_lighting_constant_1.xyz + v_lighting_constant_2.xyz;
	r3 = normalize(sum.xyz);
	r1.w = dot(normal, -r3);
	r2 = r2 * 0.333333333;
	r1.x = dot(r1, r2);
	r1.x = r1.x * -1.02332795;
	r1.x = r0.x * 0.886227012 + r1.x;
	r2.xyz = r2.x * nodes[0] + r2.y * nodes[1] + r2.z * nodes[2];
	r0.xyz = r2.xyz * float3(1, -1, 1);
	r2 = 2 * coefficients - 1.0;
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
*/


#endif
