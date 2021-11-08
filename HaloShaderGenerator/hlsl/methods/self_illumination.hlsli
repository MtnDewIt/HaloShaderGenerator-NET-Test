#ifndef _SELF_ILLUMINATION_HLSLI
#define _SELF_ILLUMINATION_HLSLI

#include "../helpers/math.hlsli"
#include "../helpers/types.hlsli"
#include "../helpers/color_processing.hlsli"

#include "albedo.hlsli"

uniform float self_illum_intensity;
uniform xform2d self_illum_map_xform;
uniform sampler2D self_illum_map;
uniform float4 self_illum_color;
uniform xform2d self_illum_detail_map_xform;
uniform sampler self_illum_detail_map;
uniform xform2d alpha_mask_map_xform;
uniform sampler alpha_mask_map;
uniform xform2d noise_map_a_xform;
uniform sampler noise_map_a;
uniform xform2d noise_map_b_xform;
uniform sampler noise_map_b;
uniform float4 color_medium;
uniform float4 color_sharp;
uniform float4 color_wide;
uniform float thinness_medium;
uniform float thinness_sharp;
uniform float thinness_wide;
uniform float4 channel_a;
uniform float4 channel_b;
uniform float4 channel_c;
uniform sampler2D meter_map;
uniform float4 meter_map_xform;
uniform float4 meter_color_off;
uniform float4 meter_color_on;
uniform float meter_value;

uniform float primary_change_color_blend;

// multilayer_additive, ml_add_four_change_color, ml_add_five_change_color
#if shadertype != k_shadertype_cortana
uniform float layer_depth;
uniform float layer_contrast;
uniform float texcoord_aspect_ratio;
uniform float depth_darken;
#endif
uniform int layers_of_4; // global

uniform float3 self_illum_heat_color;
uniform float alpha_modulation_factor;
uniform sampler2D palette;
uniform float v_coordinate;
uniform float depth_fade_range;

void calc_self_illumination_none_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
}

void calc_self_illumination_simple_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
	float2 self_illum_map_texcoord = apply_xform2d(texcoord, self_illum_map_xform);
    float3 self_illum_map_sample = tex2D(self_illum_map, self_illum_map_texcoord).rgb;
	self_illum_map_sample *= self_illum_color.rgb;
	self_illum_map_sample *= self_illum_intensity;
	self_illum_map_sample *= g_alt_exposure.x;

	diffuse += self_illum_map_sample;
}

void calc_self_illumination_three_channel_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
	float2 self_illum_map_texcoord = apply_xform2d(texcoord, self_illum_map_xform);
    float3 self_illum_map_sample = tex2D(self_illum_map, self_illum_map_texcoord).rgb;
	float3 color = float3(0, 0, 0);
	
    self_illum_map_sample.r *= channel_a.a;
    color.rgb += self_illum_map_sample.r * channel_a.rgb;
	self_illum_map_sample.g *= channel_b.a;
    color.rgb += self_illum_map_sample.g * channel_b.rgb;
	self_illum_map_sample.b *= channel_c.a;
    color.rgb += self_illum_map_sample.b * channel_c.rgb;
	
    color.rgb *= self_illum_intensity;
	
    //color.rgb *= g_alt_exposure.x;
    //diffuse += color;
	
    diffuse = color * g_alt_exposure.x + diffuse;
}

void calc_self_illumination_plasma_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
    float2 alpha_map_texcoord = apply_xform2d(texcoord, alpha_mask_map_xform);
    float4 alpha_mask_map_sample = tex2D(alpha_mask_map, alpha_map_texcoord);
    float2 noise_a_texcoord = apply_xform2d(texcoord, noise_map_a_xform);
    float2 noise_b_texcoord = apply_xform2d(texcoord, noise_map_b_xform);
	
    float4 noise_map_a_sample = tex2D(noise_map_a, noise_a_texcoord);
	float4 noise_map_b_sample = tex2D(noise_map_b, noise_b_texcoord);

    float noise = 1.0 - abs(noise_map_a_sample.x - noise_map_b_sample.x);
	
	float noise_medium = pow(noise, thinness_medium);
	float noise_sharp =  pow(noise, thinness_sharp);
    float noise_wide =   pow(noise, thinness_wide);
	
    // These three noise components represent the full [0-1] range
	
    noise_wide -= noise_medium;
    noise_medium -= noise_sharp;

    float3 color = color_medium.rgb * color_medium.a * noise_medium;
    color += color_sharp.rgb * color_sharp.a * noise_sharp;
    color += color_wide.rgb * color_wide.a * noise_wide;
    color *= alpha_mask_map_sample.a;
	
    color *= self_illum_intensity;
    color *= g_alt_exposure.x;

	diffuse += color;
}

