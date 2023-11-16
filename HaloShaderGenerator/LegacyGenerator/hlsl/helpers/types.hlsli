#ifndef _TYPES_HLSLI
#define _TYPES_HLSLI

#include "..\helpers\bitmap_rotation.hlsli"
#include "..\helpers\definition_helper.hlsli"

#ifndef misc_arg
#define misc_arg 0
#endif

#ifndef k_misc_first_person_never_with_rotating_bitmaps
#define k_misc_first_person_never_with_rotating_bitmaps 3
#endif

#define xform2d float4
#define boolf float

float2 apply_xform2d(float2 texcoord, xform2d xform)
{
	if (misc_arg == k_misc_first_person_never_with_rotating_bitmaps)
    {
        float2 new_texcoord;
        bitmap_rotation(xform, texcoord, new_texcoord);
        return new_texcoord;
    }
    
    return texcoord * xform.xy + xform.zw;
}

float2 unapply_xform2d(float2 texcoord, xform2d xform)
{
	if (misc_arg == k_misc_first_person_never_with_rotating_bitmaps)
    {
        float2 new_texcoord;
        bitmap_rotation_unapply(xform, texcoord, new_texcoord);
        return new_texcoord;
    }
    
	return (texcoord - xform.zw) / xform.xy;
}
#endif