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
	
	float4 vertex_position = input.position * position_compression_scale + position_compression_offset;
	vertex_position.xyz = transform_value(vertex_position.xyz, nodes[0], nodes[1], nodes[2]);
	vertex_position.w = 1.0;
	// r2 = vertex_position
	float4 r0 = camera_position - vertex_position.xyz;
	float4 r1;
	float4 r3;
	r3.w = 0.0;
	
	if (v_atmosphere_constant_1.w < 0.0)
	{
		output.TexCoord6.xyz = 1.0;
		output.TexCoord7.xyz = 0.0;
	}
	else
	{
		// reverse this huge thing
		
		float4 r4;
		float4 r5;
		r0.w = max(vertex_position.z - v_atmosphere_constant_3.w, 0);
		r3.x = distance(r0, 0);
		r1.w = 1.0 / r3.x;
		r3.yzww = r0.xxyw * r1.w;
		r1.w = dot(r3.yzww, v_atmosphere_constant_0);
		r3.x = r3.x + v_atmosphere_constant_0.w;
		r3.x = max(r3.x, 0);
		r3.x = max(r3.x, v_atmosphere_constant_1.w);
		r3.w = v_atmosphere_constant_2.w;
		r3.y = v_atmosphere_constant_extra.x * r1.w + r3.w;
		r4.x = pow(r3.y, -1.5);
		r3.w = v_atmosphere_constant_3.w;
		r3.y = camera_position.z - r3.w;
		r3.y = max(r3.y, 0);
		r3.z = r3.y * 1.44269502;
		r3.y = -r0.w + r3.y;
		r3.w = r3.y * r3.y;

		if (r3.w >= EPSILON)
		{
		
			r0.w = r0.w * 1.44269502;
			r3.y = 1.0 / r3.y;
			r3.w = 1.0 / v_atmosphere_constant_5.w;
			r4.y = r3.w * -r0.w;
			r4.y = exp(r4.y);
			r3.w = r3.w * -r3.z;
			r3.w = exp(r3.w);
			r3.w = r3.w - r4.y;
			r3.w = r3.x * -r3.w;
			r3.w = r3.w * v_atmosphere_constant_5.w;
			r3.w = r3.y * r3.w;
			r4.y = 1.0 / v_atmosphere_constant_4.w;
			r0.w = r4.y * -r0.w;
			r0.w = exp(r0.w);
			r4.y = r4.y * -r3.z;
			r4.y = exp(r4.y);
			r0.w = -r0.w + r4.y;
			r0.w = r3.x * -r0.w;
			r0.w = r0.w * v_atmosphere_constant_4.w;
			r0.w = r3.y * r0.w;
			r4.yzw = r0.w * v_atmosphere_constant_3.xxyz;
			r4.yzw = v_atmosphere_constant_2.xxyz * r3.w + r4;
			r5.x = exp(-r4.y);
			r5.y = exp(-r4.z);
			r5.z = exp(-r4.w);
		}
		else
		{
			r0.w = 1.0 / v_atmosphere_constant_5.w;
			r0.w = r0.w * -r3.z;
			r0.w = exp(r0.w);
			r0.w = r3.x * r0.w;
			
			r3.y = 1.0 / v_atmosphere_constant_4.w;
			r3.y = r3.y * -r3.z;
			r3.y = exp(r3.y);
			r3.x = r3.x * r3.y;
			r3.xyz = r3.x * v_atmosphere_constant_3.xyz;
			r3.xyz = v_atmosphere_constant_2.xyz * r0.w + r3.xyz;
			r5.x = exp(-r3.x);
			r5.y = exp(-r3.y);
			r5.z = exp(-r3.z);
		}
		
		r0.w = r1.w * r1.w + 1.0;
		r3.xyz = r4.x * v_atmosphere_constant_5.xyz;
		r3.xyz = r0.w * v_atmosphere_constant_4.xyz + r3.xyz;
		r3.xyz = r3.xyz * v_atmosphere_constant_1.xyz;
		r4.xyz = -r5.xyz + 1.0;
		output.Color = r5;
		output.Color1 = r3 * r4;
	}
	
	output.TexCoord6 = r0.xyz;
	
	vertex_position.w = 1.0;
	output.position.x = dot(vertex_position, view_projection[0]);
	output.position.y = dot(vertex_position, view_projection[1]);
	output.position.z = dot(vertex_position, view_projection[2]);
	output.position.w = dot(vertex_position, view_projection[3]);
	
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
	r0.x = dot(r1, -vertex_position);
	output.TexCoord6.w = min(r0.y, r0.x);
	output.TexCoord6.y = r0.y;
	output.TexCoord6.z = input.coefficient.x * 3.54490733;

	return output;
}