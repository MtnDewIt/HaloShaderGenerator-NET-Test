#include "registers\vertex_shader.hlsli"
#include "helpers\input_output.hlsli"
#include "helpers\transform_math.hlsli"
#include "helpers\math.hlsli"
#include "vertices\vertices.hlsli"
#include "helpers\vertex_shader_helper.hlsli"

uniform bool pixel_kill_enabled : register(b2);
uniform float4 sprite : register(c228);
uniform float u_tiles;
uniform float v_tiles;

#if vertextype == k_vertextype_world || vertextype == k_vertextype_rigid || vertextype == k_vertextype_skinned
VS_OUTPUT_DECAL 
#else
VS_OUTPUT_DECAL_FLAT
#endif
entry_default(input_vertex_format input)
{    
#if vertextype == k_vertextype_world || vertextype == k_vertextype_rigid || vertextype == k_vertextype_skinned
    VS_OUTPUT_DECAL output;
#else
VS_OUTPUT_DECAL_FLAT output;
#endif
    
    float4 world_position;
    float3 normal;
    float3 tangent;
    float3 binormal;
    calc_vertex_transform(input, world_position, output.position, normal, tangent, binormal, output.texcoord.xy);
    
#if vertextype == k_vertextype_world || vertextype == k_vertextype_rigid || vertextype == k_vertextype_skinned
    output.normal = normal;
    output.tangent = tangent;
    output.binormal = binormal;
#endif
    
    if (pixel_kill_enabled)
    {
        output.texcoord.xy = output.texcoord.xy;
        output.texcoord.zw = output.texcoord.xy * sprite.zw + sprite.xy;
    }
    else
    {
        output.texcoord.xy = 0.5f;
        output.texcoord.zw = output.texcoord.xy * sprite.zw + sprite.xy;
    }
    
    output.texcoord.z *= u_tiles;
    output.texcoord.w *= v_tiles;
    calculate_z_squish(output.position);
    output.depth.x = output.position.w;
    return output;
}