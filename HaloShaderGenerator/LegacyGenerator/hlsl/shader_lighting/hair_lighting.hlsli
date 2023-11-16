
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\registers\global_parameters.hlsli"
#include "..\material_models\hair.hlsli"
#include "..\material_models\lambert.hlsli"
#include "..\methods\self_illumination.hlsli"
#include "..\methods\environment_mapping.hlsli"

void get_analytic_hair_parameters(
	float3 binormal,
	float3 reflect_half,
	float3 light_intensity,
	float specular_power,
	out float3 analytic_spec)
{
    float bi_dot_h = dot(binormal, reflect_half);
    analytic_spec = pow(sqrt(1.0f - bi_dot_h * bi_dot_h), specular_power) * light_intensity;
}

float3 calculate_hair_phong_simple(float3 reflect, float4 sh_312[3])
{
    float3 phong;
    phong.x = dot(reflect, sh_312[0].xyz);
    phong.y = dot(reflect, sh_312[1].xyz);
    phong.z = dot(reflect, sh_312[2].xyz);
    return phong * -0.3805236f;
}

void calc_dynamic_lighting_hair_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
    float4 spec_map = tex2D(specular_map, common_data.texcoord);
    float spec_power = spec_map.a * specular_power;
    
    float3 diffuse = saturate(dot(common_data.light_direction, common_data.surface_normal)) * common_data.light_intensity;
    diffuse *= diffuse_coefficient;
    diffuse *= diffuse_tint;
    diffuse *= common_data.albedo.rgb;

    float spec_shift = tex2D(specular_shift_map, common_data.texcoord).r - 0.5f;
    float spec_noise = tex2D(specular_noise_map, common_data.texcoord).r;
    
    float3 shifted_binormal = common_data.binormal + common_data.normal * spec_shift;
    float3 downshifted_binormal_noisy = common_data.binormal - common_data.normal * spec_shift * spec_noise;
    shifted_binormal = normalize(shifted_binormal);
    downshifted_binormal_noisy = normalize(downshifted_binormal_noisy);
    
    float3 analytic_spec;
    get_analytic_hair_parameters(
		shifted_binormal,
		normalize(common_data.view_dir + common_data.light_direction),
		common_data.light_direction,
		spec_power,
		analytic_spec);
    
    float3 analytic_spec_noisy;
    get_analytic_hair_parameters(
		downshifted_binormal_noisy,
		normalize(common_data.view_dir + common_data.light_direction),
		common_data.light_direction,
		spec_power * spec_noise,
		analytic_spec_noisy);

    float3 final_analytic_spec = (analytic_spec + analytic_spec_noisy * spec_noise) * analytical_specular_coefficient;
    
    float3 specular = final_analytic_spec * specular_tint * spec_map.rgb;
    
    color = diffuse + specular;
    color *= final_tint;
}

