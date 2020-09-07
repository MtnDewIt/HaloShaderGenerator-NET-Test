#ifndef _BLEND_MODE_DECAL_HLSLI
#define _BLEND_MODE_DECAL_HLSLI

#define k_decal_blend_mode_opaque 0
#define k_decal_blend_mode_additive 1
#define k_decal_blend_mode_multiply 2
#define k_decal_blend_mode_alpha_blend 3
#define k_decal_blend_mode_double_multiply 4
#define k_decal_blend_mode_maximum 5
#define k_decal_blend_mode_multiply_add 6
#define k_decal_blend_mode_add_src_times_dstalpha 7
#define k_decal_blend_mode_add_src_times_srcalpha 8
#define k_decal_blend_mode_inv_alpha_blend 9
#define k_decal_blend_mode_pre_multiplied_alpha 10

float4 blend_mode_opaque(float4 input, float fade)
{
    return input;
}

float4 blend_mode_additive(float4 input, float fade)
{
    return input;
}

float4 blend_mode_multiply(float4 input, float fade)
{
    return float4(fade * (input.rgb - 1.0) + 1.0, input.a);
}

float4 blend_mode_alpha_blend(float4 input, float fade)
{
    float alpha = input.a * fade;
    return float4(input.rgb, alpha);
}

float4 blend_mode_double_multiply(float4 input, float fade)
{
    float3 color = fade * (input.rgb - 0.5) + 0.5;
    float alpha = input.a * fade;
    return float4(color, alpha);
}

float4 blend_mode_pre_multiplied_alpha(float4 input, float fade)
{
    float alpha = input.a * fade;
    return float4(input.rgb * alpha, alpha);
}

float4 blend_mode_add_src_times_srcalpha(float4 input, float fade)
{
    input.a *= fade;
    return input;
}
float4 blend_mode_add_src_times_dstalpha(float4 input, float fade)
{
    input.a *= fade;
    return input;
}

#ifndef decal_blend_mode
#define decal_blend_mode blend_mode_opaque
#endif

#endif
