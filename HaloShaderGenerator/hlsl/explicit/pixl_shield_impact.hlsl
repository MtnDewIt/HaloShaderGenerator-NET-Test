#ifndef _PIXL_SHIELD_IMPACT_HLSLI
#define _PIXL_SHIELD_IMPACT_HLSLI

#include "..\helpers\explicit_input_output.hlsli"

uniform float4 impact0_color : register(c100);
uniform float4 plasma_offsets : register(c102);
uniform float4 edge_glow : register(c103);
uniform float4 plasma_color : register(c104);
uniform float4 plasma_edge_color : register(c105);
uniform float4 edge_scales : register(c106);
uniform float4 edge_offsets : register(c107);
uniform float4 plasma_scales : register(c108);
uniform float4 depth_fade_params : register(c109);

uniform sampler2D shield_impact_noise_texture1;
uniform sampler2D shield_impact_noise_texture2;
uniform sampler2D shield_impact_depth_texture;

#include "..\registers\global_parameters.hlsli"
#include "..\helpers\color_processing.hlsli"

PS_OUTPUT_DEFAULT main(VS_OUTPUT_SHIELD_IMPACT input) : COLOR
{
    float2 pixel_pos = 0.5f + input.position.xy;
    float2 inv_texture_size = rcp(texture_size.xy);
    pixel_pos *= inv_texture_size;
    float4 shit_depth_sample = tex2D(shield_impact_depth_texture, pixel_pos);
    
    float2 depth_fade_val = (shit_depth_sample.r - input.v1.z) * depth_fade_params.xy;
    depth_fade_val = saturate(depth_fade_val);
    
    float4 scales = input.v0.w * edge_scales + edge_offsets;

    float u0 = min(scales.x, scales.y);
    u0 = pow(saturate(u0), 4) * depth_fade_val.x;
    
    float u1 = min(scales.z, scales.w);
    u1 = saturate(u1) * depth_fade_val.y;

    float2 plasma_noise_texcoord_1 = apply_xform2d(input.v1.xy, float4(plasma_scales.xx, plasma_offsets.xy));
    float4 plasma_noise_sample_1 = tex2D(shield_impact_noise_texture1, plasma_noise_texcoord_1);
    float2 plasma_noise_texcoord_2 = apply_xform2d(input.v1.xy, float4(plasma_scales.yy, -plasma_offsets.zw));
    float4 plasma_noise_sample_2 = tex2D(shield_impact_noise_texture2, plasma_noise_texcoord_2);
    
    float plasma_noise = 1.0f - abs(plasma_noise_sample_1.r - plasma_noise_sample_2.r);
    plasma_noise = pow(saturate(plasma_noise), u1 * plasma_scales.z + plasma_scales.w);

    float4 color = plasma_edge_color * u1;
    color += plasma_color;
    color += impact0_color * saturate(1.0f - input.v1.w);
    color = u0 * edge_glow + color * plasma_noise;
    
    color.rgb = expose_color(color.rgb);
    
    PS_OUTPUT_DEFAULT output;
    
    output.low_frequency = export_low_frequency(color);
    output.high_frequency = export_high_frequency(color);
    output.unknown = 0;
    
    return output;
}

#endif