#define halogram_template

#include "helpers\halogram_helper.hlsli"
#include "helpers\input_output.hlsli"

#if shaderstage == k_shaderstage_albedo
#include "halogram/entry_albedo.hlsli"
PS_OUTPUT_ALBEDO entry_albedo(VS_OUTPUT_ALBEDO input) : COLOR
{
    return halogram_entry_albedo(input);
}
#endif

#if shaderstage == k_shaderstage_static_per_pixel
#include "halogram/entry_static_per_pixel.hlsli"
PS_OUTPUT_DEFAULT entry_static_per_pixel(VS_OUTPUT_PER_PIXEL input) : COLOR
{
	return halogram_entry_static_per_pixel(input);
}
#endif

#if shaderstage == k_shaderstage_static_per_vertex
#include "halogram/entry_static_per_vertex.hlsli"
PS_OUTPUT_DEFAULT entry_static_per_vertex(VS_OUTPUT_PER_VERTEX input) : COLOR
{
	return halogram_entry_static_per_vertex(input);
}
#endif

#if shaderstage == k_shaderstage_static_per_vertex_color
#include "halogram/entry_static_per_vertex_color.hlsli"
PS_OUTPUT_DEFAULT entry_static_per_vertex_color(VS_OUTPUT_PER_VERTEX_COLOR input) : COLOR
{
	return halogram_entry_static_per_vertex_color(input);
}
#endif

#if shaderstage == k_shaderstage_static_sh || shaderstage == k_shaderstage_static_prt_ambient || shaderstage == k_shaderstage_static_prt_linear || shaderstage == k_shaderstage_static_prt_quadratic
#include "halogram/entry_prt.hlsli"
PS_OUTPUT_DEFAULT entry_static_sh(VS_OUTPUT_STATIC_SH input) : COLOR
{
	return halogram_entry_static_sh(input);
}

PS_OUTPUT_DEFAULT entry_static_prt_ambient(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return halogram_entry_static_prt(input);
}

PS_OUTPUT_DEFAULT entry_static_prt_linear(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return halogram_entry_static_prt(input);
}

PS_OUTPUT_DEFAULT entry_static_prt_quadratic(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return halogram_entry_static_prt(input);
}
#endif

#if shaderstage == k_shaderstage_dynamic_light || shaderstage == k_shaderstage_dynamic_light_cinematic
#include "halogram/entry_dynamic_light.hlsli"
PS_OUTPUT_DEFAULT entry_dynamic_light(VS_OUTPUT_DYNAMIC_LIGHT input) : COLOR
{
	return halogram_entry_dynamic_light(input);
}

PS_OUTPUT_DEFAULT entry_dynamic_light_cinematic(VS_OUTPUT_DYNAMIC_LIGHT input) : COLOR
{
	return halogram_entry_dynamic_light_cinematic(input);
}
#endif