#ifndef _TERRAIN_LIGHTING_HLSLI
#define _TERRAIN_LIGHTING_HLSLI

#include "..\helpers\input_output.hlsli"
#include "..\helpers\terrain_helper.hlsli"
#include "..\methods\terrain_blending.hlsli"
#include "..\material_models\lambert.hlsli"
#include "..\helpers\sh.hlsli"
#include "..\methods\environment_mapping.hlsli"

uniform float diffuse_coefficient_m_0;
uniform float specular_coefficient_m_0;
uniform float specular_power_m_0;
uniform float3 specular_tint_m_0;
uniform float fresnel_curve_steepness_m_0;
uniform float area_specular_contribution_m_0;
uniform float analytical_specular_contribution_m_0;
uniform float environment_specular_contribution_m_0;
uniform float albedo_specular_tint_blend_m_0;
uniform sampler2D self_illum_map_m_0;
uniform float4 self_illum_map_m_0_xform;
uniform sampler2D self_illum_detail_map_m_0;
uniform float4 self_illum_detail_map_m_0_xform;
uniform float3 self_illum_color_m_0;
uniform float self_illum_intensity_m_0;

uniform float diffuse_coefficient_m_1;
uniform float specular_coefficient_m_1;
uniform float specular_power_m_1;
uniform float3 specular_tint_m_1;
uniform float fresnel_curve_steepness_m_1;
uniform float area_specular_contribution_m_1;
uniform float analytical_specular_contribution_m_1;
uniform float environment_specular_contribution_m_1;
uniform float albedo_specular_tint_blend_m_1;
uniform sampler2D self_illum_map_m_1;
uniform float4 self_illum_map_m_1_xform;
uniform sampler2D self_illum_detail_map_m_1;
uniform float4 self_illum_detail_map_m_1_xform;
uniform float3 self_illum_color_m_1;
uniform float self_illum_intensity_m_1;

uniform float diffuse_coefficient_m_2;
uniform float specular_coefficient_m_2;
uniform float specular_power_m_2;
uniform float3 specular_tint_m_2;
uniform float fresnel_curve_steepness_m_2;
uniform float area_specular_contribution_m_2;
uniform float analytical_specular_contribution_m_2;
uniform float environment_specular_contribution_m_2;
uniform float albedo_specular_tint_blend_m_2;
uniform sampler2D self_illum_map_m_2;
uniform float4 self_illum_map_m_2_xform;
uniform sampler2D self_illum_detail_map_m_2;
uniform float4 self_illum_detail_map_m_2_xform;
uniform float3 self_illum_color_m_2;
uniform float self_illum_intensity_m_2;

uniform float diffuse_coefficient_m_3;
uniform float specular_coefficient_m_3;
uniform float specular_power_m_3;
uniform float3 specular_tint_m_3;
uniform float fresnel_curve_steepness_m_3;
uniform float area_specular_contribution_m_3;
uniform float analytical_specular_contribution_m_3;
uniform float environment_specular_contribution_m_3;
uniform float albedo_specular_tint_blend_m_3;
//uniform sampler2D self_illum_map_m_3;
//uniform float4 self_illum_map_m_3_xform;
//uniform sampler2D self_illum_detail_map_m_3;
//uniform float4 self_illum_detail_map_m_3_xform;
//uniform float3 self_illum_color_m_3;
//uniform float self_illum_intensity_m_3;

// add for all the parameters given the index

float get_specular_coefficient(int index)
{
	if (index == 0)
		if (material_type_0_arg == k_material_diffuse_plus_specular)
			return specular_coefficient_m_0;	
	if (index == 1)
		if (material_type_1_arg == k_material_diffuse_plus_specular)
			return specular_coefficient_m_1;
	if (index == 2)
		if (material_type_2_arg == k_material_diffuse_plus_specular)
			return specular_coefficient_m_2;
	if (index == 3)
		if (material_type_3_arg == k_material_diffuse_plus_specular)
			return specular_coefficient_m_3;
	return 0.0f;
}

