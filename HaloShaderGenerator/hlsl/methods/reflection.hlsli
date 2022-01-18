#ifndef _REFLECTION_HLSLi
#define _REFLECTION_HLSLi

#include "..\helpers\definition_helper.hlsli"

uniform float reflection_coefficient;
uniform float sunspot_cut;
uniform float shadow_intensity_mark;
uniform samplerCUBE environment_map;
uniform float normal_variation_tweak;

float3 calc_reflection_none_ps(float3 inc_n, float3 normal, float3 lightprobe_color)
{
    return 0.0f;
}

float3 calc_reflection_static_ps(float3 inc_n, float3 normal, float3 lightprobe_color)
{
    float3 l_shad = saturate(lightprobe_color - shadow_intensity_mark);
    float light_scale = dot(l_shad, l_shad);

#if reach_compatibility_arg == k_reach_compatibility_enabled
    float z_tweak = (1.0f - normal_variation_tweak) * (1.0f - normal.z);
    float2 xy_tweak = normal.xy * -(1.0f - normal_variation_tweak);
    float3 env_tex = reflect(-inc_n, normal + float3(xy_tweak, z_tweak));
#else
    float3 env_tex = reflect(-inc_n, normal);
#endif
    
    env_tex.z = abs(env_tex.z);
    float4 env_sample = texCUBE(environment_map, env_tex);
    
    float3 reflection = env_sample.xyz;
    reflection *= saturate(env_sample.w - sunspot_cut.x) * light_scale + min(sunspot_cut.x, env_sample.w);
    reflection *= reflection_coefficient;
    return reflection;
}

float3 calc_reflection_dynamic_ps(float3 inc_n, float3 normal, float3 lightprobe_color)
{
    return float3(1, 2, 3); // TODO
}


#ifndef calc_reflection_ps
#define calc_reflection_ps calc_reflection_none_ps
#endif

#endif