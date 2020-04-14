#define shader_template

#include "registers/shader.hlsli"
#include "helpers/input_output.hlsli"
#include "helpers/albedo_pass.hlsli"
#include "helpers/shadows.hlsli"

#include "methods/albedo.hlsli"
#include "helpers/color_processing.hlsli"

//TODO: These must be in the correct order for the registers to align, double check this
#include "methods\bump_mapping.hlsli"
#include "methods\alpha_test.hlsli"
#include "methods\specular_mask.hlsli"
#include "methods\material_model.hlsli"
#include "methods\environment_mapping.hlsli"
#include "methods\self_illumination.hlsli"
#include "methods\blend_mode.hlsli"
#include "methods\parallax.hlsli"
#include "methods\misc.hlsli"

#define aspect_ratio float2(16,9) // this is unusual, there should be a global variable, gotta check h3 (could be 4,3 or other)

PS_OUTPUT_ALBEDO entry_albedo(VS_OUTPUT_ALBEDO input) : COLOR
{
    float2 texcoord = input.texcoord.xy;
    float3 tangent = input.tangent.xyz;
    float3 binormal = input.binormal.xyz;
    float3 normal = input.normal.xyz;
    float3 unknown = input.normal.w;
    
    float4 diffuse_and_alpha = calc_albedo_ps(texcoord);
	normal = calc_bumpmap_ps(tangent, binormal, normal, texcoord);

	diffuse_and_alpha.xyz = apply_debug_tint(diffuse_and_alpha.xyz);
    diffuse_and_alpha.xyz = rgb_to_srgb(diffuse_and_alpha.xyz);

    PS_OUTPUT_ALBEDO output;
    output.diffuse = blend_type(float4(diffuse_and_alpha));
    output.normal = blend_type(float4(normal_export(normal), diffuse_and_alpha.w));
	output.unknown = unknown.xxxx;
    return output;
}

PS_OUTPUT_DEFAULT entry_active_camo(VS_OUTPUT_ACTIVE_CAMO input) : COLOR
{
	float2 fragcoord = input.position.xy + 0.5;
	float2 camo_texcoord_offset = (k_ps_active_camo_factor.yz) * input.texcoord.xy;
	camo_texcoord_offset.x /= (4 * aspect_ratio).x;
	camo_texcoord_offset.y /= (4 * aspect_ratio).y;
	float camo_scale = 0.5 - input.camo_param.w < 0 ? 1.0 / input.camo_param.w : 2.0;
	
	fragcoord.x /= texture_size.x;
	fragcoord.y /= texture_size.y;
	
	float2 ldr_texcoord = camo_texcoord_offset * camo_scale + fragcoord;

	float4 sample = tex2D(scene_ldr_texture, ldr_texcoord.xy);

    PS_OUTPUT_DEFAULT output;
	float4 final_color = float4(sample.rgb, k_ps_active_camo_factor.x);
	output.high_frequency = export_high_frequency(final_color);
	output.low_frequency = export_low_frequency(final_color);
	output.unknown = 0;
    return output;
}

#define overwrite(old, new) (clamp(old * 0.0001, 0, 0.0001) + new)

PS_OUTPUT_ALBEDO entry_static_prt(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	PS_OUTPUT_DEFAULT output;

	float3 camera_dir = input.camera_dir.xyz;
	float2 fragcoord = (input.position.xy + 0.5) / texture_size;
    
    // TODO: this may be overkill, check other shaders to see if it always use the diffuse/normal texture or it can compute the albedo again
	ALBEDO_PASS_RESULT albedo_and_normal = get_albedo_and_normal(fragcoord, input.texcoord.xy, input.tangent.xyz, input.binormal.xyz, input.normal.xyz);
	float3 albedo = albedo_and_normal.albedo;
	float3 normal = albedo_and_normal.normal;

	float3 n_camera_dir = normalize(camera_dir);
    
	float3 material_lighting = material_type(albedo, normal, n_camera_dir, input.texcoord.xy, input.extinction_factor.rgb, input.sky_radiance.rgb, camera_dir, input.prt_radiance_vector.x);
	float3 environment = envmap_type(n_camera_dir, normal);
	float4 self_illumination = calc_self_illumination_ps(input.texcoord.xy, albedo);

	float3 color = (environment + self_illumination.xyz) * input.sky_radiance.xyz + material_lighting;

	//color = overwrite(color, material_lighting);

	float3 exposed_color = expose_color(color);

    //TODO: No transparency so far, we're going to need this!!!
	float4 output_color = blend_type(float4(exposed_color, 1.0));

	output.low_frequency = export_low_frequency(output_color);
	output.high_frequency = export_high_frequency(output_color);

	output.unknown = 0;

	return output;
}

