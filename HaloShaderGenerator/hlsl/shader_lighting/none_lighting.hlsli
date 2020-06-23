#ifndef _NO_LIGHTING_HLSLI
#define _NO_LIGHTING_HLSLI

#include "..\material_models\material_shared_parameters.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\sh.hlsli"
#include "..\methods\environment_mapping.hlsli"

void calc_dynamic_lighting_none_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
	float l_dot_n = dot(common_data.light_direction, common_data.surface_normal);
	color = common_data.light_intensity * l_dot_n * common_data.albedo.rgb; // lambertian diffuse
}

float3 calc_lighting_none_ps(SHADER_COMMON common_data, out float4 unknown_output)
{
	float3 color = 0;
	ENVIRONMENT_MAPPING_COMMON env_mapping_common_data;
	
	env_mapping_common_data.reflect_dir = common_data.reflect_dir;
	env_mapping_common_data.view_dir = common_data.view_dir;
	env_mapping_common_data.env_area_specular = get_environment_contribution(common_data.sh_0);
	env_mapping_common_data.specular_coefficient = 1.0;
	env_mapping_common_data.area_specular = 0;
	env_mapping_common_data.specular_exponent = 0.0;
	envmap_type(env_mapping_common_data, color, unknown_output);
	return color;
}
#endif