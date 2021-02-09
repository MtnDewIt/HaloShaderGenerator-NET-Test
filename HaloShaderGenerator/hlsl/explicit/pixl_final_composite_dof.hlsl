
#include "helpers\final_composite_helper.hlsli"

float4 main(in float2 texcoord : TEXCOORD) : COLOR
{
    float dof_factor = get_dof_factor(texcoord);
    
    float3 blur_color = tex2D(blur_sampler, texcoord).rgb;
    float3 surface_color = tex2D(surface_sampler, texcoord).rgb;
    float3 bloom_color = tex2D(bloom_sampler, texcoord * bloom_sampler_xform.xy + bloom_sampler_xform.zw).rgb;
    
    float4 color = float4(lerp(surface_color, blur_color, dof_factor), 1.0f);
    color.rgb += bloom_color;
    
    apply_tone_and_huesat(color);
    grade_color(color);
    correct_gamma(color.rgb);
    
    return color;
}