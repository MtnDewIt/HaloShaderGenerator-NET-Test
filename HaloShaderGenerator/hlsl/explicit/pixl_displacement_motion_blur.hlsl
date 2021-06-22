
#include "..\helpers\explicit_input_output.hlsli"
#include "registers\displacement_motion_blur_registers.hlsli"

float4 calculate_merged_transform(float4 input, bool is_odst)
{
    if (is_odst)
    {
        return mul(input, transpose(combined3));
    }
    else
    {        
        float4 stw_accum = mul(input, transpose(screen_to_world));
        return mul(stw_accum, transpose(previous_view_projection));
    }
}

float4 main_odst(VS_OUTPUT_SCREEN input) : COLOR
{
    float2 scene_tex = input.position.xy; // unnormalized
    
    if (do_distortion)
    {
        float2 displacement = tex2D(displacement_sampler, input.texcoord.xy).rg;
        scene_tex += distort_constants.zw;
        scene_tex += distort_constants.xy * displacement;
    }
    
    float4 ldr_sample = tex2D(ldr_buffer, scene_tex);
    
    float2 crosshair_area = scene_tex * crosshair_constants.xy + crosshair_constants.zw;
    float d_crosshair_area = saturate(dot(crosshair_area, crosshair_area));
    
    float falloff = saturate(d_crosshair_area * ldr_sample.a - 0.1f);
    
    float some_val = 1.0f;
    if (falloff > 0.0f)
    {
        float distortion_depth = tex2D(distortion_depth_buffer, scene_tex).r;
        
        float4 transform = calculate_merged_transform(float4(scene_tex, distortion_depth, 1.0f), true);
        transform.xyz /= transform.w;

        float2 new_tex = scene_tex - transform.xy;

        float d_tr_diff = dot(new_tex, new_tex);

        //todo
    }
    
    return float4(ldr_sample.rgb / some_val, 0);

}

float4 main(VS_OUTPUT_SCREEN input) : COLOR
{    
    float2 scene_tex = (input.position.xy + 0.5f) * screen_constants.xy;
    
    float distortion_depth = 1.0f / tex2D(distortion_depth_buffer, scene_tex).r;
    distortion_depth = zbuffer_xform.x * distortion_depth + zbuffer_xform.y;

    if (do_distortion)
    {
        float2 displacement = tex2D(displacement_sampler, input.texcoord.xy).rg - 0.501960814f;
        scene_tex += (screen_constants.zw + screen_constants.zw) * displacement;
    }
    
    float4 transform = calculate_merged_transform(float4(input.texcoord.zw, distortion_depth, 1.0f), false);
    transform.xyz /= transform.w;
    
    float4 ldr_sample = tex2D(ldr_buffer, scene_tex);
    float ldr_alpha = (1.0f - ldr_sample.a);
    
    float2 tex_scale = input.texcoord.zw - transform.xy;
    tex_scale *= blur_max_and_scale.zw;
    tex_scale = max(-blur_max_and_scale.xy, min(blur_max_and_scale.xy, tex_scale)); // ??????????????
    
    float2 crosshair_center_t = input.texcoord.zw - crosshair_center.xy;
    float n_crosshair_center = misc_values.w * dot(crosshair_center_t, crosshair_center_t);
    n_crosshair_center = min(n_crosshair_center, 1.0f);
    
    float scale = n_crosshair_center * ldr_alpha;
    float falloff = -scale + 0.00999999978f;
    
    float2 tex_accum = scene_tex;
    float4 blur_accum = 0;
    for (int i = 0; i < num_taps; i++)
    {
        tex_accum += tex_scale / misc_values.x;
        
        float4 blur_sample = tex2D(ldr_buffer, tex_accum);
        float blur_alpha = (1.0f - blur_sample.a);
        
        blur_accum.rgb += blur_sample.rgb * blur_alpha;
        blur_accum.a += blur_alpha;
    }
    
    blur_accum = falloff >= 0 ? 0.0f : blur_accum;

    scale *= blur_accum.a;
    scale /= misc_values.x;
    
    float3 final_color = blur_accum.rgb / blur_accum.a - ldr_sample.rgb;
    final_color = scale * final_color + ldr_sample.rgb;

    return float4(final_color, 0.0f);

}
