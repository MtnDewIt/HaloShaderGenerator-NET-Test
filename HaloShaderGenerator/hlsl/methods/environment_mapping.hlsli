#ifndef _ENVIRONMENT_MAPPING_HLSLI
#define _ENVIRONMENT_MAPPING_HLSLI

#include "../helpers/types.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/sh.hlsli"
#include "../registers/global_parameters.hlsli"
#include "../helpers/input_output.hlsli"

uniform float3 env_tint_color;
uniform float env_roughness_scale;
uniform samplerCUBE dynamic_environment_map_0;
uniform samplerCUBE dynamic_environment_map_1;
uniform sampler flat_environment_map;

uniform float3 flat_envmap_matrix_x;
uniform float3 flat_envmap_matrix_y;
uniform float3 flat_envmap_matrix_z;
uniform float hemisphere_percentage;
uniform float4 env_bloom_override;
uniform float env_bloom_override_intensity;

float get_lod_level(in float3 reflect_dir)
{
	float3 m_reflect_dir = float3(reflect_dir.x, -reflect_dir.y, reflect_dir.z);
    
	float3 x_derivative = ddx(m_reflect_dir);
	float x_length = length(x_derivative);
	
	float3 y_derivative = ddy(m_reflect_dir);
	float y_length = length(y_derivative);
	
	float max_length = max(x_length, y_length);
    
	float lod_level = sqrt(max_length) * 6 - 0.6;
	
	return lod_level; 
}

void envmap_type_none(
in ENVIRONMENT_MAPPING_COMMON env_mapping_common_data,
inout float3 diffuse,
out float4 unknown_output)
{
	unknown_output = 0;
}

void envmap_type_per_pixel(
in ENVIRONMENT_MAPPING_COMMON env_mapping_common_data,
inout float3 diffuse,
out float4 unknown_output)
{
	float3 envmap_texcoord = float3(env_mapping_common_data.reflect_dir.x, -env_mapping_common_data.reflect_dir.y, env_mapping_common_data.reflect_dir.z);
	float4 envmap_sample = texCUBE(environment_map, envmap_texcoord);
	
	unknown_output.rgb = env_tint_color.rgb * env_mapping_common_data.specular_coefficient;
	unknown_output.a = env_mapping_common_data.specular_exponent * env_roughness_scale;
	
	float3 environment_color = (envmap_sample.rgb * env_mapping_common_data.specular_coefficient * env_mapping_common_data.env_area_specular) * env_tint_color * envmap_sample.a;
	diffuse += environment_color;
}

// on x360, per_pixel used a lod of 0.0f on the cube tex. on pc this was never used - our per_pixel functions the same as per_pixel_mip.
void envmap_type_per_pixel_mip(
in ENVIRONMENT_MAPPING_COMMON env_mapping_common_data,
inout float3 diffuse,
out float4 unknown_output)
{
    envmap_type_per_pixel(env_mapping_common_data, diffuse, unknown_output);
}

void envmap_type_dynamic(
in ENVIRONMENT_MAPPING_COMMON env_mapping_common_data,
inout float3 diffuse,
out float4 unknown_output)
{

	float lod_level = get_lod_level(env_mapping_common_data.reflect_dir);

	float roughness_level = env_mapping_common_data.specular_exponent * env_roughness_scale;

	float4 dynamic_envmap_texcoord;
	dynamic_envmap_texcoord.w = max(lod_level, 4 * roughness_level);
	dynamic_envmap_texcoord.xyz = env_mapping_common_data.reflect_dir * float3(1, -1, 1);
	
	float4 dynamic_environment_map_0_sample = texCUBElod(dynamic_environment_map_0, dynamic_envmap_texcoord);
	float4 dynamic_environment_map_1_sample = texCUBElod(dynamic_environment_map_1, dynamic_envmap_texcoord);
	
	unknown_output.rgb = env_tint_color.rgb * env_mapping_common_data.specular_coefficient;
	unknown_output.a = roughness_level;
	
	float3 env_color_0 = dynamic_environment_map_0_sample.rgb * dynamic_environment_map_0_sample.w;
	env_color_0 *= dynamic_environment_blend.rgb;
	
	float3 env_color_1 = dynamic_environment_map_1_sample.rgb * dynamic_environment_map_1_sample.w;
	env_color_1 *= 256;
	env_color_1 *= (1.0f - dynamic_environment_blend.rgb);
	env_color_0 *= 256;
	float3 environment_color = env_color_0 + env_color_1;
	environment_color *= env_mapping_common_data.specular_coefficient;
	environment_color *= env_tint_color.xyz;
	environment_color *= env_mapping_common_data.env_area_specular;

	diffuse += environment_color;
}

