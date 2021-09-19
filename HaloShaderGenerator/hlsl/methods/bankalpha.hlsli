#ifndef _BANKALPHA_HLSLi
#define _BANKALPHA_HLSLi

uniform float bankalpha_infuence_depth;
#ifndef WATERCOLOR_TEXTURE
#define WATERCOLOR_TEXTURE
uniform sampler2D watercolor_texture;
#endif

float calc_bankalpha_none_ps(float bank, float2 texcoord)
{
    return 1.0f;
}

float calc_bankalpha_depth_ps(float bank, float2 texcoord)
{
    return saturate(bank / bankalpha_infuence_depth);
}

float calc_bankalpha_paint_ps(float bank, float2 texcoord)
{
    return saturate(tex2D(watercolor_texture, texcoord).a);
}

#ifndef calc_bankalpha_ps
#define calc_bankalpha_ps calc_bankalpha_none_ps
#endif

#endif