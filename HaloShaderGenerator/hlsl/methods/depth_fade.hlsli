#ifndef _DEPTH_FADE_HLSLI
#define _DEPTH_FADE_HLSLI

#include "..\registers\global_parameters.hlsli"

uniform float depth_fade_range;

void depth_fade_on(inout float alpha, float2 vPos, float depth)
{
    float2 pos = 0.5f + vPos.xy;
    float2 inv_texture_size = float2(rcp(texture_size.x), rcp(texture_size.y));
    float2 depth_texcoord = pos * inv_texture_size;
    
    float4 depth_sample = tex2D(depth_buffer, depth_texcoord);
    
    float depth_fade = depth_sample.x - depth;
    depth_fade = saturate(depth_fade * rcp(depth_fade_range.x));
    
    alpha *= depth_fade;
}

#endif