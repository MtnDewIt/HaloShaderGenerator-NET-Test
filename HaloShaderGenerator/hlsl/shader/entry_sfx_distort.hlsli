#ifndef _SHADER_TEMPLATE_SFX_DISTORT
#define _SHADER_TEMPLATE_SFX_DISTORT

#include "..\registers\shader.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\color_processing.hlsli"

PS_OUTPUT_DEFAULT shader_entry_sfx_distort(VS_OUTPUT_SFX_DISTORT input)
{
	return export_color(0);
}
#endif