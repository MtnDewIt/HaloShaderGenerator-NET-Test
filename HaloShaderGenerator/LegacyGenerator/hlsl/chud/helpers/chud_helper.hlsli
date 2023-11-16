#ifndef _CHUD_HELPER_HLSLI
#define _CHUD_HELPER_HLSLI

#include "chud_global_parameters.hlsli"

void export_chud_alpha(inout float alpha)
{
    alpha *= g_hud_alpha;
    
    if (!chud_cortana_pixel)
    {
        alpha *= g_exposure.w;
    }
}

// interpolate between two colors (0 color1, 1.0 color2)
float3 apply_color_selector(float3 color1, float3 color2, float scalar)
{
    return color1 * (1.0f - scalar) + color2 * scalar;
}

// replicated x360 functionality
float4 sample_texquad_gradient(in sampler2D tex, in float2 texcoord)
{
    float2 resolution_scale = (1.0f / pixel_size.xy) / float2(1280, 720);
    
    float2 dir = (float2(ddx(texcoord.x), ddy(texcoord.y)) / 4.5f) * resolution_scale;
    
    float4 d1 = tex2D(tex, float2( 1,  0) * dir + texcoord);
    float4 d2 = tex2D(tex, float2(-1,  0) * dir + texcoord);
    float4 d3 = tex2D(tex, float2( 0,  1) * dir + texcoord);
    float4 d4 = tex2D(tex, float2( 0, -1) * dir + texcoord);

    return (d1 + d2 + d3 + d4) / 4.0f;
}

// test, kinda worked but needs full scene

float4 sample_texquad_sharpen(in sampler2D tex, in float2 texcoord)
{
    float4 d0 = tex2D(tex, float2( 0,  0) * basemap_pixel_size.xy + texcoord);    
    float4 d1 = tex2D(tex, float2( 1,  1) * basemap_pixel_size.xy + texcoord);
    float4 d2 = tex2D(tex, float2(-1, -1) * basemap_pixel_size.xy + texcoord);
    float4 d3 = tex2D(tex, float2(-1,  1) * basemap_pixel_size.xy + texcoord);
    float4 d4 = tex2D(tex, float2( 1, -1) * basemap_pixel_size.xy + texcoord);

    float4 quad_comb = (d1 + d2 + d3 + d4) / 4.0f;
    
    float sharpness = 2.0f;
    return d0 + (d0 - quad_comb) * sharpness;
}

float4 sample_tex_quincunx_antialiasing(in sampler2D tex, in float2 texcoord)
{
    float4 d0 = tex2D(tex, float2( 0,  0) * basemap_pixel_size.xy + texcoord) / 2.0f;
    float4 d1 = tex2D(tex, float2( 1,  1) * basemap_pixel_size.xy + texcoord) / 8.0f;
    float4 d2 = tex2D(tex, float2(-1, -1) * basemap_pixel_size.xy + texcoord) / 8.0f;
    float4 d3 = tex2D(tex, float2(-1,  1) * basemap_pixel_size.xy + texcoord) / 8.0f;
    float4 d4 = tex2D(tex, float2( 1, -1) * basemap_pixel_size.xy + texcoord) / 8.0f;
    
    return d0 + d1 + d2 + d3 + d4;
}

#endif
