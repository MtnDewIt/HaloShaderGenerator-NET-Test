#ifndef _MATERIAL_MODEL_HLSLI
#define _MATERIAL_MODEL_HLSLI

#include "../helpers/math.hlsli"
#include "../registers/shader.hlsli"
#include "../helpers/lighting.hlsli"
#include "../helpers/sh.hlsli"
#include "../material_models/cook_torrance.hlsli"
#include "../material_models/diffuse_only.hlsli"
#include "../helpers/definition_helper.hlsli"

uniform bool order3_area_specular;
uniform bool no_dynamic_lights;
uniform bool use_material_texture;

uniform float diffuse_coefficient;
uniform float specular_coefficient;

uniform float area_specular_contribution;
uniform float analytical_specular_contribution;
uniform float environment_map_specular_contribution;

xform2d material_texture_xform;


uniform float normal_specular_power;
uniform float3 normal_specular_tint;

float3 fresnel_color;
float fresnel_power;
float roughness;
float albedo_blend;
float3 specular_tint;
bool albedo_blend_with_specular_tint;
float rim_fresnel_coefficient;
float3 rim_fresnel_color;
float rim_fresnel_power;
float rim_fresnel_albedo_blend;

uniform float glancing_specular_power;
uniform float3 glancing_specular_tint;

uniform float fresnel_curve_steepness;
uniform float albedo_specular_tint_blend;
uniform float analytical_anti_shadow_control;


void get_material_parameters_2(
in float2 texcoord,
out float4 parameters,
out float c_diffuse_contribution,
out float c_analytical_specular_contribution,
out float c_area_specular_contribution)
{
	parameters.z = 0;
	if (use_material_texture)
	{
		float2 material_texture_texcoord = apply_xform2d(texcoord, material_texture_xform);
		float4 material_texture_sample = tex2D(material_texture, material_texture_texcoord);
		parameters.x = material_texture_sample.x;
		parameters.y = material_texture_sample.y;
		parameters.w = material_texture_sample.w;
		parameters *= float4(specular_coefficient, albedo_blend, 1.0, roughness);
	}
	else
	{
		parameters.x = specular_coefficient;
		parameters.y = albedo_blend;
		parameters.w = roughness;
	}

	c_diffuse_contribution = diffuse_coefficient;
	c_analytical_specular_contribution = analytical_specular_contribution;
	c_area_specular_contribution = area_specular_contribution;
	
	if (material_type_arg == k_material_model_diffuse_only)
	{
		c_diffuse_contribution = 1.0;
		c_analytical_specular_contribution = 0.0;
		c_area_specular_contribution = 0.0;
	}
}

void get_material_parameters(
in float2 texcoord,
out float c_specular_coefficient,
out float c_albedo_blend,
out float c_roughness)
{
	if (use_material_texture)
	{
		float2 material_texture_texcoord = apply_xform2d(texcoord, material_texture_xform);
		float4 material_texture_sample = tex2D(material_texture, material_texture_texcoord);
		c_specular_coefficient = material_texture_sample.x * specular_coefficient;
		c_albedo_blend = material_texture_sample.y * albedo_blend;
		c_roughness = material_texture_sample.w * roughness;
	}
	else
	{
		c_specular_coefficient = specular_coefficient;
		c_albedo_blend = albedo_blend;
		c_roughness = roughness;
	}
}

