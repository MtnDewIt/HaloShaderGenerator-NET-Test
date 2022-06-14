#ifndef _COOK_TORRANCE_LIGHTING_HLSLI
#define _COOK_TORRANCE_LIGHTING_HLSLI

#pragma warning( disable : 3571 34)

#include "..\methods\specular_mask.hlsli"
#include "..\helpers\anti_shadow.hlsli"
#include "..\material_models\cook_torrance.hlsli"
#include "..\helpers\sh.hlsli"
#include "..\methods\self_illumination.hlsli"
#include "..\methods\environment_mapping.hlsli"
#include "..\registers\global_parameters.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"
#include "..\material_models\lambert.hlsli"
#include "..\helpers\apply_hlsl_fixes.hlsli"

float3 get_specular_tint()
{
#if shadertype != k_shadertype_cortana
    if (albedo_arg == k_albedo_four_change_color_applying_to_specular)
        return quaternary_change_color;
#endif
    return specular_tint;
}

void get_material_parameters_2(
in float2 texcoord,
out float c_specular_coefficient,
out float c_albedo_blend,
out float c_roughness,
out float c_environment_map_specular_contribution,
out float c_diffuse_coefficient)
{
    float4 parameters = float4(specular_coefficient, albedo_blend, environment_map_specular_contribution, roughness);
    c_diffuse_coefficient = diffuse_coefficient;
	if (use_material_texture)
	{
        float4 material_texture_sample = tex2D(material_texture, apply_xform2d(texcoord, material_texture_xform));
		
        //if (APPLY_HLSL_FIXES == 0) // HO only / rim_fresnel
			parameters *= material_texture_sample;
		//else
        //    parameters = material_texture_sample;
#ifdef ODST_COOK_TORRANCE
		parameters = material_texture_sample;
		c_diffuse_coefficient = 1.0f - parameters.x;
#endif
    }
	
    c_specular_coefficient = parameters.x;
	c_albedo_blend = parameters.y;
    c_environment_map_specular_contribution = parameters.z;
    c_roughness = parameters.w;
}

void calc_dynamic_lighting_cook_torrance_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
	// lambertian diffuse
    float v_dot_n = dot(common_data.light_direction, common_data.surface_normal);
    color = common_data.light_intensity * v_dot_n * common_data.albedo.rgb * diffuse_coefficient;

	float3 specular_contribution = common_data.specular_mask * specular_coefficient * analytical_specular_contribution;
    specular_contribution *= get_specular_tint();

	[flatten]
	if (dot(specular_contribution, specular_contribution) > 0.0001)
    {
        float c_specular_coefficient, c_albedo_blend, c_roughness, c_environment_map_specular_contribution, c_diffuse_coefficient;
        get_material_parameters_2(
			common_data.texcoord, 
			c_specular_coefficient, 
			c_albedo_blend, 
			c_roughness, 
			c_environment_map_specular_contribution,
			c_diffuse_coefficient);
		
#ifndef ODST_COOK_TORRANCE
        float3 fresnel_f0 = albedo_blend_with_specular_tint ? fresnel_color : lerp(fresnel_color, common_data.albedo.rgb, c_albedo_blend);
#else
		float3 fresnel_f0 = lerp(fresnel_color, common_data.albedo.rgb, c_albedo_blend);
#endif
		
        float3 analytic_specular;
		calc_material_analytic_specular_cook_torrance_ps(
			common_data.view_dir, 
			common_data.surface_normal, 
			common_data.reflect_dir, 
			common_data.light_direction, 
			common_data.light_intensity, 
			fresnel_f0, 
			c_roughness, 
			1.0, 
			analytic_specular);
		
		color += analytic_specular * specular_contribution;
	}
}

