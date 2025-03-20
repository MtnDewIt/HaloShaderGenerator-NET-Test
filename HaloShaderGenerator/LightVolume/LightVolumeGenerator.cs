using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.LightVolume
{
    public class LightVolumeGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(LightVolumeMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((LightVolumeMethods)methodIndex)
            {
                case LightVolumeMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case LightVolumeMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
                case LightVolumeMethods.Fog:
                    return Enum.GetValues(typeof(Fog)).Length;
                case LightVolumeMethods.Depth_Fade:
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

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Default:
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
                case VertexType.LightVolume:
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

            rmopName = @"shaders\light_volume_options\global_light_volume";

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
                        rmopName = @"shaders\light_volume_options\albedo_diffuse_only";
                        break;
                    case Albedo.Circular:
                        result.AddFloatParameter("center_offset", 1.1f);
                        result.AddFloatParameter("falloff", 2f);
                        rmopName = @"shaders\light_volume_options\albedo_circular";
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
                    case Depth_Fade.Biased:
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(LightVolumeMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((LightVolumeMethods)methodIndex)
            {
                case LightVolumeMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case LightVolumeMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
                case LightVolumeMethods.Fog:
                    return Enum.GetValues(typeof(Fog));
                case LightVolumeMethods.Depth_Fade:
                    return Enum.GetValues(typeof(Depth_Fade));
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

            if (methodName == "blend_mode")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "fog")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "depth_fade")
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
                    case Albedo.Diffuse_Only:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Circular:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "blend_mode")
            {
                switch ((Blend_Mode)option)
                {
                    case Blend_Mode.Opaque:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Additive:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Multiply:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Alpha_Blend:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Double_Multiply:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Maximum:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Multiply_Add:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Add_Src_Times_Dstalpha:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Add_Src_Times_Srcalpha:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Inv_Alpha_Blend:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "fog")
            {
                switch ((Fog)option)
                {
                    case Fog.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Fog.On:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "depth_fade")
            {
                switch ((Depth_Fade)option)
                {
                    case Depth_Fade.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Depth_Fade.On:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Depth_Fade.Biased:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }
        }
    }
}
