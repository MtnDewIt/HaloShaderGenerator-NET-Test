using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Particle
{
    public class ParticleGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(ParticleMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (ParticleMethods)methodIndex switch
            {
                ParticleMethods.Albedo => Enum.GetValues(typeof(Albedo)).Length,
                ParticleMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)).Length,
                ParticleMethods.Specialized_Rendering => Enum.GetValues(typeof(Specialized_Rendering)).Length,
                ParticleMethods.Lighting => Enum.GetValues(typeof(Lighting)).Length,
                ParticleMethods.Render_Targets => Enum.GetValues(typeof(Render_Targets)).Length,
                ParticleMethods.Depth_Fade => Enum.GetValues(typeof(Depth_Fade)).Length,
                ParticleMethods.Black_Point => Enum.GetValues(typeof(Black_Point)).Length,
                ParticleMethods.Fog => Enum.GetValues(typeof(Fog)).Length,
                ParticleMethods.Frame_Blend => Enum.GetValues(typeof(Frame_Blend)).Length,
                ParticleMethods.Self_Illumination => Enum.GetValues(typeof(Self_Illumination)).Length,
                ParticleMethods.Warp => Enum.GetValues(typeof(Warp)).Length,
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
            result.AddFloat3ColorExternParameter("screen_constants", RenderMethodExtern.screen_constants);
            rmopName = @"shaders\particle_options\global_particle_options";

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
                        result.AddSamplerAddressParameter("base_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\particle_options\albedo_diffuse_only";
                        break;
                    case Albedo.Diffuse_Plus_Billboard_Alpha:
                        result.AddSamplerAddressParameter("base_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("alpha_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        rmopName = @"shaders\particle_options\albedo_diffuse_plus_billboard_alpha";
                        break;
                    case Albedo.Palettized:
                        result.AddSamplerAddressParameter("base_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("palette", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\particle_options\albedo_palettized";
                        break;
                    case Albedo.Palettized_Plus_Billboard_Alpha:
                        result.AddSamplerAddressParameter("base_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("palette", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("alpha_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        rmopName = @"shaders\particle_options\albedo_palettized_plus_billboard_alpha";
                        break;
                    case Albedo.Diffuse_Plus_Sprite_Alpha:
                        result.AddSamplerAddressParameter("base_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("alpha_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        rmopName = @"shaders\particle_options\albedo_diffuse_plus_sprite_alpha";
                        break;
                    case Albedo.Palettized_Plus_Sprite_Alpha:
                        result.AddSamplerAddressParameter("base_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("palette", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("alpha_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        rmopName = @"shaders\particle_options\albedo_palettized_plus_sprite_alpha";
                        break;
                    case Albedo.Diffuse_Modulated:
                        result.AddSamplerAddressParameter("base_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorParameter("tint_color", new ShaderColor(0, 255, 34, 2));
                        result.AddFloatParameter("modulation_factor", 1.0f);
                        rmopName = @"shaders\particle_options\albedo_diffuse_modulated";
                        break;
                    case Albedo.Palettized_Glow:
                        result.AddSamplerAddressParameter("base_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorParameter("tint_color", new ShaderColor(0, 255, 74, 14));
                        rmopName = @"shaders\particle_options\albedo_palettized_glow";
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

            if (methodName == "specialized_rendering")
            {
                optionName = ((Specialized_Rendering)option).ToString();

                switch ((Specialized_Rendering)option)
                {
                    case Specialized_Rendering.None:
                        break;
                    case Specialized_Rendering.Distortion:
                        result.AddFloatParameter("distortion_scale", 2.0f);
                        rmopName = @"shaders\particle_options\specialized_rendering_distortion";
                        break;
                    case Specialized_Rendering.Distortion_Expensive:
                        result.AddFloatParameter("distortion_scale", 2.0f);
                        rmopName = @"shaders\particle_options\specialized_rendering_distortion";
                        break;
                    case Specialized_Rendering.Distortion_Diffuse:
                        result.AddFloatParameter("distortion_scale", 2.0f);
                        rmopName = @"shaders\particle_options\specialized_rendering_distortion";
                        break;
                    case Specialized_Rendering.Distortion_Expensive_Diffuse:
                        result.AddFloatParameter("distortion_scale", 2.0f);
                        rmopName = @"shaders\particle_options\specialized_rendering_distortion";
                        break;
                }
            }

            if (methodName == "lighting")
            {
                optionName = ((Lighting)option).ToString();

                switch ((Lighting)option)
                {
                    case Lighting.None:
                        break;
                    case Lighting.Per_Pixel_Ravi_Order_3:
                        break;
                    case Lighting.Per_Vertex_Ravi_Order_0:
                        break;
                    case Lighting.Per_Pixel_Smooth:
                        result.AddFloatParameter("contrast_scale", 0.5f);
                        result.AddFloatParameter("contrast_offset", 0.5f);
                        rmopName = @"shaders\particle_options\smooth_lighting";
                        break;
                    case Lighting.Per_Vertex_Ambient:
                        break;
                    case Lighting.Smoke_Lighting:
                        result.AddFloatParameter("bump_contrast", 1.5f);
                        result.AddFloatParameter("bump_randomness", 0.2f);
                        rmopName = @"shaders\particle_options\smoke_lighting";
                        break;
                }
            }

            if (methodName == "render_targets")
            {
                optionName = ((Render_Targets)option).ToString();

                switch ((Render_Targets)option)
                {
                    case Render_Targets.Ldr_And_Hdr:
                        break;
                    case Render_Targets.Ldr_Only:
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
                    case Depth_Fade.Low_Res:
                        result.AddFloatParameter("depth_fade_range", 0.1f);
                        rmopName = @"shaders\particle_options\depth_fade_on";
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

            if (methodName == "frame_blend")
            {
                optionName = ((Frame_Blend)option).ToString();

                switch ((Frame_Blend)option)
                {
                    case Frame_Blend.Off:
                        break;
                    case Frame_Blend.On:
                        result.AddFloatParameter("starting_uv_scale", 1.0f);
                        result.AddFloatParameter("ending_uv_scale", 1.0f);
                        rmopName = @"shaders\particle_options\frame_blend_on";
                        break;
                }
            }

            if (methodName == "self_illumination")
            {
                optionName = ((Self_Illumination)option).ToString();

                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.None:
                        break;
                    case Self_Illumination.Constant_Color:
                        result.AddFloat3ColorParameter("self_illum_color");
                        rmopName = @"shaders\particle_options\self_illumination_constant_color";
                        break;
                }
            }

            if (methodName == "warp")
            {
                optionName = ((Warp)option).ToString();

                switch ((Warp)option)
                {
                    case Warp.None:
                        break;
                    case Warp.Sphere:
                        result.AddFloatParameter("sphere_warp_scale", 0.41f);
                        rmopName = @"shaders\particle_options\warp_sphere";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames() => Enum.GetValues(typeof(ParticleMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (ParticleMethods)methodIndex switch
            {
                ParticleMethods.Albedo => Enum.GetValues(typeof(Albedo)),
                ParticleMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)),
                ParticleMethods.Specialized_Rendering => Enum.GetValues(typeof(Specialized_Rendering)),
                ParticleMethods.Lighting => Enum.GetValues(typeof(Lighting)),
                ParticleMethods.Render_Targets => Enum.GetValues(typeof(Render_Targets)),
                ParticleMethods.Depth_Fade => Enum.GetValues(typeof(Depth_Fade)),
                ParticleMethods.Black_Point => Enum.GetValues(typeof(Black_Point)),
                ParticleMethods.Fog => Enum.GetValues(typeof(Fog)),
                ParticleMethods.Frame_Blend => Enum.GetValues(typeof(Frame_Blend)),
                ParticleMethods.Self_Illumination => Enum.GetValues(typeof(Self_Illumination)),
                ParticleMethods.Warp => Enum.GetValues(typeof(Warp)),
                _ => null,
            };
        }

        public Array GetEntryPointOrder()
        {
            return new ShaderStage[]
            {
                ShaderStage.Default,
                //case ShaderStage.Static_Default
            };
        }

        public Array GetVertexTypeOrder()
        {
            return new VertexType[]
            {
                VertexType.Particle,
                VertexType.ParticleModel
            };
        }

        public string GetCategoryPixelFunction(int category)
        {
            return (ParticleMethods)category switch
            {
                ParticleMethods.Albedo => string.Empty,
                ParticleMethods.Blend_Mode => string.Empty,
                ParticleMethods.Specialized_Rendering => string.Empty,
                ParticleMethods.Lighting => string.Empty,
                ParticleMethods.Render_Targets => string.Empty,
                ParticleMethods.Depth_Fade => string.Empty,
                ParticleMethods.Black_Point => string.Empty,
                ParticleMethods.Fog => string.Empty,
                ParticleMethods.Frame_Blend => string.Empty,
                ParticleMethods.Self_Illumination => string.Empty,
                ParticleMethods.Warp => string.Empty,
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (ParticleMethods)category switch
            {
                ParticleMethods.Albedo => string.Empty,
                ParticleMethods.Blend_Mode => string.Empty,
                ParticleMethods.Specialized_Rendering => string.Empty,
                ParticleMethods.Lighting => string.Empty,
                ParticleMethods.Render_Targets => string.Empty,
                ParticleMethods.Depth_Fade => string.Empty,
                ParticleMethods.Black_Point => string.Empty,
                ParticleMethods.Fog => string.Empty,
                ParticleMethods.Frame_Blend => string.Empty,
                ParticleMethods.Self_Illumination => string.Empty,
                ParticleMethods.Warp => string.Empty,
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (ParticleMethods)category switch
            {
                ParticleMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Diffuse_Only => string.Empty,
                    Albedo.Diffuse_Plus_Billboard_Alpha => string.Empty,
                    Albedo.Palettized => string.Empty,
                    Albedo.Palettized_Plus_Billboard_Alpha => string.Empty,
                    Albedo.Diffuse_Plus_Sprite_Alpha => string.Empty,
                    Albedo.Palettized_Plus_Sprite_Alpha => string.Empty,
                    Albedo.Diffuse_Modulated => string.Empty,
                    Albedo.Palettized_Glow => string.Empty,
                    Albedo.Palettized_Plasma => string.Empty,
                    Albedo.Palettized_2d_Plasma => string.Empty,
                    _ => null,
                },
                ParticleMethods.Blend_Mode => (Blend_Mode)option switch
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
                ParticleMethods.Specialized_Rendering => (Specialized_Rendering)option switch
                {
                    Specialized_Rendering.None => string.Empty,
                    Specialized_Rendering.Distortion => string.Empty,
                    Specialized_Rendering.Distortion_Expensive => string.Empty,
                    Specialized_Rendering.Distortion_Diffuse => string.Empty,
                    Specialized_Rendering.Distortion_Expensive_Diffuse => string.Empty,
                    _ => null,
                },
                ParticleMethods.Lighting => (Lighting)option switch
                {
                    Lighting.None => string.Empty,
                    Lighting.Per_Pixel_Ravi_Order_3 => string.Empty,
                    Lighting.Per_Vertex_Ravi_Order_0 => string.Empty,
                    Lighting.Per_Pixel_Smooth => string.Empty,
                    Lighting.Per_Vertex_Ambient => string.Empty,
                    Lighting.Smoke_Lighting => string.Empty,
                    _ => null,
                },
                ParticleMethods.Render_Targets => (Render_Targets)option switch
                {
                    Render_Targets.Ldr_And_Hdr => string.Empty,
                    Render_Targets.Ldr_Only => string.Empty,
                    _ => null,
                },
                ParticleMethods.Depth_Fade => (Depth_Fade)option switch
                {
                    Depth_Fade.Off => string.Empty,
                    Depth_Fade.On => string.Empty,
                    Depth_Fade.Palette_Shift => string.Empty,
                    Depth_Fade.Low_Res => string.Empty,
                    _ => null,
                },
                ParticleMethods.Black_Point => (Black_Point)option switch
                {
                    Black_Point.Off => string.Empty,
                    Black_Point.On => string.Empty,
                    _ => null,
                },
                ParticleMethods.Fog => (Fog)option switch
                {
                    Fog.Off => string.Empty,
                    Fog.On => string.Empty,
                    _ => null,
                },
                ParticleMethods.Frame_Blend => (Frame_Blend)option switch
                {
                    Frame_Blend.Off => string.Empty,
                    Frame_Blend.On => string.Empty,
                    _ => null,
                },
                ParticleMethods.Self_Illumination => (Self_Illumination)option switch
                {
                    Self_Illumination.None => string.Empty,
                    Self_Illumination.Constant_Color => string.Empty,
                    _ => null,
                },
                ParticleMethods.Warp => (Warp)option switch
                {
                    Warp.None => string.Empty,
                    Warp.Sphere => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (ParticleMethods)category switch
            {
                ParticleMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Diffuse_Only => string.Empty,
                    Albedo.Diffuse_Plus_Billboard_Alpha => string.Empty,
                    Albedo.Palettized => string.Empty,
                    Albedo.Palettized_Plus_Billboard_Alpha => string.Empty,
                    Albedo.Diffuse_Plus_Sprite_Alpha => string.Empty,
                    Albedo.Palettized_Plus_Sprite_Alpha => string.Empty,
                    Albedo.Diffuse_Modulated => string.Empty,
                    Albedo.Palettized_Glow => string.Empty,
                    Albedo.Palettized_Plasma => string.Empty,
                    Albedo.Palettized_2d_Plasma => string.Empty,
                    _ => null,
                },
                ParticleMethods.Blend_Mode => (Blend_Mode)option switch
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
                ParticleMethods.Specialized_Rendering => (Specialized_Rendering)option switch
                {
                    Specialized_Rendering.None => string.Empty,
                    Specialized_Rendering.Distortion => string.Empty,
                    Specialized_Rendering.Distortion_Expensive => string.Empty,
                    Specialized_Rendering.Distortion_Diffuse => string.Empty,
                    Specialized_Rendering.Distortion_Expensive_Diffuse => string.Empty,
                    _ => null,
                },
                ParticleMethods.Lighting => (Lighting)option switch
                {
                    Lighting.None => string.Empty,
                    Lighting.Per_Pixel_Ravi_Order_3 => string.Empty,
                    Lighting.Per_Vertex_Ravi_Order_0 => string.Empty,
                    Lighting.Per_Pixel_Smooth => string.Empty,
                    Lighting.Per_Vertex_Ambient => string.Empty,
                    Lighting.Smoke_Lighting => string.Empty,
                    _ => null,
                },
                ParticleMethods.Render_Targets => (Render_Targets)option switch
                {
                    Render_Targets.Ldr_And_Hdr => string.Empty,
                    Render_Targets.Ldr_Only => string.Empty,
                    _ => null,
                },
                ParticleMethods.Depth_Fade => (Depth_Fade)option switch
                {
                    Depth_Fade.Off => string.Empty,
                    Depth_Fade.On => string.Empty,
                    Depth_Fade.Palette_Shift => string.Empty,
                    Depth_Fade.Low_Res => string.Empty,
                    _ => null,
                },
                ParticleMethods.Black_Point => (Black_Point)option switch
                {
                    Black_Point.Off => string.Empty,
                    Black_Point.On => string.Empty,
                    _ => null,
                },
                ParticleMethods.Fog => (Fog)option switch
                {
                    Fog.Off => string.Empty,
                    Fog.On => string.Empty,
                    _ => null,
                },
                ParticleMethods.Frame_Blend => (Frame_Blend)option switch
                {
                    Frame_Blend.Off => string.Empty,
                    Frame_Blend.On => string.Empty,
                    _ => null,
                },
                ParticleMethods.Self_Illumination => (Self_Illumination)option switch
                {
                    Self_Illumination.None => string.Empty,
                    Self_Illumination.Constant_Color => string.Empty,
                    _ => null,
                },
                ParticleMethods.Warp => (Warp)option switch
                {
                    Warp.None => string.Empty,
                    Warp.Sphere => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
