#ifndef _ALPHA_TEST_HLSLI
#define _ALPHA_TEST_HLSLI

#include "../helpers/math.hlsli"
#include "../helpers/types.hlsli"

uniform sampler alpha_test_map;
uniform xform2d alpha_test_map_xform;

void calc_alpha_test_off_ps(float2 texcoord)
{
	return;
}

void calc_alpha_test_on_ps(float2 texcoord)
{
	float2 alpha_test_map_texcoord = apply_xform2d(texcoord, alpha_test_map_xform);
	float4 alpha_test_map_sample = tex2D(alpha_test_map, alpha_test_map_texcoord);
	clip(alpha_test_map_sample.a - 0.5);

}

#ifndef calc_alpha_test_ps
#define calc_alpha_test_ps calc_alpha_test_off_ps
#endif

#endif
