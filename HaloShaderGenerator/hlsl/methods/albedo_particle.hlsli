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
uniform float4 tint_color;
uniform float modulation_factor;

float4 albedo_diffuse_only(in float4 texcoord, in float2 alpha_map_texcoord, in float frame_blend, in float palette_v, in float color_alpha)
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

float4 albedo_diffuse_plus_billboard_alpha(in float4 texcoord, in float2 alpha_map_texcoord, in float frame_blend, in float palette_v, in float color_alpha)
{
    float4 albedo = 0;
    
    if (frame_blend_arg == k_frame_blend_on)
    {
        float2 alpha_map_tex = apply_xform2d(alpha_map_texcoord, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_tex);
        
        float2 base_map_texcoord_1 = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample_1 = tex2D(base_map, base_map_texcoord_1);
        base_map_sample_1.a = alpha_map_sample.a;
        
        float2 base_map_texcoord_2 = apply_xform2d(texcoord.zw, base_map_xform);
        float4 base_map_sample_2 = tex2D(base_map, base_map_texcoord_2);
        base_map_sample_2.a = alpha_map_sample.a;
        
        float4 base_map_sample = lerp(base_map_sample_1, base_map_sample_2, frame_blend);
        
        albedo += base_map_sample;
    }
    else
    {
        float2 base_map_texcoord = apply_xform2d(texcoord.xy, base_map_xform);
        albedo = tex2D(base_map, base_map_texcoord);
    
        float2 alpha_map_tex = apply_xform2d(alpha_map_texcoord, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_tex);
        albedo.a = alpha_map_sample.a;
    }
    
    return albedo;
}

float4 albedo_palettized(in float4 texcoord, in float2 alpha_map_texcoord, in float frame_blend, in float palette_v, in float color_alpha)
{
    float4 albedo;
    
    if (frame_blend_arg == k_frame_blend_on)
    {
        float2 base_map_texcoord_1 = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample_1 = tex2D(base_map, base_map_texcoord_1);
        float2 base_map_texcoord_2 = apply_xform2d(texcoord.zw, base_map_xform);
        float4 base_map_sample_2 = tex2D(base_map, base_map_texcoord_2);
        
        float4 palette_sample_1 = tex2D(palette, float2(base_map_sample_1.x, palette_v));
        float4 palette_sample_2 = tex2D(palette, float2(base_map_sample_2.x, palette_v));
		
        albedo = lerp(palette_sample_1, palette_sample_2, frame_blend);
    }
    else
    {
        albedo = albedo_diffuse_only(texcoord, alpha_map_texcoord, frame_blend, palette_v, color_alpha);
        albedo = tex2D(palette, float2(albedo.x, palette_v));
    }
    
    return albedo;
}

float4 albedo_palettized_plus_billboard_alpha(in float4 texcoord, in float2 alpha_map_texcoord, in float frame_blend, in float palette_v, in float color_alpha)
{
    float4 albedo = 0;
    
    if (frame_blend_arg == k_frame_blend_on)
    {
        float2 base_map_texcoord_1 = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample_1 = tex2D(base_map, base_map_texcoord_1);
        float2 base_map_texcoord_2 = apply_xform2d(texcoord.zw, base_map_xform);
        float4 base_map_sample_2 = tex2D(base_map, base_map_texcoord_2);
    
        float2 alpha_map_tex = apply_xform2d(alpha_map_texcoord, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_tex);
        
        float4 palette_sample_1 = tex2D(palette, float2(base_map_sample_1.x, palette_v));
        palette_sample_1.a = alpha_map_sample.a;
        float4 palette_sample_2 = tex2D(palette, float2(base_map_sample_2.x, palette_v));
        palette_sample_2.a = alpha_map_sample.a;
        
        float4 base_map_sample = lerp(palette_sample_1, palette_sample_2, frame_blend);
        
        albedo += base_map_sample;
    }
    else
    {
        albedo = albedo_diffuse_only(texcoord, alpha_map_texcoord, frame_blend, palette_v, color_alpha);
        albedo = tex2D(palette, float2(albedo.x, palette_v));
    
        float2 alpha_map_tex = apply_xform2d(alpha_map_texcoord, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_tex);
        albedo.a = alpha_map_sample.w;
    }
    
    return albedo;
}

