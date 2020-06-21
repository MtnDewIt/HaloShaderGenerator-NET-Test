#ifndef _NO_LIGHTING_HLSLI
#define _NO_LIGHTING_HLSLI

#include "..\material_models\material_shared_parameters.hlsli"
#include "..\helpers\input_output.hlsli"

void calc_dynamic_lighting_none_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
	float v_dot_n = dot(common_data.light_direction, common_data.surface_normal);
	color = common_data.light_intensity * v_dot_n * common_data.albedo.rgb; // lambertian diffuse
}

float3 calc_lighting_none_ps(SHADER_COMMON common_data)
{
	return 0;
}
#endif