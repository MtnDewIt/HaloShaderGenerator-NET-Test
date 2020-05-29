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

void pack_lightmap_constants(
in float4 sh[4],
out float4 sh_0,
out float4 sh_312[3],
out float4 sh_457[3],
out float4 sh_8866[3])
{
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
}

void decompress_lightmap_value(
inout float4 value, 
in float compression_factor)
{
	value.w *= compression_factor;
	value.rgb = (2.0 * value.rgb - 2.0);
	value.rgb = value.rgb * value.w;
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
	float4 temp_dominant_light_intensity;
	
	sh[0] = sample_lightprobe_texture_array(0, lightmap_texcoord, p_lightmap_compress_constant_0.x);
	sh[1] = sample_lightprobe_texture_array(1, lightmap_texcoord, p_lightmap_compress_constant_0.y);
	sh[2] = sample_lightprobe_texture_array(2, lightmap_texcoord, p_lightmap_compress_constant_0.z);
	sh[3] = sample_lightprobe_texture_array(3, lightmap_texcoord, p_lightmap_compress_constant_1.x);
	temp_dominant_light_intensity = sample_dominant_light_intensity_texture_array(lightmap_texcoord, p_lightmap_compress_constant_1.y);
	
	decompress_lightmap_value(sh[0], p_lightmap_compress_constant_0.x);
	decompress_lightmap_value(sh[1], p_lightmap_compress_constant_0.y);
	decompress_lightmap_value(sh[2], p_lightmap_compress_constant_0.z);
	decompress_lightmap_value(sh[3], p_lightmap_compress_constant_1.x);
	decompress_lightmap_value(temp_dominant_light_intensity, p_lightmap_compress_constant_1.y);
	

	pack_lightmap_constants(sh, sh_0, sh_312, sh_457, sh_8866);
	
	dominant_light_intensity = temp_dominant_light_intensity.rgb;
	dominant_light_dir = calc_dominant_light_dir(sh_312);
}

#endif
