#ifndef DECORATOR_REGISTERS_HLSLI
#define DECORATOR_REGISTERS_HLSLI

#ifdef VERTEX_SHADER

//uniform float4x4 View_Projection : register(c0);
//uniform float3 Camera_Position : register(c7);
//uniform float4 Position_Compression_Scale : register(c12);
//uniform float4 Position_Compression_Offset : register(c13);
//uniform float4 UV_Compression_Scale_Offset : register(c14);
//uniform float4 v_atmosphere_constant_extra : register(c15);
uniform float4 v_simple_lights[40] : register(c16);
uniform float4 v_simple_light_count : register(c230);
//uniform float4 v_atmosphere_constant_0 : register(c233);
//uniform float4 v_atmosphere_constant_1 : register(c234);
//uniform float4 v_atmosphere_constant_2 : register(c235);
//uniform float4 v_atmosphere_constant_3 : register(c236);
//uniform float4 v_atmosphere_constant_4 : register(c237);
//uniform float4 v_atmosphere_constant_5 : register(c238);
uniform float4 instance_compression_offset : register(c240);
uniform float4 instance_compression_scale : register(c241);
uniform float subpart_index_count : register(c242);
uniform float4 wind_data : register(c243);
uniform float4 wind_data2 : register(c244);
uniform float4 LOD_constants : register(c245);
uniform float4 translucency : register(c246);
uniform float3 sun_direction : register(c247);
uniform float4 sun_color : register(c248);
uniform float4 wave_flow : register(c249);
uniform sampler2D wind_texture : register(s0);

#else

uniform float3 contrast : register(c13);

#endif
#endif
