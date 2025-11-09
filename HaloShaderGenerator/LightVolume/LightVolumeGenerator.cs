using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.LightVolume
{
    public class LightVolumeGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(LightVolumeMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (LightVolumeMethods)methodIndex switch
            {
                LightVolumeMethods.Albedo => Enum.GetValues(typeof(Albedo)).Length,
                LightVolumeMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)).Length,
                LightVolumeMethods.Fog => Enum.GetValues(typeof(Fog)).Length,
                LightVolumeMethods.Depth_Fade => Enum.GetValues(typeof(Depth_Fade)).Length,
                _ => -1,
            };
        }

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint) => -1;

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint) => false;

        public bool IsPixelShaderShared(ShaderStage entryPoint) => false;

        public bool IsAutoMacro() => true;

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

        public Array GetMethodNames() => Enum.GetValues(typeof(LightVolumeMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (LightVolumeMethods)methodIndex switch
            {
                LightVolumeMethods.Albedo => Enum.GetValues(typeof(Albedo)),
                LightVolumeMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)),
                LightVolumeMethods.Fog => Enum.GetValues(typeof(Fog)),
                LightVolumeMethods.Depth_Fade => Enum.GetValues(typeof(Depth_Fade)),
                _ => null,
            };
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

        public string GetCategoryPixelFunction(int category)
        {
            return (LightVolumeMethods)category switch
            {
                LightVolumeMethods.Albedo => string.Empty,
                LightVolumeMethods.Blend_Mode => string.Empty,
                LightVolumeMethods.Fog => string.Empty,
                LightVolumeMethods.Depth_Fade => string.Empty,
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (LightVolumeMethods)category switch
            {
                LightVolumeMethods.Albedo => string.Empty,
                LightVolumeMethods.Blend_Mode => string.Empty,
                LightVolumeMethods.Fog => string.Empty,
                LightVolumeMethods.Depth_Fade => string.Empty,
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (LightVolumeMethods)category switch
            {
                LightVolumeMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Diffuse_Only => string.Empty,
                    Albedo.Circular => string.Empty,
                    _ => null,
                },
                LightVolumeMethods.Blend_Mode => (Blend_Mode)option switch
                {
                    Blend_Mode.Opaque => string.Empty,
                    Blend_Mode.Additive => string.Empty,
                    Blend_Mode.Multiply => string.Empty,
                    Blend_Mode.Alpha_Blend => string.Empty,
                    Blend_Mode.Double_Multiply => string.Empty,
                    Blend_Mode.Maximum => string.Empty,
                    Blend_Mode.Multiply_Add => string.Empty,
                    Blend_Mode.Add_Src_Times_Dstalpha => string.Empty,
                    Blend_Mode.Add_Src_Times_Srcalpha => string.Empty,
                    Blend_Mode.Inv_Alpha_Blend => string.Empty,
                    Blend_Mode.Pre_Multiplied_Alpha => string.Empty,
                    _ => null,
                },
                LightVolumeMethods.Fog => (Fog)option switch
                {
                    Fog.Off => string.Empty,
                    Fog.On => string.Empty,
                    _ => null,
                },
                LightVolumeMethods.Depth_Fade => (Depth_Fade)option switch
                {
                    Depth_Fade.Off => string.Empty,
                    Depth_Fade.On => string.Empty,
                    Depth_Fade.Biased => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (LightVolumeMethods)category switch
            {
                LightVolumeMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Diffuse_Only => string.Empty,
                    Albedo.Circular => string.Empty,
                    _ => null,
                },
                LightVolumeMethods.Blend_Mode => (Blend_Mode)option switch
                {
                    Blend_Mode.Opaque => string.Empty,
                    Blend_Mode.Additive => string.Empty,
                    Blend_Mode.Multiply => string.Empty,
                    Blend_Mode.Alpha_Blend => string.Empty,
                    Blend_Mode.Double_Multiply => string.Empty,
                    Blend_Mode.Maximum => string.Empty,
                    Blend_Mode.Multiply_Add => string.Empty,
                    Blend_Mode.Add_Src_Times_Dstalpha => string.Empty,
                    Blend_Mode.Add_Src_Times_Srcalpha => string.Empty,
                    Blend_Mode.Inv_Alpha_Blend => string.Empty,
                    Blend_Mode.Pre_Multiplied_Alpha => string.Empty,
                    _ => null,
                },
                LightVolumeMethods.Fog => (Fog)option switch
                {
                    Fog.Off => string.Empty,
                    Fog.On => string.Empty,
                    _ => null,
                },
                LightVolumeMethods.Depth_Fade => (Depth_Fade)option switch
                {
                    Depth_Fade.Off => string.Empty,
                    Depth_Fade.On => string.Empty,
                    Depth_Fade.Biased => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
