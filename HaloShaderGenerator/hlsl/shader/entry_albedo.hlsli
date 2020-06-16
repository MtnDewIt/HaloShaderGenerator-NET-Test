#ifndef _SHADER_TEMPLATE_ALBEDO_HLSLI
#define _SHADER_TEMPLATE_ALBEDO_HLSLI

#include "..\methods\albedo.hlsli"
#include "..\methods\parallax.hlsli"
#include "..\methods\bump_mapping.hlsli"
#include "..\methods\alpha_test.hlsli"

#include "..\registers\shader.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"

void get_albedo_and_normal(
bool calc_albedo,
float2 fragcoord,
float2 texcoord,
float3 camera_dir,
float3 tangent,
float3 binormal,
float3 normal,
out float4 albedo,
out float3 out_normal)
{
	if (calc_albedo)
	{
		out_normal = calc_bumpmap_ps(tangent, binormal, normal.xyz, texcoord);
		albedo = calc_albedo_ps(texcoord, fragcoord);
	}
	else
	{
		fragcoord += 0.5;
		float2 inv_texture_size = (1.0 / texture_size);
		float2 texcoord = fragcoord * inv_texture_size; 
		float4 normal_texture_sample = tex2D(normal_texture, texcoord);
		float4 albedo_texture_sample = tex2D(albedo_texture, texcoord);
		out_normal = normal_import(normal_texture_sample.xyz);
		albedo = albedo_texture_sample;
	}
}

PS_OUTPUT_ALBEDO shader_entry_albedo(VS_OUTPUT_ALBEDO input)
{	
	float4 albedo;
	float3 normal;
	float2 texcoord = calc_parallax_ps(input.texcoord, input.camera_dir, input.tangent, input.binormal, input.normal.xyz);
	float alpha = calc_alpha_test_ps(texcoord);
	
	get_albedo_and_normal(true, input.position.xy, texcoord, input.camera_dir, input.tangent.xyz, input.binormal.xyz, input.normal.xyz, albedo, normal);

	albedo.rgb = rgb_to_srgb(albedo.rgb);

    PS_OUTPUT_ALBEDO output;
	output.diffuse = albedo;
	output.normal = float4(normal_export(normal), albedo.w);
	output.unknown = input.normal.wwww;
    return output;
}

#endif