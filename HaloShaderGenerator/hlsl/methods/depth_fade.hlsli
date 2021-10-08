#ifndef _DEPTH_FADE_HLSLI
#define _DEPTH_FADE_HLSLI

#include "..\helpers\apply_hlsl_fixes.hlsli"

#ifndef depth_fade_arg
#define depth_fade_arg 0
#endif
#ifndef k_depth_fade_off
#define k_depth_fade_off 0
#endif

#ifndef DEPTH_BUFFER_DEFINED
uniform sampler2D depth_buffer;
#define DEPTH_BUFFER_DEFINED 1
#endif

uniform float depth_fade_range;

#include "..\registers\global_parameters.hlsli"

float apply_z_fixup(in float z)
{
#if APPLY_HLSL_FIXES == 1
        return 1.0f / ((depth_constants.x + depth_constants.y) - depth_constants.y * z.x);
#else
        return z;
#endif
}

float calc_depth_fade(float2 texcoord, float depth, bool fade_range)
{
    if (depth_fade_arg == k_depth_fade_off)
        return 1.0f;
    
    float zbuf_depth = apply_z_fixup(tex2D(depth_buffer, texcoord).r);
    
#if APPLY_HLSL_FIXES == 1
    return saturate((depth - zbuf_depth) / (fade_range ? depth_fade_range : 1.0f));
#else
    return saturate((zbuf_depth - depth) / (fade_range ? depth_fade_range : 1.0f));
#endif
}

#endif