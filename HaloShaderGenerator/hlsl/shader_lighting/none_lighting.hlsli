#ifndef _NO_LIGHTING_HLSLI
#define _NO_LIGHTING_HLSLI

#include "..\material_models\material_shared_parameters.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\sh.hlsli"
#include "..\methods\environment_mapping.hlsli"

void calc_dynamic_lighting_none_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
	float v_dot_n = dot(common_data.light_direction, common_data.surface_normal);
	color = common_data.light_intensity * v_dot_n * common_data.albedo.rgb; // lambertian diffuse
}

float3 calc_lighting_none_ps(SHADER_COMMON common_data)
{
	float3 color = 0;
	float3 env_band_0 = get_environment_contribution(common_data.sh_0);
	envmap_type(common_data.view_dir, common_data.reflect_dir, env_band_0, color);
	return color;
}
#endif