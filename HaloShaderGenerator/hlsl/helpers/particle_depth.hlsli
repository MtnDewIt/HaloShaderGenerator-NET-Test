#ifndef _PARTICLE_DEPTH_HLSLI
#define _PARTICLE_DEPTH_HLSLI

#include "..\registers\global_parameters.hlsli"

float4 sample_depth_buffer(float2 vPos)
{
    float2 depth_texcoord = (0.5f + vPos.xy) / texture_size.xy;
    return tex2D(depth_buffer, depth_texcoord);
}

float4 sample_depth_buffer_distortion(float2 vPos)
{
    float2 depth_texcoord = (0.5f + vPos.xy * 2) / texture_size.xy; // TODO: fix
    return tex2D(depth_buffer, depth_texcoord);
}

#endif