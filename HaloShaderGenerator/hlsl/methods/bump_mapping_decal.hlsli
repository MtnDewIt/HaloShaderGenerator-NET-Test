#ifndef _BUMP_MAPPING_DECAL_HLSLI
#define _BUMP_MAPPING_DECAL_HLSLI

#include "../helpers/types.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/bumpmap_math.hlsli"

uniform sampler bump_map;
uniform xform2d bump_map_xform;

float3 bump_mapping_leave(
	float3 tangent,
	float3 binormal,
	float3 normal,
	float2 texcoord)
{
    return float3(0, 0, 0);
}

float3 bump_mapping_standard(
	float3 tangent,
	float3 binormal,
	float3 normal,
	float2 texcoord)
{
    float2 bump_map_texcoord = apply_xform2d(texcoord, bump_map_xform);
    float3 bump_map_sample = sample_normal_2d(bump_map, bump_map_texcoord);
	
    float3 result = normal_transform(tangent, binormal, normal, bump_map_sample);
    result = result * 0.5f + 0.5f;
    return result;
}

float3 bump_mapping_standard_mask(
	float3 tangent,
	float3 binormal,
	float3 normal,
	float2 texcoord)
{
    return float3(0, 0, 0);
}

#ifndef decal_bump_mapping
#define decal_bump_mapping bump_mapping_leave
#endif

#endif