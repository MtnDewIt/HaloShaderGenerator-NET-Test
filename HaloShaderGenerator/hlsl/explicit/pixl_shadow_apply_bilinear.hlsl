#ifndef _PIXL_SHADOW_APPLY_BILINEAR_HLSLI
#define _PIXL_SHADOW_APPLY_BILINEAR_HLSLI

#include "..\registers\shadow_parameters.hlsli"
#include "..\registers\global_parameters.hlsli"
#include "..\helpers\explicit_input_output.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\color_processing.hlsli"

#define SHADOW_SIZE 512
#define INV_SHADOW_SIZE 1.0f / SHADOW_SIZE

#define DYNAMIC_RESOLUTION 0

#if DYNAMIC_RESOLUTION
float4 shadow_resolution_constants : register(c12); // shadow_texture_size, inv_shadow_texture_size
#endif

float2 offset_shadow_texcoord(float2 texcoord, float offsetx, float offsety)
{
    float2 offsets = float2(offsetx * INV_SHADOW_SIZE, offsety * INV_SHADOW_SIZE);
    
#if DYNAMIC_RESOLUTION
    offsets = float2(offsetx * shadow_resolution_constants.y, offsety * shadow_resolution_constants.y);
#endif
    
    return texcoord + offsets;
}

PS_OUTPUT_DEFAULT main(VS_OUTPUT_DEFAULT input) : COLOR
{
    float2 buffer_uv = apply_xform2d(input.position.xy, p_lighting_constant_6);
    float4 zbuffer_sample = tex2D(zbuffer, buffer_uv);
    float4 normal_buffer_sample = tex2D(normal_buffer, buffer_uv);
    
    float neg_depth = -zbuffer_sample.x;
    
    float2 shadow_pixel_pos = apply_xform2d(input.position.xy, p_lighting_constant_7);
    shadow_pixel_pos *= neg_depth;
    
    // this is a matrix mul()
    
    float dot4_unknown0 = dot(float4(shadow_pixel_pos, neg_depth, 1.0f), p_lighting_constant_0);
    float dot4_unknown1 = dot(float4(shadow_pixel_pos, neg_depth, 1.0f), p_lighting_constant_1);
    float dot4_unknown2 = dot(float4(shadow_pixel_pos, neg_depth, 1.0f), p_lighting_constant_2);

    float3 normal = normal_buffer_sample.xyz * 2.0f - 1.0f;
    float cos_normal = dot(normal, p_lighting_constant_9.xyz);
    float max_thing = max(cos_normal, 0.242535621);
    
    cos_normal = saturate(cos_normal);
    
    float tang = sqrt(max_thing * -max_thing + 1.0f) / max_thing;
    tang += 0.200000003f;
    float depth_val = tang * p_lighting_constant_8.z; // this looks to be scaling already...

    float3 r2xyz = p_lighting_constant_3.xyz;
    r2xyz = zbuffer_sample.x * r2xyz + p_lighting_constant_4.xyz;
    r2xyz *= g_exposure.x;
    
    float3 r3 = depth_val * float3(-3, -4.47213602, -1) + dot4_unknown2;
    
    float shadow_dark = saturate(dot4_unknown2 * 2.0f - 1.0f);
    shadow_dark = pow(shadow_dark, 3);
    shadow_dark = 1.0f - shadow_dark;
    shadow_dark *= k_ps_constant_shadow_alpha.x;
    
    float2 shadow_tex = float2(dot4_unknown0, dot4_unknown1);
    
#if DYNAMIC_RESOLUTION
    float shadow_size = shadow_resolution_constants.x;
#else
    float shadow_size = SHADOW_SIZE;
#endif
    float2 size_fraction = frac(shadow_tex * shadow_size);

    float4 shadow0 = tex2D(shadow, offset_shadow_texcoord(shadow_tex, -1.0f, -1.0f));
    float4 shadow1 = tex2D(shadow, offset_shadow_texcoord(shadow_tex, +0.0f, -1.0f));
    float4 shadow2 = tex2D(shadow, offset_shadow_texcoord(shadow_tex, +1.0f, -1.0f));
    float4 shadow3 = tex2D(shadow, offset_shadow_texcoord(shadow_tex, -1.0f, +0.0f));
    float4 shadow4 = tex2D(shadow, offset_shadow_texcoord(shadow_tex, +0.0f, +0.0f));
    float4 shadow5 = tex2D(shadow, offset_shadow_texcoord(shadow_tex, +1.0f, +0.0f));
    float4 shadow6 = tex2D(shadow, offset_shadow_texcoord(shadow_tex, -1.0f, +1.0f));
    float4 shadow7 = tex2D(shadow, offset_shadow_texcoord(shadow_tex, +0.0f, +1.0f));
    float4 shadow8 = tex2D(shadow, offset_shadow_texcoord(shadow_tex, +1.0f, +1.0f));

    float final_shadow;
    
    // 0 and 1
    
    float depth0 = shadow0.x - r3.x;
    float depth1 = shadow1.x - r3.y;
    
    float2 frac_blend = 1.0f - size_fraction.yx;
    float frac_blend2 = frac_blend.x * frac_blend.y;
    float2 frac_blend3 = frac_blend * size_fraction;
    float size_frac2 = size_fraction.x * size_fraction.y;
    
    depth0 = depth0 >= 0 ? frac_blend2 : 0;
    depth1 = depth1 >= 0 ? frac_blend.x : 0;
    final_shadow = depth1 + depth0;
    
    // 2 and 3
    
    float depth2 = shadow2.x - r3.x;
    float depth3 = shadow3.x - r3.y;
    
    depth2 = depth2 >= 0 ? frac_blend3.x : 0;
    depth3 = depth3 >= 0 ? frac_blend.y : 0;
    final_shadow += depth2;
    final_shadow += depth3;
    
    // 4
    
    float depth4 = shadow4.x - r3.z;
    depth4 = depth4 >= 0 ? 1 : 0;
    final_shadow += depth4;

    // 5 and 6
    
    float depth5 = shadow5.x - r3.y;
    float depth6 = shadow6.x - r3.x;
    
    depth5 = depth5 >= 0 ? size_fraction.x : 0;
    depth6 = depth6 >= 0 ? frac_blend3.y : 0;
    final_shadow += depth5;
    final_shadow += depth6;
    
    // 7 and 8
    
    float depth7 = shadow7.x - r3.y;
    float depth8 = shadow8.x - r3.x;
    
    depth7 = depth7 >= 0 ? size_fraction.y : 0;
    depth8 = depth8 >= 0 ? size_frac2 : 0;
    final_shadow += depth7;
    final_shadow += depth8;
    
    float r0_w = cos_normal * shadow_dark;
    float2 r0xy = shadow_dark * -cos_normal + float2(0.00100000005, 1.00999999);
    final_shadow *= r0_w;
    float r0y = saturate(final_shadow * 0.25 + r0xy.y);
    r0y *= r0y;
    r0xy.x = r0xy.x >= 0 ? 1 : r0y;
    
    return export_color(float4(r2xyz, r0xy.x));
}

#endif