using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Screen
{
    public class ScreenGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(ScreenMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (ScreenMethods)methodIndex switch
            {
                ScreenMethods.Warp => Enum.GetValues(typeof(Warp)).Length,
                ScreenMethods.Base => Enum.GetValues(typeof(Base)).Length,
                ScreenMethods.Overlay_A => Enum.GetValues(typeof(Overlay_A)).Length,
                ScreenMethods.Overlay_B => Enum.GetValues(typeof(Overlay_B)).Length,
                ScreenMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)).Length,
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

            rmopName = @"shaders\screen_options\global_screen_options";

            return result;
        }

        public ShaderParameters GetParametersInOption(string methodName, int option, out string rmopName, out string optionName)
        {
            ShaderParameters result = new ShaderParameters();
            rmopName = null;
            optionName = null;

            if (methodName == "warp")
            {
                optionName = ((Warp)option).ToString();

                switch ((Warp)option)
                {
                    case Warp.None:
                        break;
                    case Warp.Pixel_Space:
                        result.AddSamplerFilterParameter("warp_map", ShaderOptionParameter.ShaderFilterMode.Bilinear, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddFloatParameter("warp_amount", 1.0f);
                        rmopName = @"shaders\screen_options\warp_simple";
                        break;
                    case Warp.Screen_Space:
                        result.AddSamplerFilterParameter("warp_map", ShaderOptionParameter.ShaderFilterMode.Bilinear, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddFloatParameter("warp_amount", 1.0f);
                        rmopName = @"shaders\screen_options\warp_simple";
                        break;
                }
            }

            if (methodName == "base")
            {
                optionName = ((Base)option).ToString();

                switch ((Base)option)
                {
                    case Base.Single_Screen_Space:
                        result.AddSamplerFilterParameter("base_map", ShaderOptionParameter.ShaderFilterMode.Bilinear, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        rmopName = @"shaders\screen_options\base_single";
                        break;
                    case Base.Single_Pixel_Space:
                        result.AddSamplerFilterParameter("base_map", ShaderOptionParameter.ShaderFilterMode.Bilinear, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        rmopName = @"shaders\screen_options\base_single";
                        break;
                    case Base.Normal_Map_Edge_Shade:
                        result.AddSamplerFilterAddressParameter("normal_map", ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerFilterAddressParameter("palette", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddFloatParameter("palette_v", 1.0f);
                        rmopName = @"shaders\screen_options\base_normal_map_edge_shade";
                        break;
                    case Base.Single_Target_Space:
                        result.AddSamplerFilterParameter("base_map", ShaderOptionParameter.ShaderFilterMode.Bilinear, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        rmopName = @"shaders\screen_options\base_single";
                        break;
                    case Base.Normal_Map_Edge_Stencil:
                        result.AddSamplerFilterAddressParameter("normal_map", ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerFilterAddressParameter("palette", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddFloatParameter("palette_v", 1.0f);
                        result.AddSamplerFilterAddressParameter("stencil_map", ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\screen_options\base_normal_map_edge_stencil";
                        break;
                }
            }

            if (methodName == "overlay_a")
            {
                optionName = ((Overlay_A)option).ToString();

                switch ((Overlay_A)option)
                {
                    case Overlay_A.None:
                        break;
                    case Overlay_A.Tint_Add_Color:
                        result.AddFloat4ColorWithFloatParameter("tint_color", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("add_color");
                        rmopName = @"shaders\screen_options\overlay_tint_add_color";
                        break;
                    case Overlay_A.Detail_Screen_Space:
                        result.AddSamplerFilterParameter("detail_map_a", ShaderOptionParameter.ShaderFilterMode.Bilinear, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddFloatParameter("detail_fade_a", 1.0f);
                        result.AddFloatParameter("detail_multiplier_a", 4.59479f);
                        rmopName = @"shaders\screen_options\detail_a";
                        break;
                    case Overlay_A.Detail_Pixel_Space:
                        result.AddSamplerFilterParameter("detail_map_a", ShaderOptionParameter.ShaderFilterMode.Bilinear, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddFloatParameter("detail_fade_a", 1.0f);
                        result.AddFloatParameter("detail_multiplier_a", 4.59479f);
                        rmopName = @"shaders\screen_options\detail_a";
                        break;
                    case Overlay_A.Detail_Masked_Screen_Space:
                        result.AddSamplerFilterParameter("detail_map_a", ShaderOptionParameter.ShaderFilterMode.Bilinear, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddSamplerParameter("detail_mask_a", @"shaders\default_bitmaps\bitmaps\color_red");
                        result.AddFloatParameter("detail_fade_a", 1.0f);
                        result.AddFloatParameter("detail_multiplier_a", 4.59479f);
                        rmopName = @"shaders\screen_options\detail_mask_a";
                        break;
                    case Overlay_A.Palette_Lookup:
                        result.AddSamplerFilterParameter("detail_map_a", ShaderOptionParameter.ShaderFilterMode.Bilinear, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddFloatParameter("detail_fade_a", 1.0f);
                        result.AddFloat3ColorWithFloatParameter("intensity_color_u", 4.59479f, new ShaderColor(0, 255, 0, 0));
                        result.AddFloat3ColorWithFloatParameter("intensity_color_v", 4.59479f, new ShaderColor(0, 0, 255, 0));
                        rmopName = @"shaders\screen_options\palette_lookup_a";
                        break;
                }
            }

            if (methodName == "overlay_b")
            {
                optionName = ((Overlay_B)option).ToString();

                switch ((Overlay_B)option)
                {
                    case Overlay_B.None:
                        break;
                    case Overlay_B.Tint_Add_Color:
                        result.AddFloat4ColorWithFloatParameter("tint_color", 1.0f, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("add_color");
                        rmopName = @"shaders\screen_options\overlay_tint_add_color";
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
                        result.AddFloatParameter("fade", 1.0f);
                        rmopName = @"shaders\screen_options\blend";
                        break;
                    case Blend_Mode.Multiply:
                        result.AddFloatParameter("fade", 1.0f);
                        rmopName = @"shaders\screen_options\blend";
                        break;
                    case Blend_Mode.Alpha_Blend:
                        result.AddFloatParameter("fade", 1.0f);
                        rmopName = @"shaders\screen_options\blend";
                        break;
                    case Blend_Mode.Double_Multiply:
                        result.AddFloatParameter("fade", 1.0f);
                        rmopName = @"shaders\screen_options\blend";
                        break;
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        result.AddFloatParameter("fade", 1.0f);
                        rmopName = @"shaders\screen_options\blend";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames() => Enum.GetValues(typeof(ScreenMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (ScreenMethods)methodIndex switch
            {
                ScreenMethods.Warp => Enum.GetValues(typeof(Warp)),
                ScreenMethods.Base => Enum.GetValues(typeof(Base)),
                ScreenMethods.Overlay_A => Enum.GetValues(typeof(Overlay_A)),
                ScreenMethods.Overlay_B => Enum.GetValues(typeof(Overlay_B)),
                ScreenMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)),
                _ => null,
            };
        }

        public Array GetEntryPointOrder()
        {
            return new ShaderStage[]
            {
                ShaderStage.Default
                //ShaderStage.Albedo
            };
        }

        public Array GetVertexTypeOrder()
        {
            return new VertexType[]
            {
                VertexType.Screen
            };
        }

        public string GetCategoryPixelFunction(int category)
        {
            return (ScreenMethods)category switch
            {
                ScreenMethods.Warp => "warp_type",
                ScreenMethods.Base => "base_type",
                ScreenMethods.Overlay_A => "overlay_a_type",
                ScreenMethods.Overlay_B => "overlay_b_type",
                ScreenMethods.Blend_Mode => "blend_type",
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (ScreenMethods)category switch
            {
                ScreenMethods.Warp => string.Empty,
                ScreenMethods.Base => string.Empty,
                ScreenMethods.Overlay_A => string.Empty,
                ScreenMethods.Overlay_B => string.Empty,
                ScreenMethods.Blend_Mode => string.Empty,
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (ScreenMethods)category switch
            {
                ScreenMethods.Warp => (Warp)option switch
                {
                    Warp.None => "none",
                    Warp.Pixel_Space => "pixel_space",
                    Warp.Screen_Space => "screen_space",
                    _ => null,
                },
                ScreenMethods.Base => (Base)option switch
                {
                    Base.Single_Screen_Space => "single_screen_space",
                    Base.Single_Pixel_Space => "single_pixel_space",
                    Base.Normal_Map_Edge_Shade => "normal_map_edge_shade",
                    Base.Single_Target_Space => "single_target_space",
                    Base.Normal_Map_Edge_Stencil => "normal_map_edge_stencil",
                    _ => null,
                },
                ScreenMethods.Overlay_A => (Overlay_A)option switch
                {
                    Overlay_A.None => "none",
                    Overlay_A.Tint_Add_Color => "tint_add_color",
                    Overlay_A.Detail_Screen_Space => "detail_screen_space",
                    Overlay_A.Detail_Pixel_Space => "detail_pixel_space",
                    Overlay_A.Detail_Masked_Screen_Space => "detail_masked_screen_space",
                    Overlay_A.Palette_Lookup => "palette_lookup",
                    _ => null,
                },
                ScreenMethods.Overlay_B => (Overlay_B)option switch
                {
                    Overlay_B.None => "none",
                    Overlay_B.Tint_Add_Color => "tint_add_color",
                    _ => null,
                },
                ScreenMethods.Blend_Mode => (Blend_Mode)option switch
                {
                    Blend_Mode.Opaque => "opaque",
                    Blend_Mode.Additive => "additive",
                    Blend_Mode.Multiply => "multiply",
                    Blend_Mode.Alpha_Blend => "alpha_blend",
                    Blend_Mode.Double_Multiply => "double_multiply",
                    Blend_Mode.Pre_Multiplied_Alpha => "pre_multiplied_alpha",
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (ScreenMethods)category switch
            {
                ScreenMethods.Warp => (Warp)option switch
                {
                    Warp.None => string.Empty,
                    Warp.Pixel_Space => string.Empty,
                    Warp.Screen_Space => string.Empty,
                    _ => null,
                },
                ScreenMethods.Base => (Base)option switch
                {
                    Base.Single_Screen_Space => string.Empty,
                    Base.Single_Pixel_Space => string.Empty,
                    Base.Normal_Map_Edge_Shade => string.Empty,
                    Base.Single_Target_Space => string.Empty,
                    Base.Normal_Map_Edge_Stencil => string.Empty,
                    _ => null,
                },
                ScreenMethods.Overlay_A => (Overlay_A)option switch
                {
                    Overlay_A.None => string.Empty,
                    Overlay_A.Tint_Add_Color => string.Empty,
                    Overlay_A.Detail_Screen_Space => string.Empty,
                    Overlay_A.Detail_Pixel_Space => string.Empty,
                    Overlay_A.Detail_Masked_Screen_Space => string.Empty,
                    Overlay_A.Palette_Lookup => string.Empty,
                    _ => null,
                },
                ScreenMethods.Overlay_B => (Overlay_B)option switch
                {
                    Overlay_B.None => string.Empty,
                    Overlay_B.Tint_Add_Color => string.Empty,
                    _ => null,
                },
                ScreenMethods.Blend_Mode => (Blend_Mode)option switch
                {
                    Blend_Mode.Opaque => string.Empty,
                    Blend_Mode.Additive => string.Empty,
                    Blend_Mode.Multiply => string.Empty,
                    Blend_Mode.Alpha_Blend => string.Empty,
                    Blend_Mode.Double_Multiply => string.Empty,
                    Blend_Mode.Pre_Multiplied_Alpha => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
