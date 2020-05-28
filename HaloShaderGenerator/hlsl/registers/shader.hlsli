#ifndef _SHADER_HLSLI
#define _SHADER_HLSLI

#include "../helpers/types.hlsli"
#include "../helpers/definition_helper.hlsli"

#define aspect_ratio float2(16, 9)
#define shadowmap_texture_size 512
#define default_lightmap_size 1024

#if misc_arg == k_misc_first_person_sometimes || misc_arg == k_misc_first_person_always || blend_type_arg == k_blend_mode_additive || blend_type_arg == k_blend_mode_double_multiply || blend_type_arg ==  k_blend_mode_multiply || blend_type_arg == k_blend_mode_alpha_blend || blend_type_arg == k_blend_mode_pre_multiplied_alpha
uniform bool actually_calc_albedo : register(b12);
#else
#define actually_calc_albedo false
#endif
//
// Macros for shader generation
//
/* Most of these macros could be set from the C# side where depending on the methods some macros are set to true or false.
*/


#if blend_type_arg ==  k_blend_mode_double_multiply || blend_type_arg ==  k_blend_mode_multiply
#define color_export_multiply_alpha true
#else
#define color_export_multiply_alpha false
#endif

#if shaderstage == k_shaderstage_dynamic_light || shaderstage == k_shaderstage_dynamic_light_cinematic
#define is_dynamic_light true
#else
#define is_dynamic_light false
#endif



#if blend_type_arg == k_blend_mode_double_multiply || blend_type_arg ==  k_blend_mode_multiply || self_illumination_arg == k_self_illumination_from_diffuse
#define calc_material false
#else
#define calc_material true
#endif


uniform float4 g_exposure : register(c0);
uniform float4 p_lighting_constant_0 : register(c1);
uniform float4 p_lighting_constant_1 : register(c2);
uniform float4 p_lighting_constant_2 : register(c3);
uniform float4 p_lighting_constant_3 : register(c4);
uniform float4 p_lighting_constant_4 : register(c5);
uniform float4 p_lighting_constant_5 : register(c6);
uniform float4 p_lighting_constant_6 : register(c7);
uniform float4 p_lighting_constant_7 : register(c8);
uniform float4 p_lighting_constant_8 : register(c9);
uniform float4 p_lighting_constant_9 : register(c10);
uniform float4 k_ps_dominant_light_direction : register(c11);
uniform float4 g_alt_exposure : register(c12);
uniform float4 k_ps_dominant_light_intensity : register(c13);
uniform float2 texture_size : register(c14);
uniform float4 dynamic_environment_blend : register(c15);
uniform float3 Camera_Position_PS : register(c16);
uniform float simple_light_count : register(c17);
uniform float4 simple_lights[40] : register(c18);
uniform float4 p_render_debug_mode : register(c94);
uniform float4 primary_change_color_old : register(c190);
uniform float4 secondary_change_color_old : register(c191);
uniform float4 p_lightmap_compress_constant_0 : register(c210);
uniform float4 p_lightmap_compress_constant_1 : register(c211);
uniform float4 k_ps_active_camo_factor : register(c212);
uniform float4 p_atmosphere_constant_0 : register(c215);
uniform float4 p_atmosphere_constant_extra : register(c221);

uniform bool dynamic_light_shadowing : register(b13);
uniform xform2d p_dynamic_light_gel_xform : register(c5);

// no idea why this is so, this seems to disappear when hightmaps are present :/
// we need a better solution for this
//NOTE: We should be able to macro this out

#if envmap_type_arg != k_environment_mapping_dynamic
uniform sampler __unknown_s1 : register(s1);
#endif




uniform bool k_is_lightmap_exist;
uniform bool k_is_water_interaction;
uniform int layers_of_4;



uniform sampler dynamic_light_gel_texture;
uniform sampler shadow_depth_map_1;
uniform sampler2D scene_ldr_texture;
uniform sampler2D albedo_texture;
uniform sampler2D normal_texture;


// material model parameters

// BEGIN ENERGY SWORD VARIABLES, NOT SURE WHERE THESE BELONG YET

uniform float4 alpha_mask_map_xform;
uniform sampler alpha_mask_map;

uniform float4 noise_map_a_xform;
uniform sampler noise_map_a;

uniform float4 noise_map_b_xform;
uniform sampler noise_map_b;

uniform float4 color_medium;
uniform float4 color_sharp;
uniform float4 color_wide;

uniform float thinness_medium;
uniform float thinness_sharp;
uniform float thinness_wide;

// END ENERGY SWORD VARIABLES

uniform float diffuse_coefficient;
uniform float specular_coefficient;

uniform float area_specular_contribution;
uniform float analytical_specular_contribution;
uniform float environment_map_specular_contribution;

//TODO: Figure out location of this
//uniform boolf order3_area_specular; // bool order3_area_specular; this is weird, its a bool sitting in a constant register?????? 

uniform float normal_specular_power;
uniform float3 normal_specular_tint;

// START


float3 fresnel_color;
float fresnel_power;
float roughness;
float albedo_blend;
float3 specular_tint;
bool albedo_blend_with_specular_tint;
float rim_fresnel_coefficient;
float3 rim_fresnel_color;
float rim_fresnel_power;
float rim_fresnel_albedo_blend;

// END

uniform float glancing_specular_power;
uniform float3 glancing_specular_tint;

uniform float fresnel_curve_steepness;
uniform float albedo_specular_tint_blend;
uniform float analytical_anti_shadow_control;

uniform float k_f0; // figure out what this is


#endif
