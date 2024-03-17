
#include "helpers\definition_helper.hlsli"
#include "helpers\input_output.hlsli"
#include "helpers\types.hlsli"
#include "registers\vertex_shader.hlsli"
#include "helpers\atmosphere.hlsli"

struct WATER_VERTEX_INPUT
{
    float4	pos1xyz_tc1x		: POSITION0;
    float4	tc1y_tan1xyz		: POSITION1;
    float4	bin1xyz_lm1x		: POSITION2;
    float4	lm1y_pos2xyz		: POSITION3;
    float4	tc2xy_tan2xy		: POSITION4;
    float4	tan2z_bin2xyz		: POSITION5;
    float4	lm2xy_pos3xy		: POSITION6;
    float4	pos3z_tc3xy_tan3x	: POSITION7;
    float4	tan3yz_bin3xy		: TEXCOORD0;
    float3	bin3z_lm3xy			: TEXCOORD1;
	float4	li1xyz_bt1x			: NORMAL0;
    float4	bt1yz_li2xy			: NORMAL1;
    float4	li2z_bt2xyz			: NORMAL2;
    float4	li3xyz_bt3x			: NORMAL3;
    float2	bt3yz			    : NORMAL4;
	float3	bc					: TEXCOORD2;
};

struct WATER_TESSELATION_PARAMETERS
{
    float3 position;
    float2 texcoord;
    float3 tangent;
    float3 binormal;
    float2 lm_tex;
    float3 local_info;
    float3 base_tex;
};

struct WATER_TESSELATED_VERTEX
{
    float4 position;
    float4 texcoord;
    float4 normal;
    float4 tangent;
    float4 binormal;
    float4 local_info;
    float4 base_tex;
    float4 lm_tex;
};

uniform float4 wave_displacement_array_xform;
uniform float time_warp;
uniform float wave_height;
uniform float4 wave_slope_array_xform;
uniform float time_warp_aux;
uniform float wave_height_aux;

uniform float displacement_range_x;
uniform float displacement_range_y;
uniform float displacement_range_z;

uniform float choppiness_forward;
uniform float choppiness_backward;
uniform float choppiness_side;

uniform float wave_visual_damping_distance;
uniform float globalshape_infuence_depth;

uniform int category_global_shape;
uniform int category_waveshape;

uniform sampler2D SAMPLER_0 : register(s0); // prob used somewhere
uniform sampler2D tex_ripple_buffer_slope_height : register(s1);
uniform sampler3D wave_displacement_array;
uniform sampler2D global_shape_texture;

uniform bool k_is_lightmap_exist : register(b0);
uniform bool k_is_water_interaction : register(b1);
uniform bool k_is_water_tessellated : register(b2);
uniform bool k_is_camera_underwater : register(b3);
uniform float k_water_index_offset : register(c131);
uniform float k_ripple_buffer_radius : register(c133);
uniform float2 k_ripple_buffer_center : register(c134);

uniform int category_reach_compatibility;
uniform float4 global_shape_texture_xform;

float4 bc_interp(float4 a, float4 b, float4 c, float3 weights)
{
	return a * weights.z + b * weights.y + c * weights.x;
}
float3 bc_interp(float3 a, float3 b, float3 c, float3 weights)
{
	return a * weights.z + b * weights.y + c * weights.x;
}
float2 bc_interp(float2 a, float2 b, float2 c, float3 weights)
{
	return a * weights.z + b * weights.y + c * weights.x;
}

float3 decompress_displacement(float3 displacement, float3 scale, float3 min, float height)
{
    displacement *= scale;
    displacement += min;
    displacement *= height;
    return displacement;
}

float3 apply_displacement_choppiness(float3 displacement, float choppiness_forward, float choppiness_backward, float choppiness_side)
{
    displacement.y *= choppiness_side;
    displacement.x *= (displacement.x < 0) ? choppiness_forward : choppiness_backward;
    return displacement;
}

