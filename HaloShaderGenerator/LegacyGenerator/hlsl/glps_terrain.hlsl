#include "registers/global_parameters.hlsli"
#include "helpers/input_output.hlsli"

PS_OUTPUT_SHADOW_GENERATE entry_shadow_generate(VS_OUTPUT_SHADOW_GENERATE input) : COLOR
{
	PS_OUTPUT_SHADOW_GENERATE output;
	
	output.unknown = 1.0;
	output.depth = input.depth;

    return output;
}
