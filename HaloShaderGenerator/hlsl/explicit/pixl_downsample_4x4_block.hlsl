
uniform sampler2D source_sampler : register(s0);
uniform float4 pixel_size : register(c1);
uniform float4 scale : register(c2);

float4 main(in float2 texcoord : TEXCOORD) : COLOR
{
    float4 color = tex2D(source_sampler, texcoord + float2(-pixel_size.x, -pixel_size.y)) +
        tex2D(source_sampler, texcoord + float2(pixel_size.x, -pixel_size.y)) + 
        tex2D(source_sampler, texcoord + float2(-pixel_size.x, pixel_size.y)) +
        tex2D(source_sampler, texcoord + float2( pixel_size.x,  pixel_size.y));
    
    return color * 0.25f;
}