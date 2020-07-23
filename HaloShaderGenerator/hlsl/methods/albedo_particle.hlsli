#ifndef _ALBEDO_PARTICLE_HLSLI
#define _ALBEDO_PARTICLE_HLSLI

#include "../helpers/definition_helper.hlsli"
#include "../helpers/types.hlsli"

uniform sampler base_map;
uniform xform2d base_map_xform;
uniform sampler base_map2;
uniform xform2d base_map2_xform;
uniform sampler palette;
uniform sampler alpha_map;
uniform xform2d alpha_map_xform;
uniform float alpha_modulation_factor;

float4 albedo_diffuse_only(in float4 texcoord, in float3 alpha_tex, in float frame_blend, in float palettized_w, in float color_alpha)
{
    float4 base_map_sample;
	
    if (frame_blend_arg == k_frame_blend_on)
    {
        float2 base_map_texcoord_1 = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample_1 = tex2D(base_map, base_map_texcoord_1);
        float2 base_map_texcoord_2 = apply_xform2d(texcoord.zw, base_map_xform);
        float4 base_map_sample_2 = tex2D(base_map, base_map_texcoord_2);
		
        base_map_sample = lerp(base_map_sample_1, base_map_sample_2, frame_blend);
    }
    else
    {
        float2 base_map_texcoord = apply_xform2d(texcoord.xy, base_map_xform);
        base_map_sample = tex2D(base_map, base_map_texcoord);
    }
	
    return base_map_sample;
}

float4 albedo_diffuse_plus_billboard_alpha(in float4 texcoord, in float3 alpha_tex, in float frame_blend, in float palettized_w, in float color_alpha)
{
    float4 albedo = 0;
    
    if (frame_blend_arg == k_frame_blend_on)
    {
        float2 base_map_texcoord_1 = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample_1 = tex2D(base_map, base_map_texcoord_1);
        float2 base_map_texcoord_2 = apply_xform2d(texcoord.zw, base_map_xform);
        float4 base_map_sample_2 = tex2D(base_map, base_map_texcoord_2);
    
        float2 alpha_map_texcoord = apply_xform2d(alpha_tex.yz, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_texcoord);
		
        // todo: fix, compiles with mad separated into add and mul
        albedo.rgb = (base_map_sample_2.rgb - base_map_sample_1.rgb) * frame_blend.x;
        albedo += float4(base_map_sample_1.rgb, alpha_map_sample.a);
    }
    else
    {
        float2 base_map_texcoord = apply_xform2d(texcoord.xy, base_map_xform);
        albedo = tex2D(base_map, base_map_texcoord);
    
        float2 alpha_map_texcoord = apply_xform2d(alpha_tex.yz, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_texcoord);
        albedo.a = alpha_map_sample.a + sqrt(alpha_map_sample.a);
    }
    
    return albedo;
}

float4 albedo_palettized(in float4 texcoord, in float3 alpha_tex, in float frame_blend, in float palettized_w, in float color_alpha)
{
    float4 albedo;
    
    if (frame_blend_arg == k_frame_blend_on)
    {
        float2 base_map_texcoord_1 = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample_1 = tex2D(base_map, base_map_texcoord_1);
        float2 base_map_texcoord_2 = apply_xform2d(texcoord.zw, base_map_xform);
        float4 base_map_sample_2 = tex2D(base_map, base_map_texcoord_2);
        
        float4 palette_sample_1 = tex2D(palette, float2(base_map_sample_1.x, palettized_w));
        float4 palette_sample_2 = tex2D(palette, float2(base_map_sample_2.x, palettized_w));
		
        albedo = lerp(palette_sample_1, palette_sample_2, frame_blend);
    }
    else
    {
        albedo = albedo_diffuse_only(texcoord, alpha_tex, frame_blend, palettized_w, color_alpha);
        albedo = tex2D(palette, float2(albedo.x, palettized_w));
    }
    
    return albedo;
}

float4 albedo_palettized_plus_billboard_alpha(in float4 texcoord, in float3 alpha_tex, in float frame_blend, in float palettized_w, in float color_alpha)
{
    float4 albedo = 0;
    
    if (frame_blend_arg == k_frame_blend_on)
    {
        float2 base_map_texcoord_1 = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample_1 = tex2D(base_map, base_map_texcoord_1);
        float2 base_map_texcoord_2 = apply_xform2d(texcoord.zw, base_map_xform);
        float4 base_map_sample_2 = tex2D(base_map, base_map_texcoord_2);
        
        float4 palette_sample_1 = tex2D(palette, float2(base_map_sample_1.x, palettized_w));
        float4 palette_sample_2 = tex2D(palette, float2(base_map_sample_2.x, palettized_w));
    
        float2 alpha_map_texcoord = apply_xform2d(alpha_tex.yz, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_texcoord);
		
        // todo: fix, compiles with mad separated into add and mul
        albedo.rgb = (palette_sample_2.rgb - palette_sample_1.rgb) * frame_blend.x;
        albedo += float4(palette_sample_1.rgb, alpha_map_sample.a);
    }
    else
    {
        albedo = albedo_diffuse_only(texcoord, alpha_tex, frame_blend, palettized_w, color_alpha);
        albedo = tex2D(palette, float2(albedo.x, palettized_w));
    
        float2 alpha_map_texcoord = apply_xform2d(alpha_tex.yz, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_texcoord);
        albedo.a = alpha_map_sample.w;
    }
    
    return albedo;
}

