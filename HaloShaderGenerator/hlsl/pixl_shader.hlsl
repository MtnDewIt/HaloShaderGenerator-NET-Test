#define shader_template


#include "methods\albedo.hlsli"
#include "methods\bump_mapping.hlsli"
#include "methods\alpha_test.hlsli"
#include "methods\specular_mask.hlsli"
#include "methods\material_model.hlsli"
#include "methods\environment_mapping.hlsli"
#include "methods\self_illumination.hlsli"
#include "methods\blend_mode.hlsli"
#include "methods\parallax.hlsli"
#include "methods\misc.hlsli"

#include "registers\shader.hlsli"
#include "helpers\input_output.hlsli"
#include "helpers\shadows.hlsli"
#include "helpers\definition_helper.hlsli"
#include "helpers\color_processing.hlsli"

ALBEDO_PASS_RESULT get_albedo_and_normal(bool calc_albedo, float2 fragcoord, float2 texcoord, float3 camera_dir, float3 tangent, float3 binormal, float3 normal)
{
	ALBEDO_PASS_RESULT result;
	float2 new_texcoord = calc_parallax_ps(texcoord, camera_dir, tangent, binormal, normal);
	calc_alpha_test_ps(new_texcoord);
	if (calc_albedo)
	{
		result.normal = calc_bumpmap_ps(tangent, binormal, normal.xyz, new_texcoord);
		result.albedo = calc_albedo_ps(new_texcoord, fragcoord);
	}
	else
	{
		float2 inv_texture_size = (1.0 / texture_size);
		float2 texcoord = (fragcoord + 0.5) * inv_texture_size;
		float4 normal_texture_sample = tex2D(normal_texture, texcoord);
		float4 albedo_texture_sample = tex2D(albedo_texture, texcoord);
		result.albedo = albedo_texture_sample.xyzw;
		result.normal = normal_texture_sample.xyz * 2.0 - 1.0;
	}
	return result;
}

PS_OUTPUT_ALBEDO entry_albedo(VS_OUTPUT_ALBEDO input) : COLOR
{	
	ALBEDO_PASS_RESULT albedo_pass = get_albedo_and_normal(true, input.position.xy, input.texcoord.xy, input.camera_dir, input.tangent.xyz, input.binormal.xyz, input.normal.xyz);
	float4 albedo = albedo_pass.albedo;
	float3 normal = albedo_pass.normal;
	
	albedo.rgb = rgb_to_srgb(albedo.rgb);

    PS_OUTPUT_ALBEDO output;
	output.diffuse = albedo;
	output.normal = float4(normal_export(normal), albedo.w);
	output.unknown = input.normal.wwww;
    return output;
}

PS_OUTPUT_DEFAULT entry_active_camo(VS_OUTPUT_ACTIVE_CAMO input) : COLOR
{
	float2 fragcoord = 0.5f + input.position.xy;
	fragcoord.x /= texture_size.x;
	fragcoord.y /= texture_size.y;
	
	float2 camo_texcoord = (k_ps_active_camo_factor.yz * input.texcoord) / (4 * aspect_ratio);
	float camo_scale;

	[flatten]
	if (0.5f - input.camo_param.w < 0)
		camo_scale = 1.0f / input.camo_param.w;
	else
		camo_scale = 2.0f;
	
	float2 ldr_texcoord = fragcoord.xy + camo_texcoord * camo_scale;
	float4 sample = tex2D(scene_ldr_texture, ldr_texcoord);
	float4 final_color = float4(sample.rgb, k_ps_active_camo_factor.x);
	
	return export_color(blend_type(final_color, 1.0f));
}

