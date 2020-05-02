#ifndef _SH_HLSL
#define _SH_HLSL

#include "../registers/shader.hlsli"
#include "math.hlsli"
#include "color_processing.hlsli"

float4 sample_lightprobe_texture_array(int band_index, float2 texcoord, float compression_factor)
{
	float4 result;
	float4 sample1 = tex3D(lightprobe_texture_array, float3(texcoord, 0.0625 + band_index * 0.25));
	float4 sample2 = tex3D(lightprobe_texture_array, float3(texcoord, 0.1875 + band_index * 0.25));
	result.xyz = 2.0 * (sample1.xyz + sample2.xyz) - 2.0;
	result.w = sample1.w * sample2.w * compression_factor;
	result.xyz = result.w * result.xyz;
	return result;
}

float4 sample_dominant_light_intensity_texture_array(float2 texcoord, float compression_factor)
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

float3 lightmap_diffuse_reflectance(float3 normal, float2 lightmap_texcoord)
{
	float4 sh[4];
	float4 sh_0;
	float4 sh_312[3];
	float4 dominant_light_intensity;
	float3 dominant_light_dir;

	sh[0] = sample_lightprobe_texture_array(0, lightmap_texcoord, p_lightmap_compress_constant_0.x);
	sh[1] = sample_lightprobe_texture_array(1, lightmap_texcoord, p_lightmap_compress_constant_0.y);
	sh[2] = sample_lightprobe_texture_array(2, lightmap_texcoord, p_lightmap_compress_constant_0.z);
	sh[3] = sample_lightprobe_texture_array(3, lightmap_texcoord, p_lightmap_compress_constant_1.x);
	
	dominant_light_intensity = sample_dominant_light_intensity_texture_array(lightmap_texcoord, p_lightmap_compress_constant_1.y);
	dominant_light_dir = calc_dominant_light_dir(sh);
	
	sh_0 = sh[0];
	
	sh_312[0] = float4(sh[3].r, sh[1].r, -sh[2].r, 1.0f);
	sh_312[1] = float4(sh[3].g, sh[1].g, -sh[2].g, 1.0f);
	sh_312[2] = float4(sh[3].b, sh[1].b, -sh[2].b, 1.0f);
	
	// add dominant light contribution
	sh_0.rgb += 0.28209478f * -dominant_light_intensity.rgb;
	sh_312[0].xyz += -0.4886025f * dominant_light_dir.xyz * -dominant_light_intensity.r;
	sh_312[1].xyz += -0.4886025f * dominant_light_dir.xyz * -dominant_light_intensity.g;
	sh_312[2].xyz += -0.4886025f * dominant_light_dir.xyz * -dominant_light_intensity.b;
	
	float c2 = 0.511664f;
	float c4 = 0.886227f;
	float3 x1;
	//linear
	x1.r = dot(normal, sh_312[0].xyz);
	x1.g = dot(normal, sh_312[1].xyz);
	x1.b = dot(normal, sh_312[2].xyz);
	
	float3 lightprobe_color = c4 * sh_0.rgb + (-2.f * c2) * x1;
	lightprobe_color /= PI;
	
	float3 intensity_unknown = 0.280999988 * dominant_light_intensity * dot(normal, dominant_light_dir);
	return lightprobe_color + intensity_unknown;
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
	
	return lightprobe_color / 3.1415926535f;
}

void pack_constants(in float3 sh[9], out float4 lc[10])
{
	lc[0] = float4(sh[0], 0);
	lc[1] = float4(sh[3].r, sh[1].r, -sh[2].r, 0);
	lc[2] = float4(sh[3].g, sh[1].g, -sh[2].g, 0);
	lc[3] = float4(sh[3].b, sh[1].b, -sh[2].b, 0);
	lc[4] = float4(-sh[4].r, sh[5].r, sh[7].r, 0);
	lc[5] = float4(-sh[4].g, sh[5].g, sh[7].g, 0);
	lc[6] = float4(-sh[4].b, sh[5].b, sh[7].b, 0);
	lc[7] = float4(-sh[8].r, sh[8].r, -sh[6].r * 1.7320508f, sh[6].r * 1.7320508f);
	lc[8] = float4(-sh[8].g, sh[8].g, -sh[6].g * 1.7320508f, sh[6].g * 1.7320508f);
	lc[9] = float4(-sh[8].b, sh[8].b, -sh[6].b * 1.7320508f, sh[6].b * 1.7320508f);
}

float3 sh_rotate_023(int irgb, float3 rotate_x, float3 rotate_z, float4 sh_0, float4 sh_312[3])
{
	float3 result = float3(sh_0[irgb], -dot(rotate_z.xyz, sh_312[irgb].xyz), dot(rotate_x.xyz, sh_312[irgb].xyz));
	return result;
}

