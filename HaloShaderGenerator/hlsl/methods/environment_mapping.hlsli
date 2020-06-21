#ifndef _ENVIRONMENT_MAPPING_HLSLI
#define _ENVIRONMENT_MAPPING_HLSLI

#include "../helpers/types.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/sh.hlsli"
#include "../registers/shader.hlsli"
#include "../methods/blend_mode.hlsli"
#include "../helpers/input_output.hlsli"

uniform float3 env_tint_color;
uniform float env_roughness_scale;
uniform samplerCUBE dynamic_environment_map_0;
uniform samplerCUBE dynamic_environment_map_1;


float get_lod_level(in float3 reflect_dir)
{
	float3 m_reflect_dir = float3(reflect_dir.x, -reflect_dir.y, reflect_dir.z);
    
	float3 x_derivative = ddx(m_reflect_dir);
	float3 y_derivative = ddy(m_reflect_dir);
    
	float max_length = max(length(x_derivative), length(y_derivative));
    
	float lod_level = sqrt(max_length) * 6 - 0.6;
	return max(lod_level, 0);
    
}

void envmap_type_none(
in ENVIRONMENT_MAPPING_COMMON env_mapping_common_data,
inout float3 diffuse,
out float3 unknown_output)
{
	unknown_output = 0;
}

void envmap_type_per_pixel(
in ENVIRONMENT_MAPPING_COMMON env_mapping_common_data,
inout float3 diffuse,
out float3 unknown_output)
{
	float3 envmap_texcoord = float3(env_mapping_common_data.reflect_dir.x, -env_mapping_common_data.reflect_dir.y, env_mapping_common_data.reflect_dir.z);
	float4 envmap_sample = texCUBE(environment_map, envmap_texcoord);
	float3 environment_color = (envmap_sample.rgb * env_mapping_common_data.sh_0_env_color) * env_tint_color * envmap_sample.a;
	diffuse += environment_color;
	unknown_output = env_tint_color.rgb;

}

void envmap_type_dynamic(
in ENVIRONMENT_MAPPING_COMMON env_mapping_common_data,
inout float3 diffuse,
out float3 unknown_output)
{
	float4 dynamic_envmap_texcoord;
	dynamic_envmap_texcoord.w = get_lod_level(env_mapping_common_data.reflect_dir);
	dynamic_envmap_texcoord.xyz = env_mapping_common_data.reflect_dir;
	dynamic_envmap_texcoord *= float4(1, -1, 1, -1);
	float4 dynamic_environment_map_0_sample = texCUBElod(dynamic_environment_map_0, dynamic_envmap_texcoord);
	float3 env_color_0 = dynamic_environment_map_0_sample.rgb * dynamic_environment_map_0_sample.w;
	env_color_0 *= 256;
	env_color_0 *= (1.0f - dynamic_environment_blend.rgb);
    
	float4 dynamic_environment_map_1_sample = texCUBElod(dynamic_environment_map_1, dynamic_envmap_texcoord);
	float3 env_color_1 = dynamic_environment_map_1_sample.rgb * dynamic_environment_map_1_sample.w;
	env_color_1 *= dynamic_environment_blend.rgb;
	env_color_1 *= 256;
    
	float3 environment_color = env_color_0 + env_color_1;
	environment_color *= env_mapping_common_data.specular_coefficient;
	environment_color *= env_tint_color.xyz;
	environment_color *= env_mapping_common_data.sh_0_env_color;
	environment_color *= max(env_mapping_common_data.area_specular, 0.001);
	diffuse += environment_color;
	
	unknown_output = env_mapping_common_data.specular_coefficient * env_tint_color.rgb;
}

void envmap_type_from_flat_texture(
in ENVIRONMENT_MAPPING_COMMON env_mapping_common_data,
inout float3 diffuse,
out float3 unknown_output)
{
	unknown_output = 0;
}

void envmap_type_custom_map(
in ENVIRONMENT_MAPPING_COMMON env_mapping_common_data,
inout float3 diffuse,
out float3 unknown_output)
{
	unknown_output = 0;
}

#ifndef envmap_type
#define envmap_type envmap_type_none
#endif

#endif
