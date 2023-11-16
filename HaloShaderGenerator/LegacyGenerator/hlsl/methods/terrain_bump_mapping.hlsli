#ifndef _TERRAIN_BUMP_MAPPING_HLSLI
#define _TERRAIN_BUMP_MAPPING_HLSLI

#include "../helpers/types.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/bumpmap_math.hlsli"
#include "..\helpers\terrain_helper.hlsli"

float reconstruct_normal_test(float2 normal)
{
	float z_squared = 1.0 - saturate(dot(normal, normal));
	float normal_z = sqrt(z_squared);
	return normal_z;
}

float3 calc_bumpmap(
	float3 tangent,
	float3 binormal,
	float3 normal,
	float2 texcoord,
	sampler bump_map,
	xform2d bump_map_xform
) 
{
    float2 bump_map_texcoord = apply_xform2d(texcoord, bump_map_xform);
	float2 bump_map_sample = sample_bump_map_2d(bump_map, bump_map_texcoord);
	float3 bump = float3(bump_map_sample, reconstruct_normal_test(bump_map_sample));
	return bump;
}

float3 calc_bumpmap_detail(
	float3 tangent,
	float3 binormal,
	float3 normal,
	float2 texcoord,
	sampler bump_map,
	xform2d bump_map_xform,
	sampler bump_detail_map,
	xform2d bump_detail_map_xform
)
{
	float2 bump_map_sample = sample_bump_map_2d(bump_map, apply_xform2d(texcoord, bump_map_xform));
	float2 bump_detail_map_sample = sample_bump_map_2d(bump_detail_map, apply_xform2d(texcoord, bump_detail_map_xform));
	bump_map_sample += bump_detail_map_sample;
	float3 bump = float3(bump_map_sample, reconstruct_normal_test(bump_map_sample));
	return bump;
}

float3 calc_terrain_bumpmap(
	float3 tangent,
	float3 binormal,
	float3 normal,
	float2 texcoord,
	sampler bump_map,
	xform2d bump_map_xform,
	sampler bump_detail_map,
	xform2d bump_detail_map_xform,
	bool has_illum
)
{
    if (!four_materials_active && !has_illum)
		return calc_bumpmap_detail(tangent, binormal, normal, texcoord, bump_map, bump_map_xform, bump_detail_map, bump_detail_map_xform);
	else
		return calc_bumpmap(tangent, binormal, normal, texcoord, bump_map, bump_map_xform);
}



#endif