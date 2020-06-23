#ifndef _GLASS_LIGHTING_HLSLI
#define _GLASS_LIGHTING_HLSLI

#include "..\material_models\material_shared_parameters.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\sh.hlsli"

void calc_dynamic_lighting_glass_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
    color = 0;
}


float3 calc_lighting_glass_ps(SHADER_COMMON common_data, out float4 unknown_output)
{
    unknown_output = 0;
    return 0;
}


#endif