#ifndef _TINTING_HLSLi
#define _TINTING_HLSLi

#define k_decal_tinting_none 0
#define k_decal_tinting_unmodulated 1
#define k_decal_tinting_partially_modulated 2
#define k_decal_tinting_fully_modulated 3

uniform float4 tint_color;
uniform float intensity;
uniform float modulation_factor;

void tinting_none(inout float4 color)
{
}

void tinting_unmodulated(inout float4 color)
{
    float3 tint = tint_color.rgb * intensity;
    
    color.rgb *= tint;
}

void tinting_partially_modulated(inout float4 color)
{
    float _var = sqrt(dot(color.rgb, color.rgb)) * modulation_factor * 0.577350259f;
    
    float3 tint = (tint_color.rgb + _var * (1.0f - tint_color.rgb)) * intensity;

    color.rgb *= tint;
}

void tinting_fully_modulated(inout float4 color)
{
    float _var = sqrt(dot(color.rgb, color.rgb)) * 0.577350259f;
    
    float3 tint = (tint_color.rgb + _var * (1.0f - tint_color.rgb)) * intensity;

    color.rgb *= tint;
}

#ifndef decal_tinting
#define decal_tinting tinting_none
#endif

#endif