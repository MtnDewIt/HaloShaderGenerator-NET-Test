#ifndef _SHADOWS_HLSLI
#define _SHADOWS_HLSLI

#include "../helpers/math.hlsli"

uniform sampler2D shadow_depth_map_1;


float sample_shadowmap(float2 texcoord, float2 offset, float depth_offset, float scale)
{
	float map_sample = tex2D(shadow_depth_map_1, texcoord + offset).x;
	float depth = map_sample - depth_offset * scale;
	return depth < 0.0 ? 0.0 : 1.0;
}

float shadows_percentage_closer_filtering_3x3(float2 shadowmap_texcoord, float shadowmap_size, float depth_scale, float depth_offset)
{
	float shadow_coefficient = 0;
	float scale = 1.0 / depth_scale;
	float shadowmap_size_inv = 1.0 / shadowmap_size;
	
	[unroll(3)]
	for (int y = -1; y < 2; y++)
	{
		[unroll(3)]
		for (int x = -1; x < 2; x++)
		{
			float2 texcoord_offset = shadowmap_size_inv * float2(x, y);
			shadow_coefficient += sample_shadowmap(shadowmap_texcoord, texcoord_offset, depth_offset, scale);
		}
	}

	shadow_coefficient /= 9.0;
	
	return shadow_coefficient;
}

float shadows_percentage_closer_filtering_custom_4x4(float2 shadowmap_texcoord, float shadowmap_size, float depth_scale, float depth_offset)
{
	float shadow_coefficient = 0;
	float scale = 1.0 / depth_scale;
	float shadowmap_size_inv_1 = 1.0 / (2.0 * shadowmap_size);
	// this part took way too long to reverse
	float2 texcoord_pix = shadowmap_texcoord * shadowmap_size + 0.5;
	float2 right_bound = frac(texcoord_pix);
	float2 left_bound = 1.0 - right_bound;
	
	shadow_coefficient += left_bound.x * left_bound.y * sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(-3, -3), depth_offset, scale);
	shadow_coefficient += left_bound.y * sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(-1, -3), depth_offset, scale);
	shadow_coefficient += left_bound.y * sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(1, -3), depth_offset, scale);
	shadow_coefficient += right_bound.x * left_bound.y * sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(3, -3), depth_offset, scale);
	
	shadow_coefficient += left_bound.x * sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(-3, -1), depth_offset, scale);
	shadow_coefficient += sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(-1, -1), depth_offset, scale);
	shadow_coefficient += sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(1, -1), depth_offset, scale);
	shadow_coefficient += right_bound.x * sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(3, -1), depth_offset, scale);
	
	shadow_coefficient += left_bound.x * sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(-3, 1), depth_offset, scale);
	shadow_coefficient += sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(-1, 1), depth_offset, scale);
	shadow_coefficient += sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(1, 1), depth_offset, scale);
	shadow_coefficient += right_bound.x * sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(3, 1), depth_offset, scale);
	
	shadow_coefficient += left_bound.x * right_bound.y * sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(-3, 3), depth_offset, scale);
	shadow_coefficient += right_bound.y * sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(-1, 3), depth_offset, scale);
	shadow_coefficient += right_bound.y * sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(1, 3), depth_offset, scale);
	shadow_coefficient += right_bound.x * right_bound.y * sample_shadowmap(shadowmap_texcoord, shadowmap_size_inv_1 * float2(3, 3), depth_offset, scale);

	shadow_coefficient /= 9.0;
	
	return shadow_coefficient;
}

#endif
