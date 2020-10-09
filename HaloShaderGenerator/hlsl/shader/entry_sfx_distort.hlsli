#ifndef _SHADER_TEMPLATE_SFX_DISTORT
#define _SHADER_TEMPLATE_SFX_DISTORT

#include "..\registers\global_parameters.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"

uniform sampler2D distort_map;
uniform float4 distort_map_xform;
uniform float distort_scale;

PS_OUTPUT_DEFAULT shader_entry_sfx_distort(VS_OUTPUT_SFX_DISTORT input)
{
    PS_OUTPUT_DEFAULT output;
    
    if (distortion_arg == k_distortion_on)
    {
        float2 distort_map_texcoord = apply_xform2d(input.texcoord.xy, distort_map_xform);
        float4 distort_map_sample = tex2D(distort_map, distort_map_texcoord);
        
        float2 distortion = distort_map_sample.yw * 2.00787401f - 1.00787401f;
        float2 distortion2 = distortion * 10;
        
        distortion *= distort_scale;
        distortion *= input.distortion;
        
        output.low_frequency.xy = distortion * 0.5f + 0.50195998f;
        output.low_frequency.z = 1;
        output.low_frequency.w = saturate(dot(abs(distortion2), 1.0f)) * input.texcoord.w;
        
        output.high_frequency = 0;
        output.unknown = 0;
    }
    else
    {
        output.low_frequency = 0;
        output.high_frequency = 0;
        output.unknown = 0;
    }
	
    return output;
}
#endif