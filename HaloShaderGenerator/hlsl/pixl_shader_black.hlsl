#define shader_template

#include "registers/shader.hlsli"
#include "helpers/input_output.hlsli"
#include "helpers/shadows.hlsli"

#include "helpers/color_processing.hlsli"

PS_OUTPUT_ALBEDO entry_albedo(VS_OUTPUT_BLACK_ALBEDO input) : COLOR
{
    PS_OUTPUT_ALBEDO output;
	output.diffuse.rgb = expose_color(input.color.rgb);
	output.diffuse.a = 1.0;
    output.normal = 0; // no idea what this is in H3, no constants defined
	output.unknown = 0;
    return output;
}

