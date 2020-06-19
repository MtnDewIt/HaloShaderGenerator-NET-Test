#ifndef _LIGHTING_HLSLI
#define _LIGHTING_HLSLI

#include "../helpers/math.hlsli"
#include "../registers/shader.hlsli"


struct SimpleLight
{
	float4 position;
	float4 direction;
	float4 color;
	float4 unknown3;
	float4 unknown4;
};

SimpleLight get_simple_light(int index)
{
	SimpleLight light;
	light.position = simple_lights[index * 5 + 0];
	light.direction = simple_lights[index * 5 + 1];
	light.color = simple_lights[index * 5 + 2];
	light.unknown3 = simple_lights[index * 5 + 3];
	light.unknown4 = simple_lights[index * 5 + 4];
	return light;
}

float3 get_simple_light_color(SimpleLight light)
{
	return light.color.rgb;
}


float get_light_diffuse_intensity(
SimpleLight light,
float3 normal,
float3 light_dir)
{
	return max(0.05, dot(normal, light_dir));
}

void get_simple_light_parameters(
SimpleLight light,
float3 surface_to_light,
float light_dist_squared,
out float distance_attenuation,
out float cone_attenuation)
{
	float2 packed_params;
	
	packed_params.x = 1.0 / (light_dist_squared + light.position.w);
	packed_params.y = dot(surface_to_light, light.direction.xyz);
	packed_params = packed_params * light.unknown3.xy + light.unknown3.zw;
	packed_params = max(0.0001, packed_params);
	
	distance_attenuation = packed_params.x;
	cone_attenuation = packed_params.y;
	
	cone_attenuation = pow(cone_attenuation, light.color.w);
	cone_attenuation = saturate(cone_attenuation + light.direction.w);
	distance_attenuation = saturate(distance_attenuation);
}

float3 calculate_lambertian_reflectance(
float3 n_dot_l, 
float3 light_intensity, 
float3 light_color)
{
	return light_color * light_intensity * n_dot_l;
}

float get_simple_light_max_range(SimpleLight light)
{
	return light.unknown4.x;
}

void calculate_simple_light(
SimpleLight simple_light, 
float3 normal,
float3 vertex_world_position,
float3 dominant_light_reflect_dir,
float other_specular,
inout float3 diffuse_accumulation,
inout float3 specular_accumulation)
{
	float3 v_to_light_dir = simple_light.position.xyz - vertex_world_position;
	float light_dist_squared = dot(v_to_light_dir, v_to_light_dir);

	[flatten]
	if (light_dist_squared - get_simple_light_max_range(simple_light) < 0)
	{
		float3 L = normalize(v_to_light_dir);	// normalized surface to light direction

		float distance_attenuation, cone_attenuation;
		get_simple_light_parameters(simple_light, L, light_dist_squared, distance_attenuation, cone_attenuation);

		float intensity = cone_attenuation * distance_attenuation;
		float3 light_result = get_simple_light_color(simple_light) * intensity;
		float n_dot_l = max(0.05, dot(normal, L));
		float3 diffuse = light_result * n_dot_l;
		diffuse_accumulation += diffuse;
		
		float dl_dot_l = dot(L, dominant_light_reflect_dir); 
		float specular_c = pow(max(dl_dot_l, 0), other_specular);
		float3 specular = light_result * specular_c;
		specular_accumulation += specular;
	}
}

#endif
