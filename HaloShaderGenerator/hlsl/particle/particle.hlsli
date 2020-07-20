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

float4 particle_entry_default_main(VS_OUTPUT_PARTICLE input)
{
    float3 normal = normalize(input.normal.xyz);
    float4 depth_sample = sample_depth_buffer(input.position.xy);
    
    float4 color = particle_albedo(input.texcoord, input.parameters.x);
    
    if (depth_fade_arg == k_depth_fade_on)
    {
        color.a *= depth_fade_on(depth_sample.x, input.color2.w);
    }
    if (black_point_arg == k_black_point_on)
    {
        black_point_on(color.a, input.parameters.y);
    }
    
    color *= input.color;
    
    if (lighting_arg == k_lighting_per_pixel_ravi_order_3)
    {
        per_pixel_ravi_order_3(normal, color);
    }
    
    color.rgb += input.color2.rgb;
    
    if (particle_blend_type_arg == k_particle_blend_mode_pre_multiplied_alpha)
        color.rgb *= color.a;
    
    return color;
}

float4 particle_entry_default_distortion(VS_OUTPUT_PARTICLE input)
{
    float4 depth_sample = sample_depth_buffer_distortion(input.position.xy);
    //float4 depth_sample = sample_depth_buffer(input.position.xy); // temp fix
    
    float4 color = particle_albedo(input.texcoord, input.parameters.x);
    
    if (specialized_rendering_arg == k_specialized_rendering_distortion || specialized_rendering_arg == k_specialized_rendering_distortion_expensive)
        color.xy = color.xy * 2.00787401 + -1.00787401;
    color.z = -color.y;
    
    float depth_fade = 1.0f;
    if (depth_fade_arg == k_depth_fade_on)
    {
        depth_fade = depth_fade_on(depth_sample.x, input.color2.w);
    }
    
    float2 screen_depth = (color.xz * screen_constants.z * input.color.w);
    screen_depth *= depth_fade;
    
    float3 r0;
    r0.x = dot(input.color2.xy, screen_depth.xy);
    r0.y = dot(input.normal.xy, screen_depth.xy);
    
    r0.xy *= (1 / input.color2.w);
    r0.z = dot(r0.xy, r0.xy);
    
    r0.xy *= distortion_scale.x;
    float2 distortion_value = r0.xy * screen_constants.xy; // val /= game_resolution
    
    clip(-r0.z < 0 ? 1 : -1);
    
    if (specialized_rendering_arg == k_specialized_rendering_distortion_expensive || specialized_rendering_arg == k_specialized_rendering_distortion_expensive_diffuse)
    {
        float2 _zw = r0.xy * 0.000488296151 + input.position.xy;

        float4 depth_sample_expensive = sample_depth_buffer_distortion(_zw);
        //float4 depth_sample_expensive = sample_depth_buffer(_zw); // temp fix
    
        float depth_sat = saturate(depth_sample_expensive.x + -input.color2.w); // depth_fade code???
    
        clip(-depth_sat.x < 0 ? 1 : -1);
    }
    
    return float4(distortion_value.x * 0.0312509537, distortion_value.y * 0.0312509537, 0, 0);
}

#endif