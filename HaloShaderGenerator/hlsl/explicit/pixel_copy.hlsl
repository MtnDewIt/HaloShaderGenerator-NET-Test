
uniform sampler2D source_sampler : register(s0);
uniform float4 scale : register(c2);

float4 ps_default(float2 texcoord : TEXCOORD) : COLOR
{
    return tex2D(source_sampler, texcoord) * scale.x + scale.y;
}