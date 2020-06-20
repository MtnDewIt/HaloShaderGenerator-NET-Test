#ifndef _NO_LIGHTING_HLSLI
#define _NO_LIGHTING_HLSLI

#include "..\material_models\material_shared_parameters.hlsli"
#include "..\helpers\input_output.hlsli"

void calc_dynamic_lighting_none_ps(float3 light_dir, float3 view_dir, float3 reflect_dir, float3 surface_normal, float2 texcoord, float3 light_intensity, float3 albedo, out float3 color)
{
	float v_dot_n = dot(light_dir, surface_normal);
	color = light_intensity * v_dot_n * albedo.rgb; // lambertian diffuse
}

float3 calc_lighting_none_ps(SHADER_COMMON common_data)
{
	return 0;
}
#endif