#ifndef _PARTICLE_HLSLI
#define _PARTICLE_HLSLI

#include "..\methods\albedo.hlsli"
#include "..\methods\fog.hlsli"

#include "../helpers/definition_helper.hlsli"
#include "../helpers/types.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/color_processing.hlsli"
#include "../helpers/sh.hlsli"

float4 particle_entry_default_main(VS_OUTPUT_PARTICLE input)
{
    float3 normal = normalize(input.normal.xyz);
    
    float4 albedo = particle_albedo(input.texcoord.xy, input.color, input.o2.rgb);
    
    if (lighting_arg == k_lighting_per_pixel_ravi_order_3)
    {
        diffuse_reflectance_ravi_order_3(normal, albedo);
    }
    
    float4 color;
    color.rgb = albedo.rgb + input.o2.rgb;
    color.a = albedo.a;
    
    return color;
}

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