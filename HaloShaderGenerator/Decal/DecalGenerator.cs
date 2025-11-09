using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Decal
{
    public class DecalGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(DecalMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (DecalMethods)methodIndex switch
            {
                DecalMethods.Albedo => Enum.GetValues(typeof(Albedo)).Length,
                DecalMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)).Length,
                DecalMethods.Render_Pass => Enum.GetValues(typeof(Render_Pass)).Length,
                DecalMethods.Specular => Enum.GetValues(typeof(Specular)).Length,
                DecalMethods.Bump_Mapping => Enum.GetValues(typeof(Bump_Mapping)).Length,
                DecalMethods.Tinting => Enum.GetValues(typeof(Tinting)).Length,
                DecalMethods.Parallax => Enum.GetValues(typeof(Parallax)).Length,
                DecalMethods.Interier => Enum.GetValues(typeof(Interier)).Length,
                _ => -1,
            };
        }

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint)
        {
            return entryPoint switch
            {
                ShaderStage.Default => 4,
                _ => -1,
            };
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            return entryPoint switch
            {
                ShaderStage.Default => true,
                _ => false,
            };
        }

        public bool IsPixelShaderShared(ShaderStage entryPoint) => false;

        public bool IsAutoMacro() => true;

        public ShaderParameters GetGlobalParameters(out string rmopName)
        {
            var result = new ShaderParameters();

            rmopName = @"shaders\decal_options\global_decal_options";

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
                        result.AddFloatParameter("u_tiles", 1.0f);
                        result.AddFloatParameter("v_tiles", 1.0f);
                        rmopName = @"shaders\decal_options\albedo_diffuse_only";
                        break;
                    case Albedo.Palettized:
                        result.AddSamplerAddressParameter("base_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("palette", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatParameter("u_tiles", 1.0f);
                        result.AddFloatParameter("v_tiles", 1.0f);
                        rmopName = @"shaders\decal_options\albedo_palettized";
                        break;
                    case Albedo.Palettized_Plus_Alpha:
                        result.AddSamplerAddressParameter("base_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("palette", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("alpha_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        result.AddFloatParameter("u_tiles", 1.0f);
                        result.AddFloatParameter("v_tiles", 1.0f);
                        rmopName = @"shaders\decal_options\albedo_palettized_plus_alpha";
                        break;
                    case Albedo.Diffuse_Plus_Alpha:
                        result.AddSamplerAddressParameter("base_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("alpha_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        result.AddFloatParameter("u_tiles", 1.0f);
                        result.AddFloatParameter("v_tiles", 1.0f);
                        rmopName = @"shaders\decal_options\albedo_diffuse_plus_alpha";
                        break;
                    case Albedo.Emblem_Change_Color:
                        result.AddSamplerExternAddressParameter("tex0_sampler", RenderMethodExtern.emblem_player_shoulder_texture, ShaderOptionParameter.ShaderAddressMode.BlackBorder, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerExternAddressParameter("tex1_sampler", RenderMethodExtern.emblem_player_shoulder_texture, ShaderOptionParameter.ShaderAddressMode.BlackBorder, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat4ColorExternParameter("emblem_color_background_argb", RenderMethodExtern.none, new ShaderColor(0, 255, 0, 0)); // rmExtern - object_emblem_color_background
                        result.AddFloat4ColorExternParameter("emblem_color_icon1_argb", RenderMethodExtern.none, new ShaderColor(0, 19, 255, 0)); // rmExtern - object_emblem_color_primary
                        result.AddFloat4ColorExternParameter("emblem_color_icon2_argb", RenderMethodExtern.none, new ShaderColor(0, 13, 0, 255)); // rmExtern - object_emblem_color_secondary
                        result.AddFloatParameter("u_tiles", 1.0f);
                        result.AddFloatParameter("v_tiles", 1.0f);
                        result.AddSamplerExternWithColorParameter("foreground0_sampler", RenderMethodExtern.none, new ShaderColor(0, 13, 0, 255), @"shaders\default_bitmaps\bitmaps\gray_50_percent"); // rmExtern - object_emblem_bitmap_and_data
                        rmopName = @"shaders\decal_options\albedo_emblem_change_color";
                        break;
                    case Albedo.Change_Color:
                        result.AddSamplerAddressParameter("change_color_map", ShaderOptionParameter.ShaderAddressMode.BlackBorder, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat3ColorExternParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, new ShaderColor(0, 255, 0, 0));
                        result.AddFloat3ColorExternParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary, new ShaderColor(0, 19, 255, 0));
                        result.AddFloat3ColorExternParameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary, new ShaderColor(0, 13, 0, 255));
                        result.AddFloatParameter("u_tiles", 1.0f);
                        result.AddFloatParameter("v_tiles", 1.0f);
                        rmopName = @"shaders\decal_options\albedo_change_color";
                        break;
                    case Albedo.Diffuse_Plus_Alpha_Mask:
                        result.AddSamplerAddressParameter("base_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("alpha_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        result.AddFloatParameter("u_tiles", 1.0f);
                        result.AddFloatParameter("v_tiles", 1.0f);                        
                        rmopName = @"shaders\decal_options\albedo_diffuse_plus_alpha_mask";
                        break;
                    case Albedo.Palettized_Plus_Alpha_Mask:
                        result.AddSamplerAddressParameter("base_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("palette", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerAddressParameter("alpha_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        result.AddFloatParameter("u_tiles", 1.0f);
                        result.AddFloatParameter("v_tiles", 1.0f);
                        rmopName = @"shaders\decal_options\albedo_palettized_plus_alpha_mask";
                        break;
                    case Albedo.Vector_Alpha:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatParameter("u_tiles", 1.0f);
                        result.AddFloatParameter("v_tiles", 1.0f);
                        result.AddSamplerParameter("vector_map", @"shaders\default_bitmaps\bitmaps\reference_grids");
                        result.AddFloatParameter("vector_sharpness", 1000.0f);
                        result.AddFloatParameter("antialias_tweak", 0.025f);
                        rmopName = @"shaders\decal_options\albedo_vector_alpha";
                        break;
                    case Albedo.Vector_Alpha_Drop_Shadow:
                        result.AddSamplerParameter("base_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatParameter("u_tiles", 1.0f);
                        result.AddFloatParameter("v_tiles", 1.0f);
                        result.AddSamplerParameter("vector_map", @"shaders\default_bitmaps\bitmaps\reference_grids");
                        result.AddFloatParameter("vector_sharpness", 1000.0f);
                        result.AddSamplerParameter("shadow_vector_map", @"shaders\default_bitmaps\bitmaps\reference_grids");
                        result.AddFloatParameter("shadow_darkness", 1.0f);
                        result.AddFloatParameter("shadow_sharpness", 2.0f);
                        result.AddFloatParameter("antialias_tweak", 0.025f);
                        rmopName = @"shaders\decal_options\albedo_vector_alpha_drop_shadow";
                        break;
                    case Albedo.Patchy_Emblem:
                        result.AddSamplerExternWithColorParameter("foreground0_sampler", RenderMethodExtern.none, new ShaderColor(0, 13, 0, 255), @"shaders\default_bitmaps\bitmaps\gray_50_percent"); // rmExtern - object_emblem_bitmap_and_data
                        result.AddSamplerParameter("alpha_map", @"shaders\default_bitmaps\bitmaps\clouds_256");
                        result.AddFloatParameter("alpha_max", 1.0f);
                        result.AddFloatParameter("alpha_min");
                        rmopName = @"shaders\decal_options\albedo_patchy_emblem";
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

            if (methodName == "render_pass")
            {
                optionName = ((Render_Pass)option).ToString();

                switch ((Render_Pass)option)
                {
                    case Render_Pass.Pre_Lighting:
                        break;
                    case Render_Pass.Post_Lighting:
                        break;
                    case Render_Pass.Transparent:
                        break;
                }
            }

            if (methodName == "specular")
            {
                optionName = ((Specular)option).ToString();

                switch ((Specular)option)
                {
                    case Specular.Leave:
                        break;
                    case Specular.Modulate:
                        result.AddFloatParameter("specular_multiplier");
                        rmopName = @"shaders\decal_options\specular_modulate";
                        break;
                }
            }

            if (methodName == "bump_mapping")
            {
                optionName = ((Bump_Mapping)option).ToString();

                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Leave:
                        break;
                    case Bump_Mapping.Standard:
                        result.AddSamplerAddressParameter("bump_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\decal_options\bump_mapping_standard";
                        break;
                    case Bump_Mapping.Standard_Mask:
                        result.AddSamplerAddressParameter("bump_map", ShaderOptionParameter.ShaderAddressMode.Clamp, @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\decal_options\bump_mapping_standard_mask";
                        break;
                }
            }

            if (methodName == "tinting")
            {
                optionName = ((Tinting)option).ToString();

                switch ((Tinting)option)
                {
                    case Tinting.None:
                        break;
                    case Tinting.Unmodulated:
                        result.AddFloat3ColorParameter("tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("intensity", 1.0f);
                        rmopName = @"shaders\decal_options\tinting_unmodulated";
                        break;
                    case Tinting.Partially_Modulated:
                        result.AddFloat3ColorParameter("tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("intensity", 1.0f);
                        result.AddFloatParameter("modulation_factor", 1.0f);
                        rmopName = @"shaders\decal_options\tinting_partially_modulated";
                        break;
                    case Tinting.Fully_Modulated:
                        result.AddFloat3ColorParameter("tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("intensity", 1.0f);
                        rmopName = @"shaders\decal_options\tinting_fully_modulated";
                        break;
                }
            }

            if (methodName == "parallax")
            {
                optionName = ((Parallax)option).ToString();

                switch ((Parallax)option)
                {
                    case Parallax.Off:
                        break;
                    case Parallax.Simple:
                        result.AddSamplerParameter("height_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        result.AddFloatParameter("height_scale", 0.1f);
                        rmopName = @"shaders\decal_options\parallax_simple";
                        break;
                    case Parallax.Sphere:
                        result.AddFloatParameter("sphere_radius", 0.5f);
                        result.AddFloatParameter("sphere_height", 0.2f);
                        rmopName = @"shaders\decal_options\parallax_sphere";
                        break;
                }
            }

            if (methodName == "interier")
            {
                optionName = ((Interier)option).ToString();

                switch ((Interier)option)
                {
                    case Interier.Off:
                        break;
                    case Interier.Simple:
                        result.AddSamplerParameter("interier", @"shaders\default_bitmaps\bitmaps\checker_board");
                        result.AddFloatParameter("mask_threshold", 0.5f);
                        rmopName = @"shaders\decal_options\interier_shell";
                        break;
                    case Interier.Floor:
                        result.AddSamplerParameter("interier", @"shaders\default_bitmaps\bitmaps\checker_board");
                        result.AddFloatParameter("mask_threshold", 0.5f);
                        result.AddFloatParameter("thin_shell_height", 0.5f);
                        rmopName = @"shaders\decal_options\interier_thin_shell";
                        break;
                    case Interier.Hole:
                        result.AddSamplerParameter("interier", @"shaders\default_bitmaps\bitmaps\checker_board");
                        result.AddFloatParameter("mask_threshold", 0.5f);
                        result.AddFloatParameter("thin_shell_height", 0.5f);
                        result.AddSamplerParameter("wall_map", @"shaders\default_bitmaps\bitmaps\checker_board");
                        result.AddFloatParameter("hole_radius", 0.5f);
                        result.AddFloatParameter("fog_factor", 0.5f);
                        result.AddFloat3ColorWithFloatParameter("fog_top_color", 0.5f, new ShaderColor(0, 121, 116, 116));
                        result.AddFloat3ColorWithFloatParameter("fog_bottom_color", 0.5f, new ShaderColor(0, 121, 116, 116));
                        rmopName = @"shaders\decal_options\interier_hole";
                        break;
                    case Interier.Box:                        
                        result.AddSamplerParameter("interier", @"shaders\default_bitmaps\bitmaps\random");
                        result.AddFloatParameter("mask_threshold", 0.5f);
                        result.AddFloatParameter("thin_shell_height", 0.5f);
                        result.AddSamplerParameter("wall_map", @"shaders\default_bitmaps\bitmaps\random");
                        result.AddFloatParameter("box_size", 0.5f);
                        result.AddFloatParameter("fog_factor", 0.5f);
                        result.AddFloat3ColorWithFloatParameter("fog_top_color", 0.5f, new ShaderColor(0, 121, 116, 116));
                        result.AddFloat3ColorWithFloatParameter("fog_bottom_color", 0.5f, new ShaderColor(0, 121, 116, 116));
                        rmopName = @"shaders\decal_options\interier_box";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames() => Enum.GetValues(typeof(DecalMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (DecalMethods)methodIndex switch
            {
                DecalMethods.Albedo => Enum.GetValues(typeof(Albedo)),
                DecalMethods.Blend_Mode => Enum.GetValues(typeof(Blend_Mode)),
                DecalMethods.Render_Pass => Enum.GetValues(typeof(Render_Pass)),
                DecalMethods.Specular => Enum.GetValues(typeof(Specular)),
                DecalMethods.Bump_Mapping => Enum.GetValues(typeof(Bump_Mapping)),
                DecalMethods.Tinting => Enum.GetValues(typeof(Tinting)),
                DecalMethods.Parallax => Enum.GetValues(typeof(Parallax)),
                DecalMethods.Interier => Enum.GetValues(typeof(Interier)),
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
                VertexType.World,
                VertexType.Rigid,
                VertexType.Skinned,
                VertexType.FlatWorld,
                VertexType.FlatRigid,
                VertexType.FlatSkinned
            };
        }

        public string GetCategoryPixelFunction(int category)
        {
            return (DecalMethods)category switch
            {
                DecalMethods.Albedo => string.Empty,
                DecalMethods.Blend_Mode => "blend_type",
                DecalMethods.Render_Pass => string.Empty,
                DecalMethods.Specular => string.Empty,
                DecalMethods.Bump_Mapping => string.Empty,
                DecalMethods.Tinting => string.Empty,
                DecalMethods.Parallax => "calc_parallax_ps",
                DecalMethods.Interier => "update_interier_layer_ps",
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (DecalMethods)category switch
            {
                DecalMethods.Albedo => string.Empty,
                DecalMethods.Blend_Mode => string.Empty,
                DecalMethods.Render_Pass => string.Empty,
                DecalMethods.Specular => string.Empty,
                DecalMethods.Bump_Mapping => string.Empty,
                DecalMethods.Tinting => string.Empty,
                DecalMethods.Parallax => "calc_parallax_vs",
                DecalMethods.Interier => "update_interier_layer_vs",
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (DecalMethods)category switch
            {
                DecalMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Diffuse_Only => string.Empty,
                    Albedo.Palettized => string.Empty,
                    Albedo.Palettized_Plus_Alpha => string.Empty,
                    Albedo.Diffuse_Plus_Alpha => string.Empty,
                    Albedo.Emblem_Change_Color => string.Empty,
                    Albedo.Change_Color => string.Empty,
                    Albedo.Diffuse_Plus_Alpha_Mask => string.Empty,
                    Albedo.Palettized_Plus_Alpha_Mask => string.Empty,
                    Albedo.Vector_Alpha => string.Empty,
                    Albedo.Vector_Alpha_Drop_Shadow => string.Empty,
                    Albedo.Patchy_Emblem => string.Empty,
                    _ => null,
                },
                DecalMethods.Blend_Mode => (Blend_Mode)option switch
                {
                    Blend_Mode.Opaque => "opaque",
                    Blend_Mode.Additive => "additive",
                    Blend_Mode.Multiply => "multiply",
                    Blend_Mode.Alpha_Blend => "alpha_blend",
                    Blend_Mode.Double_Multiply => "double_multiply",
                    Blend_Mode.Maximum => "maximum",
                    Blend_Mode.Multiply_Add => "multiply_add",
                    Blend_Mode.Add_Src_Times_Dstalpha => "add_src_times_dstalpha",
                    Blend_Mode.Add_Src_Times_Srcalpha => "add_src_times_srcalpha",
                    Blend_Mode.Inv_Alpha_Blend => "inv_alpha_blend",
                    Blend_Mode.Pre_Multiplied_Alpha => "pre_multiplied_alpha",
                    _ => null,
                },
                DecalMethods.Render_Pass => (Render_Pass)option switch
                {
                    Render_Pass.Pre_Lighting => string.Empty,
                    Render_Pass.Post_Lighting => string.Empty,
                    Render_Pass.Transparent => string.Empty,
                    _ => null,
                },
                DecalMethods.Specular => (Specular)option switch
                {
                    Specular.Leave => string.Empty,
                    Specular.Modulate => string.Empty,
                    _ => null,
                },
                DecalMethods.Bump_Mapping => (Bump_Mapping)option switch
                {
                    Bump_Mapping.Leave => string.Empty,
                    Bump_Mapping.Standard => string.Empty,
                    Bump_Mapping.Standard_Mask => string.Empty,
                    _ => null,
                },
                DecalMethods.Tinting => (Tinting)option switch
                {
                    Tinting.None => string.Empty,
                    Tinting.Unmodulated => string.Empty,
                    Tinting.Partially_Modulated => string.Empty,
                    Tinting.Fully_Modulated => string.Empty,
                    _ => null,
                },
                DecalMethods.Parallax => (Parallax)option switch
                {
                    Parallax.Off => "calc_parallax_off_ps",
                    Parallax.Simple => "calc_parallax_simple_ps",
                    Parallax.Sphere => "calc_parallax_sphere_ps",
                    _ => null,
                },
                DecalMethods.Interier => (Interier)option switch
                {
                    Interier.Off => "update_interier_layer_off_ps",
                    Interier.Simple => "update_interier_layer_simple_ps",
                    Interier.Floor => "update_interier_layer_floor_ps",
                    Interier.Hole => "update_interier_layer_hole_ps",
                    Interier.Box => "update_interier_layer_box_ps",
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (DecalMethods)category switch
            {
                DecalMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Diffuse_Only => string.Empty,
                    Albedo.Palettized => string.Empty,
                    Albedo.Palettized_Plus_Alpha => string.Empty,
                    Albedo.Diffuse_Plus_Alpha => string.Empty,
                    Albedo.Emblem_Change_Color => string.Empty,
                    Albedo.Change_Color => string.Empty,
                    Albedo.Diffuse_Plus_Alpha_Mask => string.Empty,
                    Albedo.Palettized_Plus_Alpha_Mask => string.Empty,
                    Albedo.Vector_Alpha => string.Empty,
                    Albedo.Vector_Alpha_Drop_Shadow => string.Empty,
                    Albedo.Patchy_Emblem => string.Empty,
                    _ => null,
                },
                DecalMethods.Blend_Mode => (Blend_Mode)option switch
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
                DecalMethods.Render_Pass => (Render_Pass)option switch
                {
                    Render_Pass.Pre_Lighting => string.Empty,
                    Render_Pass.Post_Lighting => string.Empty,
                    Render_Pass.Transparent => string.Empty,
                    _ => null,
                },
                DecalMethods.Specular => (Specular)option switch
                {
                    Specular.Leave => string.Empty,
                    Specular.Modulate => string.Empty,
                    _ => null,
                },
                DecalMethods.Bump_Mapping => (Bump_Mapping)option switch
                {
                    Bump_Mapping.Leave => string.Empty,
                    Bump_Mapping.Standard => string.Empty,
                    Bump_Mapping.Standard_Mask => string.Empty,
                    _ => null,
                },
                DecalMethods.Tinting => (Tinting)option switch
                {
                    Tinting.None => string.Empty,
                    Tinting.Unmodulated => string.Empty,
                    Tinting.Partially_Modulated => string.Empty,
                    Tinting.Fully_Modulated => string.Empty,
                    _ => null,
                },
                DecalMethods.Parallax => (Parallax)option switch
                {
                    Parallax.Off => "calc_parallax_off_vs",
                    Parallax.Simple => "calc_parallax_simple_vs",
                    Parallax.Sphere => "calc_parallax_sphere_vs",
                    _ => null,
                },
                DecalMethods.Interier => (Interier)option switch
                {
                    Interier.Off => "update_interier_layer_off_vs",
                    Interier.Simple => "update_interier_layer_simple_vs",
                    Interier.Floor => "update_interier_layer_floor_vs",
                    Interier.Hole => "update_interier_layer_hole_vs",
                    Interier.Box => "update_interier_layer_box_vs",
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
