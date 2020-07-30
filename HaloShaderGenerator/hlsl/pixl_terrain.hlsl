#define terrain_template

#include "helpers\terrain_helper.hlsli"
#include "helpers\input_output.hlsli"

#if shaderstage == k_shaderstage_albedo
#include "terrain/entry_albedo.hlsli"
PS_OUTPUT_ALBEDO entry_albedo(VS_OUTPUT_ALBEDO input) : COLOR
{	
	return shader_entry_albedo(input);
}
#endif

#if shaderstage == k_shaderstage_lightmap_debug_mode
#include "terrain/entry_lightmap_debug.hlsli"
PS_OUTPUT_DEFAULT entry_lightmap_debug_mode(VS_OUTPUT_LIGHTMAP_DEBUG_MODE input) : COLOR
{
	return shader_entry_lightmap_debug_mode(input);
}
#endif
