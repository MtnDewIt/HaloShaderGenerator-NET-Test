#ifndef _CHUD_HELPER_HLSLI
#define _CHUD_HELPER_HLSLI

#include "chud_global_parameters.hlsli"

void export_chud_alpha(inout float alpha)
{
    alpha *= g_hud_alpha;
    
    if (!chud_cortana_pixel)
    {
        alpha *= g_exposure.w;
    }
}

// interpolate between two colors (0 color1, 1.0 color2)
float3 apply_color_selector(float3 color1, float3 color2, float scalar)
{
    return color1 * (1.0f - scalar) + color2 * scalar;
}

#endif
