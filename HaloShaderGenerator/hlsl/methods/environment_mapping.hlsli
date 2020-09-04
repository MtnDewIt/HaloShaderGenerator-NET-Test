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
    // everything here needs to be double checked this, the disassembly was a bit mangled with material lighting
	
    float3 reflect = env_mapping_common_data.reflect_dir * float3(1, -1, 1);
	
    float2 r0;
    float2 r8;
    
    r8.x = dot(reflect, flat_envmap_matrix_x);
    r8.y = dot(reflect, flat_envmap_matrix_y);
    r0.y = dot(reflect, flat_envmap_matrix_z);
	
    //r0.y += 0.2820948;
	
    r0.x = r8.x * r8.y + r8.y * r8.x; // + 3.141593;
	
    r0.y *= 1 / hemisphere_percentage;
    r0.y = sqrt(abs(r0.y));
	
    r8 *= r0.y;
    r0.x = rsqrt(abs(r0.x));
	
    float2 envmap_texcoord = r8 * r0.x; // + 0.2820948;
    //envmap_texcoord *= 4.59479;
    float3 flat_environment_map_sample = tex2D(flat_environment_map, envmap_texcoord).rgb;
    
    float3 envmap_color = flat_environment_map_sample.rgb;
    
    float bloom_alpha = dot(envmap_color.zxy, float3(0.5, 0.65, 1)); // ???
    
    bloom_alpha = -env_bloom_override.a - -bloom_alpha; // ???
    
    bloom_alpha = max(bloom_alpha, 0); // 3.141593
    
    envmap_color *= (env_bloom_override.rgb * bloom_alpha) * env_bloom_override_intensity;
	
    unknown_output.rgb = env_tint_color.rgb * env_mapping_common_data.specular_coefficient;
    unknown_output.a = env_mapping_common_data.specular_exponent;
    
    float3 environment_color = (envmap_color.rgb * env_mapping_common_data.specular_coefficient * env_mapping_common_data.env_area_specular) * env_tint_color;
    diffuse += environment_color;
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