void unpack_tesselation_parameters(
    in WATER_VERTEX_INPUT input,
    out WATER_TESSELATION_PARAMETERS tess1,
    out WATER_TESSELATION_PARAMETERS tess2,
    out WATER_TESSELATION_PARAMETERS tess3)
{
    tess1.position   = float3(input.pos1xyz_tc1x.x, input.pos1xyz_tc1x.y, input.pos1xyz_tc1x.z);
	tess1.texcoord   = float2(input.pos1xyz_tc1x.w, input.tc1y_tan1xyz.x);
	tess1.tangent    = float3(input.tc1y_tan1xyz.y, input.tc1y_tan1xyz.z, input.tc1y_tan1xyz.w);
    tess1.binormal   = float3(input.bin1xyz_lm1x.x, input.bin1xyz_lm1x.y, input.bin1xyz_lm1x.z);
    tess1.lm_tex     = float2(input.bin1xyz_lm1x.w, input.lm1y_pos2xyz.x);
	
    tess2.position   = float3(input.lm1y_pos2xyz.y,  input.lm1y_pos2xyz.z,  input.lm1y_pos2xyz.w);
	tess2.texcoord   = float2(input.tc2xy_tan2xy.x,  input.tc2xy_tan2xy.y);
	tess2.tangent    = float3(input.tc2xy_tan2xy.z,  input.tc2xy_tan2xy.w,  input.tan2z_bin2xyz.x);
    tess2.binormal   = float3(input.tan2z_bin2xyz.y, input.tan2z_bin2xyz.z, input.tan2z_bin2xyz.w);
	tess2.lm_tex     = float2(input.lm2xy_pos3xy.x,  input.lm2xy_pos3xy.y);
	
    tess3.position   = float3(input.lm2xy_pos3xy.z,      input.lm2xy_pos3xy.w,          input.pos3z_tc3xy_tan3x.x);
    tess3.texcoord   = float2(input.pos3z_tc3xy_tan3x.y, input.pos3z_tc3xy_tan3x.z);
	tess3.tangent    = float3(input.pos3z_tc3xy_tan3x.w, input.tan3yz_bin3xy.x,         input.tan3yz_bin3xy.y);
    tess3.binormal   = float3(input.tan3yz_bin3xy.z,     input.tan3yz_bin3xy.w,         input.bin3z_lm3xy.x);
	tess3.lm_tex     = float2(input.bin3z_lm3xy.y,       input.bin3z_lm3xy.z);

    tess1.local_info    = float3(input.li1xyz_bt1x.x, input.li1xyz_bt1x.y, input.li1xyz_bt1x.z);
    tess1.base_tex      = float3(input.li1xyz_bt1x.w, input.bt1yz_li2xy.x, input.bt1yz_li2xy.y);
    tess2.local_info    = float3(input.bt1yz_li2xy.z, input.bt1yz_li2xy.w, input.li2z_bt2xyz.x);
	tess2.base_tex      = float3(input.li2z_bt2xyz.y, input.li2z_bt2xyz.z, input.li2z_bt2xyz.w);
    tess3.local_info    = float3(input.li3xyz_bt3x.x, input.li3xyz_bt3x.y, input.li3xyz_bt3x.z);
	tess3.base_tex      = float3(input.li3xyz_bt3x.w, input.bt3yz.x, input.bt3yz.y);
}

WATER_TESSELATED_VERTEX get_vertex(WATER_VERTEX_INPUT input)
{
    WATER_TESSELATED_VERTEX output;

	float3 bc_weights = input.bc;

	WATER_TESSELATION_PARAMETERS tess1, tess2, tess3;
    unpack_tesselation_parameters(input, tess1, tess2, tess3);

	output.position =  float4(bc_interp(tess1.position, tess2.position, tess3.position, bc_weights), 1);
	output.texcoord =  float4(bc_interp(tess1.texcoord, tess2.texcoord, tess3.texcoord, bc_weights), 0, 0);
	output.tangent  =  float4(bc_interp(tess1.tangent,  tess2.tangent,  tess3.tangent,  bc_weights), 0);
	output.binormal = -float4(bc_interp(tess1.binormal, tess2.binormal, tess3.binormal, bc_weights), 0);
	output.lm_tex 	=  float4(bc_interp(tess1.lm_tex,   tess2.lm_tex,   tess3.lm_tex,   bc_weights), 0, 0);

	output.local_info  = float4(bc_interp(tess1.local_info, tess2.local_info, tess3.local_info, bc_weights), 0);
	output.base_tex    = float4(bc_interp(tess1.base_tex,   tess2.base_tex,   tess3.base_tex,   bc_weights), 0);

    output.tangent =    normalize(output.tangent);
    output.binormal =   normalize(output.binormal);
    output.normal.xyz = normalize(cross(output.tangent.xyz, output.binormal.xyz));
    output.normal.w =   0.0f;
    return output;
}

bool reach_compatibility_enabled()
{
#if APPLY_HLSL_FIXES == 1
    if (category_reach_compatibility == k_reach_compatibility_enabled)
        return true;
#endif
    return false;
}

