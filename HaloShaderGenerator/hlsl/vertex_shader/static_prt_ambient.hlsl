#define shader_template

#include "registers/vertex_shader.hlsli"
#include "../helpers/input_output.hlsli"
#include "helpers/projection_math.hlsli"
#include "../helpers/math.hlsli"

// temporary definition, will have to macro the hell out of this
VS_OUTPUT_STATIC_PTR_AMBIENT global_entry_rigid_static_prt_ambient(VS_INPUT_RIGID_VERTEX_AMBIENT_PRT input)
{
	VS_OUTPUT_STATIC_PTR_AMBIENT output;
	output.tangent = transform_value(input.tangent.xyz, nodes[0], nodes[1], nodes[2]);
	output.normal = transform_value(input.normal.xyz, nodes[0], nodes[1], nodes[2]);
	output.binormal = transform_value(input.binormal.xyz, nodes[0], nodes[1], nodes[2]);
	output.texcoord.xy = input.texcoord.xy * uv_compression_scale_offset.xy + uv_compression_scale_offset.zw;
	float4 vertex_position = input.position * position_compression_scale + position_compression_offset; //model space
	vertex_position.xyz = transform_value(vertex_position.xyz, nodes[0], nodes[1], nodes[2]); // world space
	float3 camera_dir = camera_position.xyz - vertex_position.xyz;
	output.camera_dir = camera_dir;
	
	if (v_atmosphere_constant_1.w < 0.0)
	{
		output.sky_radiance.xyz = 1.0;
		output.extinction_factor.xyz = 0.0;
	}
	else
	{
		float camera_dist;		// distance from camera to vertex
		float overall_distance; // distance travelled by the light to get to the camera
		float3 n_camera_dir;	// vector from vertex to camera, normalized
		float costheta;			// cos of angle between camera dir and sun dir
		
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
		float3 extinction_factor;
		if (atmosphere_height_at_camera * atmosphere_height_at_camera >= EPSILON) // if enough thickness
		{
		
			float scale1 = -overall_distance * v_atmosphere_constant_5.w * (exp(-unknown_height / v_atmosphere_constant_5.w) - exp(-atmosphere_height_at_vertex * LOG2_E / v_atmosphere_constant_5.w)) * (1.0 / atmosphere_thickness);
			float scale2 = -overall_distance * v_atmosphere_constant_4.w * (exp(-unknown_height / v_atmosphere_constant_4.w) - exp(-atmosphere_height_at_vertex * LOG2_E / v_atmosphere_constant_4.w)) * (1.0 / atmosphere_thickness);
			extinction_factor = exp(-(scale2 * v_atmosphere_constant_3.xyz + v_atmosphere_constant_2.xyz * scale1));
		}
		else // |s * cos(theta) | << 1 (i.e close to 0
		{
			// r3.z = alpha, h_0 = 1 / atm_param for type then scale1 = H_1, scale2 = H_2
			float scale1 = overall_distance * exp(-unknown_height / v_atmosphere_constant_5.w);
			float scale2 = overall_distance * exp(-unknown_height / v_atmosphere_constant_4.w);
			extinction_factor = exp(-(scale2 * v_atmosphere_constant_3.rgb + v_atmosphere_constant_2.rgb * scale1));
		}
		output.extinction_factor = extinction_factor;
		output.sky_radiance = (v_atmosphere_constant_1.rgb * (costheta * costheta + 1.0 * v_atmosphere_constant_4.xyz + unknown_power * v_atmosphere_constant_5.xyz)) * (1.0 - extinction_factor);
	}

	vertex_position.w = 1.0;
	output.position.x = dot(vertex_position, view_projection[0]);
	output.position.y = dot(vertex_position, view_projection[1]);
	output.position.z = dot(vertex_position, view_projection[2]);
	output.position.w = dot(vertex_position, view_projection[3]);
	
	float4 r0;
	// sh stuff begins here
	r0.y = 0.333333333;
	r0.x = dot(v_lighting_constant_0, r0.yyyy);
	r0.y = r0.x * input.coefficient.x;
	r0.x = r0.x * 0.282094806;
	r0.xy = max(r0.xy, 0.01);
	r0.x = 1.0 / r0.x;
	output.TexCoord7.x = r0.x * r0.y;
	vertex_position.xyz = v_lighting_constant_1.xyz;
	r0.xzw = vertex_position.xyyz + v_lighting_constant_2.xyyz;
	r0.xzw = r0 + v_lighting_constant_3.xyyz;
	vertex_position.xyz = normalize(r0.xzww);
	r0.x = dot(output.normal, -vertex_position);
	output.TexCoord7.w = min(r0.y, r0.x);
	output.TexCoord7.y = r0.y;
	output.TexCoord7.z = input.coefficient.x * 3.54490733;

	return output;
}