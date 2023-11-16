#include "helpers\\chud_input_output.hlsli"
#include "helpers\\chud_global_parameters.hlsli"
#include "helpers\\chud_helper.hlsli"

float4 ps_default(in s_chud_vertex_output input) : COLOR
{
    float4 tex_sample = tex2D(basemap_sampler, input.micro_texcoord);
    float color_selector = tex_sample.g;
    float highlight = tex_sample.b;
    float alpha = tex_sample.a;

    float3 final;
    
    // compile issue
    //final = apply_color_selector(chud_color_output_B.rgb, chud_color_output_A.rgb, color_selector);
    final = chud_color_output_A.rgb * color_selector;
    final += chud_color_output_B.rgb * (1.0f - color_selector);
    
    final = apply_color_selector(final.rgb, chud_color_output_D.rgb, chud_scalar_output_ABCD.x);
    final += (chud_color_output_C.rgb * highlight);
    export_chud_alpha(alpha);
    
    return float4(final, alpha);
}
