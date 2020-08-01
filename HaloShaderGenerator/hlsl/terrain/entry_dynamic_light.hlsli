#ifndef _TERRAIN_TEMPLATE_DYNAMIC_LIGHT_HLSLI
#define _TERRAIN_TEMPLATE_DYNAMIC_LIGHT_HLSLI

#include "..\registers\global_parameters.hlsli"

#include "..\helpers\terrain_helper.hlsli"
#include "..\helpers\color_processing.hlsli"
#include "..\helpers\lighting.hlsli"
#include "..\helpers\shadows.hlsli"
#include "..\terrain_lighting\terrain_lighting.hlsli"


uniform sampler2D dynamic_light_gel_texture;

struct VS_OUTPUT_TERRAIN_DYNAMIC_LIGHT
{
	float4 position : SV_Position;
	float2 texcoord : TEXCOORD0;
	float3 normal : TEXCOORD1;
	float3 binormal : TEXCOORD2;
	float3 tangent : TEXCOORD3;
	float3 camera_dir : TEXCOORD4;
	float4 shadowmap_texcoord : TEXCOORD5;
	float3 extinction_factor : COLOR0;
	float3 sky_radiance : COLOR1;
};

PS_OUTPUT_DEFAULT calculate_dynamic_light(
float2 position,
float2 texcoord,
float3 camera_dir,
float3 tangent,
float3 binormal,
float3 normal,
int light_index,
float depth_scale,
float depth_offset,
float2 shadowmap_texcoord,
float3 extinction_factor,
bool is_cinematic)
{
	float3 view_dir = normalize(camera_dir);

	float3 world_position = Camera_Position_PS - camera_dir;
	float4 albedo;
	float3 surface_normal;
	float2 fragcoord = position.xy;
	fragcoord += 0.5;
	float2 inv_texture_size = (1.0 / texture_size);
	float2 rt_texcoord = fragcoord * inv_texture_size;
	float4 normal_texture_sample = tex2D(normal_texture, rt_texcoord);
	float4 albedo_texture_sample = tex2D(albedo_texture, rt_texcoord);
	surface_normal = normal_import(normal_texture_sample.xyz);
	albedo = albedo_texture_sample;
	
	
	SimpleLight light = get_simple_light(light_index);
	
	float3 v_to_light = light.position.xyz - world_position;
	float light_distance_squared = dot(v_to_light, v_to_light);
	v_to_light = normalize(v_to_light);
	
	float3 L = normalize(v_to_light); // normalized surface to light direction

	float distance_attenuation, cone_attenuation;
	get_simple_light_parameters(light, L, light_distance_squared, distance_attenuation, cone_attenuation);

	float3 intensity = distance_attenuation * cone_attenuation;
	
	float2 shadowmap_texcoord_depth_adjusted = shadowmap_texcoord * (1.0 / depth_scale);
	
	float2 gel_texcoord = apply_xform2d(shadowmap_texcoord_depth_adjusted, p_dynamic_light_gel_xform);
	float4 gel_sample = tex2D(dynamic_light_gel_texture, gel_texcoord);
	
	float3 light_intensity = intensity * light.color.rgb * gel_sample.rgb;

	float3 color;
	
	SHADER_DYNAMIC_LIGHT_COMMON common_data;

	common_data.albedo = albedo;
	common_data.texcoord = texcoord;
	common_data.surface_normal = surface_normal;
	common_data.normal = normal;
	common_data.view_dir = view_dir;
	common_data.reflect_dir = 0;
	common_data.light_intensity = light_intensity;
	common_data.light_direction = L;
	common_data.specular_mask = 1.0f;
	
	calc_dynamic_lighting_terrain(common_data, color);

	float threshold = dot(light_intensity, light_intensity);
	threshold = 0.0000001 - threshold;
	color.rgb = threshold < 0 ? color.rgb : 0;

	
	float shadow_coefficient;
	if (is_cinematic)
		shadow_coefficient = shadows_percentage_closer_filtering_custom_4x4(shadowmap_texcoord_depth_adjusted, shadowmap_texture_size, depth_scale, depth_offset);
	else
		shadow_coefficient = shadows_percentage_closer_filtering_3x3(shadowmap_texcoord_depth_adjusted, shadowmap_texture_size, depth_scale, depth_offset);
	
	
	float4 result;

	result.a = 1.0f;
	color *= shadow_coefficient;
	color *= extinction_factor;
	
	result.rgb = expose_color(color.rgb);
	
	
	
	return export_color(result);
}

PS_OUTPUT_DEFAULT shader_entry_dynamic_light(VS_OUTPUT_TERRAIN_DYNAMIC_LIGHT input)
{
	return calculate_dynamic_light(input.position.xy, input.texcoord, input.camera_dir, input.tangent, input.binormal, input.normal, 0, input.shadowmap_texcoord.w, input.shadowmap_texcoord.z, input.shadowmap_texcoord.xy, input.extinction_factor, false);
}

PS_OUTPUT_DEFAULT shader_entry_dynamic_light_cinematic(VS_OUTPUT_TERRAIN_DYNAMIC_LIGHT input)
{
	return calculate_dynamic_light(input.position.xy, input.texcoord, input.camera_dir, input.tangent, input.binormal, input.normal, 0, input.shadowmap_texcoord.w, input.shadowmap_texcoord.z, input.shadowmap_texcoord.xy, input.extinction_factor, true);
}

#endif