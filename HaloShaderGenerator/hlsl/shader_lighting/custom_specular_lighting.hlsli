
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\registers\global_parameters.hlsli"
#include "..\material_models\custom_specular.hlsli"
#include "..\methods\self_illumination.hlsli"
#include "..\methods\environment_mapping.hlsli"

void get_analytic_custom_specular_parameters(
    in float3 n_view_dir,
    in float3 normal_dir,
    in float3 light_direction,
    in float3 light_intensity,
    in float2 texcoord,
	out float3 glance_falloff,
	out float3 analytic_spec
)
{    
    glance_falloff = tex1D(glancing_falloff, dot(normal_dir, n_view_dir)).rgb;
    
    float lobe_x = saturate(dot(normalize(n_view_dir + light_direction), normal_dir));
    float lobe_y = tex2D(material_map, apply_xform2d(texcoord, material_map_xform)).y;
    float4 spec_lobe = tex2D(specular_lobe, float2(lobe_x, lobe_y));

    analytic_spec = light_intensity * glance_falloff * spec_lobe.rgb * spec_lobe.rgb;
}

void calc_dynamic_lighting_custom_specular_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
    float3 specular_contribution = common_data.specular_mask * specular_coefficient * analytical_specular_contribution;
	
    color = common_data.light_intensity * dot(common_data.light_direction, common_data.surface_normal) * common_data.albedo.rgb * diffuse_coefficient;
    
    if (dot(specular_contribution, specular_contribution) > 0.0001)
    {
        float3 glance_falloff;
        float3 analytic_spec;
        get_analytic_custom_specular_parameters(
            common_data.view_dir,
            common_data.surface_normal,
            common_data.light_direction,
            common_data.light_intensity,
            common_data.texcoord,
            glance_falloff,
            analytic_spec);
	
        color += analytic_spec * specular_contribution;
    }
}

float3 calc_lighting_custom_specular_ps(SHADER_COMMON common_data, out float4 unknown_output)
{    
    float3 glance_falloff;
    float3 analytic_spec;
    get_analytic_custom_specular_parameters(
        common_data.n_view_dir, 
        common_data.surface_normal, 
        common_data.dominant_light_direction, 
        common_data.dominant_light_intensity, 
        common_data.texcoord,
        glance_falloff,
        analytic_spec);

    float3 specular = common_data.specular_mask * (analytic_spec * specular_coefficient);
    specular *= common_data.precomputed_radiance_transfer.z;
    
    float3 env_area_specular = common_data.precomputed_radiance_transfer.z;
    float3 env_specular_contribution = glance_falloff * common_data.specular_mask * environment_map_specular_contribution * specular_coefficient;
    
    float3 diffuse = common_data.precomputed_radiance_transfer.x * common_data.diffuse_reflectance;
    diffuse = diffuse * diffuse_coefficient;
	
    diffuse *= common_data.albedo.rgb;
	
    env_area_specular = max(env_area_specular, 0.001);
    ENVIRONMENT_MAPPING_COMMON env_mapping_common_data;
	
    env_mapping_common_data.reflect_dir = common_data.reflect_dir;
    env_mapping_common_data.view_dir = common_data.view_dir;
    env_mapping_common_data.env_area_specular = env_area_specular;
    env_mapping_common_data.specular_coefficient = env_specular_contribution;
    env_mapping_common_data.area_specular = 0.0f;
    env_mapping_common_data.specular_exponent = 0.0f;
	
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
