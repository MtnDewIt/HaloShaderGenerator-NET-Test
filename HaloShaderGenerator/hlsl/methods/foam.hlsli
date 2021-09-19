#ifndef _HLSLI_FOAM
#define _HLSLI_FOAM

#include "..\registers\water_parameters.hlsli"
#include "..\helpers\types.hlsli"

uniform sampler2D foam_texture;
uniform float4 foam_texture_xform;
uniform sampler2D foam_texture_detail;
uniform float4 foam_texture_detail_xform;
uniform float foam_height;
uniform float foam_pow;

#define FOAM_THRESHOLD 0.002f

float4 calc_foam_none_ps(float3 lightmap_color, float2 texcoord, float2 shape_tex, float2 height, float ripple_foam)
{
    float foam_mag = max(ripple_foam, 0.0f);
    
    float4 foam_color;
    if (foam_mag > FOAM_THRESHOLD)
    {
        float4 foam_sample = tex2D(foam_texture, apply_xform2d(texcoord, foam_texture_xform));
        float4 foam_detail_sample = tex2D(foam_texture_detail, apply_xform2d(texcoord, foam_texture_detail_xform));
        foam_color = foam_sample * foam_detail_sample;
        foam_color.a *= foam_mag;
    }
    else
    {
        foam_color.rgb = 0;
        foam_color.a = foam_mag;
    }
    
    foam_color.rgb *= lightmap_color;
    return foam_color;
}

float4 calc_foam_auto_ps(float3 lightmap_color, float2 texcoord, float2 shape_tex, float2 height, float ripple_foam)
{
    float2 foam_h = height - foam_height;
    float auto_foam = saturate(foam_h.x / saturate(foam_h.y));
    auto_foam = pow(auto_foam, foam_pow);
    float foam_mag = max(ripple_foam, auto_foam);
    
    float4 foam_color;
    if (foam_mag > FOAM_THRESHOLD)
    {
        float4 foam_sample = tex2D(foam_texture, apply_xform2d(texcoord, foam_texture_xform));
        float4 foam_detail_sample = tex2D(foam_texture_detail, apply_xform2d(texcoord, foam_texture_detail_xform));
        foam_color = foam_sample * foam_detail_sample;
        foam_color.a *= foam_mag;
    }
    else
    {
        foam_color.rgb = 0;
        foam_color.a = foam_mag;
    }
    
    foam_color.rgb *= lightmap_color;
    return foam_color;
}

float4 calc_foam_paint_ps(float3 lightmap_color, float2 texcoord, float2 shape_tex, float2 height, float ripple_foam)
{
    float painted_foam = tex2D(global_shape_texture, shape_tex).z;
    float foam_mag = max(ripple_foam, painted_foam);
    
    float4 foam_color;
    if (foam_mag > FOAM_THRESHOLD)
    {
        float4 foam_sample = tex2D(foam_texture, apply_xform2d(texcoord, foam_texture_xform));
        float4 foam_detail_sample = tex2D(foam_texture_detail, apply_xform2d(texcoord, foam_texture_detail_xform));
        foam_color = foam_sample * foam_detail_sample;
        foam_color.a *= foam_mag;
    }
    else
    {
        foam_color.rgb = 0;
        foam_color.a = foam_mag;
    }
    
    foam_color.rgb *= lightmap_color;
    return foam_color;
}

float4 calc_foam_both_ps(float3 lightmap_color, float2 texcoord, float2 shape_tex, float2 height, float ripple_foam)
{
    float2 foam_h = height - foam_height;
    float auto_foam = saturate(foam_h.x / saturate(foam_h.y));
    auto_foam = pow(auto_foam, foam_pow);
    float painted_foam = tex2D(global_shape_texture, shape_tex).z;
    float foam_mag = max(ripple_foam, max(auto_foam, painted_foam));
    
    float4 foam_color;
    if (foam_mag > FOAM_THRESHOLD)
    {
        float4 foam_sample = tex2D(foam_texture, apply_xform2d(texcoord, foam_texture_xform));
        float4 foam_detail_sample = tex2D(foam_texture_detail, apply_xform2d(texcoord, foam_texture_detail_xform));
        foam_color = foam_sample * foam_detail_sample;
        foam_color.a *= foam_mag;
    }
    else
    {
        foam_color.rgb = 0;
        foam_color.a = foam_mag;
    }
    
    foam_color.rgb *= lightmap_color;
    return foam_color;
}

#ifndef calc_foam_ps
#define calc_foam_ps calc_foam_none_ps
#endif

#endif
