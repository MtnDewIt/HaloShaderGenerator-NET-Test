#ifndef _PARTICLE_HLSLI
#define _PARTICLE_HLSLI

#include "..\methods\albedo.hlsli"
#include "..\methods\fog.hlsli"

#include "../helpers/types.hlsli"
#include "../helpers/math.hlsli"
#include "../helpers/color_processing.hlsli"

PS_OUTPUT_PARTICLE particle_entry_default(VS_OUTPUT_PARTICLE input)
{    
    float4 albedo = particle_albedo(input.texcoord.xy, input.color, input.o2.rgb);
    
    PS_OUTPUT_PARTICLE output;
    
    output.diffuse.a = albedo.a * g_exposure.w;
    output.diffuse.rgb = albedo.rgb;
    
    // not sure what this is from yet, doesnt occur in all shaders
    output.oC1.a = albedo.a * g_exposure.z;
    float inv_exposure_y = (1 / g_exposure.y);
    output.oC1.rgb = albedo.rgb * inv_exposure_y;
    
    output.oC2 = 0;
    return output;
}

#endif