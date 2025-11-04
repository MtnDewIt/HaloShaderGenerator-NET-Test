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
                VertexType.LightVolume
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
                    case Albedo.Circular:
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
                    case Depth_Fade.Biased:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                }
            }
        }
    }
}
