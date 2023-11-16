#ifndef _ALBEDO_HLSLI
#define _ALBEDO_HLSLI

#include "../helpers/types.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/color_processing.hlsli"
#include "../helpers/terrain_helper.hlsli"

uniform float4 debug_tint;

float3 apply_debug_tint(float3 color)
{
	float3 negative_tinted_color = debug_tint.rgb - color;
	return color + debug_tint.a * negative_tinted_color;
}

float4 calc_terrain_albedo(float2 texcoord, sampler base_map, xform2d base_map_xform, sampler detail_map, xform2d detail_map_xform, bool has_illum)
{
	float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
	float4 base_map_sample = tex2D(base_map, base_map_texcoord);
    float4 detail_map_sample;
	
    if (has_illum)
    {
        detail_map_sample = 1.0f / DETAIL_MULTIPLIER;
    }
	else
    {
        float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);
        detail_map_sample = tex2D(detail_map, detail_map_texcoord);
    }
	
	float4 albedo = detail_map_sample * base_map_sample;
	return albedo;
}

#endif