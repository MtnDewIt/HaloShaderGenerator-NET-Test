#ifndef _CHUD_PARAMETERS_HLSLI
#define _CHUD_PARAMETERS_HLSLI

#ifndef VERTEX_SHADER

uniform bool chud_cortana_pixel : register(b7);
uniform bool chud_comp_colorize_enabled : register(b8);

uniform float4 g_exposure : register(c0);
uniform float4 chud_color_output_A : register(c24);
uniform float4 chud_color_output_B : register(c25);
uniform float4 chud_color_output_C : register(c26);
uniform float4 chud_color_output_D : register(c27);
uniform float4 chud_color_output_E : register(c28);
uniform float4 chud_color_output_F : register(c29);
uniform float4 chud_scalar_output_ABCD : register(c30);
uniform float4 chud_scalar_output_EF : register(c31);
uniform float4 chud_texture_bounds : register(c32);
uniform float4 UNKNOWN_CONSTANT_33 : register(c33);
uniform float4 chud_screen_flash0_color : register(c34);
uniform float4 chud_screen_flash0_data : register(c35);
uniform float4 chud_screen_flash1_color : register(c36);
uniform float4 chud_screen_flash1_data : register(c37);
uniform float4 chud_screen_flash2_color : register(c38);
uniform float4 chud_screen_flash2_data : register(c39);
uniform float4 chud_screen_flash3_color : register(c40);
uniform float4 chud_screen_flash3_data : register(c41);
uniform float4 chud_screen_flash_center : register(c42); // crosshair pixel location <resolution * 0.5, resolution * 0.3>
uniform float4 chud_widget_transform1_ps : register(c50);
uniform float4 chud_widget_transform2_ps : register(c51);
uniform float4 chud_widget_transform3_ps : register(c52);
uniform float4 chud_widget_mirror_ps : register(c53);
// unconfirmed if 54-56 exist here. might be some spacing in case they wanted to change some stuff
uniform float4 cortana_back_colormix_result : register(c57);
uniform float4 cortana_hsv_data : register(c58); // made up name <hue, saturation, value, 0>
uniform float4 cortana_background_color : register(c59); // made up name <color.rgb, 0>
uniform float4 cortana_comp_solarize_inmix : register(c60);
uniform float4 cortana_comp_solarize_outmix : register(c61);
uniform float4 cortana_comp_solarize_result : register(c62);
uniform float4 cortana_comp_doubling_inmix : register(c63);
uniform float4 cortana_comp_doubling_outmix : register(c64);
uniform float4 cortana_comp_doubling_result : register(c65);
uniform float4 cortana_comp_colorize_inmix : register(c66);
uniform float4 cortana_comp_colorize_outmix : register(c67);
uniform float4 cortana_comp_colorize_result : register(c68);
uniform float4 cortana_comp_unknown_inmix : register(c69);
uniform float4 cortana_comp_unknown_outmix : register(c70);
uniform float4 cortana_comp_unknown_result : register(c71);
uniform float4 cortana_vignette_data : register(c72);

uniform sampler2D basemap_sampler : register(s0); // tex0_sampler in chud_emblem, TODO fix

#define g_hud_alpha chud_scalar_output_EF.w

#else // vertex shader

uniform bool chud_cortana_vertex : register(b7);

uniform float4 chud_widget_offset : register(c19);
uniform float4 chud_widget_transform1 : register(c20);
uniform float4 chud_widget_transform2 : register(c21);
uniform float4 chud_widget_transform3 : register(c22);
uniform float4 chud_screen_size : register(c23);
uniform float4 chud_basis_0 : register(c24);
uniform float4 chud_basis_1 : register(c25);
uniform float4 chud_basis_2 : register(c26);
uniform float4 chud_basis_3 : register(c27);
uniform float4 chud_screen_scale_and_offset : register(c28);
uniform float4 chud_project_scale_and_offset : register(c29);
uniform float4 chud_widget_mirror_ps : register(c30);
uniform float4 chud_screenshot_info : register(c31);
uniform float4 chud_texture_transform : register(c32);

#endif // #ifndef VERTEX_SHADER


#endif
