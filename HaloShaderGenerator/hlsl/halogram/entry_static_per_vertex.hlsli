#ifndef _HALOGRAM_STATIC_PER_VERTEX_HLSLI
#define _HALOGRAM_STATIC_PER_VERTEX_HLSLI

#include "..\helpers\input_output.hlsli"
#include "..\methods\albedo.hlsli"
#include "..\methods\warp.hlsli"
#include "..\methods\self_illumination.hlsli"
#include "..\methods\overlay.hlsli"
#include "..\methods\edge_fade.hlsli"
#include "..\methods\soft_fade.hlsli"

PS_OUTPUT_DEFAULT halogram_entry_static_per_vertex(VS_OUTPUT_PER_VERTEX input)
{
    float4 albedo = float4(0, 0, 0, 1);
    
    float3 normal;
    
    float3 camera_dir = normalize(input.camera_dir);
    
    float view_tangent = dot(input.tangent, camera_dir);
    float view_binormal = dot(input.binormal, camera_dir);
    
    float2 texcoord = calc_warp(input.texcoord.xy, input.camera_dir, input.tangent, input.binormal, input.normal.xyz);
    
    float3 sky_radiance = float3(input.texcoord.zw, input.extinction_factor.w);
    
    if (actually_calc_albedo)
    {
        normal = input.normal;
        albedo = calc_albedo_ps(texcoord, input.position.xy, input.normal.xyz, input.camera_dir);
        apply_soft_fade(albedo, dot(camera_dir, normalize(normal)), input.position);
    }
    else
    {
        float2 position = input.position.xy;
        position += 0.5;
        float2 inv_texture_size = (1.0 / texture_size);
        float2 frag_texcoord = position * inv_texture_size;
        float4 normal_texture_sample = tex2D(normal_texture, frag_texcoord);
        normal = normal_import(normal_texture_sample.xyz);
        float4 albedo_texture_sample = tex2D(albedo_texture, frag_texcoord);
        albedo = albedo_texture_sample;
    }
    
    normal = normalize(normal);
    
    float view_normal = dot(camera_dir, normal);
    
    float4 color = 0;
    
	calc_self_illumination_ps(input.position.xy, texcoord.xy, albedo.rgb, 0, input.camera_dir.xyz, view_normal, view_tangent, view_binormal, color.rgb);
    
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