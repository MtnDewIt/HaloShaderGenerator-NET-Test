#ifndef _BLEND_MODE_DECAL_HLSLI
#define _BLEND_MODE_DECAL_HLSLI

float4 blend_mode_opaque(float4 input, float fade)
{
    return input;
}

float4 blend_mode_additive(float4 input, float fade)
{
    return float4(input.rgb * fade, input.a);
}

float4 blend_mode_multiply(float4 input, float fade)
{
    return float4(fade * (input.rgb - 1.0) + 1.0, input.a);
}

float4 blend_mode_alpha_blend(float4 input, float fade)
{
    return float4(input.rgb, input.a * fade);
}

float4 blend_mode_double_multiply(float4 input, float fade)
{
    return float4(fade * (input.rgb - 0.5) + 0.5, input.a);
}

float4 blend_mode_maximum(float4 input, float fade)
{
    // TODO: verify
    return input;
}

float4 blend_mode_multiply_add(float4 input, float fade)
{
    // TODO: verify
    return input;
}

float4 blend_mode_add_src_times_dstalpha(float4 input, float fade)
{
    return float4(input.rgb, input.a * fade);
}

float4 blend_mode_add_src_times_srcalpha(float4 input, float fade)
{
    return float4(input.rgb, input.a * fade);
}

float4 blend_mode_inv_alpha_blend(float4 input, float fade)
{
    return float4(input.rgb, input.a * fade);
}

float4 blend_mode_pre_multiplied_alpha(float4 input, float fade)
{
    // premultiplication applied later
    return float4(input.rgb, input.a * fade);
}

#ifndef decal_blend_mode
#define decal_blend_mode blend_mode_opaque
#endif

#endif
