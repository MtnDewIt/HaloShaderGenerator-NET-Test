#ifndef _MATERIAL_MODEL_HLSLItempx
#define _MATERIAL_MODEL_HLSLI

#include "../helpers/math.hlsli"
#include "../registers/shader.hlsli"
#include "../helpers/lighting.hlsli"
#include "../helpers/sh.hlsli"
#include "../material_models/cook_torrance.hlsli"

#define MATERIAL_TYPE_ARGS float3 diffuse, float3 normal, float3 view_dir, float2 texcoord, float3 camera_dir, float3 prt_result, float3 vertex_color
#define MATERIAL_TYPE_ARGNAMES diffuse, normal, view_dir, texcoord, camera_dir, prt_result, vertex_color

float3 material_type_diffuse_only(MATERIAL_TYPE_ARGS)
{
	float3 diffuse_ref = diffuse_reflectance(normal);
	float3 ligthing;

    if (no_dynamic_lights)
    {
		ligthing = diffuse_ref * prt_result;
	}
    else
    {
        float3 vertex_world_position = Camera_Position_PS - camera_dir;
        float3 accumulation = float3(0, 0, 0);
        
		if (simple_light_count > 0)
		{
			accumulation = calculate_simple_light(get_simple_light(0), accumulation, normal, vertex_world_position);
			if (simple_light_count > 1)
			{
				accumulation = calculate_simple_light(get_simple_light(1), accumulation, normal, vertex_world_position);
				if (simple_light_count > 2)
				{
					accumulation = calculate_simple_light(get_simple_light(2), accumulation, normal, vertex_world_position);
					if (simple_light_count > 3)
					{
						accumulation = calculate_simple_light(get_simple_light(3), accumulation, normal, vertex_world_position);
						if (simple_light_count > 4)
						{
							accumulation = calculate_simple_light(get_simple_light(4), accumulation, normal, vertex_world_position);
							if (simple_light_count > 5)
							{
								accumulation = calculate_simple_light(get_simple_light(5), accumulation, normal, vertex_world_position);
								if (simple_light_count > 6)
								{
									accumulation = calculate_simple_light(get_simple_light(6), accumulation, normal, vertex_world_position);
									if (simple_light_count > 7)
									{
										accumulation = calculate_simple_light(get_simple_light(7), accumulation, normal, vertex_world_position);
									}
								}
							}
						}
					}
				}
			}
		}
        
		ligthing = diffuse_ref * prt_result + accumulation;
	}
	return diffuse * (ligthing + vertex_color);
}


float3 material_type_cook_torrance(MATERIAL_TYPE_ARGS)
{
	float c_specular_coefficient;
	float c_albedo_blend;
	float c_roughness;
    float3 color = float3(1, 0, 0);

	get_material_parameters(texcoord, c_specular_coefficient, c_albedo_blend, c_roughness);
	// to verify
    float3 specular_color = fresnel_color;
    if (albedo_blend_with_specular_tint.x)
    {
		specular_color = lerp(fresnel_color, diffuse, c_albedo_blend);
	}

    // diffuse cook torrance?
	
	// area specular (order 3 or not)
	
	// calc_material_analytic_specular_cook_torrance_ps
	
	// iterate on all the lights
	return color;
}

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
}

#ifndef material_type
#define material_type material_type_none
#endif

#endif
