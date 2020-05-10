#ifndef _ALBEDO_HLSLI
#define _ALBEDO_HLSLI

#include "../helpers/types.hlsli"
#include "../helpers/math.hlsli"
#include "../registers/shader.hlsli"
#include "../helpers/color_processing.hlsli"

float4 calc_albedo_default_ps(float2 texcoord, float2 position)
{
    float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
    float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);

    float4 base_map_sample = tex2D(base_map, base_map_texcoord);
    float4 detail_map_sample = tex2D(detail_map, detail_map_texcoord);
	float4 albedo = base_map_sample * detail_map_sample * albedo_color;
	albedo.rgb = apply_debug_tint(albedo.rgb);
	return albedo;
}

float4 calc_albedo_detail_blend_ps(float2 texcoord, float2 position)
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

float4 calc_albedo_constant_color_ps(float2 texcoord, float2 position)
{
	float3 albedo = lerp(albedo_color.rgb, debug_tint.rgb, debug_tint.w);
	return float4(albedo, albedo_color.a);
}

float4 calc_albedo_two_change_color_ps(float2 texcoord, float2 position)
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

    float4 base_detail_aggregate = base_map_sample * detail_map_sample;
	float4 albedo = float4(base_detail_aggregate.xyz * change_aggregate, base_detail_aggregate.w); 
	albedo.rgb = apply_debug_tint(albedo.rgb);
	return albedo;
}

float4 calc_albedo_four_change_color_ps(float2 texcoord, float2 position)
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

float4 calc_albedo_three_detail_blend_ps(float2 texcoord, float2 position)
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

float4 calc_albedo_two_detail_overlay_ps(float2 texcoord, float2 position)
{
    float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
    float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);
    float2 detail_map2_texcoord = apply_xform2d(texcoord, detail_map2_xform);
    float2 detail_map_overlay_texcoord = apply_xform2d(texcoord, detail_map_overlay_xform);

    float4 base_map_sample = tex2D(base_map, base_map_texcoord);
    float4 detail_map_sample = tex2D(detail_map, detail_map_texcoord);
    float4 detail_map2_sample = tex2D(detail_map2, detail_map2_texcoord);
    float4 detail_map_overlay_sample = tex2D(detail_map_overlay, detail_map_overlay_texcoord);

    float4 detail_blend = lerp(detail_map_sample, detail_map2_sample, base_map_sample.w);

	float3 detail_color = base_map_sample.xyz * detail_blend.xyz * detail_map_overlay_sample.xyz * DEBUG_TINT_FACTOR;

	float alpha = detail_blend.w * detail_map_overlay_sample.w;

	float4 albedo = float4(detail_color, alpha);
	albedo.rgb = apply_debug_tint(albedo.rgb);
	return albedo;
}

float4 calc_albedo_two_detail_ps(float2 texcoord, float2 position)
{
    float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
    float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);
    float2 detail_map2_texcoord = apply_xform2d(texcoord, detail_map2_xform);

    float4 base_map_sample = tex2D(base_map, base_map_texcoord);
    float4 detail_map_sample = tex2D(detail_map, detail_map_texcoord);
    float4 detail_map2_sample = tex2D(detail_map2, detail_map2_texcoord);

	float4 albedo = base_map_sample * detail_map_sample * detail_map2_sample;

	albedo.rgb = apply_debug_tint(albedo.rgb * DEBUG_TINT_FACTOR);
	return albedo;
}

float4 calc_albedo_color_mask_ps(float2 texcoord, float2 position)
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

float4 calc_albedo_two_detail_black_point_ps(float2 texcoord, float2 position)
{
	float2 base_map_texcoord = apply_xform2d(texcoord, base_map_xform);
	float2 detail_map_texcoord = apply_xform2d(texcoord, detail_map_xform);
	float2 detail_map2_texcoord = apply_xform2d(texcoord, detail_map2_xform);

	float4 base_map_sample = tex2D(base_map, base_map_texcoord);
	float4 detail_map_sample = tex2D(detail_map, detail_map_texcoord);
	float4 detail_map2_sample = tex2D(detail_map2, detail_map2_texcoord);

	float4 albedo = base_map_sample * detail_map_sample * detail_map2_sample;

	albedo.rgb = apply_debug_tint(albedo.rgb * DEBUG_TINT_FACTOR);
    
    // blackpoint code
    
	float base = (1.0 + base_map_sample.a) * 0.5;
	float white_base = saturate(detail_map_sample.a * detail_map2_sample.a - base);
	float scale = saturate((detail_map_sample.a * detail_map2_sample.a - base_map_sample.a) / lerp(-base_map_sample.a, 1.0, 0.5));
    
	albedo.a = base * scale + white_base;
    
	return albedo;
}

float4 calc_albedo_two_change_color_anim_ps(float2 texcoord, float2 position)
{
	return random_debug_color(11);
}

float4 calc_albedo_chameleon_ps(float2 texcoord, float2 position)
{
	return random_debug_color(12);
}

float4 calc_albedo_two_change_color_chameleon_ps(float2 texcoord, float2 position)
{
	return random_debug_color(13);
}

float4 calc_albedo_chameleon_masked_ps(float2 texcoord, float2 position)
{
	return random_debug_color(14);
}

float4 calc_albedo_color_mask_hard_light_ps(float2 texcoord, float2 position)
{
	return random_debug_color(15);
}



float4 calc_albedo_default_vs(float2 texcoord, float2 position)
{
	return random_debug_color(0);
}


//fixups
#define calc_albedo_two_change_color_anim_overlay_ps calc_albedo_two_change_color_anim_ps

#ifndef calc_albedo_ps
#define calc_albedo_ps calc_albedo_default_ps
#endif
#ifndef calc_albedo_vs
#define calc_albedo_vs calc_albedo_default_vs
#endif

#endif
