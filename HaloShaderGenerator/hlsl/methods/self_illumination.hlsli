#ifndef _SELF_ILLUMINATION_HLSLI
#define _SELF_ILLUMINATION_HLSLI

#include "../helpers/math.hlsli"
#include "../helpers/types.hlsli"
#include "../helpers/color_processing.hlsli"
#include "albedo.hlsli"

uniform float self_illum_intensity;
uniform xform2d self_illum_map_xform;
uniform sampler2D self_illum_map;
uniform float4 self_illum_color;
uniform xform2d self_illum_detail_map_xform;
uniform sampler self_illum_detail_map;
uniform xform2d alpha_mask_map_xform;
uniform sampler alpha_mask_map;
uniform xform2d noise_map_a_xform;
uniform sampler noise_map_a;
uniform xform2d noise_map_b_xform;
uniform sampler noise_map_b;
uniform float4 color_medium;
uniform float4 color_sharp;
uniform float4 color_wide;
uniform float thinness_medium;
uniform float thinness_sharp;
uniform float thinness_wide;
uniform float4 channel_a;
uniform float4 channel_b;
uniform float4 channel_c;
uniform sampler2D meter_map;
uniform float4 meter_map_xform;
uniform float4 meter_color_off;
uniform float4 meter_color_on;
uniform float meter_value;

uniform float primary_change_color_blend;

void calc_self_illumination_none_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{
}

void calc_self_illumination_simple_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{
	float2 self_illum_map_texcoord = apply_xform2d(texcoord, self_illum_map_xform);
    float3 self_illum_map_sample = tex2D(self_illum_map, self_illum_map_texcoord).rgb;
	self_illum_map_sample *= self_illum_color.rgb;
	self_illum_map_sample *= self_illum_intensity;
	self_illum_map_sample *= g_alt_exposure.x;

	diffuse += self_illum_map_sample;
}

void calc_self_illumination_three_channel_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{
	float2 self_illum_map_texcoord = apply_xform2d(texcoord, self_illum_map_xform);
    float3 self_illum_map_sample = tex2D(self_illum_map, self_illum_map_texcoord).rgb;
	
    self_illum_map_sample.r *= channel_a.a;
    self_illum_map_sample.g *= channel_b.a;
    self_illum_map_sample.b *= channel_c.a;
	
    float3 color = float3(0, 0, 0);
	
    color.rgb += self_illum_map_sample.r * channel_a.rgb;
    color.rgb += self_illum_map_sample.g * channel_b.rgb;
    color.rgb += self_illum_map_sample.b * channel_c.rgb;
	
    color.rgb *= self_illum_intensity;
	
    //color.rgb *= g_alt_exposure.x;
    //diffuse += color;
	
    diffuse = color * g_alt_exposure.x + diffuse;
}

void calc_self_illumination_plasma_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{
	float2 alpha_map_texcoord = apply_xform2d(texcoord, alpha_mask_map_xform);
	float2 noise_a_texcoord = apply_xform2d(texcoord, noise_map_a_xform);
	float2 noise_b_texcoord = apply_xform2d(texcoord, noise_map_b_xform);
    
	float4 alpha_mask_map_sample = tex2D(alpha_mask_map, alpha_map_texcoord);
	float4 noise_map_a_sample = tex2D(noise_map_a, noise_a_texcoord);
	float4 noise_map_b_sample = tex2D(noise_map_b, noise_b_texcoord);

    float noise = 1.0 - abs(noise_map_a_sample.x - noise_map_b_sample.x);
    float log_noise = log2(noise);
	
	float noise_medium = exp2(log_noise * thinness_medium);
	float noise_sharp = exp2(log_noise * thinness_sharp);
	float noise_wide = exp2(log_noise * thinness_wide);
	
	float noise_medium_to_sharp = noise_medium - noise_sharp;
	float noise_wide_to_medium = noise_wide - noise_medium;
	
	
    // These three noise components represent the full [0-1] range


    float3 color = float3(0, 0, 0);
	color += color_medium.rgb * color_medium.a * noise_medium_to_sharp;
	color += color_sharp.rgb * color_sharp.a * noise_sharp;
	color += color_wide.rgb * color_wide.a * noise_wide_to_medium;

    color *= alpha_mask_map_sample.a;
    color *= self_illum_intensity;
    color *= g_alt_exposure.x;

    // not 100% sure about alpha yet
	diffuse += color;
}

