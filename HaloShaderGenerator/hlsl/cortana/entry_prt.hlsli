#ifndef _CORTANA_TEMPLATE_PRT_HLSLI
#define _CORTANA_TEMPLATE_PRT_HLSLI

#include "..\methods\albedo.hlsli"
#include "..\methods\bump_mapping.hlsli"
#include "..\registers\global_parameters.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"

#include "..\cortana_lighting\cortana_lighting.hlsli"
#include "..\methods\material_model.hlsli"

#include "..\methods\blend_mode.hlsli"
#include "..\methods\misc.hlsli"
#include "..\methods\alpha_test.hlsli"

PS_OUTPUT_DEFAULT entry_static_sh_prt(
float2 position,
float2 texcoord,
float3 camera_dir,
float3 normal,
float3 tangent,
float3 binormal,
float3 sky_radiance,
float3 extinction_factor,
float4 prt)
{
    SHADER_COMMON common_data;
	{
        common_data.view_dir = camera_dir;
        common_data.n_view_dir = normalize(camera_dir);
        common_data.fragcoord = position.xy;
        common_data.tangent = tangent;
        common_data.binormal = binormal;
        common_data.normal = normal;
        common_data.texcoord = texcoord;
        common_data.alpha = calc_alpha_test_ps(common_data.texcoord, 1.0f);

        if (true) // actually_calc_albedo
        {
            float2 calc_albedo_texcoord = common_data.texcoord;
			
            common_data.surface_normal = calc_bumpmap_ps(tangent, binormal, normal, calc_albedo_texcoord);
            //common_data.albedo = calc_albedo_ps(calc_albedo_texcoord, position.xy, common_data.surface_normal, common_data.view_dir);
            
            float3 view_frame = mul(float3x3(normalize(tangent), normalize(binormal), normalize(normal)), common_data.n_view_dir);
            common_data.albedo = calc_albedo_cortana_ps(calc_albedo_texcoord, position, common_data.surface_normal, view_frame);
        }
        else
        {
            position += 0.5;
            float2 inv_texture_size = (1.0 / texture_size);
            float2 texcoord = position * inv_texture_size;
            float4 normal_texture_sample = tex2D(normal_texture, texcoord);
            common_data.surface_normal = normal_import(normal_texture_sample.xyz);
            float4 albedo_texture_sample = tex2D(albedo_texture, texcoord);
            common_data.albedo = albedo_texture_sample;
        }
		
        common_data.surface_normal = normalize(common_data.surface_normal);
		
        common_data.specular_mask = 1.0;
		
        float v_dot_n = dot(common_data.n_view_dir, common_data.surface_normal);
        common_data.half_dir = v_dot_n * common_data.surface_normal - common_data.n_view_dir;
        common_data.reflect_dir = common_data.half_dir * 2 + common_data.n_view_dir;
        common_data.world_position = Camera_Position_PS - common_data.view_dir;
		
        get_current_sh_coefficients_quadratic(common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, common_data.dominant_light_direction, common_data.dominant_light_intensity);
		
        common_data.sh_0_no_dominant_light = common_data.sh_0;
        common_data.sh_312_no_dominant_light[0] = common_data.sh_312[0];
        common_data.sh_312_no_dominant_light[1] = common_data.sh_312[1];
        common_data.sh_312_no_dominant_light[2] = common_data.sh_312[2];
		
        common_data.diffuse_reflectance = diffuse_reflectance(common_data.surface_normal);
		
        common_data.precomputed_radiance_transfer = prt;
        common_data.per_vertex_color = 0.0f;
        common_data.no_dynamic_lights = no_dynamic_lights;
		
        if (!calc_atmosphere_no_material && !calc_material)
        {
            common_data.sky_radiance = 0.0;
            common_data.extinction_factor = 1.0;
        }
        else
        {
            common_data.sky_radiance = sky_radiance;
            common_data.extinction_factor = extinction_factor;
        }
    }

    float4 color;
    float4 ssr_out = 0;
    color.rgb = calc_cortana_cook_torrance_lighting(common_data, ssr_out);
    color.a = 1.0f - common_data.albedo.a;

    PS_OUTPUT_DEFAULT output = export_color(color);
    if (calc_env_output)
    {
        output.unknown = ssr_out;
    }
    return output;
}


PS_OUTPUT_DEFAULT cortana_entry_static_sh(VS_OUTPUT_STATIC_SH input)
{
    return entry_static_sh_prt(input.position.xy, input.texcoord.xy, input.camera_dir.xyz, input.normal, input.tangent, input.binormal, input.sky_radiance, input.extinction_factor, 1.0);
}

PS_OUTPUT_DEFAULT cortana_entry_static_prt(VS_OUTPUT_CORTANA_PRT input)
{
    return entry_static_sh_prt(input.position.xy, input.texcoord.xy, input.camera_dir.xyz, input.normal, input.tangent, input.binormal, input.sky_radiance.xyz, input.extinction_factor, input.prt_radiance_vector);
}

#endif