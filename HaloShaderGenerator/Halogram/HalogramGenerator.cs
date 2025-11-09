using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Halogram
{
    public class HalogramGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(HalogramMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (HalogramMethods)methodIndex switch
            {
                HalogramMethods.Albedo => Enum.GetValues(typeof(Albedo)).Length,
                HalogramMethods.Self_Illumination => Enum.GetValues(typeof(Self_Illumination)).Length,
                HalogramMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)).Length,
                HalogramMethods.Misc => Enum.GetValues(typeof(Misc)).Length,
                HalogramMethods.Warp => Enum.GetValues(typeof(Warp)).Length,
                HalogramMethods.Overlay => Enum.GetValues(typeof(Overlay)).Length,
                HalogramMethods.Edge_Fade => Enum.GetValues(typeof(Edge_Fade)).Length,
                HalogramMethods.Distortion => Enum.GetValues(typeof(Distortion)).Length,
                HalogramMethods.Soft_Fade => Enum.GetValues(typeof(Soft_Fade)).Length,
                _ => -1,
            };
        }

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint) => -1;

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint) => false;

        public bool IsPixelShaderShared(ShaderStage entryPoint)
        {
            return entryPoint switch
            {
                ShaderStage.Shadow_Generate => true,
                _ => false,
            };
        }

        public bool IsAutoMacro() => false;

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
            //result.AddSamplerFilterAddressParameter("g_direction_lut", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\direction_lut_1002");
            //result.AddSamplerFilterAddressParameter("g_sample_vmf_diffuse", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\diffusetable");
            //result.AddSamplerFilterAddressParameter("g_sample_vmf_diffuse_vs", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\diffusetable");
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
                        result.AddFloatWithColorParameter("blend_alpha", new ShaderColor(255, 255, 255, 255), 1.0f);
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
                        result.AddSamplerParameter("camouflage_change_color_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatParameter("camouflage_scale");
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
                        result.AddSamplerParameter("camouflage_change_color_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatParameter("camouflage_scale");
                        rmopName = @"shaders\shader_options\albedo_four_change_color";
                        break;
                    case Albedo.Three_Detail_Blend:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map2", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerWithScaleParameter("detail_map3", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloatWithColorParameter("blend_alpha", new ShaderColor(255, 255, 255, 255), 1.0f);
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
                    case Self_Illumination.Ml_Add_Four_Change_Color:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternParameter("self_illum_color", RenderMethodExtern.object_change_color_quaternary, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("layer_depth", 0.1f);
                        result.AddFloatParameter("layer_contrast", 4.0f);
                        result.AddIntegerParameter("layers_of_4", 4);
                        result.AddFloatParameter("texcoord_aspect_ratio", 1.0f);
                        result.AddFloatParameter("depth_darken", 1.0f);
                        rmopName = @"shaders\shader_options\illum_multilayer_four_change_color";
                        break;
                    case Self_Illumination.Ml_Add_Five_Change_Color:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternParameter("self_illum_color", RenderMethodExtern.object_change_color_quinary, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("layer_depth", 0.1f);
                        result.AddFloatParameter("layer_contrast", 4.0f);
                        result.AddIntegerParameter("layers_of_4", 4);
                        result.AddFloatParameter("texcoord_aspect_ratio", 1.0f);
                        result.AddFloatParameter("depth_darken", 1.0f);
                        rmopName = @"shaders\shader_options\illum_multilayer_five_change_color";
                        break;
                    case Self_Illumination.Scope_Blur:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloat3ColorParameter("self_illum_heat_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatExternParameter("z_camera_pixel_size", RenderMethodExtern.z_camera_pixel_size);
                        rmopName = @"shaders\shader_options\illum_scope_blur";
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
                    case Self_Illumination.Palettized_Plasma_Change_Color:
                        result.AddSamplerParameter("noise_map_a", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("noise_map_b", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerFilterAddressParameter("palette", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("alpha_mask_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        result.AddFloatParameter("alpha_modulation_factor", 0.1f);
                        result.AddSamplerExternFilterAddressParameter("depth_buffer", RenderMethodExtern.texture_global_target_z, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddFloatParameter("depth_fade_range", 0.1f);
                        result.AddFloat3ColorExternParameter("self_illum_color", RenderMethodExtern.object_change_color_primary, new ShaderColor(1, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddFloatParameter("v_coordinate", 0.5f);
                        result.AddFloatExternParameter("global_depth_constants", RenderMethodExtern.global_depth_constants);
                        result.AddFloatExternParameter("global_camera_forward", RenderMethodExtern.global_camera_forward);
                        rmopName = @"shaders\screen_options\illum_palettized_plasma_change_color";
                        break;
                    case Self_Illumination.Palettized_Depth_Fade:
                        result.AddSamplerParameter("noise_map_a", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
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
                        rmopName = @"shaders\screen_options\illum_palettized_depth_fade";
                        break;
                    case Self_Illumination.Plasma_Wide_And_Sharp_Five_Change_Color:
                        result.AddSamplerParameter("noise_map_a", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("noise_map_b", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat4ColorWithFloatParameter("color_medium", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat3ColorExternWithFloatParameter("color_wide", RenderMethodExtern.object_change_color_quinary, 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("color_wide_alpha", 1.0f);
                        result.AddFloat3ColorExternWithFloatParameter("color_sharp", RenderMethodExtern.object_change_color_quinary, 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("color_sharp_alpha", 1.0f);
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
                        result.AddSamplerParameter("alpha_mask_map", @"shaders\default_bitmaps\bitmaps\alpha_white");
                        result.AddFloatParameter("thinness_medium", 16.0f);
                        result.AddFloatParameter("thinness_wide", 4.0f);
                        result.AddFloatParameter("thinness_sharp", 32.0f);
                        rmopName = @"shaders\shader_options\illum_plasma_wide_and_sharp_five_change_color";
                        break;
                    case Self_Illumination.Self_Illum_Holograms:
                        result.AddSamplerParameter("self_illum_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorParameter("self_illum_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", 1.0f);
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
                        result.AddSamplerParameter("warp_map", @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddFloatParameter("warp_amount_x", 1.0f);
                        result.AddFloatParameter("warp_amount_y", 1.0f);
                        rmopName = @"shaders\shader_options\warp_from_texture";
                        break;
                    case Warp.Parallax_Simple:
                        result.AddSamplerParameter("height_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        result.AddFloatParameter("height_scale", 0.1f);
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
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\overlay_additive";
                        break;
                    case Overlay.Additive_Detail:
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddSamplerParameter("overlay_detail_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\overlay_additive_detail";
                        break;
                    case Overlay.Multiply:
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\overlay_additive";
                        break;
                    case Overlay.Multiply_And_Additive_Detail:
                        result.AddSamplerParameter("overlay_multiply_map", @"shaders\default_bitmaps\bitmaps\reference_grids");
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddSamplerParameter("overlay_detail_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
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
                        result.AddFloat3ColorParameter("edge_fade_edge_tint");
                        result.AddFloat3ColorParameter("edge_fade_center_tint", new ShaderColor(0, 255, 255, 255));
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
            return result;
        }

        public Array GetMethodNames() => Enum.GetValues(typeof(HalogramMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (HalogramMethods)methodIndex switch
            {
                HalogramMethods.Albedo => Enum.GetValues(typeof(Albedo)),
                HalogramMethods.Self_Illumination => Enum.GetValues(typeof(Self_Illumination)),
                HalogramMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)),
                HalogramMethods.Misc => Enum.GetValues(typeof(Misc)),
                HalogramMethods.Warp => Enum.GetValues(typeof(Warp)),
                HalogramMethods.Overlay => Enum.GetValues(typeof(Overlay)),
                HalogramMethods.Edge_Fade => Enum.GetValues(typeof(Edge_Fade)),
                HalogramMethods.Distortion => Enum.GetValues(typeof(Distortion)),
                HalogramMethods.Soft_Fade => Enum.GetValues(typeof(Soft_Fade)),
                _ => null,
            };
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
                ShaderStage.Static_Per_Vertex_Color,
                ShaderStage.Dynamic_Light_Cinematic,
                ShaderStage.Sfx_Distort,
                //ShaderStage.Stipple,
                //ShaderStage.Active_Camo
            };
        }

        public Array GetVertexTypeOrder()
        {
            return new VertexType[]
            {
                VertexType.World,
                VertexType.Rigid,
                VertexType.Skinned
            };
        }

        public string GetCategoryPixelFunction(int category)
        {
            return (HalogramMethods)category switch
            {
                HalogramMethods.Albedo => "calc_albedo_ps",
                HalogramMethods.Self_Illumination => "calc_self_illumination_ps",
                HalogramMethods.Blend_Mode => "blend_type",
                HalogramMethods.Misc => "bitmap_rotation",
                HalogramMethods.Warp => "calc_parallax_ps",
                HalogramMethods.Overlay => "calc_overlay_ps",
                HalogramMethods.Edge_Fade => "calc_edge_fade_ps",
                HalogramMethods.Distortion => "distort_proc_ps",
                HalogramMethods.Soft_Fade => "apply_soft_fade",
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (HalogramMethods)category switch
            {
                HalogramMethods.Albedo => "calc_albedo_vs",
                HalogramMethods.Self_Illumination => string.Empty,
                HalogramMethods.Blend_Mode => string.Empty,
                HalogramMethods.Misc => string.Empty,
                HalogramMethods.Warp => string.Empty,
                HalogramMethods.Overlay => string.Empty,
                HalogramMethods.Edge_Fade => string.Empty,
                HalogramMethods.Distortion => string.Empty,
                HalogramMethods.Soft_Fade => string.Empty,
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (HalogramMethods)category switch
            {
                HalogramMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Default => "calc_albedo_default_ps",
                    Albedo.Detail_Blend => "calc_albedo_detail_blend_ps",
                    Albedo.Constant_Color => "calc_albedo_constant_color_ps",
                    Albedo.Two_Change_Color => "calc_albedo_two_change_color_ps",
                    Albedo.Four_Change_Color => "calc_albedo_four_change_color_ps",
                    Albedo.Three_Detail_Blend => "calc_albedo_three_detail_blend_ps",
                    Albedo.Two_Detail_Overlay => "calc_albedo_two_detail_overlay_ps",
                    Albedo.Two_Detail => "calc_albedo_two_detail_ps",
                    Albedo.Color_Mask => "calc_albedo_color_mask_ps",
                    Albedo.Two_Detail_Black_Point => "calc_albedo_two_detail_black_point_ps",
                    _ => null,
                },
                HalogramMethods.Self_Illumination => (Self_Illumination)option switch
                {
                    Self_Illumination.Off => "calc_self_illumination_none_ps",
                    Self_Illumination.Simple => "calc_self_illumination_simple_ps",
                    Self_Illumination._3_Channel_Self_Illum => "calc_self_illumination_three_channel_ps",
                    Self_Illumination.Plasma => "calc_self_illumination_plasma_ps",
                    Self_Illumination.From_Diffuse => "calc_self_illumination_from_albedo_ps",
                    Self_Illumination.Illum_Detail => "calc_self_illumination_detail_ps",
                    Self_Illumination.Meter => "calc_self_illumination_meter_ps",
                    Self_Illumination.Self_Illum_Times_Diffuse => "calc_self_illumination_times_diffuse_ps",
                    Self_Illumination.Multilayer_Additive => "calc_self_illumination_multilayer_ps",
                    Self_Illumination.Ml_Add_Four_Change_Color => "calc_self_illumination_multilayer_ps",
                    Self_Illumination.Ml_Add_Five_Change_Color => "calc_self_illumination_multilayer_ps",
                    Self_Illumination.Scope_Blur => "calc_self_illumination_scope_blur_ps",
                    Self_Illumination.Palettized_Plasma => "calc_self_illumination_palettized_plasma_ps",
                    Self_Illumination.Palettized_Plasma_Change_Color => "calc_self_illumination_palettized_plasma_ps",
                    Self_Illumination.Palettized_Depth_Fade => "calc_self_illumination_palettized_depth_fade_ps",
                    Self_Illumination.Plasma_Wide_And_Sharp_Five_Change_Color => "calc_self_illumination_plasma_wide_and_sharp_five_change_color_ps",
                    Self_Illumination.Self_Illum_Holograms => "calc_self_illumination_holograms_ps",
                    _ => null,
                },
                HalogramMethods.Blend_Mode => (Blend_Mode)option switch
                {
                    Blend_Mode.Opaque => "opaque",
                    Blend_Mode.Additive => "additive",
                    Blend_Mode.Multiply => "multiply",
                    Blend_Mode.Alpha_Blend => "alpha_blend",
                    Blend_Mode.Double_Multiply => "double_multiply",
                    _ => null,
                },
                HalogramMethods.Misc => (Misc)option switch
                {
                    Misc.First_Person_Never => "0",
                    Misc.First_Person_Sometimes => "0",
                    Misc.First_Person_Always => "0",
                    Misc.First_Person_Never_With_Rotating_Bitmaps => "1",
                    Misc.Always_Calc_Albedo => "2",
                    _ => null,
                },
                HalogramMethods.Warp => (Warp)option switch
                {
                    Warp.None => "calc_parallax_off_ps",
                    Warp.From_Texture => "calc_warp_from_texture_ps",
                    Warp.Parallax_Simple => "calc_parallax_simple_ps",
                    _ => null,
                },
                HalogramMethods.Overlay => (Overlay)option switch
                {
                    Overlay.None => "calc_overlay_none_ps",
                    Overlay.Additive => "calc_overlay_additive_ps",
                    Overlay.Additive_Detail => "calc_overlay_additive_detail_ps",
                    Overlay.Multiply => "calc_overlay_multiply_ps",
                    Overlay.Multiply_And_Additive_Detail => "calc_overlay_multiply_and_additive_detail_ps",
                    _ => null,
                },
                HalogramMethods.Edge_Fade => (Edge_Fade)option switch
                {
                    Edge_Fade.None => "calc_edge_fade_none_ps",
                    Edge_Fade.Simple => "calc_edge_fade_simple_ps",
                    _ => null,
                },
                HalogramMethods.Distortion => (Distortion)option switch
                {
                    Distortion.Off => "distort_off_ps",
                    Distortion.On => "distort_on_ps",
                    _ => null,
                },
                HalogramMethods.Soft_Fade => (Soft_Fade)option switch
                {
                    Soft_Fade.Off => "apply_soft_fade_off",
                    Soft_Fade.On => "apply_soft_fade_on",
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (HalogramMethods)category switch
            {
                HalogramMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Default => "calc_albedo_default_vs",
                    Albedo.Detail_Blend => "calc_albedo_default_vs",
                    Albedo.Constant_Color => "calc_albedo_constant_color_vs",
                    Albedo.Two_Change_Color => "calc_albedo_default_vs",
                    Albedo.Four_Change_Color => "calc_albedo_default_vs",
                    Albedo.Three_Detail_Blend => "calc_albedo_default_vs",
                    Albedo.Two_Detail_Overlay => "calc_albedo_default_vs",
                    Albedo.Two_Detail => "calc_albedo_default_vs",
                    Albedo.Color_Mask => "calc_albedo_default_vs",
                    Albedo.Two_Detail_Black_Point => "calc_albedo_default_vs",
                    _ => null,
                },
                HalogramMethods.Self_Illumination => (Self_Illumination)option switch
                {
                    Self_Illumination.Off => string.Empty,
                    Self_Illumination.Simple => string.Empty,
                    Self_Illumination._3_Channel_Self_Illum => string.Empty,
                    Self_Illumination.Plasma => string.Empty,
                    Self_Illumination.From_Diffuse => string.Empty,
                    Self_Illumination.Illum_Detail => string.Empty,
                    Self_Illumination.Meter => string.Empty,
                    Self_Illumination.Self_Illum_Times_Diffuse => string.Empty,
                    Self_Illumination.Multilayer_Additive => string.Empty,
                    Self_Illumination.Ml_Add_Four_Change_Color => string.Empty,
                    Self_Illumination.Ml_Add_Five_Change_Color => string.Empty,
                    Self_Illumination.Scope_Blur => string.Empty,
                    Self_Illumination.Palettized_Plasma => string.Empty,
                    Self_Illumination.Palettized_Plasma_Change_Color => string.Empty,
                    Self_Illumination.Palettized_Depth_Fade => string.Empty,
                    Self_Illumination.Plasma_Wide_And_Sharp_Five_Change_Color => string.Empty,
                    Self_Illumination.Self_Illum_Holograms => string.Empty,
                    _ => null,
                },
                HalogramMethods.Blend_Mode => (Blend_Mode)option switch
                {
                    Blend_Mode.Opaque => string.Empty,
                    Blend_Mode.Additive => string.Empty,
                    Blend_Mode.Multiply => string.Empty,
                    Blend_Mode.Alpha_Blend => string.Empty,
                    Blend_Mode.Double_Multiply => string.Empty,
                    _ => null,
                },
                HalogramMethods.Misc => (Misc)option switch
                {
                    Misc.First_Person_Never => string.Empty,
                    Misc.First_Person_Sometimes => string.Empty,
                    Misc.First_Person_Always => string.Empty,
                    Misc.First_Person_Never_With_Rotating_Bitmaps => string.Empty,
                    Misc.Always_Calc_Albedo => string.Empty,
                    _ => null,
                },
                HalogramMethods.Warp => (Warp)option switch
                {
                    Warp.None => string.Empty,
                    Warp.From_Texture => string.Empty,
                    Warp.Parallax_Simple => string.Empty,
                    _ => null,
                },
                HalogramMethods.Overlay => (Overlay)option switch
                {
                    Overlay.None => string.Empty,
                    Overlay.Additive => string.Empty,
                    Overlay.Additive_Detail => string.Empty,
                    Overlay.Multiply => string.Empty,
                    Overlay.Multiply_And_Additive_Detail => string.Empty,
                    _ => null,
                },
                HalogramMethods.Edge_Fade => (Edge_Fade)option switch
                {
                    Edge_Fade.None => string.Empty,
                    Edge_Fade.Simple => string.Empty,
                    _ => null,
                },
                HalogramMethods.Distortion => (Distortion)option switch
                {
                    Distortion.Off => string.Empty,
                    Distortion.On => string.Empty,
                    _ => null,
                },
                HalogramMethods.Soft_Fade => (Soft_Fade)option switch
                {
                    Soft_Fade.Off => string.Empty,
                    Soft_Fade.On => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
