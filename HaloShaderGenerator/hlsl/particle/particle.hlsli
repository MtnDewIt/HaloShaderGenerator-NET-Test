#ifndef _PARTICLE_SHADER_HLSL
#define _PARTICLE_SHADER_HLSL

#include "..\particle_lighting\ravi_order_3.hlsli"

#include "..\methods\depth_fade.hlsli"
#include "..\methods\albedo_particle.hlsli"
#include "..\methods\black_point.hlsli"
#include "..\methods\fog.hlsli"

#include "..\helpers\particle_helper.hlsli"

float4 particle_entry_default_main(VS_OUTPUT_PARTICLE input)
{
    float3 normal = normalize(input.normal.xyz);
    
    float4 color = particle_albedo(input.texcoord, input.parameters.x);
    
    if (depth_fade_arg == k_depth_fade_on)
    {
        depth_fade_on(color.a, input.position.xy, input.color2.w);
    }
    if (black_point_arg == k_black_point_on)
    {
        black_point_on(color.a, input.parameters.y);
    }
    
    color.rgba *= input.color.rgba;
    
    if (lighting_arg == k_lighting_per_pixel_ravi_order_3)
    {
        per_pixel_ravi_order_3(normal, color);
    }
    
    color.rgb += input.color2.rgb;
    
    if (particle_blend_type_arg == 10)
        color.rgb *= color.a;
    
    return color;
}

#endif