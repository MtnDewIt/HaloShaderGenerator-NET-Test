#ifndef _RAVI_ORDER_3_HLSL
#define _RAVI_ORDER_3_HLSL

#include "..\registers\global_parameters.hlsli"

void per_pixel_ravi_order_3(float3 normal, inout float4 color)
{
    // similar to diffuse_reflectance
    
    float c0 = -0.85808599f;
    float c1 = 0.429043f;
    float c2 = 0.511664f;
    float c3 = 0.282094806f;
    float c4 = 0.886227f;
	
    float3 light_cont = c3 * p_lighting_constant_0.rgb;
    float3 lightprobe_color = c4 * p_lighting_constant_0.rgb;
	
    float3 x1, x2, x3;
	//linear
    x1.r = dot(normal, p_lighting_constant_1.rgb);
    x1.g = dot(normal, p_lighting_constant_2.rgb);
    x1.b = dot(normal, p_lighting_constant_3.rgb);
	//quadratic
    float3 a = normal.xyz * normal.yzx;
    x2.r = dot(a.xyz, p_lighting_constant_4.rgb);
    x2.g = dot(a.xyz, p_lighting_constant_5.rgb);
    x2.b = dot(a.xyz, p_lighting_constant_6.rgb);
    float4 b = float4(normal.xyz * normal.xyz, 1.f / 3.f);
    x3.r = dot(b.xyzw, p_lighting_constant_7.rgba);
    x3.g = dot(b.xyzw, p_lighting_constant_8.rgba);
    x3.b = dot(b.xyzw, p_lighting_constant_9.rgba);
	
    lightprobe_color += (-2.f * c2) * x1;
    lightprobe_color += c0 * x2;
    lightprobe_color -= c1 * x3;
	
    lightprobe_color = (lightprobe_color.rgb / PI) - light_cont.rgb;
    lightprobe_color = color.a * lightprobe_color.rgb + light_cont.rgb;
	
    color.rgb *= lightprobe_color.rgb;
}

#endif