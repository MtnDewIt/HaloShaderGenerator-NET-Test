
#ifndef HLSL_DISPLACEMENT_REGISTERS
#define HLSL_DISPLACEMENT_REGISTERS 1

uniform bool do_distortion                : register(b2);
uniform int num_taps                      : register(i2);

// scale = g_motion_blur_scale * 2, 
// x = scale, y = ((g_motion_blur_max * 2000) / 4) / scale, z = scale / 6, w = 2(scale) / 6
uniform float4 pixel_blur_constants       : register(c158);
uniform float4 unknown_c159               : register(c159); // set in odst, unused
uniform float4x4 view_projection          : register(c160);
uniform float4x4 previous_view_projection : register(c164);
uniform float4x4 screen_to_world          : register(c168);
uniform float4 misc_values                : register(c172);
uniform float4 blur_max_and_scale         : register(c173);
uniform float4 crosshair_center           : register(c174);
uniform float4 zbuffer_xform              : register(c175);
uniform float4x4 combined3                : register(c188); // odst, combined previous_view_projection and screen_to_world

uniform float4 screen_constants           : register(c203); // xy = 1.0 / resolution, zw = screenshot_scale
uniform float4 window_bounds              : register(c204); // safe area to sample
uniform float4 distort_constants          : register(c205); // odst, xy = resolution * screen_scale, zw = xy * -0.50000763
uniform float4 resolution_constants       : register(c207); // odst, xy = resolution, zw = 1.0 / resolution
uniform float4 crosshair_constants        : register(c209); // odst, 

sampler2D displacement_sampler            : register(s0); // _surface_distortion
sampler2D ldr_buffer                      : register(s1); // _surface_post_LDR
sampler2D distortion_depth_buffer         : register(s3); // _surface_depth_fp32

#endif
