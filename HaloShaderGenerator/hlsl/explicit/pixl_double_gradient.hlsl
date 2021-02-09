
uniform sampler2D source_sampler : register(s0);
uniform float4 scale : register(c2);

// vertex output is s_screen_vertex but doesn't really matter
float4 main(in float2 texcoord : TEXCOORD) : COLOR
{
    return tex2D(source_sampler, texcoord.xy) * scale;
}