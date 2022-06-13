#ifndef _TERRAIN_TEMPLATE_LIGHTMAP_DEBUG_HLSLI
#define _TERRAIN_TEMPLATE_LIGHTMAP_DEBUG_HLSLI

#include "..\registers\global_parameters.hlsli"
#include "..\helpers\input_output_terrain.hlsli"
#include "..\helpers\color_processing.hlsli"

PS_OUTPUT_DEFAULT shader_entry_lightmap_debug_mode(VS_OUTPUT_LIGHTMAP_DEBUG_MODE_TERRAIN input) : COLOR
{	
	float3 result_color = 0;
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

			if (temp.x == 0)
			{
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
			
			if (debug_mode < 3)
				result_color.xyz = input.normal;
			else if (debug_mode < 4)
				result_color.xyz = 0.0f;
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
	float4 color = float4(result_color, 0);
	color.rgb = max(color.rgb, 0);
	
	PS_OUTPUT_DEFAULT output;
	output.low_frequency = export_low_frequency(color);
	output.high_frequency = export_high_frequency(color);
	output.unknown = 0;
	
	return output;
}

#endif