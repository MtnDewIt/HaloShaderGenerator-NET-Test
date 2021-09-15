#ifndef _HLSLI_WATER_GLOBALS
#define _HLSLI_WATER_GLOBALS

uniform bool k_is_lightmap_exist : register(b0);
uniform bool k_is_water_interaction : register(b1);
uniform bool k_is_camera_underwater : register(b3);

uniform float4x4 k_ps_water_view_xform_inverse : register(c213);
uniform float4 k_ps_water_view_depth_constant : register(c217);
uniform float4 k_ps_water_player_view_constant : register(c218);

uniform sampler2D unknown_s0 : register(s0); //todo: remove

#endif

#ifndef _GLOBAL_SHAPE_TEX
#define _GLOBAL_SHAPE_TEX
uniform sampler2D global_shape_texture;
#endif