float3 calc_lighting_cook_torrance_ps(SHADER_COMMON common_data, out float4 unknown_output)
{
    float c_specular_coefficient, c_albedo_blend, c_roughness, c_environment_map_specular_contribution, c_diffuse_coefficient;
	get_material_parameters_2(
		common_data.texcoord, 
		c_specular_coefficient, 
		c_albedo_blend, 
		c_roughness,
		c_environment_map_specular_contribution,
		c_diffuse_coefficient);
		
#ifndef ODST_COOK_TORRANCE
    float3 fresnel_f0 = albedo_blend_with_specular_tint ? fresnel_color : lerp(fresnel_color, common_data.albedo.rgb, c_albedo_blend);
#else
	float3 fresnel_f0 = lerp(fresnel_color, common_data.albedo.rgb, c_albedo_blend);
#endif
	
	float3 analytic_specular;
	calc_material_analytic_specular_cook_torrance_ps(
		common_data.n_view_dir, 
		common_data.surface_normal, 
		common_data.reflect_dir, 
		common_data.dominant_light_direction, 
		common_data.dominant_light_intensity, 
		fresnel_f0, 
		c_roughness, 
		common_data.precomputed_radiance_transfer.w,
		analytic_specular);
	
    if (analytical_anti_shadow_control.x > 0)
		calc_analytical_specular_with_anti_shadow(common_data, analytical_anti_shadow_control, analytic_specular);
	
	float3 diffuse_accumulation = 0;
	float3 specular_accumulation = 0;
	if (!common_data.no_dynamic_lights)
	{
		float spec_power = 0.272909999 * pow(roughness.x, -2.19729996);
		
        calc_material_lambert_diffuse_ps(
			common_data.surface_normal, 
			common_data.world_position, 
			common_data.reflect_dir, 
			spec_power, 
			diffuse_accumulation, 
			specular_accumulation);
		
        specular_accumulation *= spec_power;
    }
	
	float r_dot_l = dot(common_data.dominant_light_direction.xyz, common_data.reflect_dir);
    float r_dot_l_area_specular = max(r_dot_l, 0.0f) * 0.65f + 0.35f;
		
	float3 area_specular_part = 0;
    float3 rim_area_specular_part = 0;
    float3 env_area_specular_part = 0;
    float3 rim_env_area_specular_part = 0;
		
    if (order3_area_specular)
    {
        calc_cook_torrance_area_specular_order_3(
				common_data.n_view_dir,
				common_data.surface_normal,
				common_data.sh_0,
				common_data.sh_312,
				common_data.sh_457,
				common_data.sh_8866,
				c_roughness,
				fresnel_power,
				r_dot_l_area_specular,
				area_specular_part,
				env_area_specular_part);
		
#ifndef ODST_COOK_TORRANCE
        if (rim_fresnel_coefficient > 0.0f)
        {
            calc_cook_torrance_area_specular_order_3(
				common_data.n_view_dir,
				common_data.surface_normal,
				common_data.sh_0,
				common_data.sh_312,
				common_data.sh_457,
				common_data.sh_8866,
				c_roughness,
				rim_fresnel_power,
				r_dot_l_area_specular,
				rim_area_specular_part,
				rim_env_area_specular_part);
        }
#endif
    }
    else
    {
        calc_cook_torrance_area_specular_order_2(
				common_data.n_view_dir,
				common_data.surface_normal,
				common_data.sh_0,
				common_data.sh_312,
				c_roughness,
				fresnel_power,
				r_dot_l_area_specular,
				area_specular_part,
				env_area_specular_part);
		
#ifndef ODST_COOK_TORRANCE
        if (rim_fresnel_coefficient > 0.0f)
        {
            calc_cook_torrance_area_specular_order_2(
				common_data.n_view_dir,
				common_data.surface_normal,
				common_data.sh_0,
				common_data.sh_312,
				c_roughness,
				rim_fresnel_power,
				r_dot_l_area_specular,
				rim_area_specular_part,
				rim_env_area_specular_part);
        }
#endif
    }
	
    float3 area_specular = area_specular_part * fresnel_f0 + (1 - fresnel_f0) * env_area_specular_part;
		
    float3 env_area_specular = area_specular;
#ifndef ODST_COOK_TORRANCE
    if (use_fresnel_color_environment)
    {
        float3 fresnel_env_f0 = lerp(fresnel_color_environment, common_data.albedo.rgb, c_albedo_blend);
        if (albedo_blend_with_specular_tint)
            fresnel_env_f0 = fresnel_color_environment;
        env_area_specular = area_specular_part * fresnel_env_f0 + (1 - fresnel_env_f0) * env_area_specular_part;
    }
#endif
	
    float3 c_specular_tint = get_specular_tint();
#ifndef ODST_COOK_TORRANCE
    if (albedo_blend_with_specular_tint)
        c_specular_tint = c_specular_tint * (1.0 - c_albedo_blend) + c_albedo_blend * common_data.albedo.rgb;
#endif
	
#if shadertype != k_shadertype_cortana
    if (albedo_arg == k_albedo_four_change_color_applying_to_specular)
    {
        c_specular_tint = tertiary_change_color.rgb - c_specular_tint;
    }
#endif
	
    env_area_specular = env_area_specular * common_data.precomputed_radiance_transfer.z * c_specular_tint;
	
	c_specular_tint = common_data.specular_mask * c_specular_coefficient * c_specular_tint;
	
    float3 specular = c_specular_tint * ((specular_accumulation * fresnel_f0 + analytic_specular) * analytical_specular_contribution + max(area_specular, 0.0f) * area_specular_contribution);
	
#ifndef ODST_COOK_TORRANCE
    specular += common_data.specular_mask * c_specular_coefficient * rim_fresnel_coefficient * lerp(rim_fresnel_color, common_data.albedo.rgb, rim_fresnel_albedo_blend) * rim_env_area_specular_part;
#endif
	float env_specular_contribution = common_data.specular_mask * c_environment_map_specular_contribution * c_specular_coefficient;
	
	float3 diffuse = common_data.diffuse_reflectance * common_data.precomputed_radiance_transfer.x;
	
    float diffuse_coeff = c_diffuse_coefficient;
//#ifdef ODST_COOK_TORRANCE
//	if (use_material_texture)
//        diffuse_coeff = 1.0f - c_specular_coefficient;
//#endif
	
    diffuse = (diffuse_accumulation + diffuse) * diffuse_coeff;
	
    specular *= common_data.precomputed_radiance_transfer.z;
	
	diffuse *= common_data.albedo.rgb;
	
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
    
    float view_tangent = dot(common_data.tangent, common_data.n_view_dir);
    float view_binormal = dot(common_data.binormal, common_data.n_view_dir);
    float3 self_illum = 0;
	calc_self_illumination_ps(0, common_data.texcoord.xy, common_data.albedo.rgb, 0, common_data.view_dir, dot(common_data.n_view_dir, common_data.surface_normal), view_tangent, view_binormal, self_illum);
	
    float3 color;
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