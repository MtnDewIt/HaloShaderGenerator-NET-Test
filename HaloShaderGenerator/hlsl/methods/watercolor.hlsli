#ifndef _WATERCOLOR_HLSLi
#define _WATERCOLOR_HLSLi

uniform float3 water_color_pure;
uniform float watercolor_coefficient;
#ifndef WATERCOLOR_TEXTURE
#define WATERCOLOR_TEXTURE
uniform sampler2D watercolor_texture;
#endif

float3 calc_watercolor_pure_ps(float3 lightprobe_color, float2 texcoord)
{
    return water_color_pure * lightprobe_color;
}

float3 calc_watercolor_texture_ps(float3 lightprobe_color, float2 texcoord)
{
    return tex2D(watercolor_texture, texcoord).rgb * watercolor_coefficient * lightprobe_color;
}

#ifndef calc_watercolor_ps
#define calc_watercolor_ps calc_watercolor_pure_ps
#endif

#endif