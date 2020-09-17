#ifndef _SHADOW_HLSLi
#define _SHADOW_HLSLi

float4 k_ps_constant_shadow_alpha : register(c11);
sampler2D zbuffer : register(s0);
sampler2D shadow : register(s1);
sampler2D normal_buffer : register(s2);

#endif