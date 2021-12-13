#ifndef _SHADER_HLSLI
#define _SHADER_HLSLI

#include "../helpers/types.hlsli"
#include "../helpers/definition_helper.hlsli"

#define x360_fudge_constant 0.015625f
#define pc_fudge_constant 0.0312509537f
#define aspect_ratio float2(16, 9)
#define shadowmap_texture_size 512
#define default_lightmap_size 1024

#if shadertype == k_shadertype_shader
#if misc_arg == k_misc_first_person_sometimes || misc_arg == k_misc_first_person_always || blend_type_arg == k_blend_mode_additive || blend_type_arg == k_blend_mode_double_multiply || blend_type_arg ==  k_blend_mode_multiply || blend_type_arg == k_blend_mode_alpha_blend || blend_type_arg == k_blend_mode_pre_multiplied_alpha
uniform bool actually_calc_albedo : register(b12);
#elif misc_arg == k_misc_always_calc_albedo
#define actually_calc_albedo true
#else
#define actually_calc_albedo false
#endif
#endif

//
// Macros for shader generation
//
/* Most of these macros could be set from the C# side where depending on the methods some macros are set to true or false.
*/


#if blend_type_arg == k_blend_mode_double_multiply || blend_type_arg ==  k_blend_mode_multiply
#define color_export_multiply_alpha true
#else
#define color_export_multiply_alpha false
#endif

#if shaderstage == k_shaderstage_dynamic_light || shaderstage == k_shaderstage_dynamic_light_cinematic
#define is_dynamic_light true
#else
#define is_dynamic_light false
#endif

#if self_illumination_arg == k_self_illumination_from_diffuse
#define self_illum_is_diffuse true
#else
#define self_illum_is_diffuse false
#endif

#if blend_type_arg == k_blend_mode_double_multiply || blend_type_arg ==  k_blend_mode_multiply
#define calc_material false
#else
#define calc_material true
#endif

#if self_illumination_arg == k_self_illumination_from_diffuse
#define calc_atmosphere_no_material true
#else
#define calc_atmosphere_no_material false
#endif

#if envmap_type_arg != k_environment_mapping_none
#define calc_env_output true
#else
#define calc_env_output false
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
uniform float4 g_alt_exposure : register(c12);
#if shadertype != k_shadertype_water
uniform float4 k_ps_dominant_light_direction : register(c11);
uniform float4 k_ps_dominant_light_intensity : register(c13);
#endif
uniform float2 texture_size : register(c14);
uniform float4 dynamic_environment_blend : register(c15);
uniform float3 Camera_Position_PS : register(c16);
uniform float simple_light_count : register(c17);
uniform float4 simple_lights[40] : register(c18);
#if shadertype == k_shadertype_decal
#ifndef DECAL_FADE
uniform float fade : register(c32);
#define DECAL_FADE fade
#endif
#endif
uniform float4 p_render_debug_mode : register(c94);
uniform float4 primary_change_color_old : register(c190);
uniform float4 secondary_change_color_old : register(c191);
uniform float3 depth_constants : register(c201);
uniform float4 p_lightmap_compress_constant_0 : register(c210);
uniform float4 p_lightmap_compress_constant_1 : register(c211);
uniform float4 k_ps_active_camo_factor : register(c212);
uniform float4 k_ps_distort_bounds : register(c213);
uniform float4 p_atmosphere_constant_0 : register(c215);
uniform float4 p_atmosphere_constant_extra : register(c221);

uniform bool dynamic_light_shadowing : register(b13);
uniform xform2d p_dynamic_light_gel_xform : register(c5);

// Order here is important

#if shadertype != k_shadertype_water

uniform sampler2D albedo_texture;
#ifndef DEPTH_BUFFER_DEFINED
uniform sampler2D depth_buffer;
#define DEPTH_BUFFER_DEFINED 1
#endif
uniform float3 screen_constants;
uniform float3 global_depth_constants; // extern
uniform float3 global_camera_forward; // extern

#if envmap_type_arg != k_environment_mapping_dynamic
#define environment_map_register : register(s1)
#else
#define environment_map_register
#endif

uniform samplerCUBE environment_map environment_map_register;

uniform sampler2D normal_texture;
uniform sampler3D lightprobe_texture_array;
uniform sampler3D dominant_light_intensity_map;
uniform sampler2D scene_ldr_texture;

#else
uniform sampler3D lightprobe_texture_array;
uniform sampler2D scene_ldr_texture;
#ifndef DEPTH_BUFFER_DEFINED
uniform sampler2D depth_buffer;
#define DEPTH_BUFFER_DEFINED 1
#endif
#endif

#endif
