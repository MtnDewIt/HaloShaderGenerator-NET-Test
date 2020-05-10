#ifndef _PARALLAX_HLSLI
#define _PARALLAX_HLSLI

#include "../helpers/math.hlsli"
#include "../helpers/types.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/input_output.hlsli"
#include "../registers/shader.hlsli"

float calc_heightmap_value(float2 texcoord)
{
	float4 sample = tex2D(height_map, texcoord);
	float height = sample.y - 0.5;
	height = height * height_scale;
	return height;
}

float2 calc_parallax_off_ps(VS_OUTPUT_ALBEDO input)
{
	return input.texcoord;
}

float2 calc_parallax_simple_ps(VS_OUTPUT_ALBEDO input)
{
	return input.texcoord;
}

float2 calc_parallax_interpolated_ps(VS_OUTPUT_ALBEDO input)
{
	float2 output_texcoord;
	

	float3 cam_dir = normalize(input.camera_dir.xyz);
	float3 delta = float3(dot(input.tangent, cam_dir), dot(input.binormal, cam_dir), dot(input.normal.xyz, cam_dir));

	float2 height_map_texcoord_1 = apply_xform2d(input.texcoord, height_map_xform);
	float height_1 = calc_heightmap_value(height_map_texcoord_1);
	
	float2 height_map_texcoord_2 = height_1 * delta.xy + height_map_texcoord_1;
	float height_2 = calc_heightmap_value(height_map_texcoord_2);
	
	float a = (height_1) + (height_1 * delta.z - height_2);
	float step_1 = height_1 - delta.z * height_1;
	float step_2 = height_2 - delta.z * height_1;
	
	float weight = height_1 / a;
	
	float2 outcome_1 = step_2 * delta.xy + height_map_texcoord_2.xy;
	float2 outcome_2 = weight * (delta.xy * height_1) + height_map_texcoord_1;
	
	float sign1 = sign(step_1);
	float sign2 = sign(step_2);
	float sign = sign2 - sign1;
	
	[flatten]
	if (sign == 0)
	{
		output_texcoord = outcome_1;
	}
	else
	{
		output_texcoord = outcome_2;
	}
	
	return unapply_xform2d(output_texcoord, height_map_xform);
}

float2 calc_parallax_simple_detail_ps(VS_OUTPUT_ALBEDO input)
{
	return input.texcoord;
}

void calc_parallax_off_vs()
{

}

void calc_parallax_simple_vs()
{

}

void calc_parallax_interpolated_vs()
{

}

// fixups
#define calc_parallax_simple_detail_vs calc_parallax_simple_vs

#ifndef calc_parallax_ps
#define calc_parallax_ps calc_parallax_off_ps
#endif
#ifndef calc_parallax_vs
#define calc_parallax_vs calc_parallax_off_vs
#endif

#endif