float3 calc_lighting_hair_ps(SHADER_COMMON common_data, out float4 unknown_output)
{	
    float spec_shift = tex2D(specular_shift_map, common_data.texcoord).r - 0.5f;
    float spec_noise = tex2D(specular_noise_map, common_data.texcoord).r;
    
    float3 shifted_binormal = common_data.binormal + common_data.normal * spec_shift;
    float3 downshifted_binormal_noisy = common_data.binormal - common_data.normal * spec_shift * spec_noise;
    shifted_binormal = normalize(shifted_binormal);
    downshifted_binormal_noisy = normalize(downshifted_binormal_noisy);
    
    float3 nv_cross_bi = cross(common_data.n_view_dir, common_data.binormal);
	
    float3 shifted_normal = cross(shifted_binormal, nv_cross_bi);
    float3 shifted_normal_noisy = cross(downshifted_binormal_noisy, nv_cross_bi);
    shifted_normal = normalize(shifted_normal);
    shifted_normal_noisy = normalize(shifted_normal_noisy);
    
    float3 shifted_reflect = reflect(-common_data.n_view_dir, shifted_normal);
    float3 shifted_reflect_noisy = reflect(-common_data.n_view_dir, shifted_normal_noisy);
    
    float4 spec_map = tex2D(specular_map, common_data.texcoord);
    float spec_power = spec_map.a * specular_power;
    float light_spec_power = sqrt(spec_power);
    
    float3 light_diffuse_accum = 0;
    float3 light_spec_accum = 0;
    float3 light_spec_noisy_accum = 0;
    if (!common_data.no_dynamic_lights)
    {
        calc_material_lambert_diffuse_ps(
			shifted_normal,
			common_data.world_position,
			shifted_reflect,
			light_spec_power,
			light_diffuse_accum,
			light_spec_accum);
		
        calc_material_lambert_diffuse_ps(
			shifted_normal_noisy,
			common_data.world_position,
			shifted_reflect_noisy,
			light_spec_power,
			light_diffuse_accum,
			light_spec_noisy_accum);
    }
    
    float3 diffuse = (light_diffuse_accum + common_data.diffuse_reflectance);
    diffuse *= diffuse_coefficient;
    diffuse *= diffuse_tint;
    diffuse *= common_data.precomputed_radiance_transfer.x;
    diffuse *= final_tint;
    diffuse *= common_data.albedo.rgb;

    float3 analytic_spec;
    get_analytic_hair_parameters(
			shifted_binormal,
			normalize(common_data.n_view_dir + common_data.dominant_light_direction),
			common_data.dominant_light_intensity,
			spec_power,
			analytic_spec);
    analytic_spec += light_spec_accum;
    
    float3 analytic_spec_noisy;
    get_analytic_hair_parameters(
			downshifted_binormal_noisy,
			normalize(common_data.n_view_dir + common_data.dominant_light_direction),
			common_data.dominant_light_intensity,
			spec_power * spec_noise,
			analytic_spec_noisy);
    analytic_spec_noisy += light_spec_noisy_accum;
    analytic_spec_noisy *= spec_noise;

    float3 final_analytic_spec = (analytic_spec + analytic_spec_noisy) * analytical_specular_coefficient;

    float3 area_spec = calculate_hair_phong_simple(shifted_reflect_noisy, common_data.sh_312);
    area_spec = max(area_spec, 0.0f);
    area_spec *= area_specular_coefficient;

    float3 specular = (final_analytic_spec + area_spec) * specular_tint * spec_map.rgb;
    specular *= common_data.precomputed_radiance_transfer.z;
    specular *= final_tint;
    
    float3 env_area_specular = common_data.precomputed_radiance_transfer.z;
    float3 env_specular_contribution = environment_map_tint * environment_map_coefficient * spec_map.rgb;    
	
    env_area_specular = max(env_area_specular, 0.001);
    ENVIRONMENT_MAPPING_COMMON env_mapping_common_data;
	
    env_mapping_common_data.reflect_dir = common_data.reflect_dir;
    env_mapping_common_data.view_dir = common_data.view_dir;
    env_mapping_common_data.env_area_specular = env_area_specular;
    env_mapping_common_data.specular_coefficient = env_specular_contribution;
    env_mapping_common_data.area_specular = 0.0f;
    env_mapping_common_data.specular_exponent = 1.0f;
	
    float3 env_color = 0;
	envmap_type(env_mapping_common_data, env_color, unknown_output);
    
    float view_tangent = dot(common_data.tangent, common_data.n_view_dir);
    float view_binormal = dot(common_data.binormal, common_data.n_view_dir);
    float3 self_illum = 0;
	calc_self_illumination_ps(0, common_data.texcoord.xy, common_data.albedo.rgb, 0, common_data.view_dir, dot(common_data.n_view_dir, common_data.surface_normal), view_tangent, view_binormal, self_illum);
	
    float3 color = diffuse + specular;
    color += self_illum;
    color += env_color;
    
    return color;
}
