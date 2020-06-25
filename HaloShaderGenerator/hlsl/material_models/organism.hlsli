#ifndef _ORGANISM_PHONG_HLSL
#define _ORGANISM_PHONG_HLSL

#include "../registers/shader.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/color_processing.hlsli"
#include "../helpers/sh.hlsli"
#include "material_shared_parameters.hlsli"
#include "../helpers/definition_helper.hlsli"

void calc_material_analytic_specular_organism_ps(
in float3 reflect_dir,
in float3 light_dir,
in float3 light_intensity,
in float3 specular_tint,
in float specular_power,
out float3 analytic_specular)
{
	analytic_specular = 0;
}

void calc_material_area_specular_order_3_organism_ps(
in float3 reflect_dir,
in float4 sh_0,
in float4 sh_312[3],
in float4 sh_457[3],
in float4 sh_8866[3],
in float3 specular_tint,
out float3 area_specular)
{
	area_specular = 0;
}

void calc_material_area_specular_order_2_organism_ps(
in float3 reflect_dir,
in float4 sh_0,
in float4 sh_312[3],
in float3 specular_tint,
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
	
	calc_material_area_specular_order_3_organism_ps(reflect_dir, sh_0, sh_312, sh_457, sh_8866, specular_tint, area_specular);
}

void calc_material_area_specular_organism_ps(
in float3 reflect_dir,
in float4 sh_0,
in float4 sh_312[3],
in float4 sh_457[3],
in float4 sh_8866[3],
in float3 specular_tint,
out float3 area_specular)
{
	if (order3_area_specular)
	{
		calc_material_area_specular_order_3_organism_ps(reflect_dir, sh_0, sh_312, sh_457, sh_8866, specular_tint, area_specular);
	}
	else
	{
		calc_material_area_specular_order_2_organism_ps(reflect_dir, sh_0, sh_312, specular_tint, area_specular);
	}
}


#endif
