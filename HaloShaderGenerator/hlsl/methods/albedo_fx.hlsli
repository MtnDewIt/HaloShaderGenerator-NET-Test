#ifndef _ALBEDO_FX_HLSLI
#define _ALBEDO_FX_HLSLI

#include "../helpers/definition_helper.hlsli"
#include "../helpers/types.hlsli"

// fixup so dont have to include macro in all generators
#ifndef frame_blend_arg
#define frame_blend_arg 0
#define k_frame_blend_off 0
#define k_frame_blend_on 1
#endif

uniform sampler base_map;
uniform xform2d base_map_xform;
uniform sampler base_map2;
uniform xform2d base_map2_xform;

#if shadertype == k_shadertype_decal
uniform sampler alpha_map;
uniform sampler palette;
#else
uniform sampler palette;
uniform sampler alpha_map;
#endif
uniform xform2d alpha_map_xform;
uniform float alpha_modulation_factor;
uniform float center_offset;
uniform float falloff;

float4 calc_albedo_diffuse_only_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    if (frame_blend_arg == k_frame_blend_on)
    {
        float4 base_map_sample_1 = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
        float4 base_map_sample_2 = tex2D(base_map, apply_xform2d(texcoord.zw, base_map_xform));
		
        return lerp(base_map_sample_1, base_map_sample_2, frame_blend);
    }
    
    if (shadertype == k_shadertype_particle)
        return tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
    
    return tex2D(base_map, texcoord.xy);
}

float4 calc_albedo_diffuse_plus_billboard_alpha_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    if (frame_blend_arg == k_frame_blend_on)
    {
        float alpha = tex2D(alpha_map, apply_xform2d(billboard_tex, alpha_map_xform)).a;
        
        float4 base_map_sample_1 = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
        base_map_sample_1.a = alpha;
        
        float4 base_map_sample_2 = tex2D(base_map, apply_xform2d(texcoord.zw, base_map_xform));
        base_map_sample_2.a = alpha;
        
        return lerp(base_map_sample_1, base_map_sample_2, frame_blend);
    }

    float3 albedo = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform)).rgb;
    float alpha = tex2D(alpha_map, apply_xform2d(billboard_tex, alpha_map_xform)).a;
    
    return float4(albedo, alpha);
}

float4 calc_albedo_palettized_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    if (frame_blend_arg == k_frame_blend_on)
    {
        float4 base_map_sample_1 = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
        float4 base_map_sample_2 = tex2D(base_map, apply_xform2d(texcoord.zw, base_map_xform));
        
        float4 palette_sample_1 = tex2D(palette, float2(base_map_sample_1.r, v_coord));
        float4 palette_sample_2 = tex2D(palette, float2(base_map_sample_2.r, v_coord));
		
        return lerp(palette_sample_1, palette_sample_2, frame_blend);
    }
    
    if (shadertype == k_shadertype_particle)
    {
        float u_coord = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform)).r;
        return tex2D(palette, float2(u_coord, v_coord));
    }
    
    float u_coord = tex2D(base_map, texcoord.xy).r;
    return tex2D(palette, float2(u_coord, v_coord));
}

float4 calc_albedo_palettized_plus_billboard_alpha_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    if (frame_blend_arg == k_frame_blend_on)
    {
        float4 base_map_sample_1 = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
        float4 base_map_sample_2 = tex2D(base_map, apply_xform2d(texcoord.zw, base_map_xform));
        float alpha = tex2D(alpha_map, apply_xform2d(billboard_tex, alpha_map_xform)).a;
        
        float4 palette_sample_1 = tex2D(palette, float2(base_map_sample_1.r, v_coord));
        palette_sample_1.a = alpha;
        float4 palette_sample_2 = tex2D(palette, float2(base_map_sample_2.r, v_coord));
        palette_sample_2.a = alpha;
        
        return lerp(palette_sample_1, palette_sample_2, frame_blend);
    }
    else
    {
        float u_coord = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform)).r;
        float3 palette_sample = tex2D(palette, float2(u_coord, v_coord)).rgb;
        float alpha = tex2D(alpha_map, apply_xform2d(billboard_tex, alpha_map_xform)).a;
        
        return float4(palette_sample, alpha);
    }
}

