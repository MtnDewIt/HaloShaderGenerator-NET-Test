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
    
    float4 albedo = particle_albedo(input.texcoord, input.o4.x, input.o2.rgb);
    
    float color_alpha;
    if (black_point_arg == k_black_point_on)
    {
        float r0_x = 1 / (-input.o4.y + 0.5f * (1 - -input.o4.y));
        float r0_y = albedo.a - input.o4.y;
        r0_x = saturate(r0_y * r0_x);
        r0_y = 1.0f + input.o4.y;
        float r0_z = r0_y * 0.5f;
        r0_y = saturate(-(r0_y * 0.5f) + albedo.a);
        color_alpha = r0_z * r0_x + r0_y;
    }
    else
    {
        color_alpha = albedo.a;
    }
    
    float4 color;
    color.rgb = albedo.rgb * input.color.rgb;
    color.a = color_alpha * input.color.a;
    
    if (lighting_arg == k_lighting_per_pixel_ravi_order_3)
    {
        diffuse_reflectance_ravi_order_3(normal, color);
    }
    
    color.rgb += input.o2.rgb;
    
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