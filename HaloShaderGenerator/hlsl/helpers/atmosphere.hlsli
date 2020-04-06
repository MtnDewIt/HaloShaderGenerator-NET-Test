#ifndef _ATMOSPHERE_HLSL
#define _ATMOSPHERE_HLSL

#include "../registers/vertex_shader.hlsli"
#include "math.hlsli"

void calculate_atmosphere_radiance(float4 vertex_position, float3 camera_dir, out float3 extinction_factor, out float3 sky_radiance)
{
	if (v_atmosphere_constant_1.w < 0.0)
	{
		sky_radiance.rgb = 1.0;
		extinction_factor.rgb = 0.0;
	}
	else
	{
		float camera_dist; // distance from camera to vertex
		float overall_distance; // distance travelled by the light to get to the camera
		float3 n_camera_dir; // vector from vertex to camera, normalized
		float costheta; // cos of angle between camera dir and sun dir
		
		camera_dist = length(camera_dir);
		n_camera_dir = camera_dir / camera_dist;
		costheta = dot(n_camera_dir, v_atmosphere_constant_0.xyz);
		overall_distance = camera_dist + v_atmosphere_constant_0.w;
		overall_distance = clamp(overall_distance, 0, v_atmosphere_constant_1.w);

		float unknown_power = pow((v_atmosphere_constant_extra.x * costheta + v_atmosphere_constant_2.w), -1.5);
		float atmosphere_height_at_vertex = max(vertex_position.z - v_atmosphere_constant_3.w, 0);
		float atmosphere_height_at_camera = max(camera_position.z - v_atmosphere_constant_3.w, 0);
		float unknown_height = atmosphere_height_at_camera * LOG2_E;
		float atmosphere_thickness = atmosphere_height_at_camera - atmosphere_height_at_vertex;
		
		// scattering coefficient Beta_0 at Earth's surface for 2 types of particle
		// v_atmosphere_constant_3.rgb, v_atmosphere_constant_2.rgb
		
		// atmosphere thickness v_atmosphere_constant_0.w ? this is not in the paper
		// sun position v_atmosphere_constant_0.xyz
		// maximial distance travelled by light: v_atmosphere_constant_1.w
		
		// exponential decay constant, one for each particle type, seems to be 1/alpha relative to the paper
		// v_atmosphere_constant_4.w v_atmosphere_constant_5.w

		// atmosphere start height? v_atmosphere_constant_3.w
		
		// compute the extinction factor for the 2 types of particles for the specified distance
		if (atmosphere_height_at_camera * atmosphere_height_at_camera >= EPSILON) // if enough thickness
		{
		
			float scale1 = -overall_distance * v_atmosphere_constant_5.w * (exp(-unknown_height / v_atmosphere_constant_5.w) - exp(-atmosphere_height_at_vertex * LOG2_E / v_atmosphere_constant_5.w)) * (1.0 / atmosphere_thickness);
			float scale2 = -overall_distance * v_atmosphere_constant_4.w * (exp(-unknown_height / v_atmosphere_constant_4.w) - exp(-atmosphere_height_at_vertex * LOG2_E / v_atmosphere_constant_4.w)) * (1.0 / atmosphere_thickness);
			extinction_factor.rgb = exp(-(scale2 * v_atmosphere_constant_3.xyz + v_atmosphere_constant_2.xyz * scale1));
		}
		else // |s * cos(theta) | << 1 (i.e close to 0
		{
			// r3.z = alpha, h_0 = 1 / atm_param for type then scale1 = H_1, scale2 = H_2
			float scale1 = -overall_distance * exp(-unknown_height / v_atmosphere_constant_5.w);
			float scale2 = -overall_distance * exp(-unknown_height / v_atmosphere_constant_4.w);
			extinction_factor.rgb = exp(-(scale2 * v_atmosphere_constant_3.rgb + v_atmosphere_constant_2.rgb * scale1));
		}
		sky_radiance.rgb = (v_atmosphere_constant_1.rgb * (costheta * costheta + 1.0 * v_atmosphere_constant_4.xyz + unknown_power * v_atmosphere_constant_5.xyz)) * (1.0 - extinction_factor);
	}
}




#endif