#ifndef _BLACK_POINT_HLSLI
#define _BLACK_POINT_HLSLI

#include "../helpers/types.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/color_processing.hlsli"
#include "../helpers/definition_helper.hlsli"

void black_point_on(inout float alpha, float black_point)
{
    float r0_x = 1 / (-black_point + 0.5f * (1 - -black_point));
    float r0_y = alpha - black_point;
    r0_x = saturate(r0_y * r0_x);
    r0_y = 1.0f + black_point;
    float r0_z = r0_y * 0.5f;
    r0_y = saturate(-(r0_y * 0.5f) + alpha);
    alpha = r0_z * r0_x + r0_y;
}

#endif