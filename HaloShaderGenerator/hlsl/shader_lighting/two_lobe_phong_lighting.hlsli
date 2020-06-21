#ifndef _TWO_LOBE_PHONG_LIGHTING_HLSLI
#define _TWO_LOBE_PHONG_LIGHTING_HLSLI

#include "..\methods\specular_mask.hlsli"
#include "..\material_models\material_shared_parameters.hlsli"
#include "..\material_models\two_lobe_phong.hlsli"

#include "..\helpers\math.hlsli"
#include "..\registers\shader.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"

void calc_dynamic_lighting_two_lobe_phong_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
	// lambertian diffuse
	float v_dot_n = dot(common_data.light_direction, common_data.surface_normal);
	
	float3 reflect_dir = 2 * v_dot_n * -common_data.surface_normal + common_data.view_dir;
	reflect_dir = normalize(reflect_dir);
	
	float specular_mask = 1.0;
	calc_specular_mask_ps(common_data.albedo, common_data.texcoord, specular_mask);
	float3 specular_contribution = specular_mask * specular_coefficient * analytical_specular_contribution;
	
	float l_dot_r = dot(common_data.light_direction, reflect_dir);
	
	float sn_dot_n = dot(common_data.view_dir, common_data.surface_normal);
	float fresnel_curve = pow(1 - sn_dot_n, fresnel_curve_steepness);
	fresnel_curve = sn_dot_n < 0 ? 1 : fresnel_curve;
		
	float3 specular_tint = normal_specular_tint + fresnel_curve * (glancing_specular_tint - normal_specular_tint);
	specular_tint = lerp(specular_tint, common_data.albedo.rgb, albedo_specular_tint_blend);
		
	float specular_exponent = normal_specular_power + fresnel_curve * (glancing_specular_power - normal_specular_power);
	float specular_power = pow(l_dot_r, specular_exponent) * (specular_exponent + 1.0f);
	specular_power *= INV_2PI;
	
	color = common_data.light_intensity * v_dot_n * common_data.albedo.rgb * diffuse_coefficient;
	
	[flatten]
	if (dot(specular_contribution, specular_contribution) > 0.0001f)
	{
		float3 analytic_specular;
		analytic_specular = specular_power * specular_tint;
		analytic_specular *= common_data.light_intensity;
		analytic_specular = l_dot_r > 0 ? analytic_specular : 0;
		analytic_specular *= specular_contribution;
		color += analytic_specular;
	}
}

float3 calc_lighting_two_lobe_phong_ps(SHADER_COMMON common_data)
{
	float3 color = 0;

	float l_dot_r = dot(common_data.dominant_light_direction, common_data.reflect_dir);
	
	float sn_dot_n = dot(common_data.n_view_dir, common_data.surface_normal);
	float fresnel_curve = pow(1 - sn_dot_n, fresnel_curve_steepness);
	fresnel_curve = sn_dot_n < 0 ? 1 : fresnel_curve;
	
	float specular_exponent = normal_specular_power + fresnel_curve * (glancing_specular_power - normal_specular_power);
	float specular_power = pow(l_dot_r, specular_exponent) * (specular_exponent + 1.0f);
	specular_power *= INV_2PI;
	
	float3 specular_tint = normal_specular_tint + fresnel_curve * (glancing_specular_tint - normal_specular_tint);
	specular_tint = lerp(specular_tint, common_data.albedo.rgb, albedo_specular_tint_blend);
	
	float3 analytic_specular;
	calc_material_analytic_specular_two_lobe_phong_ps(common_data.reflect_dir, common_data.dominant_light_direction, common_data.dominant_light_intensity, specular_tint, specular_power, analytic_specular);
	
	float3 antishadow_control;
	float4 band_1_0_sh_green = float4(common_data.sh_312_no_dominant_light[1].xyz, common_data.sh_0_no_dominant_light.g);
	float sh_intensity_no_dominant_light = dot(band_1_0_sh_green, band_1_0_sh_green);
	float sh_intensity_dominant_light = 1.0 / (common_data.sh_0.g * common_data.sh_0.g + dot(common_data.sh_312[1].xyz, common_data.sh_312[1].xyz));
	float base = sh_intensity_no_dominant_light * sh_intensity_dominant_light - 1.0 < 0 ? (1 - sh_intensity_dominant_light * sh_intensity_no_dominant_light) : 0;
	antishadow_control = analytic_specular * pow(base, 100 * analytical_anti_shadow_control);
	
	bool use_analytical_antishadow_control = analytical_anti_shadow_control.x > 0 ? true : false;
	if (use_analytical_antishadow_control)
		analytic_specular = antishadow_control;
	
	float3 diffuse_accumulation = 0;
	float3 specular_accumulation = 0;
	
	if (!common_data.no_dynamic_lights)
	{
		calc_material_lambert_diffuse_ps(common_data.surface_normal, common_data.world_position, common_data.reflect_dir, specular_exponent, diffuse_accumulation, specular_accumulation);
		specular_accumulation *= specular_exponent;
	}
	
	float3 area_specular;
	calc_material_area_specular_two_lobe_phong_ps(common_data.reflect_dir, common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, specular_tint, area_specular);
	
	analytic_specular = max(analytic_specular, 0);
	analytic_specular = specular_accumulation + analytic_specular;
	area_specular *= area_specular_contribution;
	area_specular = max(area_specular, 0.0f);
	float3 specular = analytic_specular * analytical_specular_contribution + area_specular;
	
	float c_specular_coefficient = common_data.specular_mask * specular_coefficient;
	specular = c_specular_coefficient * specular;
	
	float3 diffuse = common_data.diffuse_reflectance * common_data.precomputed_radiance_transfer + diffuse_accumulation;
	diffuse = diffuse * diffuse_coefficient;
	color.rgb = diffuse * common_data.albedo.rgb + specular;
	
	return color;
}
#endif