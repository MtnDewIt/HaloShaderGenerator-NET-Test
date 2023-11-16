#ifndef _CORTANA_TEMPLATE_ACTIVE_CAMO_HLSLI
#define _CORTANA_TEMPLATE_ACTIVE_CAMO_HLSLI

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

uniform float warp_amount;

uniform sampler2D fade_gradient_map;
uniform float4 fade_gradient_map_xform;
uniform float fade_gradient_scale;
uniform float noise_amount;
uniform float fade_offset;
uniform float warp_fade_offset;

PS_OUTPUT_DEFAULT cortana_entry_active_camo(VS_OUTPUT_CORTANA_PRT input)
{
    SHADER_COMMON common_data;
	{
        common_data.view_dir = input.camera_dir;
        common_data.n_view_dir = normalize(input.camera_dir);
        common_data.fragcoord = input.position.xy;
        common_data.tangent = input.tangent;
        common_data.binormal = input.binormal;
        common_data.normal = input.normal;
        common_data.texcoord = input.texcoord.xy;
        common_data.alpha = calc_alpha_test_ps(common_data.texcoord, 1.0f);

        if (true) // actually_calc_albedo
        {
            float2 calc_albedo_texcoord = common_data.texcoord;
			
            common_data.surface_normal = calc_bumpmap_ps(input.tangent, input.binormal, input.normal.xyz, calc_albedo_texcoord);
            //common_data.albedo = calc_albedo_ps(calc_albedo_texcoord, input.position.xy, common_data.surface_normal, common_data.view_dir);
            
            float3 view_frame = mul(float3x3(normalize(input.tangent), normalize(input.binormal), normalize(input.normal)), common_data.n_view_dir);
            common_data.albedo = calc_albedo_cortana_ps(calc_albedo_texcoord, input.position.xy, common_data.surface_normal, view_frame);
        }
        else
        {
            float2 inv_texture_size = (1.0 / texture_size);
            float2 texcoord = input.position.xy * inv_texture_size;
            float4 normal_texture_sample = tex2D(normal_texture, texcoord);
            common_data.surface_normal = normal_import(normal_texture_sample.xyz);
            float4 albedo_texture_sample = tex2D(albedo_texture, texcoord);
            common_data.albedo = albedo_texture_sample;
        } 
        
        //common_data.surface_normal = normalize(common_data.surface_normal);
        float d_normal = dot(common_data.surface_normal, common_data.surface_normal);
        common_data.surface_normal /= sqrt(d_normal);
		
        common_data.specular_mask = 1.0;
		
        float v_dot_n = dot(common_data.n_view_dir, common_data.surface_normal);
        common_data.half_dir = v_dot_n * common_data.surface_normal - common_data.n_view_dir;
        common_data.reflect_dir = common_data.half_dir * 2 + common_data.n_view_dir;
        common_data.world_position = Camera_Position_PS - common_data.view_dir;
		
        get_current_sh_coefficients_quadratic(common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, common_data.dominant_light_direction, common_data.dominant_light_intensity);
        common_data.dominant_light_intensity *= pow(d_normal, 8.0f);
		
        common_data.sh_0_no_dominant_light = common_data.sh_0;
        common_data.sh_312_no_dominant_light[0] = common_data.sh_312[0];
        common_data.sh_312_no_dominant_light[1] = common_data.sh_312[1];
        common_data.sh_312_no_dominant_light[2] = common_data.sh_312[2];
		
        common_data.diffuse_reflectance = diffuse_reflectance(common_data.surface_normal);
		
        common_data.precomputed_radiance_transfer = input.prt_radiance_vector;
        common_data.per_vertex_color = 0.0f;
        common_data.no_dynamic_lights = no_dynamic_lights;
		
        if (!calc_atmosphere_no_material && !calc_material)
        {
            common_data.sky_radiance = 0.0;
            common_data.extinction_factor = 1.0;
        }
        else
        {
            common_data.sky_radiance = input.sky_radiance.xyz;
            common_data.extinction_factor = input.extinction_factor;
        }
    }

    float4 color;
    float4 ssr_out = 0;
    color.rgb = calc_cortana_cook_torrance_lighting(common_data, ssr_out);
    color.a = 1.0f - common_data.albedo.a;
	
    // camo
    
    float fade_gradient = tex2D(fade_gradient_map, apply_xform2d(common_data.texcoord, fade_gradient_map_xform)).a * fade_gradient_scale + input.sky_radiance.w;
    float2 camo_offset = input.camo_param.xy * warp_amount * saturate(fade_gradient + warp_fade_offset) / aspect_ratio;
	
    float2 camo_tex = ((input.position.xy + 0.5f) / texture_size) + (camo_offset / max(0.5f, input.camo_param.w));
    camo_tex = clamp(camo_tex, k_ps_distort_bounds.xy, k_ps_distort_bounds.zw);
	
    float color_interp = saturate(color.a + saturate(1.0f - fade_gradient));
    float3 final_color = lerp(color.rgb, tex2D(scene_ldr_texture, camo_tex).rgb, color_interp);

    PS_OUTPUT_DEFAULT output = export_color(float4(final_color, 1.0f));
    if (calc_env_output)
    {
        output.unknown = ssr_out;
    }
    return output;
}

#endif