float get_diffuse_coefficient(int index)
{
	if (index == 0)
	{
		if (material_type_0_arg == k_material_diffuse_plus_specular)
			return diffuse_coefficient_m_0;
		else if (material_type_0_arg == k_material_diffuse_only)
			return 1.0f;
	}
	else if (index == 1)
	{
		if (material_type_1_arg == k_material_diffuse_plus_specular)
			return diffuse_coefficient_m_1;
		else if (material_type_1_arg == k_material_diffuse_only)
			return 1.0f;
	}
	else if (index == 2)
	{
		if (material_type_2_arg == k_material_diffuse_plus_specular)
			return diffuse_coefficient_m_2;
		else if (material_type_2_arg == k_material_diffuse_only)
			return 1.0f;
	}
	else if (index == 3)
	{
		if (material_type_3_arg == k_material_diffuse_plus_specular)
			return diffuse_coefficient_m_3;
		else if (material_type_3_arg == k_material_diffuse_only)
			return 1.0f;
	}
	return 0.0f;
}

float get_specular_power(int index)
{
	if (index == 0)
		if (material_type_0_arg == k_material_diffuse_plus_specular)
			return specular_power_m_0;
	if (index == 1)
		if (material_type_1_arg == k_material_diffuse_plus_specular)
			return specular_power_m_1;	
	if (index == 2)
		if (material_type_2_arg == k_material_diffuse_plus_specular)
			return specular_power_m_2;
	if (index == 3)
		if (material_type_3_arg == k_material_diffuse_plus_specular)
			return specular_power_m_3;
	
	return 0.0f;
}

float get_specular_power(float4 blend)
{
	float specular_power = 0.001;
	specular_power += blend.x * get_specular_power(0);
	specular_power += blend.y * get_specular_power(1);
	specular_power += blend.z * get_specular_power(2);
	specular_power += blend.w * get_specular_power(3);
	return specular_power;
}

float4 get_albedo_specular_tint_blend()
{
	float4 result = 0;
	if (material_type_0_arg == k_material_diffuse_plus_specular)
		result.x = albedo_specular_tint_blend_m_0;
	if (material_type_1_arg == k_material_diffuse_plus_specular)
		result.y = albedo_specular_tint_blend_m_1;
	if (material_type_2_arg == k_material_diffuse_plus_specular)
		result.z = albedo_specular_tint_blend_m_2;
	if (material_type_3_arg == k_material_diffuse_plus_specular)
		result.w = albedo_specular_tint_blend_m_3;
	
	return result;
}

float3 get_specular_tint(int index)
{
	if (index == 0)
		if (material_type_0_arg == k_material_diffuse_plus_specular)
			return specular_tint_m_0;
	if (index == 1)
		if (material_type_1_arg == k_material_diffuse_plus_specular)
			return specular_tint_m_1;
	if (index == 2)
		if (material_type_2_arg == k_material_diffuse_plus_specular)
			return specular_tint_m_2;
	if (index == 3)
		if (material_type_3_arg == k_material_diffuse_plus_specular)
			return specular_tint_m_3;
	return 0.0f;
}

float3 get_specular_tint(float4 blend, float3 albedo)
{
	float albedo_blend = 0;
	float3 specular_tint = 0;
	float albedo_specular_tint_blend = 0;
	float4 albedo_specular_tint_blend_shared = get_albedo_specular_tint_blend();
	
	
	specular_tint += get_specular_tint(0) * blend.x * (1.0 - albedo_specular_tint_blend_shared.x);
	specular_tint += get_specular_tint(1) * blend.y * (1.0 - albedo_specular_tint_blend_shared.y);
	specular_tint += get_specular_tint(2) * blend.z * (1.0 - albedo_specular_tint_blend_shared.z);
	specular_tint += get_specular_tint(3) * blend.w * (1.0 - albedo_specular_tint_blend_shared.w);
	
	albedo_specular_tint_blend += blend.x * albedo_specular_tint_blend_shared.x;
	albedo_specular_tint_blend += blend.y * albedo_specular_tint_blend_shared.y;
	albedo_specular_tint_blend += blend.z * albedo_specular_tint_blend_shared.z;
	albedo_specular_tint_blend += blend.w * albedo_specular_tint_blend_shared.w;
	
	return lerp(specular_tint, albedo, albedo_specular_tint_blend);
}

float get_fresnel_curve_steepness(int index)
{
	if (index == 0)
		if (material_type_0_arg == k_material_diffuse_plus_specular)
			return fresnel_curve_steepness_m_0;
	if (index == 1)
		if (material_type_1_arg == k_material_diffuse_plus_specular)
			return fresnel_curve_steepness_m_1;
	if (index == 2)
		if (material_type_2_arg == k_material_diffuse_plus_specular)
			return fresnel_curve_steepness_m_2;
	if (index == 3)
		if (material_type_3_arg == k_material_diffuse_plus_specular)
			return fresnel_curve_steepness_m_3;
	return 0.0f;
}

