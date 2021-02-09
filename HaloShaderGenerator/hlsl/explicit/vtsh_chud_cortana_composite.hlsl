#include "../registers/chud_vtsh_shader.hlsli"
#include "../helpers/chud_input_output.hlsli"

VS_OUTPUT_CORTANA_COMPOSITE main()
{
	VS_OUTPUT_CORTANA_COMPOSITE output;
	output.position = dot(float4(1, 2, 3, 4), chud_basis_0);
	return output;
}