void calc_self_illumination_from_diffuse_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
	float3 color = albedo.rgb;
	color.rgb *= self_illum_color.rgb;
	color.rgb *= self_illum_intensity;
	color.rgb *= g_alt_exposure.x;
	diffuse = color;
}

void calc_self_illumination_detail_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
	float2 self_illum_map_texcoord = apply_xform2d(texcoord, self_illum_map_xform);
	float4 self_illum_map_sample = tex2D(self_illum_map, self_illum_map_texcoord);
	float2 self_illum_detail_map_texcoord = apply_xform2d(texcoord, self_illum_detail_map_xform);
	float4 self_illum_detail_map_sample = tex2D(self_illum_detail_map, self_illum_detail_map_texcoord);
	
	self_illum_map_sample.rgb *= self_illum_detail_map_sample.rgb * DETAIL_MULTIPLIER;
	self_illum_map_sample.rgb *= self_illum_color.rgb;
	self_illum_map_sample.rgb *= self_illum_intensity;
	self_illum_map_sample.rgb *= g_alt_exposure.x;

	diffuse += self_illum_map_sample.rgb;
}

void calc_self_illumination_meter_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
    float2 meter_map_texcoord = apply_xform2d(texcoord, meter_map_xform);
    float4 meter_map_sample = tex2D(meter_map, meter_map_texcoord);
	float3 color;
	
	if (meter_map_sample.x - 0.5 < 0)
		color = 0;
	else
	{
		if (-meter_map_sample.w + meter_value < 0)
			color = meter_color_off.rgb;
		else
			color = meter_color_on.rgb;

		color *= g_alt_exposure.x;
	}
	
	diffuse += color;
}

void calc_self_illumination_times_diffuse_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
	float2 self_illum_map_texcoord = apply_xform2d(texcoord, self_illum_map_xform);
	float4 self_illum_map_sample = tex2D(self_illum_map, self_illum_map_texcoord);
    float a = max(0, 10 * self_illum_map_sample.y - 9);
	
    float3 result = (a + albedo * (1.0f - a)) * ((primary_change_color_blend * primary_change_color) + (1.0 - primary_change_color_blend) * self_illum_color.rgb);
	
	result.rgb *= self_illum_intensity;
	result.rgb *= self_illum_map_sample.rgb;
	result.rgb *= g_alt_exposure.x;

	diffuse = result;
}

void calc_self_illumination_simple_with_alpha_mask_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
	float2 self_illum_map_texcoord = apply_xform2d(texcoord, self_illum_map_xform);
	float4 self_illum_map_sample = tex2D(self_illum_map, self_illum_map_texcoord);
	self_illum_map_sample *= self_illum_color;
	self_illum_map_sample.rgb *= (self_illum_intensity * self_illum_map_sample.a);
	self_illum_map_sample.rgb *= g_alt_exposure.x;

	diffuse += self_illum_map_sample.rgb;
}

void calc_self_illumination_simple_four_change_color_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
	float2 self_illum_map_texcoord = apply_xform2d(texcoord, self_illum_map_xform);
	float3 self_illum_map_sample = tex2D(self_illum_map, self_illum_map_texcoord).rgb;
	self_illum_map_sample *= self_illum_color.rgb;
	self_illum_map_sample *= self_illum_intensity;
	self_illum_map_sample *= g_alt_exposure.x;

	diffuse += self_illum_map_sample;
}