float get_fresnel_curve_steepness(float4 blend)
{
	float fresnel_curve_steepness = 0.005;
	
	fresnel_curve_steepness += blend.x * get_fresnel_curve_steepness(0);
	fresnel_curve_steepness += blend.y * get_fresnel_curve_steepness(1);
	fresnel_curve_steepness += blend.z * get_fresnel_curve_steepness(2);
	fresnel_curve_steepness += blend.w * get_fresnel_curve_steepness(3);
	return fresnel_curve_steepness;
}

float get_analytical_specular_contribution(int index)
{
	if (index == 0)
		if (material_type_0_arg == k_material_diffuse_plus_specular)
			return analytical_specular_contribution_m_0;
	if (index == 1)
		if (material_type_1_arg == k_material_diffuse_plus_specular)
			return analytical_specular_contribution_m_1;
	if (index == 2)
		if (material_type_2_arg == k_material_diffuse_plus_specular)
			return analytical_specular_contribution_m_2;
	if (index == 3)
		if (material_type_3_arg == k_material_diffuse_plus_specular)
			return analytical_specular_contribution_m_3;
	return 0.0f;
}

float get_analytical_specular_contribution(float4 blend)
{
    if (material_type_3_arg != k_material_diffuse_plus_specular && material_type_2_arg != k_material_diffuse_plus_specular && material_type_1_arg != k_material_diffuse_plus_specular && material_type_0_arg != k_material_diffuse_plus_specular)
        return 1.0f;
	
	float analytical_specular_contribution = 0;
	analytical_specular_contribution += get_specular_coefficient(0) * blend.x * get_analytical_specular_contribution(0);
	analytical_specular_contribution += get_specular_coefficient(1) * blend.y * get_analytical_specular_contribution(1);
	analytical_specular_contribution += get_specular_coefficient(2) * blend.z * get_analytical_specular_contribution(2);
	analytical_specular_contribution += get_specular_coefficient(3) * blend.w * get_analytical_specular_contribution(3);
	return analytical_specular_contribution;
}

float get_area_specular_contribution(float4 blend)
{
    if (material_type_3_arg != k_material_diffuse_plus_specular && material_type_2_arg != k_material_diffuse_plus_specular && material_type_1_arg != k_material_diffuse_plus_specular && material_type_0_arg != k_material_diffuse_plus_specular)
        return 1.0f;
	
	float area_specular_contribution = 0;
	area_specular_contribution += area_specular_contribution_m_0 * get_specular_coefficient(0) * blend.x;
	area_specular_contribution += area_specular_contribution_m_1 * get_specular_coefficient(1) * blend.y;
	area_specular_contribution += area_specular_contribution_m_2 * get_specular_coefficient(2) * blend.z;
	area_specular_contribution += area_specular_contribution_m_3 * get_specular_coefficient(3) * blend.w;
	return area_specular_contribution;
}

float get_diffuse_coefficient(float4 blend)
{
	if (material_type_3_arg != k_material_diffuse_plus_specular && material_type_2_arg != k_material_diffuse_plus_specular && material_type_1_arg != k_material_diffuse_plus_specular && material_type_0_arg != k_material_diffuse_plus_specular)
		return 1.0f;
	
	float diffuse_coefficient = 0;
	diffuse_coefficient += get_diffuse_coefficient(0) * blend.x;
	diffuse_coefficient += get_diffuse_coefficient(1) * blend.y;
	diffuse_coefficient += get_diffuse_coefficient(2) * blend.z;
	diffuse_coefficient += get_diffuse_coefficient(3) * blend.w;
	return diffuse_coefficient;
}

float get_environment_specular_contribution(float4 blend)
{
    float environment_contribution = 0;
    environment_contribution += environment_specular_contribution_m_0 * get_specular_coefficient(0) * blend.x;
    environment_contribution += environment_specular_contribution_m_1 * get_specular_coefficient(1) * blend.y;
    environment_contribution += environment_specular_contribution_m_2 * get_specular_coefficient(2) * blend.z;
    environment_contribution += environment_specular_contribution_m_3 * get_specular_coefficient(3) * blend.w;
    return environment_contribution;
}

