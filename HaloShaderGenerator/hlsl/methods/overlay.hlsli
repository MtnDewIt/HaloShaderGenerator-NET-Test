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
    //overlay_color *= overlay_intensity;

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

#endif