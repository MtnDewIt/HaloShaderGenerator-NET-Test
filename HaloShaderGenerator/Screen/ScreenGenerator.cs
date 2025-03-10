using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Screen
{
    public class ScreenGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(ScreenMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((ScreenMethods)methodIndex)
            {
                case ScreenMethods.Warp:
                    return Enum.GetValues(typeof(Warp)).Length;
                case ScreenMethods.Base:
                    return Enum.GetValues(typeof(Base)).Length;
                case ScreenMethods.Overlay_A:
                    return Enum.GetValues(typeof(Overlay_A)).Length;
                case ScreenMethods.Overlay_B:
                    return Enum.GetValues(typeof(Overlay_B)).Length;
                case ScreenMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
            }

            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Default:
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
                case VertexType.Screen:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsVertexShaderShared(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Default:
                case ShaderStage.Albedo:
                    return true;
                default:
                    return false;
            }
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();
            return result;
        }

        public ShaderParameters GetParametersInOption(string methodName, int option, out string rmopName, out string optionName)
        {
            ShaderParameters result = new ShaderParameters();
            rmopName = null;
            optionName = null;

            if (methodName == "warp")
            {
                optionName = ((Warp)option).ToString();

                switch ((Warp)option)
                {
                    case Warp.None:
                        break;
                    case Warp.Pixel_Space:
                        result.AddSamplerWithoutXFormParameter("warp_map");
                        result.AddFloatParameter("warp_amount");
                        rmopName = @"shaders\screen_options\warp_simple";
                        break;
                    case Warp.Screen_Space:
                        result.AddSamplerParameter("warp_map");
                        result.AddFloatParameter("warp_amount");
                        rmopName = @"shaders\screen_options\warp_simple";
                        break;
                }
            }

            if (methodName == "base")
            {
                optionName = ((Base)option).ToString();

                switch ((Base)option)
                {
                    case Base.Single_Screen_Space:
                        result.AddSamplerParameter("base_map");
                        rmopName = @"shaders\screen_options\base_single";
                        break;
                    case Base.Single_Pixel_Space:
                        result.AddSamplerParameter("base_map");
                        rmopName = @"shaders\screen_options\base_single";
                        break;
                    case Base.Normal_Map_Edge_Shade:
                        result.AddSamplerWithoutXFormParameter("normal_map");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddFloatParameter("palette_v");
                        rmopName = @"shaders\screen_options\base_normal_map_edge_shade";
                        break;
                    case Base.Single_Target_Space:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        rmopName = @"shaders\screen_options\base_single";
                        break;
                    case Base.Normal_Map_Edge_Stencil:
                        result.AddSamplerWithoutXFormParameter("normal_map");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddFloatParameter("palette_v");
                        result.AddSamplerWithoutXFormParameter("stencil_map");
                        rmopName = @"shaders\screen_options\base_normal_map_edge_stencil";
                        break;
                }
            }

            if (methodName == "overlay_a")
            {
                optionName = ((Overlay_A)option).ToString();

                switch ((Overlay_A)option)
                {
                    case Overlay_A.None:
                        break;
                    case Overlay_A.Tint_Add_Color:
                        result.AddFloat4ColorParameter("tint_color");
                        result.AddFloat4ColorParameter("add_color");
                        rmopName = @"shaders\screen_options\overlay_tint_add_color";
                        break;
                    case Overlay_A.Detail_Screen_Space:
                        result.AddSamplerParameter("detail_map_a");
                        result.AddFloatParameter("detail_fade_a");
                        result.AddFloatParameter("detail_multiplier_a");
                        rmopName = @"shaders\screen_options\detail_a";
                        break;
                    case Overlay_A.Detail_Pixel_Space:
                        result.AddSamplerWithoutXFormParameter("detail_map_a");
                        result.AddFloatParameter("detail_fade_a");
                        result.AddFloatParameter("detail_multiplier_a");
                        rmopName = @"shaders\screen_options\detail_a";
                        break;
                    case Overlay_A.Detail_Masked_Screen_Space:
                        result.AddSamplerParameter("detail_map_a");
                        result.AddSamplerParameter("detail_mask_a");
                        result.AddFloatParameter("detail_fade_a");
                        result.AddFloatParameter("detail_multiplier_a");
                        rmopName = @"shaders\screen_options\detail_mask_a";
                        break;
                    case Overlay_A.Palette_Lookup:
                        result.AddSamplerWithoutXFormParameter("detail_map_a");
                        result.AddFloatParameter("detail_fade_a");
                        result.AddFloat3ColorParameter("intensity_color_u");
                        result.AddFloat3ColorParameter("intensity_color_v");
                        rmopName = @"shaders\screen_options\palette_lookup_a";
                        break;
                }
            }

            if (methodName == "overlay_b")
            {
                optionName = ((Overlay_B)option).ToString();

                switch ((Overlay_B)option)
                {
                    case Overlay_B.None:
                        break;
                    case Overlay_B.Tint_Add_Color:
                        result.AddFloat4ColorParameter("tint_color");
                        result.AddFloat4ColorParameter("add_color");
                        rmopName = @"shaders\screen_options\overlay_tint_add_color";
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
                        result.AddFloatParameter("fade");
                        rmopName = @"shaders\screen_options\blend";
                        break;
                    case Blend_Mode.Multiply:
                        result.AddFloatParameter("fade");
                        rmopName = @"shaders\screen_options\blend";
                        break;
                    case Blend_Mode.Alpha_Blend:
                        result.AddFloatParameter("fade");
                        rmopName = @"shaders\screen_options\blend";
                        break;
                    case Blend_Mode.Double_Multiply:
                        result.AddFloatParameter("fade");
                        rmopName = @"shaders\screen_options\blend";
                        break;
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        result.AddFloatParameter("fade");
                        rmopName = @"shaders\screen_options\blend";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(ScreenMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((ScreenMethods)methodIndex)
            {
                case ScreenMethods.Warp:
                    return Enum.GetValues(typeof(Warp));
                case ScreenMethods.Base:
                    return Enum.GetValues(typeof(Base));
                case ScreenMethods.Overlay_A:
                    return Enum.GetValues(typeof(Overlay_A));
                case ScreenMethods.Overlay_B:
                    return Enum.GetValues(typeof(Overlay_B));
                case ScreenMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
            }

            return null;
        }

        public void GetCategoryFunctions(string methodName, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "warp")
            {
                vertexFunction = "invalid";
                pixelFunction = "warp_type";
            }

            if (methodName == "base")
            {
                vertexFunction = "invalid";
                pixelFunction = "base_type";
            }

            if (methodName == "overlay_a")
            {
                vertexFunction = "invalid";
                pixelFunction = "overlay_a_type";
            }

            if (methodName == "overlay_b")
            {
                vertexFunction = "invalid";
                pixelFunction = "overlay_b_type";
            }

            if (methodName == "blend_mode")
            {
                vertexFunction = "invalid";
                pixelFunction = "blend_type";
            }
        }

        public void GetOptionFunctions(string methodName, int option, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "warp")
            {
                switch ((Warp)option)
                {
                    case Warp.None:
                        vertexFunction = "invalid";
                        pixelFunction = "none";
                        break;
                    case Warp.Pixel_Space:
                        vertexFunction = "invalid";
                        pixelFunction = "pixel_space";
                        break;
                    case Warp.Screen_Space:
                        vertexFunction = "invalid";
                        pixelFunction = "screen_space";
                        break;
                }
            }

            if (methodName == "base")
            {
                switch ((Base)option)
                {
                    case Base.Single_Screen_Space:
                        vertexFunction = "invalid";
                        pixelFunction = "single_screen_space";
                        break;
                    case Base.Single_Pixel_Space:
                        vertexFunction = "invalid";
                        pixelFunction = "single_pixel_space";
                        break;
                    case Base.Normal_Map_Edge_Shade:
                        vertexFunction = "invalid";
                        pixelFunction = "normal_map_edge_shade";
                        break;
                    case Base.Single_Target_Space:
                        vertexFunction = "invalid";
                        pixelFunction = "single_target_space";
                        break;
                    case Base.Normal_Map_Edge_Stencil:
                        vertexFunction = "invalid";
                        pixelFunction = "normal_map_edge_stencil";
                        break;
                }
            }

            if (methodName == "overlay_a")
            {
                switch ((Overlay_A)option)
                {
                    case Overlay_A.None:
                        vertexFunction = "invalid";
                        pixelFunction = "none";
                        break;
                    case Overlay_A.Tint_Add_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "tint_add_color";
                        break;
                    case Overlay_A.Detail_Screen_Space:
                        vertexFunction = "invalid";
                        pixelFunction = "detail_screen_space";
                        break;
                    case Overlay_A.Detail_Pixel_Space:
                        vertexFunction = "invalid";
                        pixelFunction = "detail_pixel_space";
                        break;
                    case Overlay_A.Detail_Masked_Screen_Space:
                        vertexFunction = "invalid";
                        pixelFunction = "detail_masked_screen_space";
                        break;
                    case Overlay_A.Palette_Lookup:
                        vertexFunction = "invalid";
                        pixelFunction = "palette_lookup";
                        break;
                }
            }

            if (methodName == "overlay_b")
            {
                switch ((Overlay_B)option)
                {
                    case Overlay_B.None:
                        vertexFunction = "invalid";
                        pixelFunction = "none";
                        break;
                    case Overlay_B.Tint_Add_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "tint_add_color";
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
        }

        public ShaderParameters GetParameterArguments(string methodName, int option)
        {
            ShaderParameters result = new ShaderParameters();
            if (methodName == "warp")
            {
                switch ((Warp)option)
                {
                    case Warp.None:
                        break;
                    case Warp.Pixel_Space:
                        break;
                    case Warp.Screen_Space:
                        break;
                }
            }

            if (methodName == "base")
            {
                switch ((Base)option)
                {
                    case Base.Single_Screen_Space:
                        break;
                    case Base.Single_Pixel_Space:
                        break;
                    case Base.Normal_Map_Edge_Shade:
                        break;
                    case Base.Single_Target_Space:
                        break;
                    case Base.Normal_Map_Edge_Stencil:
                        break;
                }
            }

            if (methodName == "overlay_a")
            {
                switch ((Overlay_A)option)
                {
                    case Overlay_A.None:
                        break;
                    case Overlay_A.Tint_Add_Color:
                        break;
                    case Overlay_A.Detail_Screen_Space:
                        break;
                    case Overlay_A.Detail_Pixel_Space:
                        break;
                    case Overlay_A.Detail_Masked_Screen_Space:
                        break;
                    case Overlay_A.Palette_Lookup:
                        break;
                }
            }

            if (methodName == "overlay_b")
            {
                switch ((Overlay_B)option)
                {
                    case Overlay_B.None:
                        break;
                    case Overlay_B.Tint_Add_Color:
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
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        break;
                }
            }
            return result;
        }
    }
}
