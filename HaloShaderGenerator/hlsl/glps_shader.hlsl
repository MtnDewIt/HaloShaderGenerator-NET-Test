#include "registers/global_parameters.hlsli"
#include "helpers/input_output.hlsli"
#include "helpers/albedo_pass.hlsli"
#include "helpers/shadows.hlsli"

#include "methods/albedo.hlsli"
#include "helpers/color_processing.hlsli"

//TODO: These must be in the correct order for the registers to align, double check this
#include "methods\alpha_test.hlsli"


PS_OUTPUT_SHADOW_GENERATE entry_shadow_generate(VS_OUTPUT_SHADOW_GENERATE input) : COLOR
{
	PS_OUTPUT_SHADOW_GENERATE output;
	
	calc_alpha_test_ps(input.texcoord);
	
	output.unknown = 1.0;
	output.depth = input.depth;

    return output;
}
