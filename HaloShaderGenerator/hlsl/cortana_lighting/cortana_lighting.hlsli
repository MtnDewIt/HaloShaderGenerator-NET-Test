#define CORTANA_LIGHTING_HLSLi

#include "..\helpers\input_output.hlsli"
#include "..\shader_lighting\cook_torrance_lighting.hlsli"

float3 calc_cortana_cook_torrance_lighting(SHADER_COMMON common_data, out float4 ssr_out)
{
    float3 color = calc_lighting_cook_torrance_ps(common_data, ssr_out);

    color.rgb = (color.rgb * common_data.extinction_factor + common_data.sky_radiance) * g_alt_exposure.y * 2.0f;
    color.rgb *= common_data.extinction_factor;
    color.rgb += common_data.sky_radiance;
    color.rgb *= g_alt_exposure.y; // illuminate
    color.rgb *= 2.0f;
    
    return color;
}
