using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Beam
{
    public class BeamGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(BeamMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((BeamMethods)methodIndex)
            {
                case BeamMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case BeamMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
                case BeamMethods.Black_Point:
                    return Enum.GetValues(typeof(Black_Point)).Length;
                case BeamMethods.Fog:
                    return Enum.GetValues(typeof(Fog)).Length;
                case BeamMethods.Depth_Fade:
                    return Enum.GetValues(typeof(Depth_Fade)).Length;
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
            return true;
        }

        public ShaderParameters GetGlobalParameters(out string rmopName)
        {
            var result = new ShaderParameters();

            result.AddSamplerExternFilterAddressParameter("depth_buffer", RenderMethodExtern.texture_global_target_z, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp);
            rmopName = @"shaders\beam_options\global_beam_options";

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
                    case Albedo.Diffuse_Only:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\beam_options\albedo_diffuse_only";
                        break;
                    case Albedo.Palettized:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("palette", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\beam_options\albedo_palettized";
                        break;
                    case Albedo.Palettized_Plus_Alpha:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("palette", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("alpha_map", @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        rmopName = @"shaders\beam_options\albedo_palettized_plus_alpha";
                        break;
                    case Albedo.Palettized_Plasma:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("base_map2", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerFilterAddressParameter("palette", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("alpha_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        result.AddFloatParameter("alpha_modulation_factor", 0.1f);
                        rmopName = @"shaders\particle_options\albedo_palettized_plasma";
                        break;
                    case Albedo.Palettized_2d_Plasma:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("base_map2", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerFilterAddressParameter("palette", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("alpha_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        result.AddFloatParameter("alpha_modulation_factor", 0.1f);
                        rmopName = @"shaders\particle_options\albedo_palettized_plasma";
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
                    case Blend_Mode.Maximum:
                        break;
                    case Blend_Mode.Multiply_Add:
                        break;
                    case Blend_Mode.Add_Src_Times_Dstalpha:
                        break;
                    case Blend_Mode.Add_Src_Times_Srcalpha:
                        break;
                    case Blend_Mode.Inv_Alpha_Blend:
                        break;
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        break;
                }
            }

            if (methodName == "black_point")
            {
                optionName = ((Black_Point)option).ToString();

                switch ((Black_Point)option)
                {
                    case Black_Point.Off:
                        break;
                    case Black_Point.On:
                        break;
                }
            }

            if (methodName == "fog")
            {
                optionName = ((Fog)option).ToString();

                switch ((Fog)option)
                {
                    case Fog.Off:
                        break;
                    case Fog.On:
                        break;
                }
            }

            if (methodName == "depth_fade")
            {
                optionName = ((Depth_Fade)option).ToString();

                switch ((Depth_Fade)option)
                {
                    case Depth_Fade.Off:
                        break;
                    case Depth_Fade.On:
                        result.AddFloatParameter("depth_fade_range", 0.1f);
                        rmopName = @"shaders\particle_options\depth_fade_on";
                        break;
                    case Depth_Fade.Palette_Shift:
                        result.AddFloatParameter("depth_fade_range", 0.1f);
                        result.AddFloatParameter("palette_shift_amount", 0.5f);
                        rmopName = @"shaders\particle_options\depth_fade_palette_shift";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(BeamMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((BeamMethods)methodIndex)
            {
                case BeamMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case BeamMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
                case BeamMethods.Black_Point:
                    return Enum.GetValues(typeof(Black_Point));
                case BeamMethods.Fog:
                    return Enum.GetValues(typeof(Fog));
                case BeamMethods.Depth_Fade:
                    return Enum.GetValues(typeof(Depth_Fade));
            }

            return null;
        }

        public Array GetEntryPointOrder()
        {
            return new ShaderStage[]
            {
                ShaderStage.Default
            };
        }

        public Array GetVertexTypeOrder()
        {
            return new VertexType[]
            {
                VertexType.Beam
            };
        }

        public void GetCategoryFunctions(string methodName, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "albedo")
            {
                vertexFunction = "";
                pixelFunction = "";
            }

            if (methodName == "blend_mode")
            {
                vertexFunction = "";
                pixelFunction = "";
            }

            if (methodName == "black_point")
            {
                vertexFunction = "";
                pixelFunction = "";
            }

            if (methodName == "fog")
            {
                vertexFunction = "";
                pixelFunction = "";
            }

            if (methodName == "depth_fade")
            {
                vertexFunction = "";
                pixelFunction = "";
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
                    case Albedo.Diffuse_Only:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Albedo.Palettized:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Albedo.Palettized_Plus_Alpha:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Albedo.Palettized_Plasma:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Albedo.Palettized_2d_Plasma:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                }
            }

            if (methodName == "blend_mode")
            {
                switch ((Blend_Mode)option)
                {
                    case Blend_Mode.Opaque:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Blend_Mode.Additive:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Blend_Mode.Multiply:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Blend_Mode.Alpha_Blend:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Blend_Mode.Double_Multiply:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Blend_Mode.Maximum:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Blend_Mode.Multiply_Add:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Blend_Mode.Add_Src_Times_Dstalpha:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Blend_Mode.Add_Src_Times_Srcalpha:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Blend_Mode.Inv_Alpha_Blend:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                }
            }

            if (methodName == "black_point")
            {
                switch ((Black_Point)option)
                {
                    case Black_Point.Off:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Black_Point.On:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                }
            }

            if (methodName == "fog")
            {
                switch ((Fog)option)
                {
                    case Fog.Off:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Fog.On:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                }
            }

            if (methodName == "depth_fade")
            {
                switch ((Depth_Fade)option)
                {
                    case Depth_Fade.Off:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Depth_Fade.On:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Depth_Fade.Palette_Shift:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                }
            }
        }
    }
}
