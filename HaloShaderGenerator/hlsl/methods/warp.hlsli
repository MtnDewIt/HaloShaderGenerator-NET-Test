#ifndef _WARP_HLSLI
#define _WARP_HLSLI

#include "../helpers/math.hlsli"
#include "../helpers/types.hlsli"
#include "../helpers/definition_helper.hlsli"

uniform sampler warp_map;
uniform float4 warp_map_xform;
uniform float warp_amount_x;
uniform float warp_amount_y;

float2 calc_warp_none(float2 texcoord, float3 camera_dir, float3 tangent, float3 binormal, float3 normal)
{
    return texcoord;
}

float2 calc_warp_from_texture(float2 texcoord, float3 camera_dir, float3 tangent, float3 binormal, float3 normal)
{
    float2 warp_map_texcoord = apply_xform2d(texcoord, warp_map_xform);
    float4 warp_map_sample = tex2D(warp_map, warp_map_texcoord);
    
    float warp_x = warp_map_sample.x * warp_amount_x;
    float warp_y = warp_map_sample.y * warp_amount_y;
    
    float2 new_texcoord = float2(warp_x, warp_y) + texcoord;
    
    return new_texcoord;
}

float2 calc_warp_parallax_simple(float2 texcoord, float3 camera_dir, float3 tangent, float3 binormal, float3 normal)
{
    // TODO
    return texcoord;
}

#ifndef calc_warp
#define calc_warp calc_warp_none
#endif

#endif