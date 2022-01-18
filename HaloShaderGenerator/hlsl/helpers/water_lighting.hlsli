#ifndef _HLSLI_WATER_LIGHTING
#define _HLSLI_WATER_LIGHTING

#include "../helpers/math.hlsli"
#include "../registers/water_parameters.hlsli"
#include "../registers/global_parameters.hlsli"

//float3 water_get_lightprobe_color_reach(float2 tex, float3 normal)
//{
//    float4 sample0 = tex3D(lightprobe_texture_array, float3(tex, 0.0625f));
//    float4 sample1 = tex3D(lightprobe_texture_array, float3(tex, 0.1875f));
//
//    float4 dominant_light_intensity = tex3D(dominant_light_intensity_map, float3(tex, 0.25f));
//    float exponent = log2(1.0f / 512.0f) * dominant_light_intensity.r;
//
//    float3 color = (sample1.rgb * 2.0f + sample0.rgb) - 1.0f;
//    color *= 42.33554; //c74.x; // probably compression constant
//    color *= exp2(exponent);
//    color += dominant_light_intensity.a;
//
//    return color;
//}

float3 water_sample_lightprobe_array(float2 tex, float3 normal)
{
#if reach_compatibility_arg == k_reach_compatibility_disabled
    if (k_is_lightmap_exist)
    {
        float4 sample0 = tex3D(lightprobe_texture_array, float3(tex, 0.0625f));
        float4 sample1 = tex3D(lightprobe_texture_array, float3(tex, 0.1875f));
        float3 color = sample0.xyz + sample1.xyz;
        float a = sample0.w * sample1.w * p_lightmap_compress_constant_0.x;
        color = color * 2.0f - 2.0f;
        return saturate(a * color);
    }
#endif
    
    return 1.0f;
}

#endif
