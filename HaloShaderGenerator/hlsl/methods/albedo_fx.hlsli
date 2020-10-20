#ifndef _ALBEDO_FX_HLSLI
#define _ALBEDO_FX_HLSLI

#include "../helpers/definition_helper.hlsli"
#include "../helpers/types.hlsli"

uniform sampler base_map;
uniform float4 base_map_xform;
uniform sampler base_map2;
uniform float4 base_map2_xform;
uniform sampler palette;
uniform sampler alpha_map;
uniform float alpha_modulation_factor;
uniform float center_offset;
uniform float falloff;

float4 albedo_diffuse_only(float2 texcoord, float2 unknown_texcoord, float v_coord)
{
    return tex2D(base_map, texcoord);
}

float4 albedo_palettized(float2 texcoord, float2 unknown_texcoord, float v_coord)
{
    float u_coord = tex2D(base_map, texcoord).r;
    return tex2D(palette, float2(u_coord, v_coord));
}

float4 albedo_palettized_plus_alpha(float2 texcoord, float2 unknown_texcoord, float v_coord)
{
    float u_coord = tex2D(base_map, texcoord).r;
    
    return float4(tex2D(palette, float2(u_coord, v_coord)).rgb, tex2D(alpha_map, texcoord).a);
}

// -- beam only
float4 albedo_palettized_plasma(float2 texcoord, float2 unknown_texcoord, float v_coord)
{
    float4 base_map_sample = tex2D(base_map, apply_xform2d(texcoord, base_map_xform));
    float4 base_map2_sample = tex2D(base_map2, apply_xform2d(texcoord, base_map2_xform));
    float4 alpha_map_sample = tex2D(alpha_map, texcoord);
        
    float u_coord = saturate(-alpha_map_sample.a * alpha_modulation_factor.x + abs(base_map_sample.r - base_map2_sample.r));
        
    return float4(tex2D(palette, float2(u_coord, v_coord)).rgb, alpha_map_sample.a);
}

float4 albedo_palettized_2d(float2 texcoord, float2 unknown_texcoord, float v_coord)
{
    float4 base_map_sample = tex2D(base_map, apply_xform2d(texcoord, base_map_xform));
    float4 base_map2_sample = tex2D(base_map2, apply_xform2d(texcoord, base_map2_xform));
        
    float u_coord = abs(base_map_sample.r - base_map2_sample.r);
        
    return float4(tex2D(palette, float2(u_coord, v_coord)).rgb, tex2D(alpha_map, texcoord).a);
}
// -- end beam only

// -- light volume only
float4 albedo_circular(float2 texcoord, float2 unknown_texcoord, float v_coord)
{
    // TODO: figure out what this is, using texcoord for now
    float2 unknown_var = texcoord;
    
    unknown_var = unknown_var * 2.0f - 1.0f;
    float result = dot(unknown_var, unknown_var);
    result = saturate(result * center_offset + center_offset);
    result = pow(result, falloff);
    
    return float4(result, result, result, 1.0f);
}
// -- end light volume only

#ifndef contrail_albedo
#define contrail_albedo albedo_diffuse_only
#endif

#ifndef beam_albedo
#define beam_albedo albedo_diffuse_only
#endif

#ifndef light_volume_albedo
#define light_volume_albedo albedo_diffuse_only
#endif

#endif