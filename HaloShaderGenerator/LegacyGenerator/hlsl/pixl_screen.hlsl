#define particle_template

#include "helpers\definition_helper.hlsli"
#include "helpers\input_output.hlsli"

#if shaderstage == k_shaderstage_default
#include "screen/entry_default.hlsli"
float4 entry_default(VS_OUTPUT_SCREEN input) : COLOR
{
    return screen_entry_default(input);
}
#endif