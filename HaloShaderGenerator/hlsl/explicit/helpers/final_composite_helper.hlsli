
uniform sampler2D surface_sampler : register(s0);
uniform sampler2D dark_surface_sampler : register(s1);
uniform sampler2D bloom_sampler : register(s2);
uniform sampler2D unknown3 : register(s3);
uniform sampler3D color_grading0 : register(s4);
uniform sampler3D color_grading1 : register(s5);
uniform sampler2D depth_sampler : register(s6);
uniform sampler2D blur_sampler : register(s7);

uniform float4 g_exposure : register(c0);
uniform float4 pixel_size : register(c1);
uniform float4 scale : register(c2);
uniform float4 tone_curve_constants : register(c3);
uniform float4 unknown_c4 : register(c4);
uniform float4 bloom_sampler_xform : register(c5);
uniform float4 cg_blend_factor : register(c6);
uniform float4 depth_constants : register(c7);
uniform float gamma_power : register(c8);
uniform float4x3 p_postprocess_hue_saturation_matrix : register(c218);
uniform float4 p_postprocess_contrast : register(c222);

#define HALO_ONLINE_COLOR_GRADING 1
#define GAMMA_FIX 1

#pragma warning( disable : 3571 34)

float get_dof_factor(float2 texcoord)
{
    float depth = tex2D(depth_sampler, texcoord).r;
    float dof_factor = abs(-depth_constants.z + -depth) - depth_constants.x;
    
    if (dof_factor < 0)
        dof_factor = 0;
    else
        dof_factor *= depth_constants.w;
    
    return pow(min(dof_factor, depth_constants.y), 2);
}

void apply_tone_and_huesat(inout float4 color)
{
    // apply contrast and hue-sat
    color.rgb = mul(color, p_postprocess_hue_saturation_matrix);
    float d_contrast = dot(color.rgb, 0.333000004f);
    d_contrast = pow(d_contrast, p_postprocess_contrast.x) / d_contrast;
    color.rgb *= d_contrast;
    
    // apply tone curve
    color.rgb = min(color.rgb, tone_curve_constants.x);
    float3 toned_color = color.rgb;
    toned_color = toned_color * tone_curve_constants.w + tone_curve_constants.z;
    toned_color = toned_color * color.rgb + tone_curve_constants.y;
    color.rgb = toned_color * color.rgb;
}

// Halo 3 x360 does not have this block of code... and the colour grading doesn't seem to do anything
// TODO: thoroughly examine the blend factor code, see whether this can be stripped out
void grade_color(inout float4 color)
{
    if (HALO_ONLINE_COLOR_GRADING == 1)
    {
        color.rgb = color.rgb * 0.9375f + 0.03125f;

        // grade the color (does nothing lol, "color_grading1" is all black)
        float3 graded_color = tex3D(color_grading0, color.rgb).rgb;
        float3 graded_dark_color = tex3D(color_grading1, color.rgb).rgb;

        float3 final_color = lerp(graded_color, graded_dark_color, cg_blend_factor.x);
        float alpha = 1.0f / rsqrt(dot(final_color, float3(0.298999995f, 0.587000012f, 0.114f)));
    
        color.rgb = final_color;
        color.a = alpha;
    }
}

// Gamma fixup to match x360
// The sqrt() belongs in lighting entries, however that's a lot of work to reimplement at this point in time.
void correct_gamma(inout float3 color)
{
    if (GAMMA_FIX == 1)
    {
        color.rgb = pow(sqrt(color.rgb), gamma_power);
    }
}
