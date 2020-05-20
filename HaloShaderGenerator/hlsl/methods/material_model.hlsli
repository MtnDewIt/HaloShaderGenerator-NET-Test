#ifndef _MATERIAL_MODEL_HLSLItempx
#define _MATERIAL_MODEL_HLSLI

#include "../helpers/math.hlsli"
#include "../registers/shader.hlsli"
#include "../helpers/lighting.hlsli"
#include "../helpers/sh.hlsli"
#include "../material_models/cook_torrance.hlsli"

#define MATERIAL_TYPE_ARGS float3 diffuse, float3 normal, float3 view_dir, float2 texcoord, float3 camera_dir, float3 prt_result, float3 vertex_color
#define MATERIAL_TYPE_ARGNAMES diffuse, normal, view_dir, texcoord, camera_dir, prt_result, vertex_color


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
float4 sh_0,
float4 sh_312[3],
float4 sh_457[3],
float4 sh_8866[3],
float3 light_dir,
float3 light_intensity,
float3 diffuse_reflectance,
float prt)
{
	float3 ligthing;
    if (!no_dynamic_lights)
    {
        float3 vertex_world_position = Camera_Position_PS - camera_dir;
		float3 diffuse_accumulation;
		float3 specular_accumulation;
		
		calc_simple_lights(normal, vertex_world_position, 0, 0, diffuse_accumulation, specular_accumulation);
		
		ligthing = diffuse_reflectance * prt + diffuse_accumulation;
	}
	else
		ligthing = diffuse_reflectance * prt;
	
	return albedo * ligthing;
}


float3 material_type_cook_torrance(
float3 albedo, 
float3 normal, 
float3 view_dir, 
float2 texcoord, 
float3 camera_dir, 
float4 sh_0, 
float4 sh_312[3], 
float4 sh_457[3], 
float4 sh_8866[3],
float3 light_dir,
float3 light_intensity,
float3 diffuse_reflectance,
float prt)
{
	float c_specular_coefficient;
	float c_albedo_blend;
	float c_roughness;
	float3 color = 0;

	get_material_parameters(texcoord, c_specular_coefficient, c_albedo_blend, c_roughness);
	// to verify
	float3 specular_color = fresnel_color;
	

	bool use_albedo_blend_with_specular_tint = albedo_blend_with_specular_tint.x > 0 ? true : false;
	bool use_analytical_antishadow_control = analytical_anti_shadow_control.x > 0 ? true : false;
	
	if (use_albedo_blend_with_specular_tint)
	{
		specular_color = lerp(fresnel_color, albedo, c_albedo_blend);
	}
	
	float3 reflect_dir = 2 * dot(normalize(view_dir), normal) * normal - camera_dir;
	
	float3 analytic_specular;
	
	calc_material_analytic_specular_cook_torrance_ps(view_dir, normal, reflect_dir, light_dir, light_intensity, specular_color, c_roughness, analytic_specular);
	
	
	float r_dot_l = dot(reflect_dir, light_dir);
	
	float3 area_specular;
	if (order3_area_specular)
	{
		area_specular_cook_torrance(view_dir, view_dir, sh_0, sh_312, sh_457, sh_8866, c_roughness, r_dot_l, area_specular);
	}
	else
	{
		// sh457, 8866 supposed to be 0 in this case, maybe use custom function
		area_specular_cook_torrance(view_dir, view_dir, sh_0, sh_312, sh_457, sh_8866, c_roughness, r_dot_l, area_specular);
	}
	// appearrs to be some code related to rim coefficients missing here
	
	float3 r3, r4, r7;
	
	float3 diffuse_accumulation;
	float3 specular_accumulation;
	if (no_dynamic_lights)
	{
		diffuse_accumulation = 0;
		specular_accumulation = 0;
	}
	else
	{
		float3 vertex_world_position = Camera_Position_PS - camera_dir;
		diffuse_accumulation = 0;
		specular_accumulation = 0;
		float roughness_unknown = 0.272909999 * pow(roughness.x, -2.19729996);
		calc_simple_lights(normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
		specular_accumulation *= roughness_unknown;
	}
	
	float3 c_specular_tint = specular_tint;
	
	if (use_albedo_blend_with_specular_tint)
	{
		c_specular_tint = lerp(specular_tint, albedo, c_albedo_blend);
	}
	c_specular_tint *= c_specular_coefficient;
	
	
	color = area_specular * area_specular_contribution.x;
	color = color < 0 ? 0 : color;
	color += (analytic_specular + specular_accumulation) * specular_color * analytical_specular_contribution.x;
	

	float fresnel_coefficient = rim_fresnel_coefficient.x * c_specular_coefficient;
	float3 fresnel_contrib = fresnel_coefficient * (rim_fresnel_color * (1.0 - rim_fresnel_albedo_blend.x) + rim_fresnel_albedo_blend.x * albedo);
	
	color += c_specular_tint * (color) + fresnel_contrib * area_specular;
	color += albedo * (diffuse_accumulation + diffuse_reflectance) * diffuse_coefficient.x;
	
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

#endif
