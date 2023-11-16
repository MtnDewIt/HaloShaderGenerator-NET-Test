#ifndef _TRANSFORM_MATH_HLSLI
#define _TRANSFORM_MATH_HLSLI

#include "../registers/vertex_shader.hlsli"

float3 transform_vector(float3 dir, float3x3 transform)
{
	return normalize(mul(dir, transpose(transform)));
}

float3 transform_binormal(float3 normal, float3 tangent, float3 binormal)
{
	//This is Halo Online broken tangent space binormal, just return the actual binormal
	// float3 computed_binormal = cross(normal, tangent);
	// float bin_sign = sign(dot(computed_binormal, binormal));
	// return bin_sign * binormal;
	return binormal;
}

float3 decompress_vertex_position(float3 position)
{
	return (position * Position_Compression_Scale.xyz) + Position_Compression_Offset.xyz;
}

void calculate_z_squish(inout float4 screen_position)
{
	//screen_position.z = v_squish_params.w * ((v_squish_params.x * screen_position.w - v_squish_params.y) * v_squish_params.z - screen_position.z) + screen_position.z;
}

void calculate_z_squish_2(inout float4 screen_position)
{
	//calculate_z_squish(screen_position);
	//float offset;
	//if (v_mesh_squished)
	//	offset = 0.000005;
	//else
	//	offset = 0.00002;
	//screen_position.z = screen_position.z - offset;
}

float4 calculate_screenspace_position(float4 vertex_position)
{
	return mul(vertex_position, View_Projection);
}

float2 calculate_texcoord(float4 texcoord)
{
	return texcoord.xy * UV_Compression_Scale_Offset.xy + UV_Compression_Scale_Offset.zw;
}

float3 rgbe_to_rgb(float4 rgbe)
{
	rgbe = 2.0 * rgbe - 1.0; // original data encoded with signed bytes so convert to unsigned scale
	float exponent = pow(2.0, 31.75 * rgbe.w);
	float3 rgb = rgbe.rgb * exponent;
	return rgb;
}

#endif
