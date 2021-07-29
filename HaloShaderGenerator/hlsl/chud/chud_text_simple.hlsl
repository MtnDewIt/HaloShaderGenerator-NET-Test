#include "helpers\\chud_input_output.hlsli"
#include "helpers\\chud_global_parameters.hlsli"
#include "helpers\\chud_helper.hlsli"

float4 ps_default(in s_chud_vertex_output input) : COLOR
{
    float4 tex_sample = tex2D(basemap_sampler, input.micro_texcoord);
    float alpha = tex_sample.a;
    
    float3 color = apply_color_selector(chud_color_output_A.rgb, chud_color_output_B.rgb, chud_scalar_output_ABCD.y);
    
    float2 f = tex_sample.gb;
    if (tex_sample.g - tex_sample.b < 0)
        f = tex_sample.bg;
    
    float r = 0.000199999995f - (max(tex_sample.r, f.x) - min(tex_sample.r, f.y));
    
    float3 final;
    if (r < 0)
        final = tex_sample.rgb;
    else
        final = color;
        
    export_chud_alpha(alpha);
    
    return float4(final, alpha);
}