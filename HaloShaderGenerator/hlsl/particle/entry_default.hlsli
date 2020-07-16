#ifndef _PARTICLE_HLSLI
#define _PARTICLE_HLSLI

#include "particle.hlsli"
#include "../helpers/color_processing.hlsli"

PS_OUTPUT_DEFAULT particle_entry_default_ldr_and_hdr(VS_OUTPUT_PARTICLE input)
{    
    float4 color = particle_entry_default_main(input);
    
    PS_OUTPUT_DEFAULT output;
    
    output.low_frequency = export_low_frequency(color);
    output.high_frequency = export_high_frequency(color);
    
    output.unknown = 0;
    return output;
}

PS_OUTPUT_DEFAULT_LDR_ONLY particle_entry_default_ldr_only(VS_OUTPUT_PARTICLE input)
{
    float4 color = particle_entry_default_main(input);
    
    PS_OUTPUT_DEFAULT_LDR_ONLY output;
    
    output.low_frequency = export_low_frequency(color);
    
    return output;
}

#endif