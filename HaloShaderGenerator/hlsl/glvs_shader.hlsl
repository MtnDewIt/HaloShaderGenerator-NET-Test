//#include "registers\vertex_shader.hlsli"
#include "registers\vertex_shader.hlsli"
#include "helpers\input_output.hlsli"
#include "helpers\projection_math.hlsli"

// temporary definition, will have to macro the hell out of this
VS_OUTPUT_ALBEDO entry_albedo_rigid(VS_INPUT_RIGID_VERTEX_ALBEDO input)
{
    VS_OUTPUT_ALBEDO output;

	output.texcoord.xy = input.texcoord.xy * uv_compression_scale_offset.xy + uv_compression_scale_offset.zw;
	output.normal.xyz = transform_value(input.normal.xyz, nodes[0].xyz, nodes[1].xyz, nodes[2].xyz);
	float3 temp_binormal = transform_binormal(input.normal.xyz, input.tangent.xyz, input.binormal.xyz);
	output.binormal.xyz = transform_value(temp_binormal, nodes[0].xyz, nodes[1].xyz, nodes[2].xyz);
	output.tangent.xyz = transform_value(input.tangent.xyz, nodes[0].xyz, nodes[1].xyz, nodes[2].xyz);
	float4 vertex_position = input.position * position_compression_scale + position_compression_offset; //model space
	vertex_position.w = 1.0;
	vertex_position.x = dot(vertex_position, nodes[0]);
	vertex_position.y = dot(vertex_position, nodes[1]);
	vertex_position.z = dot(vertex_position, nodes[2]);
	output.camera_dir = camera_position - vertex_position.xyz;
	vertex_position = mul(vertex_position, view_projection);
	vertex_position.z = v_squish_params.w * (v_squish_params.z * (v_squish_params.x * vertex_position.w - v_squish_params.y) - vertex_position.z) + vertex_position.z;
	output.position = vertex_position.xyzw;
	output.normal.w = vertex_position.w;
	return output;
}
// TODO: verify compiled output for v_squish_params