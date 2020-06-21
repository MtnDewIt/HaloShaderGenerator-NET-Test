#ifndef _NO_MATERIAL_LIGHTING_HLSLI
#define _NO_MATERIAL_LIGHTING_HLSLI

#include "..\helpers\input_output.hlsli"
#include "..\helpers\sh.hlsli"
#include "..\methods\environment_mapping.hlsli"


float3 calc_lighting_no_material_ps(SHADER_COMMON common_data, inout float3 color)
{
	float3 env_band_0 = get_environment_contribution(common_data.sh_0);
	envmap_type(common_data.view_dir, common_data.reflect_dir, env_band_0, color);
	return color;
}
#endif