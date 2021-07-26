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

#endif
