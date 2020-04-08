#ifndef _COLOR_PROCESSING_HLSLI
#define _COLOR_PROCESSING_HLSLI

#include "../registers/shader.hlsli"

#define DEBUG_TINT_FACTOR 4.595

float3 apply_debug_tint(float3 color)
{
	float debug_tint_factor = DEBUG_TINT_FACTOR;
	float3 positive_color = color * debug_tint_factor;
	float3 negative_tinted_color = debug_tint.rgb - color * debug_tint_factor;
	return positive_color + debug_tint.a * negative_tinted_color;
}

/*
* Convert RGB color to sRGB
*/
float3 rgb_to_srgb(float3 color)
{
	float3 color1 = 1.05499995 * exp(log(color) * 0.416666657) - 0.0549999997;
	return color <= 0.00313080009 ? 12.9200001 * color : color1;
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