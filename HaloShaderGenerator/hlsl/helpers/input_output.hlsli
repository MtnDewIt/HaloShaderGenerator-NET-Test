#ifndef _INPUT_OUTPUT_HLSLI
#define _INPUT_OUTPUT_HLSLI

/* This include file defines all the input and ouputs for the vertex and pixel shaders. Generating vertex shaders will require automatic vertex struct generation but code exists
*  in ms23 to do exactly that at runtime. 
*/


struct VS_OUTPUT_ALBEDO
{
    float4 position : VPOS;
    float4 texcoord : TEXCOORD;
    float4 normal : TEXCOORD1;
    float4 binormal : TEXCOORD2;
    float4 tangent : TEXCOORD3;
    float vertex_to_camera : TEXCOORD4;
};

struct PS_OUTPUT_ALBEDO
{
    float4 Diffuse;
    float4 Normal;
    float4 Unknown;
};

struct VS_OUTPUT_ACTIVE_CAMO
{
    float4 vPos : VPOS;
    float4 TexCoord : TEXCOORD0;
    float4 TexCoord1 : TEXCOORD1;
    float4 TexCoord2 : TEXCOORD2;
    float4 TexCoord3 : TEXCOORD3;
    float4 TexCoord4 : TEXCOORD4;
    float4 TexCoord5 : TEXCOORD5;
    float4 TexCoord6 : TEXCOORD6;
};

struct PS_OUTPUT_DEFAULT
{
    float4 LowFrequency;
    float4 HighFrequency;
    float4 Unknown;
};

struct VS_OUTPUT_STATIC_PTR_AMBIENT
{
    // These are from VS_OUTPUT_ALBEDO
	float4 position : VPOS;
    
    float4 texcoord : TEXCOORD;
    float4 normal : TEXCOORD3;
    float4 binormal : TEXCOORD4;
    float4 tangent : TEXCOORD5;

    float4 TexCoord6 : TEXCOORD6;
    float4 TexCoord7 : TEXCOORD7;
    float4 Color : COLOR;
    float4 Color1 : COLOR1;
};

// hlsl doesn't support union so we'll have to macro the vs_input depending on the vertex and draw mode
struct VS_INPUT_RIGID_VERTEX_ALBEDO
{
    float4 position : POSITION;
    float4 texcoord : TEXCOORD;
    float4 normal : NORMAL;
    float4 tangent : TANGENT;
    float4 binormal : BINORMAL;
};

struct VS_INPUT_RIGID_VERTEX_AMBIENT_PRT
{
	float4 position : POSITION;
	float4 texcoord : TEXCOORD;
	float4 normal : NORMAL;
	float4 tangent : TANGENT;
	float4 binormal : BINORMAL;
	float coefficient : BLENDWEIGHT1;
};

struct VS_INPUT_RIGID_VERTEX_LINEAR_PRT
{
	float4 position : POSITION;
	float4 texcoord : TEXCOORD;
	float4 normal : NORMAL;
	float4 tangent : TANGENT;
	float4 binormal : BINORMAL;
	float4 coefficients : BLENDWEIGHT1;
};

struct VS_INPUT_RIGID_VERTEX_QUADRATIC_PRT
{
	float4 position : POSITION;
	float4 texcoord : TEXCOORD;
	float4 normal : NORMAL;
	float4 tangent : TANGENT;
	float4 binormal : BINORMAL;
	float3 coefficients1 : BLENDWEIGHT1;
	float3 coefficients2 : BLENDWEIGHT2;
	float3 coefficients3 : BLENDWEIGHT3;
};
#endif