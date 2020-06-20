#ifndef _TWO_LOBE_PHONG_HLSL
#define _TWO_LOBE_PHONG_HLSL

#include "../registers/shader.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/color_processing.hlsli"
#include "../helpers/sh.hlsli"
#include "material_shared_parameters.hlsli"

void calc_material_analytic_specular_two_lobe_phong_ps(
in float3 view_dir,
in float3 normal_dir,
in float3 reflect_dir,
in float3 light_dir,
in float3 light_intensity,
in float3 c_fresnel_f0,
in float c_toughness,
in float m_n_dot_l,
out float3 analytic_specular)
{
	
}

void calc_material_area_specular_order_3_two_lobe_phong_ps(
in float3 view_dir,
in float3 rotate_z,
in float4 sh_0,
in float4 sh_312[3],
in float4 sh_457[3],
in float4 sh_8866[3],
in float roughness,
in float fresnel_power,
in float rim_fresnel_power,
in float rim_fresnel_coefficient,
in float3 k_f0,
in float r_dot_l,
out float3 area_specular_part,
out float3 area_schlick_part,
out float3 rim_area_specular)
{
	area_schlick_part = 0;
	area_specular_part = 0;
	rim_area_specular = 0;
}

void calc_material_area_specular_order_2_two_lobe_phong_ps(
in float3 view_dir,
in float3 rotate_z,
in float4 sh_0,
in float4 sh_312[3],
in float roughness,
in float fresnel_power,
in float rim_fresnel_power,
in float rim_fresnel_coefficient,
in float3 k_f0,
in float r_dot_l,
out float3 area_specular_part,
out float3 area_schlick_part,
out float3 rim_area_specular)
{
	float4 sh_8866[3];
	float4 sh_457[3];
	sh_457[0] = 0;
	sh_457[1] = 0;
	sh_457[2] = 0;
	sh_8866[0] = 0;
	sh_8866[1] = 0;
	sh_8866[2] = 0;
	
	calc_material_area_specular_order_3_two_lobe_phong_ps(view_dir, rotate_z, sh_0, sh_312, sh_457, sh_8866, roughness, fresnel_power, rim_fresnel_power, rim_fresnel_coefficient, k_f0, r_dot_l, area_specular_part, area_schlick_part, rim_area_specular);
}

void calc_material_area_specular_two_lobe_phong_ps(
in float3 view_dir,
in float3 rotate_z,
in float4 sh_0,
in float4 sh_312[3],
in float4 sh_457[3],
in float4 sh_8866[3],
in float roughness,
in float fresnel_power,
in float rim_fresnel_power,
in float rim_fresnel_coefficient,
in float3 k_f0,
in float r_dot_l,
out float3 area_specular,
out float3 rim_area_specular)
{
	float3 specular_part;
	float3 schlick_part;
	if (order3_area_specular)
	{
		calc_material_area_specular_order_3_two_lobe_phong_ps(view_dir, rotate_z, sh_0, sh_312, sh_457, sh_8866, roughness, fresnel_power, rim_fresnel_power, rim_fresnel_coefficient, k_f0, r_dot_l, specular_part, schlick_part, rim_area_specular);
	}
	else
	{
		calc_material_area_specular_order_2_two_lobe_phong_ps(view_dir, rotate_z, sh_0, sh_312, roughness, fresnel_power, rim_fresnel_power, rim_fresnel_coefficient, k_f0, r_dot_l, specular_part, schlick_part, rim_area_specular);
	}
	area_specular = specular_part * k_f0 + (1 - k_f0) * schlick_part;
	
}

#endif
