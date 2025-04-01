using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Decal
{
    public class DecalGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(DecalMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((DecalMethods)methodIndex)
            {
                case DecalMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case DecalMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
                case DecalMethods.Render_Pass:
                    return Enum.GetValues(typeof(Render_Pass)).Length;
                case DecalMethods.Specular:
                    return Enum.GetValues(typeof(Specular)).Length;
                case DecalMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping)).Length;
                case DecalMethods.Tinting:
                    return Enum.GetValues(typeof(Tinting)).Length;
                case DecalMethods.Parallax:
                    return Enum.GetValues(typeof(Parallax)).Length;
                case DecalMethods.Interier:
                    return Enum.GetValues(typeof(Interier)).Length;
            }

            return -1;
        }

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Default:
                    return 4;
                default:
                    return -1;
            }
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Default:
                    return true;
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
                        //result.AddSamplerExternWithColorParameter("foreground0_sampler", RenderMethodExtern.none, new ShaderColor(0, 13, 0, 255)); // rmExtern - object_emblem_bitmap_and_data
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
                        //result.AddSamplerExternWithColorParameter("foreground0_sampler", RenderMethodExtern.none, new ShaderColor(0, 13, 0, 255)); // rmExtern - object_emblem_bitmap_and_data
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

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(DecalMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((DecalMethods)methodIndex)
            {
                case DecalMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case DecalMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
                case DecalMethods.Render_Pass:
                    return Enum.GetValues(typeof(Render_Pass));
                case DecalMethods.Specular:
                    return Enum.GetValues(typeof(Specular));
                case DecalMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping));
                case DecalMethods.Tinting:
                    return Enum.GetValues(typeof(Tinting));
                case DecalMethods.Parallax:
                    return Enum.GetValues(typeof(Parallax));
                case DecalMethods.Interier:
                    return Enum.GetValues(typeof(Interier));
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
                VertexType.World,
                VertexType.Rigid,
                VertexType.Skinned,
                VertexType.FlatWorld,
                VertexType.FlatRigid,
                VertexType.FlatSkinned
            };
        }

        public void GetCategoryFunctions(string methodName, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "albedo")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "blend_mode")
            {
                vertexFunction = "invalid";
                pixelFunction = "blend_type";
            }

            if (methodName == "render_pass")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "specular")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "bump_mapping")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "tinting")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "parallax")
            {
                vertexFunction = "calc_parallax_vs";
                pixelFunction = "calc_parallax_ps";
            }

            if (methodName == "interier")
            {
                vertexFunction = "update_interier_layer_vs";
                pixelFunction = "update_interier_layer_ps";
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
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Palettized:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Palettized_Plus_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Diffuse_Plus_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Emblem_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Diffuse_Plus_Alpha_Mask:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Palettized_Plus_Alpha_Mask:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Vector_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Vector_Alpha_Drop_Shadow:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Patchy_Emblem:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
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
                    case Blend_Mode.Additive:
                        vertexFunction = "invalid";
                        pixelFunction = "additive";
                        break;
                    case Blend_Mode.Multiply:
                        vertexFunction = "invalid";
                        pixelFunction = "multiply";
                        break;
                    case Blend_Mode.Alpha_Blend:
                        vertexFunction = "invalid";
                        pixelFunction = "alpha_blend";
                        break;
                    case Blend_Mode.Double_Multiply:
                        vertexFunction = "invalid";
                        pixelFunction = "double_multiply";
                        break;
                    case Blend_Mode.Maximum:
                        vertexFunction = "invalid";
                        pixelFunction = "maximum";
                        break;
                    case Blend_Mode.Multiply_Add:
                        vertexFunction = "invalid";
                        pixelFunction = "multiply_add";
                        break;
                    case Blend_Mode.Add_Src_Times_Dstalpha:
                        vertexFunction = "invalid";
                        pixelFunction = "add_src_times_dstalpha";
                        break;
                    case Blend_Mode.Add_Src_Times_Srcalpha:
                        vertexFunction = "invalid";
                        pixelFunction = "add_src_times_srcalpha";
                        break;
                    case Blend_Mode.Inv_Alpha_Blend:
                        vertexFunction = "invalid";
                        pixelFunction = "inv_alpha_blend";
                        break;
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "pre_multiplied_alpha";
                        break;
                }
            }

            if (methodName == "render_pass")
            {
                switch ((Render_Pass)option)
                {
                    case Render_Pass.Pre_Lighting:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Render_Pass.Post_Lighting:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Render_Pass.Transparent:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "specular")
            {
                switch ((Specular)option)
                {
                    case Specular.Leave:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Specular.Modulate:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "bump_mapping")
            {
                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Leave:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Bump_Mapping.Standard:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Bump_Mapping.Standard_Mask:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "tinting")
            {
                switch ((Tinting)option)
                {
                    case Tinting.None:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Tinting.Unmodulated:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Tinting.Partially_Modulated:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Tinting.Fully_Modulated:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "parallax")
            {
                switch ((Parallax)option)
                {
                    case Parallax.Off:
                        vertexFunction = "calc_parallax_off_vs";
                        pixelFunction = "calc_parallax_off_ps";
                        break;
                    case Parallax.Simple:
                        vertexFunction = "calc_parallax_simple_vs";
                        pixelFunction = "calc_parallax_simple_ps";
                        break;
                    case Parallax.Sphere:
                        vertexFunction = "calc_parallax_sphere_vs";
                        pixelFunction = "calc_parallax_sphere_ps";
                        break;
                }
            }

            if (methodName == "interier")
            {
                switch ((Interier)option)
                {
                    case Interier.Off:
                        vertexFunction = "update_interier_layer_off_vs";
                        pixelFunction = "update_interier_layer_off_ps";
                        break;
                    case Interier.Simple:
                        vertexFunction = "update_interier_layer_simple_vs";
                        pixelFunction = "update_interier_layer_simple_ps";
                        break;
                    case Interier.Floor:
                        vertexFunction = "update_interier_layer_floor_vs";
                        pixelFunction = "update_interier_layer_floor_ps";
                        break;
                    case Interier.Hole:
                        vertexFunction = "update_interier_layer_hole_vs";
                        pixelFunction = "update_interier_layer_hole_ps";
                        break;
                    case Interier.Box:
                        vertexFunction = "update_interier_layer_box_vs";
                        pixelFunction = "update_interier_layer_box_ps";
                        break;
                }
            }
        }
    }
}
