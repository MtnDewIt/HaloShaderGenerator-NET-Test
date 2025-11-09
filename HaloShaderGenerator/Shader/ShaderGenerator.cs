using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Shader
{
    public class ShaderGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(ShaderMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (ShaderMethods)methodIndex switch
            {
                ShaderMethods.Albedo => Enum.GetValues(typeof(Albedo)).Length,
                ShaderMethods.Bump_Mapping => Enum.GetValues(typeof(Bump_Mapping)).Length,
                ShaderMethods.Alpha_Test => Enum.GetValues(typeof(Alpha_Test)).Length,
                ShaderMethods.Specular_Mask => Enum.GetValues(typeof(Specular_Mask)).Length,
                ShaderMethods.Material_Model => Enum.GetValues(typeof(Material_Model)).Length,
                ShaderMethods.Environment_Mapping => Enum.GetValues(typeof(Environment_Mapping)).Length,
                ShaderMethods.Self_Illumination => Enum.GetValues(typeof(Self_Illumination)).Length,
                ShaderMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)).Length,
                ShaderMethods.Parallax => Enum.GetValues(typeof(Parallax)).Length,
                ShaderMethods.Misc => Enum.GetValues(typeof(Misc)).Length,
                ShaderMethods.Distortion => Enum.GetValues(typeof(Distortion)).Length,
                ShaderMethods.Soft_Fade => Enum.GetValues(typeof(Soft_Fade)).Length,
                ShaderMethods.Misc_Attr_Animation => Enum.GetValues(typeof(Misc_Attr_Animation)).Length,
                ShaderMethods.Wetness => Enum.GetValues(typeof(Wetness)).Length,
                ShaderMethods.Alpha_Blend_Source => Enum.GetValues(typeof(Alpha_Blend_Source)).Length,
                _ => -1,
            };
        }

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint) 
        {
            return entryPoint switch
            {
                ShaderStage.Shadow_Generate or 
                ShaderStage.Dynamic_Light_Cinematic => 2,
                _ => -1,
            };
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            return entryPoint switch
            {
                ShaderStage.Shadow_Generate or 
                ShaderStage.Dynamic_Light_Cinematic => true,
                _ => false,
            };
        }

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
                        result.AddSamplerParameter("camouflage_change_color_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatParameter("camouflage_scale");
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
                        result.AddFloatParameter("approximate_specular_type");
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
                        result.AddFloatParameter("roughness", 0.5f);
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
                        result.AddSamplerExternAddressParameter("g_diffuse_power_specular", RenderMethodExtern.material_diffuse_power, ShaderOptionParameter.ShaderAddressMode.Clamp);
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
                        result.AddSamplerExternAddressParameter("g_diffuse_power_specular", RenderMethodExtern.material_diffuse_power, ShaderOptionParameter.ShaderAddressMode.Clamp);
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
                    case Material_Model.Pbr:
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerParameter("material_texture", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddBooleanParameter("use_specular_tints");
                        result.AddFloat3ColorParameter("normal_specular", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat3ColorParameter("glancing_specular", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness", 5.0f);
                        result.AddFloatParameter("albedo_blend", 1.0f);
                        result.AddSamplerExternParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerExternParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerExternParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("cubemap_or_area_specular", 1.0f);
                        result.AddBooleanParameter("convert_material");
                        result.AddFloatParameter("roughness_bias");
                        result.AddFloatParameter("roughness_multiplier", 1.0f);
                        result.AddFloatParameter("metallic_bias");
                        result.AddFloatParameter("metallic_multiplier", 1.0f);
                        result.AddBooleanParameter("ct_spec_rough");
                        rmopName = @"shaders\shader_options\pbr";
                        break;
                    case Material_Model.Pbr_Spec_Gloss:
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerParameter("material_texture", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddBooleanParameter("use_specular_tints");
                        result.AddFloat3ColorParameter("normal_specular", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat3ColorParameter("glancing_specular", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness", 5.0f);
                        result.AddFloatParameter("albedo_blend", 1.0f);
                        result.AddFloatWithColorParameter("gloss_bias", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("gloss_multiplier", 1.0f);
                        result.AddFloat4ColorParameter("specular_tint", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("specular_bias");
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloatParameter("specular_coefficient", 1.0f);
                        rmopName = @"shaders\shader_options\pbr_spec_gloss";
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
                        result.AddFloatParameter("env_roughness_offset", 0.5f);
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
                        result.AddFloatParameter("env_roughness_offset", 0.5f);
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
                    case Environment_Mapping.Per_Pixel_Mip:
                        result.AddSamplerAddressWithColorParameter("environment_map", ShaderOptionParameter.ShaderAddressMode.Clamp, new ShaderColor(0, 255, 255, 255), @"shaders\default_bitmaps\bitmaps\default_dynamic_cube_map");
                        result.AddFloat3ColorParameter("env_tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("env_roughness_scale", 1.0f);
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.Dynamic_Expensive:
                        result.AddSamplerExternAddressParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddSamplerExternAddressParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        rmopName = @"shaders\shader_options\env_map_dynamic_expensive";
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
                        result.AddFloat3ColorExternParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
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
                        result.AddFloat3ColorExternParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
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
                        result.AddFloat3ColorExternParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
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

        public Array GetMethodNames() => Enum.GetValues(typeof(ShaderMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (ShaderMethods)methodIndex switch
            {
                ShaderMethods.Albedo => Enum.GetValues(typeof(Albedo)),
                ShaderMethods.Bump_Mapping => Enum.GetValues(typeof(Bump_Mapping)),
                ShaderMethods.Alpha_Test => Enum.GetValues(typeof(Alpha_Test)),
                ShaderMethods.Specular_Mask => Enum.GetValues(typeof(Specular_Mask)),
                ShaderMethods.Material_Model => Enum.GetValues(typeof(Material_Model)),
                ShaderMethods.Environment_Mapping => Enum.GetValues(typeof(Environment_Mapping)),
                ShaderMethods.Self_Illumination => Enum.GetValues(typeof(Self_Illumination)),
                ShaderMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)),
                ShaderMethods.Parallax => Enum.GetValues(typeof(Parallax)),
                ShaderMethods.Misc => Enum.GetValues(typeof(Misc)),
                ShaderMethods.Distortion => Enum.GetValues(typeof(Distortion)),
                ShaderMethods.Soft_Fade => Enum.GetValues(typeof(Soft_Fade)),
                ShaderMethods.Misc_Attr_Animation => Enum.GetValues(typeof(Misc_Attr_Animation)),
                ShaderMethods.Wetness => Enum.GetValues(typeof(Wetness)),
                ShaderMethods.Alpha_Blend_Source => Enum.GetValues(typeof(Alpha_Blend_Source)),
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

        public string GetCategoryPixelFunction(int category)
        {
            return (ShaderMethods)category switch
            {
                ShaderMethods.Albedo => "calc_albedo_ps",
                ShaderMethods.Bump_Mapping => "calc_bumpmap_ps",
                ShaderMethods.Alpha_Test => "calc_alpha_test_ps",
                ShaderMethods.Specular_Mask => "calc_specular_mask_ps",
                ShaderMethods.Material_Model => "material_type",
                ShaderMethods.Environment_Mapping => "envmap_type",
                ShaderMethods.Self_Illumination => "calc_self_illumination_ps",
                ShaderMethods.Blend_Mode => "blend_type",
                ShaderMethods.Parallax => "calc_parallax_ps",
                ShaderMethods.Misc => "bitmap_rotation",
                ShaderMethods.Distortion => "distort_proc_ps",
                ShaderMethods.Soft_Fade => "apply_soft_fade",
                ShaderMethods.Misc_Attr_Animation => string.Empty,
                ShaderMethods.Wetness => "calc_wetness_ps",
                ShaderMethods.Alpha_Blend_Source => "alpha_blend_source",
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (ShaderMethods)category switch
            {
                ShaderMethods.Albedo => "calc_albedo_vs",
                ShaderMethods.Bump_Mapping => "calc_bumpmap_vs",
                ShaderMethods.Alpha_Test => "alpha_test",
                ShaderMethods.Specular_Mask => string.Empty,
                ShaderMethods.Material_Model => string.Empty,
                ShaderMethods.Environment_Mapping => string.Empty,
                ShaderMethods.Self_Illumination => string.Empty,
                ShaderMethods.Blend_Mode => string.Empty,
                ShaderMethods.Parallax => "calc_parallax_vs",
                ShaderMethods.Misc => string.Empty,
                ShaderMethods.Distortion => "distort_proc_vs",
                ShaderMethods.Soft_Fade => string.Empty,
                ShaderMethods.Misc_Attr_Animation => string.Empty,
                ShaderMethods.Wetness => string.Empty,
                ShaderMethods.Alpha_Blend_Source => string.Empty,
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (ShaderMethods)category switch
            {
                ShaderMethods.Albedo => (Albedo)option switch
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
                    Albedo.Two_Change_Color_Anim_Overlay => "calc_albedo_two_change_color_anim_ps",
                    Albedo.Chameleon => "calc_albedo_chameleon_ps",
                    Albedo.Two_Change_Color_Chameleon => "calc_albedo_two_change_color_chameleon_ps",
                    Albedo.Chameleon_Masked => "calc_albedo_chameleon_masked_ps",
                    Albedo.Color_Mask_Hard_Light => "calc_albedo_color_mask_hard_light_ps",
                    Albedo.Four_Change_Color_Applying_To_Specular => "calc_albedo_four_change_color_applying_to_specular_ps",
                    Albedo.Simple => "calc_albedo_simple_ps",
                    Albedo.Two_Change_Color_Tex_Overlay => "calc_albedo_two_change_color_tex_overlay_ps",
                    Albedo.Chameleon_Albedo_Masked => "calc_albedo_chameleon_albedo_masked_ps",
                    Albedo.Custom_Cube => "calc_albedo_custom_cube_ps",
                    Albedo.Two_Color => "calc_albedo_two_color_ps",
                    Albedo.Emblem => "calc_albedo_emblem_ps",
                    Albedo.Scrolling_Cube_Mask => "calc_albedo_scrolling_cube_mask_ps",
                    Albedo.Scrolling_Cube => "calc_albedo_scrolling_cube_ps",
                    Albedo.Scrolling_Texture_Uv => "calc_albedo_scrolling_texture_uv_ps",
                    Albedo.Texture_From_Misc => "calc_albedo_texture_from_misc_ps",
                    _ => null,
                },
                ShaderMethods.Bump_Mapping => (Bump_Mapping)option switch
                {
                    Bump_Mapping.Off => "calc_bumpmap_off_ps",
                    Bump_Mapping.Standard => "calc_bumpmap_default_ps",
                    Bump_Mapping.Detail => "calc_bumpmap_detail_ps",
                    Bump_Mapping.Detail_Masked => "calc_bumpmap_detail_masked_ps",
                    Bump_Mapping.Detail_Plus_Detail_Masked => "calc_bumpmap_detail_plus_detail_masked_ps",
                    Bump_Mapping.Detail_Unorm => "calc_bumpmap_detail_unorm_ps",
                    Bump_Mapping.Detail_Blend => "calc_bumpmap_detail_blend_ps",
                    Bump_Mapping.Three_Detail_Blend => "calc_bumpmap_three_detail_blend_ps",
                    Bump_Mapping.Standard_Wrinkle => "calc_bumpmap_default_wrinkle_ps",
                    Bump_Mapping.Detail_Wrinkle => "calc_bumpmap_detail_wrinkle_ps",
                    _ => null,
                },
                ShaderMethods.Alpha_Test => (Alpha_Test)option switch
                {
                    Alpha_Test.None => "calc_alpha_test_off_ps",
                    Alpha_Test.Simple => "calc_alpha_test_on_ps",
                    _ => null,
                },
                ShaderMethods.Specular_Mask => (Specular_Mask)option switch
                {
                    Specular_Mask.No_Specular_Mask => "calc_specular_mask_no_specular_mask_ps",
                    Specular_Mask.Specular_Mask_From_Diffuse => "calc_specular_mask_from_diffuse_ps",
                    Specular_Mask.Specular_Mask_From_Texture => "calc_specular_mask_texture_ps",
                    Specular_Mask.Specular_Mask_From_Color_Texture => "calc_specular_mask_color_texture_ps",
                    Specular_Mask.Specular_Mask_Mult_Diffuse => "calc_specular_mask_mult_texture_ps",
                    _ => null,
                },
                ShaderMethods.Material_Model => (Material_Model)option switch
                {
                    Material_Model.Diffuse_Only => "diffuse_only",
                    Material_Model.Cook_Torrance_Rim_Fresnel => "cook_torrance_rim_fresnel",
                    Material_Model.Two_Lobe_Phong => "two_lobe_phong",
                    Material_Model.Foliage => "foliage",
                    Material_Model.None => "none",
                    Material_Model.Glass => "glass",
                    Material_Model.Organism => "organism",
                    Material_Model.Single_Lobe_Phong => "single_lobe_phong",
                    Material_Model.Car_Paint => "car_paint",
                    Material_Model.Hair => "hair",
                    Material_Model.Cook_Torrance => "cook_torrance",
                    Material_Model.Cook_Torrance_Pbr_Maps => "cook_torrance_pbr_maps",
                    Material_Model.Two_Lobe_Phong_Tint_Map => "two_lobe_phong_tint_map",
                    Material_Model.Cook_Torrance_Reach => "cook_torrance_reach",
                    Material_Model.Two_Lobe_Phong_Reach => "two_lobe_phong_reach",
                    Material_Model.Cook_Torrance_Custom_Cube => "cook_torrance_custom_cube",
                    Material_Model.Cook_Torrance_Two_Color_Spec_Tint => "cook_torrance_two_color_spec_tint",
                    Material_Model.Cook_Torrance_Scrolling_Cube_Mask => "cook_torrance_scrolling_cube_mask",
                    Material_Model.Cook_Torrance_Scrolling_Cube => "cook_torrance_scrolling_cube",
                    Material_Model.Cook_Torrance_From_Albedo => "cook_torrance_from_albedo",
                    Material_Model.Pbr => "pbr",
                    Material_Model.Pbr_Spec_Gloss => "pbr_spec_gloss",
                    _ => null,
                },
                ShaderMethods.Environment_Mapping => (Environment_Mapping)option switch
                {
                    Environment_Mapping.None => "none",
                    Environment_Mapping.Per_Pixel => "per_pixel",
                    Environment_Mapping.Dynamic => "dynamic",
                    Environment_Mapping.From_Flat_Texture => "from_flat_texture",
                    Environment_Mapping.Custom_Map => "custom_map",
                    Environment_Mapping.Dynamic_Reach => "dynamic_reach",
                    Environment_Mapping.From_Flat_Texture_As_Cubemap => "from_flat_texture_as_cubemap",
                    Environment_Mapping.Per_Pixel_Mip => "per_pixel_mip",
                    Environment_Mapping.Dynamic_Expensive => "dynamic_expensive",
                    _ => null,
                },
                ShaderMethods.Self_Illumination => (Self_Illumination)option switch
                {
                    Self_Illumination.Off => "calc_self_illumination_none_ps",
                    Self_Illumination.Simple => "calc_self_illumination_simple_ps",
                    Self_Illumination._3_Channel_Self_Illum => "calc_self_illumination_three_channel_ps",
                    Self_Illumination.Plasma => "calc_self_illumination_plasma_ps",
                    Self_Illumination.From_Diffuse => "calc_self_illumination_from_albedo_ps",
                    Self_Illumination.Illum_Detail => "calc_self_illumination_detail_ps",
                    Self_Illumination.Meter => "calc_self_illumination_meter_ps",
                    Self_Illumination.Self_Illum_Times_Diffuse => "calc_self_illumination_times_diffuse_ps",
                    Self_Illumination.Simple_With_Alpha_Mask => "calc_self_illumination_simple_with_alpha_mask_ps",
                    Self_Illumination.Simple_Four_Change_Color => "calc_self_illumination_simple_ps",
                    Self_Illumination.Illum_Change_Color => "calc_self_illumination_change_color_ps",
                    Self_Illumination.Multilayer_Additive => "calc_self_illumination_multilayer_ps",
                    Self_Illumination.Palettized_Plasma => "calc_self_illumination_palettized_plasma_ps",
                    Self_Illumination.Change_Color_Detail => "calc_self_illumination_change_color_detail_ps",
                    Self_Illumination.Illum_Detail_World_Space_Four_Cc => "calc_self_illumination_detail_world_space_ps",
                    Self_Illumination.Change_Color => "calc_self_illumination_change_color_ps",
                    _ => null,
                },
                ShaderMethods.Blend_Mode => (Blend_Mode)option switch
                {
                    Blend_Mode.Opaque => "opaque",
                    Blend_Mode.Additive => "additive",
                    Blend_Mode.Multiply => "multiply",
                    Blend_Mode.Alpha_Blend => "alpha_blend",
                    Blend_Mode.Double_Multiply => "double_multiply",
                    Blend_Mode.Pre_Multiplied_Alpha => "pre_multiplied_alpha",
                    _ => null,
                },
                ShaderMethods.Parallax => (Parallax)option switch
                {
                    Parallax.Off => "calc_parallax_off_ps",
                    Parallax.Simple => "calc_parallax_simple_ps",
                    Parallax.Interpolated => "calc_parallax_interpolated_ps",
                    Parallax.Simple_Detail => "calc_parallax_simple_detail_ps",
                    _ => null,
                },
                ShaderMethods.Misc => (Misc)option switch
                {
                    Misc.First_Person_Never => "0",
                    Misc.First_Person_Sometimes => "0",
                    Misc.First_Person_Always => "0",
                    Misc.First_Person_Never_With_Rotating_Bitmaps => "1",
                    Misc.Always_Calc_Albedo => "0",
                    Misc.Default => "0",
                    Misc.Rotating_Bitmaps_Super_Slow => "1",
                    _ => null,
                },
                ShaderMethods.Distortion => (Distortion)option switch
                {
                    Distortion.Off => "distort_off_ps",
                    Distortion.On => "distort_on_ps",
                    _ => null,
                },
                ShaderMethods.Soft_Fade => (Soft_Fade)option switch
                {
                    Soft_Fade.Off => "apply_soft_fade_off",
                    Soft_Fade.On => "apply_soft_fade_on",
                    _ => null,
                },
                ShaderMethods.Misc_Attr_Animation => (Misc_Attr_Animation)option switch
                {
                    Misc_Attr_Animation.Off => string.Empty,
                    Misc_Attr_Animation.Scrolling_Cube => string.Empty,
                    Misc_Attr_Animation.Scrolling_Projected => string.Empty,
                    _ => null,
                },
                ShaderMethods.Wetness => (Wetness)option switch
                {
                    Wetness.Default => "calc_wetness_default_ps",
                    Wetness.Flood => "calc_wetness_flood_ps",
                    Wetness.Proof => "calc_wetness_proof_ps",
                    Wetness.Simple => "calc_wetness_simple_ps",
                    Wetness.Ripples => "calc_wetness_ripples_ps",
                    _ => null,
                },
                ShaderMethods.Alpha_Blend_Source => (Alpha_Blend_Source)option switch
                {
                    Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel => "albedo_alpha_without_fresnel",
                    Alpha_Blend_Source.From_Albedo_Alpha => "albedo_alpha",
                    Alpha_Blend_Source.From_Opacity_Map_Alpha => "opacity_map_alpha",
                    Alpha_Blend_Source.From_Opacity_Map_Rgb => "opacity_map_rgb",
                    Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha => "opacity_map_alpha_and_albedo_alpha",
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (ShaderMethods)category switch
            {
                ShaderMethods.Albedo => (Albedo)option switch
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
                    Albedo.Two_Change_Color_Anim_Overlay => "calc_albedo_default_vs",
                    Albedo.Chameleon => "calc_albedo_default_vs",
                    Albedo.Two_Change_Color_Chameleon => "calc_albedo_default_vs",
                    Albedo.Chameleon_Masked => "calc_albedo_default_vs",
                    Albedo.Color_Mask_Hard_Light => "calc_albedo_default_vs",
                    Albedo.Four_Change_Color_Applying_To_Specular => "calc_albedo_default_vs",
                    Albedo.Simple => "calc_albedo_default_vs",
                    Albedo.Two_Change_Color_Tex_Overlay => "calc_albedo_default_vs",
                    Albedo.Chameleon_Albedo_Masked => "calc_albedo_default_vs",
                    Albedo.Custom_Cube => "calc_albedo_default_vs",
                    Albedo.Two_Color => "calc_albedo_default_vs",
                    Albedo.Emblem => "calc_albedo_default_vs",
                    Albedo.Scrolling_Cube_Mask => "calc_albedo_default_vs",
                    Albedo.Scrolling_Cube => "calc_albedo_default_vs",
                    Albedo.Scrolling_Texture_Uv => "calc_albedo_default_vs",
                    Albedo.Texture_From_Misc => "calc_albedo_default_vs",
                    _ => null,
                },
                ShaderMethods.Bump_Mapping => (Bump_Mapping)option switch
                {
                    Bump_Mapping.Off => "calc_bumpmap_off_vs",
                    Bump_Mapping.Standard => "calc_bumpmap_default_vs",
                    Bump_Mapping.Detail => "calc_bumpmap_detail_vs",
                    Bump_Mapping.Detail_Masked => "calc_bumpmap_detail_vs",
                    Bump_Mapping.Detail_Plus_Detail_Masked => "calc_bumpmap_default_vs",
                    Bump_Mapping.Detail_Unorm => "calc_bumpmap_default_vs",
                    Bump_Mapping.Detail_Blend => "calc_bumpmap_detail_blend_vs",
                    Bump_Mapping.Three_Detail_Blend => "calc_bumpmap_three_detail_blend_vs",
                    Bump_Mapping.Standard_Wrinkle => "calc_bumpmap_default_wrinkle_vs",
                    Bump_Mapping.Detail_Wrinkle => "calc_bumpmap_detail_wrinkle_vs",
                    _ => null,
                },
                ShaderMethods.Alpha_Test => (Alpha_Test)option switch
                {
                    Alpha_Test.None => "off",
                    Alpha_Test.Simple => "on",
                    _ => null,
                },
                ShaderMethods.Specular_Mask => (Specular_Mask)option switch
                {
                    Specular_Mask.No_Specular_Mask => string.Empty,
                    Specular_Mask.Specular_Mask_From_Diffuse => string.Empty,
                    Specular_Mask.Specular_Mask_From_Texture => string.Empty,
                    Specular_Mask.Specular_Mask_From_Color_Texture => string.Empty,
                    Specular_Mask.Specular_Mask_Mult_Diffuse => string.Empty,
                    _ => null,
                },
                ShaderMethods.Material_Model => (Material_Model)option switch
                {
                    Material_Model.Diffuse_Only => string.Empty,
                    Material_Model.Cook_Torrance_Rim_Fresnel => string.Empty,
                    Material_Model.Two_Lobe_Phong => string.Empty,
                    Material_Model.Foliage => string.Empty,
                    Material_Model.None => string.Empty,
                    Material_Model.Glass => string.Empty,
                    Material_Model.Organism => string.Empty,
                    Material_Model.Single_Lobe_Phong => string.Empty,
                    Material_Model.Car_Paint => string.Empty,
                    Material_Model.Hair => string.Empty,
                    Material_Model.Cook_Torrance => string.Empty,
                    Material_Model.Cook_Torrance_Pbr_Maps => string.Empty,
                    Material_Model.Two_Lobe_Phong_Tint_Map => string.Empty,
                    Material_Model.Cook_Torrance_Reach => string.Empty,
                    Material_Model.Two_Lobe_Phong_Reach => string.Empty,
                    Material_Model.Cook_Torrance_Custom_Cube => string.Empty,
                    Material_Model.Cook_Torrance_Two_Color_Spec_Tint => string.Empty,
                    Material_Model.Cook_Torrance_Scrolling_Cube_Mask => string.Empty,
                    Material_Model.Cook_Torrance_Scrolling_Cube => string.Empty,
                    Material_Model.Cook_Torrance_From_Albedo => string.Empty,
                    Material_Model.Pbr => string.Empty,
                    Material_Model.Pbr_Spec_Gloss => string.Empty,
                    _ => null,
                },
                ShaderMethods.Environment_Mapping => (Environment_Mapping)option switch
                {
                    Environment_Mapping.None => string.Empty,
                    Environment_Mapping.Per_Pixel => string.Empty,
                    Environment_Mapping.Dynamic => string.Empty,
                    Environment_Mapping.From_Flat_Texture => string.Empty,
                    Environment_Mapping.Custom_Map => string.Empty,
                    Environment_Mapping.Dynamic_Reach => string.Empty,
                    Environment_Mapping.From_Flat_Texture_As_Cubemap => string.Empty,
                    Environment_Mapping.Per_Pixel_Mip => string.Empty,
                    Environment_Mapping.Dynamic_Expensive => string.Empty,
                    _ => null,
                },
                ShaderMethods.Self_Illumination => (Self_Illumination)option switch
                {
                    Self_Illumination.Off => string.Empty,
                    Self_Illumination.Simple => string.Empty,
                    Self_Illumination._3_Channel_Self_Illum => string.Empty,
                    Self_Illumination.Plasma => string.Empty,
                    Self_Illumination.From_Diffuse => string.Empty,
                    Self_Illumination.Illum_Detail => string.Empty,
                    Self_Illumination.Meter => string.Empty,
                    Self_Illumination.Self_Illum_Times_Diffuse => string.Empty,
                    Self_Illumination.Simple_With_Alpha_Mask => string.Empty,
                    Self_Illumination.Simple_Four_Change_Color => string.Empty,
                    Self_Illumination.Illum_Change_Color => string.Empty,
                    Self_Illumination.Multilayer_Additive => string.Empty,
                    Self_Illumination.Palettized_Plasma => string.Empty,
                    Self_Illumination.Change_Color_Detail => string.Empty,
                    Self_Illumination.Illum_Detail_World_Space_Four_Cc => string.Empty,
                    Self_Illumination.Change_Color => string.Empty,
                    _ => null,
                },
                ShaderMethods.Blend_Mode => (Blend_Mode)option switch
                {
                    Blend_Mode.Opaque => string.Empty,
                    Blend_Mode.Additive => string.Empty,
                    Blend_Mode.Multiply => string.Empty,
                    Blend_Mode.Alpha_Blend => string.Empty,
                    Blend_Mode.Double_Multiply => string.Empty,
                    Blend_Mode.Pre_Multiplied_Alpha => string.Empty,
                    _ => null,
                },
                ShaderMethods.Parallax => (Parallax)option switch
                {
                    Parallax.Off => "calc_parallax_off_vs",
                    Parallax.Simple => "calc_parallax_simple_vs",
                    Parallax.Interpolated => "calc_parallax_interpolated_vs",
                    Parallax.Simple_Detail => "calc_parallax_simple_vs",
                    _ => null,
                },
                ShaderMethods.Misc => (Misc)option switch
                {
                    Misc.First_Person_Never => string.Empty,
                    Misc.First_Person_Sometimes => string.Empty,
                    Misc.First_Person_Always => string.Empty,
                    Misc.First_Person_Never_With_Rotating_Bitmaps => string.Empty,
                    Misc.Always_Calc_Albedo => string.Empty,
                    Misc.Default => string.Empty,
                    Misc.Rotating_Bitmaps_Super_Slow => string.Empty,
                    _ => null,
                },
                ShaderMethods.Distortion => (Distortion)option switch
                {
                    Distortion.Off => "distort_nocolor_vs",
                    Distortion.On => "distort_nocolor_vs",
                    _ => null,
                },
                ShaderMethods.Soft_Fade => (Soft_Fade)option switch
                {
                    Soft_Fade.Off => string.Empty,
                    Soft_Fade.On => string.Empty,
                    _ => null,
                },
                ShaderMethods.Misc_Attr_Animation => (Misc_Attr_Animation)option switch
                {
                    Misc_Attr_Animation.Off => string.Empty,
                    Misc_Attr_Animation.Scrolling_Cube => string.Empty,
                    Misc_Attr_Animation.Scrolling_Projected => string.Empty,
                    _ => null,
                },
                ShaderMethods.Wetness => (Wetness)option switch
                {
                    Wetness.Default => string.Empty,
                    Wetness.Flood => string.Empty,
                    Wetness.Proof => string.Empty,
                    Wetness.Simple => string.Empty,
                    Wetness.Ripples => string.Empty,
                    _ => null,
                },
                ShaderMethods.Alpha_Blend_Source => (Alpha_Blend_Source)option switch
                {
                    Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel => string.Empty,
                    Alpha_Blend_Source.From_Albedo_Alpha => string.Empty,
                    Alpha_Blend_Source.From_Opacity_Map_Alpha => string.Empty,
                    Alpha_Blend_Source.From_Opacity_Map_Rgb => string.Empty,
                    Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
