#ifndef _OVERLAY_HLSLI
#define _OVERLAY_HLSLI

#include "../helpers/math.hlsli"
#include "../helpers/types.hlsli"

uniform sampler overlay_map;
uniform float4 overlay_map_xform;
uniform float4 overlay_tint;
uniform float overlay_intensity;
uniform sampler overlay_detail_map;
uniform float4 overlay_detail_map_xform;
uniform sampler overlay_multiply_map;
uniform float4 overlay_multiply_map_xform;

void calc_overlay_none_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{
}

void calc_overlay_additive_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{
    float2 overlay_map_texcoord = apply_xform2d(texcoord, overlay_map_xform);
    float4 overlay_sample = tex2D(overlay_map, overlay_map_texcoord);
    
    float3 overlay_color = overlay_sample.rgb;
    overlay_color *= overlay_tint.rgb * overlay_intensity;

    diffuse += overlay_color;
}

void calc_overlay_additive_detail_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{
    float2 overlay_map_texcoord = apply_xform2d(texcoord, overlay_map_xform);
    float4 overlay_sample = tex2D(overlay_map, overlay_map_texcoord);
    
    float2 overlay_detail_map_texcoord = apply_xform2d(texcoord, overlay_detail_map_xform);
    float4 overlay_detail_sample = tex2D(overlay_detail_map, overlay_detail_map_texcoord);
    
    float3 overlay_color = overlay_sample.rgb * overlay_detail_sample.rgb;
    overlay_color *= overlay_tint.rgb;
    overlay_color *= overlay_intensity;
    overlay_color *= DETAIL_MULTIPLIER;

    diffuse += overlay_color;
}

void calc_overlay_multiply_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{
    float2 overlay_map_texcoord = apply_xform2d(texcoord, overlay_map_xform);
    float4 overlay_sample = tex2D(overlay_map, overlay_map_texcoord);
    
    float3 overlay_color = overlay_sample.rgb;
    overlay_color *= overlay_tint.rgb;
    overlay_color *= overlay_intensity;

    diffuse *= overlay_color;
}

void calc_overlay_multiply_and_additive_detail_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{
    float2 overlay_map_texcoord = apply_xform2d(texcoord, overlay_map_xform);
    float4 overlay_sample = tex2D(overlay_map, overlay_map_texcoord);
    
    float2 overlay_detail_map_texcoord = apply_xform2d(texcoord, overlay_detail_map_xform);
    float4 overlay_detail_sample = tex2D(overlay_detail_map, overlay_detail_map_texcoord);
    
    float3 overlay_color = overlay_sample.rgb * overlay_detail_sample.rgb;
    overlay_color *= overlay_tint.rgb;
    overlay_color *= overlay_intensity;
    overlay_color *= DETAIL_MULTIPLIER;
    
    float2 overlay_multiply_map_texcoord = apply_xform2d(texcoord, overlay_multiply_map_xform);
    float4 overlay_multiply_sample = tex2D(overlay_multiply_map, overlay_multiply_map_texcoord);
    
    diffuse *= overlay_multiply_sample.rgb;
    diffuse += overlay_color;
}

#ifndef calc_overlay_ps
#define calc_overlay_ps calc_overlay_none_ps
#endif

// SCREEN

uniform float4 tint_color;
uniform float4 add_color;
uniform sampler2D detail_map_a;
uniform float4 detail_map_a_xform;
uniform sampler2D detail_mask_a;
uniform float4 detail_mask_a_xform;
uniform float detail_fade_a;
uniform float detail_multiplier_a;

float4 screen_overlay_detail_ps(float2 texcoord, float4 base)
{
    float4 detail = tex2D(detail_map_a, apply_xform2d(texcoord, detail_map_a_xform));
    detail.rgb *= detail_multiplier_a;
    
    detail -= 1.0f;
    detail = detail_fade_a * detail;
    detail += 1.0f;
    
    return base * detail;
}

float4 screen_overlay_detail_masked(float2 texcoord, float4 base)
{
    float4 detail = tex2D(detail_map_a, apply_xform2d(texcoord.xy, detail_map_a_xform));
    detail.rgb *= detail_multiplier_a;
    
    float detail_mask = tex2D(detail_mask_a, apply_xform2d(texcoord.xy, detail_mask_a_xform)).a;
    detail_mask = saturate(detail_mask * detail_fade_a);
    
    detail -= 1.0f;
    detail = detail_mask * detail;
    detail += 1.0f;
    
    return base * detail;
}


float4 overlay_none(float4 texcoord, float4 base)
{
    return base;
}

float4 overlay_tint_add_color(float4 texcoord, float4 base)
{
    return base * tint_color + add_color;
}

float4 overlay_detail_screen_space(float4 texcoord, float4 base)
{
    return screen_overlay_detail_ps(texcoord.xy, base);
}

float4 overlay_detail_pixel_space(float4 texcoord, float4 base)
{
    return screen_overlay_detail_ps(texcoord.zw, base);
}

float4 overlay_detail_masked_screen_space(float4 texcoord, float4 base)
{
    return screen_overlay_detail_masked(texcoord.xy, base);
}

//float4 overlay_detail_masked_pixel_space(float4 texcoord, float4 base)
//{
//    return screen_overlay_detail_masked(texcoord.zw, base);
//}

#ifndef overlay_type_a
#define overlay_type_a overlay_none
#endif

#ifndef overlay_type_b
#define overlay_type_b overlay_none
#endif

#endif