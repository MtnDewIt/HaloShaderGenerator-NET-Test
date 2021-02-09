
uniform sampler2D target_sampler : register(s0);
uniform float4 pixel_size : register(c1);
uniform float4 scale : register(c2);

float4 main(in float2 texcoord : TEXCOORD) : COLOR
{
    float4 color = 0;
    
    float2 pixel_offset = pixel_size.xy;
    float2 tex;
    
    tex = texcoord + pixel_offset * float2(-4.1, 0.5);
    color += tex2D(target_sampler, tex) * 10;
    tex = texcoord + pixel_offset * float2(3.1, 0.5);
    color += tex2D(target_sampler, tex) * 10;
    tex = texcoord + pixel_offset * float2(-2.3, 0.5);
    color += tex2D(target_sampler, tex) * 120;
    tex = texcoord + pixel_offset * float2(1.3, 0.5);
    color += tex2D(target_sampler, tex) * 120;
    tex = texcoord + pixel_offset * float2(-0.5, 0.5);
    color += tex2D(target_sampler, tex) * 252;
    color /= 512;
    
    return color;
}