﻿/*
This file contains a list of blank definitions to help with Intellisense
*/

#ifndef _DEFINITION_HELPER_HLSLI
#define _DEFINITION_HELPER_HLSLI

// Enums

#define k_shaderstage_default 0
#define k_shaderstage_albedo 1
#define k_shaderstage_static_default 2
#define k_shaderstage_static_per_pixel 3
#define k_shaderstage_static_per_vertex 4
#define k_shaderstage_static_sh 5
#define k_shaderstage_static_prt_ambient 6
#define k_shaderstage_static_prt_linear 7
#define k_shaderstage_static_prt_quadratic 8
#define k_shaderstage_dynamic_light 9
#define k_shaderstage_shadow_generate 10
#define k_shaderstage_shadow_apply 11
#define k_shaderstage_active_camo 12
#define k_shaderstage_lightmap_debug_mode 13
#define k_shaderstage_static_per_vertex_color 14
#define k_shaderstage_water_tesselation 15
#define k_shaderstage_water_shading 16
#define k_shaderstage_dynamic_light_cinematic 17
#define k_shaderstage_z_only 18
#define k_shaderstage_sfx_distort 19

#define k_shadertype_shader 0
#define k_shadertype_beam 1
#define k_shadertype_contrail 2
#define k_shadertype_decal 3
#define k_shadertype_halogram 4
#define k_shadertype_light_volume 5
#define k_shadertype_particle 6
#define k_shadertype_terrain 7
#define k_shadertype_black 8
#define k_shadertype_custom 9
#define k_shadertype_water 10
#define k_shadertype_foliage 11
#define k_shadertype_glass 12
#define k_shadertype_cortana 13
#define k_shadertype_screen 14
#define k_shadertype_zonly 15

// Global Method Enums

#define k_albedo_default 0
#define k_albedo_detail_blend 1
#define k_albedo_constant_color 2
#define k_albedo_two_change_color 3
#define k_albedo_four_change_color 4
#define k_albedo_three_detail_blend 5
#define k_albedo_two_detail_overlay 6
#define k_albedo_two_detail 7
#define k_albedo_color_mask 8
#define k_albedo_two_detail_black_point 9
#define k_albedo_waterfall 10
#define k_albedo_multiply_map 11
#define k_albedo_two_change_color_anim_overlay 12
#define k_albedo_chameleon 13
#define k_albedo_two_change_color_chameleon 14
#define k_albedo_chameleon_masked 15
#define k_albedo_color_mask_hard_light 16
#define k_albedo_two_change_color_tex_overlay 17
#define k_albedo_chameleon_albedo_masked 18
#define k_albedo_diffuse_only 19
#define k_albedo_diffuse_plus_billboard_alpha 20
#define k_albedo_palettized 21
#define k_albedo_palettized_plus_billboard_alpha 22
#define k_albedo_diffuse_plus_sprite_alpha 23
#define k_albedo_palettized_plus_sprite_alpha 24
#define k_albedo_diffuse_modulated 25
#define k_albedo_palettized_glow 26
#define k_albedo_palettized_plasma 27
#define k_albedo_palettized_2d_plasma 28
#define k_albedo_circular 29
#define k_albedo_palettized_plus_alpha 30
#define k_albedo_diffuse_plus_alpha 31
#define k_albedo_emblem_change_color 32
#define k_albedo_change_color 33
#define k_albedo_diffuse_plus_alpha_mask 34
#define k_albedo_palettized_plus_alpha_mask 35
#define k_albedo_vector_alpha 36
#define k_albedo_vector_alpha_drop_shadow 37
#define k_albedo_four_change_color_applying_to_specular 38
#define k_albedo_simple 39
#define k_albedo_custom_cube 40
#define k_albedo_two_color 41
#define k_albedo_scrolling_cube_mask 42
#define k_albedo_scrolling_cube 43
#define k_albedo_scrolling_texture_uv 44
#define k_albedo_texture_from_misc 45
#define k_albedo_emblem 46

