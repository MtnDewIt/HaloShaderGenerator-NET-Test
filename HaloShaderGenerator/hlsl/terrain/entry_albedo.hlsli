#ifndef _TERRAIN_TEMPLATE_ALBEDO_HLSLI
#define _TERRAIN_TEMPLATE_ALBEDO_HLSLI

#include "..\registers\global_parameters.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\terrain_helper.hlsli"
#include "..\helpers\color_processing.hlsli"
#include "..\helpers\bumpmap_math.hlsli"

#include "..\methods\albedo_terrain.hlsli"
#include "..\methods\terrain_blending.hlsli"
#include "..\methods\terrain_bump_mapping.hlsli"

uniform sampler base_map_m_0;
uniform xform2d base_map_m_0_xform;
uniform sampler detail_map_m_0;
uniform xform2d detail_map_m_0_xform;
uniform sampler bump_map_m_0;
uniform xform2d bump_map_m_0_xform;
uniform sampler detail_bump_m_0;
uniform xform2d detail_bump_m_0_xform;

uniform sampler base_map_m_1;
uniform xform2d base_map_m_1_xform;
uniform sampler detail_map_m_1;
uniform xform2d detail_map_m_1_xform;
uniform sampler bump_map_m_1;
uniform xform2d bump_map_m_1_xform;
uniform sampler detail_bump_m_1;
uniform xform2d detail_bump_m_1_xform;

uniform sampler base_map_m_2;
uniform xform2d base_map_m_2_xform;
uniform sampler detail_map_m_2;
uniform xform2d detail_map_m_2_xform;
uniform sampler bump_map_m_2;
uniform xform2d bump_map_m_2_xform;
uniform sampler detail_bump_m_2;
uniform xform2d detail_bump_m_2_xform;

uniform sampler base_map_m_3;
uniform xform2d base_map_m_3_xform;
uniform sampler detail_map_m_3;
uniform xform2d detail_map_m_3_xform;
uniform sampler bump_map_m_3;
uniform xform2d bump_map_m_3_xform;
uniform sampler detail_bump_m_3;
uniform xform2d detail_bump_m_3_xform;

PS_OUTPUT_ALBEDO shader_entry_albedo(VS_OUTPUT_ALBEDO input)
{	
	float4 albedo = 0;
	float3 m_normal = 0;

	float2 texcoord = input.texcoord;
	float3 tangent = input.tangent;
	float3 normal = input.normal.xyz;
	float3 binormal = input.binormal;

	float3 normal_3 = 0, normal_2 = 0, normal_1 = 0, normal_0 = 0;
    float4 albedo_3 = 0, albedo_2 = 0, albedo_1 = 0, albedo_0 = 0;
	
    float4 blend = blend_type(texcoord);
    blend = normalize_additive_blend(blend);
    
    // normal
	
    if (material_type_0_arg != k_material_off)
        normal_0 = calc_terrain_bumpmap(tangent, binormal, normal, texcoord, bump_map_m_0, bump_map_m_0_xform, detail_bump_m_0, detail_bump_m_0_xform);
    if (material_type_1_arg != k_material_off)
        normal_1 = calc_terrain_bumpmap(tangent, binormal, normal, texcoord, bump_map_m_1, bump_map_m_1_xform, detail_bump_m_1, detail_bump_m_1_xform);
    if (material_type_2_arg != k_material_off)
        normal_2 = calc_terrain_bumpmap(tangent, binormal, normal, texcoord, bump_map_m_2, bump_map_m_2_xform, detail_bump_m_2, detail_bump_m_2_xform);
    if (material_type_3_arg != k_material_off)
        normal_3 = calc_terrain_bumpmap(tangent, binormal, normal, texcoord, bump_map_m_3, bump_map_m_3_xform, detail_bump_m_3, detail_bump_m_3_xform);
	
    if (blend.x > 0)
        m_normal += normal_0 * blend.x;
    if (blend.y > 0)
        m_normal += normal_1 * blend.y;
    if (blend.z > 0)
        m_normal += normal_2 * blend.z;
    if (blend.w > 0)
        m_normal += normal_3 * blend.w;
	
    m_normal = normalize(m_normal);
    m_normal = m_normal.x * normalize(tangent) + m_normal.y * normalize(binormal) + m_normal.z * normalize(normal);
    
    // albedo
	
    if (material_type_0_arg != k_material_off)
        albedo_0 = calc_terrain_albedo(texcoord, base_map_m_0, base_map_m_0_xform, detail_map_m_0, detail_map_m_0_xform);
    if (material_type_1_arg != k_material_off)
        albedo_1 = calc_terrain_albedo(texcoord, base_map_m_1, base_map_m_1_xform, detail_map_m_1, detail_map_m_1_xform);
    if (material_type_2_arg != k_material_off)
        albedo_2 = calc_terrain_albedo(texcoord, base_map_m_2, base_map_m_2_xform, detail_map_m_2, detail_map_m_2_xform);
	if (material_type_3_arg != k_material_off)
		albedo_3 = calc_terrain_albedo(texcoord, base_map_m_3, base_map_m_3_xform, detail_map_m_3, detail_map_m_3_xform);
	
    if (blend.x > 0)
        albedo += albedo_0 * blend.x;
    if (blend.y > 0)
        albedo += albedo_1 * blend.y;
    if (blend.z > 0)
        albedo += albedo_2 * blend.z;
    if (blend.w > 0)
        albedo += albedo_3 * blend.w;

    float albedo_tint = DETAIL_MULTIPLIER * global_albedo_tint;
    albedo.rgb = albedo_tint * albedo.rgb;
	albedo.rgb = apply_debug_tint(albedo.rgb);
    albedo.rgb = rgb_to_srgb(albedo.rgb);
    
    PS_OUTPUT_ALBEDO output;
    
	output.diffuse = albedo;
    output.normal = float4(normal_export(m_normal), albedo.a);
	output.unknown = input.normal.wwww;
    
    return output;
}

#endif