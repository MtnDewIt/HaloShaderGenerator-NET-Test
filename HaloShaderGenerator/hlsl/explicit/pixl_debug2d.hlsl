
#include "helpers\final_composite_helper.hlsli"
uniform float fill_color : register(c1);

float4 main(in float3 color : COLOR) : COLOR
{
    return float4(color * fill_color, 1.0f);
}