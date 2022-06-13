#ifndef _ALBEDO_HLSLI
#define _ALBEDO_HLSLI

#include "../helpers/types.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/color_processing.hlsli"
#include "../helpers/definition_helper.hlsli"

uniform float4 albedo_color;
uniform float4 albedo_color2;
uniform float4 albedo_color3;
uniform sampler base_map;
uniform xform2d base_map_xform;

#if shadertype == k_shadertype_halogram
// TODO: find out why this occurs
uniform sampler _reserved : register(s1);
#endif

uniform sampler detail_map;
uniform xform2d detail_map_xform;
uniform float4 debug_tint;
uniform sampler detail_map2;
uniform xform2d detail_map2_xform;
uniform sampler change_color_map;
uniform xform2d change_color_map_xform;
uniform float3 primary_change_color;
uniform float3 secondary_change_color;
uniform float3 tertiary_change_color;
uniform float3 quaternary_change_color;
uniform sampler detail_map3;
uniform xform2d detail_map3_xform;
uniform sampler detail_map_overlay;
uniform xform2d detail_map_overlay_xform;
uniform sampler color_mask_map;
uniform xform2d color_mask_map_xform;
uniform float4 neutral_gray;

uniform float4 primary_change_color_anim;
uniform float4 secondary_change_color_anim;

//
// Shader
//

float3 apply_debug_tint(float3 color)
{
	float debug_tint_factor = DETAIL_MULTIPLIER;
	float3 positive_color = color * debug_tint_factor;
	float3 negative_tinted_color = debug_tint.rgb - color * debug_tint_factor;
	return positive_color + debug_tint.a * negative_tinted_color;
}

float4 calc_albedo_default_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
    float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
    float4 base_map_sample = tex2D(base_map, base_map_texcoord);
	float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);
    float4 detail_map_sample = tex2D(detail_map, detail_map_texcoord);
	float4 albedo = detail_map_sample * base_map_sample;
	albedo *= albedo_color;
	albedo.rgb = apply_debug_tint(albedo.rgb);
	return albedo;
}

float4 calc_albedo_detail_blend_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
    float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
    float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);
    float2 detail_map2_texcoord = apply_xform2d(texcoord, detail_map2_xform);

    float4 base_map_sample = tex2D(base_map, base_map_texcoord);
    float4 detail_map_sample = tex2D(detail_map, detail_map_texcoord);
    float4 detail_map2_sample = tex2D(detail_map2, detail_map2_texcoord);

    float4 blended_detail = lerp(detail_map_sample, detail_map2_sample, base_map_sample.w);
	float3 albedo = base_map_sample.rgb * blended_detail.rgb;
	albedo.rgb = apply_debug_tint(albedo.rgb);
	return float4(albedo, blended_detail.w);
}

float4 calc_albedo_constant_color_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
	float3 albedo = lerp(albedo_color.rgb, debug_tint.rgb, debug_tint.w);
	return float4(albedo, albedo_color.a);
}

float4 calc_albedo_two_change_color_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
	float3 primary_change;
	float3 secondary_change;

    
	float old_contrib = position.y * secondary_change_color_old.w - primary_change_color_old.w;
	old_contrib = saturate(old_contrib * 15.0 + 0.5);
    
	primary_change = old_contrib * (primary_change_color_old.rgb - primary_change_color.rgb) + primary_change_color.rgb;
	secondary_change = lerp(secondary_change_color, secondary_change_color_old.rgb, old_contrib);
    
    
    float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
    float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);
    float2 change_color_map_texcoord = apply_xform2d(texcoord, change_color_map_xform);

	float4 base_map_sample = tex2D(base_map, base_map_texcoord);
    float4 detail_map_sample = tex2D(detail_map, detail_map_texcoord);
    float4 change_color_map_sample = tex2D(change_color_map, change_color_map_texcoord);

    float2 change_color_value = change_color_map_sample.xy;
    float2 change_color_value_invert = 1.0 - change_color_value;
	
	float3 change_primary = change_color_value.x * primary_change.rgb + change_color_value_invert.x;
	float3 change_secondary = change_color_value.y * secondary_change.rgb + change_color_value_invert.y;

    float3 change_aggregate = change_primary * change_secondary;

	float4 base_detail_aggregate = detail_map_sample * base_map_sample;
	float4 albedo = float4(base_detail_aggregate.xyz * change_aggregate, base_detail_aggregate.w); 
	albedo.rgb = apply_debug_tint(albedo.rgb);
	return albedo;
}

