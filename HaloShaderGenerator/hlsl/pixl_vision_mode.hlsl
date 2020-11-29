#ifndef _PIXL_VISION_MODE_HLSLI
#define _PIXL_VISION_MODE_HLSLI

#include "helpers\explicit_input_output.hlsli"

uniform float4 g_exposure : register(c0);
uniform float4 falloff : register(c94);
uniform float4x4 screen_to_world : register(c95);
uniform float4 ping : register(c99);
uniform float4 colors[14] : register(c100);
uniform float overlapping_dimming_factor[5] : register(c114);
uniform float4 falloff_throught_the_wall : register(c120);
uniform float4 zbuf_params : register(c121);
uniform float float_index : register(c122);

uniform sampler2D depth_sampler : register(s0);
uniform sampler2D mask_sampler : register(s2);
uniform sampler2D depth_through_walls_sampler : register(s3);

float4 main(VS_OUTPUT_DEFAULT input) : COLOR
{
    float2 texcoord = input.texcoord;
    
    float4 depth_sample = tex2D(depth_sampler, texcoord);
    
    float depth = zbuf_params.x / depth_sample.x + zbuf_params.y;
    float reverse_depth = 1.0f - depth;
    
    //this is some operation with the 4x4 matrix, determine later
    
    float4 transform = screen_to_world[1] * texcoord.y;
    transform += screen_to_world[0] * texcoord.x;

    float4 transform_depth = reverse_depth * screen_to_world[2] + transform;
    transform_depth += screen_to_world[3];
    
    float3 u_0 = transform_depth.xyz / transform_depth.w;
    float dot_u_0 = dot(u_0, u_0);
    dot_u_0 = rcp(rsqrt(dot_u_0));

    float2 ping_xy = ping.xy - dot_u_0;
    
    dot_u_0 *= falloff.x;
    dot_u_0 = exp2(-pow(dot_u_0, 4));
    
    float ping_falloff = pow(saturate(ping.z * ping_xy.x + 1.0f), 3);
    
    float frac_float_index = frac(float_index);
    
    
    float cmp_val = -frac_float_index >= 0 ? 1.0f : 0.0f;
    frac_float_index = float_index - frac_float_index;
    cmp_val = float_index >= 0 ? 0.0f : cmp_val;
    frac_float_index += cmp_val;
    
    float4 r4 = frac_float_index + float4(-0, -1, -2, -3);
    float4 r5 = frac_float_index + float4(-3, -4, -5, -6);
    frac_float_index += -3;

    float3 color_result = 0;
    if (-abs(r4.x) >= 0)
        color_result = colors[1].rgb;
    if (-abs(r4.y) >= 0)
        color_result = colors[3].rgb;
    if (-abs(r4.z) >= 0)
        color_result = colors[5].rgb;
    if (-abs(r5.x) >= 0)
        color_result = colors[7].rgb;
    if (-abs(r5.y) >= 0)
        color_result = colors[9].rgb;
    if (-abs(r5.z) >= 0)
        color_result = colors[11].rgb;
    if (-abs(r5.w) >= 0)
        color_result = colors[13].rgb;

    color_result *= ping_falloff;

    if (ping_xy.x < 0)
        color_result = 0;
    
    float r0_w = 1.0f;
    if (ping_xy.y < 0)
        r0_w = 0;

    float3 color_result_2 = 0;
    if (-abs(r4.x) >= 0)
        color_result_2 = colors[0].rgb;
    if (-abs(r4.y) >= 0)
        color_result_2 = colors[2].rgb;
    if (-abs(r4.z) >= 0)
        color_result_2 = colors[4].rgb;
    if (-abs(r5.x) >= 0)
        color_result_2 = colors[6].rgb;
    if (-abs(r5.y) >= 0)
        color_result_2 = colors[8].rgb;
    if (-abs(r5.z) >= 0)
        color_result_2 = colors[10].rgb;
    if (-abs(r5.w) >= 0)
        color_result_2 = colors[12].rgb;
    
    color_result += color_result_2;
    
    float overlap_dimming = 0;
    if (-abs(r4.x) >= 0)
        overlap_dimming = overlapping_dimming_factor[0];
    if (-abs(r4.y) >= 0)
        overlap_dimming = overlapping_dimming_factor[1];
    if (-abs(r4.z) >= 0)
        overlap_dimming = overlapping_dimming_factor[2];
    if (-abs(r4.w) >= 0)
        overlap_dimming = overlapping_dimming_factor[3];
    if (-abs(frac_float_index) >= 0)
        overlap_dimming = overlapping_dimming_factor[4];
    
    float2 zbuf_texcoord1 = zbuf_params.xw * 2 + texcoord;
    float4 depth_sample1 = tex2D(depth_sampler, float2(zbuf_texcoord1.x, texcoord.y));
    float depth1 = 1.0f - (zbuf_params.x / depth_sample1.x + zbuf_params.y);
    
    float2 zbuf_texcoord2 = zbuf_params.xw * -2 + texcoord;
    float4 depth_sample2 = tex2D(depth_sampler, float2(zbuf_texcoord2.x, texcoord.y));
    float depth2 = zbuf_params.x / depth_sample2.x + zbuf_params.y;

    depth1 -= depth2;
    depth1 += 1.0f;
    depth1 += reverse_depth * -2;
    
    

}

#endif