float4 albedo_diffuse_plus_sprite_alpha(in float4 texcoord, in float2 alpha_map_texcoord, in float frame_blend, in float palette_v, in float color_alpha)
{
    float4 albedo = 0;
    
    if (frame_blend_arg == k_frame_blend_on)
    {
        float2 alpha_map_tex = apply_xform2d(texcoord.xy, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_tex);
        
        float2 base_map_texcoord_1 = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample_1 = tex2D(base_map, base_map_texcoord_1);
        base_map_sample_1.a = alpha_map_sample.a;
        
        float2 base_map_texcoord_2 = apply_xform2d(texcoord.zw, base_map_xform);
        float4 base_map_sample_2 = tex2D(base_map, base_map_texcoord_2);
        base_map_sample_2.a = alpha_map_sample.a;
    
        float4 base_map_sample = lerp(base_map_sample_1, base_map_sample_2, frame_blend);
        
        albedo += base_map_sample;
    }
    else
    {
        float2 base_map_texcoord = apply_xform2d(texcoord.xy, base_map_xform);
        albedo = tex2D(base_map, base_map_texcoord);
    
        float2 alpha_map_tex = apply_xform2d(texcoord.xy, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_tex);
        albedo.a = alpha_map_sample.w;
    }
    
    return albedo;
}

float4 albedo_palettized_plus_sprite_alpha(in float4 texcoord, in float2 alpha_map_texcoord, in float frame_blend, in float palette_v, in float color_alpha)
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
        float2 alpha_map_texcoord_2 = apply_xform2d(texcoord.zw, alpha_map_xform);
        float4 alpha_map_sample_2 = tex2D(alpha_map, alpha_map_texcoord_2);
        
        float4 palette_sample_1 = tex2D(palette, float2(base_map_sample_1.x, palette_v));
        palette_sample_1.a = alpha_map_sample.a;
        float4 palette_sample_2 = tex2D(palette, float2(base_map_sample_2.x, palette_v));
        palette_sample_2.a = alpha_map_sample_2.a;
        
        float4 base_map_sample = lerp(palette_sample_1, palette_sample_2, frame_blend);
        
        albedo += base_map_sample;
    }
    else
    {
        albedo = albedo_diffuse_only(texcoord, alpha_map_texcoord, frame_blend, palette_v, color_alpha);
        albedo = tex2D(palette, float2(albedo.x, palette_v));
    
        float2 alpha_map_tex = apply_xform2d(texcoord.xy, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_tex);
        albedo.a = alpha_map_sample.w;
    }
    
    return albedo;
}

// ODST

// 6
float4 albedo_diffuse_modulated(in float4 texcoord, in float2 alpha_map_texcoord, in float frame_blend, in float palette_v, in float color_alpha)
{
    float4 albedo = 0;
    
    if (frame_blend_arg == k_frame_blend_on)
    {
        float2 base_map_texcoord_1 = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample_1 = tex2D(base_map, base_map_texcoord_1);
        float2 base_map_texcoord_2 = apply_xform2d(texcoord.zw, base_map_xform);
        float4 base_map_sample_2 = tex2D(base_map, base_map_texcoord_2);
        
        float3 tint = 1.0f - tint_color.rgb;
        
        float dot_sample1x = dot(base_map_sample_1.brg, base_map_sample_1.brg);
        float dot_sample2y = dot(base_map_sample_2.brg, base_map_sample_2.brg);
        
        float2 u1 = (dot_sample2y * 0.57735f) * float2(dot_sample2y, dot_sample1x);
        
        float3 color2 = u1.x * tint.bgr + tint_color.bgr;
        float3 color1 = u1.y * tint.rgb + tint_color.rgb;
        
        float3 color1thing = color1.bgr * base_map_sample_1.bgr;
        
        float modulated_alpha = color1thing.z - base_map_sample_1.a;
        
        // lerp
        float3 modulated_color2 = color2.bgr * base_map_sample_2.rgb - base_map_sample_1.bgr;
        float alpha = modulated_alpha * frame_blend.x + base_map_sample_1.a;
        float3 modulated_color1 = modulated_color2.rgb * frame_blend.x - base_map_sample_1.bgr;
        
        albedo = float4(modulated_color1, alpha);
    }
    else
    {
        // TODO: cleanup above first then write a non-frameblend version
    }
    
    return albedo;
}

