#ifndef _INPUT_OUTPUT_HLSLI
#define _INPUT_OUTPUT_HLSLI

struct ENVIRONMENT_MAPPING_COMMON
{
	float3 reflect_dir;
	float3 view_dir;
	float3 env_area_specular;
	float specular_coefficient;
	float3 area_specular;
	float specular_exponent;
};

struct SHADER_DYNAMIC_LIGHT_COMMON
{
	float4 albedo;
	float2 texcoord;
	float3 surface_normal;
	float3 normal;
	float3 view_dir;
	float3 reflect_dir;
	float3 light_direction;
	float3 light_intensity;
	float specular_mask;
};

struct SHADER_COMMON
{
	float alpha;
	float4 albedo;
	float2 fragcoord;
	float3 world_position;
	float2 texcoord;
	float3 normal;
	float3 tangent;
	float3 binormal;
	float3 surface_normal;
	float3 sky_radiance;
	float3 extinction_factor;
	float4 sh_0, sh_312[3], sh_457[3], sh_8866[3];
	float4 sh_0_no_dominant_light, sh_312_no_dominant_light[3];
	float3 dominant_light_direction, dominant_light_intensity;
	float3 diffuse_reflectance;
	float4 precomputed_radiance_transfer;
	float3 per_vertex_color;
	float3 view_dir;
	float3 n_view_dir;
	float3 half_dir;
	float3 reflect_dir;
	
	bool no_dynamic_lights;
	float specular_mask;
	
};

struct ALBEDO_PASS_RESULT
{
	float4 albedo;
	float alpha;
	float3 normal;
};

struct VS_OUTPUT_BLACK_ALBEDO
{
	float4 position : SV_Position;
	float3 color : COLOR0;
};

struct VS_OUTPUT_SHADOW_GENERATE
{
	float4 position : SV_Position;
	float depth : TEXCOORD;
	float2 texcoord : TEXCOORD1;
};

struct VS_OUTPUT_ALBEDO
{
    float4 position : SV_Position;
    float2 texcoord : TEXCOORD;
    float4 normal : TEXCOORD1;
    float3 binormal : TEXCOORD2;
    float3 tangent : TEXCOORD3;
    float3 camera_dir : TEXCOORD4;
};

struct VS_OUTPUT_STATIC_PRT
{
	float4 position : SV_Position;
	float2 texcoord : TEXCOORD;
	float3 normal : TEXCOORD3;
	float3 binormal : TEXCOORD4;
	float3 tangent : TEXCOORD5;
	float3 camera_dir : TEXCOORD6;
	float4 prt_radiance_vector : TEXCOORD7;
	float3 extinction_factor : COLOR;
	float3 sky_radiance : COLOR1;
};

struct VS_OUTPUT_ACTIVE_CAMO
{
	float4 position : SV_Position;
	float4 texcoord : TEXCOORD1;
	float4 camo_param : TEXCOORD;
};

struct VS_OUTPUT_DYNAMIC_LIGHT
{
	float4 position : SV_Position;
	float2 texcoord : TEXCOORD0;
	float3 normal : TEXCOORD1;
	float3 binormal : TEXCOORD2;
	float3 tangent : TEXCOORD3;
	float3 camera_dir : TEXCOORD4;
	float4 shadowmap_texcoord : TEXCOORD5;
};

struct VS_OUTPUT_SFX_DISTORT
{
	float4 position : SV_Position;
	float4 texcoord : TEXCOORD;
	float distortion : TEXCOORD1;
};

struct VS_OUTPUT_LIGHTMAP_DEBUG_MODE
{
	float4 position : SV_Position;
	float2 lightmap_texcoord : TEXCOORD;
	float3 normal : TEXCOORD1;
	float2 texcoord : TEXCOORD2;
	float3 tangent : TEXCOORD3;
	float3 binormal : TEXCOORD4;
	float3 camera_dir : TEXCOORD5;
};


struct PS_OUTPUT_ALBEDO
{
	float4 diffuse;
	float4 normal;
	float4 unknown;
};

struct PS_OUTPUT_DEFAULT
{
    float4 low_frequency;
    float4 high_frequency;
	float4 unknown;
};

struct PS_OUTPUT_DEFAULT_LDR_ONLY
{
    float4 low_frequency;
};

struct VS_OUTPUT_STATIC_SH
{
	float4 position : SV_Position;
	float3 texcoord : TEXCOORD; //z is used for the angle of the dominant light dir?
	float3 normal : TEXCOORD3; 
	float3 binormal : TEXCOORD4;
	float3 tangent : TEXCOORD5;
	float3 camera_dir : TEXCOORD6;
	float3 extinction_factor : COLOR;
	float3 sky_radiance : COLOR1;
};

struct PS_OUTPUT_SHADOW_GENERATE
{
	float4 unknown;
	float4 depth;
};

struct VS_OUTPUT_PER_PIXEL
{
	float4 position : SV_Position;
	float2 texcoord : TEXCOORD;
	float3 normal : TEXCOORD3;
	float3 binormal : TEXCOORD4;
	float3 tangent : TEXCOORD5;
	float2 lightmap_texcoord : TEXCOORD6_CENTROID;
	float3 camera_dir : TEXCOORD7;
	float3 extinction_factor : COLOR;
	float3 sky_radiance : COLOR1;
};

struct VS_OUTPUT_PER_VERTEX
{
	float4 position : SV_Position;
	float4 texcoord : TEXCOORD;
	float3 camera_dir : TEXCOORD1;
	float3 tangent : TEXCOORD2;
	float3 normal : TEXCOORD3;
	float3 binormal : TEXCOORD4;
	
	float4 color1 : TEXCOORD5;
	float4 color2 : TEXCOORD6;
	float4 color3 : TEXCOORD7;
	float3 color4 : TEXCOORD8;
	
	float4 extinction_factor : COLOR;
};

struct VS_OUTPUT_PER_VERTEX_COLOR
{
	float4 position : SV_Position;
	float2 texcoord : TEXCOORD;
	float3 vertex_color : TEXCOORD1;
	float3 camera_dir : TEXCOORD2;
	float3 normal : TEXCOORD3;
	float3 binormal : TEXCOORD4;
	float3 tangent : TEXCOORD5;
	
	float3 extinction_factor : COLOR;
	float3 sky_radiance : COLOR1;
};

struct VS_OUTPUT_PARTICLE
{
	// TODO: fix these names - the render_distortion structure has different fields
    float4 position : SV_Position;
    float4 color : COLOR; // float4 main_color
    float4 color2 : COLOR1; // float3 add_color, float depth ////// float2 tangent
    float4 texcoord : TEXCOORD;
    float4 parameters : TEXCOORD1;
    float4 normal : TEXCOORD2; // float3 normal, float palette_unknown ////// float2 binormal
};

struct VS_OUTPUT_FX
{
    float4 position : SV_Position;
    float4 color : COLOR;
    float4 color2 : COLOR1;
    float4 texcoord : TEXCOORD;
};

#endif