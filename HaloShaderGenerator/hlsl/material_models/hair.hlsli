#ifndef _material_model_hair_hlsli
#define _material_model_hair_hlsli

uniform float diffuse_coefficient;

float3 diffuse_tint;

float area_specular_coefficient;
float analytical_specular_coefficient;
float3 specular_tint;
float specular_power;
sampler specular_map;
sampler specular_shift_map;
sampler specular_noise_map;

float environment_map_coefficient;
float3 environment_map_tint;

float3 final_tint;

float analytical_anti_shadow_control;

#endif
