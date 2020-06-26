#ifndef _GLASS_HLSL
#define _GLASS_HLSL

#include "..\material_models\material_shared_parameters.hlsli"
#include "..\helpers\math.hlsli"

void calc_material_analytic_specular_glass_ps(
in float3 reflect_dir,
in float3 light_dir,
in float3 light_intensity,
in float roughness,
out float3 analytic_specular)
{
    // Beckmann distribution for specular highlight (microfacet)
    float cos_alpha = dot(light_dir, reflect_dir);
    cos_alpha = max(cos_alpha, 0);
    
    float roughness_squared = roughness * roughness;
    
    float cos_alpha_squared = cos_alpha * cos_alpha;
    float sin_alpha_squared = 1 - cos_alpha_squared;

    float roughness_part = cos_alpha_squared * cos_alpha * roughness_squared * 3.1415925; // should be cos_alpha_squared * cos_alpha_squared * roughness_squared
    
    float numerator = -sin_alpha_squared / cos_alpha_squared;
    numerator /= roughness_squared;
    numerator = exp(numerator);
    
    float specular_value = numerator * roughness_part; // should be a division according to wikipedia
    analytic_specular = light_intensity * specular_value;
}

void calc_material_area_specular_order_3_glass_ps(
in float3 reflect_dir,
in float4 sh_0,
in float4 sh_312[3],
in float4 sh_457[3],
in float4 sh_8866[3],
in float3 band_1_reflect_color,
in float order_2_specular_scale,
out float3 area_specular
)
{
    //order3?
    
    area_specular = sh_0.rgb * 0.282094985;
    area_specular += band_1_reflect_color.rgb * order_2_specular_scale;
    
    // ???
    //area_specular = max(area_specular, 0.0f);
}

void calc_material_area_specular_order_2_glass_ps(
in float3 reflect_dir,
in float4 sh_0,
in float4 sh_312[3],
in float3 band_1_reflect_color,
in float order_2_specular_scale,
out float3 area_specular
)
{
    float4 sh_8866[3];
    float4 sh_457[3];
    sh_457[0] = 0;
    sh_457[1] = 0;
    sh_457[2] = 0;
    sh_8866[0] = 0;
    sh_8866[1] = 0;
    sh_8866[2] = 0;
    
    calc_material_area_specular_order_3_glass_ps(reflect_dir, sh_0, sh_312, sh_457, sh_8866, band_1_reflect_color, order_2_specular_scale, area_specular);
}


void calc_material_area_specular_glass_ps(
in float3 reflect_dir,
in float4 sh_0,
in float4 sh_312[3],
in float c_roughness,
out float3 area_specular
)
{
    float3 band_1_reflect_color = float3(dot(reflect_dir, sh_312[0].xyz), dot(reflect_dir, sh_312[1].xyz), dot(reflect_dir, sh_312[2].xyz));
    
    float order_2_specular_scale = c_roughness * -0.140736952 + 0.512894571;
    order_2_specular_scale += (c_roughness * c_roughness) * -0.00266006659;
    order_2_specular_scale *= -0.6;
    
    // TODO: double check here, order3 mayb?

    calc_material_area_specular_order_2_glass_ps(reflect_dir, sh_0, sh_312, band_1_reflect_color, order_2_specular_scale, area_specular);
}


#endif