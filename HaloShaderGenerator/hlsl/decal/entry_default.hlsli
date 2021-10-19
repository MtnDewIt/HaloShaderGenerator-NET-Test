#ifndef _DECAL_HLSLI
#define _DECAL_HLSLI

#include "..\helpers\color_processing.hlsli"
#include "..\helpers\decal_helper.hlsli"

#define k_decal_render_pass_pre_lighting 0
#define k_decal_render_pass_post_lighting 1

#define k_decal_specular_leave 0
#define k_decal_specular_modulate 1

#define k_decal_bump_mapping_leave 0
#define k_decal_bump_mapping_standard 1
#define k_decal_bump_mapping_standard_mask 2

#include "..\methods\albedo_fx.hlsli"
#include "..\methods\blend_mode_decal.hlsli"
#include "..\methods\bump_mapping_decal.hlsli"
#include "..\methods\tinting.hlsli"

#include "..\helpers\definition_helper.hlsli"
#include "..\registers\global_parameters.hlsli"

#ifndef DECAL_FADE
uniform float fade : register(c32);
#define DECAL_FADE fade
#endif

#define APPLY_ALPHA_FADE (decal_bump_mapping_arg != k_decal_bump_mapping_leave || decal_specular_arg != k_decal_specular_leave)
#define APPLY_BLEND_FADE (blend_type_arg == k_blend_mode_alpha_blend || blend_type_arg == k_blend_mode_add_src_times_dstalpha || blend_type_arg == k_blend_mode_add_src_times_srcalpha        \
    || blend_type_arg == k_blend_mode_pre_multiplied_alpha || blend_type_arg == k_blend_mode_inv_alpha_blend)
#define BLEND_IS_MULTIPLY (blend_type_arg == k_blend_mode_multiply || blend_type_arg == k_blend_mode_double_multiply)

void decal_apply_fade(inout float4 color)
{
    color = decal_blend_mode(color, DECAL_FADE);
	
    if (APPLY_ALPHA_FADE && !APPLY_BLEND_FADE)
    {
        color.a *= DECAL_FADE;
    }
    
    if (blend_type_arg == k_blend_mode_pre_multiplied_alpha)
    {
        if (albedo_arg != k_albedo_vector_alpha_drop_shadow && albedo_arg != k_albedo_vector_alpha)
            color.rgb *= color.a;
        color.rgb *= DECAL_FADE;
    }
}

float4 decal_entry_default_calculate_color(VS_OUTPUT_DECAL input)
{
    // calc base diffuse
    float4 color = calc_albedo_ps(float4(input.texcoord.zw, 0, 0), input.texcoord.xy, 0.0f, 0.0f, 1.0f, 1.0f);
    // apply tint and modulate
    decal_tinting(color);
    // apply lighting modulation
    if (decal_render_pass_arg == k_decal_render_pass_post_lighting && !BLEND_IS_MULTIPLY)
    {
        if (blend_type_arg == k_blend_mode_additive || blend_type_arg == k_blend_mode_add_src_times_srcalpha)
            color.rgb *= g_alt_exposure.y;
        else
            color.rgb *= g_exposure.x;
    }
    // apply fade    
    decal_apply_fade(color);
    
    return color;
}

PS_OUTPUT_DECAL_SIMPLE decal_entry_default_simple(VS_OUTPUT_DECAL input)
{
    texcoord_clip(input.texcoord.xy);
    
    float4 color = decal_entry_default_calculate_color(input);
    
    PS_OUTPUT_DECAL_SIMPLE output;
    output.color = color;
    return output;
}

PS_OUTPUT_DECAL decal_entry_default(VS_OUTPUT_DECAL input)
{
    texcoord_clip(input.texcoord.xy);
    
    float4 color = decal_entry_default_calculate_color(input);
    
    PS_OUTPUT_DECAL output;
    
    if (decal_render_pass_arg == k_decal_render_pass_post_lighting)
    {
        if (BLEND_IS_MULTIPLY)
        {
            output.color_ldr = color * g_exposure.w;
            output.color_hdr = color * g_exposure.z;
        }
        else
        {
            if (blend_type_arg == k_blend_mode_pre_multiplied_alpha)
                color.rgb *= color.a;
            output.color_ldr = export_low_frequency(color);
            output.color_hdr = export_high_frequency(color);
        }
        
        output.unknown = 0;
    }
    else if (decal_bump_mapping_arg != k_decal_bump_mapping_leave)
    {
        float3 bump_mapping = decal_bump_mapping(input.tangent.xyz, input.binormal.xyz, input.normal.xyz, input.texcoord.zw);
        
        output.color_ldr = color;
        output.color_hdr.rgb = bump_mapping;
        output.color_hdr.a = color.a;
        output.unknown = input.depth;
    }
    else
    {
        output.color_ldr = color;
        output.color_hdr = color;
        output.unknown = 0;
    }
    
    return output;
}

#endif