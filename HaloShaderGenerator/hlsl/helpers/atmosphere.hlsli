#ifndef _ATMOSPHERE_HLSL
#define _ATMOSPHERE_HLSL

#pragma warning( disable : 3571 34)

#include "../registers/vertex_shader.hlsli"
#include "math.hlsli"

float calculate_atmosphere_particle_factor_with_thickness(float overall_distance, float atmosphere_height_at_vertex, float unknown_height, float one_over_thickness, float atmosphere_constant)
{
	float temp_const = 1.0 / atmosphere_constant;
	return -(exp2(-atmosphere_height_at_vertex * temp_const) - exp2(-unknown_height * temp_const)) * overall_distance * atmosphere_constant * one_over_thickness;
}

float calculate_atmosphere_particle_factor_without_thickness(float travel_distance, float atmosphere_height, float atmosphere_constant)
{
	float temp_const = 1.0 / atmosphere_constant;
	return exp2(-atmosphere_height * temp_const) * travel_distance;
}

void calculate_atmosphere_radiance(float4 vertex_position, float3 camera_dir, out float3 extinction_factor, out float3 sky_radiance)
{
	// note: code is optimzied to compiled exactly as in the original vertex shader, not the cleanest way (log2_e is automatically added by the compiler when using exp instead of 
	// exp2)
	
	// scattering coefficient Beta_0 at Earth's surface for 2 types of particle
	// v_atmosphere_constant_3.rgb, v_atmosphere_constant_2.rgb
		
	// atmosphere thickness v_atmosphere_constant_0.w ? this is not in the paper
	// sun position v_atmosphere_constant_0.xyz
	// maximal distance travelled by light: v_atmosphere_constant_1.w
		
	// exponential decay constant, one for each particle type, seems to be 1/alpha relative to the paper
	// v_atmosphere_constant_4.w v_atmosphere_constant_5.w

	// atmosphere start height? v_atmosphere_constant_3.w
	
	if (v_atmosphere_constant_1.w < 0.0)
	{
		sky_radiance.rgb = 0.0;
		extinction_factor.rgb = 1.0;
	}
	else
	{
		float camera_dist;					// distance from camera to vertex
		float overall_distance;				// distance travelled by the light to get to the camera
		float3 n_camera_dir;				// vector from vertex to camera, normalized
		float costheta;						// cos of angle between camera dir and sun dir
		
		camera_dist = length(camera_dir);
		n_camera_dir = normalize(camera_dir);
		
		costheta = dot(n_camera_dir, v_atmosphere_constant_0.xyz);
		float cts = (costheta * costheta + 1.0);
		overall_distance = camera_dist + v_atmosphere_constant_0.w;
		overall_distance = clamp(overall_distance, 0, v_atmosphere_constant_1.w);
		
		float atmosphere_height_at_camera = max(Camera_Position.z - v_atmosphere_constant_3.w, 0);
		
		float atmosphere_height_at_vertex = vertex_position.z - v_atmosphere_constant_3.w;
		atmosphere_height_at_vertex = max(atmosphere_height_at_vertex, 0); // swap order if max are not merged
		
		// TODO: constant ordering issue between -1.5 and EPSILON, but instruction order is 1-1 anyway.
		
		float atmosphere_thickness = atmosphere_height_at_camera - atmosphere_height_at_vertex;
		float atmosphere_height_at_camera_log2e = atmosphere_height_at_camera * LOG2_E;
		float thickness = atmosphere_thickness * atmosphere_thickness;
		float atmosphere_base = v_atmosphere_constant_extra.x * costheta + v_atmosphere_constant_2.w;
		float power = pow(atmosphere_base, -1.5);

		// compute the extinction factor for the 2 types of particles for the specified distance	
		float scale1, scale2;
		if (EPSILON < thickness) // if atmosphere height difference is large enough between camera and vertex, use extra calculations
		{
			atmosphere_height_at_vertex *= LOG2_E;
			float one_over_thickness = 1.0 / atmosphere_thickness;
			scale1 = calculate_atmosphere_particle_factor_with_thickness(overall_distance, atmosphere_height_at_camera_log2e, atmosphere_height_at_vertex, one_over_thickness, v_atmosphere_constant_4.w);
			scale2 = calculate_atmosphere_particle_factor_with_thickness(overall_distance, atmosphere_height_at_camera_log2e, atmosphere_height_at_vertex, one_over_thickness, v_atmosphere_constant_5.w);
		}
		else
		{
			scale1 = calculate_atmosphere_particle_factor_without_thickness(overall_distance, atmosphere_height_at_camera_log2e, v_atmosphere_constant_4.w);
			scale2 = calculate_atmosphere_particle_factor_without_thickness(overall_distance, atmosphere_height_at_camera_log2e, v_atmosphere_constant_5.w);
		}
		extinction_factor.rgb = exp2(-(v_atmosphere_constant_2.rgb * scale2 + scale1 * v_atmosphere_constant_3.rgb));
		float3 atmosphere_color = v_atmosphere_constant_1.rgb * ((v_atmosphere_constant_4.xyz * cts) + (v_atmosphere_constant_5.xyz * power));
		sky_radiance.rgb = atmosphere_color * (1.0 - extinction_factor);
	}
}

