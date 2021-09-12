#ifndef _PIXL_VISION_MODE_HLSLI
#define _PIXL_VISION_MODE_HLSLI

#include "..\helpers\explicit_input_output.hlsli"

// xps_3_0 instructions
float sgt(in float src1, in float src2) {
    return src1 > src2 ? 1.0f : 0.0f;
}
float2 sgt2(in float2 src1, in float2 src2) {
    return float2(src1.x > src2.x ? 1.0f : 0.0f, src1.y > src2.y ? 1.0f : 0.0f);
}
float cndgt(in float src1, in float src2, in float src3) {
    return (src1.x > 0.0f) ? src2.x : src3.x;
}
float2 sge12(in float2 src1, in float src2) {
    return float2((src1.x >= src2.x) ? 1.0f : 0.0f, (src1.y >= src2.x) ? 1.0f : 0.0f);
}

// 0 for ms23 shader
#define ODST_SHADER 0

uniform float4 g_exposure : register(c0);
uniform float falloff : register(c94);
uniform float4x4 screen_to_world : register(c95);
uniform float4 ping : register(c99);
uniform float4 colors[14] : register(c100); // extra 4 in HO for variant colours
uniform float overlapping_dimming_factor[5] : register(c114);
uniform float4 falloff_throught_the_wall : register(c120);
uniform float4 zbuf_params : register(c121);
uniform float float_index : register(c122); // use this instead of color_sampler

uniform sampler2D depth_sampler : register(s0);
uniform sampler2D color_sampler : register(s1);
uniform sampler2D mask_sampler : register(s2);
uniform sampler2D depth_through_walls_sampler : register(s3);

#if ODST_SHADER == 1
float4 ps_default(VS_OUTPUT_DEFAULT input) : COLOR
{
    float2 texcoord = input.texcoord;
    float4 final_color = sgt(-abs(texcoord.x), 0.0f);
    
    float mask = tex2D(mask_sampler, texcoord).r;
    if (mask > 0)
    {
        // not setup in HO
        //float color_index = floor(tex2D(color_sampler, texcoord).z);
        float color_index = float_index / 4.0f;
        
        float depth = 1.0f - (zbuf_params.x / tex2D(depth_sampler, texcoord).r + zbuf_params.y);
        
        //float4 stw_2 = screen_to_world[2];
        //stw_2.w = -stw_2.w;
        
        //float4 transform =  screen_to_world[0] * texcoord.x;
        //transform +=        screen_to_world[1] * texcoord.y;
        //transform +=        screen_to_world[2] * depth;
        //transform +=        screen_to_world[3];
        float4 transform = mul(float4(texcoord.xy, depth, 1.0f), transpose(screen_to_world));
        transform.xyz /=    transform.w;
        
        float d_screen_to_world_t = sqrt(abs(dot(transform.xyz, transform.xyz)));
        
        float2 ping_falloff;
        ping_falloff.x =    pow(abs(d_screen_to_world_t * falloff), 2);
        ping_falloff.y =    ping.x * d_screen_to_world_t;
        ping_falloff.y =    saturate(ping_falloff.y * ping.z + 1.0f);
        ping_falloff =      pow(ping_falloff, 3);
        ping_falloff.y =    exp2(-ping_falloff.y) * mask;
        ping_falloff *=     sge12(ping.xy, d_screen_to_world_t);
        
        float4 depth_bounds;
        depth_bounds.x = 1.0f - (zbuf_params.x / tex2D(depth_sampler, texcoord + (float2( 1,  0) * zbuf_params.zw)).r + zbuf_params.y);// / 10240.0f; // z
        depth_bounds.y = 1.0f - (zbuf_params.x / tex2D(depth_sampler, texcoord + (float2( 0,  1) * zbuf_params.zw)).r + zbuf_params.y);// / 10240.0f; // w
        depth_bounds.z = 1.0f - (zbuf_params.x / tex2D(depth_sampler, texcoord + (float2(-1,  0) * zbuf_params.zw)).r + zbuf_params.y);// / 10240.0f; // x
        depth_bounds.w = 1.0f - (zbuf_params.x / tex2D(depth_sampler, texcoord + (float2( 0, -1) * zbuf_params.zw)).r + zbuf_params.y);// / 10240.0f; // y
        
// TODO: double check this
        float2 l;
        l.x = (depth_bounds.x + depth_bounds.z);
        l.y = (depth_bounds.y + depth_bounds.w);
        
        //final_color.rg = 1.0f - depth;
        //final_color.b = 0;
        
        l -= 2 * color_index;
        float b = sqrt(dot(l.xy, l.xy));
        float m = saturate(b / color_index.r);
        
        //float a_index = color_index * 4.0f + 0.5f;
        float4 primary_color = colors[color_index];
        float3 secondary_color = colors[color_index + 1.0f].rgb;
        
        float dimming_factor = m;
        if (l.x + l.y > 0.0f)
            dimming_factor *= overlapping_dimming_factor[color_index];
		
        final_color.rgb = dimming_factor * (primary_color.rgb + secondary_color.rgb * ping_falloff.x);
        final_color.a = primary_color.a * g_exposure.w;
        final_color.a *= ping_falloff.y;
    }
    
    return final_color;
}

#else

