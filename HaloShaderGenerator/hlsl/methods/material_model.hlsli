#ifndef _MATERIAL_MODEL_HLSLI
#define _MATERIAL_MODEL_HLSLI

#include "../helpers/definition_helper.hlsli"

uniform bool no_dynamic_lights;

#if material_type_arg == k_material_model_diffuse_only
#include "../shader_lighting/diffuse_only_lighting.hlsli"
#endif

#if material_type_arg == k_material_model_cook_torrance
#include "../shader_lighting/cook_torrance_lighting.hlsli"
#endif

#if material_type_arg == k_material_model_foliage
#include "../shader_lighting/foliage_lighting.hlsli"
#endif

#if material_type_arg == k_material_model_two_lobe_phong
#include "../shader_lighting/two_lobe_phong_lighting.hlsli"
#endif

#if material_type_arg == k_material_model_single_lobe_phong
#include "../shader_lighting/single_lobe_phong_lighting.hlsli"
#endif

#if material_type_arg == k_material_model_organism
#include "../shader_lighting/organism_lighting.hlsli"
#endif

#if material_type_arg == k_material_model_glass
#include "../shader_lighting/glass_lighting.hlsli"
#endif

#if material_type_arg == k_material_model_custom_specular
#include "../shader_lighting/custom_specular_lighting.hlsli"
#endif

#if material_type_arg == k_material_model_hair
#include "../shader_lighting/hair_lighting.hlsli"
#endif

#include "../shader_lighting/no_material_lighting.hlsli"
#include "../shader_lighting/none_lighting.hlsli"
#include "../shader_lighting/per_vertex_color_lighting.hlsli"

#ifndef calc_lighting_ps
#define calc_lighting_ps calc_lighting_none_ps
#endif

#ifndef calc_dynamic_lighting_ps
#define calc_dynamic_lighting_ps calc_dynamic_lighting_none_ps
#endif

#ifndef calc_material_analytic_specular
#define calc_material_analytic_specular calc_material_analytic_specular_none_ps
#endif

#ifndef calc_material_area_specular
#define calc_material_area_specular calc_material_area_specular_none_ps
#endif

#endif