void envmap_type_from_flat_texture(
in ENVIRONMENT_MAPPING_COMMON env_mapping_common_data,
inout float3 diffuse,
out float4 unknown_output)
{
    float3 reflect = env_mapping_common_data.reflect_dir * float3(1, -1, 1);
	
    float3 env_reflect;
    env_reflect.x = dot(reflect, flat_envmap_matrix_x);
    env_reflect.y = dot(reflect, flat_envmap_matrix_y);
    env_reflect.z = dot(reflect, flat_envmap_matrix_z);
	
    float dot_xy = dot(env_reflect.xy, env_reflect.xy);
    float curvature = (env_reflect.z + 1.0f) / hemisphere_percentage;
    curvature = sqrt(abs(curvature));
    env_reflect.xy *= curvature;
    
    float2 env_tex = env_reflect.xy / sqrt(dot_xy) + 1.0f;
    env_tex *= 0.5f;
    
    float3 flat_envmap_sample = tex2D(flat_environment_map, env_tex).rgb;
    
    float3 flat_env_constants = float3(0.114f, 0.299f, 0.587f);
    float flat_env_bloom_alpha = dot(flat_envmap_sample.bgr, flat_env_constants) - env_bloom_override.a;
    flat_env_bloom_alpha = max(flat_env_bloom_alpha, 0);
    
    float3 environment_color = flat_envmap_sample.rgb; 
    environment_color *= env_mapping_common_data.specular_coefficient;
    environment_color *= env_mapping_common_data.env_area_specular;
    environment_color *= env_tint_color;
    environment_color *= env_bloom_override.rgb;
    environment_color *= flat_env_bloom_alpha; 
    environment_color *= env_bloom_override_intensity;
    diffuse += environment_color;
    
    unknown_output.rgb = env_tint_color.rgb * env_mapping_common_data.specular_coefficient;
    unknown_output.a = env_mapping_common_data.specular_exponent;
}

void envmap_type_custom_map(
in ENVIRONMENT_MAPPING_COMMON env_mapping_common_data,
inout float3 diffuse,
out float4 unknown_output)
{
    float lod_level = get_lod_level(env_mapping_common_data.reflect_dir);

    float roughness_level = env_mapping_common_data.specular_exponent * env_roughness_scale;

    float4 envmap_texcoord;
    envmap_texcoord.w = max(lod_level, 4 * roughness_level);
    envmap_texcoord.xyz = env_mapping_common_data.reflect_dir * float3(1, -1, 1);
	
    float4 envmap_sample = texCUBElod(environment_map, envmap_texcoord);
	
    unknown_output.rgb = env_tint_color.rgb * env_mapping_common_data.specular_coefficient;
    unknown_output.a = roughness_level;
	
    float3 env_color_0 = envmap_sample.rgb;
    env_color_0 *= 256;
	
    float3 environment_color = env_color_0;
    environment_color *= env_mapping_common_data.specular_coefficient;
    environment_color *= env_mapping_common_data.env_area_specular;
    environment_color *= env_tint_color.xyz;
    environment_color *= envmap_sample.w;

    diffuse += environment_color;
}

#ifndef envmap_type
#define envmap_type envmap_type_none
#endif

#endif
