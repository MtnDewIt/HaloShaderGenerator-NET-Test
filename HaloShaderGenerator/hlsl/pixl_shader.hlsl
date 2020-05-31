#define shader_template

#include "shader/entry_albedo.hlsli"
#include "shader/entry_active_camo.hlsli"
#include "shader/entry_lightmap_debug.hlsli"
#include "shader/entry_sfx_distort.hlsli"
#include "shader/entry_prt.hlsli"
#include "shader/entry_dynamic_light.hlsli"
#include "shader/entry_per_pixel_lighting.hlsli"
#include "shader/entry_per_vertex_color_lighting.hlsli"
#include "shader/entry_per_vertex_lighting.hlsli"

#include "helpers\input_output.hlsli"
#include "helpers\definition_helper.hlsli"


PS_OUTPUT_ALBEDO entry_albedo(VS_OUTPUT_ALBEDO input) : COLOR
{	
	return shader_entry_albedo(input);
}

PS_OUTPUT_DEFAULT entry_active_camo(VS_OUTPUT_ACTIVE_CAMO input) : COLOR
{
	return shader_entry_active_camo(input);
}

PS_OUTPUT_DEFAULT entry_static_sh(VS_OUTPUT_STATIC_SH input) : COLOR
{
	return shader_entry_static_sh(input);
}

PS_OUTPUT_DEFAULT entry_static_prt_ambient(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return shader_entry_static_prt(input);
}

PS_OUTPUT_DEFAULT entry_static_prt_linear(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return shader_entry_static_prt(input);
}

PS_OUTPUT_DEFAULT entry_static_prt_quadratic(VS_OUTPUT_STATIC_PRT input) : COLOR
{
	return shader_entry_static_prt(input);
}

PS_OUTPUT_DEFAULT entry_sfx_distort(VS_OUTPUT_SFX_DISTORT input) : COLOR
{
	return shader_entry_sfx_distort(input);
}

PS_OUTPUT_DEFAULT entry_dynamic_light(VS_OUTPUT_DYNAMIC_LIGHT input) : COLOR
{
	return shader_entry_dynamic_light(input);
}

PS_OUTPUT_DEFAULT entry_dynamic_light_cinematic(VS_OUTPUT_DYNAMIC_LIGHT input) : COLOR
{
	return shader_entry_dynamic_light_cinematic(input);
}

PS_OUTPUT_DEFAULT entry_lightmap_debug_mode(VS_OUTPUT_LIGHTMAP_DEBUG_MODE input) : COLOR
{
	return shader_entry_lightmap_debug_mode(input);
}

PS_OUTPUT_DEFAULT entry_static_per_vertex_color(VS_OUTPUT_PER_VERTEX_COLOR input) : COLOR
{
	return shader_entry_static_per_vertex_color(input);
}

PS_OUTPUT_DEFAULT entry_static_per_pixel(VS_OUTPUT_PER_PIXEL input) : COLOR
{
	return shader_entry_static_per_pixel(input);
}

PS_OUTPUT_DEFAULT entry_static_per_vertex(VS_OUTPUT_PER_VERTEX input) : COLOR
{
	return shader_entry_static_per_vertex(input);
}