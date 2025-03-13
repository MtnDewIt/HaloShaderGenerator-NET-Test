using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Cortana
{
    public class CortanaGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(CortanaMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((CortanaMethods)methodIndex)
            {
                case CortanaMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case CortanaMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping)).Length;
                case CortanaMethods.Alpha_Test:
                    return Enum.GetValues(typeof(Alpha_Test)).Length;
                case CortanaMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model)).Length;
                case CortanaMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping)).Length;
                case CortanaMethods.Warp:
                    return Enum.GetValues(typeof(Warp)).Length;
                case CortanaMethods.Lighting:
                    return Enum.GetValues(typeof(Lighting)).Length;
                case CortanaMethods.Scanlines:
                    return Enum.GetValues(typeof(Scanlines)).Length;
                case CortanaMethods.Transparency:
                    return Enum.GetValues(typeof(Transparency)).Length;
            }

            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Active_Camo:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Static_Sh:
                case ShaderStage.Albedo:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsMethodSharedInEntryPoint(ShaderStage entryPoint, int method_index)
        {
            switch (method_index)
            {
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
                case ShaderStage.Static_Sh:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Active_Camo:
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
                        result.AddFloat4ColorParameter("detail_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("depth_darken", default, default, default, default, default, 1f);
                        result.AddFloatParameter("layer_contrast", default, default, default, default, default, 4f);
                        result.AddFloatParameter("layer_depth", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("texcoord_aspect_ratio", default, default, default, default, default, 1f);
                        result.AddIntegerParameter("layer_count", default, default, default, default, default, 2);
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloatParameter("screen_constants", RenderMethodExtern.screen_constants);
                        rmopName = @"shaders\shader_options\cortana_albedo";
                        break;
                }
            }

            if (methodName == "bump_mapping")
            {
                optionName = ((Bump_Mapping)option).ToString();

                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Standard:
                        result.AddSamplerParameter("bump_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\bump_default";
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
                }
            }

            if (methodName == "material_model")
            {
                optionName = ((Material_Model)option).ToString();

                switch ((Material_Model)option)
                {
                    case Material_Model.Cook_Torrance:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular", default, default, default, default, default, default);
                        result.AddBooleanParameter("use_material_texture", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color", default, default, default, default, default, new ShaderColor(1, 128, 128, 128));
                        result.AddFloat3ColorParameter("specular_color_by_angle", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_anti_shadow_control", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_roughness", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("approximate_specular_type", default, default, default, default, default, default);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness", default, default, default, default, default, 5f);
                        result.AddFloatParameter("material_texture_black_roughness", default, default, default, default, default, 1f);
                        result.AddFloatParameter("material_texture_black_specular_multiplier", default, default, default, default, default, 1f);
                        result.AddFloatParameter("roughness", default, default, default, default, default, 0.04f);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236, default, default, default, default, default);
                        result.AddSamplerParameter("material_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_cook_torrance_option";
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
                }
            }

            if (methodName == "warp")
            {
                optionName = ((Warp)option).ToString();

                switch ((Warp)option)
                {
                    case Warp.Default:
                        result.AddFloatParameter("warp_amount", default, default, default, default, default, 100f);
                        rmopName = @"shaders\shader_options\warp_cortana_default";
                        break;
                }
            }

            if (methodName == "lighting")
            {
                optionName = ((Lighting)option).ToString();

                switch ((Lighting)option)
                {
                    case Lighting.Default:
                        break;
                }
            }

            if (methodName == "scanlines")
            {
                optionName = ((Scanlines)option).ToString();

                switch ((Scanlines)option)
                {
                    case Scanlines.Default:
                        result.AddFloatParameter("scanline_amount_opaque", default, default, default, default, default, default);
                        result.AddFloatParameter("scanline_amount_transparent", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("scanline_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\shader_options\cortana_screenspace";
                        break;
                }
            }

            if (methodName == "transparency")
            {
                optionName = ((Transparency)option).ToString();

                switch ((Transparency)option)
                {
                    case Transparency.Default:
                        result.AddFloatParameter("fade_gradient_scale", default, default, default, default, default, 1f);
                        result.AddFloatParameter("fade_offset", default, default, default, default, default, 10f);
                        result.AddFloatParameter("noise_amount", default, default, default, default, default, default);
                        result.AddFloatParameter("warp_fade_offset", default, default, default, default, default, default);
                        result.AddSamplerParameter("fade_gradient_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerParameter("fade_noise_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\shader_options\cortana_transparency";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(CortanaMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((CortanaMethods)methodIndex)
            {
                case CortanaMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case CortanaMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping));
                case CortanaMethods.Alpha_Test:
                    return Enum.GetValues(typeof(Alpha_Test));
                case CortanaMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model));
                case CortanaMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping));
                case CortanaMethods.Warp:
                    return Enum.GetValues(typeof(Warp));
                case CortanaMethods.Lighting:
                    return Enum.GetValues(typeof(Lighting));
                case CortanaMethods.Scanlines:
                    return Enum.GetValues(typeof(Scanlines));
                case CortanaMethods.Transparency:
                    return Enum.GetValues(typeof(Transparency));
            }

            return null;
        }

        public void GetCategoryFunctions(string methodName, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "albedo")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "bump_mapping")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "alpha_test")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_alpha_test_ps";
            }

            if (methodName == "material_model")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "environment_mapping")
            {
                vertexFunction = "invalid";
                pixelFunction = "envmap_type";
            }

            if (methodName == "warp")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "lighting")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "scanlines")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "transparency")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
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
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "bump_mapping")
            {
                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Standard:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "alpha_test")
            {
                switch ((Alpha_Test)option)
                {
                    case Alpha_Test.None:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_alpha_test_off_ps";
                        break;
                    case Alpha_Test.Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_alpha_test_on_ps";
                        break;
                }
            }

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Cook_Torrance:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
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
                }
            }

            if (methodName == "warp")
            {
                switch ((Warp)option)
                {
                    case Warp.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "lighting")
            {
                switch ((Lighting)option)
                {
                    case Lighting.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "scanlines")
            {
                switch ((Scanlines)option)
                {
                    case Scanlines.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "transparency")
            {
                switch ((Transparency)option)
                {
                    case Transparency.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
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
                }
            }

            if (methodName == "bump_mapping")
            {
                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Standard:
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
                }
            }

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Cook_Torrance:
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
                }
            }

            if (methodName == "warp")
            {
                switch ((Warp)option)
                {
                    case Warp.Default:
                        break;
                }
            }

            if (methodName == "lighting")
            {
                switch ((Lighting)option)
                {
                    case Lighting.Default:
                        break;
                }
            }

            if (methodName == "scanlines")
            {
                switch ((Scanlines)option)
                {
                    case Scanlines.Default:
                        break;
                }
            }

            if (methodName == "transparency")
            {
                switch ((Transparency)option)
                {
                    case Transparency.Default:
                        break;
                }
            }
            return result;
        }
    }
}
