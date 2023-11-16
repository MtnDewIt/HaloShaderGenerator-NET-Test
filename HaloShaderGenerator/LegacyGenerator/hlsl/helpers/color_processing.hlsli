#ifndef _COLOR_PROCESSING_HLSLI
#define _COLOR_PROCESSING_HLSLI

#include "../registers/global_parameters.hlsli"
#include "input_output.hlsli"
#include "../helpers/definition_helper.hlsli"

float3 rgb_to_srgb(float3 color)
{
	return color <= 0.00313080009 ? 12.9200001 * color : 1.055 * exp(log(color) / 2.4) - 0.055;
}

float3 srgb_to_rgb(float3 color)
{
	float3 color1 = exp(log((color + 0.055) / 1.055) * 2.4);
	return color <= 0.04045 ? 0.07739938 * color : color1;
}

float3 rec709_to_rgb(float3 color)
{
    return color < 0.081f ? color / 4.5f : pow((color + 0.099f) / 1.099f, 2.2f);
}

float3 rgb_to_rec709(float3 color)
{
    return color < 0.018f ? color * 4.5f : 1.099f * pow(color, 1.0f / 2.2f) - 0.099f;
} 

// Compiler doesn't like this but yeh
// Not sure if this conversion is 100% correct
//float3 rgb_to_xenon_hdtv(float3 color)
//{
//    if (color < 0.0625f)
//        return color * 4.0f;
//    if (color < 0.125f)
//        return (color - 0.0625f) * 2.0f + 0.25f;
//    if (color < 0.5f)
//        return color - 0.125f + 0.375f;
//    return (color - 0.5f) * 0.5f + 0.75f;
//}
//float3 xenon_hdtv_to_rgb(float3 color)
//{
//    if (color < 0.25f)
//        return color / 4.0f;
//    if (color < 0.375f)
//        return (color - 0.25f) / 2.0f + 0.0625f;
//    if (color < 0.75f)
//        return (color - 0.375f) + 0.125f;
//    return (color - 0.75f) * 2.0f + 0.5f;
//}

float3 expose_color(float3 input)
{
	if (color_export_multiply_alpha && !is_dynamic_light)
		return input;
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
	
	if (color_export_multiply_alpha && !is_dynamic_light)
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