#ifndef _BASE_HLSLI
#define _BASE_HLSLI

#include "../helpers/types.hlsli"

uniform sampler2D base_map;
uniform float4 base_map_xform;

float4 calc_base_single_screen_space(float4 texcoord)
{
    return tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
}

float4 calc_base_single_pixel_space(float4 texcoord)
{
    return tex2D(base_map, apply_xform2d(texcoord.zw, base_map_xform));
}

#ifndef calc_base
#define calc_base calc_base_single_screen_space
#endif

#endif