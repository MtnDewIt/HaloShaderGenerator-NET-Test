#ifndef _CONTRAIL_HLSLI
#define _CONTRAIL_HLSLI

#include "../methods/albedo_fx.hlsli"
#include "../helpers/definition_helper.hlsli"
#include "../helpers/color_processing.hlsli"
#include "../methods/black_point.hlsli"

PS_OUTPUT_DEFAULT beam_entry_default(VS_OUTPUT_FX input)
{
    float4 color = calc_albedo_ps(float4(input.texcoord.xy, 0, 0), float2(0, 0), input.texcoord.z, 0.0f, 1.0f);
    
    if (black_point_arg == k_black_point_on)
    {
        black_point_on(color.a, input.color2.w);
    }
    
    if (blend_type_arg == k_blend_mode_multiply)
    {
        color *= input.color;
        color.rgb -= 1.0f;
        color.rgb = color.a * color.rgb + 1.0f;
    }
    else
    {
        color *= input.color;
    }
    
    if (blend_type_arg != k_blend_mode_multiply)
    {
        color.rgb += input.color2.rgb;
    }
    
    if (blend_type_arg == k_blend_mode_pre_multiplied_alpha)
        color.rgb *= color.a;
    
    PS_OUTPUT_DEFAULT output;

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
    
    output.unknown = 0;
    return output;
}

#endif