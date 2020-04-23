#include "registers/chud_pixl_shader.hlsli"
#include "helpers/chud_input_output.hlsli"
#include "helpers/color_processing.hlsli"

float4 main(VS_OUTPUT_CORTANA_COMPOSITE input) : COLOR
{
	float4 color_mix;
	
	
	float distance_to_frag = length(input.v1 - 0.5);
	float2 texcoord = input.v0 * input.v3;
	
	float4 basemap_sample = tex2D(basemap_sampler, texcoord);
	float4 cortana_sample = tex2D(cortana_sampler, texcoord);
	float4 goo_sample = tex2D(goo_sampler, input.v2.xy);
	
	float vignette = saturate((distance_to_frag - cortana_vignette_data.x) / (cortana_vignette_data.y - cortana_vignette_data.x));
	float2 unknown_vignette = (cortana_sample.x * 0.5 + vignette * vignette) * float2(0.4, 0.6);
	float4 color_mix_scale = 1.0 - (unknown_vignette.x * goo_sample + unknown_vignette.y);
	
	
	
	
	
	
	
	
	float2 transformed_texcoord = 2.0 * texcoord - 1.0;
	float scale_1 = 0.8 * cortana_comp_doubling_result.z;
	float scale_2 = 0.5 * cortana_comp_doubling_result.z;
	float2 texcoord_scale_1 = transformed_texcoord * scale_1;
	float2 texcoord_scale_2 = transformed_texcoord * scale_2;
	texcoord_scale_1 = 0.5 + 0.5 * texcoord_scale_1;
	texcoord_scale_2 = 0.5 + 0.5 * texcoord_scale_2;

	
	float4 sample1 = tex2D(cortana_sampler, texcoord_scale_1);
	float4 sample2 = tex2D(cortana_sampler, texcoord_scale_2);
	float4 sampler_result = sample1 + sample2;
	float doubling_in_mix = dot(sampler_result, cortana_comp_doubling_inmix);

	
	
	color_mix.a = 1.0;
	color_mix.rgb = basemap_sample.rgb * cortana_back_colormix_result.xyz;
	color_mix.rgb = mul(color_mix, p_postprocess_hue_saturation_matrix);
	
	
	
	float4 outmix = cortana_comp_doubling_result.x * cortana_comp_doubling_outmix;
	
	
	outmix = outmix * doubling_in_mix + cortana_sample;
	
	float solarize = 1.0 / (1.0 - cortana_comp_solarize_result.x);
	float solarize_mix = min(dot(outmix, cortana_comp_solarize_inmix), 1.0);
	solarize_mix = solarize_mix - cortana_comp_solarize_result.x;
	solarize = solarize * solarize_mix;
	
	float exp_solarize = pow(solarize, cortana_comp_solarize_result.y);

	float solarize_scale = solarize_mix < 0 ? 1 : 0;
	solarize_scale *= exp_solarize;
	
	float4 color_result = solarize_scale * cortana_comp_solarize_outmix + outmix;
	
	[unroll]
	if (chud_comp_colorize_enabled)
	{
		float colorize = min(dot(color_result, cortana_comp_colorize_inmix), 1.0);
		color_result = colorize * cortana_comp_colorize_result * cortana_comp_colorize_outmix;
	}

	color_result.rgb = srgb_to_rgb(color_result.rgb);
	
	
	
	
	
	return color_mix * color_mix_scale + color_result;
}