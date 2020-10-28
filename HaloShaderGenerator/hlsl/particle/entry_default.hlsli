#ifndef _PARTICLE_HLSLI
#define _PARTICLE_HLSLI

#include "particle.hlsli"
#include "../helpers/particle_helper.hlsli"
#include "../helpers/definition_helper.hlsli"
#include "../helpers/color_processing.hlsli"

PS_OUTPUT_DEFAULT particle_entry_default_ldr_and_hdr(VS_OUTPUT_PARTICLE input)
{
    PS_OUTPUT_DEFAULT output;
    
    if (specialized_rendering_arg == k_specialized_rendering_none)
    {
        float4 color = particle_entry_default_main(input);
        
        if (blend_type_arg == k_blend_mode_multiply)
        {
            output.low_frequency = color * g_exposure.w;
            output.high_frequency = color * g_exposure.z;
        }
        else
        {
            output.low_frequency = export_low_frequency(color);
            output.high_frequency = export_high_frequency(color);
        }
    }
    else
    {
        float4 color = particle_entry_default_distortion(input);
        output.low_frequency.xy = color.xy;
        output.low_frequency.z = 0;
        output.low_frequency.w = 1;
        output.high_frequency = 0;
    }
    
    output.unknown = 0;
    return output;
}

PS_OUTPUT_DEFAULT_LDR_ONLY particle_entry_default_ldr_only(VS_OUTPUT_PARTICLE input)
{
    PS_OUTPUT_DEFAULT_LDR_ONLY output;
    
    float4 color;
    if (specialized_rendering_arg == k_specialized_rendering_none)
    {
        color = particle_entry_default_main(input);
        
        if (blend_type_arg == k_blend_mode_multiply)
        {
            output.low_frequency = color * g_exposure.w;
        }
        else
        {
            output.low_frequency = export_low_frequency(color);
        }
    }
    else
    {
        color = particle_entry_default_distortion(input);
        output.low_frequency.xy = color.xy;
        output.low_frequency.z = 0;
        output.low_frequency.w = 1;
    }
    
    return output;
}

#endif