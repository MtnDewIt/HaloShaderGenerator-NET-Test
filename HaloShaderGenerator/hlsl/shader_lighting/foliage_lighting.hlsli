#ifndef _FOLIAGE_LIGHTING_HLSLI
#define _FOLIAGE_LIGHTING_HLSLI

#include "..\material_models\material_shared_parameters.hlsli"
#include "..\helpers\input_output.hlsli"

float3 calc_lighting_foliage_ps(SHADER_COMMON common_data)
{
	float3 diffuse;
	if (common_data.no_dynamic_lights)
	{
		diffuse = 0;
	}
	else
	{
		float3 diffuse_accumulation;
		float3 specular_accumulation;
		
		calc_material_lambert_diffuse_ps(common_data.surface_normal, common_data.world_position, 0, 0, diffuse_accumulation, specular_accumulation);
		
		diffuse = diffuse_accumulation;
	}
	diffuse += common_data.diffuse_reflectance * common_data.precomputed_radiance_transfer;
	return diffuse;
}
#endif