float4 calc_albedo_palettized_plus_alpha_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    if (frame_blend_arg == k_frame_blend_on)
    {
        float4 base_map_sample_1 = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
        float4 base_map_sample_2 = tex2D(base_map, apply_xform2d(texcoord.zw, base_map_xform));
        float alpha_frame1 = tex2D(alpha_map, apply_xform2d(texcoord.xy, alpha_map_xform)).a;
        float alpha_frame2 = tex2D(alpha_map, apply_xform2d(texcoord.zw, alpha_map_xform)).a;
        
        float4 frame1 = tex2D(palette, float2(base_map_sample_1.x, v_coord));
        frame1.a = alpha_frame1;
        float4 frame2 = tex2D(palette, float2(base_map_sample_2.x, v_coord));
        frame2.a = alpha_frame2;
        
        return lerp(frame1, frame2, frame_blend);
    }
    
    float2 basemap_tex = texcoord.xy;
    float2 alphamap_tex = texcoord.xy;
    if (shadertype == k_shadertype_particle)
    {
        basemap_tex = apply_xform2d(texcoord.xy, base_map_xform);
        alphamap_tex = apply_xform2d(texcoord.xy, alpha_map_xform);
    }
    
    float u_coord = tex2D(base_map, basemap_tex).r;
    float3 palette_sample = tex2D(palette, float2(u_coord, v_coord)).rgb;
    float alpha = tex2D(alpha_map, alphamap_tex).a;
    
    return float4(palette_sample, alpha);
}

float4 calc_albedo_palettized_plus_sprite_alpha_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    return calc_albedo_palettized_plus_alpha_ps(texcoord, billboard_tex, v_coord, frame_blend, color_alpha, depth_fade_val);
}

float4 calc_albedo_diffuse_plus_sprite_alpha_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    if (frame_blend_arg == k_frame_blend_on)
    {
        float alpha = tex2D(alpha_map, apply_xform2d(texcoord.xy, alpha_map_xform)).a;
        
        float4 base_map_sample_1 = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
        base_map_sample_1.a = alpha;
        float4 base_map_sample_2 = tex2D(base_map, apply_xform2d(texcoord.zw, base_map_xform));
        base_map_sample_2.a = alpha;
    
        return lerp(base_map_sample_1, base_map_sample_2, frame_blend);
    }
    else
    {
        if (shadertype == k_shadertype_decal)
        {
            float3 albedo = tex2D(base_map, texcoord.xy).rgb;
            float alpha = tex2D(alpha_map, texcoord.xy).a;
            return float4(albedo, alpha);
        }
        
        float3 albedo = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform)).rgb;
        float alpha = tex2D(alpha_map, apply_xform2d(texcoord.xy, alpha_map_xform)).a;
        return float4(albedo, alpha);
    }
}

float4 calc_albedo_diffuse_plus_alpha_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    return calc_albedo_diffuse_plus_sprite_alpha_ps(texcoord, billboard_tex, v_coord, frame_blend, color_alpha, depth_fade_val);
}

// prevent register confliction with decal
#if shadertype == k_shadertype_particle
uniform float4 tint_color;
uniform float modulation_factor;

