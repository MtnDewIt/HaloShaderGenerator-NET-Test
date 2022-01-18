#ifndef _HLSLI_WATER_WAVESHAPE
#define _HLSLI_WATER_WAVESHAPE

uniform sampler3D wave_slope_array;

#include "../helpers/math.hlsli"
#include "../helpers/definition_helper.hlsli"
#include "../helpers/bumpmap_math.hlsli"

#include "../registers/water_parameters.hlsli"
#include "../registers/global_parameters.hlsli"

uniform float4 wave_displacement_array_xform;
uniform float time_warp;
uniform float4 wave_slope_array_xform;
uniform float time_warp_aux;

// bleh register order
#include "foam.hlsli"

uniform float slope_range_x;
uniform float slope_range_y;
uniform float detail_slope_scale_x;
uniform float detail_slope_scale_y;
uniform float detail_slope_scale_z;
uniform float detail_slope_steepness;
uniform float slope_scaler; // reach

uniform sampler bump_map;
uniform xform2d bump_map_xform;
uniform sampler bump_detail_map;
uniform xform2d bump_detail_map_xform;

#if refraction_arg == k_refraction_dynamic
#include "refraction.hlsli"
#endif

void calc_waveshape_default_ps(
    float2 texcoord,
    float slope_mag,
    float aux_slope_steepness,
    float wave_slope_steepness,
    float2 ripple_height,
    float n_scale,
    float3 v1,
    float3 v2,
    out float3 normal,
    out float2 refraction_s,
    out float foam_height_const)
{
    float2 slope_range_xy = float2(slope_range_x, slope_range_y);
    float2 detail_slope_scale = float2(detail_slope_scale_x, detail_slope_scale_y);
    
    // detail
    float3 detail_slope_tex = float3(apply_xform2d(texcoord * detail_slope_scale, wave_displacement_array_xform), time_warp * detail_slope_scale_z);
    float2 detail_slope = tex3D(wave_slope_array, detail_slope_tex).xy * 2.00787401f - 1.00787401f;
#if reach_compatibility_arg != k_reach_compatibility_enabled
    detail_slope = detail_slope * slope_range_xy + (slope_range_xy * -0.5f);
#endif
    
    float2 restored_detail_slope = detail_slope * (detail_slope_steepness / slope_mag);
    
    // aux
    float3 aux_slope_tex = float3(apply_xform2d(texcoord, wave_slope_array_xform), time_warp_aux);
    float2 aux_slope = tex3D(wave_slope_array, aux_slope_tex).xy * 2.00787401f - 1.00787401f;
#if reach_compatibility_arg != k_reach_compatibility_enabled
    aux_slope = aux_slope * slope_range_xy + (slope_range_xy * -0.5f);
#endif
    
    float2 restored_aux_slope = aux_slope * (aux_slope_steepness / slope_mag);
    
    // wave
    float3 wave_slope_tex = float3(apply_xform2d(texcoord, wave_displacement_array_xform), time_warp);
    float2 wave_slope = tex3D(wave_slope_array, wave_slope_tex).xy * 2.00787401f - 1.00787401f;
    
#if reach_compatibility_arg != k_reach_compatibility_enabled
    refraction_s = wave_slope * slope_range_xy + (slope_range_xy * -0.5f);
#else
    refraction_s = wave_slope;
#endif
    float2 restored_wave_slope = refraction_s * (wave_slope_steepness / slope_mag);
    
#if refraction_arg == k_refraction_dynamic
        refraction_s = refraction_s * max(wave_slope_steepness / slope_mag, minimal_wave_disturbance) + restored_aux_slope;
#endif
    
    float2 slope_accum = restored_wave_slope + restored_aux_slope + restored_detail_slope;
    float slope_scale = n_scale >= 1.0f ? 1.0f / n_scale : 1.0f;
    slope_accum = slope_accum * slope_scale;

#if reach_compatibility_arg == k_reach_compatibility_enabled
    slope_accum *= slope_scaler;
#endif
    slope_accum += ripple_height;

    float3 n_slope = normalize(float3(slope_accum, 1.0f));
    
    float3 n_m_mul = n_slope.y * v2;
    n_m_mul = n_slope.x * v1 + n_m_mul;
    n_m_mul = n_slope.z * -normalize((v2.yzx * v1.zxy) - (v2.zxy * v1.yzx)) + n_m_mul;
    normal = normalize(n_m_mul);

    float2 slope_diff = (1.0f - abs(float2(wave_slope.x, aux_slope.x))) - abs(float2(wave_slope.y, aux_slope.y));
    foam_height_const = max(slope_diff.x, slope_diff.y);
}

void calc_waveshape_none_ps(
    float2 texcoord,
    float slope_mag,
    float aux_slope_steepness,
    float wave_slope_steepness,
    float2 ripple_height,
    float n_scale,
    float3 v1,
    float3 v2,
    out float3 normal,
    out float2 refraction_s,
    out float foam_height_const)
{
    refraction_s = 0.0f;
    foam_height_const = 0.0f;
    float3 n_slope = normalize(float3(0.0f, 0.0f, 1.0f));
    
    float3 n_m_mul = n_slope.y * v2;
    n_m_mul = n_slope.x * v1 + n_m_mul;
    n_m_mul = n_slope.z * -normalize((v2.yzx * v1.zxy) - (v2.zxy * v1.yzx)) + n_m_mul;
    normal = normalize(n_m_mul);
}

void calc_waveshape_bump_ps(
    float2 texcoord,
    float slope_mag,
    float aux_slope_steepness,
    float wave_slope_steepness,
    float2 ripple_height,
    float n_scale,
    float3 v1,
    float3 v2,
    out float3 normal,
    out float2 refraction_s,
    out float foam_height_const)
{
    float3 bump_map_sample = sample_normal_2d(bump_map, apply_xform2d(texcoord, bump_map_xform));
    float3 bump = normalize(bump_map_sample);
    float3 bump_detail_map_sample = sample_normal_2d(bump_detail_map, apply_xform2d(texcoord, bump_detail_map_xform));
	
    float scalar = 1.0f;
#if reach_compatibility_arg == k_reach_compatibility_enabled
    scalar = slope_scaler;
#endif

    bump.xy += normalize(bump_detail_map_sample).xy;
    bump.xy /= max(bump.z, 0.01f) * scalar;
    
    refraction_s = bump.xy;
    float3 n_slope = normalize(float3(bump.xy, 1.0f));
    
    float3 n_m_mul = n_slope.y * v2;
    n_m_mul = n_slope.x * v1 + n_m_mul;
    n_m_mul = n_slope.z * -normalize((v2.yzx * v1.zxy) - (v2.zxy * v1.yzx)) + n_m_mul;
    normal = normalize(n_m_mul);

    foam_height_const = 0.0f;
}

#ifndef calc_waveshape_ps
#define calc_waveshape_ps calc_waveshape_default_ps
#endif

#endif
