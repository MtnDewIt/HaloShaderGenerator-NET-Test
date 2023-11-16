#ifndef _PARTICLE_SHADER_HLSL
#define _PARTICLE_SHADER_HLSL

#include "..\methods\depth_fade.hlsli"
#include "..\particle_lighting\ravi_order_3.hlsli"
#include "..\registers\global_parameters.hlsli"

uniform float distortion_scale;

#include "..\methods\albedo_fx.hlsli"
#include "..\methods\black_point.hlsli"
#include "..\methods\fog.hlsli"

//#include "..\helpers\particle_helper.hlsli"
#include "..\helpers\apply_hlsl_fixes.hlsli"
#include "..\helpers\bumpmap_math.hlsli"

uniform bool is_normal_basemap : register(b14); // h3 bubbleshield diffuse is not a normal map...

float2 get_x360_z_buffer_coord(in float2 pos, bool distortion = false)
{
#if APPLY_HLSL_FIXES == 0
    if (distortion)
        return (pos * 2.0f + 0.5f) / texture_size.xy;
#endif
    return (pos + 0.5f) / texture_size.xy;
}

float4 particle_entry_default_main(VS_OUTPUT_PARTICLE input)
{
    float3 normal = normalize(input.parameters3.xyz);
    float palette_v_coord = input.parameters3.w;
    float frame_blend_interpolator = input.parameters2.x;
    float black_point = input.parameters2.y;
    float2 billboard_texcoord = input.parameters2.zw;
    float3 add_color = input.parameters.rgb;
    float input_depth = input.parameters.w;
    
    float depth_fade = 1.0f;
    if (albedo_arg == k_albedo_palettized_2d_plasma && depth_fade_arg == k_depth_fade_on)
    {
        float2 frag_coord = get_x360_z_buffer_coord(input.position.xy);
        depth_fade = calc_depth_fade(frag_coord, input_depth, true);
    }
    
    float4 color = calc_albedo_ps(input.texcoord, billboard_texcoord, palette_v_coord, frame_blend_interpolator, input.color.a, depth_fade);
    
    if (depth_fade_arg == k_depth_fade_on)
    {
        if (albedo_arg != k_albedo_palettized_2d_plasma) // This is here so we can have 1-1 compile :)
        {
            float2 frag_coord = get_x360_z_buffer_coord(input.position.xy);
            depth_fade = calc_depth_fade(frag_coord, input_depth, true);
        }
        color.a *= depth_fade;
    }
    
    if (black_point_arg == k_black_point_on)
    {
        black_point_on(color.a, black_point);
    }
    
    if (blend_type_arg == k_blend_mode_multiply)
    {
        color *= input.color;
        color.rgb -= 1.0f;
        color.rgb = color.a * color.rgb + 1.0f;
    }
    else
    {
        color *= input.color;
    }
    
    if (lighting_arg == k_lighting_per_pixel_ravi_order_3)
    {
        per_pixel_ravi_order_3(normal, color);
    }
    
    if (blend_type_arg != k_blend_mode_multiply)
    {
        color.rgb += add_color;
        
        if (blend_type_arg == k_blend_mode_pre_multiplied_alpha)
            color.rgb *= color.a;
    }
    
    return color;
}

#define DISTORTION_EXPENSIVE specialized_rendering_arg == k_specialized_rendering_distortion_expensive || specialized_rendering_arg == k_specialized_rendering_distortion_expensive_diffuse

float4 particle_entry_default_distortion(VS_OUTPUT_PARTICLE input)
{
    float2 binormal = input.parameters3.xy;
    float palette_v_coord = input.parameters3.w;
    float frame_blend_interpolator = input.parameters2.x;
    float black_point = input.parameters2.y;
    float2 billboard_texcoord = input.parameters2.zw;
    float2 tangent = input.parameters.xy;
    float input_depth = input.parameters.w;
    float2 frag_pos = get_x360_z_buffer_coord(input.position.xy, true);
    
    float depth_fade = calc_depth_fade(frag_pos, input_depth, true);
    
    float2 distortion_diffuse = calc_albedo_ps(input.texcoord, billboard_texcoord, palette_v_coord, frame_blend_interpolator, input.color.a, depth_fade).xy;
    
    [branch]
    if ((specialized_rendering_arg == k_specialized_rendering_distortion || specialized_rendering_arg == k_specialized_rendering_distortion_expensive) 
        && is_normal_basemap)
        unpack_dxn_to_signed(distortion_diffuse);
    
    distortion_diffuse.y = -distortion_diffuse.y;
    
    float2 screen_depth = (distortion_diffuse * screen_constants.z * input.color.a);
    screen_depth *= depth_fade;
    
    float2 pixel_change = mul(float2x2(tangent, binormal), screen_depth.xy);
    pixel_change /= input_depth;
    float depth_val = dot(pixel_change, pixel_change);
    float2 distort_val = distortion_scale.x * pixel_change.xy;
    clip(depth_val == 0 ? -1 : 1);
    
    float rt_b = x360_fudge_constant;
    if (!APPLY_HLSL_FIXES)
        rt_b = pc_fudge_constant;
    
    if (DISTORTION_EXPENSIVE)
    {
        float2 expensive_frag_pos = get_x360_z_buffer_coord(distort_val * rt_b + input.position.xy);
        float depth_fade_exp = calc_depth_fade(expensive_frag_pos, input_depth, false);
        clip(-depth_fade_exp < 0 ? 1 : -1);
    }
    
    distort_val *= screen_constants.xy;
    distort_val *= !APPLY_HLSL_FIXES ? pc_fudge_constant : x360_fudge_constant;
    
    return float4(distort_val, 0, 0);
}

#endif