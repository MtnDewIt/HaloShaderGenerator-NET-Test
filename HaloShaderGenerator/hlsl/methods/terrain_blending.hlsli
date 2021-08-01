#ifndef _TERRAIN_BLENDING_HLSLI
#define _TERRAIN_BLENDING_HLSLI

#include "../helpers/types.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/terrain_helper.hlsli"

uniform sampler blend_map;
uniform xform2d blend_map_xform;

uniform float global_albedo_tint;
uniform float4 dynamic_material;
uniform float transition_sharpness;
uniform float transition_threshold;

// TODO: check if weight depends on active materials

float4 normalize_additive_blend(inout float4 blend)
{
	float total_blend = 0;
	
	if(material_type_0_arg != k_material_off)
		total_blend += blend.x;
	else
		blend.x = 0;
	
	if (material_type_1_arg != k_material_off)
		total_blend += blend.y;
	else
		blend.y = 0;
	
	if (material_type_2_arg != k_material_off)
		total_blend += blend.z;
	else
		blend.z = 0;
	
	if (material_type_3_arg != k_material_off)
		total_blend += blend.w;
	else
		blend.w = 0;
	
	blend /= total_blend;
	return blend;
}

float4 normalize_multiply_blend(inout float4 blend)
{
	float total_blend = 1.0f;
	
	if (material_type_0_arg != k_material_off)
		total_blend *= blend.x;
	
	if (material_type_1_arg != k_material_off)
		total_blend *= blend.y;
	
	if (material_type_2_arg != k_material_off)
		total_blend *= blend.z;
	
	if (material_type_3_arg != k_material_off)
		total_blend *= blend.w;
	
	blend /= total_blend;
	return blend;
}

float blend_sum_active_materials(in float4 blend)
{
    float sum = 0;
	
    if (material_type_0_arg != k_material_off && material_type_0_arg != k_material_diffuse_only)
        sum += blend.x;
    if (material_type_1_arg != k_material_off && material_type_1_arg != k_material_diffuse_only)
        sum += blend.y;
    if (material_type_2_arg != k_material_off && material_type_2_arg != k_material_diffuse_only)
        sum += blend.z;
    if (material_type_3_arg != k_material_off && material_type_3_arg != k_material_diffuse_only)
        sum += blend.w;
	
    return sum;
}

float4 morph(float2 texcoord)
{
	float2 blend_map_texcoord = apply_xform2d(texcoord, blend_map_xform);
	float4 blend = tex2D(blend_map, blend_map_texcoord);
	return blend;
}

float4 dynamic_morph(float2 texcoord)
{
    float2 blend_map_texcoord = apply_xform2d(texcoord, blend_map_xform);
    float4 blend = tex2D(blend_map, blend_map_texcoord);
	
    float4 result;
	
    result.xyz = -blend.xyz + dynamic_material.xyz;
    result.w = saturate(transition_sharpness * (-transition_threshold - -blend.w));
    result.xyz = (result.xyz * result.w + blend.xyz);
    result.w *= dynamic_material.w;
	
    return result;
}


#ifndef blend_type
#define blend_type morph
#endif


#endif
