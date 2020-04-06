#ifndef _VS_SHADER_HLSLI
#define _VS_SHADER_HLSLI

uniform float4x4 view_projection : register(c0);
uniform float3 camera_forward : register(c4);
uniform float3 camera_left : register(c5);
uniform float3 camera_up : register(c6);
uniform float3 camera_position : register(c7);
uniform float4 position_compression_scale : register(c12);
uniform float4 position_compression_offset : register(c13);
uniform float4 uv_compression_scale_offset : register(c14);
uniform float4 v_atmosphere_constant_extra : register(c15);
uniform float4 nodes[210] : register(c16); // node transformations, supports up to 70 nodes


uniform float4 v_atmosphere_constant_0 : register(c233);
uniform float4 v_atmosphere_constant_1 : register(c234);
uniform float4 v_atmosphere_constant_2 : register(c235);
uniform float4 v_atmosphere_constant_3 : register(c236);
uniform float4 v_atmosphere_constant_4 : register(c237);
uniform float4 v_atmosphere_constant_5 : register(c238);

// this seems to only appear for dynamic lights, need macros
//uniform float4x4 shadow_projection : register(c240);

uniform float4 v_lighting_constant_0 : register(c240);
uniform float4 v_lighting_constant_1 : register(c241);
uniform float4 v_lighting_constant_2 : register(c242);
uniform float4 v_lighting_constant_3 : register(c243);
uniform float4 v_lighting_constant_4 : register(c244);
uniform float4 v_lighting_constant_5 : register(c245);
uniform float4 v_lighting_constant_6 : register(c246);
uniform float4 v_lighting_constant_7 : register(c247);
uniform float4 v_lighting_constant_8 : register(c248);
uniform float4 v_lighting_constant_9 : register(c249);
uniform float4 v_squish_params : register(c250);

#endif
