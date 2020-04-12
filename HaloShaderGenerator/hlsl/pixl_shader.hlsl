#define shader_template

#include "registers/shader.hlsli"
#include "helpers/input_output.hlsli"
#include "helpers/albedo_pass.hlsli"

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
	float2 camo_texcoord_offset = (k_ps_active_camo_factor.yz) * input.texcoord1.xy;
	camo_texcoord_offset.x /= (4 * aspect_ratio).x;
	camo_texcoord_offset.y /= (4 * aspect_ratio).y;
	float camo_scale = 0.5 - input.texcoord2.w < 0 ? 1.0 / input.texcoord2.w : 2.0;
	
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
