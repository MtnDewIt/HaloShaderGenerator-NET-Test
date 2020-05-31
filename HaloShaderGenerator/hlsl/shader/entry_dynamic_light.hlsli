#ifndef _SHADER_TEMPLATE_DYNAMIC_LIGHT_HLSLI
#define _SHADER_TEMPLATE_DYNAMIC_LIGHT_HLSLI

#include "entry_albedo.hlsli"
#include "..\registers\shader.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"
#include "..\helpers\lighting.hlsli"
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
	texcoord = calc_parallax_ps(texcoord, camera_dir, tangent, binormal, normal);
	float alpha = calc_alpha_test_ps(texcoord);
	
	float3 world_position = Camera_Position_PS - camera_dir;
	float shadow_coefficient;
	float3 diffuse;

	SimpleLight light = get_simple_light(light_index);
	
	float3 v_to_light = light.position.xyz - world_position;
	float light_distance_squared = dot(v_to_light, v_to_light);
	v_to_light = normalize(v_to_light);
	
	float attenuation = 1.0 / (light_distance_squared + light.position.w);
	float light_angle = dot(v_to_light, light.direction.xyz);

	float2 packed_light_values = float2(attenuation, light_angle);
	packed_light_values = max(0.0001, packed_light_values * light.unknown3.xy + light.unknown3.zw);
	float specular_power = pow(packed_light_values.y, light.color.w);

	float intensity = saturate(specular_power + light.direction.w) * saturate(packed_light_values.x);
	
	float2 shadowmap_texcoord_depth_adjusted = shadowmap_texcoord * (1.0 / depth_scale);
	
	float2 gel_texcoord = apply_xform2d(shadowmap_texcoord_depth_adjusted, p_dynamic_light_gel_xform);
	float4 gel_sample = tex2D(dynamic_light_gel_texture, gel_texcoord);

	diffuse = (intensity * light.color.rgb) * gel_sample.rgb;
	
	float4 albedo;
	float3 modified_normal;

	get_albedo_and_normal(actually_calc_albedo, position.xy, texcoord.xy, camera_dir, tangent.xyz, binormal.xyz, normal.xyz, albedo, modified_normal);
	
	diffuse *= dot(v_to_light, modified_normal);
	diffuse *= albedo.rgb;
	
	
	if (dynamic_light_shadowing)
	{
		if (is_cinematic)
			shadow_coefficient = shadows_percentage_closer_filtering_custom_4x4(shadowmap_texcoord_depth_adjusted, shadowmap_texture_size, depth_scale, depth_offset, diffuse);
		else
			shadow_coefficient = shadows_percentage_closer_filtering_3x3(shadowmap_texcoord_depth_adjusted, shadowmap_texture_size, depth_scale, depth_offset, diffuse);
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
	
	result.rgb = expose_color(diffuse);
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