#define k_bump_mapping_off 0
#define k_bump_mapping_standard 1
#define k_bump_mapping_detail 2
#define k_bump_mapping_detail_masked 3
#define k_bump_mapping_detail_plus_detail_masked 4
#define k_alpha_test_none 0
#define k_alpha_test_simple 1
#define k_alpha_test_multiply_map 2
#define k_alpha_test_from_albedo_alpha 3
#define k_specular_mask_no_specular_mask 0
#define k_specular_mask_specular_mask_from_diffuse 1
#define k_specular_mask_specular_mask_from_texture 2
#define k_specular_mask_specular_mask_from_color_texture 3
#define k_material_model_diffuse_only 0
#define k_material_model_cook_torrance 1
#define k_material_model_two_lobe_phong 2
#define k_material_model_foliage 3
#define k_material_model_none 4
#define k_material_model_glass 5
#define k_material_model_organism 6
#define k_material_model_single_lobe_phong 7
#define k_material_model_car_paint 8
#define k_material_model_hair 9
#define k_material_model_custom_specular 10
#define k_environment_mapping_none 0
#define k_environment_mapping_per_pixel 1
#define k_environment_mapping_dynamic 2
#define k_environment_mapping_from_flat_texture 3
#define k_environment_mapping_custom_map 4
#define k_environment_mapping_per_pixel_mip 5
#define k_self_illumination_off 0
#define k_self_illumination_simple 1
#define k_self_illumination__3_channel_self_illum 2
#define k_self_illumination_plasma 3
#define k_self_illumination_from_diffuse 4
#define k_self_illumination_illum_detail 5
#define k_self_illumination_meter 6
#define k_self_illumination_self_illum_times_diffuse 7
#define k_self_illumination_simple_with_alpha_mask 8
#define k_self_illumination_simple_four_change_color 9
#define k_self_illumination_none 10
#define k_self_illumination_multilayer_additive 11
#define k_self_illumination_ml_add_four_change_color 12
#define k_self_illumination_ml_add_five_change_color 13
#define k_self_illumination_scope_blur 14
#define k_self_illumination_palettized_plasma 15
#define k_self_illumination_palettized_plasma_change_color 16
#define k_self_illumination_constant_color 17
#define k_self_illumination_window_room 18
#define k_blend_mode_opaque 0
#define k_blend_mode_additive 1
#define k_blend_mode_multiply 2
#define k_blend_mode_alpha_blend 3
#define k_blend_mode_double_multiply 4
#define k_blend_mode_maximum 5
#define k_blend_mode_multiply_add 6
#define k_blend_mode_add_src_times_dstalpha 7
#define k_blend_mode_add_src_times_srcalpha 8
#define k_blend_mode_inv_alpha_blend 9
#define k_blend_mode_pre_multiplied_alpha 10
#define k_parallax_off 0
#define k_parallax_simple 1
#define k_parallax_interpolated 2
#define k_parallax_simple_detail 3
#define k_misc_first_person_never 0
#define k_misc_first_person_sometimes 1
#define k_misc_first_person_always 2
#define k_misc_first_person_never_with_rotating_bitmaps 3
#define k_misc_always_calc_albedo 4
#define k_distortion_off 0
#define k_distortion_on 1
#define k_soft_fade_off 0
#define k_soft_fade_on 1
#define k_specialized_rendering_none 0
#define k_specialized_rendering_distortion 1
#define k_specialized_rendering_distortion_expensive 2
#define k_specialized_rendering_distortion_diffuse 3
#define k_specialized_rendering_distortion_expensive_diffuse 4
#define k_specialized_rendering_none 0
#define k_lighting_none 0
#define k_lighting_per_pixel_ravi_order_3 1
#define k_lighting_per_vertex_ravi_order_0 2
#define k_depth_fade_off 0
#define k_depth_fade_on 1
#define k_render_targets_ldr_and_hdr 0
#define k_render_targets_ldr_only 1
#define k_black_point_off 0
#define k_black_point_on 1
#define k_fog_off 0
#define k_fog_on 1
#define k_frame_blend_off 0
#define k_frame_blend_on 1
#define k_warp_none 0
#define k_warp_pixel_space 1
#define k_warp_screen_space 2
#define k_base_single_screen_space 0
#define k_base_single_pixel_space 1
#define k_overlay_a_none 0
#define k_overlay_a_tint_add_color 1
#define k_overlay_a_detail_screen_space 2
#define k_overlay_a_detail_pixel_space 3
#define k_overlay_a_detail_masked_screen_space 4
#define k_overlay_b_none 0
#define k_overlay_b_tint_add_color 1

#define k_waveshape_default 0
#define k_waveshape_none 1
#define k_waveshape_bump 2
#define k_refraction_none 0
#define k_refraction_dynamic 1
#define k_reflection_none 0
#define k_reflection_static 1
#define k_reflection_dynamic 2
#define k_watercolor_pure 0
#define k_watercolor_texture 1
#define k_foam_none 0
#define k_foam_auto 1
#define k_foam_paint 2
#define k_foam_both 3
#define k_bankalpha_none 0
#define k_bankalpha_depth 1
#define k_bankalpha_paint 2
#define k_bankalpha_from_shape_texture_alpha 3
#define k_global_shape_none 0
#define k_global_shape_paint 1
#define k_global_shape_depth 2
#define k_reach_compatibility_disabled 0
#define k_reach_compatibility_enabled 1

// Generics
#define shaderstage 0
#define shadertype 0

// Current Method Arguments

#define albedo_arg 0
#define self_illumination_arg 0
#define material_type_arg 0
#define envmap_type_arg 0
#define blend_type_arg 0
#define misc_arg 0
#define distortion_arg 0
#define soft_fade_arg 0
#define specialized_rendering_arg 0
#define render_targets_arg 0
#define lighting_arg 0
#define depth_fade_arg 0
#define black_point_arg 0
#define fog_arg 0
#define frame_blend_arg 0
#define decal_render_pass_arg 0
#define decal_specular_arg 0
#define decal_bump_mapping_arg 0
#define decal_tinting_arg 0
#define warp_arg 0
#define base_arg 0
#define overlay_a_arg 0
#define overlay_b_arg 0
#define alpha_test_arg 0

#define waveshape_arg 0
#define watercolor_arg 0
#define reflection_arg 0
#define refraction_arg 0
#define bankalpha_arg 0
#define appearance_arg 0
#define global_shape_arg 0
#define foam_arg 0
#define reach_compatibility_arg 0

#endif