void calc_self_illumination_multilayer_additive_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
    float3 final_color = float3(0.0f, 0.0f, 0.0f);
	
    float2 aspected_xform = self_illum_map_xform.xy * float2(view_tangent, view_binormal) * float2(texcoord_aspect_ratio, 1.0f) * layer_depth;
	
    if (shaderstage == k_shaderstage_static_per_vertex_color)
        aspected_xform = float2(0.0f, 0.0f);
		
    float2 self_illum_map_texcoord = apply_xform2d(texcoord, self_illum_map_xform);
		
    float layer_darkness = 1.0f;
    for (int i = 0; i < layers_of_4; i++)
    {
        float4 sample_0 = tex2D(self_illum_map, self_illum_map_texcoord);
		
        sample_0.rgb = layer_darkness * sample_0.rgb + final_color;
        self_illum_map_texcoord = aspected_xform * -rcp(4.0f * layers_of_4) + self_illum_map_texcoord;
        layer_darkness = layer_darkness * depth_darken;
		
        float4 sample_1 = tex2D(self_illum_map, self_illum_map_texcoord);
        if (shaderstage == k_shaderstage_static_per_vertex_color)
            sample_1.rgb = sample_0.rgb;
		
        sample_0.rgb = layer_darkness * sample_1.rgb + sample_0.rgb;
        self_illum_map_texcoord = aspected_xform * -rcp(4.0f * layers_of_4) + self_illum_map_texcoord;
        layer_darkness = layer_darkness * depth_darken;
		
        float4 sample_2 = tex2D(self_illum_map, self_illum_map_texcoord);
        if (shaderstage == k_shaderstage_static_per_vertex_color)
            sample_2.rgb = sample_0.rgb;
		
        sample_0.rgb = layer_darkness * sample_2.rgb + sample_0.rgb;
        self_illum_map_texcoord = aspected_xform * -rcp(4.0f * layers_of_4) + self_illum_map_texcoord;
        layer_darkness = layer_darkness * depth_darken;
		
        float4 sample_3 = tex2D(self_illum_map, self_illum_map_texcoord);
        if (shaderstage == k_shaderstage_static_per_vertex_color)
            sample_3.rgb = sample_0.rgb;
		
        final_color = layer_darkness * sample_3.rgb + sample_0.rgb;
        self_illum_map_texcoord = aspected_xform * -rcp(4.0f * layers_of_4) + self_illum_map_texcoord;
        layer_darkness = layer_darkness * depth_darken;
    }
		
    final_color *= rcp(4.0f * layers_of_4);
    final_color = pow(final_color, layer_contrast);
	
    final_color *= self_illum_color.rgb;
    final_color *= self_illum_intensity;
    final_color *= g_alt_exposure.x;
	
    diffuse += final_color;
}

void calc_self_illumination_scope_blur_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
    float4 scope_blur_bound_modifiers = float4(0.000867999974, 0.00156250002, -0.000867999974, -0.00156250002);

    float2 scope_blur_texcoord = apply_xform2d(texcoord, self_illum_map_xform);
    float4 bound_texcoords = float4(scope_blur_texcoord, scope_blur_texcoord) + scope_blur_bound_modifiers;
	
    float4 sample_0 = tex2D(self_illum_map, bound_texcoords.xy);
    float4 sample_1 = tex2D(self_illum_map, bound_texcoords.zy);
    float4 sample_2 = tex2D(self_illum_map, bound_texcoords.zw);
    float4 sample_3 = tex2D(self_illum_map, bound_texcoords.xw);
	
    float2 _color = sample_0.xy;
    _color += sample_1.xy;
    _color += sample_2.xy;
    _color += sample_3.xy;
	
    _color *= 0.25f;
    _color.y = (1.0f - _color.x) * _color.y;

    float3 final_color = _color.y * self_illum_heat_color.rgb;
    final_color = _color.x * self_illum_color.rgb + final_color;
    final_color *= self_illum_intensity;
    final_color *= g_alt_exposure.x;
	
    diffuse += final_color;
}

void calc_self_illumination_palettized_plasma_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
    float alpha_mask = tex2D(alpha_mask_map, apply_xform2d(texcoord, alpha_mask_map_xform)).a;
    float noise_a = tex2D(noise_map_a, apply_xform2d(texcoord, noise_map_a_xform)).r;
    float noise_b = tex2D(noise_map_b, apply_xform2d(texcoord, noise_map_b_xform)).r;
    float plasma_noise = abs(noise_a - noise_b);
    
    float depth = tex2D(depth_buffer, (position.xy + 0.5f) / texture_size.xy).r; // * global_depth_constants.y + global_depth_constants.z;
    depth = -abs(dot(camera_dir, global_camera_forward)) + depth; // + rcp(depth);
    
    float plasma_depth_modulation = saturate((n_camera_dir / depth_fade_range) * depth);
        
    float u_coordinate = alpha_mask * -plasma_depth_modulation + 1.0f;
    u_coordinate = saturate(u_coordinate * alpha_modulation_factor.x + plasma_noise);
        
    float3 color = tex2D(palette, float2(u_coordinate, v_coordinate)).rgb;
	
    color *= self_illum_color.rgb;
    color *= self_illum_intensity;
    color *= g_alt_exposure.x;
    
    diffuse += color;
}

