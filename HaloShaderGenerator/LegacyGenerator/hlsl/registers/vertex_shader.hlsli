#ifndef _VS_SHADER_HLSLI
#define _VS_SHADER_HLSLI

uniform bool v_mesh_squished : register(b8);

uniform float4x4 View_Projection : register(c0);
uniform float3 Camera_Forward : register(c4);
uniform float3 Camera_Left : register(c5);
uniform float3 Camera_Up : register(c6);
uniform float3 Camera_Position : register(c7);

uniform float3 screen_xform_x : register(c8);
uniform float3 screen_xform_y : register(c9);
uniform float3 viewport_scale : register(c10);
uniform float3 viewport_offset : register(c11);

#if shadertype != k_shadertype_water
uniform float4 Position_Compression_Scale : register(c12);
uniform float4 Position_Compression_Offset : register(c13);
uniform float4 UV_Compression_Scale_Offset : register(c14);
uniform float4 Nodes[210] : register(c16); // node transformations, supports up to 70 nodes
#endif

uniform float4 v_atmosphere_constant_extra : register(c15);

uniform float4 g_exposure : register(c232);
uniform float4 v_atmosphere_constant_0 : register(c233);
uniform float4 v_atmosphere_constant_1 : register(c234);
uniform float4 v_atmosphere_constant_2 : register(c235);
uniform float4 v_atmosphere_constant_3 : register(c236);
uniform float4 v_atmosphere_constant_4 : register(c237);
uniform float4 v_atmosphere_constant_5 : register(c238);
uniform float4 g_alt_exposure : register(c239);

uniform float4x4 shadow_projection : register(c240);

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