float4 albedo_diffuse_plus_sprite_alpha(in float4 texcoord, in float3 alpha_tex, in float frame_blend, in float palettized_w, in float color_alpha)
{
    float4 albedo = 0;
    
    if (frame_blend_arg == k_frame_blend_on)
    {
        float2 base_map_texcoord_1 = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample_1 = tex2D(base_map, base_map_texcoord_1);
        float2 base_map_texcoord_2 = apply_xform2d(texcoord.zw, base_map_xform);
        float4 base_map_sample_2 = tex2D(base_map, base_map_texcoord_2);
    
        float2 alpha_map_texcoord = apply_xform2d(texcoord.xy, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_texcoord);
		
        // todo: fix, compiles with mad separated into add and mul
        albedo.rgb = (base_map_sample_2.rgb - base_map_sample_1.rgb) * frame_blend.x;
        albedo += float4(base_map_sample_1.rgb, alpha_map_sample.a);
    }
    else
    {
        float2 base_map_texcoord = apply_xform2d(texcoord.xy, base_map_xform);
        albedo = tex2D(base_map, base_map_texcoord);
    
        float2 alpha_map_texcoord = apply_xform2d(texcoord.xy, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_texcoord);
        albedo.a = alpha_map_sample.w;
    }
    
    return albedo;
}

float4 albedo_palettized_plus_sprite_alpha(in float4 texcoord, in float3 alpha_tex, in float frame_blend, in float palettized_w, in float color_alpha)
{
    float4 albedo = 0;
    
    if (frame_blend_arg == k_frame_blend_on)
    {
        float2 base_map_texcoord_1 = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample_1 = tex2D(base_map, base_map_texcoord_1);
        float2 base_map_texcoord_2 = apply_xform2d(texcoord.zw, base_map_xform);
        float4 base_map_sample_2 = tex2D(base_map, base_map_texcoord_2);
        
        float4 palette_sample_1 = tex2D(palette, float2(base_map_sample_1.x, palettized_w));
        float4 palette_sample_2 = tex2D(palette, float2(base_map_sample_2.x, palettized_w));
    
        float2 alpha_map_texcoord = apply_xform2d(texcoord.xy, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_texcoord);
		
        // todo: fix, compiles with mad separated into add and mul
        albedo.rgb = (palette_sample_2.rgb - palette_sample_1.rgb) * frame_blend.x;
        albedo += float4(palette_sample_1.rgb, alpha_map_sample.a);
    }
    else
    {
        albedo = albedo_diffuse_only(texcoord, alpha_tex, frame_blend, palettized_w, color_alpha);
        albedo = tex2D(palette, float2(albedo.x, palettized_w));
    
        float2 alpha_map_texcoord = apply_xform2d(texcoord.xy, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_texcoord);
        albedo.a = alpha_map_sample.w;
    }
    
    return albedo;
}

// ODST

// 8 
float4 albedo_palettized_plasma(in float4 texcoord, in float3 alpha_tex, in float frame_blend, in float palettized_w, in float color_alpha)
{
    float4 albedo = 0;
    
    if (frame_blend_arg == k_frame_blend_on)
    {
        float2 base_map_texcoord = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample = tex2D(base_map, base_map_texcoord);
        float2 base_map_texcoord2 = apply_xform2d(texcoord.zw, base_map_xform);
        float4 base_map_sample2 = tex2D(base_map, base_map_texcoord2);
        
        float2 base_map2_texcoord = apply_xform2d(texcoord.xy, base_map2_xform);
        float4 base_map2_sample = tex2D(base_map2, base_map2_texcoord);
        float2 base_map2_texcoord2 = apply_xform2d(texcoord.zw, base_map2_xform);
        float4 base_map2_sample2 = tex2D(base_map2, base_map2_texcoord2);
    
        float2 alpha_map_texcoord = apply_xform2d(alpha_tex.yz, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_texcoord);
        albedo.a = alpha_map_sample.w;
        
        float2 palette_tex = float(-albedo.a * color_alpha + 1.0f).xx;
        palette_tex = saturate(palette_tex * alpha_modulation_factor.x + abs(float2(base_map_sample.x - base_map2_sample.x, base_map_sample2.x - base_map2_sample2.x)));
        
        float4 palette_sample_1 = tex2D(palette, float2(palette_tex.x, palettized_w));
        float4 palette_sample_2 = tex2D(palette, float2(palette_tex.x, palettized_w));
		
        albedo.rgb = (palette_sample_2.rgb - palette_sample_1.rgb) * frame_blend.x;
        albedo += float4(palette_sample_1.rgb, alpha_map_sample.a);
    }
    else
    {
        float2 base_map_texcoord = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample = tex2D(base_map, base_map_texcoord);
        float2 base_map2_texcoord = apply_xform2d(texcoord.xy, base_map2_xform);
        float4 base_map2_sample = tex2D(base_map2, base_map2_texcoord);
        
        float2 alpha_map_texcoord = apply_xform2d(alpha_tex.yz, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_texcoord);
        albedo.a = alpha_map_sample.w;
        
        float palette_tex = -albedo.a * color_alpha + 1.0f;
        palette_tex = saturate(palette_tex * alpha_modulation_factor.x + abs(base_map_sample.x - base_map2_sample.x));
        
        albedo.rgb = tex2D(palette, float2(palette_tex, palettized_w)).rgb;
    }
    
    return albedo;
    
}

#ifndef particle_albedo
#define particle_albedo albedo_diffuse_only
#endif

#endif