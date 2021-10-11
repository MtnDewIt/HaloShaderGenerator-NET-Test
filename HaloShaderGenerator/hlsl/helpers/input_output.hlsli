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

struct VS_OUTPUT_STATIC_PRT_FOLIAGE
{
    float4 position : SV_Position;
    float2 texcoord : TEXCOORD;
    float3 unknown_lighting_color : TEXCOORD1;
    float3 normal : TEXCOORD3;
    float3 binormal : TEXCOORD4;
    float3 tangent : TEXCOORD5;
    float3 camera_dir : TEXCOORD6;
    float4 prt_radiance_vector : TEXCOORD7;
    float3 extinction_factor : COLOR;
    float3 sky_radiance : COLOR1;
};

struct VS_OUTPUT_STATIC_PRT_TERRAIN
{
    float4 position : SV_Position;
    float2 texcoord : TEXCOORD;
    float3 camera_dir : TEXCOORD4;
    float4 prt_radiance_vector : TEXCOORD5;
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

struct VS_OUTPUT_STATIC_SH_FOLIAGE
{
    float4 position : SV_Position;
    float3 texcoord : TEXCOORD; //z is used for the angle of the dominant light dir?
    float3 unknown_lighting_color : TEXCOORD1;
    float3 normal : TEXCOORD3;
    float3 binormal : TEXCOORD4;
    float3 tangent : TEXCOORD5;
    float3 camera_dir : TEXCOORD6;
    float3 extinction_factor : COLOR;
    float3 sky_radiance : COLOR1;
};

struct VS_OUTPUT_STATIC_SH_TERRAIN
{
    float4 position : SV_Position;
    float3 texcoord : TEXCOORD; //z is used for the angle of the dominant light dir?
    float3 camera_dir : TEXCOORD4;
    float3 extinction_factor : COLOR;
    float3 sky_radiance : COLOR1;
};

struct VS_OUTPUT_ZONLY // doesnt look like this but ehh good enough
{
    float4 position : SV_Position;
    float2 texcoord : TEXCOORD;
    float4 normal_and_w : TEXCOORD1;
    float3 binormal : TEXCOORD2;
    float3 tangent : TEXCOORD3;
    float3 camera_dir : TEXCOORD4;
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

struct VS_OUTPUT_PER_PIXEL_FOLIAGE
{
    float4 position : SV_Position;
    float2 texcoord : TEXCOORD;
    float3 unknown_lighting_color : TEXCOORD1;
    float3 normal : TEXCOORD3;
    float3 binormal : TEXCOORD4;
    float3 tangent : TEXCOORD5;
    float2 lightmap_texcoord : TEXCOORD6_CENTROID;
    float3 camera_dir : TEXCOORD7;
    float3 extinction_factor : COLOR;
    float3 sky_radiance : COLOR1;
};

struct VS_OUTPUT_PER_PIXEL_TERRAIN
{
    float4 position : SV_Position;
    float2 texcoord : TEXCOORD;
    float2 lightmap_texcoord : TEXCOORD4_CENTROID;
    float3 camera_dir : TEXCOORD5;
    float3 extinction_factor : COLOR;
    float3 sky_radiance : COLOR1;
};

struct s_per_vertex_lightmap_coefficients
{
    float4 color1 : TEXCOORD5;
    float4 color2 : TEXCOORD6;
    float4 color3 : TEXCOORD7;
    float3 color4 : TEXCOORD8;
};

struct VS_OUTPUT_PER_VERTEX
{
	float4 position : SV_Position;
	float4 texcoord : TEXCOORD;
	float3 camera_dir : TEXCOORD1;
	float3 tangent : TEXCOORD2;
	float3 normal : TEXCOORD3;
	float3 binormal : TEXCOORD4;
    s_per_vertex_lightmap_coefficients lightmap_coefficients;
    float4 extinction_factor : COLOR;
};

struct VS_OUTPUT_PER_VERTEX_FOLIAGE
{
    float4 position : SV_Position;
    float4 texcoord : TEXCOORD;
    float3 camera_dir : TEXCOORD1;
    float3 tangent : TEXCOORD2;
    float3 normal : TEXCOORD3;
    float3 binormal : TEXCOORD4;
    s_per_vertex_lightmap_coefficients lightmap_coefficients;
    float4 extinction_factor : COLOR;
    float3 foliage_sky_radiance : COLOR1;
};

struct VS_OUTPUT_PER_VERTEX_TERRAIN
{
    float4 position : SV_Position;
    float4 texcoord : TEXCOORD;
    float3 camera_dir : TEXCOORD4;
    s_per_vertex_lightmap_coefficients lightmap_coefficients;
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
    float4 position : SV_Position;
    float4 color : COLOR;
    float4 parameters : COLOR1;
    float4 texcoord : TEXCOORD; // texcoord, next_texcoord
    float4 parameters2 : TEXCOORD1;
    float4 parameters3 : TEXCOORD2;
};

struct VS_OUTPUT_FX
{
    float4 position : SV_Position;
    float4 color : COLOR;
    float4 color2 : COLOR1;
    float4 texcoord : TEXCOORD;
};

struct VS_OUTPUT_DECAL
{
    float4 position : SV_Position;
    float4 texcoord : TEXCOORD;
    float depth : TEXCOORD1;
    float3 tangent : TEXCOORD2;
    float3 binormal : TEXCOORD3;
    float3 normal : TEXCOORD4;
};

struct VS_OUTPUT_DECAL_FLAT
{
    float4 position : SV_Position;
    float4 texcoord : TEXCOORD;
    float depth : TEXCOORD1;
};

struct PS_OUTPUT_DECAL_SIMPLE
{
    float4 color;
};

struct PS_OUTPUT_DECAL
{
    float4 color_ldr;
    float4 color_hdr;
    float4 unknown;
};

struct VS_OUTPUT_WATER
{
    float4 position : SV_Position;
    float4 unknown_0 : TEXCOORD;
    float4 unknown_1 : TEXCOORD1;
    float4 unknown_2 : TEXCOORD2;
    float4 unknown_3 : TEXCOORD3;
    float4 unknown_4 : TEXCOORD4;
    float4 unknown_5 : TEXCOORD5;
    float4 unknown_6 : TEXCOORD6;
    float4 unknown_7 : TEXCOORD7;
    float4 unknown_8 : TEXCOORD8;
    float4 unknown_9 : TEXCOORD9;
    float4 unknown_10 : TEXCOORD10;
    float4 unknown_11 : TEXCOORD11;
};

struct VS_OUTPUT_SCREEN
{
    float4 position : SV_Position;
    float4 texcoord : TEXCOORD;
};

#endif