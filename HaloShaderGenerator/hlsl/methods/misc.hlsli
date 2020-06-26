#ifndef _MISC_HLSLi
#define _MISC_HLSLi

#include "../helpers/math.hlsli"

void bitmap_rotation_1(
    in float4 xform,
    in float2 texcoord,
    out float2 new_texcoord)
{
	float sin, cos;
	sincos((2.0f * PI) * frac(0.159154937f * xform.x + 0.5) - PI, sin, cos);
    
    texcoord.xy -= xform.zw;

	new_texcoord.x = xform.y * (cos * texcoord.x - sin * texcoord.y);
	new_texcoord.y = xform.y * (sin * texcoord.x + cos * texcoord.y);
    
    new_texcoord = new_texcoord.xy + xform.zw;
}

void bitmap_rotation_0(
    in float4 xform,
    in float2 texcoord,
    out float2 new_texcoord)
{
    
}

void bitmap_rotation_unapply_1(
    in float4 xform,
    in float2 texcoord,
    out float2 new_texcoord)
{
    // TODO
}

void bitmap_rotation_unapply_0(
    in float4 xform,
    in float2 texcoord,
    out float2 new_texcoord)
{
    
}

#endif