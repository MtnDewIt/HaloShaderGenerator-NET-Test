#ifndef _SHADER_TEMPLATE_PRT_HLSLI
#define _SHADER_TEMPLATE_PRT_HLSLI

#pragma warning( disable : 3571 34)

#include "entry_albedo.hlsli"

#include "..\registers\shader.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"

#include "..\methods\material_model.hlsli"
#include "..\shader_lighting\no_material_lighting.hlsli"
#include "..\methods\environment_mapping.hlsli"
#include "..\methods\self_illumination.hlsli"
#include "..\methods\blend_mode.hlsli"
#include "..\methods\misc.hlsli"

PS_OUTPUT_DEFAULT entry_static_sh_prt(
float2 position,
float2 texcoord,
float3 camera_dir,
float3 normal,
float3 tangent,
float3 binormal,
float3 sky_radiance,
float3 extinction_factor,
float prt)
{
	SHADER_COMMON common_data;
	{
		common_data.view_dir = camera_dir;
		common_data.n_view_dir = normalize(camera_dir);
		common_data.fragcoord = position.xy;
		common_data.tangent = tangent;
		common_data.binormal = binormal;
		common_data.normal = normal;
		common_data.texcoord = calc_parallax_ps(texcoord, common_data.n_view_dir, tangent, binormal, normal);
		common_data.alpha = calc_alpha_test_ps(common_data.texcoord);

		if (actually_calc_albedo)
		{
			common_data.surface_normal = calc_bumpmap_ps(tangent, binormal, normal.xyz, common_data.texcoord);
			common_data.albedo = calc_albedo_ps(common_data.texcoord, position);
		}
		else
		{
			position += 0.5;
			float2 inv_texture_size = (1.0 / texture_size);
			float2 texcoord = position * inv_texture_size;
			float4 normal_texture_sample = tex2D(normal_texture, texcoord);
			common_data.surface_normal = normal_import(normal_texture_sample.xyz);
			float4 albedo_texture_sample = tex2D(albedo_texture, texcoord);
			common_data.albedo = albedo_texture_sample;
		}
		
		common_data.surface_normal = normalize(common_data.surface_normal);
		float v_dot_n = dot(common_data.n_view_dir, common_data.surface_normal);
		common_data.half_dir = v_dot_n * common_data.surface_normal - common_data.n_view_dir;
		common_data.reflect_dir = common_data.half_dir * 2 + common_data.n_view_dir;
		common_data.world_position = Camera_Position_PS - common_data.view_dir;
		
		get_current_sh_coefficients_quadratic(common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, common_data.dominant_light_direction, common_data.dominant_light_intensity);
		
		common_data.sh_0_no_dominant_light = common_data.sh_0;
		common_data.sh_312_no_dominant_light[0] = common_data.sh_312[0];
		common_data.sh_312_no_dominant_light[1] = common_data.sh_312[1];
		common_data.sh_312_no_dominant_light[2] = common_data.sh_312[2];
		
		remove_dominant_light_contribution(common_data.dominant_light_direction, common_data.dominant_light_intensity, common_data.sh_0_no_dominant_light, common_data.sh_312_no_dominant_light);
	
		common_data.diffuse_reflectance = diffuse_reflectance(common_data.surface_normal);
		
		common_data.precomputed_radiance_transfer = prt;
		common_data.per_vertex_color = 0.0f;
		common_data.no_dynamic_lights = no_dynamic_lights;
		
		if (!calc_atmosphere_no_material && !calc_material)
		{
			common_data.sky_radiance = 0.0;
			common_data.extinction_factor = 1.0;
		}
		else
		{
			common_data.sky_radiance = sky_radiance;
			common_data.extinction_factor = extinction_factor;
		}
		
		common_data.specular_mask = 1.0;
		calc_specular_mask_ps(common_data.albedo, common_data.texcoord, common_data.specular_mask);
	}

	float4 color;
	float4 unknown_color = 0;
	if (calc_material)
	{
		color.rgb = calc_lighting_ps(common_data, unknown_color);
	}
	else
	{
		color.rgb = common_data.albedo.rgb;
		calc_lighting_no_material_ps(common_data, color.rgb, unknown_color);
	}
	
	calc_self_illumination_ps(common_data.texcoord.xy, common_data.albedo.rgb, color.rgb);
	
	color.rgb = color.rgb * common_data.extinction_factor;
		
	color.a = blend_type_calculate_alpha_blending(common_data.albedo, common_data.alpha);
	
	if (blend_type_arg != k_blend_mode_additive)
	{
		color.rgb += common_data.sky_radiance.rgb;
	}

	if (blend_type_arg == k_blend_mode_double_multiply)
		color.rgb *= 2;

	color.rgb = expose_color(color.rgb);
	
	if (blend_type_arg == k_blend_mode_pre_multiplied_alpha)
		color.rgb *= color.a;

	PS_OUTPUT_DEFAULT output = export_color(color);
	if (calc_env_output)
	{
		output.unknown = unknown_color;
	}
	return output;
}


PS_OUTPUT_DEFAULT shader_entry_static_sh(VS_OUTPUT_STATIC_SH input)
{
	return entry_static_sh_prt(input.position.xy, input.texcoord.xy, input.camera_dir.xyz, input.normal, input.tangent, input.binormal, input.sky_radiance, input.extinction_factor, 1.0);
}

PS_OUTPUT_DEFAULT shader_entry_static_prt(VS_OUTPUT_STATIC_PRT input)
{
	return entry_static_sh_prt(input.position.xy, input.texcoord.xy, input.camera_dir.xyz, input.normal, input.tangent, input.binormal, input.sky_radiance, input.extinction_factor, input.prt_radiance_vector.x);
}

#endif