#ifndef _LIGHTING_HLSLI
#define _LIGHTING_HLSLI

#include "../helpers/math.hlsli"
#include "../registers/shader.hlsli"

float get_light_diffuse_intensity(SimpleLight light, float3 normal, float3 light_dir)
{
	return max(0.05, dot(normal, light_dir));
}

// TODO: fix this crap while maintaining compiled code the same
void calculate_simple_light(
SimpleLight simple_light, 
float3 normal,
float3 vertex_world_position,
float3 dominant_light_reflect_dir,
float other_specular,
inout float3 diffuse_accumulation,
inout float3 specular_accumulation)
{
    float4 r5 = float4(0, 0, 0, 0);
    float4 r6 = float4(0, 0, 0, 0);

	float3 v_to_light_dir = simple_light.position.xyz - vertex_world_position;
	float v_to_light_dir_norm = dot(v_to_light_dir, v_to_light_dir);
	float3 v_to_light_dir_n = (v_to_light_dir / sqrt(v_to_light_dir_norm)).xyz;
	float n_dot_l = dot(normal, v_to_light_dir_n);

	float dl_dot_l = max(dot(dominant_light_reflect_dir, v_to_light_dir_n), 0);
	float specular_c = pow(dl_dot_l, other_specular);
	
	r6.x = 1.0 / (v_to_light_dir_norm + simple_light.position.w);

	r6.y = dot(v_to_light_dir_n, simple_light.direction.xyz);

    r5.xy = (r6 * simple_light.unknown3 + simple_light.unknown3.zwzw).xy;

    r6.xy = max(0.0001, r5).xy;

    float temporary2 = pow(r6.y, simple_light.color.w);

	temporary2 = saturate(temporary2 + simple_light.direction.w);

    r6.x = saturate(r6.x);

    float3 attenuation  = temporary2 * r6.x;
	float diffuse_impact = max(0.05, n_dot_l); // cos between light and normal directions
    // unknown4.x is the maximal distance the light has an effect, if distance_m_max_falloff >=0 means light is too far to render
	float distance_m_max_falloff = v_to_light_dir_norm - simple_light.unknown4.x;
	float3 light_color = (simple_light.color.rgb * attenuation);
	
	float3 diffuse = (light_color * diffuse_impact);
	float3 specular = (light_color * specular_c);
	
	diffuse_accumulation = distance_m_max_falloff >= 0 ? diffuse_accumulation : diffuse + diffuse_accumulation;
	specular_accumulation = distance_m_max_falloff >= 0 ? specular_accumulation : specular + specular_accumulation;
}

#endif
