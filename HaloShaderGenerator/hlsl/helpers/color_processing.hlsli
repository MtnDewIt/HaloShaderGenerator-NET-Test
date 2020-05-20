#ifndef _COLOR_PROCESSING_HLSLI
#define _COLOR_PROCESSING_HLSLI

#include "../registers/shader.hlsli"
#include "input_output.hlsli"

#define DEBUG_TINT_FACTOR 4.59479

float3 apply_debug_tint(float3 color)
{
	float debug_tint_factor = DEBUG_TINT_FACTOR;
	float3 positive_color = color * debug_tint_factor;
	float3 negative_tinted_color = debug_tint.rgb - color * debug_tint_factor;
	return positive_color + debug_tint.a * negative_tinted_color;
}

float3 rgb_to_srgb(float3 color)
{
	return color <= 0.00313080009 ? 12.9200001 * color : 1.055 * exp(log(color) / 2.4) - 0.055;
}

float3 srgb_to_rgb(float3 color)
{
	float3 color1 = exp(log((color + 0.055) / 1.055) * 2.4);
	return color <= 0.04045 ? 0.07739938 * color : color1;
}

float3 expose_color(float3 input)
{
	if (color_export_multiply_alpha)
		return input; // should compile as max but compiles as cmp for some reason, equivalent result
	else
		return input * g_exposure.x;
}

float4 export_high_frequency(float4 input)
{
	return float4(input.rgb / g_exposure.y, input.a * g_exposure.z);
}

float4 export_low_frequency(float4 input)
{
	return float4(input.rgb, input.a * g_exposure.w);
}

float luminance(float3 color)
{
	return color.r * 0.21265601 + color.g * 0.71515799 + color.b * 0.07218560;
}

PS_OUTPUT_DEFAULT export_color(float4 color)
{
	color.rgb = max(color.rgb, 0);
	PS_OUTPUT_DEFAULT output;
	[flatten]
	if (color_export_multiply_alpha)
	{
		output.low_frequency = color * g_exposure.w;
		output.high_frequency = color * g_exposure.z;
	}
	else
	{
		output.low_frequency = export_low_frequency(color);
		output.high_frequency = export_high_frequency(color);
	}
	output.unknown = 0;
	return output;
}

#endif