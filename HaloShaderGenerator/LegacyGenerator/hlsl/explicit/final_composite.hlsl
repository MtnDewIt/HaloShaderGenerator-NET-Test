
#include "helpers\final_composite_helper.hlsli"

float4 ps_default(in float2 texcoord : TEXCOORD) : COLOR
{
    float3 surface_color = tex2D(surface_sampler, texcoord).rgb;
    float3 bloom_color = tex2D(bloom_sampler, texcoord * bloom_sampler_xform.xy + bloom_sampler_xform.zw).rgb;
    
    float4 color = float4(surface_color, 1.0f);
    color.rgb += bloom_color;
    
    apply_tone_and_huesat(color);
    grade_color(color);
    correct_gamma(color.rgb);
    
    return color;
}