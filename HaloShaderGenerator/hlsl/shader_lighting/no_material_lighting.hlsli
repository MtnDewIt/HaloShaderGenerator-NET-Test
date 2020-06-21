#ifndef _NO_MATERIAL_LIGHTING_HLSLI
#define _NO_MATERIAL_LIGHTING_HLSLI

#include "..\helpers\input_output.hlsli"
#include "..\helpers\sh.hlsli"
#include "..\methods\environment_mapping.hlsli"


float3 calc_lighting_no_material_ps(SHADER_COMMON common_data, inout float3 color, out float3 unknown_output)
{
	ENVIRONMENT_MAPPING_COMMON env_mapping_common_data;
	
	env_mapping_common_data.reflect_dir = common_data.reflect_dir;
	env_mapping_common_data.view_dir = common_data.view_dir;
	env_mapping_common_data.sh_0_env_color = get_environment_contribution(common_data.sh_0);
	env_mapping_common_data.specular_coefficient = 1.0;
	env_mapping_common_data.area_specular = 0;

	envmap_type(env_mapping_common_data, color, unknown_output);
	
	return color;
}
#endif