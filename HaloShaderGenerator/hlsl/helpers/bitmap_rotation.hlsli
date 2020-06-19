#include "definition_helper.hlsli"
#include "..\methods\misc.hlsli"

#if misc_arg == k_misc_first_person_never_with_rotating_bitmaps
#define bitmap_rotation bitmap_rotation_1
#else 
#define bitmap_rotation bitmap_rotation_0
#endif

#if misc_arg == k_misc_first_person_never_with_rotating_bitmaps
#define bitmap_rotation_unapply bitmap_rotation_unapply_1
#else 
#define bitmap_rotation_unapply bitmap_rotation_unapply_0
#endif