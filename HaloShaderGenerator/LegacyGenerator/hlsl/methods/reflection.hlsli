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

float3 calc_reflection_static_reach_ps(float3 inc_n, float3 normal, float3 lightprobe_color)
{		
    float3 reflect_dir = reflect(-inc_n, lerp(normal, float3(0.0f, 0.0f, 1.0f), 1.0f - normal_variation_tweak));
    reflect_dir.y *= -1.0;

    float4 environment_sample = texCUBE(environment_map, reflect_dir);
    environment_sample.rgb *= 256;

    float sun_mag = dot(saturate(lightprobe_color - shadow_intensity_mark), saturate(lightprobe_color - shadow_intensity_mark));

    float sun_cut = saturate(environment_sample.a - sunspot_cut) * sun_mag + min(environment_sample.a, sunspot_cut);
    return environment_sample.rgb * sun_cut * reflection_coefficient;
}

float3 calc_reflection_static_ps(float3 inc_n, float3 normal, float3 lightprobe_color)
{
#if reach_compatibility_arg == k_reach_compatibility_enabled
    return calc_reflection_static_reach_ps(inc_n, normal, lightprobe_color);
#endif

    float3 l_shad = saturate(lightprobe_color - shadow_intensity_mark);
    float light_scale = dot(l_shad, l_shad);

#if reach_compatibility_arg == k_reach_compatibility_enabled
    //float z_tweak = (1.0f - normal_variation_tweak) * (1.0f - normal.z);
    //float2 xy_tweak = normal.xy * -(1.0f - normal_variation_tweak);
    //float3 env_tex = reflect(-inc_n, normal + float3(xy_tweak, z_tweak));
    float3 normal_reflect = lerp(normal, float3(0.0f, 0.0f, 1.0f), 1.0f - normal_variation_tweak);
    float3 env_tex = reflect(-inc_n, normal_reflect);
#else
    float3 env_tex = reflect(-inc_n, normal);
#endif
    
    env_tex.z = abs(env_tex.z);
    float4 env_sample = texCUBE(environment_map, env_tex);
    
    float env_scale = 1.0f;
#if reach_compatibility_arg == k_reach_compatibility_enabled
    env_scale = 256.0f;
#endif
    
    float3 reflection = env_sample.xyz * env_scale;
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