// 8 
float4 albedo_palettized_plasma(in float4 texcoord, in float2 alpha_map_texcoord, in float frame_blend, in float palette_v, in float color_alpha)
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
    
        float2 alpha_map_tex = apply_xform2d(alpha_map_texcoord, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_tex);
        
        float2 palette_tex = float(-alpha_map_sample.a * color_alpha + 1.0f).xx;
        palette_tex = saturate(palette_tex * alpha_modulation_factor.x + abs(float2(base_map_sample.x - base_map2_sample.x, base_map_sample2.x - base_map2_sample2.x)));
        
        float4 palette_sample_1 = tex2D(palette, float2(palette_tex.x, palette_v));
        palette_sample_1.a = alpha_map_sample.a;
        float4 palette_sample_2 = tex2D(palette, float2(palette_tex.y, palette_v));
        palette_sample_2.a = alpha_map_sample.a;
        
        float4 interpolated = lerp(palette_sample_1, palette_sample_2, frame_blend);
        
        albedo += interpolated;
    }
    else
    {
        float2 base_map_texcoord = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample = tex2D(base_map, base_map_texcoord);
        float2 base_map2_texcoord = apply_xform2d(texcoord.xy, base_map2_xform);
        float4 base_map2_sample = tex2D(base_map2, base_map2_texcoord);
        
        float2 alpha_map_tex = apply_xform2d(alpha_map_texcoord, alpha_map_xform);
        float4 alpha_map_sample = tex2D(alpha_map, alpha_map_tex);
        
        float palette_tex = -alpha_map_sample.w * color_alpha + 1.0f;
        palette_tex = saturate(palette_tex * alpha_modulation_factor.x + abs(base_map_sample.x - base_map2_sample.x));
        
        albedo.rgb = tex2D(palette, float2(palette_tex, palette_v)).rgb;
        albedo.a = alpha_map_sample.w;
    }
    
    return albedo;
}

// Halo Reach

float4 albedo_palettized_2d_plasma(in float4 texcoord, in float2 alpha_map_texcoord, in float frame_blend, in float palette_v, in float color_alpha)
{
    float4 albedo = 0;
    
    if (frame_blend_arg == k_frame_blend_on)
    {
        float4 base_map_sample = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
        float4 base_map_sample2 = tex2D(base_map, apply_xform2d(texcoord.zw, base_map_xform));
        
        float4 base_map2_sample = tex2D(base_map2, apply_xform2d(texcoord.xy, base_map2_xform));
        float4 base_map2_sample2 = tex2D(base_map2, apply_xform2d(texcoord.zw, base_map2_xform));
    
        float4 alpha_map_sample = tex2D(alpha_map, apply_xform2d(alpha_map_texcoord, alpha_map_xform));
        
        float2 palette_tex = abs(float2(base_map_sample.x - base_map2_sample.x, base_map_sample2.x - base_map2_sample2.x));
        
        float4 palette_sample_1 = tex2D(palette, float2(palette_tex.x, 0.0f));
        palette_sample_1.a = alpha_map_sample.a;
        float4 palette_sample_2 = tex2D(palette, float2(palette_tex.y, 0.0f));
        palette_sample_2.a = alpha_map_sample.a;
        
        float4 interpolated = lerp(palette_sample_1, palette_sample_2, frame_blend);
        
        albedo += interpolated;
    }
    else
    {
        float4 base_map_sample = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
        float4 base_map2_sample = tex2D(base_map2, apply_xform2d(texcoord.xy, base_map2_xform));
        
        // the v-coord may be wrong, couldn't check the constant at the time of reversing the shader. palettes are generally 1 pixel in height so 0 should be fine.
        albedo.rgb = tex2D(palette, float2(abs(base_map_sample.r - base_map2_sample.r), 0.0f)).rgb;
        albedo.a = tex2D(alpha_map, apply_xform2d(alpha_map_texcoord, alpha_map_xform)).a;
    }
    
    return albedo;
}

#ifndef particle_albedo
#define particle_albedo albedo_diffuse_only
#endif

#endif