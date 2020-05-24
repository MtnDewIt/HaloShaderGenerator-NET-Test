#ifndef _SH_HLSL
#define _SH_HLSL

#include "../registers/shader.hlsli"
#include "math.hlsli"
#include "color_processing.hlsli"

float4 sample_lightprobe_texture_array(
int band_index, 
float2 texcoord,
float compression_factor)
{
	float4 result;
	float4 sample1 = tex3D(lightprobe_texture_array, float3(texcoord, 0.0625 + band_index * 0.25));
	float4 sample2 = tex3D(lightprobe_texture_array, float3(texcoord, 0.1875 + band_index * 0.25));
	result.xyz = 2.0 * (sample1.xyz + sample2.xyz) - 2.0;
	result.w = sample1.w * sample2.w * compression_factor;
	result.xyz = result.w * result.xyz;
	return result;
}

float4 sample_dominant_light_intensity_texture_array(
float2 texcoord,
float compression_factor)
{
	float4 result;
	float4 sample1 = tex3D(dominant_light_intensity_map, float3(texcoord, 0.25));
	float4 sample2 = tex3D(dominant_light_intensity_map, float3(texcoord, 0.75));
	result.xyz = 2.0 * (sample1.xyz + sample2.xyz) - 2.0;
	result.w = sample1.w * sample2.w * compression_factor;
	result.xyz = result.w * result.xyz;
	return result;
}

float3 calc_dominant_light_dir(float4 sh[4])
{
	return normalize(float3(-luminance(sh[3].rgb), -luminance(sh[1].rgb), luminance(sh[2].rgb)));
}

void lightmap_diffuse_reflectance(
in float3 normal,
in float4 sh_0,
in float4 sh_312[3],
in float4 sh_457[3],
in float4 sh_8866[3],
in float3 dominant_light_dir,
in float3 dominant_light_intensity,
out float3 diffuse_reflectance)
{
	float c2 = 0.511664f;
	float c4 = 0.886227f;
	float3 x1;
	//linear
	x1.r = dot(normal, sh_312[0].xyz);
	x1.g = dot(normal, sh_312[1].xyz);
	x1.b = dot(normal, sh_312[2].xyz);
	
	float3 lightprobe_color = c4 * sh_0.rgb + (-2.f * c2) * x1;
	lightprobe_color /= PI;
	
	float3 intensity_unknown = 0.280999988 * dominant_light_intensity.rgb * dot(normal, dominant_light_dir);
	diffuse_reflectance = lightprobe_color + intensity_unknown;
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
	
	dominant_light_intensity = sample_dominant_light_intensity_texture_array(lightmap_texcoord, p_lightmap_compress_constant_1.y).rgb;
	dominant_light_dir = calc_dominant_light_dir(sh).xyz;
	
	sh_0 = sh[0];
	
	sh_312[0] = float4(sh[3].r, sh[1].r, -sh[2].r, 1.0f);
	sh_312[1] = float4(sh[3].g, sh[1].g, -sh[2].g, 1.0f);
	sh_312[2] = float4(sh[3].b, sh[1].b, -sh[2].b, 1.0f);
	
	// add dominant light contribution
	sh_0.rgb += 0.28209478f * -dominant_light_intensity.rgb;
	sh_312[0].xyz += -0.4886025f * dominant_light_dir.xyz * -dominant_light_intensity.r;
	sh_312[1].xyz += -0.4886025f * dominant_light_dir.xyz * -dominant_light_intensity.g;
	sh_312[2].xyz += -0.4886025f * dominant_light_dir.xyz * -dominant_light_intensity.b;
	
	// quadratic terms set to 0
	sh_457[0] = 0;
	sh_457[1] = 0;
	sh_457[2] = 0;
	sh_8866[0] = 0;
	sh_8866[1] = 0;
	sh_8866[2] = 0;
}

// Lighting and materials of Halo 3

