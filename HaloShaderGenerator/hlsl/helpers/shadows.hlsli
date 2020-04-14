#ifndef _SHADOWS_HLSLI
#define _SHADOWS_HLSLI

#include "../helpers/math.hlsli"
#include "../registers/shader.hlsli"

//
// TODO: There might be a way to optimize this and remove a few instructions (around 10 for 3x3 and 6 for 4x4) to make it faster
//

float sample_shadowmap(float2 texcoord, float2 offset, float depth_offset, float scale)
{
	float map_sample = tex2D(shadow_depth_map_1, texcoord + offset).x;
	float depth = map_sample - depth_offset * scale;
	return depth < 1.0 ? 1.0 : 0.0;
}

float shadows_percentage_closer_filtering_3x3(float2 shadowmap_texcoord, float shadowmap_size, float depth_scale, float depth_offset, float3 diffuse)
{
	float shadow_coefficient = 0;
	float scale = 1.0 / depth_scale;

	[unroll(3)]
	for (int x = -1; x < 2; ++x)
	{
		[unroll(3)]
		for (int y = -1; y < 2; ++y)
		{
			shadow_coefficient += sample_shadowmap(shadowmap_texcoord, float2(x / shadowmap_size, y / shadowmap_size), depth_offset, scale);
		}
	}

	shadow_coefficient /= 9.0;
	shadow_coefficient = -dot(diffuse.rgb, diffuse.rgb) >= 0 ? 1.0 : shadow_coefficient;
	return shadow_coefficient;
}

float shadows_percentage_closer_filtering_4x4(float2 shadowmap_texcoord, float shadowmap_size, float depth_scale, float depth_offset, float3 diffuse)
{
	float shadow_coefficient = 0;
	float scale = 1.0 / depth_scale;

	[unroll(4)]
	for (int x = 0; x < 4; ++x)
	{
		[unroll(4)]
		for (int y = 0; y < 4; ++y)
		{
			shadow_coefficient += sample_shadowmap(shadowmap_texcoord, float2((-1.5 +  x) / shadowmap_size, (-1.5 + y) / shadowmap_size), depth_offset, scale);
		}
	}

	shadow_coefficient /= 16.0;
	shadow_coefficient = -dot(diffuse.rgb, diffuse.rgb) >= 0 ? 1.0 : shadow_coefficient;
	return shadow_coefficient;
}

#endif
