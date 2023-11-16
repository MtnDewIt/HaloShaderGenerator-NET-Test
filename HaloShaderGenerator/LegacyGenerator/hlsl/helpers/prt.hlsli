#ifndef _PRT_HLSL
#define _PRT_HLSL

#include "../registers/vertex_shader.hlsli"
#include "math.hlsli"
#include "../vertices/vertices.hlsli"
#include "../vertices/vertex_definitions.hlsli"
#include "vertex_shader_helper.hlsli"

#ifndef transform_dominant_light
#define transform_dominant_light transform_dominant_light_rigid
#endif

#if vertextype == k_vertextype_world
#define get_local_transformation get_local_transformation_world
#elif vertextype == k_vertextype_rigid
#define get_local_transformation get_local_transformation_rigid
#elif vertextype == k_vertextype_skinned
#define get_local_transformation get_local_transformation_skinned
#else
#define get_local_transformation get_local_transformation_world
#endif

// normalization factor for ambient and directional light
#define ambient_light_norm 3.54490770
#define dir_light_norm 2.956793086
// cosine lobe zonal harmonic coefficients for lambert diffuse BDRF projected into sh basis
#define h_0_0 0.282094806
#define h_1_0 0.325735007
#define h_2_0 0.157695782
// convolution constant per band
#define conv_0 3.54490733
#define conv_1 2.04665342
#define conv_2 1.58533092
// util
#define third 0.333333333

float4 calculate_ambient_radiance_vector(float coefficient, float3 normal)
{
	float diffuse_prt, specular_prt, env_prt, angle_attenuation;

	float sh_0 = dot(v_lighting_constant_0.xyz, third);
	float diffuse_reflectance = max(sh_0 * h_0_0, 0.01);
	float prt = max(sh_0 * coefficient, 0.01);
	diffuse_prt = prt / diffuse_reflectance;
	
	float3 dominant_light_direction = normalize(v_lighting_constant_1.xyz + v_lighting_constant_2.xyz + v_lighting_constant_3.xyz);
	float n_dot_l = dot(normal, -dominant_light_direction);
	
	angle_attenuation = min(n_dot_l, prt);
	specular_prt = prt;
	env_prt = ambient_light_norm * coefficient;
	
	return float4(diffuse_prt, specular_prt, env_prt, angle_attenuation);
}

float3 transform_dominant_light_rigid(RIGID_VERTEX vert, float3 input)
{
	input *= third;
	return input.x * Nodes[0].yzx + input.y * Nodes[1].yzx + input.z * Nodes[2].yzx;
}

float3 transform_dominant_light_world(WORLD_VERTEX vert, float3 input)
{
	//float4 r2 = input.xxyy * float4(0, third, third, 0);
	//r2.xyz = r2.xxy + r2.zww;
	//return input.z * float3(0, third, 0) + r2.xyz;
	return input * third;
}

float3 transform_dominant_light_skinned(SKINNED_VERTEX vert, float3 input)
{
	int4 indices = int4(3 * floor(vert.node_indices));
	float4 weights = vert.node_weights * (1.0 / dot(vert.node_weights, 1));
	// compute transformation matrix for weighted vertices
	float4 basis1 = weights.x * Nodes[indices.x + 0] + weights.y * Nodes[indices.y + 0] + weights.z * Nodes[indices.z + 0] + weights.w * Nodes[indices.w + 0];
	float4 basis2 = weights.x * Nodes[indices.x + 1] + weights.y * Nodes[indices.y + 1] + weights.z * Nodes[indices.z + 1] + weights.w * Nodes[indices.w + 1];
	float4 basis3 = weights.x * Nodes[indices.x + 2] + weights.y * Nodes[indices.y + 2] + weights.z * Nodes[indices.z + 2] + weights.w * Nodes[indices.w + 2];
	input *= third;
	return input.x * basis1.yzx + input.y * basis2.yzx + input.z * basis3.yzx;
}

