#ifndef _MATERIAL_SHARED_PARAMETERS_HLSLI
#define _MATERIAL_SHARED_PARAMETERS_HLSLI

#pragma warning( disable : 3571 34)

#include "..\registers\shader.hlsli"
#include "..\helpers\lighting.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\sh.hlsli"

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
uniform bool use_fresnel_color_environment;
float3 fresnel_color_environment;
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

uniform float fresnel_coefficient;
uniform float fresnel_curve_steepness;
uniform float fresnel_curve_bias;
//float roughness;

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

void calc_analytical_specular_with_anti_shadow(
in SHADER_COMMON common_data,
in float3 analytic_specular,
out float3 anti_shadow_control)
{
	get_sh_coefficients_no_dominant_light(common_data.dominant_light_direction, common_data.dominant_light_intensity, common_data.sh_0_no_dominant_light, common_data.sh_312_no_dominant_light);
	
	// this part of code determines if there is a shadow correction required when comparing with and without dominant light contribution, since green has the largest
	// contribution to visible light this approximation is faster than using the luminance

	// why is this required is still a mystery, it is probably related to the consequences of having a static dominant light instead of a real dynamic light
	
	float4 band_1_0_sh_green = float4(common_data.sh_312_no_dominant_light[1].xyz, common_data.sh_0_no_dominant_light.g);
	float sh_intensity_no_dominant_light = dot(band_1_0_sh_green, band_1_0_sh_green);
	float sh_intensity_dominant_light = 1.0 / (common_data.sh_0.g * common_data.sh_0.g + dot(common_data.sh_312[1].xyz, common_data.sh_312[1].xyz));
	float base = sh_intensity_no_dominant_light * sh_intensity_dominant_light - 1.0 < 0 ? (1 - sh_intensity_dominant_light * sh_intensity_no_dominant_light) : 0;
	anti_shadow_control = analytic_specular * pow(base, 100 * analytical_anti_shadow_control);
}

#endif
