#ifndef _LIGHTMAP_HLSL
#define _LIGHTMAP_HLSL

#include "../registers/shader.hlsli"
#include "math.hlsli"

uniform sampler3D lightprobe_texture_array;
uniform sampler3D dominant_light_intensity_map;

float4 sample_lightprobe_texture_array(
int band_index, 
float2 texcoord,
float compression_factor)
{
	float4 result;
	float4 sample1 = tex3D(lightprobe_texture_array, float3(texcoord, 0.0625 + band_index * 0.25));
	float4 sample2 = tex3D(lightprobe_texture_array, float3(texcoord, 0.1875 + band_index * 0.25));
	result.xyz = sample1.xyz + sample2.xyz;
	result.w = sample1.w * sample2.w;
	return result;
}

float4 sample_dominant_light_intensity_texture_array(
float2 texcoord,
float compression_factor)
{
	float4 result;
	float4 sample1 = tex3D(dominant_light_intensity_map, float3(texcoord, 0.25));
	float4 sample2 = tex3D(dominant_light_intensity_map, float3(texcoord, 0.75));
	result.xyz = sample1.xyz + sample2.xyz;
	result.w = sample1.w * sample2.w;
	return result;
}

float3 calc_dominant_light_dir(float4 sh_312[3])
{
	float3 dir = 0;
	dir.xyz += sh_312[0].xyz * -0.21265601;
	dir.xyz += sh_312[1].xyz * -0.71515799;
	dir.xyz += sh_312[2].xyz * -0.07218560;
	return normalize(dir);
}

void get_lightmap_sh_coefficients(
in float2 lightmap_texcoord,
out float4 sh_0,
out float4 sh_312[3],
out float4 sh_457[3],
out float4 sh_8866[3],
out float3 dominant_light_dir,
out float3 dominant_light_intensity)
{
	float4 sh[4];

	sh[0] = sample_lightprobe_texture_array(0, lightmap_texcoord, p_lightmap_compress_constant_0.x);
	sh[1] = sample_lightprobe_texture_array(1, lightmap_texcoord, p_lightmap_compress_constant_0.y);
	sh[2] = sample_lightprobe_texture_array(2, lightmap_texcoord, p_lightmap_compress_constant_0.z);
	sh[3] = sample_lightprobe_texture_array(3, lightmap_texcoord, p_lightmap_compress_constant_1.x);
	float4 dli = sample_dominant_light_intensity_texture_array(lightmap_texcoord, p_lightmap_compress_constant_1.y);
	
	sh[1].w *= p_lightmap_compress_constant_0.y;
	sh[1].rgb = (2.0 * sh[1].rgb - 2.0);
	sh[1].rgb = sh[1].rgb * sh[1].w;
	
	sh[2].w *= p_lightmap_compress_constant_0.z;
	sh[2].rgb = (2.0 * sh[2].rgb - 2.0);
	sh[2].rgb = sh[2].rgb * sh[2].w;
	
	sh[3].w *= p_lightmap_compress_constant_1.x;
	sh[3].rgb = (2.0 * sh[3].rgb - 2.0);
	sh[3].rgb = sh[3].rgb * sh[3].w;
	
	dli.w *= p_lightmap_compress_constant_1.y;
	dli.rgb = (2.0 * dli.rgb - 2.0);
	dominant_light_intensity = dli.rgb * dli.w;
	
	sh[0].w *= p_lightmap_compress_constant_0.x;
	sh[0].rgb = (2.0 * sh[0].rgb - 2.0);
	sh[0].rgb = sh[0].w * sh[0].rgb;
	
	sh_0 = sh[0];
	sh_312[0] = float4(sh[3].r, sh[1].r, -sh[2].r, 0.0);
	sh_312[1] = float4(sh[3].g, sh[1].g, -sh[2].g, 0.0);
	sh_312[2] = float4(sh[3].b, sh[1].b, -sh[2].b, 0.0);
	sh_457[0] = 0;
	sh_457[1] = 0;
	sh_457[2] = 0;
	sh_8866[0] = 0;
	sh_8866[1] = 0;
	sh_8866[2] = 0;
	
	dominant_light_dir = calc_dominant_light_dir(sh_312);
}

#endif
