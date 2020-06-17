#ifndef _NONE_HLSL
#define _NONE_HLSL


void calc_material_analytic_specular_none_ps(
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
	analytic_specular = 0.0f;
}

void calc_material_area_specular_none_ps(
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
	area_specular = 0.0f;
	rim_area_specular = 0.0f;
}

#endif