VS_OUTPUT_WATER vs_water_out(WATER_TESSELATED_VERTEX input)
{
    float3 inc_pos = Camera_Position - input.position.xyz;

    float4 incident_ws = float4(inc_pos, length(inc_pos));
    incident_ws.xyz = normalize(incident_ws.xyz);

    float mip_level = max(incident_ws.w / wave_visual_damping_distance, 1.0f);

    float shape_height = 1.0f;
    float shape_choppiness = 1.0f;
    if (category_global_shape == k_global_shape_paint)
    {
        float2 global_shape_tex = input.base_tex.xy;
        
        if (reach_compatibility_enabled())
            global_shape_tex = global_shape_tex * global_shape_texture_xform.xy + global_shape_texture_xform.zw;

        float4 global_shape = tex2Dlod(global_shape_texture, float4(global_shape_tex, 0.0f, mip_level));
        shape_height = global_shape.x;
        shape_choppiness = global_shape.y;
    }
    else if (category_global_shape == k_global_shape_depth)
    {
        shape_height = saturate(input.local_info.y / globalshape_infuence_depth);
    }

    float2 ripple_tex = 0.0f;
    if (k_is_water_interaction)
    {
        ripple_tex = (input.position.xy - Camera_Position.xy) / k_ripple_buffer_radius;
        ripple_tex *= rsqrt(length(ripple_tex));
        ripple_tex += k_ripple_buffer_center;
        ripple_tex = saturate(ripple_tex * 0.5f + 0.5f);
    }

    float4 position = input.position;
    float displacement_height = 0.0f;
    float max_displacement_height = 1.0f;

    if (k_is_water_tessellated)
    {
        float3 displacement = 0.0f;
        if (category_waveshape == k_waveshape_default)
        {
            float3 displacement_scale = 2.0f;
            float3 displacement_min = -1.0f;
            
            if (!reach_compatibility_enabled())
            {
                displacement_scale = float3(displacement_range_x, displacement_range_y, displacement_range_z);
                displacement_min = -displacement_scale / 2.0f;
            }
			
            float4 displacement_tex = float4(input.texcoord.xy * wave_displacement_array_xform.xy + wave_displacement_array_xform.zw, time_warp, mip_level);
            float4 displacement_tex_aux = float4(input.texcoord.xy * wave_slope_array_xform.xy + wave_slope_array_xform.zw, time_warp_aux, mip_level);
			
            displacement = tex3Dlod(wave_displacement_array, displacement_tex).xyz;
            float3 displacement_aux = tex3Dlod(wave_displacement_array, displacement_tex_aux).xyz;
			
            displacement = decompress_displacement(displacement, displacement_scale, displacement_min, wave_height);
            displacement_aux = decompress_displacement(displacement_aux, displacement_scale, displacement_min, wave_height_aux);
            
            float wave_scale = sqrt(wave_displacement_array_xform.x * wave_displacement_array_xform.y);

            if (!reach_compatibility_enabled())
            {
                displacement /= wave_scale;
                displacement_aux /= wave_scale;
            }

            displacement = displacement + displacement_aux;

            displacement = apply_displacement_choppiness(displacement,
								choppiness_forward * shape_choppiness,
								choppiness_backward * shape_choppiness,
								choppiness_side * shape_choppiness);
            
            if (!reach_compatibility_enabled())
            {
                displacement *= input.local_info.x;
                max_displacement_height = 0.5f * input.local_info.x * displacement_range_z * (wave_height + wave_height_aux) / wave_scale;
            }

            displacement.z *= shape_height;
        }

        displacement_height = displacement.z;
	
        if (k_is_water_interaction)
        {
            float buffer_slope_height = tex2Dlod(tex_ripple_buffer_slope_height, float4(ripple_tex, 0, 0)).r;
            buffer_slope_height = buffer_slope_height * 2.0f - 1.0f;

            float base_ripple_scale = 1.0f / (1.0f + 3.0f * abs(buffer_slope_height));

            float height_additive = buffer_slope_height * 0.1f;
            height_additive *= min(input.local_info.y * 10 + 0.1f, 1.0f);

            displacement.z = displacement.z * base_ripple_scale + height_additive;
            displacement.xy *= base_ripple_scale;
        }

        float4 displacement_vec = input.tangent * displacement.x + input.binormal * displacement.y + input.normal * displacement.z;
        position += displacement_vec;
        position.w = 1.0f;
    }

    float3 extinction_factor;
    float3 sky_radiance;
    calculate_atmosphere_radiance_new(Camera_Position, position.xyz, extinction_factor, sky_radiance);

    VS_OUTPUT_WATER output;

    output.position = mul(position, View_Projection);
    output.position_ss = output.position;

    output.texcoord = float4(input.texcoord.xyz, mip_level);
    output.tangent__height_scale.xyz = input.tangent.xyz;
    output.binormal__height_scale_aux.xyz = input.binormal.xyz;

    output.incident_ws__view_dist = incident_ws;
    output.position_ws__water_depth.xyz = position.xyz;

    output.tangent__height_scale.w = sqrt(shape_height * wave_height);
    output.binormal__height_scale_aux.w = sqrt(shape_height * wave_height_aux);
    output.position_ws__water_depth.w = input.local_info.y;

    output.base_tex = float4(input.base_tex.xy, displacement_height * 10.0f, max_displacement_height * 10.0f);
    output.lm_tex = float4(input.lm_tex.xy, ripple_tex);

    output.extinction_factor = float4(extinction_factor, 0.0f);
    if (reach_compatibility_enabled())
        output.extinction_factor.w = 1.0f / max(incident_ws.w, 0.01f);
    output.sky_radiance = float4(sky_radiance, 0.0f);

    return output;
}

VS_OUTPUT_WATER entry_static_per_pixel(WATER_VERTEX_INPUT input)
{
    WATER_TESSELATED_VERTEX water_vert = get_vertex(input);
    return vs_water_out(water_vert);
}

VS_OUTPUT_WATER entry_static_per_vertex(WATER_VERTEX_INPUT input)
{
    WATER_TESSELATED_VERTEX water_vert = get_vertex(input);
    return vs_water_out(water_vert);
}

float4 entry_water_tessellation() : SV_Position
{
    return 0;
}
