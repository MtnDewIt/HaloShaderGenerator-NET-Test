
//#define MS23_ORIGINAL
//#define VERTEX_SHADER

#ifdef VERTEX_SHADER

#include "registers\decorator_registers.hlsli"
#include "helpers\decorator_helper.hlsli"
#include "..\helpers\atmosphere.hlsli"
#include "..\helpers\math.hlsli"

VS_DECORATOR_DEFAULT_OUTPUT vs_default(in S_DECORATOR_VERTEX_INPUT input)
{
    VS_DECORATOR_DEFAULT_OUTPUT output;

    input.instance_position += 32767.0f;
    input.instance_position.xyz *= instance_compression_scale.xyz;
    input.instance_position.xyz += instance_compression_offset.xyz;

    float3 camera_to_instance = (input.instance_position.xyz - Camera_Position);
    output.lighting.a = saturate(sqrt(dot(camera_to_instance, camera_to_instance)) * LOD_constants.x + LOD_constants.y);
    
    if (output.lighting.a <= 0.3f)
    {
        output.position = 0.0f;
        output.texcoord = 0.0f;
        output.lighting.rgba = 0.0f;
        output.radiance = 0.0f;
        output.normal = 0.0f;
        return output;
    }
    
    input.instance_quaternion = input.instance_quaternion * 2.0f - 1.0f;

    float instance_movement_factor = (input.instance_position.w / 256) - floor(input.instance_position.w / 256);

    input.position.xyz = input.position.xyz * Position_Compression_Scale.xyz + Position_Compression_Offset.xyz;
    input.texcoord = input.texcoord.xy * UV_Compression_Scale_Offset.xy + UV_Compression_Scale_Offset.zw;
	
    float4 world_position;
    world_position.w = input.position.w;
    world_position.xyz = quat_transform_point(input.instance_quaternion, input.position.xyz) + input.instance_position.xyz;

    output.position = mul(float4(world_position.xyz, 1.0f), View_Projection);

    float3 world_normal = normalize(quat_transform_point(input.instance_quaternion, input.normal.xyz));

    output.texcoord.xy = input.texcoord;
    output.texcoord.zw = 0.0f;
    output.lighting.rgb = (input.instance_color.rgb * exp2(input.instance_color.a * 63.75f - 31.75f));
    
    float3 extinction;
    calculate_atmosphere_radiance_new(Camera_Position, world_position.xyz, extinction, output.radiance.xyz);
	
    output.lighting.rgb *= extinction;
	output.radiance.w = output.position.w;
    output.normal = world_normal;

    return output;
}

#else

#include "..\helpers\color_processing.hlsli"
#include "registers\decorator_registers.hlsli"
#include "helpers\decorator_helper.hlsli"

uniform sampler2D diffuse_texture : register(s0);

PS_OUTPUT_ALBEDO ps_default(in VS_DECORATOR_DEFAULT_OUTPUT input) : COLOR
{
    PS_OUTPUT_ALBEDO output;

    float4 light = input.lighting;

    float4 color = tex2D(diffuse_texture, input.texcoord.xy) * light + float4(input.radiance.rgb, 0.0f);

    color.rgb *= g_exposure.x;
    color.a *= 1.66666663f;

    color.rgb = rgb_to_srgb(color.rgb);

    output.diffuse = color;
    output.normal = float4(normal_export(input.normal), color.a);
    output.unknown = input.radiance.w;

    return output;
}

#endif
