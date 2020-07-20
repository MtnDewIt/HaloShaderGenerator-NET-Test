#ifndef _DEPTH_FADE_HLSLI
#define _DEPTH_FADE_HLSLI

#include "..\registers\global_parameters.hlsli"

uniform float depth_fade_range;

void depth_fade_on(inout float alpha, float depth_x, float depth)
{
    float depth_fade = saturate((depth_x - depth) / depth_fade_range.x);
    alpha *= depth_fade;
}

#endif