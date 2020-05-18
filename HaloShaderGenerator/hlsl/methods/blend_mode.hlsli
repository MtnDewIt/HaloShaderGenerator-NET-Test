#ifndef _BLEND_MODE_HLSLI
#define _BLEND_MODE_HLSLI

#include "../helpers/math.hlsli"

float4 blend_type_opaque(float4 input, float fade)
{
    return input;
}

float4 blend_type_additive(float4 input, float fade)
{
	return input;
}

float4 blend_type_multiply(float4 input, float fade)
{
	float3 color = (input.rgb - 1.0) * fade + 1.0;
	return float4(color, input.a);
}

float4 blend_type_alpha_blend(float4 input, float fade)
{
	float alpha = input.a * fade;
	return float4(input.rgb, alpha);
}

float4 blend_type_double_multiply(float4 input, float fade)
{
	float alpha = input.a * fade;
	float3 color = (input.rgb - 0.5) * fade + 0.5;
	return float4(color, alpha);
}

float4 blend_type_pre_multiplied_alpha(float4 input, float fade)
{
	float alpha = input.a * fade;
	return float4(input.rgb * alpha, alpha);
}


#ifndef blend_type
#define blend_type blend_type_opaque
#endif

#endif
