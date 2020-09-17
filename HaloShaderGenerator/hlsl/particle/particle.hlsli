#ifndef _PARTICLE_SHADER_HLSL
#define _PARTICLE_SHADER_HLSL

#include "..\methods\depth_fade.hlsli"
#include "..\particle_lighting\ravi_order_3.hlsli"
#include "..\registers\global_parameters.hlsli"

uniform float distortion_scale;

#include "..\methods\albedo_particle.hlsli"
#include "..\methods\black_point.hlsli"
#include "..\methods\fog.hlsli"

#include "..\helpers\particle_helper.hlsli"
#include "..\helpers\particle_depth.hlsli"
#include "..\helpers\apply_hlsl_fixes.hlsli"

float4 particle_entry_default_main(VS_OUTPUT_PARTICLE input)
{
    float3 normal = normalize(input.parameters3.xyz);
    float palette_v_coord = input.parameters3.w;
    float frame_blend_interpolator = input.parameters2.x;
    float black_point = input.parameters2.y;
    float2 alpha_map_texcoord = input.parameters2.zw;
    float3 add_color = input.parameters.rgb;
    float input_depth = input.parameters.w;
    
    float4 color = particle_albedo(input.texcoord, alpha_map_texcoord, frame_blend_interpolator, palette_v_coord, input.color.a);
    
    if (depth_fade_arg == k_depth_fade_on)
    {
        float4 depth_sample = sample_depth_buffer(input.position.xy);
        color.a *= depth_fade_on(depth_sample.x, input_depth);
    }
    
    if (black_point_arg == k_black_point_on)
    {
        black_point_on(color.a, black_point);
    }
    
    if (particle_blend_type_arg == k_particle_blend_mode_multiply)
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
    
    if (particle_blend_type_arg != k_particle_blend_mode_multiply)
    {
        color.rgb += add_color;
        
        if (particle_blend_type_arg == k_particle_blend_mode_pre_multiplied_alpha)
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
    float2 alpha_map_texcoord = input.parameters2.zw;
    float2 tangent = input.parameters.xy;
    float input_depth = input.parameters.w;
    
    // TODO: fix distortion_expensive
    
    float4 depth_sample = sample_depth_buffer_distortion(input.position.xy);
    
    float2 distortion_diffuse = particle_albedo(input.texcoord, alpha_map_texcoord, frame_blend_interpolator, palette_v_coord, input.color.a).xy;
    
    if (specialized_rendering_arg == k_specialized_rendering_distortion || !APPLY_HLSL_FIXES && specialized_rendering_arg == k_specialized_rendering_distortion_expensive)
        distortion_diffuse = distortion_diffuse * 2.00787401 - 1.00787401; // range conversion
    
    distortion_diffuse.y = -distortion_diffuse.y;
    
    float depth_fade = 1.0f;
    if (depth_fade_arg == k_depth_fade_on)
    {
        depth_fade = depth_fade_on(depth_sample.x, input_depth);
    }
    
    float2 screen_depth = (distortion_diffuse * screen_constants.z * input.color.a);
    screen_depth *= depth_fade;
    
    float2 pixel_change = mul(float2x2(tangent, binormal), screen_depth.xy);
    pixel_change /= input_depth;
    float depth_val = dot(pixel_change, pixel_change);
    float2 distort_val = distortion_scale.x * pixel_change.xy;
    clip(depth_val == 0 ? -1 : 1);
    
    if (DISTORTION_EXPENSIVE)
    {
        float2 expensive_frag_pos;
        if (!APPLY_HLSL_FIXES)
            expensive_frag_pos = distort_val * 0.000488296151 + input.position.xy;
        else
            expensive_frag_pos = distort_val * 0.015625f + input.position.xy;

        float depth_sample_expensive = sample_depth_buffer_distortion(expensive_frag_pos).x;
        float depth_fade_exp = calc_depth_fade(depth_sample_expensive, input_depth);
    
        clip(-depth_fade_exp < 0 ? 1 : -1);
    }
    
    distort_val *= screen_constants.xy;
    
    if (!APPLY_HLSL_FIXES)
        distort_val *= 0.0312509537f;
    else
        distort_val *= 0.015625f;
    
    return float4(distort_val, 0, 0);
}

#endif