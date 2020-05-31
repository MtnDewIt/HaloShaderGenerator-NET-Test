#ifndef _SHADER_TEMPLATE_PER_VERTEX_COLOR_LIGHTING_HLSLI
#define _SHADER_TEMPLATE_PER_VERTEX_COLOR_LIGHTING_HLSLI


#include "entry_albedo.hlsli"

#include "..\methods\specular_mask.hlsli"
#include "..\methods\material_model.hlsli"
#include "..\methods\environment_mapping.hlsli"
#include "..\methods\self_illumination.hlsli"
#include "..\methods\blend_mode.hlsli"
#include "..\methods\misc.hlsli"

#include "..\registers\shader.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"


PS_OUTPUT_DEFAULT shader_entry_static_per_vertex_color(VS_OUTPUT_PER_VERTEX_COLOR input)
{
	float4 albedo;
	float3 normal;
	float4 sh_0, sh_312[3], sh_457[3], sh_8866[3];
	float3 dominant_light_direction, dominant_light_intensity, diffuse_ref;
	float3 sky_radiance = input.sky_radiance;
	float3 extinction_factor = input.extinction_factor;
	
	float2 texcoord = calc_parallax_ps(input.texcoord, input.camera_dir, input.tangent, input.binormal, input.normal.xyz);
	float alpha = calc_alpha_test_ps(texcoord);
	get_albedo_and_normal(actually_calc_albedo, input.position.xy, texcoord, input.camera_dir, input.tangent.xyz, input.binormal.xyz, input.normal.xyz, albedo, normal);
	
	normal = normalize(input.normal);
	float3 view_dir = normalize(input.camera_dir);
	float3 world_position = Camera_Position_PS - input.camera_dir;
	
	get_current_sh_coefficients_quadratic(sh_0, sh_312, sh_457, sh_8866, dominant_light_direction, dominant_light_intensity);

	float4 color = 0;
	
	float3 self_illumination = calc_self_illumination_ps(input.texcoord.xy, albedo.rgb);
	
	
	if (calc_material)
	{
		float3 material_lighting = material_type(albedo.rgb, normal, view_dir, input.texcoord.xy, input.camera_dir, world_position, sh_0, sh_312, sh_457, sh_8866, dominant_light_direction, dominant_light_intensity, input.vertex_color, false, 1.0, 0.0);
		color.rgb += material_lighting;
	}
	else
	{
		color.rgb += input.vertex_color;
		sky_radiance = 0.0;
		extinction_factor = 1.0;
	}
	
	color.rgb *= albedo.rgb;
	
	color.rgb += self_illumination;
	float3 environment = envmap_type(view_dir, normal);
	
	color.rgb += environment;
	
	color.rgb = color.rgb * extinction_factor;
		
	if (blend_type_arg == k_blend_mode_additive)
	{
		color.a = 0.0;
	}
	else if (blend_type_arg == k_blend_mode_alpha_blend || blend_type_arg == k_blend_mode_pre_multiplied_alpha)
	{
		color.a = alpha * albedo.a;
	}
	else
	{
		color.a = alpha;
	}

	
	if (blend_type_arg != k_blend_mode_additive)
	{
		color.rgb += sky_radiance;
	}

	if (blend_type_arg == k_blend_mode_double_multiply)
		color.rgb *= 2;

	color.rgb = expose_color(color.rgb);
	
	color = blend_type(color, 1.0f);

	return export_color(color);
}

#endif