uniform sampler2D ceiling;
uniform float4 ceiling_xform;
uniform float distance_fade_scale;
uniform sampler2D opacity_map;
uniform float4 opacity_map_xform;
uniform sampler2D walls;
uniform float4 walls_xform;
uniform float4 transform_xform;
uniform sampler2D floors;
uniform float4 floors_xform;

// this can be removed, i think step(src2, src1) is hlsl equivalent
float2 sgt(in float2 src1, in float2 src2)
{
    return float2(src1.x > src2.x ? 1.0f : 0.0f, src1.y > src2.y ? 1.0f : 0.0f);
}

void calc_self_illumination_window_room_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
    float dir_dot_normal = n_view.x;
    float2 dir_tangent_binormal = float2(n_view.z, n_view.y);
    
    float2 xform = transform_xform.zw + 0.5f;
    xform += texcoord * transform_xform.xy;
    
    float2 dir_var = sgt(-dir_tangent_binormal.xy, dir_tangent_binormal.xy) - sgt(dir_tangent_binormal.xy, -dir_tangent_binormal.xy);
    dir_var = floor(dir_var * 0.5f + xform);
    dir_var -= transform_xform.zw;
    dir_var = dir_var / transform_xform.xy - texcoord;
    dir_var /= -dir_tangent_binormal.xy;
    
    float min_t = min(dir_var.x, dir_var.y);
    float2 transformed_tex = -dir_tangent_binormal.xy * min_t + texcoord;
    float min_v_normal = -dir_dot_normal * min_t;
    
    
    float3 self_illum_sample;
    
    //sgt(dir_var.y, dir_var.x)
    if (dir_var.y <= dir_var.x) // belongs to ceiling or floor (top\bottom)
    {
        if (-dir_tangent_binormal.y > 0)
        {
            self_illum_sample = tex2D(floors, apply_xform2d(float2(transformed_tex.x, min_v_normal), floors_xform)).rgb;
        }
        else
        {
            self_illum_sample = tex2D(ceiling, apply_xform2d(float2(transformed_tex.x, min_v_normal), ceiling_xform)).rgb;
        }
    }
    else // belongs to wall (left\right)
    {
        self_illum_sample = tex2D(walls, apply_xform2d(float2(min_v_normal, transformed_tex.y), walls_xform)).rgb;
    }
    
    float3 opacity = tex2D(opacity_map, apply_xform2d(texcoord, opacity_map_xform)).rgb;
    
    float distance_fade = min_t * distance_fade_scale * -dir_dot_normal;
    distance_fade = saturate(distance_fade + 1.0f);
    
    self_illum_sample *= self_illum_intensity;
    self_illum_sample *= distance_fade;
    self_illum_sample *= opacity;
    self_illum_sample *= g_alt_exposure.x;
    
    diffuse += self_illum_sample;
}

void calc_self_illumination_illum_change_color_ps(
in float2 position,
in float2 texcoord,
in float3 albedo,
in float3 n_view,
in float3 camera_dir,
in float n_camera_dir,
in float view_tangent,
in float view_binormal,
inout float3 diffuse)
{
    float2 self_illum_map_texcoord = apply_xform2d(texcoord, self_illum_map_xform);
    float3 self_illum_map_sample = tex2D(self_illum_map, self_illum_map_texcoord).rgb;
    self_illum_map_sample *= ((primary_change_color_blend * self_illum_color.rgb) + (1.0f - primary_change_color_blend) * self_illum_color.rgb);
    self_illum_map_sample *= self_illum_intensity;
    self_illum_map_sample *= g_alt_exposure.x;

    diffuse += self_illum_map_sample;
}


// fixups
#define calc_self_illumination_off_ps calc_self_illumination_none_ps
#define calc_self_illumination_3_channel_self_illum_ps calc_self_illumination_three_channel_ps
#define calc_self_illumination__3_channel_self_illum_ps calc_self_illumination_three_channel_ps
#define calc_self_illumination_illum_detail_ps calc_self_illumination_detail_ps
#define calc_self_illumination_self_illum_times_diffuse_ps calc_self_illumination_times_diffuse_ps

#ifndef calc_self_illumination_ps
#define calc_self_illumination_ps calc_self_illumination_none_ps
#endif

#endif
