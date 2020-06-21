#ifndef _DIFFUSE_ONLY_LIGHTING_HLSLI
#define _DIFFUSE_ONLY_LIGHTING_HLSLI

#include "..\material_models\material_shared_parameters.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\sh.hlsli"
#include "..\methods\environment_mapping.hlsli"

void calc_dynamic_lighting_diffuse_only_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
	float v_dot_n = dot(common_data.light_direction, common_data.surface_normal);
	color = common_data.light_intensity * v_dot_n * common_data.albedo.rgb; // lambertian diffuse
}


float3 calc_lighting_diffuse_only_ps(SHADER_COMMON common_data)
{
	float3 diffuse;
	if (!common_data.no_dynamic_lights)
	{
		float3 diffuse_accumulation = 0;
		float3 specular_accumulation = 0;
		
		calc_material_lambert_diffuse_ps(common_data.surface_normal, common_data.world_position, 0, 0, diffuse_accumulation, specular_accumulation);
		
		diffuse = common_data.diffuse_reflectance * common_data.precomputed_radiance_transfer + diffuse_accumulation;
	}
	else
		diffuse = common_data.diffuse_reflectance * common_data.precomputed_radiance_transfer;
	
	diffuse *= common_data.albedo.rgb;
	
#if shaderstage != k_shaderstage_static_per_vertex_color
	
	float3 env_band_0 = get_environment_contribution(common_data.sh_0);
	envmap_type(common_data.view_dir, common_data.reflect_dir, env_band_0, diffuse);
	
#endif
	

	return diffuse;
}
#endif