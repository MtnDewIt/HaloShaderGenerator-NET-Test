#ifndef _SINGLE_LOBE_PHONG_LIGHTING_HLSLI
#define _SINGLE_LOBE_PHONG_LIGHTING_HLSLI

#include "..\material_models\lambert.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\sh.hlsli"
#include "..\methods\environment_mapping.hlsli"
#include "..\methods\self_illumination.hlsli"
#include "..\material_models\single_lobe_phong.hlsli"

void calc_dynamic_lighting_single_lobe_phong_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
	float l_dot_n = dot(common_data.light_direction, common_data.surface_normal);
	color = common_data.light_intensity * l_dot_n * common_data.albedo.rgb;
}

float3 calc_lighting_single_lobe_phong_ps(SHADER_COMMON common_data, out float4 unknown_output)
{
	float3 color = 0;

	float3 analytic_specular;
	calc_material_analytic_specular_single_lobe_phong_ps(common_data.reflect_dir, common_data.dominant_light_direction, common_data.dominant_light_intensity, roughness, analytic_specular);

	
	float3 area_specular;
	calc_material_area_specular_single_lobe_phong_ps(common_data.reflect_dir, common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, specular_tint, area_specular);
	
	
	float3 diffuse_accumulation = 0;
	float3 specular_accumulation = 0;
	if (!common_data.no_dynamic_lights)
	{
		float dynamic_light_roughness = 0.272909999 * pow(roughness, -2.19729996);
		calc_material_lambert_diffuse_ps(common_data.surface_normal, common_data.world_position, common_data.reflect_dir, dynamic_light_roughness, diffuse_accumulation, specular_accumulation);
		specular_accumulation *= dynamic_light_roughness;
	}
	
	
	float3 diffuse = common_data.diffuse_reflectance + diffuse_accumulation;
	diffuse *= diffuse_coefficient;
	diffuse *= common_data.albedo.rgb;
	
	float3 env_area_specular = area_specular;
	area_specular *= area_specular_contribution;
	
	analytic_specular *= analytical_specular_contribution;
	analytic_specular = analytical_specular_contribution > 0 ? analytic_specular : 0;
	

	float3 specular = area_specular + analytic_specular + specular_accumulation;
	specular *= specular_coefficient;
	specular *= common_data.specular_mask;
	specular *= specular_tint;

	float envmap_specular_contribution = common_data.specular_mask * environment_map_specular_contribution * specular_coefficient;
	float env_specular_exponent = roughness;
	
	env_area_specular = max(env_area_specular, 0.001);
	env_area_specular *= common_data.precomputed_radiance_transfer.z;
	
	ENVIRONMENT_MAPPING_COMMON env_mapping_common_data;
	
	env_mapping_common_data.reflect_dir = common_data.reflect_dir;
	env_mapping_common_data.view_dir = common_data.view_dir;
	env_mapping_common_data.env_area_specular = env_area_specular;
	env_mapping_common_data.specular_coefficient = envmap_specular_contribution;
	env_mapping_common_data.area_specular = area_specular;
	env_mapping_common_data.specular_exponent = env_specular_exponent;
	
	float3 env_color = 0;
	envmap_type(env_mapping_common_data, env_color, unknown_output);
	
    float3 n_view;
    n_view.x = dot(common_data.n_view_dir, common_data.normal);
    n_view.y = dot(common_data.n_view_dir, common_data.binormal);
    n_view.z = dot(common_data.n_view_dir, common_data.tangent);
	
    float view_tangent = dot(common_data.tangent, common_data.n_view_dir);
    float view_binormal = dot(common_data.binormal, common_data.n_view_dir);
	
	float3 self_illum = 0;
	calc_self_illumination_ps(0, common_data.texcoord.xy, common_data.albedo.rgb, n_view, common_data.view_dir, dot(common_data.n_view_dir, common_data.surface_normal), view_tangent, view_binormal, self_illum);
	
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