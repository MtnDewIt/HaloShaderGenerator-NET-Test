#ifndef _EDGE_FADE_HLSLI
#define _EDGE_FADE_HLSLI

#pragma warning( disable : 3571 34)

uniform float3 edge_fade_center_tint;
uniform float3 edge_fade_edge_tint;
uniform float edge_fade_power;

float3 calc_edge_fade_none_ps(in float n_view_dir, in float3 normal)
{
    return float3(1, 1, 1);
}

float3 calc_edge_fade_simple_ps(in float n_view_dir, in float3 normal)
{
    float3 tint = edge_fade_center_tint - edge_fade_edge_tint;
    tint = pow(n_view_dir, edge_fade_power) * tint; // * tint.rgb + edge_fade_edge_tint;
    tint += edge_fade_edge_tint;

    return tint;
}

#ifndef calc_edge_fade_ps
#define calc_edge_fade_ps calc_edge_fade_none_ps
#endif

#endif