void get_local_transformation_rigid(input_vertex_format vert, out float3 rotate_x, out float3 rotate_y, out float3 rotate_z)
{
	rotate_x = float3(Nodes[0].x, Nodes[1].x, Nodes[2].x);
	rotate_y = float3(Nodes[0].y, Nodes[1].y, Nodes[2].y);
	rotate_z = float3(Nodes[0].z, Nodes[1].z, Nodes[2].z);
}

void get_local_transformation_skinned(SKINNED_VERTEX vert, out float3 rotate_x, out float3 rotate_y, out float3 rotate_z)
{
	int4 indices = int4(3 * floor(vert.node_indices));
	float4 weights = vert.node_weights * (1.0 / dot(vert.node_weights, 1));
	float4 basis1 = weights.x * Nodes[indices.x + 0] + weights.y * Nodes[indices.y + 0] + weights.z * Nodes[indices.z + 0] + weights.w * Nodes[indices.w + 0];
	float4 basis2 = weights.x * Nodes[indices.x + 1] + weights.y * Nodes[indices.y + 1] + weights.z * Nodes[indices.z + 1] + weights.w * Nodes[indices.w + 1];
	float4 basis3 = weights.x * Nodes[indices.x + 2] + weights.y * Nodes[indices.y + 2] + weights.z * Nodes[indices.z + 2] + weights.w * Nodes[indices.w + 2];
	rotate_x = float3(basis1.x, basis2.x, basis3.x);
	rotate_y = float3(basis1.y, basis2.y, basis3.y);
	rotate_z = float3(basis1.z, basis2.z, basis3.z);
}

void get_local_transformation_world(input_vertex_format vert, out float3 rotate_x, out float3 rotate_y, out float3 rotate_z)
{
	rotate_x = float3(1, 0, 0);
	rotate_y = float3(0, 1, 0);
	rotate_z = float3(0, 0, 1);
}

float4 calculate_linear_radiance_vector(input_vertex_format vert, float4 coefficients, float3 normal)
{
	float c2 = 0.511664f;
	float c4 = 0.886227f;
	float3 dominant_light_direction;
	float diffuse_prt, specular_prt, env_prt, angle_attenuation;

	float4 prt_coefficients = 2.0f * coefficients - 1.0f;
	
	env_prt = ambient_light_norm * prt_coefficients.x; // env specular prt

	float sh_0 = dot(v_lighting_constant_0.xyz, third);
	dominant_light_direction = v_lighting_constant_1.xyz + v_lighting_constant_3.xyz + v_lighting_constant_2.xyz;
	float3 n_dld = normalize(dominant_light_direction);
	float n_dot_dld = dot(normal, -n_dld);
	
	float3 sh_312 = third * dominant_light_direction;
	float x1 = dot(normal, sh_312);
	
	float3 model_space_dld = transform_dominant_light(vert, dominant_light_direction);
	model_space_dld = model_space_dld * float3(1, -1, 1);
	float prt = dot(float4(sh_0, model_space_dld), prt_coefficients);
	
	x1 = x1 * (-2.0f * c2);
	float diffuse_reflectance = sh_0 * c4 + x1;
	
	prt = max(prt, 0.01);
	angle_attenuation = min(n_dot_dld, prt);
	diffuse_reflectance = diffuse_reflectance / PI;
	diffuse_reflectance = max(diffuse_reflectance, 0.01);
	diffuse_prt = prt / diffuse_reflectance;
	specular_prt = prt;

	return float4(diffuse_prt, specular_prt, env_prt, angle_attenuation);
}

