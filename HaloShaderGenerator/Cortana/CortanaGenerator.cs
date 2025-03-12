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
            result.AddSamplerParameter("albedo_texture", RenderMethodExtern.texture_global_target_texaccum);
            result.AddSamplerParameter("normal_texture", RenderMethodExtern.texture_global_target_normal);
            result.AddSamplerParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture);
            result.AddSamplerParameter("shadow_depth_map_1", RenderMethodExtern.texture_global_target_shadow_buffer1);
            result.AddSamplerParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0);
            result.AddFloat3ColorParameter("debug_tint", RenderMethodExtern.debug_tint);
            result.AddSamplerParameter("active_camo_distortion_texture", RenderMethodExtern.active_camo_distortion_texture);
            result.AddSamplerParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture);
            result.AddSamplerParameter("dominant_light_intensity_map", RenderMethodExtern.texture_dominant_light_intensity_map);
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
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddFloat4ColorParameter("albedo_color");
                        result.AddFloat4ColorParameter("detail_color");
                        result.AddFloatParameter("layer_depth");
                        result.AddFloatParameter("layer_contrast");
                        result.AddIntegerParameter("layer_count");
                        result.AddFloatParameter("texcoord_aspect_ratio");
                        result.AddFloatParameter("depth_darken");
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
                        result.AddSamplerParameter("bump_map");
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
                        result.AddSamplerParameter("alpha_test_map");
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
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint");
                        result.AddFloat3ColorParameter("fresnel_color");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
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
                        result.AddSamplerParameter("environment_map", default, default, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddFloatParameter("env_roughness_scale");
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.Dynamic:
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddSamplerParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0, default, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddSamplerParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1, default, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddFloatParameter("env_roughness_scale");
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
                        result.AddFloatParameter("warp_amount");
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
                        result.AddSamplerParameter("scanline_map");
                        result.AddFloatParameter("scanline_amount_opaque");
                        result.AddFloatParameter("scanline_amount_transparent");
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
                        result.AddSamplerParameter("fade_gradient_map");
                        result.AddFloatParameter("fade_gradient_scale");
                        result.AddFloatParameter("noise_amount");
                        result.AddSamplerParameter("fade_noise_map");
                        result.AddFloatParameter("fade_offset");
                        result.AddFloatParameter("warp_fade_offset");
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
