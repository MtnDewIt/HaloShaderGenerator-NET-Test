#ifndef _ALBEDO_FX_HLSLI
#define _ALBEDO_FX_HLSLI

#include "../helpers/definition_helper.hlsli"
#include "../helpers/types.hlsli"

// hardcoding so registers line up
// TODO: fix
#ifdef _DECAL
uniform sampler tex0_sampler : register(s0);
uniform sampler tex1_sampler : register(s1);
#endif

uniform sampler base_map; // no transform, except in vector_alpha
uniform sampler alpha_map; // no transform
uniform sampler palette; // no transform
uniform sampler change_color_map; // no transform
uniform sampler vector_map; // no transform

uniform float3 primary_change_color;
uniform float3 secondary_change_color;
uniform float3 tertiary_change_color;
uniform float antialias_tweak;
uniform float vector_sharpness;
uniform float4 base_map_xform;

float4 albedo_diffuse_only(float2 texcoord, float2 unknown_texcoord, float palettized_w)
{
    float4 albedo = tex2D(base_map, texcoord);
    
    return albedo;
}

float4 albedo_palettized(float2 texcoord, float2 unknown_texcoord, float palettized_w)
{
    float4 albedo = albedo_diffuse_only(texcoord, unknown_texcoord, palettized_w);
    albedo = tex2D(palette, float2(albedo.r, palettized_w));
    
    return albedo;
}

float4 albedo_palettized_plus_alpha(float2 texcoord, float2 unknown_texcoord, float palettized_w)
{
    float4 albedo = albedo_diffuse_only(texcoord, unknown_texcoord, palettized_w);
    albedo = tex2D(palette, float2(albedo.x, palettized_w));
    
    float4 alpha_map_sample = tex2D(alpha_map, texcoord);
    albedo.a = alpha_map_sample.a;
    
    return albedo;
}

float4 albedo_diffuse_plus_alpha(float2 texcoord, float2 unknown_texcoord, float palettized_w)
{
    float4 albedo = albedo_diffuse_only(texcoord, unknown_texcoord, palettized_w);
    
    float4 alpha_map_sample = tex2D(alpha_map, texcoord);
    albedo.a = alpha_map_sample.a;
    
    return albedo;
}

#ifdef _DECAL
float4 albedo_emblem_change_color(float2 texcoord, float2 unknown_texcoord, float palettized_w)
{
    float4 albedo = tex2D(tex0_sampler, texcoord);
    
    // theres likely more code here, todo: check x360 shaders

    return albedo;
}
#endif

float4 albedo_change_color(float2 texcoord, float2 unknown_texcoord, float palettized_w)
{
    float4 albedo = tex2D(change_color_map, texcoord);
    
    float3 primary_color = albedo.r * primary_change_color.rgb + (1.0f - albedo.r);
    float3 secondary_color = albedo.g * secondary_change_color.rgb + (1.0f - albedo.g);
    float3 tertiary_color = albedo.b * tertiary_change_color.rgb + (1.0f - albedo.b);
    
    float3 final_color = primary_color * secondary_color * tertiary_color;
    
    return float4(final_color, albedo.a);
}

float4 albedo_diffuse_plus_alpha_mask(float2 texcoord, float2 unknown_texcoord, float palettized_w)
{
    float4 albedo = albedo_diffuse_only(texcoord, unknown_texcoord, palettized_w);

    float4 alpha_mask = tex2D(alpha_map, unknown_texcoord);
    
    return float4(albedo.rgb, alpha_mask.a);
}

// warning: no reference, may be incorrect
float4 palettized_plus_alpha_mask(float2 texcoord, float2 unknown_texcoord, float palettized_w)
{
    float4 albedo = albedo_palettized(texcoord, unknown_texcoord, palettized_w);

    float4 alpha_mask = tex2D(alpha_map, unknown_texcoord);
    
    return float4(albedo.rgb, alpha_mask.a);
}

float4 albedo_vector_alpha(float2 texcoord, float2 unknown_texcoord, float palettized_w)
{
    float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
    float4 albedo = tex2D(base_map, base_map_texcoord);

    float antialias = max(1000.f * antialias_tweak, 1.0f);
    float sharpness = min(vector_sharpness, antialias);
    float4 vector_sample = tex2D(vector_map, texcoord);
    float alpha = saturate((vector_sample.g * -0.5f) * sharpness + 0.5f);
    
    return float4(albedo.rgb, alpha);
}

#ifndef contrail_albedo
#define contrail_albedo albedo_diffuse_only
#endif

#ifndef beam_albedo
#define beam_albedo albedo_diffuse_only
#endif

#ifndef light_volume_albedo
#define light_volume_albedo albedo_diffuse_only
#endif

#ifndef decal_albedo
#define decal_albedo albedo_diffuse_only
#endif

#endif