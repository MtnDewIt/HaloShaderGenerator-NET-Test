﻿#include "registers\vertex_shader.hlsli"
#include "helpers\input_output.hlsli"
#include "helpers\transform_math.hlsli"
#include "helpers\math.hlsli"
#include "helpers\atmosphere.hlsli"
#include "helpers\prt.hlsli"
#include "vertices\vertices.hlsli"
#include "vertices\prt.hlsli"
#include "helpers\sfx_distortion.hlsli"

VS_OUTPUT_ALBEDO entry_albedo(input_vertex_format input)
{
    VS_OUTPUT_ALBEDO output;
	float4 world_position;
	calc_vertex_transform(input, world_position, output.position, output.normal.xyz, output.tangent, output.binormal, output.texcoord, output.camera_dir);
	calculate_z_squish(output.position);
	output.normal.w = output.position.w;
	return output;
}

VS_OUTPUT_STATIC_PRT entry_static_prt_ambient(input_vertex_format input, AMBIENT_PRT input_prt)
{
	VS_OUTPUT_STATIC_PRT output;
	float4 world_position;
	
	calc_vertex_transform(input, world_position, output.position, output.normal, output.tangent, output.binormal, output.texcoord, output.camera_dir);
	calculate_z_squish(output.position);
	output.prt_radiance_vector = calculate_ambient_radiance_vector(input_prt.coefficient, output.normal);
	calculate_atmosphere_radiance(world_position, output.camera_dir, output.extinction_factor.rgb, output.sky_radiance.rgb);
	
	return output;
}
// TODO: verify compiled shaders because they are a bit different but I can't see why
VS_OUTPUT_STATIC_PRT entry_static_prt_linear(input_vertex_format input, LINEAR_PRT input_prt)
{
	VS_OUTPUT_STATIC_PRT output;
	float4 world_position;
	
	calc_vertex_transform(input, world_position, output.position, output.normal, output.tangent, output.binormal, output.texcoord, output.camera_dir);
	calculate_z_squish(output.position);
	output.prt_radiance_vector = calculate_linear_radiance_vector(input, input_prt.coefficients, output.normal);
	calculate_atmosphere_radiance(world_position, output.camera_dir, output.extinction_factor.rgb, output.sky_radiance.rgb);
	
	return output;
}
// TODO: review code entirely
VS_OUTPUT_STATIC_PRT entry_static_prt_quadratic(input_vertex_format input, QUADRATIC_PRT input_prt)
{
	VS_OUTPUT_STATIC_PRT output;
	float4 world_position;
	
	calc_vertex_transform(input, world_position, output.position, output.normal, output.tangent, output.binormal, output.texcoord, output.camera_dir);
	calculate_atmosphere_radiance(world_position, output.camera_dir, output.extinction_factor.rgb, output.sky_radiance.rgb);
	output.prt_radiance_vector = calculate_quadratic_radiance_vector(input, input_prt, output.normal);
	return output;
}

VS_OUTPUT_SFX_DISTORT entry_sfx_distort(input_vertex_format input)
{
	VS_OUTPUT_SFX_DISTORT output;
	calc_distortion(input, output);
	return output;

}

VS_OUTPUT_DYNAMIC_LIGHT entry_dynamic_light(input_vertex_format input)
{
	VS_OUTPUT_DYNAMIC_LIGHT output;
	float4 world_position;
	
	calc_vertex_transform(input, world_position, output.position, output.normal, output.tangent, output.binormal, output.texcoord, output.camera_dir);
	calculate_z_squish(output.position);
	
	output.shadowmap_texcoord = mul(world_position, shadow_projection);
	
	return output;

}

VS_OUTPUT_DYNAMIC_LIGHT entry_dynamic_light_cinematic(input_vertex_format input)
{
	return entry_dynamic_light(input);
}

VS_OUTPUT_ACTIVE_CAMO entry_active_camo(input_vertex_format input)
{
	VS_OUTPUT_ACTIVE_CAMO output;
	float4 world_position;
	float3 normal, tangent, binormal, camera_dir;
	
	float3 unknown_pos = input.position.xyz - 0.5;
	unknown_pos.xy *= position_compression_scale.xy;
	
	output.camo_param.x = dot(input.normal.xyz, camera_left);
	output.camo_param.y = dot(input.normal.xyz, camera_up);
	output.camo_param.z = atan2(unknown_pos.x, unknown_pos.y);
	output.camo_param.w = acos(unknown_pos.z) * (position_compression_scale.z / length(position_compression_scale.xy));

	calc_vertex_transform(input, world_position, output.position, normal, tangent, binormal, output.texcoord.xy, camera_dir);
	calculate_z_squish(output.position);
	output.position.z -= 0.00002;
	// there may be a sign issue in camera_dir, doesn't compile exactly the same
	output.texcoord.w = length(camera_dir);
	output.texcoord.z = 0;
	
	return output;
}
