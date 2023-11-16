#ifndef _MATERIAL_LAMBERT_HLSLI
#define _MATERIAL_LAMBERT_HLSLI

#pragma warning( disable : 3571 34)

#include "..\registers\global_parameters.hlsli"
#include "..\helpers\lighting.hlsli"

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
		calculate_lambert_diffuse_simple_light(get_simple_light(0), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
		if (simple_light_count > 1)
		{
			calculate_lambert_diffuse_simple_light(get_simple_light(1), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
			if (simple_light_count > 2)
			{
				calculate_lambert_diffuse_simple_light(get_simple_light(2), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
				if (simple_light_count > 3)
				{
					calculate_lambert_diffuse_simple_light(get_simple_light(3), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
					if (simple_light_count > 4)
					{
						calculate_lambert_diffuse_simple_light(get_simple_light(4), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
						if (simple_light_count > 5)
						{
							calculate_lambert_diffuse_simple_light(get_simple_light(5), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
							if (simple_light_count > 6)
							{
								calculate_lambert_diffuse_simple_light(get_simple_light(6), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
								[flatten]
								if (simple_light_count > 7)
								{
									calculate_lambert_diffuse_simple_light(get_simple_light(7), normal, vertex_world_position, reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
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
