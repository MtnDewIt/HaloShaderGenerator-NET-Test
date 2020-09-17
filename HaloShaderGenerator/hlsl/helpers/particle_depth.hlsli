#ifndef _PARTICLE_DEPTH_HLSLI
#define _PARTICLE_DEPTH_HLSLI

#include "..\registers\global_parameters.hlsli"
#include "apply_hlsl_fixes.hlsli"

float4 sample_depth_buffer(float2 position)
{
    float2 depth_texcoord = (0.5f + position.xy) / texture_size.xy;
    float4 depth_sample = tex2D(depth_buffer, depth_texcoord);
    
    //if (APPLY_HLSL_FIXES)
    //    return abs(-depth_sample * depth_constants.y + depth_constants.x);
    
    return depth_sample;
}

float4 sample_depth_buffer_distortion(float2 position)
{
    float position_modifier = 2.0f; // *why*
    if (APPLY_HLSL_FIXES)
        position_modifier = 1.0f;
    
    float2 depth_texcoord = (0.5f + position.xy * position_modifier) / texture_size.xy;
    float4 depth_sample = tex2D(depth_buffer, depth_texcoord);
    
    //if (APPLY_HLSL_FIXES)
    //    return abs(-depth_sample * depth_constants.y + depth_constants.x);
    
    return depth_sample;
}

#endif