#define APPLY_FIXES 1

#define DEPTH_CONSTANT_Z zbuf_params.x
#define DEPTH_CONSTANT_W zbuf_params.y
#define DEPTH_SURFACE_WIDTH zbuf_params.z
#define DEPTH_SURFACE_HEIGHT zbuf_params.w

float saber_vision_depth_sample(float2 texcoord, float x_offset_mult, float y_offset_mult)
{
    float2 zbuf_texcoord;
    zbuf_texcoord.x = (DEPTH_SURFACE_WIDTH  * 2 * x_offset_mult) + texcoord.x;
    zbuf_texcoord.y = (DEPTH_SURFACE_HEIGHT * 2 * y_offset_mult) + texcoord.y;
    
    return DEPTH_CONSTANT_Z / tex2D(depth_sampler, zbuf_texcoord).r + DEPTH_CONSTANT_W;
}

float4 ps_default(VS_OUTPUT_DEFAULT input) : COLOR
{
    float2 texcoord = input.texcoord;
    
    float4 depth_sample = tex2D(depth_sampler, texcoord);
    
    float depth = zbuf_params.x / depth_sample.x + zbuf_params.y;
    float reverse_depth = 1.0f - depth; // hmm
    
    float4 transform = screen_to_world[0] * texcoord.x;
    transform += screen_to_world[1] * texcoord.y;
    transform += screen_to_world[2] * reverse_depth;
    transform += screen_to_world[3];
    float3 position = transform.xyz /= transform.w;
    
    float p_dist = rsqrt(dot(position, position));
    p_dist = rcp(p_dist); // hmm
    
    float2 _ping = ping.xy - p_dist;
    
    float falloff_dist_val;
    falloff_dist_val = p_dist * falloff.x;
    dot_u_0 = exp2(-pow(falloff_dist_val, 4));
    
    float ping_falloff = pow(saturate(ping.z * _ping + 1.0f), 3);
    
    float frac_float_index = frac(float_index);
    
    
    float cmp_val = -frac_float_index >= 0 ? 1.0f : 0.0f;
    frac_float_index = float_index - frac_float_index;
    cmp_val = float_index >= 0 ? 0.0f : cmp_val;
    frac_float_index += cmp_val;
    
    float4 r4 = frac_float_index + float4(-0, -1, -2, -3);
    float4 r5 = frac_float_index + float4(-3, -4, -5, -6);
    frac_float_index += -3;

    float3 color_result = 0;
    if (-abs(r4.x) >= 0)
        color_result = colors[1].rgb;
    if (-abs(r4.y) >= 0)
        color_result = colors[3].rgb;
    if (-abs(r4.z) >= 0)
        color_result = colors[5].rgb;
    if (-abs(r5.x) >= 0)
        color_result = colors[7].rgb;
    if (-abs(r5.y) >= 0)
        color_result = colors[9].rgb;
    if (-abs(r5.z) >= 0)
        color_result = colors[11].rgb;
    if (-abs(r5.w) >= 0)
        color_result = colors[13].rgb;

    color_result *= ping_falloff;

    if (_ping.x < 0)
        color_result = 0;
    
    float r0_w = 1.0f;
    if (_ping.y < 0)
        r0_w = 0;

    float3 color_result_2 = 0;
    if (-abs(r4.x) >= 0)
        color_result_2 = colors[0].rgb;
    if (-abs(r4.y) >= 0)
        color_result_2 = colors[2].rgb;
    if (-abs(r4.z) >= 0)
        color_result_2 = colors[4].rgb;
    if (-abs(r5.x) >= 0)
        color_result_2 = colors[6].rgb;
    if (-abs(r5.y) >= 0)
        color_result_2 = colors[8].rgb;
    if (-abs(r5.z) >= 0)
        color_result_2 = colors[10].rgb;
    if (-abs(r5.w) >= 0)
        color_result_2 = colors[12].rgb;
    
    color_result += color_result_2;
    
    float overlap_dimming = 0;
    if (-abs(r4.x) >= 0)
        overlap_dimming = overlapping_dimming_factor[0];
    if (-abs(r4.y) >= 0)
        overlap_dimming = overlapping_dimming_factor[1];
    if (-abs(r4.z) >= 0)
        overlap_dimming = overlapping_dimming_factor[2];
    if (-abs(r4.w) >= 0)
        overlap_dimming = overlapping_dimming_factor[3];
    if (-abs(frac_float_index) >= 0)
        overlap_dimming = overlapping_dimming_factor[4];
    
    float2 zbuf_texcoord1 = zbuf_params.zw * 2 + texcoord;
    float4 depth_sample1 = tex2D(depth_sampler, float2(zbuf_texcoord1.x, texcoord.y));
    float depth1 = 1.0f - (zbuf_params.x / depth_sample1.x + zbuf_params.y);
    
    float2 zbuf_texcoord2 = zbuf_params.zw * -2 + texcoord;
    float4 depth_sample2 = tex2D(depth_sampler, float2(zbuf_texcoord2.x, texcoord.y));
    float depth2 = zbuf_params.x / depth_sample2.x + zbuf_params.y;

    depth1 -= depth2;
    depth1 += 1.0f;
    depth1 += reverse_depth * -2;
    
    

}
#endif

#endif