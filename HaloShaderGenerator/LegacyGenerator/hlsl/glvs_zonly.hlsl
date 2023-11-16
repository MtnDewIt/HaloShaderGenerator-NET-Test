#include "registers\vertex_shader.hlsli"
#include "helpers\input_output.hlsli"
#include "vertices\vertices.hlsli"
#include "helpers\vertex_shader_helper.hlsli"

VS_OUTPUT_ZONLY entry_z_only(input_vertex_format input)
{
    VS_OUTPUT_ZONLY output;
    float4 world_position;
	calc_vertex_transform(input, world_position, output.position, output.normal_and_w.xyz, output.tangent, output.binormal, output.texcoord);
    calculate_z_squish(output.position);
    output.camera_dir = Camera_Position - world_position.xyz;
    output.normal_and_w.w = output.position.w;
    return output;
}
