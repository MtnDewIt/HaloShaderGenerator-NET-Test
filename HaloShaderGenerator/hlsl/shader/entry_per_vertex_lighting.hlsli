#ifndef _SHADER_TEMPLATE_PER_VERTEX_LIGHTING_HLSLI
#define _SHADER_TEMPLATE_PER_VERTEX_LIGHTING_HLSLI

#include "..\helpers\lightmaps.hlsli"
#include "..\methods\albedo.hlsli"
#include "..\methods\parallax.hlsli"
#include "..\methods\bump_mapping.hlsli"

#include "..\methods\specular_mask.hlsli"
#include "..\methods\self_illumination.hlsli"
#include "..\methods\alpha_test.hlsli"
#include "..\methods\material_model.hlsli"
#include "..\shader_lighting\no_material_lighting.hlsli"
#include "..\methods\environment_mapping.hlsli"

#include "..\methods\blend_mode.hlsli"
#include "..\methods\misc.hlsli"

#include "..\registers\global_parameters.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"

#include "..\methods\soft_fade.hlsli"
#include "entry_sfx_distort.hlsli"

PS_OUTPUT_DEFAULT shader_entry_static_per_vertex(VS_OUTPUT_PER_VERTEX input)
{
	
	SHADER_COMMON common_data;
	{
        unpack_per_vertex_lightmap_coefficients(input.lightmap_coefficients, common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, common_data.dominant_light_direction, common_data.dominant_light_intensity);
		
		
		common_data.view_dir = input.camera_dir;
		common_data.n_view_dir = normalize(input.camera_dir);
		common_data.fragcoord = input.position.xy;
		common_data.tangent = input.tangent;
		common_data.binormal = input.binormal;
		common_data.normal = input.normal.xyz;
		common_data.texcoord = calc_parallax_ps(input.texcoord.xy, common_data.n_view_dir, input.tangent, input.binormal, input.normal.xyz);
		common_data.alpha = calc_alpha_test_ps(common_data.texcoord, 1.0f);

		if (actually_calc_albedo)
        {
            float2 calc_albedo_texcoord = common_data.texcoord;
			
            apply_sfx_distortion(calc_albedo_texcoord);
			
            common_data.surface_normal = calc_bumpmap_ps(common_data.tangent, common_data.binormal, common_data.normal.xyz, calc_albedo_texcoord);
            common_data.albedo = calc_albedo_ps(calc_albedo_texcoord, common_data.fragcoord, common_data.surface_normal, common_data.view_dir);
			
            apply_soft_fade(common_data.albedo, dot(common_data.n_view_dir, normalize(common_data.surface_normal)), float4(common_data.fragcoord, 0.0f, input.normal.w));
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
		
		common_data.specular_mask = 1.0;
		calc_specular_mask_ps(common_data.albedo, common_data.texcoord, common_data.specular_mask);
		
		float v_dot_n = dot(common_data.n_view_dir, common_data.surface_normal);
		common_data.half_dir = v_dot_n * common_data.surface_normal - common_data.n_view_dir;
		common_data.reflect_dir = common_data.half_dir * 2 + common_data.n_view_dir;
		
		common_data.diffuse_reflectance = lightmap_diffuse_reflectance(common_data.surface_normal, common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, common_data.dominant_light_direction, common_data.dominant_light_intensity, common_data.sh_0_no_dominant_light, common_data.sh_312_no_dominant_light);
		
		
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
			common_data.sky_radiance = float3(input.texcoord.zw, input.extinction_factor.w);
			common_data.extinction_factor = input.extinction_factor.rgb;
		}
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
#endif