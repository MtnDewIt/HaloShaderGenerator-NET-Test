using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Fur
{
    public class FurGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(FurMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((FurMethods)methodIndex)
            {
                case FurMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case FurMethods.Warp:
                    return Enum.GetValues(typeof(Warp)).Length;
                case FurMethods.Overlay:
                    return Enum.GetValues(typeof(Overlay)).Length;
                case FurMethods.Edge_Fade:
                    return Enum.GetValues(typeof(Edge_Fade)).Length;
                case FurMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
            }

            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Albedo:
                case ShaderStage.Static_Sh:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Dynamic_Light:
                //case ShaderStage.Imposter_Static_Sh:
                //case ShaderStage.Imposter_Static_Prt_Ambient:
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
                case VertexType.Rigid:
                case VertexType.Skinned:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsVertexShaderShared(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Albedo:
                case ShaderStage.Static_Sh:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Dynamic_Light:
                    return true;
                default:
                    return false;
            }
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();
            result.AddFloat3ColorExternWithFloatAndIntegerParameter("debug_tint", RenderMethodExtern.debug_tint, 1.0f, 1, new ShaderColor(255, 255, 255, 255));
            result.AddSamplerExternParameter("active_camo_distortion_texture", RenderMethodExtern.active_camo_distortion_texture);
            result.AddSamplerExternParameter("albedo_texture", RenderMethodExtern.texture_global_target_texaccum);
            result.AddSamplerExternParameter("dominant_light_intensity_map", RenderMethodExtern.texture_dominant_light_intensity_map);
            result.AddSamplerExternParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0);
            result.AddSamplerAddressParameter("g_diffuse_power_specular", ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\diffuse_power_specular\diffuse_power");
            result.AddSamplerFilterAddressParameter("g_direction_lut", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\direction_lut_1002");
            result.AddSamplerFilterAddressParameter("g_sample_vmf_diffuse_vs", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\diffusetable");
            result.AddSamplerFilterAddressParameter("g_sample_vmf_diffuse", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\diffusetable");
            result.AddSamplerFilterAddressParameter("g_sample_vmf_phong_specular", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\diffuse_power_specular\diffuse_power");
            result.AddSamplerExternFilterParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture, ShaderOptionParameter.ShaderFilterMode.Bilinear);
            result.AddSamplerExternParameter("normal_texture", RenderMethodExtern.texture_global_target_normal);
            result.AddSamplerExternParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture);
            result.AddSamplerExternParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerExternFilterAddressParameter("shadow_depth_map_1", RenderMethodExtern.texture_global_target_shadow_buffer1, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp);
            result.AddSamplerExternFilterAddressParameter("shadow_mask_texture", RenderMethodExtern.none, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp); // rmExtern - texture_global_target_shadow_mask
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
                    case Albedo.Fur_Multilayer:
                        result.AddFloat3ColorParameter("fur_deep_color", new ShaderColor(0, 17, 17, 17));
                        result.AddFloat3ColorParameter("fur_tint_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("depth_darken", 1.0f);
                        result.AddFloatParameter("fur_alpha_scale", 1.0f);
                        result.AddFloatParameter("fur_fix", 1.0f);
                        result.AddFloatParameter("fur_intensity", 1.0f);
                        result.AddFloatParameter("fur_shear_x");
                        result.AddFloatParameter("fur_shear_y");
                        result.AddFloatParameter("layer_depth", 0.1f);
                        result.AddFloatParameter("texcoord_aspect_ratio", 1.0f);
                        result.AddIntegerWithFloatParameter("layers_of_4", 3.0f, 3);
                        result.AddSamplerParameter("fur_hairs_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("fur_tint_map", @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\fur_options\fur_multilayer";
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
                    case Warp.From_Texture:
                        result.AddFloatParameter("warp_amount_x", 1.0f);
                        result.AddFloatParameter("warp_amount_y", 1.0f);
                        result.AddSamplerParameter("warp_map", @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        rmopName = @"shaders\shader_options\warp_from_texture";
                        break;
                    case Warp.Parallax_Simple:
                        break;
                }
            }

            if (methodName == "overlay")
            {
                optionName = ((Overlay)option).ToString();

                switch ((Overlay)option)
                {
                    case Overlay.None:
                        break;
                    case Overlay.Additive:
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        rmopName = @"shaders\shader_options\overlay_additive";
                        break;
                    case Overlay.Additive_Detail:
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
                        result.AddSamplerParameter("overlay_detail_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        rmopName = @"shaders\shader_options\overlay_additive_detail";
                        break;
                    case Overlay.Multiply:
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        rmopName = @"shaders\shader_options\overlay_additive";
                        break;
                    case Overlay.Multiply_And_Additive_Detail:
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
                        result.AddSamplerParameter("overlay_detail_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddSamplerParameter("overlay_multiply_map", @"shaders\default_bitmaps\bitmaps\reference_grids");
                        rmopName = @"shaders\shader_options\overlay_multiply_additive_detail";
                        break;
                }
            }

            if (methodName == "edge_fade")
            {
                optionName = ((Edge_Fade)option).ToString();

                switch ((Edge_Fade)option)
                {
                    case Edge_Fade.None:
                        break;
                    case Edge_Fade.Simple:
                        result.AddFloat3ColorParameter("edge_fade_center_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat3ColorParameter("edge_fade_edge_tint");
                        result.AddFloatParameter("edge_fade_power", 1.0f);
                        rmopName = @"shaders\shader_options\edge_fade_simple";
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
                    case Blend_Mode.Alpha_Blend:
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(FurMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((FurMethods)methodIndex)
            {
                case FurMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case FurMethods.Warp:
                    return Enum.GetValues(typeof(Warp));
                case FurMethods.Overlay:
                    return Enum.GetValues(typeof(Overlay));
                case FurMethods.Edge_Fade:
                    return Enum.GetValues(typeof(Edge_Fade));
                case FurMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
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
                pixelFunction = "calc_albedo_ps";
            }

            if (methodName == "warp")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_parallax_ps";
            }

            if (methodName == "overlay")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_overlay_ps";
            }

            if (methodName == "edge_fade")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_edge_fade_ps";
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

            if (methodName == "albedo")
            {
                switch ((Albedo)option)
                {
                    case Albedo.Fur_Multilayer:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_albedo_multilayer_ps";
                        break;
                }
            }

            if (methodName == "warp")
            {
                switch ((Warp)option)
                {
                    case Warp.None:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_parallax_off_ps";
                        break;
                    case Warp.From_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_warp_from_texture_ps";
                        break;
                    case Warp.Parallax_Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_parallax_simple_ps";
                        break;
                }
            }

            if (methodName == "overlay")
            {
                switch ((Overlay)option)
                {
                    case Overlay.None:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_none_ps";
                        break;
                    case Overlay.Additive:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_additive_ps";
                        break;
                    case Overlay.Additive_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_additive_detail_ps";
                        break;
                    case Overlay.Multiply:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_multiply_ps";
                        break;
                    case Overlay.Multiply_And_Additive_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_multiply_and_additive_detail_ps";
                        break;
                }
            }

            if (methodName == "edge_fade")
            {
                switch ((Edge_Fade)option)
                {
                    case Edge_Fade.None:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_edge_fade_none_ps";
                        break;
                    case Edge_Fade.Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_edge_fade_simple_ps";
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
                    case Blend_Mode.Alpha_Blend:
                        vertexFunction = "invalid";
                        pixelFunction = "alpha_blend";
                        break;
                }
            }
        }
    }
}