PS_OUTPUT_DEFAULT entry_final_pass(
float4 albedo,
float alpha,
float3 normal,
float2 texcoord,
float3 camera_dir,
float3 view_dir,
float3 world_position,
float4 sh_0,
float4 sh_312[3],
float4 sh_457[3],
float4 sh_8866[3],
float3 dominant_light_direction,
float3 dominant_light_intensity,
float3 diffuse_reflectance,
float radiance_transfer,
float3 vertex_color,
bool no_dynamic_lights,
float3 sky_radiance,
float3 extinction_factor)
{
	float4 color;
	if (calc_material)
	{
		float3 material_lighting = material_type(albedo.rgb, normal, view_dir, texcoord.xy, camera_dir, world_position, sh_0, sh_312, sh_457, sh_8866, dominant_light_direction, dominant_light_intensity, diffuse_reflectance, no_dynamic_lights, radiance_transfer, vertex_color);
		material_lighting = material_lighting * extinction_factor;

		float3 environment = envmap_type(view_dir, normal);
		float4 self_illumination = calc_self_illumination_ps(texcoord.xy, albedo.rgb);

		color.rgb = (environment + self_illumination.rgb) * sky_radiance.xyz + material_lighting;
		
		if (blend_type_arg != k_blend_mode_additive)
		{
			color.rgb += sky_radiance.rgb;
		}
		
		if (blend_type_arg == k_blend_mode_additive)
		{
			color.a = 0.0;
		}
		else if (blend_type_arg == k_blend_mode_alpha_blend || blend_type_arg == k_blend_mode_pre_multiplied_alpha)
		{
			color.a = alpha;
		}
		else
		{
			color.a = 1.0;
		}
	}
	else
	{
		color.rgb = albedo.rgb * vertex_color;
		color.a = 1.0;
	}

	if (blend_type_arg == k_blend_mode_double_multiply)
		color.rgb *= 2;

	color.rgb = expose_color(color.rgb);
	
	float4 output_color = blend_type(color, 1.0f);

	return export_color(output_color);
}

PS_OUTPUT_DEFAULT entry_static_prt_any(
float2 position,
float2 texcoord,
float3 camera_dir,
float3 normal,
float3 tangent,
float3 binormal,
float3 sky_radiance,
float3 extinction_factor,
float prt)
{
	ALBEDO_PASS_RESULT albedo_pass = get_albedo_and_normal(actually_calc_albedo, position.xy, texcoord.xy, camera_dir, tangent.xyz, binormal.xyz, normal.xyz);
	float3 albedo = albedo_pass.albedo.rgb;
	float alpha = albedo_pass.albedo.a;
	normal = normalize(albedo_pass.normal);
	
	float3 view_dir = normalize(camera_dir);
	float3 world_position = Camera_Position_PS - camera_dir;
	
	float4 sh_0, sh_312[3], sh_457[3], sh_8866[3];
	float3 dominant_light_dir, dominant_light_intensity;
	bool m_no_dynamic_lights = no_dynamic_lights;
	get_current_sh_coefficients_quadratic(sh_0, sh_312, sh_457, sh_8866, dominant_light_dir, dominant_light_intensity);
	float3 diffuse_ref = diffuse_reflectance(normal);
	
	return entry_final_pass(albedo_pass.albedo, alpha, normal, texcoord, camera_dir, view_dir, world_position, sh_0, sh_312, sh_457, sh_8866, dominant_light_dir, dominant_light_intensity, diffuse_ref, prt, 0, m_no_dynamic_lights, sky_radiance, extinction_factor);
}
/*
PS_OUTPUT_DEFAULT entry_sh(float2 position,
float2 texcoord,
float3 camera_dir,
float3 tangent,
float3 binormal,
float3 input_normal,
float3 sky_radiance,
float3 extinction_factor,
float prt,
bool per_vertex_color,
float3 vertex_color,
bool use_lightmap,
float2 lightmap_texcoord) : COLOR
{
	ALBEDO_PASS_RESULT albedo_pass = get_albedo_and_normal(actually_calc_albedo, position.xy, texcoord.xy, camera_dir, tangent.xyz, binormal.xyz, input_normal.xyz);
	float3 albedo = albedo_pass.albedo.rgb;
	float alpha = albedo_pass.albedo.a;
	float3 normal = normalize(albedo_pass.normal);
	
	float4 color;
	
	if (calc_material)
	{
		float3 n_camera_dir = normalize(camera_dir);
		float4 sh_0, sh_312[3], sh_457[3], sh_8866[3];
		float3 dominant_light_dir, dominant_light_intensity;
		float3 diffuse_ref;
		bool m_no_dynamic_lights = no_dynamic_lights;
		
		if (use_lightmap)
		{
			lightmap_diffuse_reflectance(input_normal, lightmap_texcoord, diffuse_ref, sh_0, sh_312, sh_457, sh_8866, dominant_light_dir, dominant_light_intensity);
		}
		else if (per_vertex_color)
		{
			diffuse_ref = vertex_color;
			m_no_dynamic_lights = false;
			normal = normalize(input_normal);
		}
		else
		{
			get_current_sh_coefficients_quadratic(sh_0, sh_312, sh_457, sh_8866, dominant_light_dir, dominant_light_intensity);
			diffuse_ref = diffuse_reflectance(normal);
		}
		
		float3 material_lighting = material_type(albedo, normal, n_camera_dir, texcoord.xy, camera_dir, sh_0, sh_312, sh_457, sh_8866, dominant_light_dir, dominant_light_intensity, diffuse_ref, m_no_dynamic_lights, prt);
		
		material_lighting = material_lighting * extinction_factor;

		float3 environment = envmap_type(n_camera_dir, normal);
		float4 self_illumination = calc_self_illumination_ps(texcoord.xy, albedo);

		color.rgb = (environment + self_illumination.xyz) * sky_radiance.xyz + material_lighting;
		
		if (blend_type_arg != k_blend_mode_additive)
		{
			color.rgb += sky_radiance.rgb;
		}
		
		if (blend_type_arg == k_blend_mode_additive)
		{
			color.a = 0.0;
		}
		else if (blend_type_arg == k_blend_mode_alpha_blend || blend_type_arg == k_blend_mode_pre_multiplied_alpha)
		{
			color.a = alpha;
		}
		else
		{
			color.a = 1.0;
		}
	}
	else
	{
		color.rgb = albedo;
		color.a = 1.0;
	}

	if (blend_type_arg == k_blend_mode_double_multiply)
		color.rgb *= 2;

	color.rgb = expose_color(color.rgb);
	
	float4 output_color = blend_type(color, 1.0f);

	return export_color(output_color);
}
*/
PS_OUTPUT_DEFAULT entry_static_sh(VS_OUTPUT_STATIC_SH input) : COLOR
{
	return entry_static_prt_any(input.position.xy, input.texcoord.xy, input.camera_dir.xyz,input.normal, input.tangent, input.binormal, input.sky_radiance, input.extinction_factor, 1.0);
}