PS_OUTPUT_DEFAULT entry_static_prt_ambient(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return entry_static_prt(input);

}

PS_OUTPUT_DEFAULT entry_static_prt_linear(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return entry_static_prt(input);
}

PS_OUTPUT_DEFAULT entry_static_prt_quadratic(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return entry_static_prt(input);
}

PS_OUTPUT_DEFAULT entry_sfx_distort(VS_OUTPUT_SFX_DISTORT input) : COLOR
{
	PS_OUTPUT_DEFAULT output;
	
	output.low_frequency = 0;
	output.high_frequency = 0;
	output.unknown = 0;
	return output;
}

PS_OUTPUT_DEFAULT entry_dynamic_light(VS_OUTPUT_DYNAMIC_LIGHT input) : COLOR
{
	// TODO: move that code into functions while preserving compile order
	float3 world_position = Camera_Position_PS - input.camera_dir;
	SimpleLight light = get_simple_light(0);
	
	float3 v_to_light = light.position.xyz - world_position;
	float light_distance_squared = dot(v_to_light, v_to_light);
	v_to_light = normalize(v_to_light);
	
	float attenuation = 1.0 / (light_distance_squared + light.position.w);
	
	float3 light_dir = light.direction.xyz;
	float light_angle = dot(v_to_light, light_dir);
	
	float2 packed_light_values = float2(attenuation, light_angle);
	packed_light_values = max(0.0001, packed_light_values * light.unknown3.xy + light.unknown3.zw);
	float specular_power = pow(packed_light_values.y, light.color.w);
	

	float intensity = saturate(specular_power + light.direction.w) * saturate(packed_light_values.x);
	float2 scale = 1.0 / texture_size;
	float2 fragcoord = (input.position.xy + 0.5) * scale;
	
	float3 normal = 2 * tex2D(normal_texture, fragcoord).xyz - 1;

	float angle2 = dot(v_to_light, normal);
	
	float2 shadowmap_texcoord = (1.0 / input.shadowmap_texcoord.w) * input.shadowmap_texcoord.xy;
	float2 gel_texcoord = apply_xform2d(shadowmap_texcoord, p_dynamic_light_gel_xform);
	float4 gel_sample = tex2D(dynamic_light_gel_texture, gel_texcoord);
	
	float3 diffuse = gel_sample.rgb * light.color.rgb * intensity * angle2;
	float3 albedo = tex2D(albedo_texture, fragcoord).rgb;
	diffuse *= albedo;
	
	float sc1 = -0.001953125; // c2x -1/512
	float sc2 = 0.001953125; //c2w 1/512
	
	float shadow_coefficient = 0.0;
	
	if (dynamic_light_shadowing)
	{	
		shadow_coefficient = shadows_percentage_closer_filtering_3x3(shadowmap_texcoord, 512, input.shadowmap_texcoord.w, input.shadowmap_texcoord.z, diffuse);
	}
	else
	{
		shadow_coefficient = 1.0;
	}
	
	diffuse *= shadow_coefficient;
	float4 result = float4(expose_color(diffuse), 0);
	
	PS_OUTPUT_DEFAULT output;
	output.low_frequency = export_low_frequency(result);
	output.high_frequency = export_high_frequency(result);
	output.unknown = 0;
	return output;
}

