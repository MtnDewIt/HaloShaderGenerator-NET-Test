#ifndef _SOFT_FADE_HLSLI
#define _SOFT_FADE_HLSLI

#include "../helpers/definition_helper.hlsli"

uniform sampler2D depth_map;
uniform bool use_soft_fresnel;
uniform float soft_fresnel_power;
uniform bool use_soft_z;
uniform float soft_z_range;
uniform float4 screen_params;

void apply_soft_fade(inout float4 albedo, in float v_dot_n, in float4 position)
{
#ifdef soft_fade_arg
    if (soft_fade_arg == k_soft_fade_on)
    {
        float f0 = saturate(abs(v_dot_n));
        float f1 = (3.0f - 2.0f * f0) * pow(f0, 2.0f);
        float soft_fresnel = pow(f1, soft_fresnel_power);
        
        float albedo_fade = use_soft_fresnel > 0 ? soft_fresnel : 1.0f;
        
        if (use_soft_z)
        {
            float depth = tex2D(depth_map, (position.xy + 0.5f) * screen_params.xy).x;
            depth = -0.078125596f / (depth - 1.00000763f) - position.w;
            
            float soft_z = saturate(depth * soft_z_range);
            albedo_fade *= soft_z;
        }
        
#ifndef blend_type_arg
        albedo.rgb *= albedo_fade;
#else
        if (blend_type_arg == k_blend_mode_alpha_blend)
            albedo.a *= albedo_fade;
        else
            albedo.rgb *= albedo_fade;
#endif
    }
#endif
}

#endif

