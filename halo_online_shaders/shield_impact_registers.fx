/*
SHIELD_IMPACT_REGISTERS.FX
Copyright (c) Microsoft Corporation, 2007. all rights reserved.
4/05/2007 9:15:00 AM (kuttas)
	
*/

#if DX_VERSION == 9

#include "shield_impact_registers.h"

// ensure that these don't conflict with oneshot/persist registers

VERTEX_CONSTANT(float, extrusion_distance, k_vs_shield_impact_extrusion_distance)

// xyz = center, w = radius
PIXEL_CONSTANT(float4, bound_sphere, k_ps_shield_impact_bound_sphere)

// x = time, y = shield damage, z = overshield amount (= shield_vitality - 1.0f)
PIXEL_CONSTANT(float4, shield_dynamic_quantities, k_ps_shield_impact_shield_dynamic_quantities)

// x = UV scale factor, y = scroll speed modifier
PIXEL_CONSTANT(float4, texture_quantities, k_ps_shield_impact_texture_quantities)

// x = sharpness, y = scale, z = threshold
PIXEL_CONSTANT(float4, plasma1_settings, k_ps_shield_impact_plasma1_settings)
PIXEL_CONSTANT(float4, plasma2_settings, k_ps_shield_impact_plasma2_settings)

// xyz = color, w = intensity
PIXEL_CONSTANT(float3, overshield_color1, k_ps_shield_impact_overshield_color1)
PIXEL_CONSTANT(float3, overshield_color2, k_ps_shield_impact_overshield_color2)
PIXEL_CONSTANT(float3, overshield_ambient_color, k_ps_shield_impact_overshield_ambient_color)
PIXEL_CONSTANT(float3, shield_impact_color1, k_ps_shield_impact_color1)
PIXEL_CONSTANT(float3, shield_impact_color2, k_ps_shield_impact_color2)
PIXEL_CONSTANT(float3, shield_impact_ambient_color, k_ps_shield_impact_ambient_color)

#elif DX_VERSION == 11

CBUFFER_BEGIN(ShieldImpactVS)
	CBUFFER_CONST(ShieldImpactVS,	float,		extrusion_distance,					k_vs_shield_impact_extrusion_distance)
CBUFFER_END

CBUFFER_BEGIN(ShieldImpactPS)
	CBUFFER_CONST(ShieldImpactPS,	float4,		bound_sphere,						k_ps_shield_impact_bound_sphere)
	CBUFFER_CONST(ShieldImpactPS,	float4,		shield_dynamic_quantities,			k_ps_shield_impact_shield_dynamic_quantities)
	CBUFFER_CONST(ShieldImpactPS,	float4,		texture_quantities,					k_ps_shield_impact_texture_quantities)
	CBUFFER_CONST(ShieldImpactPS,	float4,		plasma1_settings,					k_ps_shield_impact_plasma1_settings)
	CBUFFER_CONST(ShieldImpactPS,	float4,		plasma2_settings,					k_ps_shield_impact_plasma2_settings)
	CBUFFER_CONST(ShieldImpactPS,	float3,		overshield_color1,					k_ps_shield_impact_overshield_color1)
	CBUFFER_CONST(ShieldImpactPS,	float,		overshield_color1_pad,				k_ps_shield_impact_overshield_color1_pad)
	CBUFFER_CONST(ShieldImpactPS,	float3,		overshield_color2,					k_ps_shield_impact_overshield_color2)
	CBUFFER_CONST(ShieldImpactPS,	float,		overshield_color2_pad,				k_ps_shield_impact_overshield_color2_pad)
	CBUFFER_CONST(ShieldImpactPS,	float3,		overshield_ambient_color,			k_ps_shield_impact_overshield_ambient_color)
	CBUFFER_CONST(ShieldImpactPS,	float,		overshield_ambient_color_pad,		k_ps_shield_impact_overshield_ambient_color_pad)
	CBUFFER_CONST(ShieldImpactPS,	float3,		shield_impact_color1,				k_ps_shield_impact_color1)
	CBUFFER_CONST(ShieldImpactPS,	float,		shield_impact_color1_pad,			k_ps_shield_impact_color1_pad)
	CBUFFER_CONST(ShieldImpactPS,	float3,		shield_impact_color2,				k_ps_shield_impact_color2)
	CBUFFER_CONST(ShieldImpactPS,	float,		shield_impact_color2_pad,			k_ps_shield_impact_color2_pad)
	CBUFFER_CONST(ShieldImpactPS,	float3,		shield_impact_ambient_color,		k_ps_shield_impact_ambient_color)
	CBUFFER_CONST(ShieldImpactPS,	float,		shield_impact_ambient_color_pad,	k_ps_shield_impact_ambient_color_pad)
CBUFFER_END

#endif
