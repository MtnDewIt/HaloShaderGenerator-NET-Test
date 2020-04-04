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

#endif
