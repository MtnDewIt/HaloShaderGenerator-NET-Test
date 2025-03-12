using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Water
{
    public class WaterGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(WaterMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((WaterMethods)methodIndex)
            {
                case WaterMethods.Waveshape:
                    return Enum.GetValues(typeof(Waveshape)).Length;
                case WaterMethods.Watercolor:
                    return Enum.GetValues(typeof(Watercolor)).Length;
                case WaterMethods.Reflection:
                    return Enum.GetValues(typeof(Reflection)).Length;
                case WaterMethods.Refraction:
                    return Enum.GetValues(typeof(Refraction)).Length;
                case WaterMethods.Bankalpha:
                    return Enum.GetValues(typeof(Bankalpha)).Length;
                case WaterMethods.Appearance:
                    return Enum.GetValues(typeof(Appearance)).Length;
                case WaterMethods.Global_Shape:
                    return Enum.GetValues(typeof(Global_Shape)).Length;
                case WaterMethods.Foam:
                    return Enum.GetValues(typeof(Foam)).Length;
                case WaterMethods.Detail:
                    return Enum.GetValues(typeof(Detail)).Length;
                case WaterMethods.Reach_Compatibility:
                    return Enum.GetValues(typeof(Reach_Compatibility)).Length;
            }

            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Water_Tessellation:
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Lightmap_Debug_Mode:
                case ShaderStage.Single_Pass_Per_Vertex:
                case ShaderStage.Single_Pass_Per_Pixel:
                case ShaderStage.Static_Default:
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
                case VertexType.Water:
                case VertexType.World:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsVertexShaderShared(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Static_Sh:
                case ShaderStage.Water_Tessellation:
                case ShaderStage.Lightmap_Debug_Mode:
                    return true;
                default:
                    return false;
            }
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();
            result.AddFloat3ColorParameter("water_memory_export_addr", RenderMethodExtern.water_memory_export_address);
            result.AddSamplerParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture);
            result.AddSamplerParameter("depth_buffer", RenderMethodExtern.texture_global_target_z, ShaderOptionParameter.ShaderFilterMode.Bilinear, default);
            result.AddSamplerParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture, ShaderOptionParameter.ShaderFilterMode.Bilinear, default);
            result.AddSamplerParameter("g_direction_lut", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp);
            return result;
        }

        public ShaderParameters GetParametersInOption(string methodName, int option, out string rmopName, out string optionName)
        {
            ShaderParameters result = new ShaderParameters();
            rmopName = null;
            optionName = null;

            if (methodName == "waveshape")
            {
                optionName = ((Waveshape)option).ToString();

                switch ((Waveshape)option)
                {
                    case Waveshape.Default:
                        result.AddFloatParameter("displacement_range_x");
                        result.AddFloatParameter("displacement_range_y");
                        result.AddFloatParameter("displacement_range_z");
                        result.AddFloatParameter("slope_range_x");
                        result.AddFloatParameter("slope_range_y");
                        result.AddSamplerParameter("wave_displacement_array");
                        result.AddFloatParameter("wave_height");
                        result.AddFloatParameter("time_warp");
                        result.AddSamplerParameter("wave_slope_array");
                        result.AddFloatParameter("wave_height_aux");
                        result.AddFloatParameter("time_warp_aux");
                        result.AddFloatParameter("choppiness_forward");
                        result.AddFloatParameter("choppiness_backward");
                        result.AddFloatParameter("choppiness_side");
                        result.AddFloatParameter("detail_slope_scale_x");
                        result.AddFloatParameter("detail_slope_scale_y");
                        result.AddFloatParameter("detail_slope_scale_z");
                        result.AddFloatParameter("detail_slope_steepness");
                        result.AddFloatParameter("wave_visual_damping_distance");
                        result.AddFloatParameter("slope_scaler");
                        result.AddFloatParameter("wave_tessellation_level");
                        rmopName = @"shaders\water_options\waveshape_default";
                        break;
                    case Waveshape.None:
                        break;
                    case Waveshape.Bump:
                        result.AddSamplerParameter("bump_map");
                        result.AddSamplerParameter("bump_detail_map");
                        result.AddFloatParameter("wave_visual_damping_distance");
                        result.AddFloatParameter("slope_scaler");
                        rmopName = @"shaders\water_options\waveshape_bump";
                        break;
                }
            }

            if (methodName == "watercolor")
            {
                optionName = ((Watercolor)option).ToString();

                switch ((Watercolor)option)
                {
                    case Watercolor.Pure:
                        result.AddFloat3ColorParameter("water_color_pure");
                        rmopName = @"shaders\water_options\watercolor_pure";
                        break;
                    case Watercolor.Texture:
                        result.AddSamplerParameter("watercolor_texture");
                        result.AddFloatParameter("watercolor_coefficient");
                        rmopName = @"shaders\water_options\watercolor_texture";
                        break;
                }
            }

            if (methodName == "reflection")
            {
                optionName = ((Reflection)option).ToString();

                switch ((Reflection)option)
                {
                    case Reflection.None:
                        break;
                    case Reflection.Static:
                        result.AddSamplerParameter("environment_map", default, default, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddFloatParameter("reflection_coefficient");
                        result.AddFloatParameter("sunspot_cut");
                        result.AddFloatParameter("shadow_intensity_mark");
                        result.AddFloatParameter("normal_variation_tweak");
                        rmopName = @"shaders\water_options\reflection_static";
                        break;
                    case Reflection.Dynamic:
                        result.AddFloatParameter("reflection_coefficient");
                        result.AddFloatParameter("sunspot_cut");
                        result.AddFloatParameter("shadow_intensity_mark");
                        result.AddFloatParameter("normal_variation_tweak");
                        rmopName = @"shaders\water_options\reflection_dynamic";
                        break;
                    case Reflection.Static_Ssr:
                        result.AddSamplerParameter("environment_map", default, default, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddFloatParameter("reflection_coefficient");
                        result.AddFloatParameter("sunspot_cut");
                        result.AddFloatParameter("shadow_intensity_mark");
                        result.AddFloatParameter("ssr_transparency");
                        result.AddFloatParameter("ssr_smooth_factor");
                        rmopName = @"shaders\water_options\reflection_static_ssr";
                        break;
                }
            }

            if (methodName == "refraction")
            {
                optionName = ((Refraction)option).ToString();

                switch ((Refraction)option)
                {
                    case Refraction.None:
                        break;
                    case Refraction.Dynamic:
                        result.AddFloatParameter("refraction_texcoord_shift");
                        result.AddFloatParameter("water_murkiness");
                        result.AddFloatParameter("refraction_extinct_distance");
                        result.AddFloatParameter("minimal_wave_disturbance");
                        result.AddFloatParameter("refraction_depth_dominant_ratio");
                        rmopName = @"shaders\water_options\refraction_dynamic";
                        break;
                }
            }

            if (methodName == "bankalpha")
            {
                optionName = ((Bankalpha)option).ToString();

                switch ((Bankalpha)option)
                {
                    case Bankalpha.None:
                        break;
                    case Bankalpha.Depth:
                        result.AddFloatParameter("bankalpha_infuence_depth");
                        rmopName = @"shaders\water_options\bankalpha_depth";
                        break;
                    case Bankalpha.Paint:
                        result.AddSamplerParameter("watercolor_texture");
                        rmopName = @"shaders\water_options\bankalpha_paint";
                        break;
                    case Bankalpha.From_Shape_Texture_Alpha:
                        result.AddSamplerParameter("global_shape_texture");
                        rmopName = @"shaders\water_options\bankalpha_from_shape_texture_alpha";
                        break;
                }
            }

            if (methodName == "appearance")
            {
                optionName = ((Appearance)option).ToString();

                switch ((Appearance)option)
                {
                    case Appearance.Default:
                        result.AddFloatParameter("fresnel_coefficient");
                        result.AddFloat3ColorParameter("water_diffuse");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("fresnel_dark_spot");
                        rmopName = @"shaders\water_options\appearance_default";
                        break;
                }
            }

            if (methodName == "global_shape")
            {
                optionName = ((Global_Shape)option).ToString();

                switch ((Global_Shape)option)
                {
                    case Global_Shape.None:
                        result.AddSamplerParameter("global_shape_texture");
                        rmopName = @"shaders\water_options\globalshape_none";
                        break;
                    case Global_Shape.Paint:
                        result.AddSamplerParameter("global_shape_texture");
                        rmopName = @"shaders\water_options\globalshape_paint";
                        break;
                    case Global_Shape.Depth:
                        result.AddFloatParameter("globalshape_infuence_depth");
                        rmopName = @"shaders\water_options\globalshape_depth";
                        break;
                }
            }

            if (methodName == "foam")
            {
                optionName = ((Foam)option).ToString();

                switch ((Foam)option)
                {
                    case Foam.None:
                        result.AddSamplerParameter("foam_texture");
                        result.AddSamplerParameter("foam_texture_detail");
                        rmopName = @"shaders\water_options\foam_none";
                        break;
                    case Foam.Auto:
                        result.AddSamplerParameter("foam_texture");
                        result.AddSamplerParameter("foam_texture_detail");
                        result.AddFloatParameter("foam_height");
                        result.AddFloatParameter("foam_pow");
                        result.AddFloatParameter("foam_cut");
                        result.AddFloatParameter("foam_start_side");
                        result.AddFloatParameter("foam_coefficient");
                        rmopName = @"shaders\water_options\foam_auto";
                        break;
                    case Foam.Paint:
                        result.AddSamplerParameter("foam_texture");
                        result.AddSamplerParameter("foam_texture_detail");
                        result.AddSamplerParameter("global_shape_texture");
                        result.AddFloatParameter("foam_coefficient");
                        rmopName = @"shaders\water_options\foam_paint";
                        break;
                    case Foam.Both:
                        result.AddSamplerParameter("foam_texture");
                        result.AddSamplerParameter("foam_texture_detail");
                        result.AddSamplerParameter("global_shape_texture");
                        result.AddFloatParameter("foam_height");
                        result.AddFloatParameter("foam_pow");
                        result.AddFloatParameter("foam_cut");
                        result.AddFloatParameter("foam_start_side");
                        result.AddFloatParameter("foam_coefficient");
                        rmopName = @"shaders\water_options\foam_both";
                        break;
                }
            }

            if (methodName == "detail")
            {
                optionName = ((Detail)option).ToString();

                switch ((Detail)option)
                {
                    case Detail.None:
                        break;
                    case Detail.Repeat:
                        result.AddFloatParameter("detail_slope_scale_x");
                        result.AddFloatParameter("detail_slope_scale_y");
                        result.AddFloatParameter("detail_slope_scale_z");
                        result.AddFloatParameter("detail_slope_steepness");
                        rmopName = @"shaders\water_options\detail_repeat";
                        break;
                }
            }

            if (methodName == "reach_compatibility")
            {
                optionName = ((Reach_Compatibility)option).ToString();

                switch ((Reach_Compatibility)option)
                {
                    case Reach_Compatibility.Disabled:
                        break;
                    case Reach_Compatibility.Enabled:
                        result.AddFloatParameter("slope_scaler");
                        result.AddFloatParameter("normal_variation_tweak");
                        result.AddFloatParameter("fresnel_dark_spot");
                        result.AddFloatParameter("foam_coefficient");
                        result.AddFloatParameter("foam_cut");
                        result.AddSamplerParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0);
                        result.AddFloatParameter("detail_slope_scale_x");
                        result.AddFloatParameter("detail_slope_scale_y");
                        result.AddFloatParameter("detail_slope_scale_z");
                        result.AddFloatParameter("detail_slope_steepness");
                        rmopName = @"shaders\water_options\reach_compatibility_enabled";
                        break;
                    case Reach_Compatibility.Enabled_Detail_Repeat:
                        result.AddFloatParameter("slope_scaler");
                        result.AddFloatParameter("normal_variation_tweak");
                        result.AddFloatParameter("fresnel_dark_spot");
                        result.AddFloatParameter("foam_coefficient");
                        result.AddFloatParameter("foam_cut");
                        result.AddSamplerParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0);
                        result.AddFloatParameter("detail_slope_scale_x");
                        result.AddFloatParameter("detail_slope_scale_y");
                        result.AddFloatParameter("detail_slope_scale_z");
                        result.AddFloatParameter("detail_slope_steepness");
                        rmopName = @"shaders\water_options\reach_compatibility_enabled";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(WaterMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((WaterMethods)methodIndex)
            {
                case WaterMethods.Waveshape:
                    return Enum.GetValues(typeof(Waveshape));
                case WaterMethods.Watercolor:
                    return Enum.GetValues(typeof(Watercolor));
                case WaterMethods.Reflection:
                    return Enum.GetValues(typeof(Reflection));
                case WaterMethods.Refraction:
                    return Enum.GetValues(typeof(Refraction));
                case WaterMethods.Bankalpha:
                    return Enum.GetValues(typeof(Bankalpha));
                case WaterMethods.Appearance:
                    return Enum.GetValues(typeof(Appearance));
                case WaterMethods.Global_Shape:
                    return Enum.GetValues(typeof(Global_Shape));
                case WaterMethods.Foam:
                    return Enum.GetValues(typeof(Foam));
                case WaterMethods.Detail:
                    return Enum.GetValues(typeof(Detail));
                case WaterMethods.Reach_Compatibility:
                    return Enum.GetValues(typeof(Reach_Compatibility));
            }

            return null;
        }

        public void GetCategoryFunctions(string methodName, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "waveshape")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "watercolor")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "reflection")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "refraction")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "bankalpha")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "appearance")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "global_shape")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "foam")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "detail")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "reach_compatibility")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }
        }

        public void GetOptionFunctions(string methodName, int option, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "waveshape")
            {
                switch ((Waveshape)option)
                {
                    case Waveshape.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Waveshape.None:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Waveshape.Bump:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "watercolor")
            {
                switch ((Watercolor)option)
                {
                    case Watercolor.Pure:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Watercolor.Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "reflection")
            {
                switch ((Reflection)option)
                {
                    case Reflection.None:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Reflection.Static:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Reflection.Dynamic:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Reflection.Static_Ssr:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "refraction")
            {
                switch ((Refraction)option)
                {
                    case Refraction.None:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Refraction.Dynamic:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "bankalpha")
            {
                switch ((Bankalpha)option)
                {
                    case Bankalpha.None:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Bankalpha.Depth:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Bankalpha.Paint:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Bankalpha.From_Shape_Texture_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "appearance")
            {
                switch ((Appearance)option)
                {
                    case Appearance.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "global_shape")
            {
                switch ((Global_Shape)option)
                {
                    case Global_Shape.None:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Global_Shape.Paint:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Global_Shape.Depth:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "foam")
            {
                switch ((Foam)option)
                {
                    case Foam.None:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Foam.Auto:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Foam.Paint:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Foam.Both:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "detail")
            {
                switch ((Detail)option)
                {
                    case Detail.None:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Detail.Repeat:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "reach_compatibility")
            {
                switch ((Reach_Compatibility)option)
                {
                    case Reach_Compatibility.Disabled:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Reach_Compatibility.Enabled:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Reach_Compatibility.Enabled_Detail_Repeat:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }
        }

        public ShaderParameters GetParameterArguments(string methodName, int option)
        {
            ShaderParameters result = new ShaderParameters();
            if (methodName == "waveshape")
            {
                switch ((Waveshape)option)
                {
                    case Waveshape.Default:
                        break;
                    case Waveshape.None:
                        break;
                    case Waveshape.Bump:
                        break;
                }
            }

            if (methodName == "watercolor")
            {
                switch ((Watercolor)option)
                {
                    case Watercolor.Pure:
                        break;
                    case Watercolor.Texture:
                        break;
                }
            }

            if (methodName == "reflection")
            {
                switch ((Reflection)option)
                {
                    case Reflection.None:
                        break;
                    case Reflection.Static:
                        break;
                    case Reflection.Dynamic:
                        break;
                    case Reflection.Static_Ssr:
                        break;
                }
            }

            if (methodName == "refraction")
            {
                switch ((Refraction)option)
                {
                    case Refraction.None:
                        break;
                    case Refraction.Dynamic:
                        break;
                }
            }

            if (methodName == "bankalpha")
            {
                switch ((Bankalpha)option)
                {
                    case Bankalpha.None:
                        break;
                    case Bankalpha.Depth:
                        break;
                    case Bankalpha.Paint:
                        break;
                    case Bankalpha.From_Shape_Texture_Alpha:
                        break;
                }
            }

            if (methodName == "appearance")
            {
                switch ((Appearance)option)
                {
                    case Appearance.Default:
                        break;
                }
            }

            if (methodName == "global_shape")
            {
                switch ((Global_Shape)option)
                {
                    case Global_Shape.None:
                        break;
                    case Global_Shape.Paint:
                        break;
                    case Global_Shape.Depth:
                        break;
                }
            }

            if (methodName == "foam")
            {
                switch ((Foam)option)
                {
                    case Foam.None:
                        break;
                    case Foam.Auto:
                        break;
                    case Foam.Paint:
                        break;
                    case Foam.Both:
                        break;
                }
            }

            if (methodName == "detail")
            {
                switch ((Detail)option)
                {
                    case Detail.None:
                        break;
                    case Detail.Repeat:
                        break;
                }
            }

            if (methodName == "reach_compatibility")
            {
                switch ((Reach_Compatibility)option)
                {
                    case Reach_Compatibility.Disabled:
                        break;
                    case Reach_Compatibility.Enabled:
                        break;
                    case Reach_Compatibility.Enabled_Detail_Repeat:
                        break;
                }
            }
            return result;
        }
    }
}
