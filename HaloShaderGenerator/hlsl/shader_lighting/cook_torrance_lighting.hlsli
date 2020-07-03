#ifndef _COOK_TORRANCE_LIGHTING_HLSLI
#define _COOK_TORRANCE_LIGHTING_HLSLI

#pragma warning( disable : 3571 34)

#include "..\methods\specular_mask.hlsli"
#include "..\helpers\anti_shadow.hlsli"
#include "..\material_models\cook_torrance.hlsli"
#include "..\helpers\sh.hlsli"
#include "..\methods\self_illumination.hlsli"
#include "..\methods\environment_mapping.hlsli"
#include "..\registers\shader.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"
#include "..\material_models\lambert.hlsli"

void get_material_parameters_2(
in float2 texcoord,
out float c_specular_coefficient,
out float c_albedo_blend,
out float c_roughness,
out float c_diffuse_contribution,
out float c_analytical_specular_contribution,
out float c_area_specular_contribution,
out float c_environment_map_specular_contribution)
{
	float4 parameters;
	parameters.z = 0;
	if (use_material_texture)
	{
		float2 material_texture_texcoord = apply_xform2d(texcoord, material_texture_xform);
		float4 material_texture_sample = tex2D(material_texture, material_texture_texcoord);
		parameters.x = material_texture_sample.x;
		parameters.y = material_texture_sample.y;
		parameters.z = material_texture_sample.z;
		parameters.w = material_texture_sample.w;
		parameters *= float4(specular_coefficient, albedo_blend, environment_map_specular_contribution, roughness);
	}
	else
	{
		parameters.x = specular_coefficient;
		parameters.y = albedo_blend;
		parameters.z = environment_map_specular_contribution;
		parameters.w = roughness;
	}

	c_diffuse_contribution = diffuse_coefficient;
	c_analytical_specular_contribution = analytical_specular_contribution;
	c_area_specular_contribution = area_specular_contribution;
	
	c_roughness = parameters.w;
	c_albedo_blend = parameters.y;
	c_specular_coefficient = parameters.x;
	c_environment_map_specular_contribution = parameters.z;

}

void calc_dynamic_lighting_cook_torrance_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
	float v_dot_n = dot(common_data.light_direction, common_data.surface_normal);

	float3 specular_contribution = common_data.specular_mask * specular_coefficient * analytical_specular_contribution;
	
	float c_albedo_blend, c_roughness, c_specular_coefficient;
	float c_diffuse_coefficient, c_analytical_specular_coefficient, c_area_specular_coefficient, c_environment_map_specular_contribution;
	get_material_parameters_2(common_data.texcoord, c_specular_coefficient, c_albedo_blend, c_roughness, c_diffuse_coefficient, c_analytical_specular_coefficient, c_area_specular_coefficient, c_environment_map_specular_contribution);
	
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