float4 calc_albedo_four_change_color_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
    float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
    float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);
    float2 change_color_map_texcoord = apply_xform2d(texcoord, change_color_map_xform);

	float4 base_map_sample = tex2D(base_map, base_map_texcoord);
    float4 detail_map_sample = tex2D(detail_map, detail_map_texcoord);
    float4 change_color_map_sample = tex2D(change_color_map, change_color_map_texcoord);

    float4 change_color_value = change_color_map_sample;
    float4 change_color_value_invert = 1.0 - change_color_value;

	float3 change_primary = change_color_value.x * primary_change_color + change_color_value_invert.x;
	float3 change_secondary = change_color_value.y * secondary_change_color + change_color_value_invert.y;
	float3 change_tertiary = change_color_value.z * tertiary_change_color + change_color_value_invert.z;
    float3 change_quaternary = change_color_value.w * quaternary_change_color + change_color_value_invert.w;

    float3 change_aggregate = change_primary * change_secondary * change_tertiary  * change_quaternary;

    float4 base_detail_aggregate = base_map_sample * detail_map_sample;

    float4 albedo = float4(base_detail_aggregate.xyz * change_aggregate, base_detail_aggregate.w);
	albedo.rgb = apply_debug_tint(albedo.rgb);
	return albedo;
}

float4 calc_albedo_three_detail_blend_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
    float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
    float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);
    float2 detail_map2_texcoord = apply_xform2d(texcoord, detail_map2_xform);
    float2 detail_map3_texcoord = apply_xform2d(texcoord, detail_map3_xform);

    float4 base_map_sample = tex2D(base_map, base_map_texcoord);
    float4 detail_map_sample = tex2D(detail_map, detail_map_texcoord);
    float4 detail_map2_sample = tex2D(detail_map2, detail_map2_texcoord);
    float4 detail_map3_sample = tex2D(detail_map3, detail_map3_texcoord);

    float alpha2 = saturate(base_map_sample.a * 2.0); // I don't understand why this is so
    float4 blended_detailA = lerp(detail_map_sample, detail_map2_sample, alpha2);

	float alpha2b = saturate(base_map_sample.a * 2.0 - 1.0); // I don't understand why this is so
    float4 blended_detailB = lerp(blended_detailA, detail_map3_sample, alpha2b);

	float4 albedo = float4(base_map_sample.rgb * blended_detailB.rgb, blended_detailB.a);
	albedo.rgb = apply_debug_tint(albedo.rgb);
	return albedo;
}

float4 calc_albedo_two_detail_overlay_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
    float4 base_map_sample = tex2D(base_map, apply_xform2d(texcoord, base_map_xform));
    float4 detail_map_sample = tex2D(detail_map, apply_xform2d(texcoord, detail_map_xform));
    float4 detail_map2_sample = tex2D(detail_map2, apply_xform2d(texcoord, detail_map2_xform));
    float4 detail_map_overlay_sample = tex2D(detail_map_overlay, apply_xform2d(texcoord, detail_map_overlay_xform));

    float4 detail_blend = lerp(detail_map_sample, detail_map2_sample, base_map_sample.w);

	float3 detail_color = base_map_sample.xyz * detail_blend.xyz * detail_map_overlay_sample.xyz * DETAIL_MULTIPLIER;

	float alpha = detail_blend.w * detail_map_overlay_sample.w;

	float4 albedo = float4(detail_color, alpha);
	albedo.rgb = apply_debug_tint(albedo.rgb);
	return albedo;
}

float4 calc_albedo_two_detail_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
    float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
    float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);
    float2 detail_map2_texcoord = apply_xform2d(texcoord, detail_map2_xform);

    float4 base_map_sample = tex2D(base_map, base_map_texcoord);
    float4 detail_map_sample = tex2D(detail_map, detail_map_texcoord);
    float4 detail_map2_sample = tex2D(detail_map2, detail_map2_texcoord);

	float4 albedo = base_map_sample * detail_map_sample * detail_map2_sample;

	albedo.rgb = apply_debug_tint(albedo.rgb * DETAIL_MULTIPLIER);
	return albedo;
}