float4 calc_albedo_diffuse_modulated_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    if (frame_blend_arg == k_frame_blend_on)
    {
        float4 base_map_sample_1 = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
        float4 base_map_sample_2 = tex2D(base_map, apply_xform2d(texcoord.zw, base_map_xform));
        
        float3 inv_tint = 1.0f - tint_color.rgb;
        
        float dot_frame1 = dot(base_map_sample_1.rgb, base_map_sample_1.rgb);
        float dot_frame2 = dot(base_map_sample_2.rgb, base_map_sample_2.rgb);
        
        float2 modulation = (modulation_factor * 0.57735f) * float2(dot_frame1, dot_frame2);
        
        float3 frame1_tint = modulation.x * inv_tint.rgb + tint_color.rgb;
        float3 frame2_tint = modulation.y * inv_tint.rgb + tint_color.rgb;
        
        // lerp
        float alpha = (base_map_sample_2.a - base_map_sample_1.a) * frame_blend.x + base_map_sample_1.a;
        float3 frame_color = (frame2_tint.rgb * base_map_sample_2.rgb) - (frame1_tint.rgb * base_map_sample_1.rgb);
        frame_color = frame_color * frame_blend.x + (frame1_tint.rgb * base_map_sample_1.rgb);
        
        return float4(frame_color, alpha);
    }
    
    float4 albedo = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
    
    float modulation = modulation_factor * 0.57735f * dot(albedo.rgb, albedo.rgb);
    float3 tint = modulation.x * (1.0f - tint_color.rgb) + tint_color.rgb;
    albedo.rgb *= tint;
    
    return albedo;
}
#endif

float4 calc_albedo_palettized_plasma_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    if (shadertype == k_shadertype_beam)
    {
        float4 base_map_sample = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
        float4 base_map2_sample = tex2D(base_map2, apply_xform2d(texcoord.xy, base_map2_xform));
        float4 alpha_map_sample = tex2D(alpha_map, texcoord.xy);
        
        float u_coord = saturate(-alpha_map_sample.a * alpha_modulation_factor.x + abs(base_map_sample.r - base_map2_sample.r));
        
        return float4(tex2D(palette, float2(u_coord, v_coord)).rgb, alpha_map_sample.a);
    }
    else // particle
    {
        if (frame_blend_arg == k_frame_blend_on)
        {
            float4 base_map_sample = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
            float4 base_map_sample2 = tex2D(base_map, apply_xform2d(texcoord.zw, base_map_xform));
            float4 base_map2_sample = tex2D(base_map2, apply_xform2d(texcoord.xy, base_map2_xform));
            float4 base_map2_sample2 = tex2D(base_map2, apply_xform2d(texcoord.zw, base_map2_xform));
            float alpha = tex2D(alpha_map, billboard_tex).a;
        
            float2 palette_tex = float(-alpha * color_alpha + 1.0f).xx;
            palette_tex = saturate(palette_tex * alpha_modulation_factor.x + abs(float2(base_map_sample.x - base_map2_sample.x, base_map_sample2.x - base_map2_sample2.x)));
        
            float4 palette_sample_1 = tex2D(palette, float2(palette_tex.x, v_coord));
            palette_sample_1.a = alpha;
            float4 palette_sample_2 = tex2D(palette, float2(palette_tex.y, v_coord));
            palette_sample_2.a = alpha;
        
            return lerp(palette_sample_1, palette_sample_2, frame_blend);
        }
        else
        {
            float4 base_map_sample = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
            float4 base_map2_sample = tex2D(base_map2, apply_xform2d(texcoord.xy, base_map2_xform));
            float alpha = tex2D(alpha_map, billboard_tex).a;
        
            float u_coord = -alpha * color_alpha + 1.0f;
            u_coord = saturate(u_coord * alpha_modulation_factor.x + abs(base_map_sample.x - base_map2_sample.x));
        
            return float4(tex2D(palette, float2(u_coord, v_coord)).rgb, alpha);
        }
    }
}

