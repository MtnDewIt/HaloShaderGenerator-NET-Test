#ifndef _REFRACTION_HLSLi
#define _REFRACTION_HLSLi

#include "..\helpers\apply_hlsl_fixes.hlsli"
#include "..\registers\water_parameters.hlsli"
#include "..\registers\global_parameters.hlsli"

uniform float refraction_texcoord_shift;
uniform float refraction_extinct_distance;
uniform float minimal_wave_disturbance;
uniform float refraction_depth_dominant_ratio;
uniform float water_murkiness;

void calc_refraction_none_ps(float3 vpos, float2 refraction_tex, float2 ripple_buffer_height, float4 v4, float4 v3, float slope_mag, out float2 out_texcoord, out float final_murkiness)
{
    out_texcoord = vpos.xy / texture_size; // todo: actual
    final_murkiness = 0.0f;
}

void calc_refraction_dynamic_ps(float3 vpos, float2 refraction_tex, float2 ripple_buffer_height, float4 v4, float4 v3, float slope_mag, out float2 out_texcoord, out float final_murkiness)
{
    refraction_tex = ripple_buffer_height + refraction_tex;
    refraction_tex = refraction_tex * v4.yx;
    refraction_tex = refraction_tex * refraction_texcoord_shift;
    
    float2 water_view_tex = (v3.xy / v3.w) * 0.5f + 0.5f;
    water_view_tex.y = 1.0f - water_view_tex.y;
    water_view_tex = water_view_tex * k_ps_water_player_view_constant.zw + k_ps_water_player_view_constant.xy;
    float water_view_depth = tex2D(depth_buffer, water_view_tex).x;
    water_view_depth = k_ps_water_view_depth_constant.x * rcp(water_view_depth) + k_ps_water_view_depth_constant.y;
    
    float4 xform = float4((v3.xy / v3.w), 1.0f - water_view_depth, 1.0f);
    float4 water_view = mul(xform, k_ps_water_view_xform_inverse);
    water_view.xyz = water_view.xyz / water_view.w - vpos;
    
    refraction_tex *= saturate(rcp(rsqrt(dot(water_view.xyz, water_view.xyz))) * 3.0f);
    refraction_tex = saturate(rcp(v4.w) + rcp(v4.w)) * refraction_tex;
    refraction_tex = refraction_tex * k_ps_water_player_view_constant.zw;
    refraction_tex = refraction_tex * slope_mag + water_view_tex;
    float2 final_refract_tex = min(k_ps_water_player_view_constant.zw + k_ps_water_player_view_constant.xy - 0.001f, max(refraction_tex, 0.001f + k_ps_water_player_view_constant.xy));
    
    float refract_depth = tex2D(depth_buffer, final_refract_tex).x;
    refract_depth = k_ps_water_view_depth_constant.x * rcp(refract_depth) + k_ps_water_view_depth_constant.y;
    
    out_texcoord = (v3.z / v3.w - refract_depth) >= 0 ? water_view_tex : final_refract_tex;
    
    float final_depth = tex2D(depth_buffer, out_texcoord).x;
    final_depth = k_ps_water_view_depth_constant.x * rcp(final_depth) + k_ps_water_view_depth_constant.y;
    
    float4 final_view = mul(float4(float2(out_texcoord.x, 1.0f - out_texcoord.y) * 2.0f - 1.0f, 1.0f - final_depth, 1.0f), k_ps_water_view_xform_inverse);
    final_view.xyz = final_view.xyz / final_view.w - vpos;
    
    float depth_mag = lerp(abs(final_view.z), rcp(rsqrt(dot(final_view.xyz, final_view.xyz))), refraction_depth_dominant_ratio);
    float murkiness = depth_mag * water_murkiness.x;
    murkiness = murkiness * 1.44269502f;
    murkiness = exp2(murkiness);
    murkiness = saturate(rcp(murkiness));
    murkiness = 1.0f - murkiness;
    murkiness = 1.0f - (murkiness * slope_mag);
    float murkiness_cut = saturate(1.0f - (v4.w / refraction_extinct_distance));
    murkiness_cut = murkiness_cut * murkiness;
    
    final_murkiness = murkiness >= 0 ? murkiness_cut : 0.0f;
}

#ifndef calc_refraction_ps
#define calc_refraction_ps calc_refraction_none_ps
#endif

#endif