PS_OUTPUT_DEFAULT entry_dynamic_light_cinematic(VS_OUTPUT_DYNAMIC_LIGHT input) : COLOR
{
	// TODO: move that code into functions while preserving compile order
	float3 world_position = Camera_Position_PS - input.camera_dir;
	SimpleLight light = get_simple_light(0);
	
	float3 v_to_light = light.position.xyz - world_position;
	float light_distance_squared = dot(v_to_light, v_to_light);
	v_to_light = normalize(v_to_light);
	
	float attenuation = 1.0 / (light_distance_squared + light.position.w);
	
	float3 light_dir = light.direction.xyz;
	float light_angle = dot(v_to_light, light_dir);
	
	float2 packed_light_values = float2(attenuation, light_angle);
	packed_light_values = max(0.0001, packed_light_values * light.unknown3.xy + light.unknown3.zw);
	float specular_power = pow(packed_light_values.y, light.color.w);
	

	float intensity = saturate(specular_power + light.direction.w) * saturate(packed_light_values.x);
	float2 scale = 1.0 / texture_size;
	float2 fragcoord = (input.position.xy + 0.5) * scale;
	
	float3 normal = 2 * tex2D(normal_texture, fragcoord).xyz - 1;

	float angle2 = dot(v_to_light, normal);
	
	float2 shadowmap_texcoord = (1.0 / input.shadowmap_texcoord.w) * input.shadowmap_texcoord.xy;
	float2 gel_texcoord = apply_xform2d(shadowmap_texcoord, p_dynamic_light_gel_xform);
	float4 gel_sample = tex2D(dynamic_light_gel_texture, gel_texcoord);
	
	float3 diffuse = gel_sample.rgb * light.color.rgb * intensity * angle2;
	float3 albedo = tex2D(albedo_texture, fragcoord).rgb;
	diffuse *= albedo;
	
	float sc1 = -0.001953125; // c2x -1/512
	float sc2 = 0.001953125; //c2w 1/512
	
	float shadow_coefficient = 0.0;
	
	if (dynamic_light_shadowing)
	{
		shadow_coefficient = shadows_percentage_closer_filtering_4x4(shadowmap_texcoord, 512, input.shadowmap_texcoord.w, input.shadowmap_texcoord.z, diffuse);
	}
	else
	{
		shadow_coefficient = 1.0;
	}
	
	diffuse *= shadow_coefficient;
	float4 result = float4(expose_color(diffuse), 0);
	
	PS_OUTPUT_DEFAULT output;
	output.low_frequency = export_low_frequency(result);
	output.high_frequency = export_high_frequency(result);
	output.unknown = 0;
	return output;
}

PS_OUTPUT_DEFAULT entry_lightmap_debug_mode(VS_OUTPUT_LIGHTMAP_DEBUG_MODE input) : COLOR
{
	PS_OUTPUT_DEFAULT output;
	
	float3 result_color;
	
	[branch]
	if (p_render_debug_mode < 1)
	{
		result_color = float3(input.lightmap_texcoord, 0);
	}
	else
	{
		[branch]
		if (p_render_debug_mode < 2)
		{
			float2 temp = floor(1024 * input.lightmap_texcoord);
			temp = floor(0.5 * temp);
			float4 unknown = -abs(temp.y) < 0 ? float4(1, 0.7, 0.3, 0) : float4(1, 0, 0, 0);
			float3 temp2 = float3(unknown.w, temp);
			result_color.xyz = -abs(temp.x) < 0 ? temp2 :  unknown.xyz;

		}
		else
		{
			float3 temp_color = float3(input.texcoord, 0);
			float4 offset = p_render_debug_mode + float4(-7, -8, -9, -10);
			temp_color = temp_color < offset.w ? 0 : temp_color;
			temp_color = temp_color < offset.z ? temp_color : 0;
			temp_color = temp_color < offset.y ? temp_color : 0;
			temp_color = temp_color < offset.x ? temp_color : 0;
			
			offset = p_render_debug_mode + float4(-3, -4, -5, -6);
			temp_color.xyz = temp_color < offset.w ? temp_color : input.binormal;
			temp_color.xyz = temp_color < offset.z ? temp_color : input.tangent;
			temp_color.xyz = temp_color < offset.y ? temp_color : input.normal;
			result_color.xyz = temp_color < offset.x ? temp_color : input.normal;
		}
	}
	result_color = max(result_color, 0);
	output.low_frequency = export_low_frequency(float4(result_color, 0));
	output.high_frequency = export_high_frequency(float4(result_color, 0));
	output.unknown = 0;
	return output;
}