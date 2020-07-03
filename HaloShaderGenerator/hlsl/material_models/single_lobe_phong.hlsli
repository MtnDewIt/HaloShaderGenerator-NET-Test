#ifndef _SINGLE_LOBE_PHONG_HLSL
#define _SINGLE_LOBE_PHONG_HLSL

#include "../registers/shader.hlsli"
#include "../helpers/math.hlsli"

uniform float diffuse_coefficient;
uniform float specular_coefficient;
uniform float area_specular_contribution;
uniform float analytical_specular_contribution;
uniform float environment_map_specular_contribution;
uniform float roughness;
uniform float3 specular_tint;
uniform bool order3_area_specular;

void calc_material_analytic_specular_single_lobe_phong_ps(
in float3 reflect_dir,
in float3 light_dir,
in float3 light_intensity,
in float roughness,
out float3 analytic_specular)
{
	// Beckmann distribution for specular highlight (microfacet)
	float roughness_squared = roughness * roughness;
	float cos_alpha = dot(light_dir, reflect_dir);
	cos_alpha = max(cos_alpha, 0);

	float cos_alpha_squared = cos_alpha * cos_alpha;
	float sin_alpha_squared = 1 - cos_alpha_squared;
	float numerator = sin_alpha_squared / cos_alpha_squared;
	
	
	float roughness_part = (cos_alpha * roughness_squared * cos_alpha_squared) * 3.1415925; // should be cos_alpha_squared * cos_alpha_squared * roughness_squared
	
	
	numerator = - numerator / roughness_squared;
	numerator = exp(numerator);
	
	float specular_value = roughness_part * numerator; // should be a division according to wikipedia
	analytic_specular = specular_value * light_intensity;
}

void calc_material_area_specular_order_3_single_lobe_phong_ps(
in float3 reflect_dir,
in float4 sh_0,
in float4 sh_312[3],
in float4 sh_457[3],
in float4 sh_8866[3],
in float order_2_specular_scale,
in float3 band_1_reflect_color,
inout float3 area_specular)
{
	float order_3_specular_scale = -0.554101527 * roughness + 0.721252501;
	order_3_specular_scale += (roughness * roughness) * 0.0796054006;
	order_3_specular_scale *= -0.5;
	
	area_specular = sh_0.rgb * 0.282094985;
	area_specular += band_1_reflect_color.rgb * order_2_specular_scale;
	
	
	float3 test = reflect_dir.xyz * reflect_dir.yzx;
	float3 band_2_457_reflect_color = float3(dot(test, sh_457[0].xyz), dot(test, sh_457[1].xyz), dot(test, sh_457[2].xyz));
	area_specular += order_3_specular_scale * band_2_457_reflect_color.rgb;
	
	float4 test2 = float4(reflect_dir.xyz * reflect_dir.xyz, 1 / 3.0);
	float3 band_2_8866_reflect_color = float3(dot(test2, sh_8866[0]), dot(test2, sh_8866[1]), dot(test2, sh_8866[2]));
	area_specular += order_3_specular_scale * band_2_8866_reflect_color.rgb;
	
	area_specular = max(area_specular, 0.0f);
}

void calc_material_area_specular_order_2_single_lobe_phong_ps(
in float3 reflect_dir,
in float4 sh_0,
in float4 sh_312[3],
in float order_2_specular_scale,
in float3 band_1_reflect_color,
inout float3 area_specular)
{
	float4 sh_8866[3];
	float4 sh_457[3];
	sh_457[0] = 0;
	sh_457[1] = 0;
	sh_457[2] = 0;
	sh_8866[0] = 0;
	sh_8866[1] = 0;
	sh_8866[2] = 0;
	
	calc_material_area_specular_order_3_single_lobe_phong_ps(reflect_dir, sh_0, sh_312, sh_457, sh_8866, order_2_specular_scale, band_1_reflect_color, area_specular);
}

void calc_material_area_specular_single_lobe_phong_ps(
in float3 reflect_dir,
in float4 sh_0,
in float4 sh_312[3],
in float4 sh_457[3],
in float4 sh_8866[3],
in float3 specular_tint,
out float3 area_specular)
{
	float3 band_1_reflect_color = float3(dot(reflect_dir, sh_312[0].xyz), dot(reflect_dir, sh_312[1].xyz), dot(reflect_dir, sh_312[2].xyz));
	
	float order_2_specular_scale = roughness * -0.140736952 + 0.512894571;
	order_2_specular_scale += (roughness * roughness) * -0.00266006659;
	order_2_specular_scale *= -0.6;

	if (order3_area_specular)
	{
		calc_material_area_specular_order_3_single_lobe_phong_ps(reflect_dir, sh_0, sh_312, sh_457, sh_8866, order_2_specular_scale, band_1_reflect_color, area_specular);
	}
	else
	{
		calc_material_area_specular_order_2_single_lobe_phong_ps(reflect_dir, sh_0, sh_312, order_2_specular_scale, band_1_reflect_color, area_specular);
	}
}


#endif
