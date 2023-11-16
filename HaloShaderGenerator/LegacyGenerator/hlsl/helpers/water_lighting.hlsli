#ifndef _HLSLI_WATER_LIGHTING
#define _HLSLI_WATER_LIGHTING

#include "../helpers/math.hlsli"
#include "../registers/water_parameters.hlsli"
#include "../registers/global_parameters.hlsli"

uniform float4 lightmap_tweak;

float3 water_sample_lightprobe_array(float2 tex, float3 normal)
{
//#if reach_compatibility_arg == k_reach_compatibility_enabled
//    if (k_is_lightmap_exist)
//    {
//        return water_sample_vmf_lightprobe(tex);
//    }
//#else
    if (k_is_lightmap_exist)
    {
        float4 sample0 = tex3D(lightprobe_texture_array, float3(tex, 0.0625f));
        float4 sample1 = tex3D(lightprobe_texture_array, float3(tex, 0.1875f));
        float3 color = sample0.xyz + sample1.xyz;
        float a = sample0.w * sample1.w * p_lightmap_compress_constant_0.x;
        color = color * 2.0f - 2.0f;

#if reach_compatibility_arg == k_reach_compatibility_enabled
        // Reach's water lighting is weird, this attempts to simulate it
        float scale = 1.0f;
        float sun_visibility = 1.0f;
        return saturate(a * color) * scale + sun_visibility;
#endif
        return saturate(a * color);
    }
//#endif
    
    return 1.0f;
}

#endif
