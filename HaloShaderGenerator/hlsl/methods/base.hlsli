#ifndef _BASE_HLSLI
#define _BASE_HLSLI

#include "../helpers/types.hlsli"

uniform float4 screenspace_xform : register(c200);
uniform sampler2D base_map;
uniform float4 base_map_xform;

float4 calc_base_single_screen_space(float2 texcoord, float2 screenspace_offset, float2 warp_tex)
{
    return tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
}

float4 calc_base_single_pixel_space(float2 texcoord, float2 screenspace_offset, float2 warp_tex)
{
    float2 screen_tex = apply_xform2d(warp_tex, float4(rcp(screenspace_xform.xy), screenspace_offset));
    return tex2D(base_map, apply_xform2d(screen_tex, base_map_xform));
}

#ifndef calc_base
#define calc_base calc_base_single_screen_space
#endif

#endif