void calc_material_analytic_specular_cook_torrance_ps(
in float3 view_dir,
in float3 normal_dir,
in float3 reflect_dir,
in float3 light_dir,
in float3 light_intensity,
in float3 c_fresnel_f0,
in float c_toughness,
out float3 analytic_specular)
{
	float n_dot_l = dot(normal_dir, light_dir);
	float n_dot_v = dot(normal_dir, view_dir);
	float min_dot = min(n_dot_l, n_dot_v);
	if (min_dot > 0)
	{
 		// geometric attenuation
		float3 half_vector = normalize(view_dir + light_dir);
		float n_dot_h = dot(normal_dir, half_vector);
		float v_dot_h = dot(view_dir, half_vector);
		float G = 2 * n_dot_h * min_dot / (saturate(v_dot_h));
		//calculate fresnel term
		float3 f0 = c_fresnel_f0;
		float3 sqrt_f0 = sqrt(f0);
		float3 n = (1.f + sqrt_f0) / (1.0 - sqrt_f0);
		float3 g = sqrt(n * n + v_dot_h * v_dot_h - 1.f);
		float3 gpc = g + v_dot_h;
		float3 gmc = g - v_dot_h;
		float3 r = (v_dot_h * gpc - 1.f) / (v_dot_h * gmc + 1.f);
		float3 F = (0.5f * ((gmc * gmc) / (gpc * gpc + 0.00001f)) * (1.f + r * r));
		//calculate the distribution term
		float t_roughness = c_toughness;
		float m_squared = t_roughness * t_roughness;
		float cosine_alpha_squared = n_dot_h * n_dot_h;
		float D;
		D = exp((cosine_alpha_squared - 1) / (m_squared * cosine_alpha_squared)) / (m_squared * cosine_alpha_squared * cosine_alpha_squared);
		//puting it all together
		analytic_specular = D * saturate(G) / (3.14159265 * n_dot_v) * F;
		analytic_specular *= light_intensity;
	}
	else
	{
		analytic_specular = 0.0f;
	}
}

void area_specular_cook_torrance(
in float3 view_dir,
in float3 rotate_z,
in float4 sh_0,
in float4 sh_312[3],
in float4 sh_457[3],
in float4 sh_8866[3],
in float roughness,
in float r_dot_l,
out float3 area_specular)
{
	float3 specular_part;
	float3 schlick_part;
	//build the local frame
	float3 rotate_x = normalize(view_dir - dot(view_dir, rotate_z) * rotate_z);
	float3 rotate_y = cross(rotate_z, rotate_x);
	//calculate the texure coord for lookup
	float2 view_lookup = float2(dot(view_dir, rotate_x), roughness);
	// bases: 0,2,3,6
	float4 c_value = tex2D(g_sampler_cc0236, view_lookup);
	float4 d_value = tex2D(g_sampler_dd0236, view_lookup);
	//rotate lighting basis 0,2,3,6 into local frame
	float4 quadratic_a, quadratic_b, sh_local;
	quadratic_a.xyz = rotate_z.yzx * rotate_z.xyz * (-SQRT3);
	quadratic_b = float4(rotate_z.xyz * rotate_z.xyz, 1.0f / 3.0f) * 0.5f * (-SQRT3);
	//red
	sh_local.xyz = sh_rotate_023(0,rotate_x,rotate_z,sh_0,sh_312);
	sh_local.w = dot(quadratic_a.xyz,
	sh_457[0].xyz) + dot(quadratic_b.xyzw, sh_8866[0].xyzw);
	//dot with C and D look up
	sh_local *= float4(1.0f, r_dot_l, r_dot_l, r_dot_l);
	specular_part.r = dot(c_value, sh_local);
	schlick_part.r = dot(d_value, sh_local);
	//repeat for green and blue

	// basis - 7
	c_value = tex2D(g_sampler_c78d78, view_lookup);
	quadratic_a.xyz = rotate_x.xyz * rotate_z.yzx + rotate_x.yzx *
	rotate_z.xyz;
	quadratic_b.xyz = rotate_x.xyz * rotate_z.xyz;
	sh_local.rgb = float3(dot(quadratic_a.xyz, sh_457[0].xyz) +
	dot(quadratic_b.xyz, sh_8866[0].xyz),
	dot(quadratic_a.xyz, sh_457[1].xyz) +
	dot(quadratic_b.xyz, sh_8866[1].xyz),
	dot(quadratic_a.xyz, sh_457[2].xyz) +
	dot(quadratic_b.xyz, sh_8866[2].xyz));
	sh_local *= r_dot_l;
	//c7 * L7
	specular_part.rgb += c_value.x * sh_local.rgb;
	//d7 * L7
	schlick_part.rgb += c_value.z * sh_local.rgb;
	//basis - 8
	quadratic_a.xyz = rotate_x.xyz * rotate_x.yzx - rotate_y.yzx *
	rotate_y.xyz;
	quadratic_b.xyz = 0.5f * (rotate_x.xyz * rotate_x.xyz - rotate_y.xyz *
	rotate_y.xyz);
	sh_local.rgb = float3(-dot(quadratic_a.xyz, sh_457[0].xyz) -
	dot(quadratic_b.xyz, sh_8866[0].xyz),-dot(quadratic_a.xyz, sh_457[1].xyz) -dot(quadratic_b.xyz, sh_8866[1].xyz),-dot(quadratic_a.xyz, sh_457[2].xyz) -dot(quadratic_b.xyz, sh_8866[2].xyz));
	sh_local *= r_dot_l;
	//c8 * L8
	specular_part.rgb += c_value.y * sh_local.rgb;
	//d8 * L8
	schlick_part.rgb += c_value.w * sh_local.rgb;
	schlick_part = schlick_part * 0.01f;
	area_specular = specular_part * k_f0 + (1 - k_f0) * schlick_part;
}
#endif