float4 calculate_quadratic_radiance_vector(input_vertex_format vert, QUADRATIC_PRT prt, float3 normal)
{
	float diffuse_prt, specular_prt, env_prt, angle_attenuation;
	float c2 = 0.511664f;
	float c4 = 0.886227f;
	float c5 = 0.429043f;
	
	env_prt = ambient_light_norm * prt.coefficients1.x;
	
	float sh_0 = dot(v_lighting_constant_0.xyz, third);
	float3 dominant_light_direction = v_lighting_constant_1.xyz + v_lighting_constant_2.xyz + v_lighting_constant_3.xyz;
	float3 sh_312 = third * dominant_light_direction;
	float3 sh_457 = v_lighting_constant_4.xyz + v_lighting_constant_5.xyz + v_lighting_constant_6.xyz;
	sh_457 *= third;
	float4 sh_8866 = v_lighting_constant_7 + v_lighting_constant_8 + v_lighting_constant_9;
	sh_8866 *= third;
	
	float4 local_sh;
	float3 rotate_x, rotate_y, rotate_z;
	float4 quadratic_a, quadratic_b;
	get_local_transformation(vert, rotate_x, rotate_y, rotate_z);
	
	quadratic_a.xyz = rotate_y * rotate_x.yxz + rotate_x * rotate_y.yzx;
	quadratic_b.xyz = rotate_y * rotate_x;
	local_sh.x = -dot(quadratic_a.xyz, sh_457) - dot(quadratic_b.xyz, sh_457);
	
	quadratic_a.xyz = rotate_y.yzx * rotate_z + rotate_y * rotate_z.yzx;
	quadratic_b.xyz = rotate_y * rotate_z;
	local_sh.y = dot(quadratic_a.xyz, sh_457) + dot(quadratic_b.xyz, sh_8866.xyz);
	
	quadratic_a.xyz = rotate_x.yzx * rotate_z + rotate_x * rotate_z.yzx;
	quadratic_b.xyz = rotate_x * rotate_z;
	local_sh.w = dot(quadratic_a.xyz, sh_457) + dot(quadratic_b.xyz, sh_8866.xyz);
	
	quadratic_a.xyz = rotate_z * rotate_z * (-SQRT3);
	quadratic_b = float4(rotate_z * rotate_z, 1.0f / 3.0f) * 0.5f * (-SQRT3);
	local_sh.z = dot(quadratic_a.xyz, sh_457) + dot(quadratic_b, sh_8866);
	
	quadratic_a.xyz = (rotate_x * rotate_x - rotate_y * rotate_y) * 0.5;
	quadratic_b.xyz = rotate_y * rotate_y.yzx + rotate_x * rotate_x.yzx;
	float unknown_local = -dot(quadratic_a.xyz, sh_457) - dot(quadratic_b.yzx, sh_457);
	
	float x1 = dot(normal, sh_312);
	float3 a = normal.yzx * normal.xyz;
	float x2 = dot(a, sh_457);
	float4 b = float4(normal * normal, third);
	float x3 = dot(b, sh_8866);

	float diffuse_reflectance = sh_0 * c4 + x1 * (-2.0f * c2) + (-2.f * c5) * x2 - c5 * x3;
	diffuse_reflectance = diffuse_reflectance / PI;
	diffuse_reflectance = max(diffuse_reflectance, 0.01);
	
	float3 model_space_dld = transform_dominant_light(vert, dominant_light_direction);
	model_space_dld = model_space_dld * float3(1, -1, 1);
	
	float radiance_transfer = dot(float4(sh_0, model_space_dld), float4(prt.coefficients1, prt.coefficients2.x));

	float4 model_sh_4567 = float4(prt.coefficients2.yz, prt.coefficients3.xy);
	radiance_transfer += dot(local_sh, model_sh_4567);
	
	
	radiance_transfer += unknown_local * prt.coefficients3.z;
	
	float3 n_dld = normalize(dominant_light_direction);
	float n_dot_dld = dot(normal, -n_dld);
	
	radiance_transfer = max(0.01, radiance_transfer);
	angle_attenuation = min(n_dot_dld, radiance_transfer);
	
	diffuse_prt = radiance_transfer / diffuse_reflectance;
	specular_prt = radiance_transfer;

	return float4(diffuse_prt, specular_prt, env_prt, angle_attenuation);
}


#endif
