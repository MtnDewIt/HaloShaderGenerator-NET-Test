#ifndef _MATERIAL_SHARED_PARAMETERS_HLSLI
#define _MATERIAL_SHARED_PARAMETERS_HLSLI

#include "..\registers\shader.hlsli"
#include "..\helpers\lighting.hlsli"


uniform sampler2D material_texture;

uniform float diffuse_coefficient;
uniform float specular_coefficient;

uniform float area_specular_contribution;
uniform float analytical_specular_contribution;
uniform float environment_map_specular_contribution;

xform2d material_texture_xform;


// cook torrance parameters

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


uniform bool order3_area_specular;

// two lobe phong

uniform float normal_specular_power;
uniform float3 normal_specular_tint;
uniform float glancing_specular_power;
uniform float3 glancing_specular_tint;
uniform float fresnel_curve_steepness;
uniform float albedo_specular_tint_blend;

uniform float analytical_anti_shadow_control;
uniform bool no_dynamic_lights;
uniform bool use_material_texture;


void calc_material_lambert_diffuse_ps(
float3 normal,
float3 vertex_world_position,
float3 reflect_dir,
float roughness_unknown,
inout float3 diffuse_accumulation,
inout float3 specular_accumulation)
{
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



#endif
