#ifndef _FOLIAGE_TEMPLATE_ALBEDO_HLSLI
#define _FOLIAGE_TEMPLATE_ALBEDO_HLSLI

#include "..\helpers\input_output.hlsli"
#include "..\methods\albedo.hlsli"

PS_OUTPUT_ALBEDO foliage_entry_albedo(VS_OUTPUT_ALBEDO input)
{
    float4 albedo = calc_albedo_ps(input.texcoord, input.position.xy, input.normal.xyz, input.camera_dir);

    albedo.rgb = rgb_to_srgb(albedo.rgb);
    
    PS_OUTPUT_ALBEDO output;
    output.diffuse = albedo;
    output.normal = float4(normal_export(input.normal.xyz), albedo.w);
	
    if (blend_type_arg == k_blend_mode_opaque)
    {
        output.normal.w = 1.0;
        output.diffuse.w = 1.0;
    }
    
    output.unknown = input.normal.wwww;
    return output;
}

#endif