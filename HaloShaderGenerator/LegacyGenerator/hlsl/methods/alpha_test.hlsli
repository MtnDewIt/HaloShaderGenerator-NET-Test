#ifndef _ALPHA_TEST_HLSLI
#define _ALPHA_TEST_HLSLI

#include "../helpers/math.hlsli"
#include "../helpers/types.hlsli"
#include "../helpers/definition_helper.hlsli"
#include "../helpers/apply_hlsl_fixes.hlsli"

uniform sampler alpha_test_map;
uniform xform2d alpha_test_map_xform;

float calc_alpha_test_none_ps(float2 texcoord, float albedo_alpha)
{
	return 1.0;
}

float calc_alpha_test_simple_ps(float2 texcoord, float albedo_alpha)
{
	float2 alpha_test_map_texcoord = apply_xform2d(texcoord, alpha_test_map_xform);
	float4 alpha_test_map_sample = tex2D(alpha_test_map, alpha_test_map_texcoord);
#if shadertype != k_shadertype_foliage || APPLY_HLSL_FIXES == 1
	clip(alpha_test_map_sample.a - 0.5);
#endif
	return alpha_test_map_sample.a;
}

// RMCS

uniform sampler multiply_map;
uniform xform2d multiply_map_xform;

float calc_alpha_test_multiply_map_ps(float2 texcoord, float albedo_alpha)
{
    float alpha_test = tex2D(alpha_test_map, apply_xform2d(texcoord, alpha_test_map_xform)).a;
    alpha_test *= tex2D(multiply_map, apply_xform2d(texcoord, multiply_map_xform)).a;
    clip(alpha_test - 0.5);
    return alpha_test;
}

float calc_alpha_test_from_albedo_alpha_ps(float2 texcoord, float albedo_alpha)
{
    clip(albedo_alpha - 0.5);
    return albedo_alpha;
}

#ifndef calc_alpha_test_ps
#define calc_alpha_test_ps calc_alpha_test_none_ps
#endif

#endif
