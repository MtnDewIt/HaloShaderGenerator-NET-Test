#ifndef _SHADER_TEMPLATE_SFX_DISTORT
#define _SHADER_TEMPLATE_SFX_DISTORT

#include "..\registers\global_parameters.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\definition_helper.hlsli"

uniform sampler2D distort_map;
uniform float4 distort_map_xform;
uniform float distort_scale;

float2 sample_sfx_distort_map(float2 texcoord)
{
    float4 distort_map_sample = tex2D(distort_map, apply_xform2d(texcoord, distort_map_xform));
    return distort_map_sample.yw * 2.00787401f - 1.00787401f;
}

float2 get_sfx_distortion(float2 texcoord)
{
    return sample_sfx_distort_map(texcoord) * distort_scale;
}

void apply_sfx_distortion(inout float2 texcoord)
{
    if (distortion_arg == k_distortion_on)
    {
        texcoord += get_sfx_distortion(texcoord);
    }
}

PS_OUTPUT_DEFAULT shader_entry_sfx_distort(VS_OUTPUT_SFX_DISTORT input)
{
    PS_OUTPUT_DEFAULT output;
    
    if (distortion_arg == k_distortion_on)
    {        
        float2 distort_sample = sample_sfx_distort_map(input.texcoord.xy);
        
        float2 sfx_distort = distort_sample * distort_scale * input.distortion;
        
        output.low_frequency.xy = sfx_distort * 0.5f + 0.50195998f;
        output.low_frequency.z = 1;
        output.low_frequency.w = saturate(dot(abs(distort_sample * 10.0f), 1.0f)) * input.texcoord.w;
    }
    else
    {
        output.low_frequency = 0;
    }
    
    output.high_frequency = 0;
    output.unknown = 0;
	
    return output;
}
#endif