void calc_self_illumination_from_albedo_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{
	float3 color = albedo.rgb;
	color.rgb *= self_illum_color.rgb;
	color.rgb *= self_illum_intensity;
	color.rgb *= g_alt_exposure.x;
	diffuse = color;
}

void calc_self_illumination_detail_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{
	float2 self_illum_map_texcoord = apply_xform2d(texcoord, self_illum_map_xform);
	float2 self_illum_detail_map_texcoord = apply_xform2d(texcoord, self_illum_detail_map_xform);
	
	float4 self_illum_map_sample = tex2D(self_illum_map, self_illum_map_texcoord);
	float4 self_illum_detail_map_sample = tex2D(self_illum_detail_map, self_illum_detail_map_texcoord);
	
	self_illum_map_sample.rgb *= self_illum_detail_map_sample.rgb * DETAIL_MULTIPLIER;
	self_illum_map_sample.rgb *= self_illum_color.rgb;
	self_illum_map_sample.rgb *= self_illum_intensity;
	self_illum_map_sample.rgb *= g_alt_exposure.x;

	diffuse += self_illum_map_sample.rgb;
}

void calc_self_illumination_meter_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{
    float2 meter_map_texcoord = apply_xform2d(texcoord, meter_map_xform);
    float4 meter_map_sample = tex2D(meter_map, meter_map_texcoord);
	float3 color;
	
	if (meter_map_sample.x - 0.5 < 0)
		color = 0;
	else
	{
		if (-meter_map_sample.w + meter_value < 0)
			color = meter_color_off.rgb;
		else
			color = meter_color_on.rgb;

		color *= g_alt_exposure.x;
	}
	
	diffuse += color;
}

void calc_self_illumination_times_diffuse_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{
	float2 self_illum_map_texcoord = apply_xform2d(texcoord, self_illum_map_xform);
	float4 self_illum_map_sample = tex2D(self_illum_map, self_illum_map_texcoord);
	float a = max(0, 10 * self_illum_map_sample.y - 9);
	float3 color = primary_change_color_blend * primary_change_color + (1.0 - primary_change_color_blend) * self_illum_color.rgb;
	
	float3 interpolation = lerp(a, 1.0, albedo);

	float3 result = 0;
	result.rgb = color * interpolation;
	result.rgb *= self_illum_intensity;
	result.rgb *= self_illum_map_sample.rgb;
	result.rgb *= g_alt_exposure.x;

	diffuse += result;
}

void calc_self_illumination_simple_with_alpha_mask_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{
	float2 self_illum_map_texcoord = apply_xform2d(texcoord, self_illum_map_xform);
	float4 self_illum_map_sample = tex2D(self_illum_map, self_illum_map_texcoord);
	self_illum_map_sample *= self_illum_color;
	self_illum_map_sample.rgb *= (self_illum_intensity * self_illum_map_sample.a);
	self_illum_map_sample.rgb *= g_alt_exposure.x;

	diffuse += self_illum_map_sample.rgb;
}

void calc_self_illumination_simple_four_change_color_ps(
in float2 texcoord,
in float3 albedo,
inout float3 diffuse)
{

}

// fixups
#define calc_self_illumination_off_ps calc_self_illumination_none_ps
#define calc_self_illumination_3_channel_self_illum_ps calc_self_illumination_three_channel_ps
#define calc_self_illumination__3_channel_self_illum_ps calc_self_illumination_three_channel_ps
#define calc_self_illumination_from_diffuse_ps calc_self_illumination_from_albedo_ps
#define calc_self_illumination_illum_detail_ps calc_self_illumination_detail_ps
#define calc_self_illumination_self_illum_times_diffuse_ps calc_self_illumination_times_diffuse_ps

#ifndef calc_self_illumination_ps
#define calc_self_illumination_ps calc_self_illumination_none_ps
#endif

#endif
