#ifndef _SHADER_TEMPLATE_DYNAMIC_LIGHT_HLSLI
#define _SHADER_TEMPLATE_DYNAMIC_LIGHT_HLSLI

#include "entry_albedo.hlsli"
#include "..\registers\global_parameters.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"
#include "..\helpers\lighting.hlsli"
#include "..\methods\material_model.hlsli"
#include "..\helpers\shadows.hlsli"

uniform sampler2D dynamic_light_gel_texture;

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
bool is_cinematic)
{
	float3 view_dir = normalize(camera_dir);
	texcoord = calc_parallax_ps(texcoord, view_dir, tangent, binormal, normal);
	float alpha = calc_alpha_test_ps(texcoord);
	
	float3 world_position = Camera_Position_PS - camera_dir;

	SimpleLight light = get_simple_light(light_index);
	
	float3 v_to_light = light.position.xyz - world_position;
	float light_distance_squared = dot(v_to_light, v_to_light);
	v_to_light = normalize(v_to_light);
	
	float3 L = normalize(v_to_light); // normalized surface to light direction

	float distance_attenuation, cone_attenuation;
	get_simple_light_parameters(light, L, light_distance_squared, distance_attenuation, cone_attenuation);

	float3 intensity = cone_attenuation * distance_attenuation;
	
	float2 shadowmap_texcoord_depth_adjusted = shadowmap_texcoord * (1.0 / depth_scale);
	float2 gel_texcoord = apply_xform2d(shadowmap_texcoord_depth_adjusted, p_dynamic_light_gel_xform);
	float4 gel_sample = tex2D(dynamic_light_gel_texture, gel_texcoord);
	
	float3 light_intensity = intensity * light.color.rgb * gel_sample.rgb;
	
	float4 albedo;
	float3 surface_normal;
	get_albedo_and_normal(actually_calc_albedo, position.xy, texcoord.xy, camera_dir, tangent.xyz, binormal.xyz, normal.xyz, albedo, surface_normal);
	float c_specular_mask = 1.0;
	calc_specular_mask_ps(albedo, texcoord, c_specular_mask);
	float3 reflect_dir = 2 * dot(view_dir, surface_normal) * surface_normal - camera_dir;
	
	
	float3 color;
	
	SHADER_DYNAMIC_LIGHT_COMMON common_data;

	common_data.albedo = albedo;
	common_data.texcoord = texcoord;
	common_data.surface_normal = surface_normal;
	common_data.normal = normal;
	common_data.view_dir = view_dir;
	common_data.reflect_dir = reflect_dir;
	common_data.light_intensity = light_intensity;
	common_data.light_direction = L;
	common_data.specular_mask = c_specular_mask;
	
	calc_dynamic_lighting_ps(common_data, color);
	
	float shadow_coefficient;
	if (dynamic_light_shadowing)
	{
		if (is_cinematic)
			shadow_coefficient = shadows_percentage_closer_filtering_custom_4x4(shadowmap_texcoord_depth_adjusted, shadowmap_texture_size, depth_scale, depth_offset);
		else
			shadow_coefficient = shadows_percentage_closer_filtering_3x3(shadowmap_texcoord_depth_adjusted, shadowmap_texture_size, depth_scale, depth_offset);
		
		if (dot(color.rgb, color.rgb) <= 0)
			shadow_coefficient = 1.0;
	}
	else
	{
		shadow_coefficient = 1.0;
	}
	
	float4 result;
	if (blend_type_arg == k_blend_mode_additive)
	{
		result.a = 0.0;
	}
	else if (blend_type_arg == k_blend_mode_alpha_blend || blend_type_arg == k_blend_mode_pre_multiplied_alpha)
	{
		result.a = alpha * albedo.a;
	}
	else
	{
		result.a = alpha;
	}
	
	result.rgb = expose_color(color.rgb);
	result.rgb *= shadow_coefficient;
	
	return export_color(result);
}

PS_OUTPUT_DEFAULT shader_entry_dynamic_light(VS_OUTPUT_DYNAMIC_LIGHT input)
{
	return calculate_dynamic_light(input.position.xy, input.texcoord, input.camera_dir, input.tangent, input.binormal, input.normal, 0, input.shadowmap_texcoord.w, input.shadowmap_texcoord.z, input.shadowmap_texcoord.xy, false);
}

PS_OUTPUT_DEFAULT shader_entry_dynamic_light_cinematic(VS_OUTPUT_DYNAMIC_LIGHT input)
{
	return calculate_dynamic_light(input.position.xy, input.texcoord, input.camera_dir, input.tangent, input.binormal, input.normal, 0, input.shadowmap_texcoord.w, input.shadowmap_texcoord.z, input.shadowmap_texcoord.xy, true);
}

#endif