#ifndef _HALOGRAM_STATIC_PER_VERTEX_HLSLI
#define _HALOGRAM_STATIC_PER_VERTEX_HLSLI

#include "..\helpers\halogram_helper.hlsli"

#include "..\helpers\input_output.hlsli"
#include "..\methods\albedo_halogram.hlsli"
#include "..\methods\warp.hlsli"
#include "..\methods\self_illumination.hlsli"

#if (self_illumination_arg == k_self_illumination_simple && blend_type_arg != k_blend_mode_opaque) || (self_illumination_arg != k_self_illumination_off && self_illumination_arg != k_self_illumination_simple)
uniform bool actually_calc_albedo : register(b12);
#define actually_calc_albedo_visible 1
#endif

#if albedo_arg == k_albedo_two_detail_overlay || self_illumination_arg == k_self_illumination_from_diffuse || self_illumination_arg == k_self_illumination_self_illum_times_diffuse
#define sample_albedo 1
#else
#define sample_albedo 0
#endif

#include "..\methods\overlay.hlsli"
#include "..\methods\edge_fade.hlsli"

PS_OUTPUT_DEFAULT halogram_entry_static_per_vertex(VS_OUTPUT_PER_VERTEX input)
{
    float4 albedo = float4(0, 0, 0, 1);
    
    float3 normal;
    
    float3 camera_dir = normalize(input.camera_dir);
    
    float view_tangent = dot(input.tangent, camera_dir);
    float view_binormal = dot(input.binormal, camera_dir);
    
    float2 texcoord = calc_warp(input.texcoord.xy, input.camera_dir, input.tangent, input.binormal, input.normal.xyz);
    
    float3 sky_radiance = float3(input.texcoord.zw, input.extinction_factor.w);
    
#ifdef actually_calc_albedo_visible
    if (actually_calc_albedo)
    {
        normal = input.normal;
        if (sample_albedo == 1)
        {
			albedo = calc_albedo_ps(texcoord, input.position.xy, input.normal.xyz, input.camera_dir);
        }
    }
    else
    {
        float2 position = input.position.xy;
        position += 0.5;
        float2 inv_texture_size = (1.0 / texture_size);
        float2 frag_texcoord = position * inv_texture_size;
        float4 normal_texture_sample = tex2D(normal_texture, frag_texcoord);
        normal = normal_import(normal_texture_sample.xyz);
        if (sample_albedo == 1)
        {
            float4 albedo_texture_sample = tex2D(albedo_texture, frag_texcoord);
			albedo = albedo_texture_sample;
        }
    }
#else
    float2 position = input.position.xy;
    position += 0.5;
    float2 inv_texture_size = (1.0 / texture_size);
    float2 frag_texcoord = position * inv_texture_size;
    float4 normal_texture_sample = tex2D(normal_texture, frag_texcoord);
    normal = normal_import(normal_texture_sample.xyz);
#endif
    
    normal = normalize(normal);
    
    float view_normal = dot(camera_dir, normal);
    
    float4 color = albedo;
    
    // TODO: find proper condition
    if (albedo_arg == k_albedo_two_detail_overlay)
        color.rgb = 0;
    
	calc_self_illumination_ps(texcoord.xy, albedo.rgb, view_tangent, view_binormal, color.rgb);
    
    calc_overlay_ps(texcoord.xy, albedo.rgb, color.rgb);
    
    float3 edge_fade_color = calc_edge_fade_ps(view_normal, normal);
    color.rgb *= edge_fade_color;

    color.rgb = color.rgb * input.extinction_factor.rgb;
    
    // calculate_alpha_blending
    if (blend_type_arg == k_blend_mode_additive)
    {
        color.a = 0.0;
    }
    else if (blend_type_arg == k_blend_mode_alpha_blend)
    {
        color.a = albedo.a;
    }
    else
    {
        color.a = 1.0f;
    }
	
    if (blend_type_arg != k_blend_mode_additive)
    {
        color.rgb += sky_radiance.rgb;
    }

    if (blend_type_arg == k_blend_mode_double_multiply)
        color.rgb *= 2;

    color.rgb = expose_color(color.rgb);

    PS_OUTPUT_DEFAULT output = export_color(color);
    
    return output;
}

#endif