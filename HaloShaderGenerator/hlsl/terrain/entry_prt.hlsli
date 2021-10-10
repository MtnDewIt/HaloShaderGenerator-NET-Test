#ifndef _TERRAIN_TEMPLATE_PRT_HLSLI
#define _TERRAIN_TEMPLATE_PRT_HLSLI

#include "..\registers\global_parameters.hlsli"
#include "..\helpers\terrain_helper.hlsli"
#include "..\helpers\color_processing.hlsli"
#include "..\terrain_lighting\terrain_lighting.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\sh.hlsli"

PS_OUTPUT_DEFAULT entry_static_sh_prt(
float2 position,
float2 texcoord,
float3 camera_dir,
float3 normal,
float3 tangent,
float3 binormal,
float3 sky_radiance,
float3 extinction_factor,
float4 prt)
{
	SHADER_COMMON common_data;
	{
		common_data.view_dir = camera_dir;
		common_data.n_view_dir = normalize(camera_dir);
		common_data.fragcoord = position.xy;
		common_data.tangent = tangent;
		common_data.binormal = binormal;
		common_data.normal = normal;
		common_data.texcoord = texcoord;
		common_data.alpha = 1.0f;

		position += 0.5;
		float2 inv_texture_size = (1.0 / texture_size);
		float2 texcoord = position * inv_texture_size;
		float4 normal_texture_sample = tex2D(normal_texture, texcoord);
		common_data.surface_normal = normal_import(normal_texture_sample.xyz);
		float4 albedo_texture_sample = tex2D(albedo_texture, texcoord);
		common_data.albedo = albedo_texture_sample;
		
		//common_data.surface_normal = normalize(common_data.surface_normal);
		
		common_data.specular_mask = 1.0;
		
		float v_dot_n = dot(common_data.n_view_dir, common_data.surface_normal);
		common_data.half_dir = v_dot_n * common_data.surface_normal - common_data.n_view_dir;
		common_data.reflect_dir = common_data.half_dir * 2 + common_data.n_view_dir;
		common_data.world_position = Camera_Position_PS - common_data.view_dir;
		
		get_current_sh_coefficients_quadratic(common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, common_data.dominant_light_direction, common_data.dominant_light_intensity);
		
		common_data.sh_0_no_dominant_light = common_data.sh_0;
		common_data.sh_312_no_dominant_light[0] = common_data.sh_312[0];
		common_data.sh_312_no_dominant_light[1] = common_data.sh_312[1];
		common_data.sh_312_no_dominant_light[2] = common_data.sh_312[2];
		
		common_data.diffuse_reflectance = diffuse_reflectance(common_data.surface_normal);
		
		common_data.precomputed_radiance_transfer = prt;
		common_data.per_vertex_color = 0.0f;
		common_data.no_dynamic_lights = false;
		
		common_data.sky_radiance = sky_radiance;
		common_data.extinction_factor = extinction_factor;
	}

	float4 color;
	float4 unknown_color;
	
	color.rgb = calc_lighting_terrain(common_data, unknown_color);
	color.a = 1.0f;
	
	color.rgb *= common_data.extinction_factor;
	color.rgb += common_data.sky_radiance;
	
	color.rgb = color.rgb * g_exposure.x;
	color.rgb = max(color.rgb, 0);
	PS_OUTPUT_DEFAULT output;
	output.low_frequency = export_low_frequency(color);
	output.high_frequency = export_high_frequency(color);

	if (calc_env_output)
		output.unknown = unknown_color;
	else
		output.unknown = 0.0f;
	
	return output;
}


PS_OUTPUT_DEFAULT shader_entry_static_sh(VS_OUTPUT_STATIC_SH_TERRAIN input)
{
    return entry_static_sh_prt(input.position.xy, input.texcoord.xy, input.camera_dir.xyz, 0, 0, 0, input.sky_radiance, input.extinction_factor, 1.0);
}

PS_OUTPUT_DEFAULT shader_entry_static_prt(VS_OUTPUT_STATIC_PRT_TERRAIN input)
{
    return entry_static_sh_prt(input.position.xy, input.texcoord.xy, input.camera_dir.xyz, 0, 0, 0, input.sky_radiance, input.extinction_factor, input.prt_radiance_vector);
}

#endif