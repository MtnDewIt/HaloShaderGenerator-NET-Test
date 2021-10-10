#define foliage_template

#include "helpers\definition_helper.hlsli"
#include "helpers\input_output.hlsli"

#if shaderstage == k_shaderstage_albedo
#include "foliage/entry_albedo.hlsli"
PS_OUTPUT_ALBEDO entry_albedo(VS_OUTPUT_ALBEDO input) : COLOR
{	
	return foliage_entry_albedo(input);
}
#endif

#if shaderstage == k_shaderstage_static_sh || shaderstage == k_shaderstage_static_prt_ambient || shaderstage == k_shaderstage_static_prt_linear || shaderstage == k_shaderstage_static_prt_quadratic
#include "foliage/entry_prt.hlsli"
PS_OUTPUT_DEFAULT entry_static_sh(VS_OUTPUT_STATIC_SH input) : COLOR
{
	return foliage_entry_static_sh(input);
}

PS_OUTPUT_DEFAULT entry_static_prt_ambient(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return foliage_entry_static_prt(input);
}

PS_OUTPUT_DEFAULT entry_static_prt_linear(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return foliage_entry_static_prt(input);
}

PS_OUTPUT_DEFAULT entry_static_prt_quadratic(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return foliage_entry_static_prt(input);
}
#endif

#if shaderstage == k_shaderstage_static_per_pixel
#include "foliage/entry_per_pixel_lighting.hlsli"
PS_OUTPUT_DEFAULT entry_static_per_pixel(VS_OUTPUT_PER_PIXEL input) : COLOR
{
	return foliage_entry_static_per_pixel(input);
}

#endif

#if shaderstage == k_shaderstage_static_per_vertex
#include "foliage/entry_per_vertex_lighting.hlsli"
PS_OUTPUT_DEFAULT entry_static_per_vertex(VS_OUTPUT_PER_VERTEX input) : COLOR
{
	return foliage_entry_static_per_vertex(input);
}
#endif