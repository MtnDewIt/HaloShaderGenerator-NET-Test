
uniform sampler2D target_sampler : register(s0);
uniform float4 pixel_size : register(c1);
uniform float4 scale : register(c2);

#define kernel_sum 512
#define kernel_size 5 // size * 2 + 1

static const float2 kernel[kernel_size] = { float2(-4.1, 10), float2(-2.3, 120.0), float2(-0.5, 252), float2(1.3, 120), float2(3.1, 10) };

float4 blur_sample_vertical(float2 texcoord, float2 offset, int index)
{
    return tex2D(target_sampler, texcoord + offset * float2(0.5, kernel[index].x)) * kernel[index].y;
}
//float4 blur_sample_horizontal(float2 texcoord, float2 offset, int index)
//{
//    return tex2D(target_sampler, texcoord + offset * float2(kernel[index].x, 0.5)) * kernel[index].y;
//}

float4 ps_default(in float2 texcoord : TEXCOORD) : COLOR
{
    float4 color = 0;
    
    //float2 resolution = 1.0f / pixel_size.xy;
    //
    //for (int i = 0; i < kernel_size; i++)
    //{
    //    color += blur_sample_vertical(texcoord, pixel_size.xy * scale.xy, i);
    //}
    //color /= kernel_sum;
    
    float2 pixel_offset = pixel_size.xy;
    float2 tex;
    
    tex = texcoord + pixel_offset * float2(0.5, -4.1);
    color += tex2D(target_sampler, tex) * 10;
    tex = texcoord + pixel_offset * float2(0.5, 3.1);
    color += tex2D(target_sampler, tex) * 10;
    tex = texcoord + pixel_offset * float2(0.5, -2.3);
    color += tex2D(target_sampler, tex) * 120;
    tex = texcoord + pixel_offset * float2(0.5, 1.3);
    color += tex2D(target_sampler, tex) * 120;
    tex = texcoord + pixel_offset * float2(0.5, -0.5);
    color += tex2D(target_sampler, tex) * 252;
    color /= 512;
    
    return color;
}