float4 calc_albedo_palettized_2d_plasma_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    float depth_fade = color_alpha;
    
    if (shadertype == k_shadertype_beam)
    {
        float4 base_map_sample = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
        float4 base_map2_sample = tex2D(base_map2, apply_xform2d(texcoord.xy, base_map2_xform));
        float alpha = tex2D(alpha_map, texcoord.xy).a;
        
        float2 palette_tex = float2(abs(base_map_sample.r - base_map2_sample.r), depth_fade_val);
        return float4(tex2D(palette, palette_tex).rgb, alpha);
    }
    else
    {
        if (frame_blend_arg == k_frame_blend_on)
        {
            float4 base_map_sample = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
            float4 base_map_sample2 = tex2D(base_map, apply_xform2d(texcoord.zw, base_map_xform));
            float4 base_map2_sample = tex2D(base_map2, apply_xform2d(texcoord.xy, base_map2_xform));
            float4 base_map2_sample2 = tex2D(base_map2, apply_xform2d(texcoord.zw, base_map2_xform));
            float alpha = tex2D(alpha_map, billboard_tex).a;
        
            float2 palette_u = abs(float2(base_map_sample.r - base_map2_sample.r, base_map_sample2.r - base_map2_sample2.r));
        
            float4 palette_sample_1 = tex2D(palette, float2(palette_u.x, depth_fade_val));
            palette_sample_1.a = alpha;
            float4 palette_sample_2 = tex2D(palette, float2(palette_u.y, depth_fade_val));
            palette_sample_2.a = alpha;
        
            return lerp(palette_sample_1, palette_sample_2, frame_blend);
        }
        else
        {
            float4 base_map_sample = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform));
            float4 base_map2_sample = tex2D(base_map2, apply_xform2d(texcoord.xy, base_map2_xform));
            float alpha = tex2D(alpha_map, billboard_tex).a;
        
            float2 palette_tex = float2(abs(base_map_sample.r - base_map2_sample.r), depth_fade_val);
            return float4(tex2D(palette, palette_tex).rgb, alpha);
        }
    }
}

float4 calc_albedo_circular_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    float2 tex = texcoord.xy;
    tex = tex * 2.0f - 1.0f;
    float result = saturate(center_offset - dot(tex, tex) * center_offset);
    result = pow(result, falloff);
    return float4(result, result, result, 1.0f);
}

float4 calc_albedo_diffuse_plus_alpha_mask_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    float3 albedo = tex2D(base_map, texcoord.xy).rgb;
    float alpha_mask = tex2D(alpha_map, billboard_tex).a;
    return float4(albedo, alpha_mask);
}

float4 calc_albedo_palettized_plus_alpha_mask_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    float u_coord = tex2D(base_map, texcoord.xy).r;
    float3 albedo = tex2D(palette, float2(u_coord, v_coord)).rgb;
    float alpha_mask = tex2D(alpha_map, billboard_tex).a;
    
    return float4(albedo, alpha_mask);
}

// macro here to prevent register conflictions
#if shadertype == k_shadertype_decal
uniform sampler tex0_sampler : register(s0);
uniform sampler tex1_sampler : register(s1);
uniform sampler change_color_map; // no transform
uniform sampler vector_map; // no transform

uniform float3 primary_change_color;
uniform float3 secondary_change_color;
uniform float3 tertiary_change_color;
uniform float antialias_tweak;
uniform float vector_sharpness;

float4 calc_albedo_emblem_change_color_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    return tex2D(tex0_sampler, texcoord.xy);
}

float4 calc_albedo_change_color_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    float4 albedo = tex2D(change_color_map, texcoord.xy);
    
    float3 primary_color = albedo.r * primary_change_color.rgb + (1.0f - albedo.r);
    float3 secondary_color = albedo.g * secondary_change_color.rgb + (1.0f - albedo.g);
    float3 tertiary_color = albedo.b * tertiary_change_color.rgb + (1.0f - albedo.b);
    
    float3 final_color = primary_color * secondary_color * tertiary_color;
    
    return float4(final_color, albedo.a);
}

float4 calc_albedo_vector_alpha_ps(float4 texcoord, float2 billboard_tex, float v_coord, in float frame_blend, in float color_alpha, in float depth_fade_val)
{
    float3 albedo = tex2D(base_map, apply_xform2d(texcoord.xy, base_map_xform)).rgb;

    float antialias = max(1000.f * antialias_tweak, 1.0f);
    float sharpness = min(vector_sharpness, antialias);
    float4 vector_sample = tex2D(vector_map, texcoord.xy);
    float alpha = saturate((vector_sample.g * -0.5f) * sharpness + 0.5f);
    
    return float4(albedo.rgb, alpha);
}
#endif

#ifndef calc_albedo_ps
#define calc_albedo_ps calc_albedo_diffuse_only_ps
#endif

#endif