void blend_specular_parameters(in float4 blend, out float specular_power, out float fresnel_steepness)
{
    specular_power = 0.001;
    specular_power += blend.x * get_specular_power(0);
    specular_power += blend.y * get_specular_power(1);
    specular_power += blend.z * get_specular_power(2);
    specular_power += blend.w * get_specular_power(3);
	
    fresnel_steepness = 0.005;
    fresnel_steepness += blend.x * get_fresnel_curve_steepness(0);
    fresnel_steepness += blend.y * get_fresnel_curve_steepness(1);
    fresnel_steepness += blend.z * get_fresnel_curve_steepness(2);
    fresnel_steepness += blend.w * get_fresnel_curve_steepness(3);
}

void terrain_self_illum(
float2 texcoord, 
sampler2D self_illum_map, 
float4 self_illum_map_xform,
sampler2D self_illum_detail_map,
float4 self_illum_detail_map_xform,
float3 self_illum_color,
float self_illum_intensity,
inout float3 illum_accum)
{
    float3 illum = tex2D(self_illum_map, apply_xform2d(texcoord, self_illum_map_xform)).rgb;
    float3 detail = tex2D(self_illum_detail_map, apply_xform2d(texcoord, self_illum_detail_map_xform)).rgb * DETAIL_MULTIPLIER;
	
    illum_accum += (illum * detail * self_illum_color) * self_illum_intensity;
}

float3 calc_self_illum_terrain(float2 texcoord)
{
    float3 illum_accum = 0.0f;
	
    if (material_has_illum(0))
    {
        terrain_self_illum(
			texcoord, 
			self_illum_map(0), 
			self_illum_map_xform(0), 
			self_illum_detail_map(0), 
			self_illum_detail_map_xform(0),
			self_illum_color(0),
			self_illum_intensity(0),
			illum_accum);
    }
    if (material_has_illum(1))
    {
        terrain_self_illum(
			texcoord,
			self_illum_map(1),
			self_illum_map_xform(1),
			self_illum_detail_map(1),
			self_illum_detail_map_xform(1),
			self_illum_color(1),
			self_illum_intensity(1),
			illum_accum);
    }
    if (material_has_illum(2))
    {
        terrain_self_illum(
			texcoord,
			self_illum_map(2),
			self_illum_map_xform(2),
			self_illum_detail_map(2),
			self_illum_detail_map_xform(2),
			self_illum_color(2),
			self_illum_intensity(2),
			illum_accum);
    }
    //if (material_has_illum(3))
    //{
    //    terrain_self_illum(
	//		texcoord,
	//		self_illum_map(3),
	//		self_illum_map_xform(3),
	//		self_illum_detail_map(3),
	//		self_illum_detail_map_xform(3),
	//		self_illum_color(3),
	//		self_illum_intensity(3),
	//		illum_accum);
    //}

    return illum_accum;
}

void calc_dynamic_lighting_terrain(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
    float v_dot_n = dot(common_data.surface_normal, common_data.view_dir);
    float sn_dot_n = dot(common_data.view_dir, common_data.surface_normal);
	
	float3 reflect_dir = (v_dot_n * common_data.surface_normal - common_data.view_dir) * 2 + common_data.view_dir;
	reflect_dir = normalize(reflect_dir);
	
	float l_dot_n = dot(common_data.light_direction, common_data.surface_normal);
	float l_dot_r = dot(common_data.light_direction, reflect_dir);
	l_dot_r = max(l_dot_r, 0);
	
	float4 blend = blend_type(common_data.texcoord);
	blend = normalize_additive_blend(blend);
	
    float blend_sum = blend_sum_active_materials(blend);
	float blend_normalized = blend_sum < 0 ? 1000 : 1 / (blend_sum + 0.001);
	
    float specular_exponent;
    float fresnel_curve_steepness;
    blend_specular_parameters(blend, specular_exponent, fresnel_curve_steepness);
	
    float specular_power = pow(l_dot_r, specular_exponent * blend_normalized);
    specular_power *= specular_exponent * blend_normalized + 1.0f;
    specular_power *= INV_2PI;
	
    fresnel_curve_steepness *= blend_normalized;
	
    float3 specular_tint = get_specular_tint(blend, common_data.albedo.rgb);
	
    float fresnel_base = 1.0f - saturate(sn_dot_n);
	float fresnel = pow(fresnel_base, fresnel_curve_steepness);
	float3 fresnel_color = lerp(specular_tint, 1.0f, fresnel);
    fresnel_color = common_data.albedo.a * fresnel_color;
	
    float3 specular;
    float3 diffuse;
	
    if (v_dot_n > 0 && l_dot_n > 0)
        specular = specular_power * common_data.light_intensity;
    else
        specular = 0;
	
	specular *= fresnel_color;
	specular *= get_analytical_specular_contribution(blend);
	
    diffuse = common_data.light_intensity * l_dot_n;
	diffuse *= common_data.albedo.rgb;
	diffuse *= get_diffuse_coefficient(blend);
	
	color = diffuse + specular;
}

