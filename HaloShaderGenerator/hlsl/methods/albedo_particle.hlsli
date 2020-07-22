#ifndef _ALBEDO_PARTICLE_HLSLI
#define _ALBEDO_PARTICLE_HLSLI

#include "../helpers/definition_helper.hlsli"
#include "../helpers/types.hlsli"

uniform sampler base_map;
uniform xform2d base_map_xform;
uniform sampler palette;
uniform xform2d palette_xform;
uniform sampler alpha_map;
uniform xform2d alpha_map_xform;

float4 albedo_diffuse_only(in float4 texcoord, in float3 alpha_tex, in float frame_blend, in float4 input_color)
{
    float4 base_map_sample;
	
    if (frame_blend_arg == k_frame_blend_on)
    {
        float2 base_map_texcoord_1 = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample_1 = tex2D(base_map, base_map_texcoord_1);
        float2 base_map_texcoord_2 = apply_xform2d(texcoord.zw, base_map_xform);
        float4 base_map_sample_2 = tex2D(base_map, base_map_texcoord_2);
		
        base_map_sample = lerp(base_map_sample_1, base_map_sample_2, frame_blend);
        //dest = src0 * src1 + (1-src0) * src2
        //base_map_sample = base_map_sample_1 + frame_blend * (base_map_sample_2 - base_map_sample_1);
    }
    else
    {
        float2 base_map_texcoord = apply_xform2d(texcoord.xy, base_map_xform);
        base_map_sample = tex2D(base_map, base_map_texcoord);
    }
	
    return base_map_sample;
}

float4 albedo_diffuse_plus_billboard_alpha(in float4 texcoord, in float3 alpha_tex, in float frame_blend, in float4 input_color)
{
    float4 albedo;
    
    albedo = albedo_diffuse_only(texcoord, alpha_tex, frame_blend, input_color);
    
    float2 alpha_map_texcoord = apply_xform2d(alpha_tex.yz, alpha_map_xform);
    float4 alpha_map_sample = tex2D(alpha_map, alpha_map_texcoord);
    albedo.a = alpha_map_sample.w;
    
    return albedo;
}

#ifndef particle_albedo
#define particle_albedo albedo_diffuse_only
#endif

#endif