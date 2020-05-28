#ifndef _TYPES_HLSLI
#define _TYPES_HLSLI

#define xform2d float4
#define boolf float

float2 apply_xform2d(float2 texcoord, xform2d xform)
{
    return texcoord * xform.xy + xform.zw;
}

float2 unapply_xform2d(float2 texcoord, xform2d xform)
{
	return (texcoord - xform.zw) / xform.xy;
}
#endif