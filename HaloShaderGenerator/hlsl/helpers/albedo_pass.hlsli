#ifndef _ALBEDO_PASS_HLSLI
#define _ALBEDO_PASS_HLSLI

#include "color_processing.hlsli"
#include "../methods/albedo.hlsli"
#include "../methods/bump_mapping.hlsli"


struct ALBEDO_PASS_RESULT
{
	float3 albedo;
	float alpha;
	float3 normal;
};

ALBEDO_PASS_RESULT get_albedo_and_normal(float2 fragcoord, float2 texcoord, float3 tangent, float3 binormal, float3 normal)
{
	ALBEDO_PASS_RESULT result = (ALBEDO_PASS_RESULT) 0;

	// this is set by a b12 if misc is sometimes or always, otherwise it samples from the frame buffer
	if (actually_calc_albedo)
	{
		float4 diffuse_and_alpha = calc_albedo_ps(texcoord, fragcoord);
		result.albedo = apply_debug_tint(diffuse_and_alpha.xyz);
		result.alpha = diffuse_and_alpha.w;
		result.normal = calc_bumpmap_ps(tangent, binormal, normal, texcoord);
		return result;
	}
	else
	{
        // sample from framebuffer
		float4 albedo_texture_sample = tex2D(albedo_texture, fragcoord);
		result.albedo = albedo_texture_sample.xyz;
		result.alpha = albedo_texture_sample.w;

		float4 normal_texture_sample = tex2D(normal_texture, fragcoord);
		result.normal = normalize(normal_texture_sample.xyz * 2.0 - 1.0);
	}

	return result;
}

#endif
