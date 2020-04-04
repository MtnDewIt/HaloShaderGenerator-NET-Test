#define shader_template

#include "registers/vertex_shader.hlsli"
#include "../helpers/input_output.hlsli"


// temporary definition, will have to macro the hell out of this
VS_OUTPUT_ALBEDO global_entry_rigid_albedo(VS_INPUT_RIGID_VERTEX input)
{
    VS_OUTPUT_ALBEDO output;

    output.texcoord.xy = (input.texcoord.xy * uv_compression_scale_offset.xy) + uv_compression_scale_offset.zw;

    // TODO: add remaining code
}