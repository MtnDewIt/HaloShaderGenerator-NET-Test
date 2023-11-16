#ifndef _LIGHTMAP_HLSL
#define _LIGHTMAP_HLSL

#include "../registers/global_parameters.hlsli"
#include "math.hlsli"
#include "input_output.hlsli"



float4 sample_lightprobe_texture_array(
int band_index, 
float2 texcoord)
{
	float4 result;
	float4 sample1 = tex3D(lightprobe_texture_array, float3(texcoord, 0.0625 + band_index * 0.25));
	float4 sample2 = tex3D(lightprobe_texture_array, float3(texcoord, 0.1875 + band_index * 0.25));
	result.xyz = sample1.xyz + sample2.xyz;
	result.w = sample1.w * sample2.w;
	return result;
}

float4 sample_dominant_light_intensity_texture_array(
float2 texcoord)
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
	dir.xyz += -sh_312[0].xyz * 0.21265601;
	dir.xyz += -sh_312[1].xyz * 0.71515799;
	dir.xyz += -sh_312[2].xyz * 0.07218560;
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

float4 decompress_lightmap_value(
in float4 value1, 
in float4 value2,
in float compression_factor)
{
	float4 result;
	result.w = value1.w * value2.w;
	result.w *= compression_factor;
	result.rgb = value1.rgb + value2.rgb;
	result.rgb = (2.0 * result.rgb - 2.0);
	result.rgb = result.rgb * result.w;
	return result;

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
	/*
	sh[0] = sample_lightprobe_texture_array(0, lightmap_texcoord);
	sh[1] = sample_lightprobe_texture_array(1, lightmap_texcoord);
	sh[2] = sample_lightprobe_texture_array(2, lightmap_texcoord);
	sh[3] = sample_lightprobe_texture_array(3, lightmap_texcoord);
	temp_dominant_light_intensity = sample_dominant_light_intensity_texture_array(lightmap_texcoord);*/
	
	float4 sample1 = tex3D(lightprobe_texture_array, float3(lightmap_texcoord, 0.0625 + 0 * 0.25));
	float4 sample2 = tex3D(lightprobe_texture_array, float3(lightmap_texcoord, 0.1875 + 0 * 0.25));
	float4 sample3 = tex3D(lightprobe_texture_array, float3(lightmap_texcoord, 0.0625 + 1 * 0.25));
	float4 sample4 = tex3D(lightprobe_texture_array, float3(lightmap_texcoord, 0.1875 + 1 * 0.25));
	float4 sample5 = tex3D(lightprobe_texture_array, float3(lightmap_texcoord, 0.0625 + 2 * 0.25));
	float4 sample6 = tex3D(lightprobe_texture_array, float3(lightmap_texcoord, 0.1875 + 2 * 0.25));
	float4 sample7 = tex3D(lightprobe_texture_array, float3(lightmap_texcoord, 0.0625 + 3 * 0.25));
	float4 sample8 = tex3D(lightprobe_texture_array, float3(lightmap_texcoord, 0.1875 + 3 * 0.25));
	float4 sample9 = tex3D(dominant_light_intensity_map, float3(lightmap_texcoord, 0.25));
	float4 sample10 = tex3D(dominant_light_intensity_map, float3(lightmap_texcoord, 0.75));

	sh[0] = decompress_lightmap_value(sample1, sample2, p_lightmap_compress_constant_0.x);

	sh[1] = decompress_lightmap_value(sample3, sample4, p_lightmap_compress_constant_0.y);

	sh[2] = decompress_lightmap_value(sample5, sample6, p_lightmap_compress_constant_0.z);

	sh[3] = decompress_lightmap_value(sample7, sample8, p_lightmap_compress_constant_1.x);

	temp_dominant_light_intensity = decompress_lightmap_value(sample9, sample10, p_lightmap_compress_constant_1.y);
	
	pack_lightmap_constants(sh, sh_0, sh_312, sh_457, sh_8866);
	
	dominant_light_intensity = temp_dominant_light_intensity.rgb;
	dominant_light_dir = calc_dominant_light_dir(sh_312);
}

void unpack_per_vertex_lightmap_coefficients(
in s_per_vertex_lightmap_coefficients coefficients,
out float4 sh_0,
out float4 sh_312[3],
out float4 sh_457[3],
out float4 sh_8866[3],
out float3 dominant_light_dir,
out float3 dominant_light_intensity)
{
    sh_0 = float4(coefficients.color1.x, coefficients.color2.x, coefficients.color3.x, 0.0);
    sh_312[0] = float4(coefficients.color1.w, coefficients.color1.y, -coefficients.color1.z, 0);
	sh_312[1] = float4(coefficients.color2.w, coefficients.color2.y, -coefficients.color2.z, 0);
    sh_312[2] = float4(coefficients.color3.w, coefficients.color3.y, -coefficients.color3.z, 0);
	sh_457[0] = 0;
	sh_457[1] = 0;
	sh_457[2] = 0;
	sh_8866[0] = 0;
	sh_8866[1] = 0;
	sh_8866[2] = 0;
    dominant_light_intensity = coefficients.color4;
    dominant_light_dir = 0.21265601 * coefficients.color1.wyz + 0.71515799 * coefficients.color2.wyz + 0.07218560 * coefficients.color3.wyz;
	dominant_light_dir = normalize(float3(1, 1, -1) * -dominant_light_dir);
}

#endif
