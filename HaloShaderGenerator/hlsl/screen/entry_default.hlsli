#ifndef _SCREEN_HLSLI
#define _SCREEN_HLSLI

#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\input_output.hlsli"

#include "..\registers\global_parameters.hlsli"

#include "..\methods\warp.hlsli"
#include "..\methods\base.hlsli"
#include "..\methods\overlay.hlsli"

uniform float fade;

float4 calc_screen_blending(float4 color)
{
    if (blend_type_arg == k_blend_mode_additive)
    {
        return (color * fade) * float4(1, 1, 1, 0.03125f);
    }
    if (blend_type_arg == k_blend_mode_multiply)
    {
        return (color * fade + (1.0f - fade)) * 0.03125f;
    }
    // TODO
    if (blend_type_arg == k_blend_mode_alpha_blend)
    {
    }
    if (blend_type_arg == k_blend_mode_double_multiply)
    {
    }
    if (blend_type_arg == k_blend_mode_pre_multiplied_alpha)
    {
    }
    
    return color * float4(1, 1, 1, 0.03125f);
}

float4 screen_entry_default(VS_OUTPUT_SCREEN input)
{
    float2 warp = calc_screen_warp(input.texcoord.xy);
    float2 texcoord = warp + input.texcoord.xy;
    
    float4 base = calc_base(texcoord, input.texcoord.zw, warp);
    
    float4 overlay_a = overlay_type_a(texcoord, base);
    float4 overlay_b = overlay_type_b(texcoord, overlay_a);

    return calc_screen_blending(overlay_b);
}

#endif