float3 calc_lighting_terrain(SHADER_COMMON common_data, out float4 unknown_output)
{	
    float3 diffuse = common_data.diffuse_reflectance;
	
    float v_dot_n = dot(common_data.surface_normal, common_data.view_dir);
    float sn_dot_n = dot(normalize(common_data.view_dir), common_data.surface_normal);
	
    float3 reflect_dir = (v_dot_n * common_data.surface_normal - common_data.view_dir) * 2 + common_data.view_dir;
    reflect_dir = normalize(reflect_dir);
	
    float l_dot_n = dot(common_data.dominant_light_direction, common_data.surface_normal);
    float l_dot_r = dot(common_data.dominant_light_direction, reflect_dir);
    l_dot_r = max(l_dot_r, 0);
	
    float4 blend = blend_type(common_data.texcoord);
    blend = normalize_additive_blend(blend);
	
    float blend_sum = blend_sum_active_materials(blend);
    float blend_normalized = blend_sum < 0 ? 1000 : 1 / (blend_sum + 0.001);
	
    float specular_exponent;
    float fresnel_curve_steepness;
    blend_specular_parameters(blend, specular_exponent, fresnel_curve_steepness);
	
    float bn_spec_exp = specular_exponent * blend_normalized;
	
    float3 diffuse_accumulation = 0;
    float3 specular_accumulation = 0;
    calc_material_lambert_diffuse_ps(common_data.surface_normal, common_data.world_position, common_data.reflect_dir, bn_spec_exp, diffuse_accumulation, specular_accumulation);
    specular_accumulation *= bn_spec_exp;
	
    float specular_power = pow(l_dot_r, bn_spec_exp);
    specular_power *= bn_spec_exp + 1.0f;
    specular_power *= INV_2PI;
	
    fresnel_curve_steepness *= blend_normalized;
	
    float3 specular_tint = get_specular_tint(blend, common_data.albedo.rgb);
	
    float fresnel_base = 1.0f - saturate(sn_dot_n);
    float fresnel = pow(fresnel_base, fresnel_curve_steepness);
    float3 fresnel_color = lerp(specular_tint, 1.0f, fresnel);
    fresnel_color = common_data.albedo.a * fresnel_color;
	
    float3 specular;
	
    if (v_dot_n > 0 && l_dot_n > 0)
        specular = specular_power * common_data.dominant_light_intensity;
    else
        specular = 0;
	
    float3 lightprobe_color = lightmap_lightprobe_color(common_data.surface_normal, common_data.sh_0, common_data.sh_312);
    float3 area_specular = lightprobe_color * get_area_specular_contribution(blend);
	
    float3 env_color = 0;
    ENVIRONMENT_MAPPING_COMMON env_mapping_common_data;
    env_mapping_common_data.reflect_dir = common_data.reflect_dir;
    env_mapping_common_data.view_dir = common_data.view_dir;
    env_mapping_common_data.env_area_specular = lightprobe_color;
    env_mapping_common_data.specular_coefficient = get_environment_specular_contribution(blend);
    env_mapping_common_data.area_specular = 0.0;
    env_mapping_common_data.specular_exponent = 0.0;
	envmap_type(env_mapping_common_data, env_color, unknown_output);
	
    specular += specular_accumulation;
    specular *= get_analytical_specular_contribution(blend);
    specular += area_specular;
    specular += env_color;
    specular *= fresnel_color;
	
    diffuse += diffuse_accumulation;
    diffuse *= common_data.albedo.rgb;
    diffuse *= get_diffuse_coefficient(blend);
	
    if (material_has_specular(0) || material_has_specular(1) || material_has_specular(2) || material_has_specular(3))
        diffuse += specular;
	
    diffuse += calc_self_illum_terrain(common_data.texcoord);

	return diffuse;
}
#endif