#ifndef _PIXL_SNIPER_SCOPE_HLSLI
#define _PIXL_SNIPER_SCOPE_HLSLI

#include "helpers\explicit_input_output.hlsli"
#include "registers\explicit_post_processing.hlsli"
#include "registers\global_parameters.hlsli"

#define _STENCIL_ENABLE 0

#if _STENCIL_ENABLE
uniform sampler2D stencil_sampler : register(s1);
#endif

float4 main(VS_OUTPUT_DEFAULT input) : COLOR
{
    float2 texcoord;
    
    // TODO: compile 1-1 (functionality is currently identical + h3 stuff)
    
    texcoord = input.texcoord;
    texcoord.y += p_lighting_constant_8.w;
    float sample_0 = tex2D(source_sampler, texcoord).x;
    sample_0 = p_lighting_constant_8.x / sample_0 + p_lighting_constant_8.y;
    
    texcoord = input.texcoord;
    float sample_1 = tex2D(source_sampler, texcoord).x;
    sample_1 = p_lighting_constant_8.x / sample_1 + p_lighting_constant_8.y;
    
    texcoord = input.texcoord;
    texcoord.x -= p_lighting_constant_8.z;
    texcoord.y += p_lighting_constant_8.w;
    float sample_2 = tex2D(source_sampler, texcoord).x;
    sample_2 = p_lighting_constant_8.x / sample_2 + p_lighting_constant_8.y;
    
    texcoord = input.texcoord;
    texcoord.x -= p_lighting_constant_8.z;
    float sample_3 = tex2D(source_sampler, texcoord).x;
    sample_3 = p_lighting_constant_8.x / sample_3 + p_lighting_constant_8.y;
    
    texcoord = input.texcoord;
    texcoord.y -= p_lighting_constant_8.w;
    float sample_4 = tex2D(source_sampler, texcoord).x;
    sample_4 = p_lighting_constant_8.x / sample_4 + p_lighting_constant_8.y;
    
    texcoord = input.texcoord;
    texcoord -= p_lighting_constant_8.zw;
    float sample_5 = tex2D(source_sampler, texcoord).x;
    sample_5 = p_lighting_constant_8.x / sample_5 + p_lighting_constant_8.y;
    
    texcoord = input.texcoord;
    texcoord.x += p_lighting_constant_8.z;
    texcoord.y -= p_lighting_constant_8.w;
    float sample_6 = tex2D(source_sampler, texcoord).x;
    sample_6 = p_lighting_constant_8.x / sample_6 + p_lighting_constant_8.y;
    
    texcoord = input.texcoord;
    texcoord.x += p_lighting_constant_8.z;
    float sample_7 = tex2D(source_sampler, texcoord).x;
    sample_7 = p_lighting_constant_8.x / sample_7 + p_lighting_constant_8.y;
    
    float4 r0;
    r0.zw = float2(sample_2, sample_0) - float2(sample_3, sample_1);
    r0.xy = float2(sample_3, sample_1) - float2(sample_5, sample_4);
    r0 *= r0;
    
    float4 r3;
    r3.xy = float2(sample_4, sample_6) - float2(sample_5, sample_4);
    r3.zw = float2(sample_1, sample_7) - float2(sample_3, sample_1);
    r3 *= r3;
    
    r0 += r3;
    r0 = sqrt(r0);
    r0 = saturate(r0);
    
    float4 _one = float4(1, 1, 1, 1);
    float final = dot(r0, _one);
    final *= scale.x;
    
    float green_channel = 0;
    
#if _STENCIL_ENABLE
    // NEW CODE (STENCILING FROM H3)
    // Final result will be a combination of H3 + HO :)
    texcoord = input.texcoord;
    float4 stencil_sample = tex2D(stencil_sampler, texcoord);
    green_channel = step(0.2509803921568627, stencil_sample.z);
    green_channel *= scale.x;
#endif
    
    return float4(final, green_channel, 0, 0);
}

#endif