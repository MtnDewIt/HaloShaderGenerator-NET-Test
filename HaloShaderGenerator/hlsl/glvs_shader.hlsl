#include "registers\vertex_shader.hlsli"
#include "helpers\input_output.hlsli"
#include "helpers\transform_math.hlsli"

VS_OUTPUT_ALBEDO entry_albedo_rigid(VS_INPUT_RIGID_VERTEX_ALBEDO input)
{
    VS_OUTPUT_ALBEDO output;

	float3x3 node_transformation = float3x3(nodes[0].xyz, nodes[1].xyz, nodes[2].xyz);
	float4x4 v_node_transformation = float4x4(nodes[0], nodes[1], nodes[2], float4(0,0,0,0));
	
	output.texcoord.xy = calculate_texcoord(input.texcoord);
	output.normal.xyz = transform_vector(input.normal.xyz, node_transformation);
	output.binormal.xyz = transform_vector(transform_binormal(input.normal.xyz, input.tangent.xyz, input.binormal.xyz), node_transformation);
	output.tangent.xyz = transform_vector(input.tangent.xyz, node_transformation);
	float4 vertex_position = float4(decompress_vertex_position(input.position.xyz), 1.0);
	vertex_position.xyz = mul(v_node_transformation, vertex_position.xyzw).xyz;
	float4 screen_position = calculate_screenspace_position(vertex_position);
	output.camera_dir = camera_position - vertex_position.xyz;
	output.position = screen_position.xyzw;
	output.normal.w = screen_position.w;	
	return output;
}