float4 calc_albedo_color_mask_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
	float4 masked_color;
	float4 color;
    
    float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
    float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);
    float2 color_mask_map_texcoord = apply_xform2d(texcoord, color_mask_map_xform);

    float4 base_map_sample = tex2D(base_map, base_map_texcoord);
    float4 detail_map_sample = tex2D(detail_map, detail_map_texcoord);
    float4 color_mask_map_sample = tex2D(color_mask_map, color_mask_map_texcoord);

    color = base_map_sample * detail_map_sample;

    float3 color_mask_invert = 1.0 - color_mask_map_sample.rgb;
    float4 neutral_invert = float4((1.0 / neutral_gray.rgb), 1.0);

    float4 masked_color0 = color_mask_map_sample.r * albedo_color;
    float4 masked_color1 = color_mask_map_sample.g * albedo_color2;
    float4 masked_color2 = color_mask_map_sample.b * albedo_color3;
    
	masked_color = masked_color0 * neutral_invert + color_mask_invert.rrrr;
	masked_color *= masked_color1 * neutral_invert + color_mask_invert.gggg;
	masked_color *= masked_color2 * neutral_invert + color_mask_invert.bbbb;


	float4 albedo = masked_color * color;
	albedo.rgb = apply_debug_tint(albedo.rgb);
	return albedo;
}

float4 calc_albedo_two_detail_black_point_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
	float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
	float4 base_map_sample = tex2D(base_map, base_map_texcoord);
	float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);
	float4 detail_map_sample = tex2D(detail_map, detail_map_texcoord);
	
	float2 detail_map2_texcoord = apply_xform2d(texcoord, detail_map2_xform);
	float4 detail_map2_sample = tex2D(detail_map2, detail_map2_texcoord);

	float4 albedo;
	albedo.rgb = base_map_sample.rgb * detail_map_sample.rgb * detail_map2_sample.rgb;
	albedo.rgb = apply_debug_tint(albedo.rgb * DETAIL_MULTIPLIER);
	
    // blackpoint code
	float details = detail_map_sample.a * detail_map2_sample.a;
	float base = (1.0 + base_map_sample.a) * 0.5;
	float x = (details - base_map_sample.a) / (base - base_map_sample.a);
	float b = details - base;
	albedo.a = base * saturate(x) + saturate(b);
	
	return albedo;
}

float4 calc_albedo_two_change_color_anim_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
	float3 primary_change;
	float3 secondary_change;

    
	float anim = position.y * secondary_change_color_anim.w - primary_change_color_anim.w;
	anim = saturate(anim * 15.0 + 0.5);
	
	
	primary_change = anim * (primary_change_color_anim.rgb - primary_change_color.rgb) + primary_change_color.rgb;
	secondary_change = anim * (secondary_change_color_anim.rgb - secondary_change_color.rgb) + secondary_change_color.rgb;

	float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
	float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);
	float4 base_map_sample = tex2D(base_map, base_map_texcoord);
	float4 detail_map_sample = tex2D(detail_map, detail_map_texcoord);
	float4 base_detail_aggregate = detail_map_sample * base_map_sample;
	base_detail_aggregate.rgb *= DETAIL_MULTIPLIER;
	
	primary_change = primary_change * base_detail_aggregate.rgb - base_detail_aggregate.rgb;
	
	float2 change_color_map_texcoord = apply_xform2d(texcoord, change_color_map_xform);
	float4 change_color_map_sample = tex2D(change_color_map, change_color_map_texcoord);

	float2 change_color_value = change_color_map_sample.xy;
	
	primary_change = change_color_value.x * primary_change + base_detail_aggregate.rgb;
	
	secondary_change = secondary_change * base_detail_aggregate.rgb - primary_change;
	
	float3 change_aggregate = change_color_value.y * secondary_change + primary_change;

	
	float4 albedo = float4(change_aggregate, base_detail_aggregate.w);

	float3 negative_tinted_color = debug_tint.rgb - albedo.rgb;
	albedo.rgb = albedo.rgb + debug_tint.a * negative_tinted_color;
	
	return albedo;
}

