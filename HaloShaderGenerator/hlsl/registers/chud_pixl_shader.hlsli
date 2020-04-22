#ifndef _CHUD_PIXL_SHADER_HLSLI
#define _CHUD_PIXL_SHADER_HLSLI

#include "../helpers/types.hlsli"
#include "../helpers/definition_helper.hlsli"

uniform bool chud_comp_colorize_enabled : register(b8);

uniform float4 cortana_back_colormix_result : register(c57);

uniform float4 cortana_comp_solarize_inmix : register(c60);
uniform float4 cortana_comp_solarize_outmix : register(c61);
uniform float4 cortana_comp_solarize_result : register(c62);

uniform float4 cortana_comp_doubling_inmix : register(c63);
uniform float4 cortana_comp_doubling_outmix : register(c64);
uniform float4 cortana_comp_doubling_result : register(c65);

uniform float4 cortana_comp_colorize_inmix : register(c66);
uniform float4 cortana_comp_colorize_outmix : register(c67);
uniform float4 cortana_comp_colorize_result : register(c68);

uniform float4 cortana_vignette_data : register(c72);

uniform float4x3 p_postprocess_hue_saturation_matrix : register(c218);

uniform sampler2D basemap_sampler : register(s0);
uniform sampler2D cortana_sampler : register(s1);
uniform sampler2D goo_sampler : register(s2);

// TODO: add remaining























#endif
