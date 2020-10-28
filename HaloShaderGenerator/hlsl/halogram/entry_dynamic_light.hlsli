#ifndef _HALOGRAM_DYNAMIC_LIGHT_HLSLI
#define _HALOGRAM_DYNAMIC_LIGHT_HLSLI

#include "..\helpers\halogram_helper.hlsli"

#include "..\helpers\input_output.hlsli"
#include "..\methods\albedo.hlsli"
#include "..\methods\warp.hlsli"
#include "..\methods\self_illumination.hlsli"

#if (self_illumination_arg == k_self_illumination_simple && blend_type_arg != k_blend_mode_opaque) || (self_illumination_arg != k_self_illumination_off && self_illumination_arg != k_self_illumination_simple)
uniform bool actually_calc_albedo : register(b12);
#define actually_calc_albedo_visible 1
#endif

#include "..\methods\overlay.hlsli"
#include "..\methods\edge_fade.hlsli"

#include "..\helpers\shadows.hlsli"
#include "..\helpers\lighting.hlsli"
uniform sampler2D dynamic_light_gel_texture;

PS_OUTPUT_DEFAULT calculate_dynamic_light(
float2 position,
float2 texcoord,
float3 camera_dir,
float3 tangent,
float3 binormal,
float3 normal,
int light_index,
float depth_scale,
float depth_offset,
float2 shadowmap_texcoord,
bool is_cinematic)
{
    float3 view_dir = normalize(camera_dir);
    texcoord = calc_warp(texcoord, view_dir, tangent, binormal, normal);
	
    float3 world_position = Camera_Position_PS - camera_dir;

    SimpleLight light = get_simple_light(light_index);
	
    float3 v_to_light = light.position.xyz - world_position;
    float light_distance_squared = dot(v_to_light, v_to_light);
    v_to_light = normalize(v_to_light);
	
    float3 L = normalize(v_to_light); // normalized surface to light direction

    float distance_attenuation, cone_attenuation;
    get_simple_light_parameters(light, L, light_distance_squared, distance_attenuation, cone_attenuation);

    float3 intensity = cone_attenuation * distance_attenuation;
	
    float2 shadowmap_texcoord_depth_adjusted = shadowmap_texcoord * (1.0 / depth_scale);
    float2 gel_texcoord = apply_xform2d(shadowmap_texcoord_depth_adjusted, p_dynamic_light_gel_xform);
    float4 gel_sample = tex2D(dynamic_light_gel_texture, gel_texcoord);
	
    float3 light_intensity = intensity * light.color.rgb * gel_sample.rgb;
	
    float4 albedo = float4(0, 0, 0, 1);
    float3 surface_normal;
    
#ifdef actually_calc_albedo_visible
    if (actually_calc_albedo)
    {
        surface_normal = normal;
		albedo = calc_albedo_ps(texcoord, position.xy, normal.xyz, camera_dir);
    }
    else
    {
        float2 frag_position = position.xy;
        frag_position += 0.5;
        float2 inv_texture_size = (1.0 / texture_size);
        float2 frag_texcoord = frag_position * inv_texture_size;
        float4 normal_texture_sample = tex2D(normal_texture, frag_texcoord);
        surface_normal = normal_import(normal_texture_sample.xyz);
        float4 albedo_texture_sample = tex2D(albedo_texture, frag_texcoord);
		albedo = albedo_texture_sample;
    }
#else
    float2 frag_position = position.xy;
    frag_position += 0.5;
    float2 inv_texture_size = (1.0 / texture_size);
    float2 frag_texcoord = frag_position * inv_texture_size;
    float4 normal_texture_sample = tex2D(normal_texture, frag_texcoord);
    surface_normal = normal_import(normal_texture_sample.xyz);
    float4 albedo_texture_sample = tex2D(albedo_texture, frag_texcoord);
    albedo = albedo_texture_sample;
#endif
    
    float3 reflect_dir = 2 * dot(view_dir, surface_normal) * surface_normal - camera_dir;
	
	
    float3 color;
	
    SHADER_DYNAMIC_LIGHT_COMMON common_data;

    common_data.albedo = albedo;
    common_data.texcoord = texcoord;
    common_data.surface_normal = surface_normal;
    common_data.normal = normal;
    common_data.view_dir = view_dir;
    common_data.reflect_dir = reflect_dir;
    common_data.light_intensity = light_intensity;
    common_data.light_direction = L;
    common_data.specular_mask = 1.0f;
    
    
    float l_dot_n = dot(common_data.light_direction, common_data.surface_normal);
    color = common_data.light_intensity * l_dot_n * common_data.albedo.rgb; // lambertian diffuse
	
    float shadow_coefficient;
    if (dynamic_light_shadowing)
    {
        if (is_cinematic)
            shadow_coefficient = shadows_percentage_closer_filtering_custom_4x4(shadowmap_texcoord_depth_adjusted, shadowmap_texture_size, depth_scale, depth_offset);
        else
            shadow_coefficient = shadows_percentage_closer_filtering_3x3(shadowmap_texcoord_depth_adjusted, shadowmap_texture_size, depth_scale, depth_offset);
		
        if (dot(color.rgb, color.rgb) <= 0)
            shadow_coefficient = 1.0;
    }
    else
    {
        shadow_coefficient = 1.0;
    }
	
    float4 result;
    if (blend_type_arg == k_blend_mode_additive)
    {
        result.a = 0.0;
    }
    else if (blend_type_arg == k_blend_mode_alpha_blend)
    {
        result.a = albedo.a;
    }
    else
    {
        result.a = 1.0f;
    }
	
    result.rgb = expose_color(color.rgb);
    result.rgb *= shadow_coefficient;
	
    return export_color(result);
}

PS_OUTPUT_DEFAULT halogram_entry_dynamic_light(VS_OUTPUT_DYNAMIC_LIGHT input)
{
    return calculate_dynamic_light(input.position.xy, input.texcoord, input.camera_dir, input.tangent, input.binormal, input.normal, 0, input.shadowmap_texcoord.w, input.shadowmap_texcoord.z, input.shadowmap_texcoord.xy, false);
}

PS_OUTPUT_DEFAULT halogram_entry_dynamic_light_cinematic(VS_OUTPUT_DYNAMIC_LIGHT input)
{
    return calculate_dynamic_light(input.position.xy, input.texcoord, input.camera_dir, input.tangent, input.binormal, input.normal, 0, input.shadowmap_texcoord.w, input.shadowmap_texcoord.z, input.shadowmap_texcoord.xy, true);
}

#endif