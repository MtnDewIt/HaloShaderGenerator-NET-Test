#ifndef _WATER_TEMPLATE_PER_PIXEL_LIGHTING_HLSLI
#define _WATER_TEMPLATE_PER_PIXEL_LIGHTING_HLSLI

#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\types.hlsli"

// these two are set in-engine, not routed in rmt2
uniform bool k_is_lightmap_exist : register(b0);
uniform bool k_is_water_interaction : register(b1);
uniform bool no_dynamic_lights;
uniform sampler2D tex_ripple_buffer_slope_height;

uniform float4 wave_displacement_array_xform;
uniform float time_warp_aux;

uniform float time_warp;
uniform float slope_range_x;
uniform float slope_range_y;
uniform float detail_slope_scale_x;
uniform float detail_slope_scale_y;
uniform float detail_slope_scale_z;
uniform float detail_slope_steepness;
uniform sampler2D wave_slope_array;
uniform float4 wave_slope_array_xform;

PS_OUTPUT_DEFAULT water_entry_static_per_pixel(VS_OUTPUT_WATER input)
{
    float2 texcoord = input.unknown_0.xy;
    
    float3 ripple = 0;
    if (k_is_water_interaction)
    {
        float4 ripple_tex = input.unknown_9.zwzz * float4(1, 1, 0, 0);
        float4 ripple_sample = tex2Dlod(tex_ripple_buffer_slope_height, ripple_tex);
        ripple.xy = (ripple_sample.gb - 0.5f) * 6.0f;
        ripple.z = ripple_sample.a;
    }
    
    float2 slope_range = float2(slope_range_x, slope_range_y);
    float steepness = min(max(ripple.y + ripple.x + 1.0f, 0.300000012f), 2.0999999f);
    
    float3 displacement;
    displacement.x = wave_displacement_array_xform.x * detail_slope_scale_x;
    displacement.y = wave_displacement_array_xform.y * detail_slope_scale_y;
    displacement.z = time_warp * detail_slope_scale_z;
    
    float2 slope_array_tex = texcoord * displacement.xy + wave_displacement_array_xform.zw;
    float2 slope_displacement = tex2D(wave_slope_array, slope_array_tex).rg;
    slope_displacement = (slope_displacement * 2.00787401f - 1.00787401f) * slope_range + (slope_range * -0.5f);
    slope_displacement *= (detail_slope_steepness / steepness);
    
    float r = input.unknown_3.w / steepness;
    
    float2 wave_slope = tex2D(wave_slope_array, apply_xform2d(texcoord, wave_slope_array_xform)).rg;
    wave_slope = (wave_slope * 2.00787401f - 1.00787401f) * slope_range + (slope_range * -0.5f);
    
    
    PS_OUTPUT_DEFAULT output;

    output.unknown = 0;
    return output;
}

#endif