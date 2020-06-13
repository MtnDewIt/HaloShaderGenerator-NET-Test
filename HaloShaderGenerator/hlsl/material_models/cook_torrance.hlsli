#ifndef _COOK_TORRANCE_HLSL
#define _COOK_TORRANCE_HLSL

#include "../registers/shader.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/color_processing.hlsli"
#include "../helpers/sh.hlsli"

uniform sampler2D g_sampler_cc0236;
uniform sampler2D g_sampler_dd0236;
uniform sampler2D g_sampler_c78d78;
uniform float k_f0;

// Lighting and materials of Halo 3 + modified to eliminate potential divisions by 0

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
		float G = 2 * n_dot_h * min_dot / (saturate(v_dot_h) + 0.00001f);
		//calculate fresnel term
		float3 f0 = min(c_fresnel_f0, 0.999);
		float3 sqrt_f0 = sqrt(f0);
		float3 n = (1.f + sqrt_f0) / (1.0 - sqrt_f0);
		float3 g = sqrt(n * n + v_dot_h * v_dot_h - 1.f);
		float3 gpc = g + v_dot_h;
		float3 gmc = g - v_dot_h;
		float3 r = (v_dot_h * gpc - 1.f) / (v_dot_h * gmc + 1.f);
		float3 F = (0.5f * ((gmc * gmc) / (gpc * gpc + 0.00001f)) * (1.f + r * r));
		//calculate the distribution term
		float t_roughness = max(c_toughness, 0.05);
		float m_squared = t_roughness * t_roughness;
		float cosine_alpha_squared = n_dot_h * n_dot_h;
		float D;
		D = exp((cosine_alpha_squared - 1) / (m_squared * cosine_alpha_squared)) / (m_squared * cosine_alpha_squared * cosine_alpha_squared + 0.00001f);
		//puting it all together
		analytic_specular = D * saturate(G) / (PI * n_dot_v + 0.00001f) * F;
		analytic_specular = min(analytic_specular, 2);
		analytic_specular *= light_intensity;
	}
	else
	{
		analytic_specular = 0.00001f;
	}
}

void calc_material_area_specular_order_3_cook_torrance_ps(
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
	sh_local.w = dot(quadratic_a.xyz, sh_457[0].xyz) + dot(quadratic_b.xyzw, sh_8866[0].xyzw);
	//dot with C and D look up
	sh_local *= float4(1.0f, r_dot_l, r_dot_l, r_dot_l);
	specular_part.r = dot(c_value, sh_local);
	schlick_part.r = dot(d_value, sh_local);
	//repeat for green and blue
	//green
	sh_local.xyz = sh_rotate_023(1, rotate_x, rotate_z, sh_0, sh_312);
	sh_local.w = dot(quadratic_a.xyz, sh_457[1].xyz) + dot(quadratic_b.xyzw, sh_8866[1].xyzw);
	//dot with C and D look up
	sh_local *= float4(1.0f, r_dot_l, r_dot_l, r_dot_l);
	specular_part.g = dot(c_value, sh_local);
	schlick_part.g = dot(d_value, sh_local);
	//green
	sh_local.xyz = sh_rotate_023(2, rotate_x, rotate_z, sh_0, sh_312);
	sh_local.w = dot(quadratic_a.xyz, sh_457[2].xyz) + dot(quadratic_b.xyzw, sh_8866[2].xyzw);
	//dot with C and D look up
	sh_local *= float4(1.0f, r_dot_l, r_dot_l, r_dot_l);
	specular_part.b = dot(c_value, sh_local);
	schlick_part.b = dot(d_value, sh_local);
	
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

void calc_material_area_specular_order_2_cook_torrance_ps(
in float3 view_dir,
in float3 rotate_z,
in float4 sh_0,
in float4 sh_312[3],
in float roughness,
in float r_dot_l,
out float3 area_specular)
{
	float4 sh_8866[3];
	float4 sh_457[3];
	sh_457[0] = 0;
	sh_457[1] = 0;
	sh_457[2] = 0;
	sh_8866[0] = 0;
	sh_8866[1] = 0;
	sh_8866[2] = 0;
	
	calc_material_area_specular_order_3_cook_torrance_ps(view_dir, rotate_z, sh_0, sh_312, sh_457, sh_8866, roughness, r_dot_l, area_specular);
}


#endif
