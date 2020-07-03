#ifndef _ORGANISM_PHONG_HLSL
#define _ORGANISM_PHONG_HLSL

#include "../registers/shader.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/definition_helper.hlsli"

uniform float diffuse_coefficient;
uniform float3 diffuse_tint;
uniform float area_specular_coefficient;
uniform float analytical_specular_coefficient;
uniform float3 specular_tint;
uniform float specular_power;
uniform sampler2D specular_map;
uniform sampler2D occlusion_parameter_map;

uniform float rim_coefficient;
uniform float3 rim_tint;
uniform float rim_power;
uniform float rim_start;
uniform float rim_maps_transition_ratio;
uniform float ambient_coefficient;
uniform float3 ambient_tint;

uniform sampler2D subsurface_map;
uniform float subsurface_coefficient;
uniform float3 subsurface_tint;
uniform float subsurface_propagation_bias;
uniform float subsurface_normal_detail;

uniform sampler2D transparence_map;
uniform float transparence_coefficient;
uniform float3 transparence_tint;
uniform float transparence_normal_bias;
uniform float transparence_normal_detail;

uniform float3 final_tint;


void calc_material_analytic_specular_organism_ps(
in float specular_coefficient,
in float3 light_intensity,
out float3 analytic_specular)
{
	analytic_specular = specular_coefficient * light_intensity;
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
in float4 sh_312[3],
out float3 area_specular)
{
	area_specular = float3(dot(reflect_dir, sh_312[0].xyz), dot(reflect_dir, sh_312[1].xyz), dot(reflect_dir, sh_312[2].xyz));
}


#endif
