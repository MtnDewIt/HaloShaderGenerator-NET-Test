#ifndef _CONTRAIL_HLSLI
#define _CONTRAIL_HLSLI

#include "../methods/albedo_fx.hlsli"
#include "../helpers/definition_helper.hlsli"
#include "../helpers/color_processing.hlsli"
#include "../methods/black_point.hlsli"

#define k_beam_albedo_diffuse_only 0
#define k_beam_albedo_palettized 1
#define k_beam_albedo_palettized_plus_alpha 2
#define k_beam_albedo_palettized_plasma 3
#define k_beam_albedo_palettized_2d_plasma 4

#define k_beam_blend_mode_opaque 0
#define k_beam_blend_mode_additive 1
#define k_beam_blend_mode_multiply 2
#define k_beam_blend_mode_alpha_blend 3
#define k_beam_blend_mode_double_multiply 4
#define k_beam_blend_mode_maximum 5
#define k_beam_blend_mode_multiply_add 6
#define k_beam_blend_mode_add_src_times_dstalpha 7
#define k_beam_blend_mode_add_src_times_srcalpha 8
#define k_beam_blend_mode_inv_alpha_blend 9
#define k_beam_blend_mode_pre_multiplied_alpha 10

PS_OUTPUT_DEFAULT beam_entry_default(VS_OUTPUT_FX input)
{
    float4 color = beam_albedo(input.texcoord.xy, float2(0, 0), input.texcoord.z);
    
    if (black_point_arg == k_black_point_on)
    {
        black_point_on(color.a, input.color2.w);
    }
    
    if (beam_blend_type_arg == k_beam_blend_mode_multiply)
    {
        color *= input.color;
        color.rgb -= 1.0f;
        color.rgb = color.a * color.rgb + 1.0f;
    }
    else
    {
        color *= input.color;
    }
    
    if (beam_blend_type_arg != k_beam_blend_mode_multiply)
    {
        color.rgb += input.color2.rgb;
    }
    
    if (beam_blend_type_arg == k_beam_blend_mode_pre_multiplied_alpha)
        color.rgb *= color.a;
    
    PS_OUTPUT_DEFAULT output;

    if (beam_blend_type_arg == k_beam_blend_mode_multiply)
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