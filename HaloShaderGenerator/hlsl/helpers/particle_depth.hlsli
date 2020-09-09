#ifndef _PARTICLE_DEPTH_HLSLI
#define _PARTICLE_DEPTH_HLSLI

#include "..\registers\global_parameters.hlsli"
#include "apply_hlsl_fixes.hlsli"

float4 sample_depth_buffer(float2 vPos)
{
    float2 depth_texcoord = (0.5f + vPos.xy) / texture_size.xy;
    return tex2D(depth_buffer, depth_texcoord);
}

float4 sample_depth_buffer_distortion(float2 vPos)
{
    float mul_val = 1.0f;
    if (!APPLY_HLSL_FIXES)
        mul_val = 2.0f;
    
    float2 depth_texcoord = (0.5f + vPos.xy * mul_val) / texture_size.xy;
    float4 depth_sample = tex2D(depth_buffer, depth_texcoord);
    
    return depth_sample;
    //return depth_sample * depth_constants.y + depth_constants.x;
}

#endif