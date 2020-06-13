#ifndef _DIFFUSE_ONLY_HLSL
#define _DIFFUSE_ONLY_HLSL


void calc_material_analytic_specular_diffuse_only_ps(
in float3 view_dir,
in float3 normal_dir,
in float3 reflect_dir,
in float3 light_dir,
in float3 light_intensity,
in float3 c_fresnel_f0,
in float c_toughness,
out float3 analytic_specular)
{
	analytic_specular = 0.0f;
}

void calc_material_area_specular_order_3_diffuse_only_ps(
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
	area_specular = 0.0f;
}

void calc_material_area_specular_order_2_diffuse_only_ps(
in float3 view_dir,
in float3 rotate_z,
in float4 sh_0,
in float4 sh_312[3],
in float roughness,
in float r_dot_l,
out float3 area_specular)
{
	area_specular = 0.0f;
}

#endif
