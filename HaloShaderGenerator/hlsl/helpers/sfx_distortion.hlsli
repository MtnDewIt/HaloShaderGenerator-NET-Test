#ifndef _SFX_DISTORTION_HLSLI
#define _SFX_DISTORTION_HLSLI

#include "../vertices/vertices.hlsli"
#include "input_output.hlsli"

VS_OUTPUT_SFX_DISTORT calculate_distortion_world(WORLD_VERTEX input)
{
	VS_OUTPUT_SFX_DISTORT output;

	float4 world_position = float4(input.position.xyz, 1.0);
	float4 screen_position = mul(world_position, view_projection);
	output.position = screen_position;
	output.position.z = calculate_z_squish(screen_position);
	
	float3 camera_dir = normalize(world_position.xyz - camera_position);
	float n_dot_c = dot(input.normal.xyz, camera_dir);
	float distortion_factor = min(abs(n_dot_c), 1.0);
	output.texcoord2 = distortion_factor * distortion_factor * (3.0 - 2.0 * distortion_factor);
	output.texcoord1.z = screen_position.w;
	output.texcoord1.xy = input.texcoord.xy;
	output.texcoord1.w = 1.0;
	return output;
}

#endif
