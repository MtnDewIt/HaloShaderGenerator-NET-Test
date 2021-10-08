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
#include "..\methods\watercolor.hlsli"
uniform float3 water_diffuse;
#include "..\methods\bankalpha.hlsli"
#include "..\registers\water_parameters.hlsli"

PS_OUTPUT_DEFAULT water_entry_static_per_pixel(VS_OUTPUT_WATER input)
{    
    float4 v0 = input.unknown_0;
    float2 texcoord = v0.xy;
    
    float4 v1 = input.unknown_2;
    float wave_slope_steepness = v1.w;
    
    float4 v2 = input.unknown_3;
    float aux_slope_steepness = v2.w;
    
    float4 v3 = input.unknown_4;
    float4 v4 = input.unknown_5;
    float4 v5 = input.unknown_6;
    float4 v6 = input.unknown_8;
    float4 v7 = input.unknown_9;
    float2 lightmap_tex = v7.xy;
    float2 ripple_buffer_tex = v7.zw;
    float3 v8 = input.unknown_10.xyz;
    float3 v9 = input.unknown_11.xyz;
    
    float2 ripple_buffer_height = 0.0f;
    float ripple_buffer_foam = 0.0f;
    if (k_is_water_interaction)
    {
        float4 ripple_buffer_sample = tex2Dlod(tex_ripple_buffer_slope_height, float4(ripple_buffer_tex, 0, 0));
        ripple_buffer_height = (ripple_buffer_sample.yz - 0.5f) * 6.0f;
        ripple_buffer_foam = ripple_buffer_sample.w;
    }
    
    float slope_mag = min(max(0.3f, ripple_buffer_height.y + ripple_buffer_height.x + 1.0f), 2.1f);
    
    float3 normal;
    float2 refraction_s;
    calc_waveshape_ps(texcoord, slope_mag, aux_slope_steepness, wave_slope_steepness, ripple_buffer_height, v0.w, v1.xyz, v2.xyz, normal, refraction_s);
    
    float3 lightprobe_color = water_sample_lightprobe_array(lightmap_tex);
    
    float3 watercolor = calc_watercolor_ps(lightprobe_color, v6.xy);
    
    float2 final_tex;
    float final_murkiness;
    calc_refraction_ps(v5.xyz, refraction_s, ripple_buffer_height, v4, v3, slope_mag, final_tex, final_murkiness);
    
    float3 scene_color = tex2D(scene_ldr_texture, final_tex).rgb;
    watercolor *= slope_mag;
    watercolor = final_murkiness * (scene_color - watercolor) + watercolor;
    
    // calc diffuse
    float3 diffuse_color = lightprobe_color * (saturate(normal.z) * water_diffuse);
    
    float3 reflection = calc_reflection_ps(v4.xyz, normal, lightprobe_color);
    
    if (!no_dynamic_lights)
    {        
		float3 diffuse_accumulation = 0;
		float3 specular_accumulation = 0;
        calc_material_lambert_diffuse_ps(normal, v5.xyz, 0, 20.0f, diffuse_accumulation, specular_accumulation);
        
        diffuse_color = diffuse_accumulation * water_diffuse + diffuse_color;
        reflection = specular_accumulation * 20.0f + reflection;
    }
    
    // todo: cleanup
    
    float fresnel = pow(1.0f - saturate(dot(v4.xyz, normal)), 2.5f);
    fresnel = lerp(1.0f, fresnel_coefficient, fresnel);
    
    diffuse_color = diffuse_color + lerp(watercolor, reflection, fresnel);
    
    float bankalpha = calc_bankalpha_ps(v5.w, v6.xy);
    
    float3 blended_color = lerp(scene_color, diffuse_color, bankalpha);
    
    float4 foam = calc_foam_ps(lightprobe_color, texcoord, v6.xy, v6.zw, ripple_buffer_foam);
    
    blended_color = foam.a * (foam.rgb - blended_color) + blended_color;
    
    float scale0 = (final_murkiness * (1.0f - fresnel)) * bankalpha + (1.0f - bankalpha);
    float scale1 = 1.0f - foam.a;
    float scale2 = scale0 * scale1;
    blended_color = scene_color * -scale2 + blended_color;
    float3 final_color = blended_color * v8.xyz + ((1.0f - (scale0 * scale1)) * v9.xyz);
    final_color = final_color * g_exposure.x + (scale2 * scene_color);
    
    if (APPLY_HLSL_FIXES)
    {
        [branch]
        if (k_is_camera_underwater)
        {
            float2 water_view_tex = (v3.xy / v3.w) * 0.5f + 0.5f;
            water_view_tex.y = 1.0f - water_view_tex.y;
            water_view_tex = water_view_tex * k_ps_water_player_view_constant.zw + k_ps_water_player_view_constant.xy;
            float water_view_depth = tex2D(depth_buffer, water_view_tex).x;
            water_view_depth = k_ps_water_view_depth_constant.x / water_view_depth + k_ps_water_view_depth_constant.y;
    
            float4 xform = float4((v3.xy / v3.w), 1.0f - water_view_depth, 1.0f);
            float4 water_view = mul(xform, k_ps_water_view_xform_inverse);
            
            water_view.xyz = water_view.xyz * -(1.0f / water_view.w) + k_ps_camera_position.xyz;
            float view_murkiness = rcp(rsqrt(dot(water_view.xyz, water_view.xyz))) * k_ps_underwater_murkiness;
            view_murkiness *= 1.44269502f;
            view_murkiness = saturate(1.0f / exp2(view_murkiness));
    
            view_murkiness = -view_murkiness + 1.0f;
            view_murkiness = -view_murkiness + 1.0f;
            view_murkiness *= 0.5f;
    
            final_color = lerp(k_ps_underwater_fog_color, final_color, view_murkiness);
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
