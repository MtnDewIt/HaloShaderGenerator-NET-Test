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

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                default:
                    return -1;
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
                        result.AddFloat4ColorParameter("detail_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("layer_depth", 0.1f);
                        result.AddFloatParameter("layer_contrast", 4.0f);
                        result.AddIntegerParameter("layer_count", 2);
                        result.AddFloatParameter("texcoord_aspect_ratio", 1.0f);
                        result.AddFloatParameter("depth_darken", 1.0f);
                        result.AddFloatExternParameter("screen_constants", RenderMethodExtern.screen_constants);
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
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
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
                        result.AddSamplerParameter("alpha_test_map", @"shaders\default_bitmaps\bitmaps\default_alpha_test");
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
                }
            }

            if (methodName == "warp")
            {
                optionName = ((Warp)option).ToString();

                switch ((Warp)option)
                {
                    case Warp.Default:
                        result.AddFloatParameter("warp_amount", 100.0f);
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
                        result.AddSamplerWithScaleParameter("scanline_map", 1.0f, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloatParameter("scanline_amount_opaque");
                        result.AddFloatParameter("scanline_amount_transparent", 1.0f);
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
                        result.AddSamplerWithScaleParameter("fade_gradient_map", 1.0f, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloatParameter("fade_gradient_scale", 1.0f);
                        result.AddFloatParameter("noise_amount");
                        result.AddSamplerWithScaleParameter("fade_noise_map", 1.0f, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloatParameter("fade_offset", 10f);
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

        public Array GetEntryPointOrder()
        {
            return new ShaderStage[]
            {
                ShaderStage.Active_Camo,
                ShaderStage.Static_Prt_Ambient,
                ShaderStage.Static_Sh,
                ShaderStage.Albedo
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
    }
}
