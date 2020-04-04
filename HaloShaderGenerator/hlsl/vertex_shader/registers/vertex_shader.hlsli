#ifndef _VS_SHADER_HLSLI
#define _VS_SHADER_HLSLI

uniform float4x4 view_projection : register(c0);
uniform float3 camera_position : register(c7);
uniform float4 position_compression_scale : register(c12);
uniform float4 position_compression_offset : register(c13);
uniform float4 uv_compression_scale_offset : register(c14);
uniform float4 nodes[210] : register(c16); // node transformations, supports up to 70 nodes
uniform float4 v_squish_params : register(c250);

#endif
