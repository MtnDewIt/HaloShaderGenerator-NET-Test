using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Beam
{
    public class BeamGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(BeamMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (BeamMethods)methodIndex switch
            {
                BeamMethods.Albedo => Enum.GetValues(typeof(Albedo)).Length,
                BeamMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)).Length,
                BeamMethods.Black_Point => Enum.GetValues(typeof(Black_Point)).Length,
                BeamMethods.Fog => Enum.GetValues(typeof(Fog)).Length,
                BeamMethods.Depth_Fade => Enum.GetValues(typeof(Depth_Fade)).Length,
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

            result.AddSamplerExternFilterAddressParameter("depth_buffer", RenderMethodExtern.texture_global_target_z, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp);
            rmopName = @"shaders\beam_options\global_beam_options";

            return result;
        }

        public ShaderParameters GetParametersInOption(string methodName, int option, out string rmopName, out string optionName)
        {
            ShaderParameters result = new ShaderParameters();
            rmopName = null;
            optionName = null;

            switch (methodName) 
            {
                case "albedo":
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
                    break;
                case "blend_mode":
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
                    break;
                case "black_point":
                    optionName = ((Black_Point)option).ToString();
                    switch ((Black_Point)option)
                    {
                        case Black_Point.Off:
                            break;
                        case Black_Point.On:
                            break;
                    }
                    break;
                case "fog":
                    optionName = ((Fog)option).ToString();
                    switch ((Fog)option)
                    {
                        case Fog.Off:
                            break;
                        case Fog.On:
                            break;
                    }
                    break;
                case "depth_fade":
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
                    break;
            }

            return result;
        }

        public Array GetMethodNames() => Enum.GetValues(typeof(BeamMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (BeamMethods)methodIndex switch
            {
                BeamMethods.Albedo => Enum.GetValues(typeof(Albedo)),
                BeamMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)),
                BeamMethods.Black_Point => Enum.GetValues(typeof(Black_Point)),
                BeamMethods.Fog => Enum.GetValues(typeof(Fog)),
                BeamMethods.Depth_Fade => Enum.GetValues(typeof(Depth_Fade)),
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
                VertexType.Beam
            };
        }

        public string GetCategoryPixelFunction(int category)
        {
            return (BeamMethods)category switch
            {
                BeamMethods.Albedo => string.Empty,
                BeamMethods.Blend_Mode => string.Empty,
                BeamMethods.Black_Point => string.Empty,
                BeamMethods.Fog => string.Empty,
                BeamMethods.Depth_Fade => string.Empty,
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (BeamMethods)category switch
            {
                BeamMethods.Albedo => string.Empty,
                BeamMethods.Blend_Mode => string.Empty,
                BeamMethods.Black_Point => string.Empty,
                BeamMethods.Fog => string.Empty,
                BeamMethods.Depth_Fade => string.Empty,
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (BeamMethods)category switch
            {
                BeamMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Diffuse_Only => string.Empty,
                    Albedo.Palettized => string.Empty,
                    Albedo.Palettized_Plus_Alpha => string.Empty,
                    Albedo.Palettized_Plasma => string.Empty,
                    Albedo.Palettized_2d_Plasma => string.Empty,
                    _ => null,
                },
                BeamMethods.Blend_Mode => (Blend_Mode)option switch
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
                BeamMethods.Black_Point => (Black_Point)option switch
                {
                    Black_Point.Off => string.Empty,
                    Black_Point.On => string.Empty,
                    _ => null,
                },
                BeamMethods.Fog => (Fog)option switch
                {
                    Fog.Off => string.Empty,
                    Fog.On => string.Empty,
                    _ => null,
                },
                BeamMethods.Depth_Fade => (Depth_Fade)option switch
                {
                    Depth_Fade.Off => string.Empty,
                    Depth_Fade.On => string.Empty,
                    Depth_Fade.Palette_Shift => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (BeamMethods)category switch
            {
                BeamMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Diffuse_Only => string.Empty,
                    Albedo.Palettized => string.Empty,
                    Albedo.Palettized_Plus_Alpha => string.Empty,
                    Albedo.Palettized_Plasma => string.Empty,
                    Albedo.Palettized_2d_Plasma => string.Empty,
                    _ => null,
                },
                BeamMethods.Blend_Mode => (Blend_Mode)option switch
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
                BeamMethods.Black_Point => (Black_Point)option switch
                {
                    Black_Point.Off => string.Empty,
                    Black_Point.On => string.Empty,
                    _ => null,
                },
                BeamMethods.Fog => (Fog)option switch
                {
                    Fog.Off => string.Empty,
                    Fog.On => string.Empty,
                    _ => null,
                },
                BeamMethods.Depth_Fade => (Depth_Fade)option switch
                {
                    Depth_Fade.Off => string.Empty,
                    Depth_Fade.On => string.Empty,
                    Depth_Fade.Palette_Shift => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
