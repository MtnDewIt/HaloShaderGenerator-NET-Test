#ifndef _LIGHTING_HLSLI
#define _LIGHTING_HLSLI

#include "../helpers/math.hlsli"
#include "../registers/shader.hlsli"

float3 calculate_simple_light(SimpleLight simple_light, float3 accumulation, float3 normal, float3 vertex_world_position)
{
    //def c11, 0.5, 2, -1, 0.333333343
    float4 c11 = float4(0.5, 2, -1, 0.333333343);
    //def c12, 0.318309873, 0, 9.99999975e-005, 0.0500000007
    float4 c12 = float4(0.318309873, 0, 9.99999975e-005, 0.0500000007);
    //def c58, -1.02332795, 0.886227012, -0.85808599, 0.429042995
    float4 c58 = float4(-1.02332795, 0.886227012, -0.85808599, 0.429042995);

    float4 r5 = float4(0, 0, 0, 0);
    float4 r6 = float4(0, 0, 0, 0);
    float4 r3 = float4(0, 0, 0, 0);

    float temporary0 = 0.0;
    float temporary1 = 0.0;
    float temporary2 = 0.0;

    //add r5.xyz, -r4, simple_light.unknown0
	r5.xyz = ((-vertex_world_position) + simple_light.position.xyz).xyz;
    //dp3 temporary0, r5, r5
    temporary0 = dot(r5.xyz, r5.xyz);
    //rsq temporary1, temporary0
    temporary1 = 1.0 / sqrt(temporary0);
    //mul r5.xyz, temporary1, r5
    r5.xyz = (temporary1.xxxx * r5).xyz;
    //dp3 temporary1, r2, r5
    temporary1 = dot(normal, r5.xyz);
    //add temporary2, temporary0, simple_light.unknown0.w
	temporary2 = temporary0 + simple_light.position.w;
    //rcp r6.x, temporary2
    r6.x = 1.0 / temporary2;
    //dp3 r6.y, r5, simple_light.unknown1
    r6.y = dot(r5.xyz, simple_light.unknown1.xyz);
    //mad r5.xy, r6, simple_light.unknown3, simple_light.unknown3.zwzw
    r5.xy = (r6 * simple_light.unknown3 + simple_light.unknown3.zwzw).xy;
    //max r6.xy, c12.z, r5
    r6.xy = max(c12.zzzz, r5).xy;
    //pow temporary2, r6.y, simple_light.unknown2.w
    temporary2 = pow(r6.y, simple_light.unknown2.w);
    //add_sat temporary2, temporary2, simple_light.unknown1.w
    temporary2 = saturate(temporary2 + simple_light.unknown1.w);
    //mov_sat r6.x, r6.x
    r6.x = saturate(r6.x);
    //mul temporary2, temporary2, r6.x
    temporary2 = temporary2 * r6.x;
    //max r3.w, c12.w, temporary1
    r3.w = max(c12.w, temporary1);
    //add temporary0, temporary0, -simple_light.unknown4.x
    temporary0 = temporary0 - simple_light.unknown4.x;
    //mul r5.xyz, temporary2, simple_light.unknown2
    r5.xyz = (simple_light.unknown2 * temporary2).xyz;
    //mul r5.xyz, r3.w, r5
    r5.xyz = (r5 * r3.w).xyz;

    // we need to accumulate here
    r5.xyz += accumulation;

    //cmp r5.xyz, temporary0, c12.y, r5
    r5.xyz = (temporary0.xxx >= 0 ? accumulation : r5.xyz).xyz;

    return r5.xyz;
}

float3 calculate_cook_torrance_light(SimpleLight simple_light, float3 accumulation, float3 normal, float3 vertex_world_position)
{
    float3 lighting = float3(0, 0, 0);



    return accumulation + lighting;

}

#endif
