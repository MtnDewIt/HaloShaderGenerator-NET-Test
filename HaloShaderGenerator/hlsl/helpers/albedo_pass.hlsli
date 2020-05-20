#ifndef _ALBEDO_PASS_HLSLI
#define _ALBEDO_PASS_HLSLI

#include "color_processing.hlsli"
#include "../methods/albedo.hlsli"
#include "../methods/bump_mapping.hlsli"
#include "../methods/alpha_test.hlsli"
#include "../methods/parallax.hlsli"

struct ALBEDO_PASS_RESULT
{
	float4 albedo;
	float3 normal;
};

ALBEDO_PASS_RESULT get_albedo_and_normal(float2 fragcoord, float2 texcoord, float3 camera_dir, float3 tangent, float3 binormal, float3 normal)
{
	ALBEDO_PASS_RESULT result;

	// this is set by a b12 if misc is sometimes or always, otherwise it samples from the frame buffer
	if (actually_calc_albedo)
	{
		float2 new_texcoord = calc_parallax_ps(texcoord, camera_dir, tangent, binormal, normal);
		calc_alpha_test_ps(new_texcoord);
		result.normal = calc_bumpmap_ps(tangent, binormal, normal.xyz, new_texcoord);
		result.albedo = calc_albedo_ps(new_texcoord, fragcoord);
	}
	else
	{
		float2 inv_texture_size = (1.0 / texture_size);
		float2 texcoord = (fragcoord + 0.5) * inv_texture_size;
		float4 normal_texture_sample = tex2D(normal_texture, texcoord);
		float4 albedo_texture_sample = tex2D(albedo_texture, texcoord);
		result.albedo = albedo_texture_sample.xyzw;
		result.normal = normal_texture_sample.xyz * 2.0 - 1.0;
	}
	return result;
}

float3 get_normal(float2 fragcoord, float2 texcoord, float3 tangent, float3 binormal, float3 normal)
{
	if (actually_calc_albedo)
	{
		return calc_bumpmap_ps(tangent, binormal, normal, texcoord).xyz;
	}
	else
	{
		float4 normal_texture_sample = tex2D(normal_texture, fragcoord);
		return normalize(normal_texture_sample.xyz * 2.0 - 1.0);
	}
}

float4 get_albedo(float2 fragcoord, float2 texcoord)
{
	if (actually_calc_albedo)
	{
		// check for parallax, alpha test and so on
		return calc_albedo_ps(texcoord, fragcoord);
	}
	else
	{
		return tex2D(albedo_texture, fragcoord);
	}
}

#endif
