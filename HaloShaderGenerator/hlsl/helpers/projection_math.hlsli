#ifndef _PROJECTION_MATH_HLSLI
#define _PROJECTION_MATH_HLSLI

// find better name for this
float3 transform_value(float3 value, float3 basis1, float3 basis2, float3 basis3)
{
	float3 result;
	result.x = dot(value, basis1);
	result.y = dot(value, basis2);
	result.z = dot(value, basis3);
	return normalize(result);
}

float3 transform_binormal(float3 normal, float3 tangent, float3 binormal)
{
	float3 computed_binormal = cross(normal, tangent);
	float bin_sign = sign(dot(computed_binormal, binormal));
	return bin_sign * binormal;
}


#endif
