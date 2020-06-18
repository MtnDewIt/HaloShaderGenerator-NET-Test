#ifndef _SHADER_TEMPLATE_ACTIVE_CAMO_HLSLI
#define _SHADER_TEMPLATE_ACTIVE_CAMO_HLSLI

#include "..\methods\blend_mode.hlsli"
#include "..\registers\shader.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"

PS_OUTPUT_DEFAULT shader_entry_active_camo(VS_OUTPUT_ACTIVE_CAMO input)
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
	
	if (blend_type_arg == k_blend_mode_pre_multiplied_alpha)
		final_color.rgb *= k_ps_active_camo_factor.x;

	PS_OUTPUT_DEFAULT output;
	
	if (blend_type_arg == k_blend_mode_double_multiply || blend_type_arg == k_blend_mode_multiply)
	{
		output.low_frequency = final_color * g_exposure.w;
		output.high_frequency = final_color * g_exposure.z;
	}
	else
	{
		output.low_frequency = export_low_frequency(final_color);
		output.high_frequency = export_high_frequency(final_color);
	}
	
	
	
	output.unknown = 0;
	return output;
}

#endif