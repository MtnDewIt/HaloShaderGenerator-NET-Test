#ifndef _ALBEDO_PARTICLE_HLSLI
#define _ALBEDO_PARTICLE_HLSLI

#include "../helpers/definition_helper.hlsli"
#include "../helpers/types.hlsli"

uniform sampler base_map;
uniform xform2d base_map_xform;

float4 albedo_diffuse_only(in float4 texcoord, in float frame_blend)
{
    float4 base_map_sample;
	
    if (frame_blend_arg == k_frame_blend_on)
    {
        float2 base_map_texcoord_1 = apply_xform2d(texcoord.xy, base_map_xform);
        float4 base_map_sample_1 = tex2D(base_map, base_map_texcoord_1);
        float2 base_map_texcoord_2 = apply_xform2d(texcoord.zw, base_map_xform);
        float4 base_map_sample_2 = tex2D(base_map, base_map_texcoord_2);
		
        base_map_sample = base_map_sample_1 + frame_blend * (base_map_sample_2 - base_map_sample_1);
    }
    else
    {
        float2 base_map_texcoord = apply_xform2d(texcoord.xy, base_map_xform);
        base_map_sample = tex2D(base_map, base_map_texcoord);
    }
	
    return base_map_sample;
}

#ifndef particle_albedo
#define particle_albedo albedo_diffuse_only
#endif

#endif