uniform float3 chameleon_color0;
uniform float3 chameleon_color1;
uniform float3 chameleon_color2;
uniform float3 chameleon_color3;
uniform float chameleon_color_offset1;
uniform float chameleon_color_offset2;
uniform float chameleon_fresnel_power;
uniform sampler2D chameleon_mask_map;
uniform xform2d chameleon_mask_map_xform;

float3 get_chameleon_color(float3 surface_normal, float3 n_view_dir)
{
    float n_dot_v = max(dot(surface_normal, n_view_dir), 0.0f);
    float fresnel = pow(n_dot_v, chameleon_fresnel_power);
	
    if (fresnel > chameleon_color_offset2)
    {
        float interpolant = (fresnel - chameleon_color_offset2) * rcp(1.0f - chameleon_color_offset2);
        return lerp(chameleon_color2, chameleon_color3, interpolant);
    }
    else if (fresnel > chameleon_color_offset1)
    {
        float interpolant = (fresnel - chameleon_color_offset1) * rcp(chameleon_color_offset2 - chameleon_color_offset1);
        return lerp(chameleon_color1, chameleon_color2, interpolant);
    }
	else
    {
        float interpolant = fresnel * rcp(chameleon_color_offset1);
        return lerp(chameleon_color0, chameleon_color1, interpolant);
    }
}

float4 calc_albedo_chameleon_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
	float3 chameleon = get_chameleon_color(surface_normal, normalize(camera_dir));
	
    float4 base_map_sample = tex2D(base_map, apply_xform2d(texcoord, base_map_xform));
    float4 detail_map_sample = tex2D(detail_map, apply_xform2d(texcoord, detail_map_xform));
    detail_map_sample.rgb *= DETAIL_MULTIPLIER;
	
	float4 albedo = base_map_sample * detail_map_sample;
	albedo.rgb = chameleon * albedo.rgb;
	albedo.rgb = apply_debug_tint(albedo.rgb);
	
	return albedo;
}

float4 calc_albedo_two_change_color_chameleon_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
	return random_debug_color(12);
}

float4 calc_albedo_chameleon_masked_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
	float3 chameleon = get_chameleon_color(surface_normal, normalize(camera_dir));
	
    float4 chameleon_mask_sample = tex2D(chameleon_mask_map, apply_xform2d(texcoord, chameleon_mask_map_xform));
	chameleon = lerp(1, chameleon, chameleon_mask_sample.x);
	
    float4 base_map_sample = tex2D(base_map, apply_xform2d(texcoord, base_map_xform));
    float4 detail_map_sample = tex2D(detail_map, apply_xform2d(texcoord, detail_map_xform));
    detail_map_sample.rgb *= DETAIL_MULTIPLIER;
	
	float4 albedo = base_map_sample * detail_map_sample;
	albedo.rgb = chameleon * albedo.rgb;
	albedo.rgb = apply_debug_tint(albedo.rgb);
	
	return albedo;
}

float4 calc_albedo_color_mask_hard_light_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
	float4 albedo;
	
	float2 color_mask_map_texcoord = apply_xform2d(texcoord, color_mask_map_xform);
	float4 color_mask_map_sample = tex2D(color_mask_map, color_mask_map_texcoord);
	
	float color_mask = color_mask_map_sample.x * 2 - 1;
	float3 masked_albedo = color_mask * albedo_color.rgb;
	float3 masked_albedo_2 = masked_albedo * 0.5 + 0.5;
	albedo.rgb = 1.0 - masked_albedo_2;
	
	float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
	float4 base_map_sample = tex2D(base_map, base_map_texcoord);
	
	albedo.rgb *= base_map_sample.rgb;
	masked_albedo += 2 * albedo.rgb;
	
	float3 temp = 0.5 - masked_albedo_2;
	masked_albedo_2 *= base_map_sample.rgb;
	masked_albedo_2 *= 2;
	albedo.rgb = temp < 0 ? masked_albedo : masked_albedo_2;
	float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);
	float4 detail_map_sample = tex2D(detail_map, detail_map_texcoord);
	
	albedo.rgb *= detail_map_sample.rgb;
	albedo.a = base_map_sample.a * detail_map_sample.a * albedo_color.a;
	albedo.rgb = apply_debug_tint(albedo.rgb);
	return albedo;
}

// RMCS

