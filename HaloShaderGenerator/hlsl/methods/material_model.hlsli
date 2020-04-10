#ifndef _MATERIAL_MODEL_HLSLItempx
#define _MATERIAL_MODEL_HLSLI

#include "../helpers/math.hlsli"
#include "../registers/shader.hlsli"
#include "../helpers/lighting.hlsli"
#include "../helpers/sh.hlsli"

#define MATERIAL_TYPE_ARGS float3 diffuse, float3 normal, float3 eye_world, float2 texcoord, float3 extinction_factor, float3 sky_radiance, float3 camera_dir, float3 prt_result
#define MATERIAL_TYPE_ARGNAMES diffuse, normal, eye_world, texcoord, extinction_factor, sky_radiance, camera_dir, prt_result

float3 material_type_diffuse_only(MATERIAL_TYPE_ARGS)
{
	float3 diffuse_ref = diffuse_reflectance(normal);
    float3 lighting = float3(0, 0, 0);

    if (no_dynamic_lights)
    {
        lighting = diffuse_ref.xyz * prt_result;
    }
    else
    {
        float3 vertex_world_position = Camera_Position_PS - camera_dir;
        float3 accumulation = float3(0, 0, 0);

        if (simple_light_count > 0)
        {
            accumulation = calculate_simple_light(get_simple_light(0), accumulation, normal, vertex_world_position);
            if (simple_light_count > 1)
            {
                accumulation = calculate_simple_light(get_simple_light(1), accumulation, normal, vertex_world_position);
                if (simple_light_count > 2)
                {
                    accumulation = calculate_simple_light(get_simple_light(2), accumulation, normal, vertex_world_position);
                    if (simple_light_count > 3)
                    {
                        accumulation = calculate_simple_light(get_simple_light(3), accumulation, normal, vertex_world_position);
                        if (simple_light_count > 4)
                        {
                            accumulation = calculate_simple_light(get_simple_light(4), accumulation, normal, vertex_world_position);
                            if (simple_light_count > 5)
                            {
                                accumulation = calculate_simple_light(get_simple_light(5), accumulation, normal, vertex_world_position);
                                if (simple_light_count > 6)
                                {
                                    accumulation = calculate_simple_light(get_simple_light(6), accumulation, normal, vertex_world_position);
                                    if (simple_light_count > 7)
                                    {
                                        accumulation = calculate_simple_light(get_simple_light(7), accumulation, normal, vertex_world_position);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        lighting = diffuse_ref.xyz * prt_result + accumulation;
    }
    return diffuse * lighting * extinction_factor + sky_radiance;
}

//TODO: This is likely a structure of floats
float3 sample_material_texture(float2 texcoord)
{
    float3 sc_ab_r = float3(specular_coefficient, albedo_blend, roughness);
    if(use_material_texture)
    {
        float4 material_texture_sample = tex2D(material_texture, apply_xform2d(texcoord, material_texture_xform));

        //TODO: What is in the Z channel???
        return sc_ab_r * material_texture_sample.xyw;
    }
    else
    {
        return sc_ab_r;
    }
}

float3 material_type_cook_torrance(MATERIAL_TYPE_ARGS)
{
    float3 color = float3(1, 0, 0);

    float3 material_info = sample_material_texture(texcoord);

    float3 specular_color = fresnel_color;
    if (albedo_blend_with_specular_tint.x)
    {
        specular_color = lerp(fresnel_color, diffuse, material_info.y);
    }

    color = specular_color;

    float n_dot_l = dot(normal, -normalize(k_ps_dominant_light_direction.xyz));
    float n_dot_v = dot(normal, eye_world);

    color = min(n_dot_l, n_dot_v).xxx;
    color = n_dot_l.xxx;















    float4 garbage = float4(0, 0, 0, 0);

    if (use_material_texture)
    {
        garbage += tex2D(material_texture, float2(0, 0));
    }
    if (order3_area_specular)
    {
        garbage += tex2D(g_sampler_dd0236, float2(0, 0));
        garbage += tex2D(g_sampler_c78d78, float2(0, 0));
    }
    if (no_dynamic_lights)
    {
        garbage += tex2D(g_sampler_cc0236, float2(0, 0));
    }
    garbage += diffuse_coefficient.x;
    garbage += specular_coefficient.x;
    garbage += area_specular_contribution.x;
    garbage += analytical_specular_contribution.x;
    garbage += material_texture_xform.x;
    garbage += fresnel_color.x;
    garbage += fresnel_power.x;
    garbage += roughness.x;
    garbage += albedo_blend.x;
    garbage += specular_tint.x;
    garbage += albedo_blend_with_specular_tint.x;
    garbage += rim_fresnel_coefficient.x;
    garbage += rim_fresnel_color.x;
    garbage += rim_fresnel_power.x;
    garbage += rim_fresnel_albedo_blend.x;
    garbage += analytical_anti_shadow_control;
    garbage += k_ps_dominant_light_direction.x;
    garbage += k_ps_dominant_light_intensity.x;
    garbage += tex2D(albedo_texture, float2(0, 0));
    garbage += tex2D(normal_texture, float2(0, 0));
    return color + float3(clamp(garbage.x, 0, 0.0001), 0, 0);
    return material_type_diffuse_only(MATERIAL_TYPE_ARGNAMES);
}

float3 material_type_two_lobe_phong(MATERIAL_TYPE_ARGS)
{
    return material_type_diffuse_only(MATERIAL_TYPE_ARGNAMES);
}

float3 material_type_foliage(MATERIAL_TYPE_ARGS)
{
    return material_type_diffuse_only(MATERIAL_TYPE_ARGNAMES);
}

float3 material_type_none(MATERIAL_TYPE_ARGS)
{
    return extinction_factor;
}

float3 material_type_glass(MATERIAL_TYPE_ARGS)
{
    return material_type_diffuse_only(MATERIAL_TYPE_ARGNAMES);
}

float3 material_type_organism(MATERIAL_TYPE_ARGS)
{
    return material_type_diffuse_only(MATERIAL_TYPE_ARGNAMES);
}

float3 material_type_single_lobe_phong(MATERIAL_TYPE_ARGS)
{
    return material_type_diffuse_only(MATERIAL_TYPE_ARGNAMES);
}

float3 material_type_car_paint(MATERIAL_TYPE_ARGS)
{
    return material_type_diffuse_only(MATERIAL_TYPE_ARGNAMES);
}

float3 material_type_hair(MATERIAL_TYPE_ARGS)
{
    return float3(0, 1, 0);
}

#ifndef material_type
#define material_type material_type_none
#endif

#endif
