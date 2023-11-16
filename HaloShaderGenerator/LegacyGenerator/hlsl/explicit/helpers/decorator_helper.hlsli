
#include "..\registers\decorator_registers.hlsli"

struct VS_DECORATOR_DEFAULT_OUTPUT
{
    float4 position : SV_Position;
    float4 texcoord : TEXCOORD;
    float4 lighting : TEXCOORD1;
    float4 radiance : TEXCOORD2;
    float3 normal : TEXCOORD3;
};

struct S_DECORATOR_VERTEX_INPUT
{
    float4 position : POSITION;
    float2 texcoord : TEXCOORD0;
    float3 normal : NORMAL;
    float4 instance_position : TEXCOORD1;
    float4 instance_quaternion : TEXCOORD2;
    float4 instance_color : TEXCOORD3;
};

#ifdef VERTEX_SHADER

struct s_simple_light
{
    float3 position;
    float size;
    float3 direction;
    float sphere;
    float3 color;
    float smooth;
    float2 falloff_scale;
    float2 falloff_offset;
    float bounding_radius;
    float3 PADDING96;
};

float2 sample_wind_noise(float2 tex)
{
    return tex2Dlod(wind_texture, float4(tex.xy * wind_data.zz + wind_data.xy, 0.0f, 0.0f)).rg * wind_data2.zz + wind_data2.xy;
}

s_simple_light get_simple_light(int light_index)
{
#ifdef MS23_ORIGINAL

    s_simple_light built_light;

    built_light.position =              v_simple_lights[light_index + 0].xyz;
    built_light.size =                  v_simple_lights[light_index + 0].w;
    built_light.direction =             v_simple_lights[light_index + 1].xyz;
    built_light.sphere =                v_simple_lights[light_index + 1].w;
    built_light.color =                 v_simple_lights[light_index + 2].xyz;
    built_light.smooth =                v_simple_lights[light_index + 2].w;
    built_light.falloff_scale =         v_simple_lights[light_index + 3].xy;
    built_light.falloff_offset =        v_simple_lights[light_index + 3].zw;
    built_light.bounding_radius =       v_simple_lights[light_index + 4].x;
    built_light.PADDING96 =             v_simple_lights[light_index + 4].yzw;

    return built_light;
#else

    s_simple_light built_light;

    built_light.position =          v_simple_lights[light_index * 5 + 0].xyz;
    built_light.size =              v_simple_lights[light_index * 5 + 0].w;
    built_light.direction =         v_simple_lights[light_index * 5 + 1].xyz;
    built_light.sphere =            v_simple_lights[light_index * 5 + 1].w;
    built_light.color =             v_simple_lights[light_index * 5 + 2].xyz;
    built_light.smooth =            v_simple_lights[light_index * 5 + 2].w;
    built_light.falloff_scale =     v_simple_lights[light_index * 5 + 3].xy;
    built_light.falloff_offset =    v_simple_lights[light_index * 5 + 3].zw;
    built_light.bounding_radius =   v_simple_lights[light_index * 5 + 4].x;
    built_light.PADDING96 =         v_simple_lights[light_index * 5 + 4].yzw;

    return built_light;
#endif
}

void calculate_simple_light(s_simple_light light, in float3 world_position, out float3 out_light)
{
    float3 world_to_light = light.position - world_position;
    float d_world_to_light = dot(world_to_light, world_to_light);
    world_to_light *= rsqrt(d_world_to_light);
		
    float2 falloff = float2(1.0f / (light.size + d_world_to_light), dot(world_to_light, light.direction));
    falloff = falloff * light.falloff_scale + light.falloff_offset;
    falloff = max(0.0001f, falloff);
    falloff.y = pow(falloff.y, light.smooth) + light.sphere;

    out_light = light.color * (saturate(falloff.x) * saturate(falloff.y));
}

void evaluate_simple_light(int light_index, in float3 world_position, inout float3 diffusely_reflected_light)
{
    s_simple_light light = get_simple_light(light_index);

    float3 world_to_light = light.position - world_position;

    [flatten]
    if (dot(world_to_light, world_to_light) >= light.bounding_radius)
    {
        float3 light_out;
        calculate_simple_light(light, world_position, light_out);
				
        diffusely_reflected_light += light_out;
    }
}

void calc_simple_lights_spec_only(in float3 world_position, out float3 diffusely_reflected_light)
{
    diffusely_reflected_light = float3(0.0f, 0.0f, 0.0f);
    
#ifdef MS23_ORIGINAL
    if (0 < v_simple_light_count.x)
    {
        evaluate_simple_light(0, world_position, diffusely_reflected_light);

        if (1 < v_simple_light_count.x)
        {
            evaluate_simple_light(1, world_position, diffusely_reflected_light);
            
            if (2 < v_simple_light_count.x)
            {
                evaluate_simple_light(2, world_position, diffusely_reflected_light);
                
                if (3 < v_simple_light_count.x)
                {
                    evaluate_simple_light(3, world_position, diffusely_reflected_light);
                    
                    if (4 < v_simple_light_count.x)
                    {
                        evaluate_simple_light(4, world_position, diffusely_reflected_light);
                        
                        if (5 < v_simple_light_count.x)
                        {
                            evaluate_simple_light(5, world_position, diffusely_reflected_light);
                            
                            if (6 < v_simple_light_count.x)
                            {
                                evaluate_simple_light(6, world_position, diffusely_reflected_light);
                                
                                if (7 < v_simple_light_count.x)
                                {
                                    evaluate_simple_light(7, world_position, diffusely_reflected_light);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

#else
	
	[loop]
    for (int light_index = 0; light_index < v_simple_light_count.x; light_index++)
    {
        s_simple_light light = get_simple_light(light_index);
        
        float3 world_to_light = light.position - world_position;
    
        if (dot(world_to_light, world_to_light) >= light.bounding_radius)
        {
            continue;
        }
		
        float3 light_out;
        calculate_simple_light(light, world_position, light_out);
				
        diffusely_reflected_light += light_out;
    }

#endif
}

#endif