PS_OUTPUT_DEFAULT entry_static_prt_ambient(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return entry_static_prt_any(input.position.xy, input.texcoord.xy, input.camera_dir.xyz, input.normal, input.tangent, input.binormal, input.sky_radiance, input.extinction_factor, input.prt_radiance_vector.x);
}

PS_OUTPUT_DEFAULT entry_static_prt_linear(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return entry_static_prt_any(input.position.xy, input.texcoord.xy, input.camera_dir.xyz, input.normal, input.tangent, input.binormal, input.sky_radiance, input.extinction_factor, input.prt_radiance_vector.x);
}

PS_OUTPUT_DEFAULT entry_static_prt_quadratic(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return entry_static_prt_any(input.position.xy, input.texcoord.xy, input.camera_dir.xyz, input.normal, input.tangent, input.binormal, input.sky_radiance, input.extinction_factor, input.prt_radiance_vector.x);
}

PS_OUTPUT_DEFAULT entry_sfx_distort(VS_OUTPUT_SFX_DISTORT input) : COLOR
{
	return export_color(0);
}

PS_OUTPUT_DEFAULT calculate_dynamic_lights(
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
	float3 world_position = Camera_Position_PS - camera_dir;
	float shadow_coefficient;
	float3 diffuse;
	
	
	SimpleLight light = get_simple_light(light_index);
	
	float3 v_to_light = light.position.xyz - world_position;
	float light_distance_squared = dot(v_to_light, v_to_light);
	v_to_light = normalize(v_to_light);
	
	float attenuation = 1.0 / (light_distance_squared + light.position.w);
	float light_angle = dot(v_to_light, light.direction.xyz);

	float2 packed_light_values = float2(attenuation, light_angle);
	packed_light_values = max(0.0001, packed_light_values * light.unknown3.xy + light.unknown3.zw);
	float specular_power = pow(packed_light_values.y, light.color.w);

	float intensity = saturate(specular_power + light.direction.w) * saturate(packed_light_values.x);
	
	float2 shadowmap_texcoord_depth_adjusted = shadowmap_texcoord * (1.0 / depth_scale);
	
	float2 gel_texcoord = apply_xform2d(shadowmap_texcoord_depth_adjusted, p_dynamic_light_gel_xform);
	float4 gel_sample = tex2D(dynamic_light_gel_texture, gel_texcoord);

	diffuse = (intensity * light.color.rgb) * gel_sample.rgb;
	
	ALBEDO_PASS_RESULT albedo_pass = get_albedo_and_normal(actually_calc_albedo, position.xy, texcoord.xy, camera_dir, tangent.xyz, binormal.xyz, normal.xyz);
	diffuse *= dot(v_to_light, albedo_pass.normal);
	diffuse *= albedo_pass.albedo.rgb;
	
	
	if (dynamic_light_shadowing)
	{
		if (is_cinematic)
			shadow_coefficient = shadows_percentage_closer_filtering_custom_4x4(shadowmap_texcoord_depth_adjusted, shadowmap_texture_size, depth_scale, depth_offset, diffuse);
		else
			shadow_coefficient = shadows_percentage_closer_filtering_3x3(shadowmap_texcoord_depth_adjusted, shadowmap_texture_size, depth_scale, depth_offset, diffuse);
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
	else if (blend_type_arg == k_blend_mode_alpha_blend || blend_type_arg == k_blend_mode_pre_multiplied_alpha)
	{
		result.a = albedo_pass.albedo.a;
	}
	else
	{
		result.a = 1.0;
	}
	
	result.rgb = expose_color(diffuse);
	result.rgb *= shadow_coefficient;
	
	return export_color(result);
}

PS_OUTPUT_DEFAULT entry_dynamic_light(VS_OUTPUT_DYNAMIC_LIGHT input) : COLOR
{
	return calculate_dynamic_lights(input.position.xy, input.texcoord, input.camera_dir, input.tangent, input.binormal, input.normal, 0, input.shadowmap_texcoord.w, input.shadowmap_texcoord.z, input.shadowmap_texcoord.xy, false);
}

PS_OUTPUT_DEFAULT entry_dynamic_light_cinematic(VS_OUTPUT_DYNAMIC_LIGHT input) : COLOR
{
	return calculate_dynamic_lights(input.position.xy, input.texcoord, input.camera_dir, input.tangent, input.binormal, input.normal, 0, input.shadowmap_texcoord.w, input.shadowmap_texcoord.z, input.shadowmap_texcoord.xy, true);
}

PS_OUTPUT_DEFAULT entry_lightmap_debug_mode(VS_OUTPUT_LIGHTMAP_DEBUG_MODE input) : COLOR
{
	
	ALBEDO_PASS_RESULT albedo_pass = get_albedo_and_normal(true, input.position.xy, input.texcoord.xy, input.camera_dir, input.tangent.xyz, input.binormal.xyz, input.normal.xyz);
	float3 normal = albedo_pass.normal;
	
	float3 result_color = float3(0, 0, 0);
	float debug_mode = p_render_debug_mode.x;
	
	[branch]
	if (debug_mode < 1)
	{
		result_color.rg = input.lightmap_texcoord.xy;
	}
	else
	{
		[branch]
		if (debug_mode < 2)
		{
			float2 temp = floor(default_lightmap_size * input.lightmap_texcoord.xy);
			temp = temp * 0.5 - floor(0.5 * temp);

			[unbranch]
			if (temp.x == 0)
			{
				[unbranch]
				if (temp.y == 0)
				{
					result_color.rgb = float3(1.0, 0.7, 0.3);
				}
				else
				{
					result_color.rgb = float3(0, 0, 0);
				}
			}
			else
			{
				[unbranch]
				if (temp.y == 0)
				{
					result_color.rgb = float3(0, 0, 0);
				}
				else
				{
					result_color.rgb = float3(1.0, 0.7, 0.3);
				}
			}
		}
		else
		{
			float3 default_color = float3(input.texcoord.xy, 0);
			
			[unbranch]
			if (debug_mode < 3)
				result_color.xyz = input.normal;
			else if (debug_mode < 4)
				result_color.xyz = normal;
			else if (debug_mode < 5)
				result_color.xyz = input.tangent;
			else if (debug_mode < 6)
				result_color.xyz = input.binormal;
			else if (debug_mode < 7)
				result_color.xyz = 0;
			else if (debug_mode < 8)
				result_color.xyz = 0;
			else if (debug_mode < 9)
				result_color.xyz = 0;
			else if (debug_mode >= 10)
				result_color.xyz = 0;
			else
				result_color.xyz = default_color;
			
		}
	}
	result_color = max(result_color, 0);
	PS_OUTPUT_DEFAULT output;
	output.low_frequency = export_low_frequency(float4(result_color, 0));
	output.high_frequency = export_high_frequency(float4(result_color, 0));
	output.unknown = 0;
	return output;
}

PS_OUTPUT_DEFAULT entry_static_per_vertex_color(VS_OUTPUT_PER_VERTEX_COLOR input) : COLOR
{
	ALBEDO_PASS_RESULT albedo_pass = get_albedo_and_normal(actually_calc_albedo, input.position.xy, input.texcoord.xy, input.camera_dir, input.tangent.xyz, input.binormal.xyz, input.normal.xyz);
	float3 albedo = albedo_pass.albedo.rgb;
	float alpha = albedo_pass.albedo.a;
	float3 normal = normalize(input.normal);
	
	float3 view_dir = normalize(input.camera_dir);
	float3 world_position = Camera_Position_PS - input.camera_dir;
	float3 diffuse_ref = input.vertex_color;
	
	float4 sh_0, sh_312[3], sh_457[3], sh_8866[3];
	float3 dominant_light_dir, dominant_light_intensity;
	get_current_sh_coefficients_quadratic(sh_0, sh_312, sh_457, sh_8866, dominant_light_dir, dominant_light_intensity);
	
	return entry_final_pass(albedo_pass.albedo, alpha, normal, input.texcoord, input.camera_dir, view_dir, world_position, sh_0, sh_312, sh_457, sh_8866, dominant_light_dir, dominant_light_intensity, 0.0, 1.0,input.vertex_color, false, input.sky_radiance, input.extinction_factor);
}

PS_OUTPUT_DEFAULT entry_static_per_pixel(VS_OUTPUT_PER_PIXEL input) : COLOR
{
	float4 sh_0, sh_312[3], sh_457[3], sh_8866[3];
	float3 dominant_light_dir, dominant_light_intensity, diffuse_ref;
	get_lightmap_sh_coefficients(input.lightmap_texcoord, sh_0, sh_312, sh_457, sh_8866, dominant_light_dir, dominant_light_intensity);
	
	ALBEDO_PASS_RESULT albedo_pass = get_albedo_and_normal(actually_calc_albedo, input.position.xy, input.texcoord.xy, input.camera_dir, input.tangent.xyz, input.binormal.xyz, input.normal.xyz);
	float3 albedo = albedo_pass.albedo.rgb;
	float alpha = albedo_pass.albedo.a;
	float3 normal = normalize(albedo_pass.normal);
	
	float3 view_dir = normalize(input.camera_dir);
	float3 world_position = Camera_Position_PS - input.camera_dir;
	bool m_no_dynamic_lights = no_dynamic_lights;
	
	lightmap_diffuse_reflectance(normal, sh_0, sh_312, sh_457, sh_8866, dominant_light_dir, dominant_light_intensity, diffuse_ref);
	
	return entry_final_pass(albedo_pass.albedo, alpha, normal, input.texcoord, input.camera_dir, view_dir, world_position, sh_0, sh_312, sh_457, sh_8866, dominant_light_dir, dominant_light_intensity, diffuse_ref, 1.0, 0, m_no_dynamic_lights, input.sky_radiance, input.extinction_factor);
}
