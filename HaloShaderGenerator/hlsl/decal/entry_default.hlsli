#ifndef _DECAL_HLSLI
#define _DECAL_HLSLI

#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"
#include "..\helpers\decal_helper.hlsli"

// TODO: fix errors when these are in separate file

#define k_decal_albedo_diffuse_only 0
#define k_decal_albedo_palettized 1
#define k_decal_albedo_palettized_plus_alpha 2
#define k_decal_albedo_diffuse_plus_alpha 3
#define k_decal_albedo_emblem_change_color 4
#define k_decal_albedo_change_color 5 
#define k_decal_albedo_diffuse_plus_alpha_mask 6 
#define k_decal_albedo_palettized_plus_alpha_mask 7
#define k_decal_albedo_vector_alpha 8
#define k_decal_albedo_vector_alpha_drop_shadow 9

#define k_decal_render_pass_pre_lighting 0
#define k_decal_render_pass_post_lighting 1

#define k_decal_specular_leave 0
#define k_decal_specular_modulate 1

#define k_decal_bump_mapping_leave 0
#define k_decal_bump_mapping_standard 1
#define k_decal_bump_mapping_standard_mask 2

#include "..\methods\albedo_decal.hlsli"
#include "..\methods\blend_mode_decal.hlsli"
#include "..\methods\bump_mapping_decal.hlsli"
#include "..\methods\tinting.hlsli"

#include "..\registers\global_parameters.hlsli"

void decal_apply_fade(inout float4 color)
{ 
    // todo: figure out exactly what conditions are here
    
    bool blend_is_multiply = decal_blend_type_arg == k_decal_blend_mode_pre_multiplied_alpha || decal_blend_type_arg == k_decal_blend_mode_multiply;
    
    if (decal_render_pass_arg == k_decal_render_pass_post_lighting && !blend_is_multiply)
    {
        color.rgb *= g_alt_exposure.y;
    }
    
    if (decal_blend_type_arg == k_decal_blend_mode_additive)
    {
        color.rgb *= fade;
    }
    if (decal_blend_type_arg == k_decal_blend_mode_pre_multiplied_alpha)
    {
        color.rgb *= fade;
        if (decal_render_pass_arg == k_decal_render_pass_post_lighting)
            color.rgb *= color.a;
    }
    
    // this seems weird
    if ((decal_blend_type_arg == k_decal_blend_mode_multiply || decal_blend_type_arg == k_decal_blend_mode_additive) && decal_specular_arg == k_decal_specular_modulate)
    {
        color.a *= fade;
    }
}

float4 decal_entry_default_calculate_color(VS_OUTPUT_DECAL input)
{
    float4 color = decal_albedo(input.texcoord.zw, input.texcoord.xy, 0.0f);
    
    // apply fully modulated tint
    if (decal_tinting_arg == k_decal_tinting_fully_modulated)
        decal_tinting(color);
    
    if (decal_render_pass_arg == k_decal_render_pass_post_lighting && decal_blend_type_arg == k_decal_blend_mode_pre_multiplied_alpha)
        color.rgb *= g_exposure.x;
    
    color = decal_blend_mode(color, fade);
    
    // apply tint
    if (decal_tinting_arg != k_decal_tinting_fully_modulated)
        decal_tinting(color);
    
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
        if (decal_blend_type_arg == k_decal_blend_mode_multiply)
        {
            output.color_ldr = color * g_exposure.w;
            output.color_hdr = color * g_exposure.z;
        }
        else
        {
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
        output.unknown = input.texcoord1.x;
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