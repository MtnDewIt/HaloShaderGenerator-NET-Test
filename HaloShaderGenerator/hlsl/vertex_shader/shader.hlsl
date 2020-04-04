#define shader_template

#include "registers/vertex_shader.hlsli"
#include "../helpers/input_output.hlsli"
#include "helpers/projection_math.hlsli"

// temporary definition, will have to macro the hell out of this
VS_OUTPUT_ALBEDO global_entry_rigid_albedo(VS_INPUT_RIGID_VERTEX_ALBEDO input)
{
    VS_OUTPUT_ALBEDO output;

	output.tangent = transform_value(input.tangent.xyz, nodes[0], nodes[1], nodes[2]);
	output.normal = transform_value(input.normal.xyz, nodes[0], nodes[1], nodes[2]);
	output.binormal = transform_value(input.binormal.xyz, nodes[0], nodes[1], nodes[2]);
	output.texcoord.xy = input.texcoord.xy * uv_compression_scale_offset.xy + uv_compression_scale_offset.zw;

    // TODO: add remaining code
	return output;
}