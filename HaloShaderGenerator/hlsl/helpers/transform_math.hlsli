#ifndef _TRANSFORM_MATH_HLSLI
#define _TRANSFORM_MATH_HLSLI

#include "../registers/vertex_shader.hlsli"

float3 transform_vector(float3 dir, float3x3 transform)
{
	return normalize(mul(transform, dir));
}

float3 transform_binormal(float3 normal, float3 tangent, float3 binormal)
{
	float3 computed_binormal = cross(normal, tangent);
	float bin_sign = sign(dot(computed_binormal, binormal));
	return bin_sign * binormal;
}

float3 decompress_vertex_position(float3 position)
{
	return (position * position_compression_scale.xyz) + position_compression_offset.xyz;
}

float calculate_z_squish(float4 screen_position)
{
	return v_squish_params.w * ((v_squish_params.x * screen_position.w - v_squish_params.y) * v_squish_params.z - screen_position.z) + screen_position.z;
}

float4 unknown_z_squish(float4 screen_position)
{
	float result = calculate_z_squish(screen_position);
	/* this appears in skinned ambient glvs for shader
	if (v_mesh_squished)
	{
		screen_position.z = screen_position.z - 0.000005;
	}
	else
	{
		screen_position.z = screen_position.z - 0.00002;
	}*/
	return screen_position;
}

float4 calculate_screenspace_position(float4 vertex_position)
{
	float4 screen_position = mul(vertex_position, view_projection);
	screen_position.z = calculate_z_squish(screen_position);
	return screen_position;
}

float2 calculate_texcoord(float4 texcoord)
{
	return texcoord.xy * uv_compression_scale_offset.xy + uv_compression_scale_offset.zw;
}


#endif
