#ifndef _DEPTH_FADE_HLSLI
#define _DEPTH_FADE_HLSLI

uniform float depth_fade_range;

float depth_fade_on(float depth_x, float depth)
{
    return saturate((depth_x - depth) / depth_fade_range.x);
}
float calc_depth_fade(float depth_x, float depth)
{
    return saturate(depth_x - depth);
}

#endif