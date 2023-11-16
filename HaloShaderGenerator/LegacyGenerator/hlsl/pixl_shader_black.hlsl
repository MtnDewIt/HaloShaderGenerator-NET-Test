#define shader_template

#include "registers/global_parameters.hlsli"
#include "helpers/input_output.hlsli"
#include "helpers/shadows.hlsli"

#include "helpers/color_processing.hlsli"

PS_OUTPUT_ALBEDO entry_albedo(VS_OUTPUT_BLACK_ALBEDO input) : COLOR
{
    PS_OUTPUT_ALBEDO output;
    output.diffuse.rgb = input.scattering * g_exposure.x;
	output.diffuse.a = 0.0f;
    output.normal.xyz = float3(0.0f, 0.0f, 0.0f) * 0.5f + 0.5f;
    output.normal.w = output.diffuse.a;
	output.unknown = 0;
    return output;
}

