#include "registers/chud_pixl_shader.hlsli"
#include "helpers/chud_input_output.hlsli"
#include "helpers/color_processing.hlsli"

float4 main(VS_OUTPUT_CORTANA_COMPOSITE input) : COLOR
{
	float2 frag_coord = input.v1 - 0.5;
	float distance_to_frag = length(frag_coord);
	
	float vignette = (distance_to_frag - cortana_vignette_data.x) / (cortana_vignette_data.y - cortana_vignette_data.x);
	vignette = saturate(vignette);
	vignette *= vignette;
	
	float solarize = 1.0 / (1.0 - cortana_comp_solarize_result.x);
	
	float4 combined_texcoord;
	combined_texcoord.xy = input.v0;
	combined_texcoord = combined_texcoord.xyxy * input.v3;
	float4 texcoord = 2.0 * combined_texcoord  - 1.0;
	float2 scales = float2(0.8, 0.5);
	texcoord = texcoord * scales.xxyy * cortana_comp_doubling_result.z;
	texcoord = texcoord * 0.5 + 0.5; // invert 2 * - 1
	
	float4 sample1 = tex2D(cortana_sampler, texcoord.xy);
	float4 sample2 = tex2D(cortana_sampler, texcoord.zw);
	
	float4 sampler_result = sample1 + sample2;
	
	float doubling_in_mix = dot(sampler_result, cortana_comp_doubling_inmix);
	
	sampler_result.a = 1.0;
	
	float4 basemap_sample = tex2D(basemap_sampler, combined_texcoord.zw);
	float4 color_mix;
	color_mix.rgb = basemap_sample.rgb * cortana_back_colormix_result.xyz;
	color_mix = mul(p_postprocess_hue_saturation_matrix, color_mix.rgb);
	color_mix.a = 0.0;
	
	float4 cortana_sample = tex2D(cortana_sampler, combined_texcoord.zw);
	float4 outmix = cortana_comp_doubling_result.x * cortana_comp_doubling_outmix;
	
	outmix = outmix * doubling_in_mix + cortana_sample;
	
	float solarize_mix = min(dot(outmix, cortana_comp_solarize_inmix), 1.0);
	solarize_mix = solarize_mix - cortana_comp_solarize_result.x;
	solarize = solarize * solarize_mix;
	
	float exp_solarize = pow(solarize, cortana_comp_solarize_result.y);

	float solarize_scale = solarize_mix < 0 ? exp_solarize : 0;
	
	float4 color_result = solarize_scale * cortana_comp_solarize_outmix + sampler_result;
	
	[unroll]
	if (chud_comp_colorize_enabled)
	{
		float colorize = min(dot(color_result, cortana_comp_colorize_inmix), 1.0);
		color_result = colorize * cortana_comp_colorize_result * cortana_comp_colorize_outmix;
	}

	color_result.rgb = srgb_to_rgb(color_result.rgb);
	
	float2 goo_texcoord = (cortana_sample.x * 0.5 + vignette) * float2(0.4, 0.6);
	float4 goo_sample = tex2D(goo_sampler, goo_texcoord);
	float4 color_mix_scale = 1.0 - (goo_texcoord.x * goo_sample + goo_texcoord.y);
	
	return color_mix * color_mix_scale + color_result;
}