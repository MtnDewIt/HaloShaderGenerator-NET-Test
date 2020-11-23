#ifndef _HALOGRAM_STATIC_PER_VERTEX_COLOR_HLSLI
#define _HALOGRAM_STATIC_PER_VERTEX_COLOR_HLSLI

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
#include "..\material_models\lambert.hlsli"

PS_OUTPUT_DEFAULT halogram_entry_static_per_vertex_color(VS_OUTPUT_PER_VERTEX_COLOR input)
{
    float4 albedo = float4(0, 0, 0, 1);
    
    float3 normal;
    
    float3 camera_dir = normalize(input.camera_dir);
    
    float view_tangent = dot(input.tangent, camera_dir);
    float view_binormal = dot(input.binormal, camera_dir);
    
    float2 texcoord = input.texcoord;
    float3 world_position = Camera_Position_PS - input.camera_dir;
    
#ifdef actually_calc_albedo_visible
    if (actually_calc_albedo)
    {
        normal = input.normal;
		albedo = calc_albedo_ps(texcoord, input.position.xy, input.normal.xyz, input.camera_dir);
    }
    else
    {
        float2 position = input.position.xy;
        position += 0.5;
        float2 inv_texture_size = (1.0 / texture_size);
        float2 frag_texcoord = position * inv_texture_size;
        float4 normal_texture_sample = tex2D(normal_texture, frag_texcoord);
        normal = normal_import(normal_texture_sample.xyz);
        float4 albedo_texture_sample = tex2D(albedo_texture, frag_texcoord);
		albedo = albedo_texture_sample;
    }
#else
    float2 position = input.position.xy;
    position += 0.5;
    float2 inv_texture_size = (1.0 / texture_size);
    float2 frag_texcoord = position * inv_texture_size;
    float4 normal_texture_sample = tex2D(normal_texture, frag_texcoord);
    normal = normal_import(normal_texture_sample.xyz);
    float4 albedo_texture_sample = tex2D(albedo_texture, frag_texcoord);
    albedo = albedo_texture_sample;
#endif
    
    normal = normalize(input.normal);
    
    float view_normal = dot(camera_dir, normal);
    
    float4 color = albedo;
    
    float3 self_illumination_color = float3(0, 0, 0);
	calc_self_illumination_ps(input.position.xy, texcoord.xy, albedo.rgb, 0, input.camera_dir.xyz, view_normal, view_tangent, view_binormal, self_illumination_color);
		
    float3 diffuse_accumulation = 0;
    float3 specular_accumulation = 0;
    calc_material_lambert_diffuse_ps(normal, world_position, 0, 0, diffuse_accumulation, specular_accumulation);
		
    float3 lambert_diffuse = input.vertex_color + diffuse_accumulation;
    
    if (self_illumination_arg == k_self_illumination_from_diffuse)
    {
        color.rgb = self_illumination_color;
    }
    else
    {
        color.rgb = lambert_diffuse * color.rgb;
        color.rgb += self_illumination_color;
    }

    color.rgb = color.rgb * input.extinction_factor.rgb;
    
    // calculate_alpha_blending
    if (blend_type_arg == k_blend_mode_additive)
    {
        color.a = 0.0;
    }
    else if (blend_type_arg == k_blend_mode_alpha_blend)
    {
        color.a = albedo.a;
    }
    else
    {
        color.a = 1.0f;
    }
	
    if (blend_type_arg != k_blend_mode_additive)
    {
        color.rgb += input.sky_radiance.rgb;
    }

    if (blend_type_arg == k_blend_mode_double_multiply)
        color.rgb *= 2;

    color.rgb = expose_color(color.rgb);

    PS_OUTPUT_DEFAULT output = export_color(color);
    
    return output;
}

#endif