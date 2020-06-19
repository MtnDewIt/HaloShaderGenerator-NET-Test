#ifndef _PARALLAX_HLSLI
#define _PARALLAX_HLSLI

#include "../helpers/math.hlsli"
#include "../helpers/types.hlsli"

uniform float height_scale;
uniform sampler height_map;
uniform xform2d height_map_xform;
uniform sampler height_scale_map;

float calc_heightmap_value(float2 texcoord)
{
	float4 sample = tex2D(height_map, texcoord);
	float height = sample.y - 0.5;
	height = height * height_scale;
	return height;
}

float2 calc_parallax_off_ps(float2 texcoord, float3 camera_dir, float3 tangent, float3 binormal, float3 normal)
{
	return texcoord;
}

float2 calc_parallax_simple_ps(float2 texcoord, float3 view_dir, float3 tangent, float3 binormal, float3 normal)
{
	float2 output_texcoord;
	
	float2 delta = float2(dot(binormal, view_dir), dot(tangent, view_dir));
	
	float2 height_map_texcoord = apply_xform2d(texcoord, height_map_xform);
	float height = calc_heightmap_value(height_map_texcoord);

	output_texcoord = ((height * delta.xy) / height_map_xform.xy) + texcoord.xy;	// scale texcoord offset with inverse transform value and add original texcoord
	return output_texcoord;
}

float2 calc_parallax_interpolated_ps(float2 texcoord, float3 view_dir, float3 tangent, float3 binormal, float3 normal)
{
	float2 output_texcoord;

	float3 delta = float3(dot(binormal, view_dir), dot(tangent, view_dir), dot(normal.xyz, view_dir));

	float2 height_map_texcoord_1 = apply_xform2d(texcoord, height_map_xform);
	float height_1 = calc_heightmap_value(height_map_texcoord_1);
	
	float2 height_map_texcoord_2 = height_1 * delta.xy + height_map_texcoord_1;
	float height_2 = calc_heightmap_value(height_map_texcoord_2);
	
	float step_1 = height_1 - height_1 * delta.z;
	float step_2 = height_2 - height_1 * delta.z;
	
	float a = (height_1) + (height_1 * delta.z - height_2);
	float weight = height_1 / a;
	float2 outcome_2 = weight * (delta.xy * height_1) / height_map_xform.xy + texcoord;
	
	float2 outcome_1 = step_2 * delta.xy / height_map_xform.xy + (height_map_texcoord_2 - height_map_xform.zw);
	
	[flatten]
	if (sign(step_2) != sign(step_1))
	{
		output_texcoord = outcome_2;
	}
	else
	{
		output_texcoord = outcome_1;
	}
	
	return output_texcoord;

}

float2 calc_parallax_simple_detail_ps(float2 texcoord, float3 view_dir, float3 tangent, float3 binormal, float3 normal)
{
	float2 delta = float2(dot(binormal, view_dir), dot(tangent, view_dir));
	
	float2 height_map_texcoord = apply_xform2d(texcoord, height_map_xform);
	float height = calc_heightmap_value(height_map_texcoord);
	float4 scale_map_sample = tex2D(height_scale_map, height_map_texcoord);
	
	float2 output_texcoord;

	output_texcoord = ((height * delta.xy) / height_map_xform.xy) + texcoord.xy;
	return output_texcoord;
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
