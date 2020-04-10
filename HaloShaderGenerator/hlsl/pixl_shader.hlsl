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
    output.Diffuse = blend_type(float4(diffuse_and_alpha));
    output.Normal = blend_type(float4(normal_export(normal), diffuse_and_alpha.w));
    output.Unknown = unknown.xxxx;
    return output;
}

PS_OUTPUT_DEFAULT entry_active_camo(VS_OUTPUT_ACTIVE_CAMO input) : COLOR
{
    //note: vpos is in range [0, viewportsize]
    // add a half pixel offset
    float2 vpos = input.vPos.xy;
    float2 screen_location = vpos + 0.5; // half pixel offset
    float2 texel_size = float2(1.0, 1.0) / texture_size.xy;
    float2 fragcoord = screen_location * texel_size; // converts to [0, 1] range

    // I'm not sure what is happening here with these three
    // but I think its a depth value, and this is a kind of
    // clamp of the effect at a distance
    float unknown0 = 0.5 - input.TexCoord1.w;
    float unknown1 = 1.0 / input.TexCoord1.w;
    float unknown2 = unknown0 >= 0 ? 2.0 : unknown1;

    // not sure where these values come from
    // however, the aspect ratio is 16:9 multiplied by 4
    float2 unknown3 = input.TexCoord.xy * k_ps_active_camo_factor.yz / float2(64, 36);

    float2 texcoord = unknown3 * unknown2 + fragcoord;

    float4 sample = tex2D(scene_ldr_texture, texcoord);
    float3 color = sample.xyz;

    float alpha = k_ps_active_camo_factor.x;
    
    PS_OUTPUT_DEFAULT output;
    output.HighFrequency = export_high_frequency(float4(color, alpha));
    output.LowFrequency = export_low_frequency(float4(color, alpha));
    output.Unknown = float4(0, 0, 0, 0);
    return output;
}

#define overwrite(old, new) (clamp(old * 0.0001, 0, 0.0001) + new)

PS_OUTPUT_ALBEDO entry_static_prt(VS_OUTPUT_STATIC_PTR input) : COLOR
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

	output.LowFrequency = export_low_frequency(output_color);
	output.HighFrequency = export_high_frequency(output_color);

	output.Unknown = float4(0, 0, 0, 0);

	return output;
}

PS_OUTPUT_DEFAULT entry_static_prt_ambient(VS_OUTPUT_STATIC_PTR input) : COLOR
{
	return entry_static_prt(input);

}

PS_OUTPUT_DEFAULT entry_static_prt_linear(VS_OUTPUT_STATIC_PTR input) : COLOR
{
	return entry_static_prt(input);
}

PS_OUTPUT_DEFAULT entry_static_prt_quadratic(VS_OUTPUT_STATIC_PTR input) : COLOR
{
	return entry_static_prt(input);
}
