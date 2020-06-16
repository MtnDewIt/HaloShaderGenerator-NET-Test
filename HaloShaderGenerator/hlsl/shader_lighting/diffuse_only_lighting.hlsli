#ifndef _DIFFUSE_ONLY_LIGHTING_HLSLI
#define _DIFFUSE_ONLY_LIGHTING_HLSLI

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
	
	return diffuse;
}
#endif