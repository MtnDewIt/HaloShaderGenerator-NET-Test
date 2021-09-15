#ifndef _HLSLI_WATER_LIGHTING
#define _HLSLI_WATER_LIGHTING

#include "../helpers/math.hlsli"
#include "../registers/water_parameters.hlsli"
#include "../registers/global_parameters.hlsli"

float3 water_get_lightprobe_band0_color(float2 tex)
{
    if (k_is_lightmap_exist)
    {
        float4 sample0 = tex3D(lightprobe_texture_array, float3(tex, 0.0625f));
        float4 sample1 = tex3D(lightprobe_texture_array, float3(tex, 0.1875f));
        float3 color = sample0.xyz + sample1.xyz;
        float a = sample0.w * sample1.w * p_lightmap_compress_constant_0.x;
        color = color * 2.0f - 2.0f;
        return saturate(a * color);
    }
    
    return 1.0f;
}

#endif
