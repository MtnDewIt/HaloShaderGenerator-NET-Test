#ifndef _TERRAIN_TEMPLATE_PER_PIXEL_LIGHTING_HLSLI
#define _TERRAIN_TEMPLATE_PER_PIXEL_LIGHTING_HLSLI

#include "..\helpers\lightmaps.hlsli"

#include "..\helpers\terrain_helper.hlsli"
#include "..\terrain_lighting\terrain_lighting.hlsli"

#include "..\registers\global_parameters.hlsli"
#include "..\helpers\input_output_terrain.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"

PS_OUTPUT_DEFAULT terrain_entry_static_per_pixel(VS_OUTPUT_PER_PIXEL_TERRAIN input)
{
	SHADER_COMMON common_data;
	{
		get_lightmap_sh_coefficients(input.lightmap_texcoord, common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, common_data.dominant_light_direction, common_data.dominant_light_intensity);
		
		common_data.view_dir = input.camera_dir;
		common_data.n_view_dir = normalize(input.camera_dir);
		common_data.fragcoord = input.position.xy;
        common_data.texcoord = input.texcoord.xy;
        common_data.alpha = 1.0f;
		
        float2 position = input.position.xy + 0.5;
        float2 inv_texture_size = (1.0 / texture_size);
        float2 texcoord = position * inv_texture_size;
        float4 normal_texture_sample = tex2D(normal_texture, texcoord);
        common_data.surface_normal = normal_import(normal_texture_sample.xyz);
        float4 albedo_texture_sample = tex2D(albedo_texture, texcoord);
        common_data.albedo = albedo_texture_sample;
		
        //common_data.surface_normal = normalize(common_data.surface_normal);
		
        common_data.specular_mask = 1.0;
		
		float v_dot_n = dot(common_data.n_view_dir, common_data.surface_normal);
		common_data.half_dir = v_dot_n * common_data.surface_normal - common_data.n_view_dir;
		common_data.reflect_dir = common_data.half_dir * 2 + common_data.n_view_dir;
		
		common_data.diffuse_reflectance = lightmap_diffuse_reflectance(common_data.surface_normal, common_data.sh_0, common_data.sh_312, common_data.sh_457, common_data.sh_8866, common_data.dominant_light_direction, common_data.dominant_light_intensity, common_data.sh_0_no_dominant_light, common_data.sh_312_no_dominant_light);

        common_data.world_position = Camera_Position_PS - common_data.view_dir;

        common_data.precomputed_radiance_transfer = 1.0;
        common_data.per_vertex_color = 0.0f;
        common_data.no_dynamic_lights = false;
		
        common_data.sky_radiance = input.sky_radiance;
        common_data.extinction_factor = input.extinction_factor;
    }

    float4 color;
    float4 unknown_color;
	
    color.rgb = calc_lighting_terrain(common_data, unknown_color);
    color.a = 1.0f;

    color.rgb *= common_data.extinction_factor;
    color.rgb += common_data.sky_radiance.rgb;
	
    color.rgb = color.rgb * g_exposure.x;
    color.rgb = max(color.rgb, 0);
    PS_OUTPUT_DEFAULT output;
    output.low_frequency = export_low_frequency(color);
    output.high_frequency = export_high_frequency(color);

    if (calc_env_output)
        output.unknown = unknown_color;
    else
        output.unknown = 0.0f;
	
    return output;
}
#endif