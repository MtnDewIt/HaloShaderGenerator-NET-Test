#ifndef _NO_LIGHTING_HLSLI
#define _NO_LIGHTING_HLSLI

#include "..\material_models\material_shared_parameters.hlsli"
#include "..\helpers\input_output.hlsli"

float3 calc_lighting_none_ps(SHADER_COMMON common_data)
{
	return 0;
}
#endif