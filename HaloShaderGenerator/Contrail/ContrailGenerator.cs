using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Contrail
{
    public class ContrailGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(ContrailMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (ContrailMethods)methodIndex switch
            {
                ContrailMethods.Albedo => Enum.GetValues(typeof(Albedo)).Length,
                ContrailMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)).Length,
                ContrailMethods.Black_Point => Enum.GetValues(typeof(Black_Point)).Length,
                ContrailMethods.Fog => Enum.GetValues(typeof(Fog)).Length,
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

            rmopName = @"shaders\contrail_options\global_contrail_options";

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
                        rmopName = @"shaders\contrail_options\albedo_diffuse_only";
                        break;
                    case Albedo.Palettized:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("palette", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\contrail_options\albedo_palettized";
                        break;
                    case Albedo.Palettized_Plus_Alpha:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("palette", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("alpha_map", @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        rmopName = @"shaders\contrail_options\albedo_palettized_plus_alpha";
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
            return result;
        }

        public Array GetMethodNames() => Enum.GetValues(typeof(ContrailMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (ContrailMethods)methodIndex switch
            {
                ContrailMethods.Albedo => Enum.GetValues(typeof(Albedo)),
                ContrailMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)),
                ContrailMethods.Black_Point => Enum.GetValues(typeof(Black_Point)),
                ContrailMethods.Fog => Enum.GetValues(typeof(Fog)),
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
                VertexType.Contrail
            };
        }

        public string GetCategoryPixelFunction(int category)
        {
            return (ContrailMethods)category switch
            {
                ContrailMethods.Albedo => string.Empty,
                ContrailMethods.Blend_Mode => string.Empty,
                ContrailMethods.Black_Point => string.Empty,
                ContrailMethods.Fog => string.Empty,
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (ContrailMethods)category switch
            {
                ContrailMethods.Albedo => string.Empty,
                ContrailMethods.Blend_Mode => string.Empty,
                ContrailMethods.Black_Point => string.Empty,
                ContrailMethods.Fog => string.Empty,
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (ContrailMethods)category switch
            {
                ContrailMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Diffuse_Only => string.Empty,
                    Albedo.Palettized => string.Empty,
                    Albedo.Palettized_Plus_Alpha => string.Empty,
                    _ => null,
                },
                ContrailMethods.Blend_Mode => (Blend_Mode)option switch
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
                ContrailMethods.Black_Point => (Black_Point)option switch
                {
                    Black_Point.Off => string.Empty,
                    Black_Point.On => string.Empty,
                    _ => null,
                },
                ContrailMethods.Fog => (Fog)option switch
                {
                    Fog.Off => string.Empty,
                    Fog.On => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (ContrailMethods)category switch
            {
                ContrailMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Diffuse_Only => string.Empty,
                    Albedo.Palettized => string.Empty,
                    Albedo.Palettized_Plus_Alpha => string.Empty,
                    _ => null,
                },
                ContrailMethods.Blend_Mode => (Blend_Mode)option switch
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
                ContrailMethods.Black_Point => (Black_Point)option switch
                {
                    Black_Point.Off => string.Empty,
                    Black_Point.On => string.Empty,
                    _ => null,
                },
                ContrailMethods.Fog => (Fog)option switch
                {
                    Fog.Off => string.Empty,
                    Fog.On => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