float3 diffuse_reflectance(float3 normal)
{
	float c1 = 0.429043f;
	float c2 = 0.511664f;
	float c4 = 0.886227f;
	float3 x1, x2, x3;
	//linear
	x1.r = dot(normal, p_lighting_constant_1.rgb);
	x1.g = dot(normal, p_lighting_constant_2.rgb);
	x1.b = dot(normal, p_lighting_constant_3.rgb);
	//quadratic
	float3 a = normal.xyz * normal.yzx;
	x2.r = dot(a.xyz, p_lighting_constant_4.rgb);
	x2.g = dot(a.xyz, p_lighting_constant_5.rgb);
	x2.b = dot(a.xyz, p_lighting_constant_6.rgb);
	float4 b = float4(normal.xyz * normal.xyz, 1.f / 3.f);
	x3.r = dot(b.xyzw, p_lighting_constant_7.rgba);
	x3.g = dot(b.xyzw, p_lighting_constant_8.rgba);
	x3.b = dot(b.xyzw, p_lighting_constant_9.rgba);
	float3 lightprobe_color =
	c4 * p_lighting_constant_0.rgb + (-2.f * c2) * x1 + (-2.f * c1) * x2 - c1 * x3;
	
	return lightprobe_color / PI;
}

void get_current_sh_coefficients_quadratic(
out float4 sh_0,
out float4 sh_312[3],
out float4 sh_457[3],
out float4 sh_8866[3],
out float3 dominant_light_dir,
out float3 dominant_light_intensity)
{
	sh_0 = p_lighting_constant_0;
	sh_312[0] = p_lighting_constant_1;
	sh_312[1] = p_lighting_constant_2;
	sh_312[2] = p_lighting_constant_3;
	sh_457[0] = p_lighting_constant_4;
	sh_457[1] = p_lighting_constant_5;
	sh_457[2] = p_lighting_constant_6;
	sh_8866[0] = p_lighting_constant_7;
	sh_8866[1] = p_lighting_constant_8;
	sh_8866[2] = p_lighting_constant_9;
	dominant_light_dir = k_ps_dominant_light_direction;
	dominant_light_intensity = k_ps_dominant_light_intensity;
}

void get_current_sh_coefficients_linear(
out float4 sh_0,
out float4 sh_312[3],
out float4 sh_457[3],
out float4 sh_8866[3],
out float3 dominant_light_dir,
out float3 dominant_light_intensity)
{
	sh_0 = p_lighting_constant_0;
	sh_312[0] = p_lighting_constant_1;
	sh_312[1] = p_lighting_constant_2;
	sh_312[2] = p_lighting_constant_3;
	sh_457[0] = 0;
	sh_457[1] = 0;
	sh_457[2] = 0;
	sh_8866[0] = 0;
	sh_8866[1] = 0;
	sh_8866[2] = 0;
	dominant_light_dir = k_ps_dominant_light_direction;
	dominant_light_intensity = k_ps_dominant_light_intensity;
}

void pack_constants(
in float3 sh[9],
out float4 lc[10])
{
	lc[0] = float4(sh[0], 0);
	lc[1] = float4(sh[3].r, sh[1].r, -sh[2].r, 0);
	lc[2] = float4(sh[3].g, sh[1].g, -sh[2].g, 0);
	lc[3] = float4(sh[3].b, sh[1].b, -sh[2].b, 0);
	lc[4] = float4(-sh[4].r, sh[5].r, sh[7].r, 0);
	lc[5] = float4(-sh[4].g, sh[5].g, sh[7].g, 0);
	lc[6] = float4(-sh[4].b, sh[5].b, sh[7].b, 0);
	lc[7] = float4(-sh[8].r, sh[8].r, -sh[6].r * SQRT3, sh[6].r * SQRT3);
	lc[8] = float4(-sh[8].g, sh[8].g, -sh[6].g * SQRT3, sh[6].g * SQRT3);
	lc[9] = float4(-sh[8].b, sh[8].b, -sh[6].b * SQRT3, sh[6].b * SQRT3);
}

float3 sh_rotate_023(int irgb, float3 rotate_x, float3 rotate_z, float4 sh_0, float4 sh_312[3])
{
	float3 result = float3(sh_0[irgb], -dot(rotate_z.xyz, sh_312[irgb].xyz), dot(rotate_x.xyz, sh_312[irgb].xyz));
	return result;
}

#endif
