#ifndef _WATER_TEMPLATE_PER_PIXEL_LIGHTING_HLSLI
#define _WATER_TEMPLATE_PER_PIXEL_LIGHTING_HLSLI

uniform sampler2D tex_ripple_buffer_slope_height : register(s1);

#include "..\material_models\lambert.hlsli"
#include "..\helpers\water_lighting.hlsli"

#include "..\helpers\definition_helper.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\types.hlsli"

#include "..\methods\waveshape.hlsli"
uniform bool no_dynamic_lights;
#include "..\methods\foam.hlsli"
#include "..\methods\refraction.hlsli"
#include "..\methods\reflection.hlsli"
uniform float fresnel_coefficient;
uniform float fresnel_dark_spot;
#include "..\methods\watercolor.hlsli"
uniform float3 water_diffuse;
#include "..\methods\bankalpha.hlsli"
#include "..\registers\water_parameters.hlsli"

float calc_fresnel(float3 inc, float3 normal)
{
#if reach_compatibility_arg == k_reach_compatibility_enabled
    float fresnel_dark = saturate(fresnel_dark_spot - saturate(dot(inc, normal)));
    return saturate(fresnel_coefficient * fresnel_dark * fresnel_dark);
#endif
    return fresnel_coefficient + (1.0f - fresnel_coefficient) * pow(1.0f - saturate(dot(inc, normal)), 2.5f);
}

PS_OUTPUT_DEFAULT water_entry_static_per_pixel(VS_OUTPUT_WATER input)
{
    float3 tangent = input.tangent__height_scale.xyz;
    float3 binormal = input.binormal__height_scale_aux.xyz;
    float3 incident_ws = input.incident_ws__view_dist.xyz;
    float3 position_ws = input.position_ws__water_depth.xyz;

    float height_scale = input.tangent__height_scale.w;
    float height_scale_aux = input.binormal__height_scale_aux.w;
    float view_dist = input.incident_ws__view_dist.w;
    float water_depth = input.position_ws__water_depth.w;
    
    float2 ripple_buffer_height = 0.0f;
    float ripple_buffer_foam = 0.0f;
    if (k_is_water_interaction)
    {
        float4 ripple_buffer_sample = tex2Dlod(tex_ripple_buffer_slope_height, float4(input.lm_tex.zw, 0, 0));
        ripple_buffer_height = (ripple_buffer_sample.yz - 0.5f) * 6.0f;
        ripple_buffer_foam = ripple_buffer_sample.w;
    }
    
    float slope_mag = min(max(0.3f, ripple_buffer_height.y + ripple_buffer_height.x + 1.0f), 2.1f);
    
    float3 normal;
    float2 refraction_s;
    float foam_height_const;
    float2 slope;
    calc_waveshape_ps(input.texcoord.xy, slope_mag, height_scale_aux, height_scale, ripple_buffer_height, input.texcoord.w, tangent, binormal, normal, refraction_s, foam_height_const, slope);
    
    float bankalpha = calc_bankalpha_ps(water_depth, input.base_tex.xy);
    
    float3 lightprobe_color = water_sample_lightprobe_array(input.lm_tex.xy, normal);
    
    float3 watercolor = calc_watercolor_ps(lightprobe_color, input.base_tex.xy);
    
//#if reach_compatibility_arg == k_reach_compatibility_enabled && refraction_arg == k_refraction_dynamic
//    float3 scene_color;
//    float3 color_refraction;
//    float refraction_amount;
//    calc_refraction_dynamic_reach_ps(input.position_ss, float4(position_ws, 1.0f), slope, bankalpha, input.extinction_factor.w, watercolor, color_refraction, scene_color, refraction_amount);
//    watercolor = color_refraction;
//#else
    float2 final_tex;
    float final_murkiness;
    calc_refraction_ps(input.position_ws__water_depth, refraction_s, ripple_buffer_height, input.incident_ws__view_dist, input.position_ss, slope_mag, final_tex, final_murkiness);

    float refraction_amount = final_murkiness;
    
    float3 scene_color = tex2D(scene_ldr_texture, final_tex).rgb;
    watercolor *= slope_mag;
    watercolor = final_murkiness * (scene_color - watercolor) + watercolor;
//#endif
    
    float3 sun_dir = float3(0.0f, 0.0f, 1.0f);
    float3 diffuse_color = lightprobe_color * (saturate(dot(sun_dir, normal)) * water_diffuse);
    
    float3 reflection = calc_reflection_ps(incident_ws, normal, lightprobe_color);
    
    if (!no_dynamic_lights)
    {        
		float3 diffuse_accumulation = 0;
		float3 specular_accumulation = 0;
        calc_material_lambert_diffuse_ps(normal, position_ws, 0, 20.0f, diffuse_accumulation, specular_accumulation);
        
        diffuse_color = diffuse_accumulation * water_diffuse + diffuse_color;
        reflection = specular_accumulation * 20.0f + reflection;
    }
    
    float3 fres_normal = normal;
#if APPLY_HLSL_FIXES == 1
    if (k_is_camera_underwater)
        fres_normal = -normal;
#endif
    float fresnel = calc_fresnel(incident_ws, fres_normal);
    refraction_amount *= 1.0f - fresnel;
    
    diffuse_color = diffuse_color + lerp(watercolor, reflection, fresnel);
    
    //float bankalpha = calc_bankalpha_ps(water_depth, input.base_tex.xy);
    refraction_amount = (1.0f - bankalpha) + refraction_amount * bankalpha;
    
    float3 blended_color = lerp(scene_color, diffuse_color, bankalpha);
    
    float4 foam = calc_foam_ps(lightprobe_color, input.texcoord.xy, input.base_tex, ripple_buffer_foam, view_dist, foam_height_const);
    refraction_amount *= (1.0f - foam.a);
    
    blended_color = foam.a * (foam.rgb - blended_color) + blended_color;
    
    float3 final_color = blended_color - scene_color * refraction_amount;
    final_color = final_color * input.extinction_factor.rgb + input.sky_radiance.rgb * (1.0f - refraction_amount);
    final_color = final_color * g_exposure.x;
    final_color = final_color + scene_color * refraction_amount;
    
    if (APPLY_HLSL_FIXES)
    {
        [branch]
        if (k_is_camera_underwater)
        {
            float fog = 0.5f * saturate(1.0f / exp(k_ps_underwater_murkiness * view_dist));
            final_color = lerp(k_ps_underwater_fog_color, final_color, fog);
        }
    }
    
    PS_OUTPUT_DEFAULT output;
    
    output.low_frequency.xyz = max(final_color, 0.0f);
    output.high_frequency.xyz = output.low_frequency.xyz / g_exposure.y;
    output.low_frequency.w = g_exposure.w;
    output.high_frequency.w = g_exposure.z;
    output.unknown = 0;
    return output;
}

#endif
