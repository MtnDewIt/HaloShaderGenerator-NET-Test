#include "registers\vertex_shader.hlsli"
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

VS_OUTPUT_ACTIVE_CAMO entry_active_camo(input_vertex_format input)
{
	VS_OUTPUT_ACTIVE_CAMO output;
	return output;
}

