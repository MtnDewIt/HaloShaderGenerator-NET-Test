#ifndef _FOLIAGE_TEMPLATE_PER_PIXEL_LIGHTING_HLSLI
#define _FOLIAGE_TEMPLATE_PER_PIXEL_LIGHTING_HLSLI

#include "..\methods\albedo.hlsli"
#include "..\helpers\lightmaps.hlsli"
#include "..\registers\global_parameters.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"
#include "..\methods\alpha_test.hlsli"

PS_OUTPUT_DEFAULT foliage_entry_static_per_pixel(VS_OUTPUT_PER_PIXEL_FOLIAGE input)
{
    SHADER_COMMON common_data;
	{
        get_lightmap_sh_coefficients(input.lightmap_texcoord, common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, common_data.dominant_light_direction, common_data.dominant_light_intensity);
		
        common_data.view_dir = input.camera_dir;
        common_data.n_view_dir = normalize(input.camera_dir);
        common_data.fragcoord = input.position.xy;
        common_data.tangent = input.tangent;
        common_data.binormal = input.binormal;
        common_data.normal = input.normal;
        common_data.texcoord = input.texcoord.xy;
        
        {
            float2 position = input.position.xy;
            position += 0.5;
            float2 inv_texture_size = (1.0 / texture_size);
            float2 texcoord = position * inv_texture_size;
            float4 normal_texture_sample = tex2D(normal_texture, texcoord);
            common_data.surface_normal = normal_import(normal_texture_sample.xyz);
            float4 albedo_texture_sample = tex2D(albedo_texture, texcoord);
            common_data.albedo = albedo_texture_sample;
        }
		
        common_data.alpha = calc_alpha_test_ps(common_data.texcoord, common_data.albedo.a);
		
        common_data.surface_normal = normalize(common_data.surface_normal);
				
        common_data.specular_mask = 1.0;
		
        float v_dot_n = dot(common_data.n_view_dir, common_data.surface_normal);
        common_data.half_dir = v_dot_n * common_data.surface_normal - common_data.n_view_dir;
        common_data.reflect_dir = common_data.half_dir * 2 + common_data.n_view_dir;
		
        //common_data.diffuse_reflectance = lightmap_diffuse_reflectance(common_data.surface_normal, common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, common_data.dominant_light_direction, common_data.dominant_light_intensity, common_data.sh_0_no_dominant_light, common_data.sh_312_no_dominant_light);

        common_data.world_position = Camera_Position_PS - common_data.view_dir;

        common_data.precomputed_radiance_transfer = 1.0;
        common_data.per_vertex_color = 0.0f;
        common_data.no_dynamic_lights = true;
		
        if (!calc_atmosphere_no_material && !calc_material)
        {
            common_data.sky_radiance = 0.0;
            common_data.extinction_factor = 1.0;
        }
        else
        {
            common_data.sky_radiance = input.sky_radiance;
            common_data.extinction_factor = input.extinction_factor;
        }
		
    }
	
    float4 color;

    color.rgb = common_data.albedo.rgb * input.unknown_lighting_color;
    color.rgb *= common_data.extinction_factor;
    color.a = common_data.alpha;
	
    color.rgb += common_data.sky_radiance.rgb;

    color.rgb = expose_color(color.rgb);

    PS_OUTPUT_DEFAULT output = export_color(color);
    output.unknown = 0;
    return output;
}

#endif
