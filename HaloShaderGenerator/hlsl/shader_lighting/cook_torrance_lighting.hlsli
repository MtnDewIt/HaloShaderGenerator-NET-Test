#ifndef _COOK_TORRANCE_LIGHTING_HLSLI
#define _COOK_TORRANCE_LIGHTING_HLSLI

#include "..\methods\specular_mask.hlsli"
#include "..\material_models\material_shared_parameters.hlsli"
#include "..\material_models\cook_torrance.hlsli"

#include "..\registers\shader.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"


void calc_dynamic_lighting_cook_torrance_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
	float v_dot_n = dot(common_data.light_direction, common_data.surface_normal);
	float c_specular_mask = 1.0;
	calc_specular_mask_ps(common_data.albedo, common_data.texcoord, c_specular_mask);
	float3 specular_contribution = c_specular_mask * specular_coefficient * analytical_specular_contribution;
	
	float c_albedo_blend, c_roughness, c_specular_coefficient;
	float c_diffuse_coefficient, c_analytical_specular_coefficient, c_area_specular_coefficient;
	get_material_parameters_2(common_data.texcoord, c_specular_coefficient, c_albedo_blend, c_roughness, c_diffuse_coefficient, c_analytical_specular_coefficient, c_area_specular_coefficient);
	
	// lambertian diffuse
	color = common_data.light_intensity * v_dot_n * common_data.albedo.rgb * c_diffuse_coefficient;

	specular_contribution *= specular_tint;

	[flatten]
	if (dot(specular_contribution, specular_contribution) > 0.0001)
	{
		float3 analytic_specular;
		float3 fresnel_f0 = albedo_blend_with_specular_tint.x > 0 ? fresnel_color : lerp(fresnel_color, common_data.albedo.rgb, c_albedo_blend);

		calc_material_analytic_specular_cook_torrance_ps(common_data.view_dir, common_data.surface_normal, common_data.reflect_dir, common_data.light_direction, common_data.light_intensity, fresnel_f0, c_roughness, 1.0, analytic_specular);
		color += analytic_specular * specular_contribution;
	}
}

float3 calc_lighting_cook_torrance_ps(SHADER_COMMON common_data)
{
	float3 color = 0;
	float c_albedo_blend, c_roughness, c_specular_coefficient;
	float c_diffuse_coefficient, c_analytical_specular_coefficient, c_area_specular_coefficient;
	get_material_parameters_2(common_data.texcoord, c_specular_coefficient, c_albedo_blend, c_roughness, c_diffuse_coefficient, c_analytical_specular_coefficient, c_area_specular_coefficient);
	bool use_albedo_blend_with_specular_tint = albedo_blend_with_specular_tint.x > 0 ? true : false;
	bool use_analytical_antishadow_control = analytical_anti_shadow_control.x > 0 ? true : false;
		
	float3 analytic_specular;
	float3 fresnel_f0 = use_albedo_blend_with_specular_tint ? fresnel_color : lerp(fresnel_color, common_data.albedo.rgb, c_albedo_blend);
		
	calc_material_analytic_specular_cook_torrance_ps(common_data.n_view_dir, common_data.surface_normal, common_data.reflect_dir, common_data.dominant_light_direction, common_data.dominant_light_intensity, fresnel_f0, c_roughness, dot(common_data.normal, common_data.dominant_light_direction), analytic_specular);

	float3 specular;
	float3 antishadow_control;
	
	// this part of code determines if there is a shadow correction required when comparing with and without dominant light contribution, since green has the largest
	// contribution to visible light this approximation is faster than using the luminance

	// why is this required is still a mystery, it is probably related to the consequences of having a static dominant light instead of a real dynamic light
	
	float4 band_1_0_sh_green;
	
	band_1_0_sh_green.xyz = common_data.sh_312_no_dominant_light[1].xyz;
	band_1_0_sh_green.w = common_data.sh_0_no_dominant_light.g;
	
	float sh_intensity_no_dominant_light = dot(band_1_0_sh_green, band_1_0_sh_green);
	
	float sh_intensity_dominant_light = 1.0 / (common_data.sh_0.g * common_data.sh_0.g + dot(common_data.sh_312[1].xyz, common_data.sh_312[1].xyz));
			
	float base = sh_intensity_no_dominant_light * sh_intensity_dominant_light - 1.0 < 0 ? (1 - sh_intensity_dominant_light * sh_intensity_no_dominant_light) : 0;
	antishadow_control = analytic_specular * pow(base, 100 * analytical_anti_shadow_control);
		
	specular = analytic_specular;
	if (use_analytical_antishadow_control)
		specular = antishadow_control;
	
	float3 diffuse;
	float3 diffuse_accumulation = 0;
	float3 specular_accumulation = 0;
	if (!common_data.no_dynamic_lights)
	{
		float roughness_unknown = 0.272909999 * pow(roughness.x, -2.19729996);
		calc_material_lambert_diffuse_ps(common_data.surface_normal, common_data.world_position, common_data.reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
		specular_accumulation *= roughness_unknown;
	}
	
	
	float r_dot_l = dot(common_data.dominant_light_direction.xyz, common_data.reflect_dir);
	float r_dot_l_area_specular = r_dot_l < 0 ? 0.35f : r_dot_l * 0.65f + 0.35f;
		
	float3 area_specular = 0;
	float3 rim_area_specular = 0;
	
	calc_material_area_specular_cook_torrance_ps(common_data.n_view_dir, common_data.surface_normal, common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, c_roughness, fresnel_power, rim_fresnel_power, rim_fresnel_coefficient, fresnel_f0, r_dot_l_area_specular, area_specular, rim_area_specular);

	calc_specular_mask_ps(common_data.albedo, common_data.texcoord, c_specular_coefficient);

	float3 c_specular_tint = specular_tint;
	
	if (use_albedo_blend_with_specular_tint)
	{
		c_specular_tint = specular_tint * (1.0 - c_albedo_blend) + c_albedo_blend * common_data.albedo.rgb;
	}
	c_specular_tint = c_specular_coefficient * c_specular_tint;
		
	specular += specular_accumulation * fresnel_f0;
	specular *= c_analytical_specular_coefficient;
	specular += area_specular < 0 ? 0.0f : area_specular * c_area_specular_coefficient;

	float fresnel_coefficient = c_specular_coefficient * rim_fresnel_coefficient.x;
	float3 temp = common_data.albedo.rgb - rim_fresnel_color.rgb;
	temp = rim_fresnel_albedo_blend.x * temp + rim_fresnel_color;
	temp *= fresnel_coefficient;
	temp *= rim_area_specular;
	color.rgb += c_specular_tint * specular + temp;
	diffuse = common_data.diffuse_reflectance * common_data.precomputed_radiance_transfer + diffuse_accumulation;
	color.rgb += diffuse * c_diffuse_coefficient * common_data.albedo.rgb;
	return color;
}
#endif