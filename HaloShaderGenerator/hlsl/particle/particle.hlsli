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
    float4 color = particle_albedo(input.texcoord, input.parameters.yzw, input.parameters.x, input.normal.w);
    
    if (depth_fade_arg == k_depth_fade_on)
    {
        float4 depth_sample = sample_depth_buffer(input.position.xy);
        color.a *= depth_fade_on(depth_sample.x, input.color2.w);
    }
    if (black_point_arg == k_black_point_on)
    {
        black_point_on(color.a, input.parameters.y);
    }
    
    color *= input.color;
    
    if (lighting_arg == k_lighting_per_pixel_ravi_order_3)
    {
        float3 normal = normalize(input.normal.xyz);
        per_pixel_ravi_order_3(normal, color);
    }
    
    color.rgb += input.color2.rgb;
    
    if (particle_blend_type_arg == k_particle_blend_mode_pre_multiplied_alpha)
        color.rgb *= color.a;
    
    return color;
}

float4 particle_entry_default_distortion(VS_OUTPUT_PARTICLE input)
{
    // The fixes here still need more work - the pixel computation is wrong when resolution is different from window resolution
    
    float4 depth_sample = sample_depth_buffer_distortion(input.position.xy);
    
    float4 color = particle_albedo(input.texcoord, input.parameters.yzw, input.parameters.x, input.normal.w);
    
    if (specialized_rendering_arg == k_specialized_rendering_distortion || !APPLY_HLSL_FIXES && specialized_rendering_arg == k_specialized_rendering_distortion_expensive)
        color.xy = color.xy * 2.00787401 + -1.00787401; // range conversion
    color.z = -color.y;
    
    float depth_fade = 1.0f;
    if (depth_fade_arg == k_depth_fade_on)
    {
        depth_fade = depth_fade_on(depth_sample.x, input.color2.w);
    }
    
    float2 screen_depth = (color.xz * screen_constants.z * input.color.w);
    screen_depth *= depth_fade;
    
    float2 pixel_change = mul(float2x2(input.color2.xy, input.normal.xy), screen_depth.xy);
    pixel_change /= input.color2.w;
    float depth_val = dot(pixel_change, pixel_change);
    float2 distort_val = distortion_scale.x * pixel_change.xy;
    clip(depth_val == 0 ? -1 : 1);
    
    if (specialized_rendering_arg == k_specialized_rendering_distortion_expensive || specialized_rendering_arg == k_specialized_rendering_distortion_expensive_diffuse)
    {
        float2 _zw;
        if (!APPLY_HLSL_FIXES)
            _zw = distort_val * 0.000488296151 + input.position.xy;
        else
            _zw = distort_val * 0.015625f + input.position.xy;

        float depth_sample_expensive = sample_depth_buffer_distortion(_zw).x;
        float depth_sat = calc_depth_fade(depth_sample_expensive, input.color2.w);
    
        clip(-depth_sat.x < 0 ? 1 : -1);
    }
    
    distort_val *= screen_constants.xy;
    
    if (!APPLY_HLSL_FIXES)
        distort_val *= 0.0312509537f;
    else
        distort_val *= 0.015625f;
    
    return float4(distort_val.x, distort_val.y, 0, 0);
}

#endif