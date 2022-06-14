#ifndef _ANTI_SHADOW_HLSLI
#define _ANTI_SHADOW_HLSLI

#include "..\helpers\input_output.hlsli"
#include "..\helpers\sh.hlsli"

void calc_analytical_specular_with_anti_shadow(
in SHADER_COMMON common_data,
in float analytic_anti_shadow_control,
inout float3 analytic_specular)
{
	get_sh_coefficients_no_dominant_light(common_data.dominant_light_direction, common_data.dominant_light_intensity, common_data.sh_0_no_dominant_light, common_data.sh_312_no_dominant_light);
	
	// this part of code determines if there is a shadow correction required when comparing with and without dominant light contribution, since green has the largest
	// contribution to visible light this approximation is faster than using the luminance

	// why is this required is still a mystery, it is probably related to the consequences of having a static dominant light instead of a real dynamic light
	
	float4 band_1_0_sh_green = float4(common_data.sh_312_no_dominant_light[1].xyz, common_data.sh_0_no_dominant_light.g);
	float sh_intensity_no_dominant_light = dot(band_1_0_sh_green, band_1_0_sh_green);
	float sh_intensity_dominant_light = 1.0 / (common_data.sh_0.g * common_data.sh_0.g + dot(common_data.sh_312[1].xyz, common_data.sh_312[1].xyz));
	float base = sh_intensity_no_dominant_light * sh_intensity_dominant_light - 1.0 < 0 ? (1 - sh_intensity_dominant_light * sh_intensity_no_dominant_light) : 0;
	analytic_specular *= pow(base, 100 * analytic_anti_shadow_control);
}

#endif
