#ifndef _COLOR_PROCESSING_HLSLI
#define _COLOR_PROCESSING_HLSLI

#include "../registers/shader.hlsli"
/*
* RGB to sRGB, rename later
*/
float3 bungie_color_processing(float3 color)
{
    float4 r0 = float4(color, 0);
    float4 r1 = float4(0, 0, 0, 0);
    float4 r2 = float4(0, 0, 0, 0);

    r1.xyz = color.xyz * 4.59478998;
    r1.w = 4.59478998;
    r0.xyz = r0.xyz * -r1.xyz + debug_tint.xyz;
    r0.xyz = debug_tint.www * r0.xyz + r1.xyz;
    //r0 is the linear RGB color
    // the following 3 lines compute r0 ^ (5/12)
    r1.xyz = log(r0.xyz);
    r1.xyz = r1.xyz * (5.0 / 12.0); // 5/12
    r2.xyz = exp(r1.xyz);
    //  compute the 2 possible cases for sRGB values, 1.055 * r0^(5/12) - 0.055 or 12.92 * r0
    r1.xyz = r2.xyz * 1.055 - 0.055;
    r2.xyz = r0.xyz;
    r0.xyz = r0.xyz * 12.92;
    return r2.xyz <= 0.00313 ? r0.xyz : r1.xyz
    // this was the original code, not sure if it's intended to be ugly like that but that's not standard linear to sRGB conversion
    //r2.xyz = (-r0.xyz) * 12.92; 
    //r0.xyz = r0.xyz * 12.92;
    //return r2.xyz >= 0 ? r0.xyz : r1.xyz;
}

float3 expose_color(float3 input)
{
    return max(input * g_exposure.x, float3(0.0, 0.0, 0.0));
}

float4 export_high_frequency(float4 input)
{
    float alpha = input.w;
    float3 color = input.xyz;

    return float4(color / g_exposure.y, alpha * g_exposure.z);
}

float4 export_low_frequency(float4 input)
{
    float alpha = input.w;
    float3 color = input.xyz;

    return float4(color, alpha * g_exposure.w);
}

#endif