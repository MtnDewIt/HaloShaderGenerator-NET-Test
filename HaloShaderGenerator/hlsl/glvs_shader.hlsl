#include "registers\vertex_shader.hlsli"
#include "helpers\input_output.hlsli"
#include "helpers\transform_math.hlsli"
#include "helpers\math.hlsli"
#include "helpers\atmosphere.hlsli"
#include "helpers\prt.hlsli"
#include "vertices\vertices.hlsli"
#include "vertices\prt.hlsli"

VS_OUTPUT_ALBEDO entry_albedo(input_vertex_format input)
{
    VS_OUTPUT_ALBEDO output;
	float4 world_position;
	calc_vertex_transform(input, world_position, output.position, output.normal.xyz, output.tangent, output.binormal, output.texcoord, output.camera_dir);
	output.normal.w = output.position.w;
	return output;
}

// TODO: check skinned and world vertex transformation, they seem to have extra parameters (boolean for squish?)
VS_OUTPUT_STATIC_PTR entry_static_prt_ambient(input_vertex_format input, AMBIENT_PRT input_prt)
{
	VS_OUTPUT_STATIC_PTR output;
	float4 world_position;
	
	calc_vertex_transform(input, world_position, output.position, output.normal, output.tangent, output.binormal, output.texcoord, output.camera_dir);
	calculate_atmosphere_radiance(world_position, output.camera_dir, output.extinction_factor.rgb, output.sky_radiance.rgb);
	output.prt_radiance_vector = calculate_ambient_radiance_vector(input_prt.coefficient, output.normal);

	return output;
}