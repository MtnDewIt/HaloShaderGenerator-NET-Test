#define particle_template

#include "helpers\definition_helper.hlsli"
#include "helpers\input_output.hlsli"

#if shaderstage == k_shaderstage_default
#include "particle/entry_default.hlsli"
PS_OUTPUT_PARTICLE entry_default(VS_OUTPUT_PARTICLE input) : COLOR
{	
    return particle_entry_default(input);
}
#endif