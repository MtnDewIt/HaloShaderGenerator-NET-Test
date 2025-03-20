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
                case ShaderStage.Water_Tessellation:
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Per_Vertex:
                //case ShaderStage.Lightmap_Debug_Mode:
                //case ShaderStage.Single_Pass_Per_Vertex:
                //case ShaderStage.Single_Pass_Per_Pixel:
                //case ShaderStage.Static_Default:
                //case ShaderStage.Albedo:
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
                default:
                    return false;
            }
        }

        public bool IsVertexFormatSupported(VertexType vertexType)
        {
            switch (vertexType)
            {
                case VertexType.Water:
                //case VertexType.World:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsAutoMacro()
        {
            return true;
        }

        public ShaderParameters GetGlobalParameters(out string rmopName)
        {
            var result = new ShaderParameters();

            result.AddFloat3ColorExternParameter("water_memory_export_addr", RenderMethodExtern.water_memory_export_address);
            result.AddSamplerExternFilterParameter("depth_buffer", RenderMethodExtern.texture_global_target_z, ShaderOptionParameter.ShaderFilterMode.Bilinear);
            result.AddSamplerFilterAddressParameter("g_direction_lut", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\direction_lut_1002");
            result.AddSamplerExternFilterParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture, ShaderOptionParameter.ShaderFilterMode.Bilinear);
            result.AddSamplerExternParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture);
            result.AddSamplerExternParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            rmopName = @"shaders\water_options\water_global";

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
                        result.AddFloatParameter("choppiness_backward", 1.0f);
                        result.AddFloatParameter("choppiness_forward", 1.0f);
                        result.AddFloatParameter("choppiness_side", 0.4f);
                        result.AddFloatParameter("detail_slope_scale_x", 10.0f);
                        result.AddFloatParameter("detail_slope_scale_y", 5.0f);
                        result.AddFloatParameter("detail_slope_scale_z", 2.0f);
                        result.AddFloatParameter("detail_slope_steepness", 0.5f);
                        result.AddFloatParameter("displacement_range_x");
                        result.AddFloatParameter("displacement_range_y");
                        result.AddFloatParameter("displacement_range_z");
                        result.AddFloatParameter("slope_range_x");
                        result.AddFloatParameter("slope_range_y");
                        result.AddFloatParameter("slope_scaler", 1.0f);
                        result.AddFloatParameter("time_warp_aux", 1.0f);
                        result.AddFloatParameter("time_warp", 1.0f);
                        result.AddFloatParameter("wave_height_aux");
                        result.AddFloatParameter("wave_height", 1.0f);
                        result.AddFloatParameter("wave_tessellation_level", 0.5f);
                        result.AddFloatParameter("wave_visual_damping_distance", 4.0f);
                        //result.AddSamplerParameter("wave_displacement_array_reach", @"rasterizer\water\wave_test7\wave_test7_displ_water"); // reach specific (could add as an extra sampler in the HLSL)
                        result.AddSamplerParameter("wave_displacement_array", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        //result.AddSamplerParameter("wave_slope_array_reach", @"rasterizer\water\wave_test7\wave_test7_slope_water"); // reach specific (could add as an extra sampler in the HLSL)
                        result.AddSamplerParameter("wave_slope_array", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\water_options\waveshape_default";
                        break;
                    case Waveshape.None:
                        break;
                    case Waveshape.Bump:
                        result.AddFloatParameter("slope_scaler", 1.0f);
                        result.AddFloatParameter("wave_visual_damping_distance", 4.0f);
                        result.AddSamplerWithScaleParameter("bump_detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
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
                        result.AddFloat3ColorParameter("water_color_pure", new ShaderColor(0, 1, 16, 12));
                        rmopName = @"shaders\water_options\watercolor_pure";
                        break;
                    case Watercolor.Texture:
                        result.AddFloatParameter("watercolor_coefficient", 1.0f);
                        result.AddSamplerParameter("watercolor_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
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
                        result.AddFloatParameter("normal_variation_tweak", 1.0f);
                        result.AddFloatParameter("reflection_coefficient", 0.4f);
                        result.AddFloatParameter("shadow_intensity_mark", 0.5f);
                        result.AddFloatParameter("sunspot_cut", 0.2f);
                        result.AddSamplerAddressParameter("environment_map", ShaderOptionParameter.ShaderAddressMode.Clamp);
                        rmopName = @"shaders\water_options\reflection_static";
                        break;
                    case Reflection.Dynamic:
                        result.AddFloatParameter("normal_variation_tweak", 1.0f);
                        result.AddFloatParameter("reflection_coefficient", 0.4f);
                        result.AddFloatParameter("shadow_intensity_mark", 0.5f);
                        result.AddFloatParameter("sunspot_cut", 0.2f);
                        rmopName = @"shaders\water_options\reflection_dynamic";
                        break;
                    case Reflection.Static_Ssr:
                        result.AddFloatParameter("reflection_coefficient", 0.4f);
                        result.AddFloatParameter("shadow_intensity_mark", 0.5f);
                        result.AddFloatParameter("ssr_smooth_factor", 5.0f);
                        result.AddFloatParameter("ssr_transparency", 1.0f);
                        result.AddFloatParameter("sunspot_cut", 0.2f);
                        result.AddSamplerAddressParameter("environment_map", ShaderOptionParameter.ShaderAddressMode.Clamp);
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
                        result.AddFloatParameter("minimal_wave_disturbance", 0.2f);
                        result.AddFloatParameter("refraction_depth_dominant_ratio");
                        result.AddFloatParameter("refraction_extinct_distance", 30f);
                        result.AddFloatParameter("refraction_texcoord_shift", 0.03f);
                        result.AddFloatParameter("water_murkiness", 0.3f);
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
                        result.AddFloatParameter("bankalpha_infuence_depth", 0.3f);
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
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloat3ColorParameter("water_diffuse", new ShaderColor(0, 61, 61, 61));
                        result.AddFloatParameter("fresnel_coefficient", 0.09769f);
                        result.AddFloatParameter("fresnel_dark_spot", 1.0f);
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
                        result.AddSamplerParameter("global_shape_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\water_options\globalshape_none";
                        break;
                    case Global_Shape.Paint:
                        result.AddSamplerParameter("global_shape_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\water_options\globalshape_paint";
                        break;
                    case Global_Shape.Depth:
                        result.AddFloatParameter("globalshape_infuence_depth", 1.0f);
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
                        result.AddSamplerParameter("foam_texture_detail", @"levels\shared\bitmaps\nature\water\wave_foam");
                        result.AddSamplerParameter("foam_texture", @"levels\shared\bitmaps\nature\water\wave_foam");
                        rmopName = @"shaders\water_options\foam_none";
                        break;
                    case Foam.Auto:
                        result.AddFloatParameter("foam_coefficient", 1.0f);
                        result.AddFloatParameter("foam_cut", 0.5f);
                        result.AddFloatParameter("foam_height", 0.01f);
                        result.AddFloatParameter("foam_pow", 2.0f);
                        result.AddFloatParameter("foam_start_side");
                        result.AddSamplerParameter("foam_texture_detail", @"levels\shared\bitmaps\nature\water\wave_foam");
                        result.AddSamplerParameter("foam_texture", @"levels\shared\bitmaps\nature\water\wave_foam");
                        rmopName = @"shaders\water_options\foam_auto";
                        break;
                    case Foam.Paint:
                        result.AddFloatParameter("foam_coefficient", 1.0f);
                        result.AddSamplerParameter("foam_texture_detail", @"levels\shared\bitmaps\nature\water\wave_foam");
                        result.AddSamplerParameter("foam_texture", @"levels\shared\bitmaps\nature\water\wave_foam");
                        result.AddSamplerParameter("global_shape_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\water_options\foam_paint";
                        break;
                    case Foam.Both:
                        result.AddFloatParameter("foam_coefficient", 1.0f);
                        result.AddFloatParameter("foam_cut", 0.5f);
                        result.AddFloatParameter("foam_height", 0.01f);
                        result.AddFloatParameter("foam_pow", 2.0f);
                        result.AddFloatParameter("foam_start_side");
                        result.AddSamplerParameter("foam_texture_detail", @"levels\shared\bitmaps\nature\water\wave_foam");
                        result.AddSamplerParameter("foam_texture", @"levels\shared\bitmaps\nature\water\wave_foam");
                        result.AddSamplerParameter("global_shape_texture", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\water_options\foam_both";
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
                        result.AddFloatParameter("detail_slope_scale_x", 10.0f);
                        result.AddFloatParameter("detail_slope_scale_y", 5.0f);
                        result.AddFloatParameter("detail_slope_scale_z", 2.0f);
                        result.AddFloatParameter("detail_slope_steepness", 0.5f);
                        result.AddFloatParameter("foam_coefficient");
                        result.AddFloatParameter("foam_cut");
                        result.AddFloatParameter("fresnel_dark_spot");
                        result.AddFloatParameter("normal_variation_tweak");
                        result.AddFloatParameter("slope_scaler");
                        result.AddSamplerExternAddressParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        rmopName = @"shaders\water_options\reach_compatibility_enabled";
                        break;
                    case Reach_Compatibility.Enabled_Detail_Repeat:
                        result.AddFloatParameter("detail_slope_scale_x", 10.0f);
                        result.AddFloatParameter("detail_slope_scale_y", 5.0f);
                        result.AddFloatParameter("detail_slope_scale_z", 2.0f);
                        result.AddFloatParameter("detail_slope_steepness", 0.5f);
                        result.AddFloatParameter("foam_coefficient");
                        result.AddFloatParameter("foam_cut");
                        result.AddFloatParameter("fresnel_dark_spot");
                        result.AddFloatParameter("normal_variation_tweak");
                        result.AddFloatParameter("slope_scaler");
                        result.AddSamplerExternAddressParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        rmopName = @"shaders\water_options\reach_compatibility_enabled";
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
                        result.AddFloatParameter("detail_slope_scale_x", 10.0f);
                        result.AddFloatParameter("detail_slope_scale_y", 5.0f);
                        result.AddFloatParameter("detail_slope_scale_z", 2.0f);
                        result.AddFloatParameter("detail_slope_steepness", 0.5f);
                        rmopName = @"shaders\water_options\detail_repeat";
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

            if (methodName == "reach_compatibility")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "detail")
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
        }
    }
}