uniform sampler2D waterfall_base_mask;
uniform float4 waterfall_base_mask_xform;
uniform sampler2D waterfall_layer0;
uniform float4 waterfall_layer0_xform;
uniform sampler2D waterfall_layer1;
uniform float4 waterfall_layer1_xform;
uniform sampler2D waterfall_layer2;
uniform float4 waterfall_layer2_xform;

uniform float transparency_base_weight;
uniform float transparency_bias;
uniform float transparency_frothy_weight;

float4 calc_albedo_waterfall_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
    float4 albedo;
	
    float4 base_mask = tex2D(waterfall_base_mask, apply_xform2d(texcoord, waterfall_base_mask_xform));
    float4 layer0 = tex2D(waterfall_layer0, apply_xform2d(texcoord, waterfall_layer0_xform));
    float4 layer1 = tex2D(waterfall_layer1, apply_xform2d(texcoord, waterfall_layer1_xform));
    float4 layer2 = tex2D(waterfall_layer2, apply_xform2d(texcoord, waterfall_layer2_xform));
	
    albedo.rgb = layer0.rgb * layer1.rgb * layer2.rgb;
	
    albedo.a = (layer0.a + layer1.a + layer2.a) * transparency_frothy_weight + (base_mask.a * transparency_base_weight + transparency_bias);
	
    return albedo;
}

float4 calc_albedo_four_change_color_applying_to_specular_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
    return calc_albedo_four_change_color_ps(texcoord, position, surface_normal, camera_dir);
}

float4 calc_albedo_simple_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
    return tex2D(base_map, apply_xform2d(texcoord, base_map_xform)) * albedo_color;
}

// no reference
float4 calc_albedo_two_change_color_tex_overlay_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
    return 0;
}

uniform sampler2D base_masked_map;
uniform float4 base_masked_map_xform;
uniform float4 albedo_masked_color;

float4 calc_albedo_chameleon_albedo_masked_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
    float3 chameleon = get_chameleon_color(surface_normal, normalize(camera_dir));
	
    float4 base_map_sample = tex2D(base_map, apply_xform2d(texcoord, base_map_xform));
    base_map_sample *= albedo_color;
    float4 base_masked_map_sample = tex2D(base_masked_map, apply_xform2d(texcoord, base_masked_map_xform));
    base_masked_map_sample *= albedo_masked_color;
    base_masked_map_sample.rgb *= chameleon;
	
    float4 chameleon_mask_sample = tex2D(chameleon_mask_map, apply_xform2d(texcoord, chameleon_mask_map_xform));
	
    float4 albedo = lerp(base_map_sample, base_masked_map_sample, chameleon_mask_sample.x);
    albedo.rgb = apply_debug_tint(albedo.rgb);
    return albedo;
}

uniform samplerCUBE custom_cube;

float4 calc_albedo_custom_cube_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
    float4 albedo = tex2D(base_map, apply_xform2d(texcoord, base_map_xform));
    float4 cube = texCUBE(custom_cube, surface_normal);
	
    albedo.rgb *= cube.rgb;
    albedo.rgb = apply_debug_tint(albedo.rgb);
    albedo.a *= albedo_color.a;
	
    return albedo;
}

uniform samplerCUBE blend_map;
uniform float4 albedo_second_color;

float4 calc_albedo_two_color_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
    float4 albedo = tex2D(base_map, apply_xform2d(texcoord, base_map_xform));

    float4 blend = texCUBE(blend_map, surface_normal);
    blend *= 2.0f;

    albedo *= (blend.y * albedo_color + blend.z * albedo_second_color);
    albedo.rgb = apply_debug_tint(albedo.rgb);
	
    return albedo;
}

// these options from MCC require use of the new misc attr stuff
// scrolling_texture_uv might be easily doable but the others require a new VS

