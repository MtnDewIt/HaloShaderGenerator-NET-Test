#ifndef _PER_VERTEX_COLOR_LIGHTING_HLSLI
#define _PER_VERTEX_COLOR_LIGHTING_HLSLI

#include "..\material_models\lambert.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\sh.hlsli"
#include "..\methods\environment_mapping.hlsli"
#include "..\methods\self_illumination.hlsli"

float3 calc_per_vertex_color_lighting(SHADER_COMMON common_data, out float4 unknown_output)
{
	float3 diffuse;
	float3 self_illum = 0;
	
	calc_self_illumination_ps(common_data.texcoord.xy, common_data.albedo.rgb, self_illum);
	
	if (!common_data.no_dynamic_lights)
	{
		float3 diffuse_accumulation = 0;
		float3 specular_accumulation = 0;
		
		calc_material_lambert_diffuse_ps(common_data.surface_normal, common_data.world_position, 0, 0, diffuse_accumulation, specular_accumulation);
		
		diffuse = common_data.diffuse_reflectance * common_data.precomputed_radiance_transfer.x + diffuse_accumulation;
	}
	else
		diffuse = common_data.diffuse_reflectance * common_data.precomputed_radiance_transfer.x;
	
	diffuse *= common_data.albedo.rgb;
	
	unknown_output = 0.0f;
	
	if (self_illum_is_diffuse)
		diffuse = self_illum;
	else
		diffuse += self_illum;
	
	return diffuse;
}
#endif