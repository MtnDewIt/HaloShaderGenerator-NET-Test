#define water_template

#include "helpers\definition_helper.hlsli"
#include "helpers\input_output.hlsli"

#if shaderstage == k_shaderstage_static_per_pixel
#include "water/entry_static_per_pixel.hlsli"
PS_OUTPUT_DEFAULT entry_static_per_pixel(VS_OUTPUT_WATER input) : COLOR
{
    return water_entry_static_per_pixel(input);
}
#endif

#if shaderstage == k_shaderstage_static_per_vertex
#include "water/entry_static_per_vertex.hlsli"
PS_OUTPUT_DEFAULT entry_static_per_vertex(VS_OUTPUT_WATER input) : COLOR
{	
	return water_entry_static_per_vertex(input);
}
#endif

#if shaderstage == k_shaderstage_water_tesselation
float4 entry_water_tesselation() : COLOR
{	
	return float4(0, 1, 2, 3);
}
#endif