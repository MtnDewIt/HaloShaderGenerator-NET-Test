#ifndef _TWO_LOBE_PHONG_LIGHTING_HLSLI
#define _TWO_LOBE_PHONG_LIGHTING_HLSLI

#include "..\helpers\input_output.hlsli"
#include "..\methods\specular_mask.hlsli"
#include "..\helpers\anti_shadow.hlsli"
#include "..\material_models\two_lobe_phong.hlsli"
#include "..\helpers\sh.hlsli"
#include "..\methods\environment_mapping.hlsli"
#include "..\methods\self_illumination.hlsli"
#include "..\helpers\math.hlsli"
#include "..\registers\global_parameters.hlsli"

#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"
#include "..\material_models\lambert.hlsli"

void get_two_lobe_phong_parameters(
in SHADER_COMMON common_data,
out float specular_exponent,
out float specular_power,
out float3 specular_tint)
{
	float l_dot_r = dot(common_data.dominant_light_direction, common_data.reflect_dir);
	
	float sn_dot_n = dot(common_data.n_view_dir, common_data.surface_normal);
	float fresnel_curve = pow(1 - sn_dot_n, fresnel_curve_steepness);
	fresnel_curve = sn_dot_n < 0 ? 1 : fresnel_curve;
	
	specular_exponent = normal_specular_power + fresnel_curve * (glancing_specular_power - normal_specular_power);
	specular_power = pow(l_dot_r, specular_exponent) * (specular_exponent + 1.0f);
	specular_power *= INV_2PI;
	
	specular_tint = normal_specular_tint + fresnel_curve * (glancing_specular_tint - normal_specular_tint);
	specular_tint = lerp(specular_tint, common_data.albedo.rgb, albedo_specular_tint_blend);

}


void calc_dynamic_lighting_two_lobe_phong_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
	float v_dot_n = dot(common_data.view_dir, common_data.surface_normal);
	float3 reflect_dir = common_data.view_dir - (common_data.surface_normal * (2 * v_dot_n));
	reflect_dir = normalize(reflect_dir);
	
	float l_dot_n = dot(common_data.light_direction, common_data.surface_normal);
	// lambertian diffuse
	color = common_data.light_intensity * l_dot_n * common_data.albedo.rgb;
	color *= diffuse_coefficient;
	
	float l_dot_r = dot(common_data.light_direction, -reflect_dir);
	
	float sn_dot_n = dot(common_data.view_dir, common_data.surface_normal);
	float fresnel_curve = pow(1 - sn_dot_n, fresnel_curve_steepness);
	fresnel_curve = sn_dot_n < 0 ? 1 : fresnel_curve;
	
	float specular_exponent = normal_specular_power + fresnel_curve * (glancing_specular_power - normal_specular_power);
	float specular_power = pow(l_dot_r, specular_exponent) * (specular_exponent + 1.0f);
	specular_power *= INV_2PI;
	
	float3 c_specular_tint = normal_specular_tint + fresnel_curve * (glancing_specular_tint - normal_specular_tint);
	c_specular_tint = lerp(c_specular_tint, common_data.albedo.rgb, albedo_specular_tint_blend);

	float3 specular_contribution = common_data.specular_mask * specular_coefficient * analytical_specular_contribution;
	
	[flatten]
	if (dot(specular_contribution, specular_contribution) > 0.0001f)
	{
		float3 analytic_specular;
		analytic_specular = specular_power * c_specular_tint;
		analytic_specular *= common_data.light_intensity;
		analytic_specular = l_dot_r > 0 ? analytic_specular : 0;
		analytic_specular *= specular_contribution;
		color += analytic_specular;
	}
}

float3 calc_lighting_two_lobe_phong_ps(SHADER_COMMON common_data, out float4 unknown_output)
{
	float3 color = 0;
	
	float specular_power, specular_exponent;
	float3 specular_tint;
	get_two_lobe_phong_parameters(common_data, specular_exponent, specular_power, specular_tint);
	
	float3 analytic_specular;
	calc_material_analytic_specular_two_lobe_phong_ps(common_data.reflect_dir, common_data.dominant_light_direction, common_data.dominant_light_intensity, specular_tint, specular_power, analytic_specular);
	
	float3 anti_shadow_control;
	calc_analytical_specular_with_anti_shadow(common_data, analytical_anti_shadow_control, analytic_specular, anti_shadow_control);
	
	bool use_analytical_antishadow_control = analytical_anti_shadow_control.x > 0 ? true : false;
	if (use_analytical_antishadow_control)
		analytic_specular = anti_shadow_control;
	

	float3 diffuse_accumulation = 0;
	float3 specular_accumulation = 0;
	
	if (!common_data.no_dynamic_lights)
	{
		calc_material_lambert_diffuse_ps(common_data.surface_normal, common_data.world_position, common_data.reflect_dir, specular_exponent, diffuse_accumulation, specular_accumulation);
		specular_accumulation *= specular_exponent;
	}
	
	float3 area_specular;
	calc_material_area_specular_two_lobe_phong_ps(common_data.reflect_dir, common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, specular_tint, area_specular);
	float3 env_area_specular = area_specular;
	
	analytic_specular = max(analytic_specular, 0);
	analytic_specular = specular_accumulation + analytic_specular;
	area_specular *= area_specular_contribution;
	
	area_specular = max(area_specular, 0.0f);
	float3 specular = analytic_specular * analytical_specular_contribution + area_specular;
	env_area_specular *= common_data.precomputed_radiance_transfer.z;
	float c_specular_coefficient = common_data.specular_mask * specular_coefficient;
	specular = c_specular_coefficient * specular;

	specular *= common_data.precomputed_radiance_transfer.z;
	
	float envmap_specular_contribution = common_data.specular_mask * environment_map_specular_contribution * specular_coefficient;

	float env_specular_exponent = max(1.01 - 0.005 * specular_exponent, 0.01);
	
	float3 diffuse = common_data.precomputed_radiance_transfer.x * common_data.diffuse_reflectance;
	diffuse += diffuse_accumulation;
	diffuse = diffuse * diffuse_coefficient;
	diffuse *= common_data.albedo.rgb;
	env_area_specular = max(env_area_specular, 0.001);
	
	ENVIRONMENT_MAPPING_COMMON env_mapping_common_data;
	
	env_mapping_common_data.reflect_dir = common_data.reflect_dir;
	env_mapping_common_data.view_dir = common_data.view_dir;
	env_mapping_common_data.env_area_specular = env_area_specular;
	env_mapping_common_data.specular_coefficient = envmap_specular_contribution;
	env_mapping_common_data.area_specular = area_specular;
	env_mapping_common_data.specular_exponent = env_specular_exponent;
	
	float3 env_color = 0;
	envmap_type(env_mapping_common_data, env_color, unknown_output);

	float3 self_illum = 0;
	calc_self_illumination_ps(common_data.texcoord.xy, common_data.albedo.rgb, 0, 0, self_illum);
	
	if (self_illum_is_diffuse)
	{
		color = specular + self_illum;
	}
	else
	{
		color.rgb = diffuse + specular;
		color += self_illum;
	}
	
	color += env_color;
	
	return color;
}
#endif