void calc_simple_lights(
float3 normal,
float3 vertex_world_position,
float3 reflect_dir,
float roughness_unknown,
out float3 diffuse_accumulation,
out float3 specular_accumulation)
{
	diffuse_accumulation = 0;
	specular_accumulation = 0;
	if (simple_light_count > 0)
	{
		calculate_simple_light(get_simple_light(0), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
		if (simple_light_count > 1)
		{
			calculate_simple_light(get_simple_light(1), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
			if (simple_light_count > 2)
			{
				calculate_simple_light(get_simple_light(2), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
				if (simple_light_count > 3)
				{
					calculate_simple_light(get_simple_light(3), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
					if (simple_light_count > 4)
					{
						calculate_simple_light(get_simple_light(4), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
						if (simple_light_count > 5)
						{
							calculate_simple_light(get_simple_light(5), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
							if (simple_light_count > 6)
							{
								calculate_simple_light(get_simple_light(6), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
								[flatten]
								if (simple_light_count > 7)
								{
									calculate_simple_light(get_simple_light(7), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
								}
							}
						}
					}
				}
			}
		}
	}
}

float3 material_type_diffuse_only(
float3 albedo,
float3 normal,
float3 view_dir,
float2 texcoord,
float3 camera_dir,
float3 world_position,
float4 sh_0,
float4 sh_312[3],
float4 sh_457[3],
float4 sh_8866[3],
float3 light_dir,
float3 light_intensity,
float3 diffuse_reflectance,
bool no_dynamic_lights,
float prt,
float3 vertex_color)
{
	float3 ligthing;
    if (!no_dynamic_lights)
    {
		float3 diffuse_accumulation;
		float3 specular_accumulation;
		
		calc_simple_lights(normal, world_position, 0, 0, diffuse_accumulation, specular_accumulation);
		
		ligthing = diffuse_reflectance * prt + diffuse_accumulation;
	}
	else
		ligthing = diffuse_reflectance * prt;
	ligthing += vertex_color;
	
	return ligthing;
}


float3 material_type_cook_torrance(
float3 albedo, 
float3 normal, 
float3 view_dir, 
float2 texcoord, 
float3 camera_dir,
float3 world_position,
float4 sh_0, 
float4 sh_312[3], 
float4 sh_457[3], 
float4 sh_8866[3],
float3 light_dir,
float3 light_intensity,
float3 diffuse_reflectance,
bool no_dynamic_lights,
float prt,
float3 vertex_color)
{
	float c_specular_coefficient;
	float c_albedo_blend;
	float c_roughness;
	
	float3 reflect_dir = 2 * dot(view_dir, normal) * normal - camera_dir;
	float r_dot_l = dot(reflect_dir, light_dir);
	
	get_material_parameters(texcoord, c_specular_coefficient, c_albedo_blend, c_roughness);
	// to verify

	float3 specular_color = albedo_blend_with_specular_tint.x > 0 ? lerp(fresnel_color, albedo, c_albedo_blend) : fresnel_color;

	float3 analytic_specular;
	calc_material_analytic_specular_cook_torrance_ps(view_dir, normal, reflect_dir, light_dir, light_intensity, specular_color, c_roughness, analytic_specular);
	
	// appearrs to be some code related to rim coefficients missing here
	float3 area_specular = 0;
	if (order3_area_specular)
	{
		calc_material_area_specular_order_3_cook_torrance_ps(view_dir, view_dir, sh_0, sh_312, sh_457, sh_8866, c_roughness, r_dot_l, area_specular);
	}
	else
	{
		calc_material_area_specular_order_2_cook_torrance_ps(view_dir, view_dir, sh_0, sh_312, c_roughness, r_dot_l, area_specular);
	}
	
	bool use_albedo_blend_with_specular_tint = albedo_blend_with_specular_tint.x > 0 ? true : false;
	bool use_analytical_antishadow_control = analytical_anti_shadow_control.x > 0 ? true : false;
	
	float3 diffuse_accumulation;
	float3 specular_accumulation;
	if (no_dynamic_lights)
	{
		diffuse_accumulation = 0;
		specular_accumulation = 0;
	}
	else
	{
		diffuse_accumulation = 0;
		specular_accumulation = 0;
		float roughness_unknown = 0.272909999 * pow(abs(roughness.x), -2.19729996);
		calc_simple_lights(normal, world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
		specular_accumulation *= roughness_unknown;
	}
	
	float3 c_specular_tint = specular_tint;
	
	if (use_albedo_blend_with_specular_tint)
	{
		c_specular_tint = lerp(specular_tint, albedo, c_albedo_blend);
	}
	c_specular_tint *= c_specular_coefficient;
	
	
	
	float3 color = 0;
	color += (analytic_specular + specular_accumulation) * specular_color * analytical_specular_contribution.x;
	
	color += area_specular * area_specular_contribution.x;
	
	color = color < 0 ? 0 : color;
	
	

	float fresnel_coefficient = rim_fresnel_coefficient.x * c_specular_coefficient;
	float3 fresnel_contrib = fresnel_coefficient * (rim_fresnel_color * (1.0 - rim_fresnel_albedo_blend.x) + rim_fresnel_albedo_blend.x * albedo);
	
	color += c_specular_tint * color + fresnel_contrib * area_specular;
	color +=  (diffuse_accumulation + diffuse_reflectance) * diffuse_coefficient.x;
	
	return color;
}
/*
float3 material_type_two_lobe_phong(MATERIAL_TYPE_ARGS)
{
    return material_type_diffuse_only(MATERIAL_TYPE_ARGNAMES);
}

float3 material_type_foliage(MATERIAL_TYPE_ARGS)
{
    return material_type_diffuse_only(MATERIAL_TYPE_ARGNAMES);
}

float3 material_type_none(MATERIAL_TYPE_ARGS)
{
    return 0;
}

float3 material_type_glass(MATERIAL_TYPE_ARGS)
{
    return material_type_diffuse_only(MATERIAL_TYPE_ARGNAMES);
}

float3 material_type_organism(MATERIAL_TYPE_ARGS)
{
    return material_type_diffuse_only(MATERIAL_TYPE_ARGNAMES);
}

float3 material_type_single_lobe_phong(MATERIAL_TYPE_ARGS)
{
    return material_type_diffuse_only(MATERIAL_TYPE_ARGNAMES);
}

float3 material_type_car_paint(MATERIAL_TYPE_ARGS)
{
    return material_type_diffuse_only(MATERIAL_TYPE_ARGNAMES);
}

float3 material_type_hair(MATERIAL_TYPE_ARGS)
{
    return float3(0, 1, 0);
}*/

#ifndef material_type
#define material_type material_type_cook_torrance
#endif

#ifndef calc_material_analytic_specular
#define calc_material_analytic_specular calc_material_analytic_specular_diffuse_only_ps
#endif

#ifndef calc_material_area_specular
#define calc_material_area_specular_order_3 calc_material_area_specular_order_3_diffuse_only_ps
#define calc_material_area_specular_order_2 calc_material_area_specular_order_2_diffuse_only_ps
#endif

#endif
