using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Halogram
{
    public class HalogramGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(HalogramMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((HalogramMethods)methodIndex)
            {
                case HalogramMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case HalogramMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination)).Length;
                case HalogramMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
                case HalogramMethods.Misc:
                    return Enum.GetValues(typeof(Misc)).Length;
                case HalogramMethods.Warp:
                    return Enum.GetValues(typeof(Warp)).Length;
                case HalogramMethods.Overlay:
                    return Enum.GetValues(typeof(Overlay)).Length;
                case HalogramMethods.Edge_Fade:
                    return Enum.GetValues(typeof(Edge_Fade)).Length;
                case HalogramMethods.Distortion:
                    return Enum.GetValues(typeof(Distortion)).Length;
                case HalogramMethods.Soft_Fade:
                    return Enum.GetValues(typeof(Soft_Fade)).Length;
            }

            return -1;
        }

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                default:
                    return -1;
            }
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Albedo:
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Sh:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Dynamic_Light:
                case ShaderStage.Shadow_Generate:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Static_Per_Vertex_Color:
                case ShaderStage.Dynamic_Light_Cinematic:
                //case ShaderStage.Stipple:
                //case ShaderStage.Active_Camo:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
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

        public bool IsVertexFormatSupported(VertexType vertexType)
        {
            switch (vertexType)
            {
                case VertexType.World:
                case VertexType.Rigid:
                case VertexType.Skinned:
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

            result.AddFloat3ColorExternWithFloatAndIntegerParameter("debug_tint", RenderMethodExtern.debug_tint, 1.0f, 1, new ShaderColor(255, 255, 255, 255));
            result.AddSamplerExternParameter("active_camo_distortion_texture", RenderMethodExtern.active_camo_distortion_texture);
            result.AddSamplerExternParameter("albedo_texture", RenderMethodExtern.texture_global_target_texaccum);
            result.AddSamplerExternParameter("dominant_light_intensity_map", RenderMethodExtern.texture_dominant_light_intensity_map);
            result.AddSamplerExternParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0);
            result.AddSamplerAddressParameter("g_diffuse_power_specular", ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\diffuse_power_specular\diffuse_power");
            result.AddSamplerFilterAddressParameter("g_direction_lut", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\direction_lut_1002");
            result.AddSamplerFilterAddressParameter("g_sample_vmf_diffuse_vs", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\diffusetable");
            result.AddSamplerFilterAddressParameter("g_sample_vmf_diffuse", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\diffusetable");
            result.AddSamplerFilterAddressParameter("g_sample_vmf_phong_specular", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\diffuse_power_specular\diffuse_power");
            result.AddSamplerExternFilterParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture, ShaderOptionParameter.ShaderFilterMode.Bilinear);
            result.AddSamplerExternParameter("normal_texture", RenderMethodExtern.texture_global_target_normal);
            result.AddSamplerExternParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture);
            result.AddSamplerExternParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerExternFilterAddressParameter("shadow_depth_map_1", RenderMethodExtern.texture_global_target_shadow_buffer1, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp);
            result.AddSamplerExternFilterAddressParameter("shadow_mask_texture", RenderMethodExtern.none, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp); // rmExtern - texture_global_target_shadow_mask
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
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_default";
                        break;
                    case Albedo.Detail_Blend:
                        result.AddFloatWithColorParameter("blend_alpha", new ShaderColor(255, 255, 255, 255), 1.0f);
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map2", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_detail_blend";
                        break;
                    case Albedo.Constant_Color:
                        result.AddFloat4ColorParameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_constant";
                        break;
                    case Albedo.Two_Change_Color:
                        result.AddFloat3ColorExternWithSamplerParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatParameter("camouflage_scale");
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("camouflage_change_color_map", @"rasterizer\invalid");
                        result.AddSamplerParameter("change_color_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_change_color";
                        break;
                    case Albedo.Four_Change_Color:
                        result.AddFloat3ColorExternWithSamplerParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("quaternary_change_color", RenderMethodExtern.object_change_color_quaternary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternWithSamplerParameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatParameter("camouflage_scale");
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("camouflage_change_color_map");
                        result.AddSamplerParameter("change_color_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_four_change_color";
                        break;
                    case Albedo.Three_Detail_Blend:
                        result.AddFloatWithColorParameter("blend_alpha", new ShaderColor(255, 255, 255, 255), 1.0f);
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map2", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map3", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_three_detail_blend";
                        break;
                    case Albedo.Two_Detail_Overlay:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map_overlay", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map2", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_detail_overlay";
                        break;
                    case Albedo.Two_Detail:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map2", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_detail";
                        break;
                    case Albedo.Color_Mask:
                        result.AddFloat3ColorParameter("neutral_gray", new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color2", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color3", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("color_mask_map", @"shaders\default_bitmaps\bitmaps\reference_grids");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_color_mask";
                        break;
                    case Albedo.Two_Detail_Black_Point:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddSamplerParameter("detail_map2");
                        rmopName = @"shaders\shader_options\albedo_two_detail_black_point";
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
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_simple";
                        break;
                    case Self_Illumination._3_Channel_Self_Illum:
                        result.AddFloat4ColorParameter("channel_a", new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("channel_b", new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("channel_c", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_3_channel";
                        break;
                    case Self_Illumination.Plasma:
                        result.AddFloat4ColorWithFloatParameter("color_medium", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorWithFloatParameter("color_sharp", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorWithFloatParameter("color_wide", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("thinness_medium", 16.0f);
                        result.AddFloatParameter("thinness_sharp", 32.0f);
                        result.AddFloatParameter("thinness_wide", 4.0f);
                        result.AddSamplerParameter("alpha_mask_map", @"shaders\default_bitmaps\bitmaps\alpha_white");
                        result.AddSamplerParameter("noise_map_a", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("noise_map_b", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_plasma";
                        break;
                    case Self_Illumination.From_Diffuse:
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\illum_from_diffuse";
                        break;
                    case Self_Illumination.Illum_Detail:
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddSamplerParameter("self_illum_detail_map", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_detail";
                        break;
                    case Self_Illumination.Meter:
                        result.AddFloat3ColorParameter("meter_color_off", new ShaderColor(255, 255, 0, 0));
                        result.AddFloat3ColorParameter("meter_color_on", new ShaderColor(0, 0, 255, 0));
                        result.AddFloatParameter("meter_value", 0.5f);
                        result.AddSamplerParameter("meter_map", @"shaders\default_bitmaps\bitmaps\monochrome_alpha_grid");
                        rmopName = @"shaders\shader_options\illum_meter";
                        break;
                    case Self_Illumination.Self_Illum_Times_Diffuse:
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("primary_change_color_blend");
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_times_diffuse";
                        break;
                    case Self_Illumination.Multilayer_Additive:
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("depth_darken", 1.0f);
                        result.AddFloatParameter("layer_contrast", 4.0f);
                        result.AddFloatParameter("layer_depth", 0.1f);
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("texcoord_aspect_ratio", 1.0f);
                        result.AddIntegerParameter("layers_of_4", 4);
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_multilayer";
                        break;
                    case Self_Illumination.Ml_Add_Four_Change_Color:
                        result.AddFloat3ColorExternParameter("self_illum_color", RenderMethodExtern.object_change_color_quaternary, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("depth_darken", 1.0f);
                        result.AddFloatParameter("layer_contrast", 4.0f);
                        result.AddFloatParameter("layer_depth", 0.1f);
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("texcoord_aspect_ratio", 1.0f);
                        result.AddIntegerParameter("layers_of_4", 4);
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_multilayer_four_change_color";
                        break;
                    case Self_Illumination.Ml_Add_Five_Change_Color:
                        result.AddFloat3ColorExternParameter("self_illum_color", RenderMethodExtern.object_change_color_quinary, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("depth_darken", 1.0f);
                        result.AddFloatParameter("layer_contrast", 4.0f);
                        result.AddFloatParameter("layer_depth", 0.1f);
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("texcoord_aspect_ratio", 1.0f);
                        result.AddIntegerParameter("layers_of_4", 4);
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_multilayer_five_change_color";
                        break;
                    case Self_Illumination.Scope_Blur:
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloat3ColorParameter("self_illum_heat_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatExternParameter("z_camera_pixel_size", RenderMethodExtern.z_camera_pixel_size);
                        rmopName = @"shaders\shader_options\illum_scope_blur";
                        break;
                    case Self_Illumination.Palettized_Plasma:
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(1, 255, 255, 255));
                        result.AddFloatParameter("alpha_modulation_factor", 0.1f);
                        result.AddFloatParameter("depth_fade_range", 0.1f);
                        result.AddFloatExternParameter("global_camera_forward", RenderMethodExtern.global_camera_forward);
                        result.AddFloatExternParameter("global_depth_constants", RenderMethodExtern.global_depth_constants);
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("v_coordinate", 0.5f);
                        result.AddSamplerAddressParameter("alpha_mask_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        result.AddSamplerExternFilterAddressParameter("depth_buffer", RenderMethodExtern.texture_global_target_z, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddSamplerParameter("noise_map_a", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("noise_map_b", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerFilterAddressParameter("palette", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_palettized_plasma";
                        break;
                    case Self_Illumination.Palettized_Plasma_Change_Color:
                        result.AddFloat3ColorExternParameter("self_illum_color", RenderMethodExtern.object_change_color_primary, new ShaderColor(1, 255, 255, 255));
                        result.AddFloatParameter("alpha_modulation_factor", 0.1f);
                        result.AddFloatParameter("depth_fade_range", 0.1f);
                        result.AddFloatExternParameter("global_camera_forward", RenderMethodExtern.global_camera_forward);
                        result.AddFloatExternParameter("global_depth_constants", RenderMethodExtern.global_depth_constants);
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("v_coordinate", 0.5f);
                        result.AddSamplerAddressParameter("alpha_mask_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        result.AddSamplerExternFilterAddressParameter("depth_buffer", RenderMethodExtern.texture_global_target_z, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddSamplerParameter("noise_map_a", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("noise_map_b", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerFilterAddressParameter("palette", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\screen_options\illum_palettized_plasma_change_color";
                        break;
                    case Self_Illumination.Palettized_Depth_Fade:
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(1, 255, 255, 255));
                        result.AddFloatParameter("alpha_modulation_factor", 0.1f);
                        result.AddFloatParameter("depth_fade_range", 0.1f);
                        result.AddFloatExternParameter("global_camera_forward", RenderMethodExtern.global_camera_forward);
                        result.AddFloatExternParameter("global_depth_constants", RenderMethodExtern.global_depth_constants);
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("v_coordinate", 0.5f);
                        result.AddSamplerAddressParameter("alpha_mask_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        result.AddSamplerExternFilterAddressParameter("depth_buffer", RenderMethodExtern.texture_global_target_z, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddSamplerParameter("noise_map_a", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerFilterAddressParameter("palette", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\screen_options\illum_palettized_depth_fade";
                        break;
                    case Self_Illumination.Plasma_Wide_And_Sharp_Five_Change_Color:
                        result.AddFloat3ColorExternWithFloatParameter("color_sharp", RenderMethodExtern.object_change_color_quinary, 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat3ColorExternWithFloatParameter("color_wide", RenderMethodExtern.object_change_color_quinary, 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorWithFloatParameter("color_medium", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("color_sharp_alpha", 1.0f);
                        result.AddFloatParameter("color_wide_alpha", 1.0f);
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("thinness_medium", 16.0f);
                        result.AddFloatParameter("thinness_sharp", 32.0f);
                        result.AddFloatParameter("thinness_wide", 4.0f);
                        result.AddSamplerParameter("alpha_mask_map", @"shaders\default_bitmaps\bitmaps\alpha_white");
                        result.AddSamplerParameter("noise_map_a", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("noise_map_b", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_plasma_wide_and_sharp_five_change_color";
                        break;
                    case Self_Illumination.Self_Illum_Holograms:
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_holograms";
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
                }
            }

            if (methodName == "warp")
            {
                optionName = ((Warp)option).ToString();

                switch ((Warp)option)
                {
                    case Warp.None:
                        break;
                    case Warp.From_Texture:
                        result.AddFloatParameter("warp_amount_x", 1.0f);
                        result.AddFloatParameter("warp_amount_y", 1.0f);
                        result.AddSamplerParameter("warp_map", @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        rmopName = @"shaders\shader_options\warp_from_texture";
                        break;
                    case Warp.Parallax_Simple:
                        result.AddFloatParameter("height_scale", 0.1f);
                        result.AddSamplerParameter("height_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        rmopName = @"shaders\shader_options\parallax_simple";
                        break;
                }
            }

            if (methodName == "overlay")
            {
                optionName = ((Overlay)option).ToString();

                switch ((Overlay)option)
                {
                    case Overlay.None:
                        break;
                    case Overlay.Additive:
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        rmopName = @"shaders\shader_options\overlay_additive";
                        break;
                    case Overlay.Additive_Detail:
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
                        result.AddSamplerParameter("overlay_detail_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        rmopName = @"shaders\shader_options\overlay_additive_detail";
                        break;
                    case Overlay.Multiply:
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        rmopName = @"shaders\shader_options\overlay_additive";
                        break;
                    case Overlay.Multiply_And_Additive_Detail:
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
                        result.AddSamplerParameter("overlay_detail_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddSamplerParameter("overlay_multiply_map", @"shaders\default_bitmaps\bitmaps\reference_grids");
                        rmopName = @"shaders\shader_options\overlay_multiply_additive_detail";
                        break;
                }
            }

            if (methodName == "edge_fade")
            {
                optionName = ((Edge_Fade)option).ToString();

                switch ((Edge_Fade)option)
                {
                    case Edge_Fade.None:
                        break;
                    case Edge_Fade.Simple:
                        result.AddFloat3ColorParameter("edge_fade_center_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat3ColorParameter("edge_fade_edge_tint");
                        result.AddFloatParameter("edge_fade_power", 1.0f);
                        rmopName = @"shaders\shader_options\edge_fade_simple";
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
                        result.AddBooleanParameter("distort_selfonly");
                        result.AddFloatParameter("distort_fadeoff", 10.0f);
                        result.AddFloatParameter("distort_scale", 0.1f);
                        result.AddSamplerParameter("distort_map", @"shaders\default_bitmaps\bitmaps\default_vector");
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
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(HalogramMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((HalogramMethods)methodIndex)
            {
                case HalogramMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case HalogramMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination));
                case HalogramMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
                case HalogramMethods.Misc:
                    return Enum.GetValues(typeof(Misc));
                case HalogramMethods.Warp:
                    return Enum.GetValues(typeof(Warp));
                case HalogramMethods.Overlay:
                    return Enum.GetValues(typeof(Overlay));
                case HalogramMethods.Edge_Fade:
                    return Enum.GetValues(typeof(Edge_Fade));
                case HalogramMethods.Distortion:
                    return Enum.GetValues(typeof(Distortion));
                case HalogramMethods.Soft_Fade:
                    return Enum.GetValues(typeof(Soft_Fade));
            }

            return null;
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

            if (methodName == "misc")
            {
                vertexFunction = "invalid";
                pixelFunction = "bitmap_rotation";
            }

            if (methodName == "warp")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_parallax_ps";
            }

            if (methodName == "overlay")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_overlay_ps";
            }

            if (methodName == "edge_fade")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_edge_fade_ps";
            }

            if (methodName == "distortion")
            {
                vertexFunction = "invalid";
                pixelFunction = "distort_proc_ps";
            }

            if (methodName == "soft_fade")
            {
                vertexFunction = "invalid";
                pixelFunction = "apply_soft_fade";
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
                    case Self_Illumination.Multilayer_Additive:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_multilayer_ps";
                        break;
                    case Self_Illumination.Ml_Add_Four_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_multilayer_ps";
                        break;
                    case Self_Illumination.Ml_Add_Five_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_multilayer_ps";
                        break;
                    case Self_Illumination.Scope_Blur:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_scope_blur_ps";
                        break;
                    case Self_Illumination.Palettized_Plasma:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_palettized_plasma_ps";
                        break;
                    case Self_Illumination.Palettized_Plasma_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_palettized_plasma_ps";
                        break;
                    case Self_Illumination.Palettized_Depth_Fade:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_palettized_depth_fade_ps";
                        break;
                    case Self_Illumination.Plasma_Wide_And_Sharp_Five_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_plasma_wide_and_sharp_five_change_color_ps";
                        break;
                    case Self_Illumination.Self_Illum_Holograms:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_holograms_ps";
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
                        pixelFunction = "2";
                        break;
                }
            }

            if (methodName == "warp")
            {
                switch ((Warp)option)
                {
                    case Warp.None:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_parallax_off_ps";
                        break;
                    case Warp.From_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_warp_from_texture_ps";
                        break;
                    case Warp.Parallax_Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_parallax_simple_ps";
                        break;
                }
            }

            if (methodName == "overlay")
            {
                switch ((Overlay)option)
                {
                    case Overlay.None:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_none_ps";
                        break;
                    case Overlay.Additive:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_additive_ps";
                        break;
                    case Overlay.Additive_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_additive_detail_ps";
                        break;
                    case Overlay.Multiply:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_multiply_ps";
                        break;
                    case Overlay.Multiply_And_Additive_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_multiply_and_additive_detail_ps";
                        break;
                }
            }

            if (methodName == "edge_fade")
            {
                switch ((Edge_Fade)option)
                {
                    case Edge_Fade.None:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_edge_fade_none_ps";
                        break;
                    case Edge_Fade.Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_edge_fade_simple_ps";
                        break;
                }
            }

            if (methodName == "distortion")
            {
                switch ((Distortion)option)
                {
                    case Distortion.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "distort_off_ps";
                        break;
                    case Distortion.On:
                        vertexFunction = "invalid";
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
        }
    }
}
