#ifndef _SHADER_TEMPLATE_PER_PIXEL_LIGHTING_HLSLI
#define _SHADER_TEMPLATE_PER_PIXEL_LIGHTING_HLSLI


#include "entry_albedo.hlsli"
#include "..\helpers\lightmaps.hlsli"

#include "..\methods\specular_mask.hlsli"
#include "..\methods\material_model.hlsli"
#include "..\methods\environment_mapping.hlsli"
#include "..\methods\self_illumination.hlsli"
#include "..\methods\blend_mode.hlsli"
#include "..\methods\misc.hlsli"

#include "..\registers\shader.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"


PS_OUTPUT_DEFAULT shader_entry_static_per_pixel(VS_OUTPUT_PER_PIXEL input)
{
	SHADER_COMMON common_data;
	{
		get_lightmap_sh_coefficients(input.lightmap_texcoord, common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, common_data.dominant_light_direction, common_data.dominant_light_intensity);
		
		common_data.view_dir = input.camera_dir;
		common_data.n_view_dir = normalize(input.camera_dir);
		common_data.fragcoord = input.position.xy;
		common_data.tangent = input.tangent;
		common_data.binormal = input.binormal;
		common_data.normal = input.normal;
		common_data.texcoord = calc_parallax_ps(input.texcoord.xy, common_data.n_view_dir, input.tangent, input.binormal, input.normal);
		common_data.alpha = calc_alpha_test_ps(common_data.texcoord);

		if (actually_calc_albedo)
		{
			common_data.surface_normal = calc_bumpmap_ps(common_data.tangent, common_data.binormal, common_data.normal.xyz, common_data.texcoord);
			common_data.albedo = calc_albedo_ps(common_data.texcoord, common_data.fragcoord);
		}
		else
		{
			float2 position = input.position.xy;
			position += 0.5;
			float2 inv_texture_size = (1.0 / texture_size);
			float2 texcoord = position * inv_texture_size;
			float4 normal_texture_sample = tex2D(normal_texture, texcoord);
			common_data.surface_normal = normal_import(normal_texture_sample.xyz);
			float4 albedo_texture_sample = tex2D(albedo_texture, texcoord);
			common_data.albedo = albedo_texture_sample;
		}
		
		common_data.surface_normal = normalize(common_data.surface_normal);
		
		//remove_dominant_light_contribution(common_data.dominant_light_direction, common_data.dominant_light_intensity, common_data.sh_0, common_data.sh_312);
	
		common_data.diffuse_reflectance = lightmap_diffuse_reflectance(common_data.surface_normal, common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, common_data.dominant_light_direction, common_data.dominant_light_intensity);
		//common_data.diffuse_reflectance += dominant_light_diffuse_reflectance(common_data.surface_normal, common_data.dominant_light_direction, common_data.dominant_light_intensity);
		
		
		float v_dot_n = dot(common_data.n_view_dir, common_data.surface_normal);
		common_data.half_dir = v_dot_n * common_data.surface_normal - common_data.n_view_dir;
		common_data.reflect_dir = common_data.half_dir * 2 + common_data.n_view_dir;
		common_data.world_position = Camera_Position_PS - common_data.view_dir;

		common_data.precomputed_radiance_transfer = 1.0;
		common_data.per_vertex_color = 0.0f;
		common_data.no_dynamic_lights = no_dynamic_lights;
		
		if (!calc_atmosphere_no_material && !calc_material)
		{
			common_data.sky_radiance = 0.0;
			common_data.extinction_factor = 1.0;
		}
		else
		{
			common_data.sky_radiance = input.sky_radiance;
			common_data.extinction_factor = input.extinction_factor;
		}
	}
	
	float4 color;
	if (calc_material)
	{
		color.rgb = calc_lighting_ps(common_data);
	}
	else
	{
		color.rgb = 1.0;
	}
	
	color.rgb *= common_data.albedo.rgb;
	
	float3 env_band_0 = get_environment_contribution(common_data.sh_0);
	envmap_type(common_data.view_dir, common_data.reflect_dir, env_band_0, color.rgb);
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
		output.unknown.rgb = env_tint_color.rgb;
	}
	return output;
}
#endif