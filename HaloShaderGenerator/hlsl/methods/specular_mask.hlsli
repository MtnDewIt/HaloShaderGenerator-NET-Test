#ifndef _SPECULAR_MASK_HLSLI
#define _SPECULAR_MASK_HLSLI

#include "../helpers/math.hlsli"
#include "../helpers/types.hlsli"

uniform sampler2D specular_mask_texture;
xform2d specular_mask_texture_xform;

void calc_no_specular_mask_ps(
in float4 albedo, 
in float2 texcoord,
inout float specular_coefficient)
{
}

void calc_specular_mask_from_diffuse_ps(
in float4 albedo,
in float2 texcoord,
inout float specular_coefficient)
{
	specular_coefficient *= albedo.a;
}

void calc_specular_mask_from_texture_ps(
in float4 albedo,
in float2 texcoord,
inout float specular_coefficient)
{
	float2 specular_map_texcoord = apply_xform2d(texcoord, specular_mask_texture_xform);
	float4 specular_map_sample = tex2D(specular_mask_texture, specular_map_texcoord);
	specular_coefficient *= specular_map_sample.a;
}

void calc_specular_mask_from_color_texture_ps(
in float4 albedo,
in float2 texcoord,
inout float specular_coefficient)
{
	float2 specular_map_texcoord = apply_xform2d(texcoord, specular_mask_texture_xform);
	float4 specular_map_sample = tex2D(specular_mask_texture, specular_map_texcoord);
	specular_coefficient *= specular_map_sample.a;
}

#ifndef calc_specular_mask_ps
#define calc_specular_mask_ps calc_no_specular_mask_ps
#endif

#endif
