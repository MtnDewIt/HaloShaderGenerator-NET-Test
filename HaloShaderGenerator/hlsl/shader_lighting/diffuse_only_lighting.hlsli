#ifndef _DIFFUSE_ONLY_LIGHTING_HLSLI
#define _DIFFUSE_ONLY_LIGHTING_HLSLI

#include "..\material_models\material_shared_parameters.hlsli"
#include "..\helpers\input_output.hlsli"

void calc_dynamic_lighting_diffuse_only_ps(float3 light_dir, float3 view_dir, float3 reflect_dir, float3 surface_normal, float2 texcoord, float3 light_intensity, float3 albedo, out float3 color)
{
	float v_dot_n = dot(light_dir, surface_normal);
	color = light_intensity * v_dot_n * albedo.rgb;	// lambertian diffuse
}


float3 calc_lighting_diffuse_only_ps(SHADER_COMMON common_data)
{
	float3 diffuse;
	if (!common_data.no_dynamic_lights)
	{
		float3 diffuse_accumulation;
		float3 specular_accumulation;
		
		calc_material_lambert_diffuse_ps(common_data.surface_normal, common_data.world_position, 0, 0, diffuse_accumulation, specular_accumulation);
		
		diffuse = common_data.diffuse_reflectance * common_data.precomputed_radiance_transfer + diffuse_accumulation;
	}
	else
		diffuse = common_data.diffuse_reflectance * common_data.precomputed_radiance_transfer;
	
	diffuse *= common_data.albedo.rgb;
	
	return diffuse;
}
#endif