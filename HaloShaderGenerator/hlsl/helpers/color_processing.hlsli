#ifndef _COLOR_PROCESSING_HLSLI
#define _COLOR_PROCESSING_HLSLI

#include "../registers/shader.hlsli"

#define DEBUG_TINT_FACTOR 4.59479

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
	return color <= 0.00313080009 ? 12.9200001 * color : 1.055 * exp(log(color) / 2.4) - 0.055;
}

float3 srgb_to_rgb(float3 color)
{
	return color <= 0.04045 ? 0.07739938 * color : exp(log((color + 0.055) / 1.055) * 2.4);
}

float3 expose_color(float3 input)
{
	return max(input.rgb * g_exposure.x, float3(0.0, 0.0, 0.0));
}

float4 export_high_frequency(float4 input)
{
	return float4(input.rgb / g_exposure.y, input.a * g_exposure.z);
}

float4 export_low_frequency(float4 input)
{
	return float4(input.rgb, input.a * g_exposure.w);
}

#endif