#include "registers\vertex_shader.hlsli"
#include "helpers\input_output.hlsli"
#include "helpers\transform_math.hlsli"
#include "helpers\math.hlsli"
#include "helpers\atmosphere.hlsli"
#include "helpers\prt.hlsli"
#include "vertices\vertices.hlsli"
#include "helpers\sfx_distortion.hlsli"
#include "helpers\vertex_shader_helper.hlsli"

// Cortana parameters
uniform float noise_amount;
uniform sampler2D fade_noise_map;
uniform float4 fade_noise_map_xform;
uniform float fade_offset;
// New parameter (for ss sampling)
uniform float4 screen_constants; // _render_method_extern_screen_constants

VS_OUTPUT_ALBEDO entry_albedo(input_vertex_format input)
{
    VS_OUTPUT_ALBEDO output;
    float4 world_position;
	calc_vertex_transform(input, world_position, output.position, output.normal.xyz, output.tangent, output.binormal, output.texcoord);
    output.camera_dir = Camera_Position - world_position.xyz;
    output.normal.w = output.position.w;
    return output;
}

VS_OUTPUT_STATIC_SH entry_static_sh(input_vertex_format input)
{
    VS_OUTPUT_STATIC_SH output;
    float4 world_position;
	
	calc_vertex_transform(input, world_position, output.position, output.normal, output.tangent, output.binormal, output.texcoord.xy);
	
    output.camera_dir = Camera_Position - world_position.xyz;
    calculate_atmosphere_radiance(world_position, output.camera_dir, output.extinction_factor.rgb, output.sky_radiance.rgb);

    float3 light_dir = normalize(v_lighting_constant_1.xyz + v_lighting_constant_2.xyz + v_lighting_constant_3.xyz);
    output.texcoord.z = dot(output.normal, -light_dir);
	
    return output;
}

VS_OUTPUT_CORTANA_PRT entry_active_camo(input_vertex_format input, AMBIENT_PRT input_prt)
{
    VS_OUTPUT_CORTANA_PRT output;
    float4 world_position;
    float3 normal, tangent, binormal, camera_dir;
    
	calc_vertex_transform(input, world_position, output.position, output.normal, output.tangent, output.binormal, output.texcoord.xy);
    
    output.camera_dir = Camera_Position - world_position.xyz;
	
    float3 unknown_pos = input.position.xyz - 0.5;
    unknown_pos.xy *= Position_Compression_Scale.xy;
	
    output.camo_param.x = dot(input.normal.xyz, Camera_Left);
    output.camo_param.y = dot(input.normal.xyz, Camera_Up);
    output.camo_param.z = atan2(unknown_pos.x, unknown_pos.y);
    output.camo_param.w = acos(unknown_pos.z) * (Position_Compression_Scale.z / length(Position_Compression_Scale.xy));
    
    output.prt_radiance_vector = calculate_ambient_radiance_vector(input_prt.coefficient, output.normal);

    calculate_atmosphere_radiance(world_position, output.camera_dir, output.extinction_factor.rgb, output.sky_radiance.rgb);
    
    float2 ss_scale = float2(1280.0f, 720.0f) * screen_constants.xy;
    float2 ss_tex = (output.position.xy * ss_scale) * fade_noise_map_xform.xy + fade_noise_map_xform.zw;
    float vertex_fade = 1.0f;
    vertex_fade = tex2Dlod(fade_noise_map, float4(ss_tex, 0, 0)).x;
    float ranged_noise = 2.0f * noise_amount * vertex_fade - noise_amount;
    output.sky_radiance.w = ranged_noise + fade_offset;
	
    output.texcoord.w = length(input.position.xyz - Camera_Position);
    output.texcoord.z = 0;
	
    return output;
}

VS_OUTPUT_CORTANA_PRT entry_static_prt_ambient(input_vertex_format input, AMBIENT_PRT input_prt)
{
    return entry_active_camo(input, input_prt);
}
