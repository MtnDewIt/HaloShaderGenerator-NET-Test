#ifndef _GLASS_LIGHTING_HLSLI
#define _GLASS_LIGHTING_HLSLI

#include "..\material_models\lambert.hlsli"
#include "..\material_models\glass.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\sh.hlsli"
#include "..\methods\environment_mapping.hlsli"
#include "..\methods\self_illumination.hlsli"
#include "..\helpers\definition_helper.hlsli"

void get_material_parameters_glass(
in SHADER_COMMON common_data,
out float c_roughness
)
{
    c_roughness = roughness.x;
}

void calc_dynamic_lighting_glass_ps(
SHADER_DYNAMIC_LIGHT_COMMON common_data,
out float3 color
)
{
    float l_dot_n = dot(common_data.light_direction, common_data.surface_normal);
    color = common_data.light_intensity * l_dot_n * common_data.albedo.rgb;
}

float3 calc_lighting_glass_ps(inout SHADER_COMMON common_data, out float4 unknown_output)
{
    float3 color = 0;
    
    float c_roughness;

	get_material_parameters_glass(common_data, c_roughness);
      
	float sn_dot_n = dot(common_data.n_view_dir, common_data.surface_normal);
	float fresnel_curve = pow(1 - sn_dot_n, fresnel_curve_steepness);
	fresnel_curve = sn_dot_n < 0 ? 1 : fresnel_curve;
    
	float c_fresnel_contribution = fresnel_coefficient + fresnel_curve * (1 - fresnel_coefficient);
	c_fresnel_contribution += fresnel_curve_bias;
	
	float3 analytic_specular = 0;
	calc_material_analytic_specular_glass_ps(common_data.reflect_dir, common_data.dominant_light_direction, common_data.dominant_light_intensity, c_roughness, analytic_specular);

    float3 diffuse_accumulation = 0;
    float3 specular_accumulation = 0;
    if (!common_data.no_dynamic_lights)
    {
        float dynamic_light_roughness = 0.272909999 * pow(roughness.x, -2.19729996);
        calc_material_lambert_diffuse_ps(common_data.surface_normal, common_data.world_position, common_data.reflect_dir, dynamic_light_roughness, diffuse_accumulation, specular_accumulation);
        specular_accumulation *= dynamic_light_roughness;
    }

	float3 area_specular = 0;
	calc_material_area_specular_glass_ps(common_data.reflect_dir, common_data.sh_0, common_data.sh_312, c_roughness, area_specular);
    
	float3 diffuse = diffuse_accumulation + common_data.diffuse_reflectance;
	diffuse *= diffuse_coefficient;
	diffuse *= common_data.albedo.rgb;
	diffuse *= common_data.albedo.a;

	float envmap_specular_contribution = specular_coefficient * c_fresnel_contribution;
	envmap_specular_contribution *= common_data.specular_mask;
    
	ENVIRONMENT_MAPPING_COMMON env_mapping_common_data;
	
	env_mapping_common_data.reflect_dir = common_data.reflect_dir;
	env_mapping_common_data.view_dir = common_data.view_dir;
	env_mapping_common_data.env_area_specular = max(common_data.sh_0.rgb, 0.001);
	env_mapping_common_data.specular_coefficient = envmap_specular_contribution;
	env_mapping_common_data.area_specular = 0;
	env_mapping_common_data.specular_exponent = c_roughness;

	float3 env_color = 0;
	envmap_type(env_mapping_common_data, env_color, unknown_output);
	diffuse += env_color;
    
	float3 temp = area_specular * area_specular_contribution;
	area_specular = area_specular < 0 ? 0 : temp;
	area_specular = area_specular_contribution > 0 ? area_specular : 0;
	
	analytic_specular *= analytical_specular_contribution;
	analytic_specular = analytical_specular_contribution > 0 ? analytic_specular : 0;
    

	float3 specular = area_specular + analytic_specular + specular_accumulation;
	specular *= specular_coefficient;
	specular *= common_data.specular_mask;
	
	float3 self_illum = 0;
	calc_self_illumination_ps(common_data.texcoord.xy, common_data.albedo.rgb, self_illum);
	
	if (self_illum_is_diffuse)
	{
		color += env_color;
		color += specular + self_illum;
	}
	else
	{
		color += diffuse + specular;
		color += self_illum;
	}
	
	common_data.alpha = saturate(c_fresnel_contribution + common_data.albedo.a);
	common_data.albedo.a = 1.0;
    
    return color;
}


#endif