float3 calc_lighting_cook_torrance_ps(SHADER_COMMON common_data, out float4 unknown_output)
{
	float3 color = 0;
	float c_albedo_blend, c_roughness, c_specular_coefficient;
	float c_diffuse_coefficient, c_analytical_specular_coefficient, c_area_specular_coefficient, c_environment_map_specular_contribution;
	get_material_parameters_2(common_data.texcoord, c_specular_coefficient, c_albedo_blend, c_roughness, c_diffuse_coefficient, c_analytical_specular_coefficient, c_area_specular_coefficient, c_environment_map_specular_contribution);
	bool use_albedo_blend_with_specular_tint = albedo_blend_with_specular_tint.x > 0 ? true : false;
	bool use_analytical_antishadow_control = analytical_anti_shadow_control.x > 0 ? true : false;
		
	float3 fresnel_f0 = use_albedo_blend_with_specular_tint ? fresnel_color : lerp(fresnel_color, common_data.albedo.rgb, c_albedo_blend);
	
	float unknown_thing = common_data.precomputed_radiance_transfer.w;
	if (shaderstage == k_shaderstage_static_per_pixel || shaderstage == k_shaderstage_static_per_vertex || shaderstage == k_shaderstage_static_sh)
		unknown_thing = dot(common_data.normal, common_data.dominant_light_direction);
	
	float3 analytic_specular;
	calc_material_analytic_specular_cook_torrance_ps(common_data.n_view_dir, common_data.surface_normal, common_data.reflect_dir, common_data.dominant_light_direction, common_data.dominant_light_intensity, fresnel_f0, c_roughness, unknown_thing, analytic_specular);

	float3 anti_shadow_control;
	calc_analytical_specular_with_anti_shadow(common_data, analytical_anti_shadow_control, analytic_specular, anti_shadow_control);
		
	float3 specular = analytic_specular;
	if (use_analytical_antishadow_control)
		specular = anti_shadow_control;
	
	float3 diffuse;
	float3 diffuse_accumulation = 0;
	float3 specular_accumulation = 0;
	if (!common_data.no_dynamic_lights)
	{
		float roughness_unknown = 0.272909999 * pow(roughness.x, -2.19729996);
		calc_material_lambert_diffuse_ps(common_data.surface_normal, common_data.world_position, common_data.reflect_dir, roughness_unknown, diffuse_accumulation, specular_accumulation);
		specular_accumulation *= roughness_unknown;
	}
	float3 fresnel_env_f0 = use_albedo_blend_with_specular_tint ? fresnel_color_environment : lerp(fresnel_color_environment, common_data.albedo.rgb, c_albedo_blend);
	
	float r_dot_l = dot(common_data.dominant_light_direction.xyz, common_data.reflect_dir);
	float r_dot_l_area_specular = r_dot_l < 0 ? 0.35f : r_dot_l * 0.65f + 0.35f;
		
	float3 area_specular = 0;
	float3 rim_area_specular = 0;
	float3 env_area_specular = 0;
	calc_material_area_specular_cook_torrance_ps(common_data.n_view_dir, common_data.surface_normal, common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, c_roughness, fresnel_power, rim_fresnel_power, rim_fresnel_coefficient, fresnel_f0, fresnel_env_f0, r_dot_l_area_specular, area_specular, rim_area_specular, env_area_specular);
	env_area_specular = use_fresnel_color_environment ? env_area_specular : area_specular;
	float3 c_specular_tint = specular_tint;
	
	if (use_albedo_blend_with_specular_tint)
		c_specular_tint = specular_tint * (1.0 - c_albedo_blend) + c_albedo_blend * common_data.albedo.rgb;
	
	
	env_area_specular *= common_data.precomputed_radiance_transfer.z;
	env_area_specular = env_area_specular * c_specular_tint;
	
	c_specular_tint = common_data.specular_mask * c_specular_coefficient * c_specular_tint;
		
	specular += specular_accumulation * fresnel_f0;
	specular *= c_analytical_specular_coefficient;
	specular += area_specular < 0 ? 0.0f : area_specular * c_area_specular_coefficient;
	
	float fresnel_coefficient = common_data.specular_mask * c_specular_coefficient * rim_fresnel_coefficient.x;
	float3 rim_fresnel = common_data.albedo.rgb - rim_fresnel_color.rgb;
	rim_fresnel = rim_fresnel_albedo_blend.x * rim_fresnel + rim_fresnel_color;
	rim_fresnel *= fresnel_coefficient;
	rim_fresnel *= rim_area_specular;
	
	specular = c_specular_tint * specular + rim_fresnel;

	float env_specular_contribution = common_data.specular_mask * c_environment_map_specular_contribution * c_specular_coefficient;
	
	diffuse = diffuse_accumulation + common_data.diffuse_reflectance * common_data.precomputed_radiance_transfer.x;
	diffuse *= c_diffuse_coefficient;
	diffuse *= common_data.albedo.rgb;
	
	specular *= common_data.precomputed_radiance_transfer.z;
	env_area_specular = max(env_area_specular, 0.001);
	ENVIRONMENT_MAPPING_COMMON env_mapping_common_data;
	
	env_mapping_common_data.reflect_dir = common_data.reflect_dir;
	env_mapping_common_data.view_dir = common_data.view_dir;
	env_mapping_common_data.env_area_specular = env_area_specular;
	env_mapping_common_data.specular_coefficient = env_specular_contribution;
	env_mapping_common_data.area_specular = area_specular;
	env_mapping_common_data.specular_exponent = c_roughness;
	
	float3 env_color = 0;
	envmap_type(env_mapping_common_data, env_color, unknown_output);
	
	float3 self_illum = 0;
	calc_self_illumination_ps(common_data.texcoord.xy, common_data.albedo.rgb, self_illum);
	
	if (self_illum_is_diffuse)
	{
		color = specular + self_illum;
	}
		
	else
	{
		color = diffuse + specular;
		color += self_illum;
	}	
	
	color += env_color;

	return color;
}
#endif