//uniform samplerCUBE color_blend_mask_cubemap;
//float4 calc_albedo_scrolling_cube_mask_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
//{
//    float4 albedo = tex2D(base_map, apply_xform2d(texcoord, base_map_xform));
//	
//    float3 blend = texCUBE(color_blend_mask_cubemap, misc.xyz);
//    blend *= 2.0f;
//	
//    albedo *= (blend.y * albedo_color + blend.z * albedo_second_color);
//    albedo.rgb = apply_debug_tint(albedo.rgb);
//	
//    return albedo;
//}
//
//uniform samplerCUBE color_cubemap;
//float4 calc_albedo_scrolling_cube_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
//{
//    float4 albedo = tex2D(base_map, apply_xform2d(texcoord, base_map_xform));
//	
//    albedo *= texCUBE(color_cubemap, misc.xyz);
//	
//    albedo.rgb = apply_debug_tint(albedo.rgb);
//	
//    return albedo;
//}
//
//uniform sampler2D color_texture;
//uniform float u_speed;
//uniform float v_speed;
//float4 calc_albedo_scrolling_texture_uv_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
//{
//    float2 base_map_tex = apply_xform2d(texcoord, base_map_xform);
//    float4 albedo = tex2D(base_map, base_map_tex);
//	
//    float2 color_tex = base_map_tex + ps_total_time * float2(u_speed, v_speed);
//    float4 color = tex2D(color_texture, color_tex);
//
//    albedo *= color;
//    albedo.rgb = apply_debug_tint(albedo.rgb);
//	
//    return albedo;
//}
//
//float4 calc_albedo_texture_from_misc_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
//{
//    float4 albedo = tex2D(base_map, apply_xform2d(texcoord, base_map_xform));
//
//    albedo *= tex2D(color_texture, misc.xy);
//    albedo.rgb = apply_debug_tint(albedo.rgb);
//	
//    return albedo;
//}

float4 calc_albedo_default_vs(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir)
{
	return 0;
}

#if shadertype == k_shadertype_cortana

uniform float4 detail_color;
uniform sampler2D scanline_map;
uniform float4 scanline_map_xform;
uniform float scanline_amount_transparent;
uniform float scanline_amount_opaque;
uniform float4 ss_constants; // _render_method_extern_screen_constants

uniform float layer_depth;
uniform float layer_contrast;
uniform int layer_count;
uniform float texcoord_aspect_ratio;
uniform float depth_darken;

float4 calc_albedo_cortana_ps(float2 texcoord, float2 position, float3 surface_normal, float3 camera_dir) // camera_dir = view_dir in this func
{
    float4 albedo = tex2D(base_map, apply_xform2d(texcoord, base_map_xform));
    albedo *= albedo_color;
	
    float2 res_scale = float2(1280.0f, 720.0f) * ss_constants.xy;
    float4 scanline_sample = tex2D(scanline_map, apply_xform2d(position * res_scale, scanline_map_xform));
    float scanline_blend = scanline_amount_transparent + albedo.a * (scanline_amount_opaque - scanline_amount_transparent);
    float4 scanline_final = 1.0f + scanline_blend * (scanline_sample - 1.0f);
    albedo.rgb *= scanline_final.rgb;
	
	// TODO: fix rmt2 routing
    float fp_layer_count = 0.0f;
    for (int i = 0; i < layer_count; i++)
        fp_layer_count += 1.0f; // hack
	
    float2 detail_texcoord = apply_xform2d(texcoord, detail_map_xform);
    float2 offset = camera_dir.xy * detail_map_xform.xy * float2(texcoord_aspect_ratio, 1.0f) * layer_depth / fp_layer_count;
	
    float4 accum = 0.0f;
    float darkness = 1.0f;
    for (i = 0; i < layer_count; i++)
    {
        accum += darkness * tex2D(detail_map, detail_texcoord);
        detail_texcoord -= offset;
        darkness *= depth_darken;
    }
    accum /= fp_layer_count;
	
    float4 final_detail;
    final_detail.rgb = pow(abs(accum.rgb), layer_contrast) * detail_color.rgb;
    final_detail.a = accum.a * detail_color.a;

    albedo.rgb = albedo.rgb + (1.0f - albedo.a) * final_detail.rgb;
    albedo.a = albedo.a * scanline_final.a + (1.0f - albedo.a) * final_detail.a;
	
    return albedo;
}

#endif

//fixups
#define calc_albedo_two_change_color_anim_overlay_ps calc_albedo_two_change_color_anim_ps

#ifndef calc_albedo_ps
#define calc_albedo_ps calc_albedo_default_ps
#endif
#ifndef calc_albedo_vs
#define calc_albedo_vs calc_albedo_default_vs
#endif

#endif
