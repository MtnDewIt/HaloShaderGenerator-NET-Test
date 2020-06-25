#ifndef _ORGANISM_LIGHTING_HLSLI
#define _ORGANISM_LIGHTING_HLSLI

#include "..\material_models\material_shared_parameters.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\sh.hlsli"
#include "..\methods\environment_mapping.hlsli"
#include "..\material_models\organism.hlsli"

void calc_dynamic_lighting_organism_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
	float l_dot_n = dot(common_data.light_direction, common_data.surface_normal);
	color = common_data.light_intensity * l_dot_n * common_data.albedo.rgb;
}

float3 calc_lighting_organism_ps(SHADER_COMMON common_data, out float4 unknown_output)
{
	float3 color = 0;
	unknown_output = 0;
	return color;
}
#endif