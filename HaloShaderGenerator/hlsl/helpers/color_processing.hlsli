#ifndef _COLOR_PROCESSING_HLSLI
#define _COLOR_PROCESSING_HLSLI

#include "../registers/shader.hlsli"

/*
* Not sure what this do yet,
*/
float3 apply_debug_tint(float3 color)
{
	float debug_tint_factor = 4.595;
	float3 negative_tinted_color = color * (-debug_tint_factor) + debug_tint.rgb;
	float3 positive_color = color * debug_tint_factor;
	return positive_color + negative_tinted_color * debug_tint.a;
}

/*
* Convert RGB color to sRGB
*/
float3 rgb_to_srgb(float3 color)
{
	float3 color1 = 1.05499995 * exp(log(color) * 0.416666657) - 0.0549999997;
	float3 color2 = 12.9200001 * color;
	return color <= 0.00313080009 ? color2 : color1;
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