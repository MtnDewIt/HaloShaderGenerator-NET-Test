using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Fur
{
    public class FurGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(FurMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (FurMethods)methodIndex switch
            {
                FurMethods.Albedo => Enum.GetValues(typeof(Albedo)).Length,
                FurMethods.Warp => Enum.GetValues(typeof(Warp)).Length,
                FurMethods.Overlay => Enum.GetValues(typeof(Overlay)).Length,
                FurMethods.Edge_Fade => Enum.GetValues(typeof(Edge_Fade)).Length,
                FurMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)).Length,
                _ => -1,
            };
        }

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint) => -1;

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint) => false;

        public bool IsPixelShaderShared(ShaderStage entryPoint) => false;

        public bool IsAutoMacro() => false;

        public ShaderParameters GetGlobalParameters(out string rmopName)
        {
            var result = new ShaderParameters();

            result.AddSamplerExternParameter("albedo_texture", RenderMethodExtern.texture_global_target_texaccum);
            result.AddSamplerExternParameter("normal_texture", RenderMethodExtern.texture_global_target_normal);
            result.AddSamplerExternFilterParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture, ShaderOptionParameter.ShaderFilterMode.Bilinear);
            result.AddSamplerExternFilterAddressParameter("shadow_depth_map_1", RenderMethodExtern.texture_global_target_shadow_buffer1, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp);
            result.AddSamplerExternParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0);
            result.AddFloat3ColorExternWithFloatAndIntegerParameter("debug_tint", RenderMethodExtern.debug_tint, 1.0f, 1, new ShaderColor(255, 255, 255, 255));
            result.AddSamplerExternParameter("active_camo_distortion_texture", RenderMethodExtern.active_camo_distortion_texture);
            result.AddSamplerExternParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerExternParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture);
            result.AddSamplerExternParameter("dominant_light_intensity_map", RenderMethodExtern.texture_dominant_light_intensity_map);
            //result.AddSamplerFilterAddressParameter("g_direction_lut", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\direction_lut_1002");
            //result.AddSamplerFilterAddressParameter("g_sample_vmf_diffuse", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\diffusetable");
            //result.AddSamplerFilterAddressParameter("g_sample_vmf_diffuse_vs", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\diffusetable");
            //result.AddSamplerExternFilterAddressParameter("g_sample_vmf_phong_specular", RenderMethodExtern.material_diffuse_power, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp);
            //result.AddSamplerExternFilterAddressParameter("shadow_mask_texture", RenderMethodExtern.none, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp); // rmExtern - texture_global_target_shadow_mask
            rmopName = @"shaders\shader_options\global_shader_options";

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
                        result.AddSamplerParameter("fur_hairs_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("fur_tint_map", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloat3ColorParameter("fur_deep_color", new ShaderColor(0, 17, 17, 17));
                        result.AddFloat3ColorParameter("fur_tint_color", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("fur_intensity", 1.0f);
                        result.AddFloatParameter("fur_alpha_scale", 1.0f);
                        result.AddFloatParameter("fur_shear_x");
                        result.AddFloatParameter("fur_shear_y");
                        result.AddFloatParameter("fur_fix", 1.0f);
                        result.AddFloatParameter("layer_depth", 0.1f);
                        result.AddIntegerWithFloatParameter("layers_of_4", 3.0f, 3);
                        result.AddFloatParameter("texcoord_aspect_ratio", 1.0f);
                        result.AddFloatParameter("depth_darken", 1.0f);
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
                        result.AddSamplerParameter("warp_map", @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddFloatParameter("warp_amount_x", 1.0f);
                        result.AddFloatParameter("warp_amount_y", 1.0f);
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
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\overlay_additive";
                        break;
                    case Overlay.Additive_Detail:
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddSamplerParameter("overlay_detail_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\overlay_additive_detail";
                        break;
                    case Overlay.Multiply:
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
                        rmopName = @"shaders\shader_options\overlay_additive";
                        break;
                    case Overlay.Multiply_And_Additive_Detail:
                        result.AddSamplerParameter("overlay_multiply_map", @"shaders\default_bitmaps\bitmaps\reference_grids");
                        result.AddSamplerParameter("overlay_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddSamplerParameter("overlay_detail_map", @"shaders\default_bitmaps\bitmaps\dither_pattern");
                        result.AddFloat3ColorWithFloatParameter("overlay_tint", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("overlay_intensity", 1.0f);
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
                        result.AddFloat3ColorParameter("edge_fade_edge_tint");
                        result.AddFloat3ColorParameter("edge_fade_center_tint", new ShaderColor(0, 255, 255, 255));
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

        public Array GetMethodNames() => Enum.GetValues(typeof(FurMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (FurMethods)methodIndex switch
            {
                FurMethods.Albedo => Enum.GetValues(typeof(Albedo)),
                FurMethods.Warp => Enum.GetValues(typeof(Warp)),
                FurMethods.Overlay => Enum.GetValues(typeof(Overlay)),
                FurMethods.Edge_Fade => Enum.GetValues(typeof(Edge_Fade)),
                FurMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)),
                _ => null,
            };
        }

        public Array GetEntryPointOrder()
        {
            return new ShaderStage[]
            {
                ShaderStage.Albedo,
                ShaderStage.Static_Sh,
                ShaderStage.Static_Prt_Ambient,
                ShaderStage.Dynamic_Light
                //ShaderStage.Imposter_Static_Sh,
                //ShaderStage.Imposter_Static_Prt_Ambient
            };
        }

        public Array GetVertexTypeOrder()
        {
            return new VertexType[]
            {
                VertexType.Rigid,
                VertexType.Skinned
            };
        }

        public string GetCategoryPixelFunction(int category)
        {
            return (FurMethods)category switch
            {
                FurMethods.Albedo => "calc_albedo_ps",
                FurMethods.Warp => "calc_parallax_ps",
                FurMethods.Overlay => "calc_overlay_ps",
                FurMethods.Edge_Fade => "calc_edge_fade_ps",
                FurMethods.Blend_Mode => "blend_type",
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (FurMethods)category switch
            {
                FurMethods.Albedo => string.Empty,
                FurMethods.Warp => string.Empty,
                FurMethods.Overlay => string.Empty,
                FurMethods.Edge_Fade => string.Empty,
                FurMethods.Blend_Mode => string.Empty,
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (FurMethods)category switch
            {
                FurMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Fur_Multilayer => "calc_albedo_multilayer_ps",
                    _ => null,
                },
                FurMethods.Warp => (Warp)option switch
                {
                    Warp.None => "calc_parallax_off_ps",
                    Warp.From_Texture => "calc_warp_from_texture_ps",
                    Warp.Parallax_Simple => "calc_parallax_simple_ps",
                    _ => null,
                },
                FurMethods.Overlay => (Overlay)option switch
                {
                    Overlay.None => "calc_overlay_none_ps",
                    Overlay.Additive => "calc_overlay_additive_ps",
                    Overlay.Additive_Detail => "calc_overlay_additive_detail_ps",
                    Overlay.Multiply => "calc_overlay_multiply_ps",
                    Overlay.Multiply_And_Additive_Detail => "calc_overlay_multiply_and_additive_detail_ps",
                    _ => null,
                },
                FurMethods.Edge_Fade => (Edge_Fade)option switch
                {
                    Edge_Fade.None => "calc_edge_fade_none_ps",
                    Edge_Fade.Simple => "calc_edge_fade_simple_ps",
                    _ => null,
                },
                FurMethods.Blend_Mode => (Blend_Mode)option switch
                {
                    Blend_Mode.Opaque => "opaque",
                    Blend_Mode.Alpha_Blend => "alpha_blend",
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (FurMethods)category switch
            {
                FurMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Fur_Multilayer => string.Empty,
                    _ => null,
                },
                FurMethods.Warp => (Warp)option switch
                {
                    Warp.None => string.Empty,
                    Warp.From_Texture => string.Empty,
                    Warp.Parallax_Simple => string.Empty,
                    _ => null,
                },
                FurMethods.Overlay => (Overlay)option switch
                {
                    Overlay.None => string.Empty,
                    Overlay.Additive => string.Empty,
                    Overlay.Additive_Detail => string.Empty,
                    Overlay.Multiply => string.Empty,
                    Overlay.Multiply_And_Additive_Detail => string.Empty,
                    _ => null,
                },
                FurMethods.Edge_Fade => (Edge_Fade)option switch
                {
                    Edge_Fade.None => string.Empty,
                    Edge_Fade.Simple => string.Empty,
                    _ => null,
                },
                FurMethods.Blend_Mode => (Blend_Mode)option switch
                {
                    Blend_Mode.Opaque => string.Empty,
                    Blend_Mode.Alpha_Blend => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