void calculate_atmosphere_radiance_new(in float3 camera_position, in float3 world_position, out float3 extinction, out float3 radiance)
{
    float3 light_source_direction = v_atmosphere_constant_0.xyz;
    float distance_bias = v_atmosphere_constant_0.w;
    float3 light_source_color = v_atmosphere_constant_1.xyz;
    float max_fog_thickness = v_atmosphere_constant_1.w;
    float3 beta_molecules = v_atmosphere_constant_2.xyz;
    float sun_phase = v_atmosphere_constant_2.w;
    float3 beta_particles = v_atmosphere_constant_3.xyz;
    float sea_level = v_atmosphere_constant_3.w;
    float3 beta_m_theta_prefix = v_atmosphere_constant_4.xyz;
    float mie_height_scale = v_atmosphere_constant_4.w;
    float3 beta_p_theta_prefix_phased = v_atmosphere_constant_5.xyz;
    float rayleigh_height_scale = v_atmosphere_constant_5.w;

    if (max_fog_thickness < 0.0f)
    {
        extinction = 1.0f;
        radiance = 0.0f;
    }
    else
    {
        float3 view_vector = camera_position - world_position;
        float dist = sqrt(dot(view_vector, view_vector));
        view_vector /= dist;

        float d_dir = -dot(view_vector, light_source_direction);

        dist = min(max(dist + distance_bias, 0.0f), max_fog_thickness);
		
        float camera_height = max(camera_position.z - sea_level, 0.0f);
        float world_height = max(world_position.z - sea_level, 0.0f);
        float height_diff = camera_height - world_height;
		
        camera_height *= LOG2_E;
        world_height *= LOG2_E;
		
        if (height_diff * height_diff > EPSILON)
        {
            float scale1 = -(exp2(-camera_height / mie_height_scale) - exp2(-world_height / mie_height_scale)) * dist * mie_height_scale / height_diff;
            float scale2 = -(exp2(-camera_height / rayleigh_height_scale) - exp2(-world_height / rayleigh_height_scale)) * dist * rayleigh_height_scale / height_diff;
			
            extinction = exp2(-(beta_molecules * scale2 + beta_particles * scale1));
        }
        else
        {
            float scale1 = exp2(-camera_height / mie_height_scale) * dist;
            float scale2 = exp2(-camera_height / rayleigh_height_scale) * dist;
			
            extinction = exp2(-(beta_molecules * scale2 + beta_particles * scale1));
        }
		
        float3 beta_m_theta = beta_m_theta_prefix * (1.0f + d_dir * d_dir);
        float3 beta_p_theta = beta_p_theta_prefix_phased * pow(sun_phase - v_atmosphere_constant_extra.x * d_dir, -1.5f);

        radiance = light_source_color * (beta_m_theta + beta_p_theta) * (1.0f - extinction);
    }
}

#endif