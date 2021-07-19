
uniform sampler2D source_sampler : register(s0);
uniform float4 pixel_size : register(c1);
uniform float4 scale : register(c2);
//uniform float4 zbuf_params : register(c3);

#define ODST_SHADER 1

float3 tex2D_offset(float2 texcoord, float2 offset)
{
    return tex2D(source_sampler, texcoord + (offset * pixel_size.xy)).xxx;
    //return zbuf_params.x / tex2D(source_sampler, texcoord + (offset * pixel_size.xy)).xxx + zbuf_params.y;
}

// vertex output is s_screen_vertex but doesn't really matter
float4 main(in float2 texcoord : TEXCOORD) : COLOR
{    
#if ODST_SHADER == 1
    
    float3 center = tex2D_offset(texcoord, float2( 0, 0));
    float3 right =  tex2D_offset(texcoord, float2( 1, 0));
    float3 left =   tex2D_offset(texcoord, float2(-1, 0));
    float3 top =    tex2D_offset(texcoord, float2( 0, 1));
    float3 bottom = tex2D_offset(texcoord, float2( 0,-1));
    
    float3 y_grad = pow(top + bottom - 2 * center, 2.0f);
    float3 x_grad = pow(right + left - 2 * center, 2.0f);
    
    return float4(saturate(sqrt(abs(y_grad + x_grad))) * scale.xyz, scale.w);
    
#else
    return tex2D(source_sampler, texcoord.xy) * scale;
#endif
}