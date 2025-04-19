using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Shader
{
    public class ShaderGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(ShaderMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((ShaderMethods)methodIndex)
            {
                case ShaderMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case ShaderMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping)).Length;
                case ShaderMethods.Alpha_Test:
                    return Enum.GetValues(typeof(Alpha_Test)).Length;
                case ShaderMethods.Specular_Mask:
                    return Enum.GetValues(typeof(Specular_Mask)).Length;
                case ShaderMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model)).Length;
                case ShaderMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping)).Length;
                case ShaderMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination)).Length;
                case ShaderMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
                case ShaderMethods.Parallax:
                    return Enum.GetValues(typeof(Parallax)).Length;
                case ShaderMethods.Misc:
                    return Enum.GetValues(typeof(Misc)).Length;
                case ShaderMethods.Distortion:
                    return Enum.GetValues(typeof(Distortion)).Length;
                case ShaderMethods.Soft_Fade:
                    return Enum.GetValues(typeof(Soft_Fade)).Length;
                case ShaderMethods.Misc_Attr_Animation:
                    return Enum.GetValues(typeof(Misc_Attr_Animation)).Length;
                case ShaderMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness)).Length;
                case ShaderMethods.Alpha_Blend_Source:
                    return Enum.GetValues(typeof(Alpha_Blend_Source)).Length;
            }

            return -1;
        }

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint) 
        {
            switch (entryPoint)
            {
                case ShaderStage.Shadow_Generate:
                case ShaderStage.Dynamic_Light_Cinematic:
                    return 2;
                default:
                    return -1;
            }
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Shadow_Generate:
                case ShaderStage.Dynamic_Light_Cinematic:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsPixelShaderShared(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Shadow_Generate:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsAutoMacro()
        {
            return false;
        }

        public ShaderParameters GetGlobalParameters(out string rmopName)
        {
            var result = new ShaderParameters();

            result.AddSamplerExternParameter("albedo_texture", RenderMethodExtern.texture_global_target_texaccum);
            result.AddSamplerExternParameter("normal_texture", RenderMethodExtern.texture_global_target_normal);
            result.AddSamplerExternFilterParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture, ShaderOptionParameter.ShaderFilterMode.Bilinear);
            result.AddSamplerExternFilterAddressParameter("shadow_depth_map_1", RenderMethodExtern.texture_global_target_shadow_buffer1, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp);
            result.AddSamplerExternParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0);
            result.AddFloat3ColorExternWithFloatAndIntegerParameter("debug_tint", RenderMethodExtern.debug_tint, 1.0f, 1, new ShaderColor(255, 255, 255, 255));
            result.AddSamplerExternParameter("active_camo_distortion_texture", RenderMethodExtern.active_camo_distortion_texture);
            result.AddSamplerExternParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerExternParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture);
            result.AddSamplerExternParameter("dominant_light_intensity_map", RenderMethodExtern.texture_dominant_light_intensity_map);
            //result.AddSamplerExternAddressParameter("g_diffuse_power_specular", RenderMethodExtern.material_diffuse_power, ShaderOptionParameter.ShaderAddressMode.Clamp);
            //result.AddSamplerFilterAddressParameter("g_direction_lut", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp);
            //result.AddSamplerFilterAddressParameter("g_sample_vmf_diffuse_vs", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp);
            //result.AddSamplerFilterAddressParameter("g_sample_vmf_diffuse", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp);
            //result.AddSamplerExternFilterAddressParameter("g_sample_vmf_phong_specular", RenderMethodExtern.material_diffuse_power, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp);
            //result.AddSamplerExternFilterAddressParameter("shadow_mask_texture", RenderMethodExtern.none, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp); // rmExtern - texture_global_target_shadow_mask
            rmopName = @"shaders\shader_options\global_shader_options";

            return result;
        }

        public ShaderParameters GetParametersInOption(string methodName, int option, out string rmopName, out string optionName)
        {
            ShaderParameters result = new ShaderParameters();
            rmopName = null;
            optionName = null;

            if (methodName == "albedo")
            {
                optionName = ((Albedo)option).ToString();

                switch ((Albedo)option)
                {
                    case Albedo.Default:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        rmopName = @"shaders\shader_options\albedo_default";
                        break;
                    case Albedo.Detail_Blend:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map2", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        //result.AddFloatWithColorParameter("blend_alpha", new ShaderColor(255, 255, 255, 255), 1.0f); // Breaks shader recompilation
                        rmopName = @"shaders\shader_options\albedo_detail_blend";
                        break;
                    case Albedo.Constant_Color:
                        result.AddFloat4ColorParameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_constant";
                        break;
                    case Albedo.Two_Change_Color:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("change_color_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        //result.AddSamplerParameter("camouflage_change_color_map", @"rasterizer\invalid");
                        //result.AddFloatParameter("camouflage_scale");
                        rmopName = @"shaders\shader_options\albedo_two_change_color";
                        break;
                    case Albedo.Four_Change_Color:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("change_color_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("quaternary_change_color", RenderMethodExtern.object_change_color_quaternary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        //result.AddSamplerParameter("camouflage_change_color_map");
                        //result.AddFloatParameter("camouflage_scale");
                        rmopName = @"shaders\shader_options\albedo_four_change_color";
                        break;
                    case Albedo.Three_Detail_Blend:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map2", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map3", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        //result.AddFloatWithColorParameter("blend_alpha", new ShaderColor(255, 255, 255, 255), 1.0f); // Breaks shader recompilation
                        rmopName = @"shaders\shader_options\albedo_three_detail_blend";
                        break;
                    case Albedo.Two_Detail_Overlay:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map2", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map_overlay", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_detail_overlay";
                        break;
                    case Albedo.Two_Detail:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map2", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_detail";
                        break;
                    case Albedo.Color_Mask:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("color_mask_map", @"shaders\default_bitmaps\bitmaps\reference_grids");
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color2", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color3", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat3ColorParameter("neutral_gray", new ShaderColor(255, 255, 255, 255));
                        rmopName = @"shaders\shader_options\albedo_color_mask";
                        break;
                    case Albedo.Two_Detail_Black_Point:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map2", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_detail_black_point";
                        break;
                    case Albedo.Two_Change_Color_Anim_Overlay:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("change_color_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternParameter("primary_change_color_anim", RenderMethodExtern.object_change_color_primary_anim);
                        result.AddFloat3ColorExternParameter("secondary_change_color_anim", RenderMethodExtern.object_change_color_secondary_anim);
                        rmopName = @"shaders\shader_options\albedo_two_change_color_anim";
                        break;
                    case Albedo.Chameleon:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat3ColorParameter("chameleon_color0", new ShaderColor(0, 255, 0, 255));
                        result.AddFloat3ColorParameter("chameleon_color1", new ShaderColor(0, 0, 255, 255));
                        result.AddFloat3ColorParameter("chameleon_color2", new ShaderColor(0, 255, 255, 0));
                        result.AddFloat3ColorParameter("chameleon_color3", new ShaderColor(0, 255, 0, 0));
                        result.AddFloatParameter("chameleon_color_offset1", 0.3333f);
                        result.AddFloatParameter("chameleon_color_offset2", 0.6666f);
                        result.AddFloatParameter("chameleon_fresnel_power", 2.0f);
                        rmopName = @"shaders\shader_options\albedo_chameleon";
                        break;
                    case Albedo.Two_Change_Color_Chameleon:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("change_color_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorParameter("primary_change_color_anim");
                        result.AddFloat3ColorParameter("secondary_change_color_anim");
                        result.AddFloat3ColorParameter("chameleon_color0", new ShaderColor(0, 255, 0, 255));
                        result.AddFloat3ColorParameter("chameleon_color1", new ShaderColor(0, 0, 255, 255));
                        result.AddFloat3ColorParameter("chameleon_color2", new ShaderColor(0, 255, 255, 0));
                        result.AddFloat3ColorParameter("chameleon_color3", new ShaderColor(0, 255, 0, 0));
                        result.AddFloatParameter("chameleon_color_offset1", 0.3333f);
                        result.AddFloatParameter("chameleon_color_offset2", 0.6666f);
                        result.AddFloatParameter("chameleon_fresnel_power", 2.0f);
                        rmopName = @"shaders\shader_options\albedo_two_change_color_chameleon";
                        break;
                    case Albedo.Chameleon_Masked:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("chameleon_mask_map", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloat3ColorParameter("chameleon_color0", new ShaderColor(0, 255, 0, 255));
                        result.AddFloat3ColorParameter("chameleon_color1", new ShaderColor(0, 0, 255, 255));
                        result.AddFloat3ColorParameter("chameleon_color2", new ShaderColor(0, 255, 255, 0));
                        result.AddFloat3ColorParameter("chameleon_color3", new ShaderColor(0, 255, 0, 0));
                        result.AddFloatParameter("chameleon_color_offset1", 0.3333f);
                        result.AddFloatParameter("chameleon_color_offset2", 0.6666f);
                        result.AddFloatParameter("chameleon_fresnel_power", 2.0f);
                        rmopName = @"shaders\shader_options\albedo_chameleon_masked";
                        break;
                    case Albedo.Color_Mask_Hard_Light:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("color_mask_map", @"shaders\default_bitmaps\bitmaps\reference_grids");
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        rmopName = @"shaders\shader_options\albedo_color_mask_hard_light";
                        break;
                    case Albedo.Four_Change_Color_Applying_To_Specular:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("change_color_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("quaternary_change_color", RenderMethodExtern.object_change_color_quaternary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        //result.AddSamplerParameter("camouflage_change_color_map");
                        //result.AddFloatParameter("camouflage_scale");
                        rmopName = @"shaders\shader_options\albedo_four_change_color";
                        break;
                    case Albedo.Simple:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        rmopName = @"shaders\shader_options\albedo_simple";
                        break;
                    case Albedo.Two_Change_Color_Tex_Overlay:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("change_color_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("secondary_change_color_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\albedo_two_change_color_tex_overlay";
                        break;
                    case Albedo.Chameleon_Albedo_Masked:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat4ColorParameter("albedo_color", new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerParameter("base_masked_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat4ColorParameter("albedo_masked_color", new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerParameter("chameleon_mask_map", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloat3ColorParameter("chameleon_color0", new ShaderColor(0, 255, 0, 255));
                        result.AddFloat3ColorParameter("chameleon_color1", new ShaderColor(0, 0, 255, 255));
                        result.AddFloat3ColorParameter("chameleon_color2", new ShaderColor(0, 255, 255, 0));
                        result.AddFloat3ColorParameter("chameleon_color3", new ShaderColor(0, 255, 0, 0));
                        result.AddFloatParameter("chameleon_color_offset1", 0.3333f);
                        result.AddFloatParameter("chameleon_color_offset2", 0.6666f);
                        result.AddFloatParameter("chameleon_fresnel_power", 2.0f);
                        rmopName = @"shaders\shader_options\albedo_chameleon_albedo_masked";
                        break;
                    case Albedo.Custom_Cube:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("custom_cube", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        rmopName = @"shaders\shader_options\albedo_custom_cube";
                        break;
                    case Albedo.Two_Color:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerWithScaleParameter("blend_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_second_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        rmopName = @"shaders\shader_options\albedo_two_color";
                        break;
                    case Albedo.Emblem:
                        result.AddSamplerExternParameter("emblem_map", RenderMethodExtern.emblem_player_shoulder_texture);
                        rmopName = @"shaders\shader_options\albedo_emblem";
                        break;
                    case Albedo.Scrolling_Cube_Mask:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerWithScaleParameter("color_blend_mask_cubemap", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_second_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        rmopName = @"shaders\shader_options\albedo_scrolling_cube_mask";
                        break;
                    case Albedo.Scrolling_Cube:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("color_cubemap", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\albedo_scrolling_cube";
                        break;
                    case Albedo.Scrolling_Texture_Uv:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("color_texture", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatParameter("u_speed", 1.0f);
                        result.AddFloatParameter("v_speed");
                        rmopName = @"shaders\shader_options\albedo_scrolling_texture_uv";
                        break;
                    case Albedo.Texture_From_Misc:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("color_texture", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\albedo_texture_from_misc";
                        break;
                }
            }

            if (methodName == "bump_mapping")
            {
                optionName = ((Bump_Mapping)option).ToString();

                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Off:
                        rmopName = @"shaders\shader_options\bump_off";
                        break;
                    case Bump_Mapping.Standard:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\bump_default";
                        break;
                    case Bump_Mapping.Detail:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddFloatParameter("bump_detail_coefficient", 1.0f);
                        rmopName = @"shaders\shader_options\bump_detail";
                        break;
                    case Bump_Mapping.Detail_Masked:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_detail_mask_map", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloatParameter("bump_detail_coefficient", 1.0f);
                        result.AddBooleanParameter("invert_mask");
                        rmopName = @"shaders\shader_options\bump_detail_masked";
                        break;
                    case Bump_Mapping.Detail_Plus_Detail_Masked:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_detail_mask_map", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerParameter("bump_detail_masked_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddFloatParameter("bump_detail_coefficient", 1.0f);
                        result.AddFloatParameter("bump_detail_masked_coefficient", 1.0f);
                        rmopName = @"shaders\shader_options\bump_detail_plus_detail_masked";
                        break;
                    case Bump_Mapping.Detail_Unorm:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddFloatParameter("bump_detail_coefficient", 1.0f);
                        rmopName = @"shaders\shader_options\bump_detail_unorm";
                        break;
                    case Bump_Mapping.Detail_Blend:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map2", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddFloatWithColorParameter("blend_alpha", new ShaderColor(255, 255, 255, 255), 1.0f);
                        rmopName = @"shaders\shader_options\bump_detail_blend";
                        break;
                    case Bump_Mapping.Three_Detail_Blend:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map2", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map3", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddFloatWithColorParameter("blend_alpha", new ShaderColor(255, 255, 255, 255), 1.0f);
                        rmopName = @"shaders\shader_options\bump_three_detail_blend";
                        break;
                    case Bump_Mapping.Standard_Wrinkle:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("wrinkle_normal", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("wrinkle_mask_a", @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddSamplerParameter("wrinkle_mask_b", @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddFloat4ColorExternParameter("wrinkle_weights_a", RenderMethodExtern.none); // rmExtern - wrinkle_weights_a
                        result.AddFloat4ColorExternParameter("wrinkle_weights_b", RenderMethodExtern.none); // rmExtern - wrinkle_weights_b
                        rmopName = @"shaders\shader_options\bump_default_wrinkle";
                        break;
                    case Bump_Mapping.Detail_Wrinkle:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("wrinkle_normal", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("wrinkle_mask_a", @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddSamplerParameter("wrinkle_mask_b", @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddFloat4ColorExternParameter("wrinkle_weights_a", RenderMethodExtern.none); // rmExtern - wrinkle_weights_a
                        result.AddFloat4ColorExternParameter("wrinkle_weights_b", RenderMethodExtern.none); // rmExtern - wrinkle_weights_b
                        rmopName = @"shaders\shader_options\bump_detail_wrinkle";
                        break;
                }
            }

            if (methodName == "alpha_test")
            {
                optionName = ((Alpha_Test)option).ToString();

                switch ((Alpha_Test)option)
                {
                    case Alpha_Test.None:
                        rmopName = @"shaders\shader_options\alpha_test_off";
                        break;
                    case Alpha_Test.Simple:
                        result.AddSamplerParameter("alpha_test_map", @"shaders\default_bitmaps\bitmaps\default_alpha_test");
                        rmopName = @"shaders\shader_options\alpha_test_on";
                        break;
                }
            }

            if (methodName == "specular_mask")
            {
                optionName = ((Specular_Mask)option).ToString();

                switch ((Specular_Mask)option)
                {
                    case Specular_Mask.No_Specular_Mask:
                        break;
                    case Specular_Mask.Specular_Mask_From_Diffuse:
                        break;
                    case Specular_Mask.Specular_Mask_From_Texture:
                        result.AddSamplerWithFloatParameter("specular_mask_texture", 25.0f, @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\shader_options\specular_mask_from_texture";
                        break;
                    case Specular_Mask.Specular_Mask_From_Color_Texture:
                        result.AddSamplerWithFloatParameter("specular_mask_texture", 25.0f, @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\shader_options\specular_mask_from_texture";
                        break;
                    case Specular_Mask.Specular_Mask_Mult_Diffuse:
                        result.AddSamplerWithFloatParameter("specular_mask_texture", 25.0f, @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\shader_options\specular_mask_mult_diffuse";
                        break;
                }
            }

            if (methodName == "material_model")
            {
                optionName = ((Material_Model)option).ToString();

                switch ((Material_Model)option)
                {
                    case Material_Model.Diffuse_Only:
                        result.AddBooleanParameter("no_dynamic_lights");
                        //result.AddFloatParameter("approximate_specular_type");
                        rmopName = @"shaders\shader_options\material_diffuse_only";
                        break;
                    case Material_Model.Cook_Torrance_Rim_Fresnel:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat3ColorParameter("fresnel_color", new ShaderColor(1, 128, 128, 128));
                        result.AddBooleanParameter("use_fresnel_color_environment");
                        result.AddFloat3ColorParameter("fresnel_color_environment", new ShaderColor(0, 128, 128, 128));
                        result.AddFloatParameter("fresnel_power", 1.0f);
                        result.AddFloatParameter("roughness", 0.4f);
                        result.AddFloatParameter("area_specular_contribution", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution", 0.5f);
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerExternParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerExternParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerExternParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddBooleanParameter("albedo_blend_with_specular_tint");
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddFloatParameter("rim_fresnel_coefficient");
                        result.AddFloat3ColorParameter("rim_fresnel_color", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("rim_fresnel_power", 2.0f);
                        result.AddFloatParameter("rim_fresnel_albedo_blend");
                        rmopName = @"shaders\shader_options\material_cook_torrance_rim_fresnel";
                        break;
                    case Material_Model.Two_Lobe_Phong:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloatParameter("specular_coefficient", 1.0f);
                        result.AddFloatParameter("normal_specular_power", 10.0f);
                        result.AddFloat3ColorParameter("normal_specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("glancing_specular_power", 10.0f);
                        result.AddFloat3ColorParameter("glancing_specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness", 5.0f);
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution", 0.1f);
                        result.AddFloatParameter("environment_map_specular_contribution", 0.1f);
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("albedo_specular_tint_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        //result.AddFloat3ColorParameter("specular_color_by_angle", new ShaderColor(0, 255, 255, 255));
                        //result.AddFloatParameter("roughness");
                        //result.AddFloatParameter("analytical_roughness", 0.02f);
                        //result.AddFloatParameter("approximate_specular_type");
                        //result.AddFloatParameter("analytical_power", 25f);
                        rmopName = @"shaders\shader_options\material_two_lobe_phong_option";
                        break;
                    case Material_Model.Foliage:
                        result.AddBooleanParameter("no_dynamic_lights");
                        rmopName = @"shaders\shader_options\material_foliage";
                        break;
                    case Material_Model.None:
                        break;
                    case Material_Model.Glass:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloatParameter("fresnel_coefficient", 0.1f);
                        result.AddFloatParameter("fresnel_curve_steepness", 5.0f);
                        result.AddFloatParameter("fresnel_curve_bias");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddBooleanParameter("no_dynamic_lights");
                        rmopName = @"shaders\shader_options\glass_material";
                        break;
                    case Material_Model.Organism:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloat3ColorParameter("diffuse_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("analytical_specular_coefficient");
                        result.AddFloatParameter("area_specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("specular_power", 10.0f);
                        result.AddSamplerParameter("specular_map", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloatParameter("environment_map_coefficient");
                        result.AddFloat3ColorParameter("environment_map_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness", 5.0f);
                        result.AddFloatParameter("rim_coefficient", 1.0f);
                        result.AddFloat3ColorParameter("rim_tint");
                        result.AddFloatParameter("rim_power", 2.0f);
                        result.AddFloatParameter("rim_start", 0.7f);
                        result.AddFloatParameter("rim_maps_transition_ratio");
                        result.AddFloatParameter("ambient_coefficient");
                        result.AddFloat3ColorParameter("ambient_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddSamplerParameter("occlusion_parameter_map", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloatParameter("subsurface_coefficient");
                        result.AddFloat3ColorParameter("subsurface_tint");
                        result.AddFloatParameter("subsurface_propagation_bias");
                        result.AddFloatParameter("subsurface_normal_detail");
                        result.AddSamplerParameter("subsurface_map", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloatParameter("transparence_coefficient");
                        result.AddFloat3ColorParameter("transparence_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("transparence_normal_bias");
                        result.AddFloatParameter("transparence_normal_detail");
                        result.AddSamplerParameter("transparence_map", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloat3ColorParameter("final_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddBooleanParameter("no_dynamic_lights");
                        //result.AddFloat3ColorParameter("specular_color_by_angle", new ShaderColor(0, 255, 255, 255));
                        //result.AddFloatParameter("specular_coefficient", 1.0f);
                        //result.AddFloat3ColorParameter("fresnel_color", new ShaderColor(1, 128, 128, 128));
                        //result.AddFloatParameter("area_specular_contribution", 0.16f);
                        //result.AddFloatParameter("analytical_specular_contribution", 0.5f);
                        //result.AddFloatParameter("environment_map_specular_contribution");
                        //result.AddFloatParameter("roughness", 0.04f);
                        //result.AddFloatParameter("analytical_roughness", 0.5f);
                        //result.AddBooleanParameter("use_material_texture");
                        //result.AddSamplerParameter("material_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        //result.AddFloatParameter("material_texture_black_specular_multiplier", 1.0f);
                        //result.AddFloatParameter("material_texture_black_roughness", 1.0f);
                        //result.AddFloatParameter("albedo_blend");
                        //result.AddSamplerFilterAddressParameter("g_sampler_cooktorran_array", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\color_white");
                        //result.AddFloatParameter("approximate_specular_type");
                        //result.AddFloatParameter("rim_width", 0.3f);
                        rmopName = @"shaders\shader_options\material_organism_option";
                        break;
                    case Material_Model.Single_Lobe_Phong:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddFloat3ColorParameter("specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        rmopName = @"shaders\shader_options\single_lobe_phong";
                        break;
                    case Material_Model.Car_Paint:
                        result.AddBooleanParameter("use_material_texture0");
                        result.AddBooleanParameter("use_material_texture1");
                        result.AddSamplerParameter("material_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerExternParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerExternParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerExternParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddSamplerFilterWithScaleParameter("bump_detail_map0", ShaderOptionParameter.ShaderFilterMode.Anisotropic4Expensive, 16.0f, @"shaders\default_bitmaps\bitmaps\sparklenoisemap");
                        result.AddFloatParameter("bump_detail_map0_blend_factor", 0.75f);
                        result.AddFloatParameter("diffuse_coefficient0", 0.25f);
                        result.AddFloatParameter("specular_coefficient0", 1.0f);
                        result.AddFloat3ColorParameter("specular_tint0", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat3ColorParameter("fresnel_color0", new ShaderColor(0, 128, 128, 128));
                        result.AddFloatParameter("fresnel_power0", 1.0f);
                        result.AddFloatParameter("albedo_blend0");
                        result.AddFloatParameter("roughness0", 0.5f);
                        result.AddFloatParameter("area_specular_contribution0", 0.3f);
                        result.AddFloatParameter("analytical_specular_contribution0", 0.5f);
                        result.AddBooleanParameter("order3_area_specular0");
                        result.AddFloatParameter("diffuse_coefficient1");
                        result.AddFloatParameter("specular_coefficient1", 0.15f);
                        result.AddFloat3ColorParameter("specular_tint1", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat3ColorParameter("fresnel_color1", new ShaderColor(0, 128, 128, 128));
                        result.AddFloat3ColorParameter("fresnel_color_environment1", new ShaderColor(0, 128, 128, 128));
                        result.AddFloatParameter("fresnel_power1", 1.0f);
                        result.AddFloatParameter("albedo_blend1");
                        result.AddFloatParameter("roughness1", 0.1f);
                        result.AddFloatParameter("area_specular_contribution1", 0.1f);
                        result.AddFloatParameter("analytical_specular_contribution1", 0.2f);
                        result.AddFloatParameter("environment_map_specular_contribution1", 0.3f);
                        result.AddBooleanParameter("order3_area_specular1");
                        result.AddFloatParameter("rim_fresnel_coefficient1");
                        result.AddFloat3ColorParameter("rim_fresnel_color1", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("rim_fresnel_power1", 2.0f);
                        result.AddFloatParameter("rim_fresnel_albedo_blend1");
                        rmopName = @"shaders\shader_options\material_car_paint_option";
                        break;
                    case Material_Model.Hair:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloat3ColorParameter("diffuse_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("analytical_specular_coefficient");
                        result.AddFloatParameter("area_specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("specular_power", 10.0f);
                        result.AddSamplerParameter("specular_map", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerParameter("specular_shift_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        result.AddSamplerParameter("specular_noise_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        result.AddFloatParameter("environment_map_coefficient");
                        result.AddFloat3ColorParameter("environment_map_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat3ColorParameter("final_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddBooleanParameter("no_dynamic_lights");
                        //result.AddFloatParameter("roughness", 0.5f);
                        rmopName = @"shaders\shader_options\material_hair_option";
                        break;
                    case Material_Model.Cook_Torrance:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloatParameter("specular_coefficient", 1.0f);
                        result.AddFloat3ColorParameter("specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat3ColorParameter("fresnel_color", new ShaderColor(1, 128, 128, 128));
                        result.AddFloatParameter("roughness", 0.04f);
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution", 0.5f);
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerExternParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerExternParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerExternParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        //result.AddFloat3ColorParameter("specular_color_by_angle", new ShaderColor(0, 255, 255, 255));
                        //result.AddFloatParameter("fresnel_curve_steepness", 5.0f);
                        //result.AddFloatParameter("analytical_roughness", 0.5f);
                        //result.AddFloatParameter("material_texture_black_specular_multiplier", 1.0f);
                        //result.AddFloatParameter("material_texture_black_roughness", 1.0f);
                        //result.AddFloatParameter("approximate_specular_type");
                        rmopName = @"shaders\shader_options\material_cook_torrance_option";
                        break;
                    case Material_Model.Cook_Torrance_Pbr_Maps:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat3ColorParameter("fresnel_color", new ShaderColor(1, 128, 128, 128));
                        result.AddFloatParameter("roughness", 0.4f);
                        result.AddFloatParameter("area_specular_contribution", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution", 0.5f);
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerExternParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerExternParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerExternParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddSamplerParameter("spec_tint_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_cook_torrance_pbr_maps_option";
                        break;
                    case Material_Model.Two_Lobe_Phong_Tint_Map:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloatParameter("normal_specular_power", 10.0f);
                        result.AddFloat3ColorParameter("normal_specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("glancing_specular_power", 10.0f);
                        result.AddFloat3ColorParameter("glancing_specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness", 5.0f);
                        result.AddFloatParameter("area_specular_contribution", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution", 0.5f);
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("albedo_specular_tint_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddSamplerParameter("normal_specular_tint_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("glancing_specular_tint_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_two_lobe_phong_tint_map_option";
                        break;
                    case Material_Model.Cook_Torrance_Reach:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloat3ColorParameter("specular_color_by_angle", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("specular_coefficient", 1.0f);
                        result.AddFloat3ColorParameter("specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat3ColorParameter("fresnel_color", new ShaderColor(1, 128, 128, 128));
                        result.AddFloatParameter("fresnel_curve_steepness", 5.0f);
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution", 0.5f);
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddFloatParameter("roughness", 0.04f);
                        result.AddFloatParameter("analytical_roughness", 0.5f);
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatParameter("material_texture_black_specular_multiplier", 1.0f);
                        result.AddFloatParameter("material_texture_black_roughness", 1.0f);
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("approximate_specular_type");
                        result.AddSamplerExternParameter("g_diffuse_power_specular", RenderMethodExtern.material_diffuse_power);
                        result.AddSamplerExternParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerExternParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerExternParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        rmopName = @"shaders\shader_options\material_cook_torrance_option_reach";
                        break;
                    case Material_Model.Two_Lobe_Phong_Reach:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloat3ColorParameter("specular_color_by_angle", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("specular_coefficient", 1.0f);
                        result.AddFloatParameter("normal_specular_power", 10.0f);
                        result.AddFloat3ColorParameter("normal_specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("glancing_specular_power", 10.0f);
                        result.AddFloat3ColorParameter("glancing_specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness", 5.0f);
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution", 0.1f);
                        result.AddFloatParameter("environment_map_specular_contribution", 0.1f);
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("analytical_roughness", 0.02f);
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("albedo_specular_tint_blend");
                        result.AddFloatParameter("approximate_specular_type");
                        result.AddFloatParameter("analytical_power", 25.0f);
                        result.AddSamplerExternParameter("g_diffuse_power_specular", RenderMethodExtern.material_diffuse_power);
                        rmopName = @"shaders\shader_options\material_two_lobe_phong_option_reach";
                        break;
                    case Material_Model.Cook_Torrance_Custom_Cube:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat3ColorParameter("fresnel_color", new ShaderColor(1, 128, 128, 128));
                        result.AddFloatParameter("roughness", 0.4f);
                        result.AddFloatParameter("area_specular_contribution", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution", 0.5f);
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerExternParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerExternParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerExternParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddSamplerParameter("custom_cube", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_cook_torrance_custom_cube_option";
                        break;
                    case Material_Model.Cook_Torrance_Two_Color_Spec_Tint:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat3ColorParameter("fresnel_color", new ShaderColor(1, 128, 128, 128));
                        result.AddFloatParameter("roughness", 0.4f);
                        result.AddFloatParameter("area_specular_contribution", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution", 0.5f);
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerExternParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerExternParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerExternParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddSamplerParameter("spec_blend_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorParameter("specular_second_tint", new ShaderColor(0, 255, 255, 255));
                        rmopName = @"shaders\shader_options\material_cook_torrance_two_color_spec_tint";
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube_Mask:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat3ColorParameter("fresnel_color", new ShaderColor(1, 128, 128, 128));
                        result.AddFloatParameter("roughness", 0.4f);
                        result.AddFloatParameter("area_specular_contribution", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution", 0.5f);
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerExternParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerExternParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerExternParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddSamplerParameter("tint_blend_mask_cubemap", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorParameter("specular_second_tint", new ShaderColor(0, 255, 255, 255));
                        rmopName = @"shaders\shader_options\material_cook_torrance_scrolling_cube_mask";
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("fresnel_color", new ShaderColor(1, 128, 128, 128));
                        result.AddFloatParameter("roughness", 0.4f);
                        result.AddFloatParameter("area_specular_contribution", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution", 0.5f);
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerExternParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerExternParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerExternParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddSamplerParameter("spec_tint_cubemap", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_cook_torrance_scrolling_cube";
                        break;
                    case Material_Model.Cook_Torrance_From_Albedo:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("fresnel_color", new ShaderColor(1, 128, 128, 128));
                        result.AddFloatParameter("roughness", 0.4f);
                        result.AddFloatParameter("area_specular_contribution", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution", 0.5f);
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerExternParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerExternParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerExternParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        rmopName = @"shaders\shader_options\material_cook_torrance_from_albedo";
                        break;
                }
            }

            if (methodName == "environment_mapping")
            {
                optionName = ((Environment_Mapping)option).ToString();

                switch ((Environment_Mapping)option)
                {
                    case Environment_Mapping.None:
                        break;
                    case Environment_Mapping.Per_Pixel:
                        result.AddSamplerAddressWithColorParameter("environment_map", ShaderOptionParameter.ShaderAddressMode.Clamp, new ShaderColor(0, 255, 255, 255), @"shaders\default_bitmaps\bitmaps\default_dynamic_cube_map");
                        result.AddFloat3ColorParameter("env_tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("env_roughness_scale", 1.0f);
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.Dynamic:
                        result.AddFloat3ColorParameter("env_tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddSamplerExternAddressParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddSamplerExternAddressParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddFloatParameter("env_roughness_scale", 1.0f);
                        //result.AddFloatParameter("env_roughness_offset", 0.5f); // Breaks shader recompilation
                        rmopName = @"shaders\shader_options\env_map_dynamic";
                        break;
                    case Environment_Mapping.From_Flat_Texture:
                        result.AddSamplerAddressWithColorParameter("flat_environment_map", ShaderOptionParameter.ShaderAddressMode.BlackBorder, new ShaderColor(0, 255, 255, 255), @"shaders\default_bitmaps\bitmaps\color_red");
                        result.AddFloat3ColorParameter("env_tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat4ColorExternParameter("flat_envmap_matrix_x", RenderMethodExtern.flat_envmap_matrix_x);
                        result.AddFloat4ColorExternParameter("flat_envmap_matrix_y", RenderMethodExtern.flat_envmap_matrix_y);
                        result.AddFloat4ColorExternParameter("flat_envmap_matrix_z", RenderMethodExtern.flat_envmap_matrix_z);
                        result.AddFloatParameter("hemisphere_percentage", 1.0f);
                        result.AddFloat4ColorParameter("env_bloom_override", new ShaderColor(255, 0, 0, 0));
                        result.AddFloatParameter("env_bloom_override_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\env_map_from_flat_texture";
                        break;
                    case Environment_Mapping.Custom_Map:
                        result.AddSamplerAddressWithColorParameter("environment_map", ShaderOptionParameter.ShaderAddressMode.Clamp, new ShaderColor(0, 255, 255, 255), @"shaders\default_bitmaps\bitmaps\default_dynamic_cube_map");
                        result.AddFloat3ColorParameter("env_tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("env_roughness_scale", 1.0f);
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.Dynamic_Reach:
                        result.AddFloat3ColorParameter("env_tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddSamplerExternAddressParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddSamplerExternAddressParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddFloatParameter("env_roughness_scale", 1.0f);
                        //result.AddFloatParameter("env_roughness_offset", 0.5f); // Breaks shader recompilation
                        rmopName = @"shaders\shader_options\env_map_dynamic";
                        break;
                    case Environment_Mapping.From_Flat_Texture_As_Cubemap:
                        result.AddSamplerAddressWithColorParameter("flat_environment_map", ShaderOptionParameter.ShaderAddressMode.BlackBorder, new ShaderColor(0, 255, 255, 255), @"shaders\default_bitmaps\bitmaps\color_red");
                        result.AddFloat3ColorParameter("env_tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat4ColorExternParameter("flat_envmap_matrix_x", RenderMethodExtern.flat_envmap_matrix_x);
                        result.AddFloat4ColorExternParameter("flat_envmap_matrix_y", RenderMethodExtern.flat_envmap_matrix_y);
                        result.AddFloat4ColorExternParameter("flat_envmap_matrix_z", RenderMethodExtern.flat_envmap_matrix_z);
                        result.AddFloatParameter("hemisphere_percentage", 1.0f);
                        result.AddFloat4ColorParameter("env_bloom_override", new ShaderColor(255, 0, 0, 0));
                        result.AddFloatParameter("env_bloom_override_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\env_map_from_flat_texture";
                        break;
                }
            }

            if (methodName == "self_illumination")
            {
                optionName = ((Self_Illumination)option).ToString();

                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.Off:
                        break;
                    case Self_Illumination.Simple:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\illum_simple";
                        break;
                    case Self_Illumination._3_Channel_Self_Illum:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat4ColorParameter("channel_a", new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("channel_b", new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("channel_c", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\illum_3_channel";
                        break;
                    case Self_Illumination.Plasma:
                        result.AddSamplerParameter("noise_map_a", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("noise_map_b", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat4ColorWithFloatParameter("color_medium", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorWithFloatParameter("color_wide", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorWithFloatParameter("color_sharp", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddSamplerParameter("alpha_mask_map", @"shaders\default_bitmaps\bitmaps\alpha_white");
                        result.AddFloatParameter("thinness_medium", 16.0f);
                        result.AddFloatParameter("thinness_wide", 4.0f);
                        result.AddFloatParameter("thinness_sharp", 32.0f);
                        rmopName = @"shaders\shader_options\illum_plasma";
                        break;
                    case Self_Illumination.From_Diffuse:
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\illum_from_diffuse";
                        break;
                    case Self_Illumination.Illum_Detail:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("self_illum_detail_map", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\illum_detail";
                        break;
                    case Self_Illumination.Meter:
                        result.AddSamplerParameter("meter_map", @"shaders\default_bitmaps\bitmaps\monochrome_alpha_grid");
                        result.AddFloat3ColorParameter("meter_color_off", new ShaderColor(255, 255, 0, 0));
                        result.AddFloat3ColorParameter("meter_color_on", new ShaderColor(0, 0, 255, 0));
                        result.AddFloatParameter("meter_value", 0.5f);
                        rmopName = @"shaders\shader_options\illum_meter";
                        break;
                    case Self_Illumination.Self_Illum_Times_Diffuse:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("primary_change_color_blend");
                        rmopName = @"shaders\shader_options\illum_times_diffuse";
                        break;
                    case Self_Illumination.Simple_With_Alpha_Mask:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\illum_simple";
                        break;
                    case Self_Illumination.Simple_Four_Change_Color:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternParameter("self_illum_color", RenderMethodExtern.object_change_color_quaternary, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\illum_simple_four_change_color";
                        break;
                    case Self_Illumination.Illum_Change_Color:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternParameter("self_illum_color", RenderMethodExtern.object_change_color_primary, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("primary_change_color_blend");
                        //result.AddFloat3ColorExternParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                        rmopName = @"shaders\shader_options\illum_change_color";
                        break;
                    case Self_Illumination.Multilayer_Additive:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("layer_depth", 0.1f);
                        result.AddFloatParameter("layer_contrast", 4.0f);
                        result.AddIntegerParameter("layers_of_4", 4);
                        result.AddFloatParameter("texcoord_aspect_ratio", 1.0f);
                        result.AddFloatParameter("depth_darken", 1.0f);
                        rmopName = @"shaders\shader_options\illum_multilayer";
                        break;
                    case Self_Illumination.Palettized_Plasma:
                        result.AddSamplerParameter("noise_map_a", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("noise_map_b", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerFilterAddressParameter("palette", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("alpha_mask_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        result.AddFloatParameter("alpha_modulation_factor", 0.1f);
                        result.AddSamplerExternFilterAddressParameter("depth_buffer", RenderMethodExtern.texture_global_target_z, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddFloatParameter("depth_fade_range", 0.1f);
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(1, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("v_coordinate", 0.5f);
                        result.AddFloatExternParameter("global_depth_constants", RenderMethodExtern.global_depth_constants);
                        result.AddFloatExternParameter("global_camera_forward", RenderMethodExtern.global_camera_forward);
                        rmopName = @"shaders\shader_options\illum_palettized_plasma";
                        break;
                    case Self_Illumination.Change_Color_Detail:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("self_illum_detail_map", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat3ColorExternParameter("self_illum_color", RenderMethodExtern.object_change_color_primary, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        //result.AddFloat3ColorExternParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                        rmopName = @"shaders\shader_options\illum_change_color_detail";
                        break;
                    case Self_Illumination.Illum_Detail_World_Space_Four_Cc:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("self_illum_detail_map", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat3ColorExternParameter("self_illum_color", RenderMethodExtern.object_change_color_quaternary, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloat4ColorWithFloatParameter("self_illum_obj_bounding_sphere", 1.0f);
                        rmopName = @"shaders\shader_options\illum_detail_world_space_four_cc";
                        break;
                    case Self_Illumination.Change_Color:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternParameter("self_illum_color", RenderMethodExtern.object_change_color_primary, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("primary_change_color_blend");
                        //result.AddFloat3ColorExternParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                        rmopName = @"shaders\shader_options\illum_change_color";
                        break;
                }
            }

            if (methodName == "blend_mode")
            {
                optionName = ((Blend_Mode)option).ToString();

                switch ((Blend_Mode)option)
                {
                    case Blend_Mode.Opaque:
                        break;
                    case Blend_Mode.Additive:
                        break;
                    case Blend_Mode.Multiply:
                        break;
                    case Blend_Mode.Alpha_Blend:
                        break;
                    case Blend_Mode.Double_Multiply:
                        break;
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        break;
                }
            }

            if (methodName == "parallax")
            {
                optionName = ((Parallax)option).ToString();

                switch ((Parallax)option)
                {
                    case Parallax.Off:
                        break;
                    case Parallax.Simple:
                        result.AddSamplerParameter("height_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        result.AddFloatParameter("height_scale", 0.1f);
                        rmopName = @"shaders\shader_options\parallax_simple";
                        break;
                    case Parallax.Interpolated:
                        result.AddSamplerParameter("height_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        result.AddFloatParameter("height_scale", 0.1f);
                        rmopName = @"shaders\shader_options\parallax_simple";
                        break;
                    case Parallax.Simple_Detail:
                        result.AddSamplerParameter("height_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        result.AddFloatParameter("height_scale", 0.1f);
                        result.AddSamplerParameter("height_scale_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        rmopName = @"shaders\shader_options\parallax_detail";
                        break;
                }
            }

            if (methodName == "misc")
            {
                optionName = ((Misc)option).ToString();

                switch ((Misc)option)
                {
                    case Misc.First_Person_Never:
                        break;
                    case Misc.First_Person_Sometimes:
                        break;
                    case Misc.First_Person_Always:
                        break;
                    case Misc.First_Person_Never_With_Rotating_Bitmaps:
                        break;
                    case Misc.Always_Calc_Albedo:
                        break;
                    case Misc.Default:
                        break;
                    case Misc.Rotating_Bitmaps_Super_Slow:
                        break;
                }
            }

            if (methodName == "distortion")
            {
                optionName = ((Distortion)option).ToString();

                switch ((Distortion)option)
                {
                    case Distortion.Off:
                        break;
                    case Distortion.On:
                        result.AddSamplerParameter("distort_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddFloatParameter("distort_scale", 0.1f);
                        result.AddBooleanParameter("distort_selfonly");
                        result.AddFloatParameter("distort_fadeoff", 10.0f);
                        rmopName = @"shaders\shader_options\sfx_distort";
                        break;
                }
            }

            if (methodName == "soft_fade")
            {
                optionName = ((Soft_Fade)option).ToString();

                switch ((Soft_Fade)option)
                {
                    case Soft_Fade.Off:
                        break;
                    case Soft_Fade.On:
                        result.AddSamplerExternFilterParameter("depth_map", RenderMethodExtern.texture_global_target_z, ShaderOptionParameter.ShaderFilterMode.Point);
                        result.AddBooleanWithFloatParameter("use_soft_fresnel", 0.1f);
                        result.AddFloatParameter("soft_fresnel_power");
                        result.AddBooleanParameter("use_soft_z");
                        result.AddFloatParameter("soft_z_range");
                        result.AddFloatExternParameter("screen_params", RenderMethodExtern.screen_constants);
                        rmopName = @"shaders\shader_options\soft_fade";
                        break;
                }
            }

            if (methodName == "misc_attr_animation")
            {
                optionName = ((Misc_Attr_Animation)option).ToString();

                switch ((Misc_Attr_Animation)option)
                {
                    case Misc_Attr_Animation.Off:
                        break;
                    case Misc_Attr_Animation.Scrolling_Cube:
                        result.AddIntegerParameter("misc_attr_animation_option", 1);
                        result.AddFloatParameter("scrolling_axis_x");
                        result.AddFloatParameter("scrolling_axis_y", 1.0f);
                        result.AddFloatParameter("scrolling_axis_z");
                        result.AddFloatParameter("scrolling_speed", 1.0f);
                        rmopName = @"shaders\shader_options\misc_attr_scrolling_cube";
                        break;
                    case Misc_Attr_Animation.Scrolling_Projected:
                        result.AddIntegerParameter("misc_attr_animation_option", 2);
                        result.AddFloatParameter("object_center_x");
                        result.AddFloatParameter("object_center_y");
                        result.AddFloatParameter("object_center_z", 0.65f);
                        result.AddFloatParameter("plane_u_x");
                        result.AddFloatParameter("plane_u_y", 1.0f);
                        result.AddFloatParameter("plane_u_z");
                        result.AddFloatParameter("plane_v_x");
                        result.AddFloatParameter("plane_v_y");
                        result.AddFloatParameter("plane_v_z", 1.0f);
                        result.AddFloatParameter("scale_u", 1.0f);
                        result.AddFloatParameter("scale_v", 1.0f);
                        result.AddFloatParameter("translate_u");
                        result.AddFloatParameter("translate_v");
                        result.AddFloatParameter("speed_u", 0.1f);
                        result.AddFloatParameter("speed_v");
                        rmopName = @"shaders\shader_options\misc_attr_scrolling_projected";
                        break;
                }
            }

            if (methodName == "wetness")
            {
                optionName = ((Wetness)option).ToString();

                switch ((Wetness)option)
                {
                    case Wetness.Default:
                        result.AddFloat3ColorParameter("wet_material_dim_tint", new ShaderColor(0, 216, 216, 235));
                        result.AddFloatWithColorParameter("wet_material_dim_coefficient", new ShaderColor(0, 149, 149, 149), 1.0f);
                        rmopName = @"shaders\wetness_options\wetness_simple";
                        break;
                    case Wetness.Flood:
                        result.AddFloatWithColorParameter("wet_material_dim_coefficient", new ShaderColor(0, 149, 149, 149), 1.0f);
                        result.AddFloat3ColorParameter("wet_material_dim_tint", new ShaderColor(0, 216, 216, 235));
                        result.AddFloatParameter("wet_sheen_reflection_contribution", 0.3f);
                        result.AddFloat3ColorParameter("wet_sheen_reflection_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("wet_sheen_thickness", 0.9f);
                        result.AddSamplerParameter("wet_flood_slope_map", @"rasterizer\water\static_wave\static_wave_slope_water");
                        result.AddSamplerFilterParameter("wet_noise_boundary_map", ShaderOptionParameter.ShaderFilterMode.Bilinear, @"rasterizer\rain\rain_noise_boundary");
                        result.AddFloatParameter("specular_mask_tweak_weight", 0.5f);
                        result.AddFloatParameter("surface_tilt_tweak_weight");
                        rmopName = @"shaders\wetness_options\wetness_flood";
                        break;
                    case Wetness.Proof:
                        break;
                    case Wetness.Simple:
                        result.AddFloat3ColorParameter("wet_material_dim_tint", new ShaderColor(0, 216, 216, 235));
                        result.AddFloatWithColorParameter("wet_material_dim_coefficient", new ShaderColor(0, 149, 149, 149), 1.0f);
                        rmopName = @"shaders\wetness_options\wetness_simple";
                        break;
                    case Wetness.Ripples:
                        result.AddFloatWithColorParameter("wet_material_dim_coefficient", new ShaderColor(0, 149, 149, 149), 1.0f);
                        result.AddFloat3ColorParameter("wet_material_dim_tint", new ShaderColor(0, 216, 216, 235));
                        result.AddFloatParameter("wet_sheen_reflection_contribution", 0.37f);
                        result.AddFloat3ColorParameter("wet_sheen_reflection_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("wet_sheen_thickness", 0.4f);
                        result.AddSamplerFilterParameter("wet_noise_boundary_map", ShaderOptionParameter.ShaderFilterMode.Bilinear, @"rasterizer\rain\rain_noise_boundary");
                        result.AddFloatParameter("specular_mask_tweak_weight", 0.5f);
                        result.AddFloatParameter("surface_tilt_tweak_weight", 0.3f);
                        rmopName = @"shaders\wetness_options\wetness_ripples";
                        break;
                }
            }

            if (methodName == "alpha_blend_source")
            {
                optionName = ((Alpha_Blend_Source)option).ToString();

                switch ((Alpha_Blend_Source)option)
                {
                    case Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel:
                        break;
                    case Alpha_Blend_Source.From_Albedo_Alpha:
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness", 3.0f);
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_albedo_alpha";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                        result.AddSamplerParameter("opacity_texture", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness", 3.0f);
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_opacity_map";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                        result.AddSamplerParameter("opacity_texture", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness", 3.0f);
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_opacity_map";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                        result.AddSamplerParameter("opacity_texture", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness", 3.0f);
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_opacity_map";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(ShaderMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((ShaderMethods)methodIndex)
            {
                case ShaderMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case ShaderMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping));
                case ShaderMethods.Alpha_Test:
                    return Enum.GetValues(typeof(Alpha_Test));
                case ShaderMethods.Specular_Mask:
                    return Enum.GetValues(typeof(Specular_Mask));
                case ShaderMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model));
                case ShaderMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping));
                case ShaderMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination));
                case ShaderMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
                case ShaderMethods.Parallax:
                    return Enum.GetValues(typeof(Parallax));
                case ShaderMethods.Misc:
                    return Enum.GetValues(typeof(Misc));
                case ShaderMethods.Distortion:
                    return Enum.GetValues(typeof(Distortion));
                case ShaderMethods.Soft_Fade:
                    return Enum.GetValues(typeof(Soft_Fade));
                case ShaderMethods.Misc_Attr_Animation:
                    return Enum.GetValues(typeof(Misc_Attr_Animation));
                case ShaderMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness));
                case ShaderMethods.Alpha_Blend_Source:
                    return Enum.GetValues(typeof(Alpha_Blend_Source));
            }

            return null;
        }

        public Array GetEntryPointOrder()
        {
            return new ShaderStage[]
            {
                ShaderStage.Albedo,
                ShaderStage.Static_Per_Pixel,
                ShaderStage.Static_Sh,
                ShaderStage.Static_Per_Vertex,
                ShaderStage.Dynamic_Light,
                ShaderStage.Shadow_Generate,
                ShaderStage.Static_Prt_Ambient,
                ShaderStage.Static_Prt_Linear,
                ShaderStage.Static_Prt_Quadratic,
                ShaderStage.Active_Camo,
                ShaderStage.Static_Per_Vertex_Color,
                ShaderStage.Lightmap_Debug_Mode,
                ShaderStage.Dynamic_Light_Cinematic,
                ShaderStage.Sfx_Distort
                //ShaderStage.Stipple,
                //ShaderStage.Single_Pass_Per_Pixel,
                //ShaderStage.Single_Pass_Per_Vertex,
                //ShaderStage.Single_Pass_Single_Probe,
                //ShaderStage.Single_Pass_Single_Probe_Ambient,
                //ShaderStage.Imposter_Static_Sh,
                //ShaderStage.Imposter_Static_Prt_Ambient,
            };
        }

        public Array GetVertexTypeOrder() 
        {
            return new VertexType[] 
            {
                VertexType.World,
                VertexType.Rigid,
                VertexType.Skinned,
                VertexType.DualQuat
            };
        }

        public void GetCategoryFunctions(string methodName, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "albedo")
            {
                vertexFunction = "calc_albedo_vs";
                pixelFunction = "calc_albedo_ps";
            }

            if (methodName == "bump_mapping")
            {
                vertexFunction = "calc_bumpmap_vs";
                pixelFunction = "calc_bumpmap_ps";
            }

            if (methodName == "alpha_test")
            {
                vertexFunction = "alpha_test";
                pixelFunction = "calc_alpha_test_ps";
            }

            if (methodName == "specular_mask")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_specular_mask_ps";
            }

            if (methodName == "material_model")
            {
                vertexFunction = "invalid";
                pixelFunction = "material_type";
            }

            if (methodName == "environment_mapping")
            {
                vertexFunction = "invalid";
                pixelFunction = "envmap_type";
            }

            if (methodName == "self_illumination")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_self_illumination_ps";
            }

            if (methodName == "blend_mode")
            {
                vertexFunction = "invalid";
                pixelFunction = "blend_type";
            }

            if (methodName == "parallax")
            {
                vertexFunction = "calc_parallax_vs";
                pixelFunction = "calc_parallax_ps";
            }

            if (methodName == "misc")
            {
                vertexFunction = "invalid";
                pixelFunction = "bitmap_rotation";
            }

            if (methodName == "distortion")
            {
                vertexFunction = "distort_proc_vs";
                pixelFunction = "distort_proc_ps";
            }

            if (methodName == "soft_fade")
            {
                vertexFunction = "invalid";
                pixelFunction = "apply_soft_fade";
            }

            if (methodName == "misc_attr_animation")
            {
                vertexFunction = "invalid"; // misc_attr_define (We ran out of output registers :/)
                pixelFunction = "invalid";
            }

            if (methodName == "wetness")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_wetness_ps";
            }

            if (methodName == "alpha_blend_source")
            {
                vertexFunction = "invalid";
                pixelFunction = "alpha_blend_source";
            }
        }

        public void GetOptionFunctions(string methodName, int option, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "albedo")
            {
                switch ((Albedo)option)
                {
                    case Albedo.Default:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_default_ps";
                        break;
                    case Albedo.Detail_Blend:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_detail_blend_ps";
                        break;
                    case Albedo.Constant_Color:
                        vertexFunction = "calc_albedo_constant_color_vs";
                        pixelFunction = "calc_albedo_constant_color_ps";
                        break;
                    case Albedo.Two_Change_Color:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_change_color_ps";
                        break;
                    case Albedo.Four_Change_Color:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_four_change_color_ps";
                        break;
                    case Albedo.Three_Detail_Blend:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_three_detail_blend_ps";
                        break;
                    case Albedo.Two_Detail_Overlay:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_detail_overlay_ps";
                        break;
                    case Albedo.Two_Detail:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_detail_ps";
                        break;
                    case Albedo.Color_Mask:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_color_mask_ps";
                        break;
                    case Albedo.Two_Detail_Black_Point:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_detail_black_point_ps";
                        break;
                    case Albedo.Two_Change_Color_Anim_Overlay:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_change_color_anim_ps";
                        break;
                    case Albedo.Chameleon:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_chameleon_ps";
                        break;
                    case Albedo.Two_Change_Color_Chameleon:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_change_color_chameleon_ps";
                        break;
                    case Albedo.Chameleon_Masked:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_chameleon_masked_ps";
                        break;
                    case Albedo.Color_Mask_Hard_Light:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_color_mask_hard_light_ps";
                        break;
                    case Albedo.Four_Change_Color_Applying_To_Specular:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_four_change_color_applying_to_specular_ps";
                        break;
                    case Albedo.Simple:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_simple_ps";
                        break;
                    case Albedo.Two_Change_Color_Tex_Overlay:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_change_color_tex_overlay_ps";
                        break;
                    case Albedo.Chameleon_Albedo_Masked:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_chameleon_albedo_masked_ps";
                        break;
                    case Albedo.Custom_Cube:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_custom_cube_ps";
                        break;
                    case Albedo.Two_Color:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_color_ps";
                        break;
                    case Albedo.Emblem:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_emblem_ps";
                        break;
                    case Albedo.Scrolling_Cube_Mask:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_scrolling_cube_mask_ps";
                        break;
                    case Albedo.Scrolling_Cube:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_scrolling_cube_ps";
                        break;
                    case Albedo.Scrolling_Texture_Uv:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_scrolling_texture_uv_ps";
                        break;
                    case Albedo.Texture_From_Misc:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_texture_from_misc_ps";
                        break;
                }
            }

            if (methodName == "bump_mapping")
            {
                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Off:
                        vertexFunction = "calc_bumpmap_off_vs";
                        pixelFunction = "calc_bumpmap_off_ps";
                        break;
                    case Bump_Mapping.Standard:
                        vertexFunction = "calc_bumpmap_default_vs";
                        pixelFunction = "calc_bumpmap_default_ps";
                        break;
                    case Bump_Mapping.Detail:
                        vertexFunction = "calc_bumpmap_detail_vs";
                        pixelFunction = "calc_bumpmap_detail_ps";
                        break;
                    case Bump_Mapping.Detail_Masked:
                        vertexFunction = "calc_bumpmap_detail_vs";
                        pixelFunction = "calc_bumpmap_detail_masked_ps";
                        break;
                    case Bump_Mapping.Detail_Plus_Detail_Masked:
                        vertexFunction = "calc_bumpmap_default_vs";
                        pixelFunction = "calc_bumpmap_detail_plus_detail_masked_ps";
                        break;
                    case Bump_Mapping.Detail_Unorm:
                        vertexFunction = "calc_bumpmap_default_vs";
                        pixelFunction = "calc_bumpmap_detail_unorm_ps";
                        break;
                    case Bump_Mapping.Detail_Blend:
                        vertexFunction = "calc_bumpmap_detail_blend_vs";
                        pixelFunction = "calc_bumpmap_detail_blend_ps";
                        break;
                    case Bump_Mapping.Three_Detail_Blend:
                        vertexFunction = "calc_bumpmap_three_detail_blend_vs";
                        pixelFunction = "calc_bumpmap_three_detail_blend_ps";
                        break;
                    case Bump_Mapping.Standard_Wrinkle:
                        vertexFunction = "calc_bumpmap_default_wrinkle_vs";
                        pixelFunction = "calc_bumpmap_default_wrinkle_ps";
                        break;
                    case Bump_Mapping.Detail_Wrinkle:
                        vertexFunction = "calc_bumpmap_detail_wrinkle_vs";
                        pixelFunction = "calc_bumpmap_detail_wrinkle_ps";
                        break;
                }
            }

            if (methodName == "alpha_test")
            {
                switch ((Alpha_Test)option)
                {
                    case Alpha_Test.None:
                        vertexFunction = "off";
                        pixelFunction = "calc_alpha_test_off_ps";
                        break;
                    case Alpha_Test.Simple:
                        vertexFunction = "on";
                        pixelFunction = "calc_alpha_test_on_ps";
                        break;
                }
            }

            if (methodName == "specular_mask")
            {
                switch ((Specular_Mask)option)
                {
                    case Specular_Mask.No_Specular_Mask:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_no_specular_mask_ps";
                        break;
                    case Specular_Mask.Specular_Mask_From_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_from_diffuse_ps";
                        break;
                    case Specular_Mask.Specular_Mask_From_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_texture_ps";
                        break;
                    case Specular_Mask.Specular_Mask_From_Color_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_color_texture_ps";
                        break;
                    case Specular_Mask.Specular_Mask_Mult_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_mult_texture_ps";
                        break;
                }
            }

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Diffuse_Only:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_only";
                        break;
                    case Material_Model.Cook_Torrance_Rim_Fresnel:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_rim_fresnel";
                        break;
                    case Material_Model.Two_Lobe_Phong:
                        vertexFunction = "invalid";
                        pixelFunction = "two_lobe_phong";
                        break;
                    case Material_Model.Foliage:
                        vertexFunction = "invalid";
                        pixelFunction = "foliage";
                        break;
                    case Material_Model.None:
                        vertexFunction = "invalid";
                        pixelFunction = "none";
                        break;
                    case Material_Model.Glass:
                        vertexFunction = "invalid";
                        pixelFunction = "glass";
                        break;
                    case Material_Model.Organism:
                        vertexFunction = "invalid";
                        pixelFunction = "organism";
                        break;
                    case Material_Model.Single_Lobe_Phong:
                        vertexFunction = "invalid";
                        pixelFunction = "single_lobe_phong";
                        break;
                    case Material_Model.Car_Paint:
                        vertexFunction = "invalid";
                        pixelFunction = "car_paint";
                        break;
                    case Material_Model.Hair:
                        vertexFunction = "invalid";
                        pixelFunction = "hair";
                        break;
                    case Material_Model.Cook_Torrance:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance";
                        break;
                    case Material_Model.Cook_Torrance_Pbr_Maps:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_pbr_maps";
                        break;
                    case Material_Model.Two_Lobe_Phong_Tint_Map:
                        vertexFunction = "invalid";
                        pixelFunction = "two_lobe_phong_tint_map";
                        break;
                    case Material_Model.Cook_Torrance_Reach:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_reach";
                        break;
                    case Material_Model.Two_Lobe_Phong_Reach:
                        vertexFunction = "invalid";
                        pixelFunction = "two_lobe_phong_reach";
                        break;
                    case Material_Model.Cook_Torrance_Custom_Cube:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_custom_cube";
                        break;
                    case Material_Model.Cook_Torrance_Two_Color_Spec_Tint:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_two_color_spec_tint";
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube_Mask:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_scrolling_cube_mask";
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_scrolling_cube";
                        break;
                    case Material_Model.Cook_Torrance_From_Albedo:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_from_albedo";
                        break;
                }
            }

            if (methodName == "environment_mapping")
            {
                switch ((Environment_Mapping)option)
                {
                    case Environment_Mapping.None:
                        vertexFunction = "invalid";
                        pixelFunction = "none";
                        break;
                    case Environment_Mapping.Per_Pixel:
                        vertexFunction = "invalid";
                        pixelFunction = "per_pixel";
                        break;
                    case Environment_Mapping.Dynamic:
                        vertexFunction = "invalid";
                        pixelFunction = "dynamic";
                        break;
                    case Environment_Mapping.From_Flat_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "from_flat_texture";
                        break;
                    case Environment_Mapping.Custom_Map:
                        vertexFunction = "invalid";
                        pixelFunction = "custom_map";
                        break;
                    case Environment_Mapping.Dynamic_Reach:
                        vertexFunction = "invalid";
                        pixelFunction = "dynamic_reach";
                        break;
                    case Environment_Mapping.From_Flat_Texture_As_Cubemap:
                        vertexFunction = "invalid";
                        pixelFunction = "from_flat_texture_as_cubemap";
                        break;
                }
            }

            if (methodName == "self_illumination")
            {
                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_none_ps";
                        break;
                    case Self_Illumination.Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_simple_ps";
                        break;
                    case Self_Illumination._3_Channel_Self_Illum:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_three_channel_ps";
                        break;
                    case Self_Illumination.Plasma:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_plasma_ps";
                        break;
                    case Self_Illumination.From_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_from_albedo_ps";
                        break;
                    case Self_Illumination.Illum_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_detail_ps";
                        break;
                    case Self_Illumination.Meter:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_meter_ps";
                        break;
                    case Self_Illumination.Self_Illum_Times_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_times_diffuse_ps";
                        break;
                    case Self_Illumination.Simple_With_Alpha_Mask:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_simple_with_alpha_mask_ps";
                        break;
                    case Self_Illumination.Simple_Four_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_simple_ps";
                        break;
                    case Self_Illumination.Illum_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_change_color_ps";
                        break;
                    case Self_Illumination.Multilayer_Additive:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_multilayer_ps";
                        break;
                    case Self_Illumination.Palettized_Plasma:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_palettized_plasma_ps";
                        break;
                    case Self_Illumination.Change_Color_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_change_color_detail_ps";
                        break;
                    case Self_Illumination.Illum_Detail_World_Space_Four_Cc:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_detail_world_space_ps";
                        break;
                    case Self_Illumination.Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_change_color_ps";
                        break;
                }
            }

            if (methodName == "blend_mode")
            {
                switch ((Blend_Mode)option)
                {
                    case Blend_Mode.Opaque:
                        vertexFunction = "invalid";
                        pixelFunction = "opaque";
                        break;
                    case Blend_Mode.Additive:
                        vertexFunction = "invalid";
                        pixelFunction = "additive";
                        break;
                    case Blend_Mode.Multiply:
                        vertexFunction = "invalid";
                        pixelFunction = "multiply";
                        break;
                    case Blend_Mode.Alpha_Blend:
                        vertexFunction = "invalid";
                        pixelFunction = "alpha_blend";
                        break;
                    case Blend_Mode.Double_Multiply:
                        vertexFunction = "invalid";
                        pixelFunction = "double_multiply";
                        break;
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "pre_multiplied_alpha";
                        break;
                }
            }

            if (methodName == "parallax")
            {
                switch ((Parallax)option)
                {
                    case Parallax.Off:
                        vertexFunction = "calc_parallax_off_vs";
                        pixelFunction = "calc_parallax_off_ps";
                        break;
                    case Parallax.Simple:
                        vertexFunction = "calc_parallax_simple_vs";
                        pixelFunction = "calc_parallax_simple_ps";
                        break;
                    case Parallax.Interpolated:
                        vertexFunction = "calc_parallax_interpolated_vs";
                        pixelFunction = "calc_parallax_interpolated_ps";
                        break;
                    case Parallax.Simple_Detail:
                        vertexFunction = "calc_parallax_simple_vs";
                        pixelFunction = "calc_parallax_simple_detail_ps";
                        break;
                }
            }

            if (methodName == "misc")
            {
                switch ((Misc)option)
                {
                    case Misc.First_Person_Never:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                    case Misc.First_Person_Sometimes:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                    case Misc.First_Person_Always:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                    case Misc.First_Person_Never_With_Rotating_Bitmaps:
                        vertexFunction = "invalid";
                        pixelFunction = "1";
                        break;
                    case Misc.Always_Calc_Albedo:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                    case Misc.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                    case Misc.Rotating_Bitmaps_Super_Slow:
                        vertexFunction = "invalid";
                        pixelFunction = "1";
                        break;
                }
            }

            if (methodName == "distortion")
            {
                switch ((Distortion)option)
                {
                    case Distortion.Off:
                        vertexFunction = "distort_nocolor_vs";
                        pixelFunction = "distort_off_ps";
                        break;
                    case Distortion.On:
                        vertexFunction = "distort_nocolor_vs";
                        pixelFunction = "distort_on_ps";
                        break;
                }
            }

            if (methodName == "soft_fade")
            {
                switch ((Soft_Fade)option)
                {
                    case Soft_Fade.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "apply_soft_fade_off";
                        break;
                    case Soft_Fade.On:
                        vertexFunction = "invalid";
                        pixelFunction = "apply_soft_fade_on";
                        break;
                }
            }

            if (methodName == "misc_attr_animation")
            {
                switch ((Misc_Attr_Animation)option)
                {
                    case Misc_Attr_Animation.Off:
                        vertexFunction = "invalid"; // off (We ran out of output registers :/)
                        pixelFunction = "invalid";
                        break;
                    case Misc_Attr_Animation.Scrolling_Cube:
                        vertexFunction = "invalid"; // misc_attr_exist (We ran out of output registers :/)
                        pixelFunction = "invalid";
                        break;
                    case Misc_Attr_Animation.Scrolling_Projected:
                        vertexFunction = "invalid"; // misc_attr_exist (We ran out of output registers :/)
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "wetness")
            {
                switch ((Wetness)option)
                {
                    case Wetness.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_default_ps";
                        break;
                    case Wetness.Flood:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_flood_ps";
                        break;
                    case Wetness.Proof:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_proof_ps";
                        break;
                    case Wetness.Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_simple_ps";
                        break;
                    case Wetness.Ripples:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_ripples_ps";
                        break;
                }
            }

            if (methodName == "alpha_blend_source")
            {
                switch ((Alpha_Blend_Source)option)
                {
                    case Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel:
                        vertexFunction = "invalid";
                        pixelFunction = "albedo_alpha_without_fresnel";
                        break;
                    case Alpha_Blend_Source.From_Albedo_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "albedo_alpha";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "opacity_map_alpha";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                        vertexFunction = "invalid";
                        pixelFunction = "opacity_map_rgb";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "opacity_map_alpha_and_albedo_alpha";
                        break;
                }
            }
        }
    }
}
