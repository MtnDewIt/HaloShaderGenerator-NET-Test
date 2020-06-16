#ifndef _MATERIAL_SHARED_PARAMETERS_HLSLI
#define _MATERIAL_SHARED_PARAMETERS_HLSLI

#include "..\registers\shader.hlsli"
#include "..\helpers\lighting.hlsli"


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

uniform bool order3_area_specular;
uniform bool no_dynamic_lights;
uniform bool use_material_texture;

void get_material_parameters_2(
in float2 texcoord,
out float c_specular_coefficient,
out float c_albedo_blend,
out float c_roughness,
out float c_diffuse_contribution,
out float c_analytical_specular_contribution,
out float c_area_specular_contribution)
{
	float4 parameters;
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
	
	c_roughness = parameters.w;
	c_albedo_blend = parameters.y;
	c_specular_coefficient = parameters.x;
}


void calc_material_lambert_diffuse_ps(
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

#endif
