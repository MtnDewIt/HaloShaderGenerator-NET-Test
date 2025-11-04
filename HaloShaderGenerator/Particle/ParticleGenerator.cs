using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Particle
{
    public class ParticleGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(ParticleMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((ParticleMethods)methodIndex)
            {
                case ParticleMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case ParticleMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
                case ParticleMethods.Specialized_Rendering:
                    return Enum.GetValues(typeof(Specialized_Rendering)).Length;
                case ParticleMethods.Lighting:
                    return Enum.GetValues(typeof(Lighting)).Length;
                case ParticleMethods.Render_Targets:
                    return Enum.GetValues(typeof(Render_Targets)).Length;
                case ParticleMethods.Depth_Fade:
                    return Enum.GetValues(typeof(Depth_Fade)).Length;
                case ParticleMethods.Black_Point:
                    return Enum.GetValues(typeof(Black_Point)).Length;
                case ParticleMethods.Fog:
                    return Enum.GetValues(typeof(Fog)).Length;
                case ParticleMethods.Frame_Blend:
                    return Enum.GetValues(typeof(Frame_Blend)).Length;
                case ParticleMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination)).Length;
                case ParticleMethods.Warp:
                    return Enum.GetValues(typeof(Warp)).Length;
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

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(ParticleMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((ParticleMethods)methodIndex)
            {
                case ParticleMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case ParticleMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
                case ParticleMethods.Specialized_Rendering:
                    return Enum.GetValues(typeof(Specialized_Rendering));
                case ParticleMethods.Lighting:
                    return Enum.GetValues(typeof(Lighting));
                case ParticleMethods.Render_Targets:
                    return Enum.GetValues(typeof(Render_Targets));
                case ParticleMethods.Depth_Fade:
                    return Enum.GetValues(typeof(Depth_Fade));
                case ParticleMethods.Black_Point:
                    return Enum.GetValues(typeof(Black_Point));
                case ParticleMethods.Fog:
                    return Enum.GetValues(typeof(Fog));
                case ParticleMethods.Frame_Blend:
                    return Enum.GetValues(typeof(Frame_Blend));
                case ParticleMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination));
                case ParticleMethods.Warp:
                    return Enum.GetValues(typeof(Warp));
            }

            return null;
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

            if (methodName == "specialized_rendering")
            {
                vertexFunction = "";
                pixelFunction = "";
            }

            if (methodName == "lighting")
            {
                vertexFunction = "";
                pixelFunction = "";
            }

            if (methodName == "render_targets")
            {
                vertexFunction = "";
                pixelFunction = "";
            }

            if (methodName == "depth_fade")
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

            if (methodName == "frame_blend")
            {
                vertexFunction = "";
                pixelFunction = "";
            }

            if (methodName == "self_illumination")
            {
                vertexFunction = "";
                pixelFunction = "";
            }

            if (methodName == "warp")
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
                    case Albedo.Diffuse_Plus_Billboard_Alpha:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Albedo.Palettized:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Albedo.Palettized_Plus_Billboard_Alpha:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Albedo.Diffuse_Plus_Sprite_Alpha:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Albedo.Palettized_Plus_Sprite_Alpha:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Albedo.Diffuse_Modulated:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Albedo.Palettized_Glow:
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

            if (methodName == "specialized_rendering")
            {
                switch ((Specialized_Rendering)option)
                {
                    case Specialized_Rendering.None:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Specialized_Rendering.Distortion:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Specialized_Rendering.Distortion_Expensive:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Specialized_Rendering.Distortion_Diffuse:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Specialized_Rendering.Distortion_Expensive_Diffuse:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                }
            }

            if (methodName == "lighting")
            {
                switch ((Lighting)option)
                {
                    case Lighting.None:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Lighting.Per_Pixel_Ravi_Order_3:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Lighting.Per_Vertex_Ravi_Order_0:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Lighting.Per_Pixel_Smooth:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Lighting.Per_Vertex_Ambient:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Lighting.Smoke_Lighting:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                }
            }

            if (methodName == "render_targets")
            {
                switch ((Render_Targets)option)
                {
                    case Render_Targets.Ldr_And_Hdr:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Render_Targets.Ldr_Only:
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
                    case Depth_Fade.Low_Res:
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

            if (methodName == "frame_blend")
            {
                switch ((Frame_Blend)option)
                {
                    case Frame_Blend.Off:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Frame_Blend.On:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                }
            }

            if (methodName == "self_illumination")
            {
                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.None:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Self_Illumination.Constant_Color:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                }
            }

            if (methodName == "warp")
            {
                switch ((Warp)option)
                {
                    case Warp.None:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                    case Warp.Sphere:
                        vertexFunction = "";
                        pixelFunction = "";
                        break;
                }
            }
        }
    }
}
