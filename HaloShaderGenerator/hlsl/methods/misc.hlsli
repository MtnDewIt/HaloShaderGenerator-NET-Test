#ifndef _MISC_HLSLi
#define _MISC_HLSLi

#include "../helpers/math.hlsli"

void bitmap_rotation_1
(
    in float4 xform,
    in float2 texcoord,
    out float2 new_texcoord
)
{
    float rotation = 0.5f;
    rotation += 0.159154937f * xform.x;
    rotation = frac(rotation);
    rotation = rotation * (2.0f * PI) - PI;
    
    float2 sin_cos;
    sincos(rotation, sin_cos.x, sin_cos.y);
    
    texcoord.x -= xform.z;
    texcoord.y -= xform.w;
    
    new_texcoord.x = xform.y * (-(sin_cos.x * texcoord.y) + sin_cos.y * texcoord.x);
    new_texcoord.y = xform.y * dot(sin_cos.xy, texcoord.xy); //(sin_cos.x * offset_texcoord.y + sin_cos.y * offset_texcoord.x);
    
    new_texcoord = new_texcoord.xy + xform.zw;
}

void bitmap_rotation_0
(
    in float4 xform,
    in float2 texcoord,
    out float2 new_texcoord
)
{
    
}

void bitmap_rotation_unapply_1
(
    in float4 xform,
    in float2 texcoord,
    out float2 new_texcoord
)
{
    // TODO
}

void bitmap_rotation_unapply_0
(
    in float4 xform,
    in float2 texcoord,
    out float2 new_texcoord
)
{
    
}

#endif