#ifndef _ALPHA_TEST_HLSLI
#define _ALPHA_TEST_HLSLI

#include "../helpers/math.hlsli"
#include "../helpers/types.hlsli"

uniform sampler alpha_test_map;
uniform xform2d alpha_test_map_xform;

float calc_alpha_test_none_ps(float2 texcoord)
{
	return 1.0;
}

float calc_alpha_test_simple_ps(float2 texcoord)
{
	float2 alpha_test_map_texcoord = apply_xform2d(texcoord, alpha_test_map_xform);
	float4 alpha_test_map_sample = tex2D(alpha_test_map, alpha_test_map_texcoord);
	clip(alpha_test_map_sample.a - 0.5);
	return alpha_test_map_sample.a;
}

// RMCS

uniform sampler multiply_map;
uniform xform2d multiply_map_xform;

float calc_alpha_test_multiply_map_ps(float2 texcoord)
{
    float alpha_test = tex2D(alpha_test_map, apply_xform2d(texcoord, alpha_test_map_xform)).a;
    alpha_test *= tex2D(multiply_map, apply_xform2d(texcoord, multiply_map_xform)).a;
    clip(alpha_test - 0.5);
    return alpha_test;
}

#ifndef calc_alpha_test_ps
#define calc_alpha_test_ps calc_alpha_test_off_ps
#endif

#endif
