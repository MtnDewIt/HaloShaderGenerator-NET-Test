using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Custom
{
    public class CustomGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(CustomMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((CustomMethods)methodIndex)
            {
                case CustomMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case CustomMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping)).Length;
                case CustomMethods.Alpha_Test:
                    return Enum.GetValues(typeof(Alpha_Test)).Length;
                case CustomMethods.Specular_Mask:
                    return Enum.GetValues(typeof(Specular_Mask)).Length;
                case CustomMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model)).Length;
                case CustomMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping)).Length;
                case CustomMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination)).Length;
                case CustomMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
                case CustomMethods.Parallax:
                    return Enum.GetValues(typeof(Parallax)).Length;
                case CustomMethods.Misc:
                    return Enum.GetValues(typeof(Misc)).Length;
                case CustomMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness)).Length;
            }

            return -1;
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
                case ShaderStage.Stipple:
                case ShaderStage.Single_Pass_Per_Pixel:
                case ShaderStage.Single_Pass_Per_Vertex:
                //case ShaderStage.Single_Pass_Single_Probe:
                //case ShaderStage.Single_Pass_Single_Probe_Ambient:
                //case ShaderStage.Imposter_Static_Sh:
                //case ShaderStage.Imposter_Static_Prt_Ambient:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsMethodSharedInEntryPoint(ShaderStage entryPoint, int method_index)
        {
            switch (method_index)
            {
                case 2:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Shadow_Generate:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsSharedPixelShaderWithoutMethod(ShaderStage entryPoint)
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

        public bool IsVertexShaderShared(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Albedo:
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Static_Sh:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Dynamic_Light:
                case ShaderStage.Shadow_Generate:
                case ShaderStage.Static_Per_Vertex_Color:
                case ShaderStage.Dynamic_Light_Cinematic:
                case ShaderStage.Z_Only:
                    return true;
                default:
                    return false;
            }
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();
            result.AddFloat3ColorParameter("debug_tint", RenderMethodExtern.debug_tint, default, default, default, default, new ShaderColor(255, 255, 255, 255));
            result.AddSamplerParameter("active_camo_distortion_texture", RenderMethodExtern.active_camo_distortion_texture, default, default, default, default, default);
            result.AddSamplerParameter("albedo_texture", RenderMethodExtern.texture_global_target_texaccum, default, default, default, default, default);
            result.AddSamplerParameter("dominant_light_intensity_map", RenderMethodExtern.texture_dominant_light_intensity_map, default, default, default, default, default);
            result.AddSamplerParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0, default, default, default, default, default);
            result.AddSamplerParameter("g_diffuse_power_specular", default, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"rasterizer\diffuse_power_specular\diffuse_power");
            result.AddSamplerParameter("g_direction_lut", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"rasterizer\direction_lut_1002");
            result.AddSamplerParameter("g_sample_vmf_diffuse_vs", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"rasterizer\diffusetable");
            result.AddSamplerParameter("g_sample_vmf_diffuse", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"rasterizer\diffusetable");
            result.AddSamplerParameter("g_sample_vmf_phong_specular", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"rasterizer\diffuse_power_specular\diffuse_power");
            result.AddSamplerParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture, ShaderOptionParameter.ShaderFilterMode.Bilinear, default, default, default, default);
            result.AddSamplerParameter("normal_texture", RenderMethodExtern.texture_global_target_normal, default, default, default, default, default);
            result.AddSamplerParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture, default, default, default, default, default);
            result.AddSamplerParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture, default, default, default, default, default);
            result.AddSamplerParameter("shadow_depth_map_1", RenderMethodExtern.texture_global_target_shadow_buffer1, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
            result.AddSamplerParameter("shadow_mask_texture", default, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default); // rmExtern - texture_global_target_shadow_mask
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
                        result.AddFloat4ColorParameter("albedo_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_default";
                        break;
                    case Albedo.Detail_Blend:
                        result.AddFloatParameter("blend_alpha", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map2", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_detail_blend";
                        break;
                    case Albedo.Constant_Color:
                        result.AddFloat4ColorParameter("albedo_color", default, default, default, default, default, new ShaderColor(255, 192, 192, 192));
                        rmopName = @"shaders\shader_options\albedo_constant";
                        break;
                    case Albedo.Two_Change_Color:
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, default, default, default, default, default);
                        result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary, default, default, default, default, default);
                        result.AddFloatParameter("camouflage_scale", default, default, default, default, default, default);
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("camouflage_change_color_map", default, default, default, default, default, @"rasterizer\invalid");
                        result.AddSamplerParameter("change_color_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_change_color";
                        break;
                    case Albedo.Four_Change_Color:
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, default, default, default, default, default);
                        result.AddFloat3ColorParameter("quaternary_change_color", RenderMethodExtern.object_change_color_quaternary, default, default, default, default, default);
                        result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary, default, default, default, default, default);
                        result.AddFloat3ColorParameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary, default, default, default, default, default);
                        result.AddFloatParameter("camouflage_scale", default, default, default, default, default, default);
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("camouflage_change_color_map", default, default, default, default, default, default);
                        result.AddSamplerParameter("change_color_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_four_change_color";
                        break;
                    case Albedo.Three_Detail_Blend:
                        result.AddFloatParameter("blend_alpha", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map2", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map3", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_three_detail_blend";
                        break;
                    case Albedo.Two_Detail_Overlay:
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_overlay", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map2", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_detail_overlay";
                        break;
                    case Albedo.Two_Detail:
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map2", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_detail";
                        break;
                    case Albedo.Color_Mask:
                        result.AddFloat3ColorParameter("neutral_gray", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("albedo_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("albedo_color2", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("albedo_color3", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("color_mask_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\reference_grids");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_color_mask";
                        break;
                    case Albedo.Two_Detail_Black_Point:
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map2", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_detail_black_point";
                        break;
                    case Albedo.Waterfall:
                        result.AddFloatParameter("transparency_base_weight", default, default, default, default, default, 3f);
                        result.AddFloatParameter("transparency_bias", default, default, default, default, default, -3f);
                        result.AddFloatParameter("transparency_frothy_weight", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("waterfall_base_mask", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerParameter("waterfall_layer0", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerParameter("waterfall_layer1", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerParameter("waterfall_layer2", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\custom_options\albedo_waterfall";
                        break;
                    case Albedo.Multiply_Map:
                        break;
                    case Albedo.Simple:
                        result.AddFloat4ColorParameter("albedo_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\albedo_simple";
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
                        result.AddSamplerParameter("bump_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\bump_default";
                        break;
                    case Bump_Mapping.Detail:
                        result.AddFloatParameter("bump_detail_coefficient", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("bump_detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\bump_detail";
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
                        result.AddSamplerParameter("alpha_test_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_alpha_test");
                        rmopName = @"shaders\shader_options\alpha_test_on";
                        break;
                    case Alpha_Test.Multiply_Map:
                        result.AddSamplerParameter("alpha_test_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_alpha_test");
                        result.AddSamplerParameter("multiply_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_alpha_test");
                        rmopName = @"shaders\custom_options\alpha_test_multiply_map";
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
                        result.AddSamplerParameter("specular_mask_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\shader_options\specular_mask_from_texture";
                        break;
                    case Specular_Mask.Specular_Mask_Mult_Diffuse:
                        result.AddSamplerParameter("specular_mask_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\shader_options\specular_mask_mult_diffuse";
                        break;
                    case Specular_Mask.Specular_Mask_From_Color_Texture:
                        break;
                }
            }

            if (methodName == "material_model")
            {
                optionName = ((Material_Model)option).ToString();

                switch ((Material_Model)option)
                {
                    case Material_Model.Diffuse_Only:
                        break;
                    case Material_Model.Two_Lobe_Phong:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("glancing_specular_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("normal_specular_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_color_by_angle", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_anti_shadow_control", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_power", default, default, default, default, default, 25f);
                        result.AddFloatParameter("analytical_roughness", default, default, default, default, default, 0.02f);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("approximate_specular_type", default, default, default, default, default, default);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("fresnel_curve_steepness", default, default, default, default, default, 5f);
                        result.AddFloatParameter("glancing_specular_power", default, default, default, default, default, 10f);
                        result.AddFloatParameter("normal_specular_power", default, default, default, default, default, 10f);
                        result.AddFloatParameter("roughness", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, 1f);
                        rmopName = @"shaders\shader_options\material_two_lobe_phong_option";
                        break;
                    case Material_Model.Foliage:
                        break;
                    case Material_Model.None:
                        break;
                    case Material_Model.Custom_Specular:
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, default);
                        result.AddSamplerParameter("glancing_falloff", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
                        result.AddSamplerParameter("material_map", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("specular_lobe", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"shaders\default_bitmaps\bitmaps\checker_board");
                        rmopName = @"shaders\custom_options\material_custom_specular";
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
                        result.AddFloat3ColorParameter("env_tint_color", default, default, default, default, default, default);
                        result.AddFloatParameter("env_roughness_offset", default, default, default, default, default, 0.5f);
                        result.AddSamplerParameter("environment_map", default, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"shaders\default_bitmaps\bitmaps\default_dynamic_cube_map");
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.Dynamic:
                        result.AddFloat3ColorParameter("env_tint_color", default, default, default, default, default, default);
                        result.AddFloatParameter("env_roughness_offset", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("env_roughness_scale", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
                        result.AddSamplerParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
                        rmopName = @"shaders\shader_options\env_map_dynamic";
                        break;
                    case Environment_Mapping.From_Flat_Texture:
                        result.AddFloat3ColorParameter("env_tint_color", default, default, default, default, default, default);
                        result.AddFloat4ColorParameter("env_bloom_override", default, default, default, default, default, default);
                        result.AddFloat4ColorParameter("flat_envmap_matrix_x", RenderMethodExtern.flat_envmap_matrix_x, default, default, default, default, default);
                        result.AddFloat4ColorParameter("flat_envmap_matrix_y", RenderMethodExtern.flat_envmap_matrix_y, default, default, default, default, default);
                        result.AddFloat4ColorParameter("flat_envmap_matrix_z", RenderMethodExtern.flat_envmap_matrix_z, default, default, default, default, default);
                        result.AddFloatParameter("env_bloom_override_intensity", default, default, default, default, default, 1f);
                        result.AddFloatParameter("hemisphere_percentage", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("flat_environment_map", default, default, ShaderOptionParameter.ShaderAddressMode.BlackBorder, default, default, @"shaders\default_bitmaps\bitmaps\color_red");
                        rmopName = @"shaders\shader_options\env_map_from_flat_texture";
                        break;
                    case Environment_Mapping.Per_Pixel_Mip:
                        result.AddFloat3ColorParameter("env_tint_color", default, default, default, default, default, default);
                        result.AddFloatParameter("env_roughness_offset", default, default, default, default, default, 0.5f);
                        result.AddSamplerParameter("environment_map", default, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"shaders\default_bitmaps\bitmaps\default_dynamic_cube_map");
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.Dynamic_Reach:
                        result.AddFloat3ColorParameter("env_tint_color", default, default, default, default, default, default);
                        result.AddFloatParameter("env_roughness_offset", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("env_roughness_scale", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
                        result.AddSamplerParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
                        rmopName = @"shaders\shader_options\env_map_dynamic";
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
                        result.AddFloat3ColorParameter("self_illum_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_simple";
                        break;
                    case Self_Illumination._3_Channel_Self_Illum:
                        result.AddFloat4ColorParameter("channel_a", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("channel_b", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("channel_c", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_3_channel";
                        break;
                    case Self_Illumination.Plasma:
                        result.AddFloat4ColorParameter("color_medium", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("color_sharp", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("color_wide", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddFloatParameter("thinness_medium", default, default, default, default, default, 16f);
                        result.AddFloatParameter("thinness_sharp", default, default, default, default, default, 32f);
                        result.AddFloatParameter("thinness_wide", default, default, default, default, default, 4f);
                        result.AddSamplerParameter("alpha_mask_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\alpha_white");
                        result.AddSamplerParameter("noise_map_a", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("noise_map_b", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_plasma";
                        break;
                    case Self_Illumination.From_Diffuse:
                        result.AddFloat3ColorParameter("self_illum_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        rmopName = @"shaders\shader_options\illum_from_diffuse";
                        break;
                    case Self_Illumination.Illum_Detail:
                        result.AddFloat3ColorParameter("self_illum_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("self_illum_detail_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_detail";
                        break;
                    case Self_Illumination.Meter:
                        result.AddFloat3ColorParameter("meter_color_off", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("meter_color_on", default, default, default, default, default, default);
                        result.AddFloatParameter("meter_value", default, default, default, default, default, 0.5f);
                        result.AddSamplerParameter("meter_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\monochrome_alpha_grid");
                        rmopName = @"shaders\shader_options\illum_meter";
                        break;
                    case Self_Illumination.Self_Illum_Times_Diffuse:
                        result.AddFloat3ColorParameter("self_illum_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("primary_change_color_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_times_diffuse";
                        break;
                    case Self_Illumination.Window_Room:
                        result.AddFloatParameter("distance_fade_scale", default, default, default, default, default, 2f);
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("ceiling", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_blue");
                        //result.AddSamplerParameter("ceiling_reach", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_yellow"); // reach specific (could add as an extra sampler in the HLSL)
                        result.AddSamplerParameter("floors", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_green");
                        result.AddSamplerParameter("opacity_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("transform", default, default, default, default, default, default);
                        result.AddSamplerParameter("walls", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_red");
                        result.AddSamplerParameter("window_property_map", default, ShaderOptionParameter.ShaderFilterMode.Point, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white_alpha_black");
                        rmopName = @"shaders\custom_options\window_room_map";
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

            if (methodName == "parallax")
            {
                optionName = ((Parallax)option).ToString();

                switch ((Parallax)option)
                {
                    case Parallax.Off:
                        break;
                    case Parallax.Simple:
                        result.AddFloatParameter("height_scale", default, default, default, default, default, 0.1f);
                        result.AddSamplerParameter("height_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        rmopName = @"shaders\shader_options\parallax_simple";
                        break;
                    case Parallax.Interpolated:
                        result.AddFloatParameter("height_scale", default, default, default, default, default, 0.1f);
                        result.AddSamplerParameter("height_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        rmopName = @"shaders\shader_options\parallax_simple";
                        break;
                    case Parallax.Simple_Detail:
                        result.AddFloatParameter("height_scale", default, default, default, default, default, 0.1f);
                        result.AddSamplerParameter("height_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        result.AddSamplerParameter("height_scale_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
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
                    case Misc.Default:
                        break;
                    case Misc.Rotating_Bitmaps_Super_Slow:
                        break;
                    case Misc.Always_Calc_Albedo:
                        break;
                }
            }

            if (methodName == "wetness")
            {
                optionName = ((Wetness)option).ToString();

                switch ((Wetness)option)
                {
                    case Wetness.Default:
                        result.AddFloat3ColorParameter("wet_material_dim_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("wet_material_dim_coefficient", default, default, default, default, default, 1f);
                        rmopName = @"shaders\wetness_options\wetness_simple";
                        break;
                    case Wetness.Flood:
                        result.AddFloat3ColorParameter("wet_material_dim_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("wet_sheen_reflection_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_mask_tweak_weight", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("surface_tilt_tweak_weight", default, default, default, default, default, default);
                        result.AddFloatParameter("wet_material_dim_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("wet_sheen_reflection_contribution", default, default, default, default, default, 0.3f);
                        result.AddFloatParameter("wet_sheen_thickness", default, default, default, default, default, 0.9f);
                        result.AddSamplerParameter("wet_flood_slope_map", default, default, default, default, default, @"rasterizer\water\static_wave\static_wave_slope_water");
                        result.AddSamplerParameter("wet_noise_boundary_map", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, default, default, default, @"rasterizer\rain\rain_noise_boundary");
                        rmopName = @"shaders\wetness_options\wetness_flood";
                        break;
                    case Wetness.Proof:
                        break;
                    case Wetness.Ripples:
                        result.AddFloat3ColorParameter("wet_material_dim_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("wet_sheen_reflection_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_mask_tweak_weight", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("surface_tilt_tweak_weight", default, default, default, default, default, 0.3f);
                        result.AddFloatParameter("wet_material_dim_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("wet_sheen_reflection_contribution", default, default, default, default, default, 0.37f);
                        result.AddFloatParameter("wet_sheen_thickness", default, default, default, default, default, 0.4f);
                        result.AddSamplerParameter("wet_noise_boundary_map", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, default, default, default, @"rasterizer\rain\rain_noise_boundary");
                        rmopName = @"shaders\wetness_options\wetness_ripples";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(CustomMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((CustomMethods)methodIndex)
            {
                case CustomMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case CustomMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping));
                case CustomMethods.Alpha_Test:
                    return Enum.GetValues(typeof(Alpha_Test));
                case CustomMethods.Specular_Mask:
                    return Enum.GetValues(typeof(Specular_Mask));
                case CustomMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model));
                case CustomMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping));
                case CustomMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination));
                case CustomMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
                case CustomMethods.Parallax:
                    return Enum.GetValues(typeof(Parallax));
                case CustomMethods.Misc:
                    return Enum.GetValues(typeof(Misc));
                case CustomMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness));
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

            if (methodName == "wetness")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_wetness_ps";
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
                    case Albedo.Waterfall:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_waterfall_ps";
                        break;
                    case Albedo.Multiply_Map:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_multiply_map_ps";
                        break;
                    case Albedo.Simple:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_simple_ps";
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
                    case Alpha_Test.Multiply_Map:
                        vertexFunction = "multmap";
                        pixelFunction = "calc_alpha_test_multiply_map_ps";
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
                    case Specular_Mask.Specular_Mask_Mult_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_mult_texture_ps";
                        break;
                    case Specular_Mask.Specular_Mask_From_Color_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_color_texture_ps";
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
                    case Material_Model.Custom_Specular:
                        vertexFunction = "invalid";
                        pixelFunction = "custom_specular";
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
                    case Environment_Mapping.Per_Pixel_Mip:
                        vertexFunction = "invalid";
                        pixelFunction = "per_pixel_mip";
                        break;
                    case Environment_Mapping.Dynamic_Reach:
                        vertexFunction = "invalid";
                        pixelFunction = "dynamic_reach";
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
                    case Self_Illumination.Window_Room:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_window_room_ps";
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
                    case Misc.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                    case Misc.Rotating_Bitmaps_Super_Slow:
                        vertexFunction = "invalid";
                        pixelFunction = "1";
                        break;
                    case Misc.Always_Calc_Albedo:
                        vertexFunction = "invalid";
                        pixelFunction = "2";
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
                    case Wetness.Ripples:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_ripples_ps";
                        break;
                }
            }
        }

        public ShaderParameters GetParameterArguments(string methodName, int option)
        {
            ShaderParameters result = new ShaderParameters();
            if (methodName == "albedo")
            {
                switch ((Albedo)option)
                {
                    case Albedo.Default:
                        break;
                    case Albedo.Detail_Blend:
                        break;
                    case Albedo.Constant_Color:
                        break;
                    case Albedo.Two_Change_Color:
                        break;
                    case Albedo.Four_Change_Color:
                        break;
                    case Albedo.Three_Detail_Blend:
                        break;
                    case Albedo.Two_Detail_Overlay:
                        break;
                    case Albedo.Two_Detail:
                        break;
                    case Albedo.Color_Mask:
                        break;
                    case Albedo.Two_Detail_Black_Point:
                        break;
                    case Albedo.Waterfall:
                        break;
                    case Albedo.Multiply_Map:
                        break;
                    case Albedo.Simple:
                        break;
                }
            }

            if (methodName == "bump_mapping")
            {
                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Off:
                        break;
                    case Bump_Mapping.Standard:
                        break;
                    case Bump_Mapping.Detail:
                        break;
                }
            }

            if (methodName == "alpha_test")
            {
                switch ((Alpha_Test)option)
                {
                    case Alpha_Test.None:
                        break;
                    case Alpha_Test.Simple:
                        break;
                    case Alpha_Test.Multiply_Map:
                        break;
                }
            }

            if (methodName == "specular_mask")
            {
                switch ((Specular_Mask)option)
                {
                    case Specular_Mask.No_Specular_Mask:
                        break;
                    case Specular_Mask.Specular_Mask_From_Diffuse:
                        break;
                    case Specular_Mask.Specular_Mask_From_Texture:
                        break;
                    case Specular_Mask.Specular_Mask_Mult_Diffuse:
                        break;
                    case Specular_Mask.Specular_Mask_From_Color_Texture:
                        break;
                }
            }

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Diffuse_Only:
                        break;
                    case Material_Model.Two_Lobe_Phong:
                        break;
                    case Material_Model.Foliage:
                        break;
                    case Material_Model.None:
                        break;
                    case Material_Model.Custom_Specular:
                        break;
                }
            }

            if (methodName == "environment_mapping")
            {
                switch ((Environment_Mapping)option)
                {
                    case Environment_Mapping.None:
                        break;
                    case Environment_Mapping.Per_Pixel:
                        break;
                    case Environment_Mapping.Dynamic:
                        break;
                    case Environment_Mapping.From_Flat_Texture:
                        break;
                    case Environment_Mapping.Per_Pixel_Mip:
                        break;
                    case Environment_Mapping.Dynamic_Reach:
                        break;
                }
            }

            if (methodName == "self_illumination")
            {
                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.Off:
                        break;
                    case Self_Illumination.Simple:
                        break;
                    case Self_Illumination._3_Channel_Self_Illum:
                        break;
                    case Self_Illumination.Plasma:
                        break;
                    case Self_Illumination.From_Diffuse:
                        break;
                    case Self_Illumination.Illum_Detail:
                        break;
                    case Self_Illumination.Meter:
                        break;
                    case Self_Illumination.Self_Illum_Times_Diffuse:
                        break;
                    case Self_Illumination.Window_Room:
                        break;
                }
            }

            if (methodName == "blend_mode")
            {
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

            if (methodName == "parallax")
            {
                switch ((Parallax)option)
                {
                    case Parallax.Off:
                        break;
                    case Parallax.Simple:
                        break;
                    case Parallax.Interpolated:
                        break;
                    case Parallax.Simple_Detail:
                        break;
                }
            }

            if (methodName == "misc")
            {
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
                    case Misc.Default:
                        break;
                    case Misc.Rotating_Bitmaps_Super_Slow:
                        break;
                    case Misc.Always_Calc_Albedo:
                        break;
                }
            }

            if (methodName == "wetness")
            {
                switch ((Wetness)option)
                {
                    case Wetness.Default:
                        break;
                    case Wetness.Flood:
                        break;
                    case Wetness.Proof:
                        break;
                    case Wetness.Ripples:
                        break;
                }
            }
            return result;
        }
    }
}
