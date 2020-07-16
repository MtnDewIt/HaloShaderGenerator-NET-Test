#ifndef _PARTICLE_HLSLI
#define _PARTICLE_HLSLI

#include "..\methods\depth_fade.hlsli"
#include "..\methods\albedo.hlsli"
#include "..\methods\black_point.hlsli"
#include "..\methods\fog.hlsli"

#include "../helpers/particle_helper.hlsli"
#include "../helpers/definition_helper.hlsli"
#include "../helpers/types.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/color_processing.hlsli"
#include "../helpers/sh.hlsli"

float4 particle_entry_default_main(VS_OUTPUT_PARTICLE input)
{
    float3 normal = normalize(input.normal.xyz);
    
    float4 color = particle_albedo(input.texcoord, input.o4.x, input.o2.rgb);
    
    if (depth_fade_arg == k_depth_fade_on)
    {
        // would rather not input these global registers but cant use in the hlsli file???
        color.a = depth_fade_on(color.a, input.position.xy, input.o2.w, texture_size.xy, depth_buffer);
    }
    if (black_point_arg == k_black_point_on)
    {
        color.a = black_point_on(color.a, input.o4.y);
    }
    
    color.rgba *= input.color.rgba;
    
    if (lighting_arg == k_lighting_per_pixel_ravi_order_3)
    {
        diffuse_reflectance_ravi_order_3(normal, color);
    }
    
    color.rgb += input.o2.rgb;
    
    if (particle_blend_type_arg == 10)
        color.rgb *= color.a;
    
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