#ifndef _ALBEDO_FX_HLSLI
#define _ALBEDO_FX_HLSLI

#include "../helpers/definition_helper.hlsli"
#include "../helpers/types.hlsli"

uniform sampler base_map; // no transform
uniform sampler palette; // no transform
uniform sampler alpha_map; // no transform

float4 albedo_diffuse_only(float2 texcoord, float palettized_w)
{
    float4 albedo = tex2D(base_map, texcoord);
    
    return albedo;
}

// no reference for this
float4 albedo_palettized(float2 texcoord, float palettized_w)
{
    float4 albedo = albedo_diffuse_only(texcoord, palettized_w);
    albedo = tex2D(palette, float2(albedo.x, palettized_w));
    
    return albedo;
}

float4 albedo_palettized_plus_alpha(float2 texcoord, float palettized_w)
{
    float4 albedo = albedo_diffuse_only(texcoord, palettized_w);
    albedo = tex2D(palette, float2(albedo.x, palettized_w));
    
    float4 alpha_map_sample = tex2D(alpha_map, texcoord);
    albedo.a = alpha_map_sample.w;
    
    return albedo;
}

#ifndef contrail_albedo
#define contrail_albedo albedo_diffuse_only
#endif

#ifndef beam_albedo
#define beam_albedo albedo_diffuse_only
#endif

#endif