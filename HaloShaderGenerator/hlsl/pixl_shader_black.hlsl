#define shader_template

#include "registers/shader.hlsli"
#include "helpers/input_output.hlsli"
#include "helpers/shadows.hlsli"

#include "helpers/color_processing.hlsli"

PS_OUTPUT_ALBEDO entry_albedo(VS_OUTPUT_BLACK_ALBEDO input) : COLOR
{
    PS_OUTPUT_ALBEDO output;
    output.diffuse.rgb = input.color.rgb * g_exposure.x; // should be expose_color(), doesnt show g_exposure as a pixl parameter though?
	output.diffuse.a = 1.0;
    output.normal = 0;
	output.unknown = 0;
    return output;
}

