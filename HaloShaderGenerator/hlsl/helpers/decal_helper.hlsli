#ifndef _DECAL_HELPER_HLSLI
#define _DECAL_HELPER_HLSLI

#ifndef DECAL_IS_SIMPLE
#define DECAL_IS_SIMPLE 1
#endif

void texcoord_clip(float2 texcoord)
{
    float4 clip_val = texcoord.xyxy * float4(1, 1, -1, -1) + float4(0, 0, 1, 1);
    clip(clip_val);
}

#endif