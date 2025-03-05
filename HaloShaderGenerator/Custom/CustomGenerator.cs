using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Custom
{
    public class CustomGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;
        private bool ApplyFixes;

        Albedo albedo;
        Bump_Mapping bump_mapping;
        Alpha_Test alpha_test;
        Specular_Mask specular_mask;
        Material_Model material_model;
        Environment_Mapping environment_mapping;
        Self_Illumination self_illumination;
        Blend_Mode blend_mode;
        Parallax parallax;
        Misc misc;
        Wetness wetness;

        public CustomGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        public CustomGenerator(Albedo albedo, Bump_Mapping bump_mapping, Alpha_Test alpha_test, Specular_Mask specular_mask, Material_Model material_model, Environment_Mapping environment_mapping, Self_Illumination self_illumination, Blend_Mode blend_mode, Parallax parallax, Misc misc, Wetness wetness, bool applyFixes = false)
        {
            this.albedo = albedo;
            this.bump_mapping = bump_mapping;
            this.alpha_test = alpha_test;
            this.specular_mask = specular_mask;
            this.material_model = material_model;
            this.environment_mapping = environment_mapping;
            this.self_illumination = self_illumination;
            this.blend_mode = blend_mode;
            this.parallax = parallax;
            this.misc = misc;
            this.wetness = wetness;

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public CustomGenerator(byte[] options, bool applyFixes = false)
        {
            options = ValidateOptions(options);

            this.albedo = (Albedo)options[0];
            this.bump_mapping = (Bump_Mapping)options[1];
            this.alpha_test = (Alpha_Test)options[2];
            this.specular_mask = (Specular_Mask)options[3];
            this.material_model = (Material_Model)options[4];
            this.environment_mapping = (Environment_Mapping)options[5];
            this.self_illumination = (Self_Illumination)options[6];
            this.blend_mode = (Blend_Mode)options[7];
            this.parallax = (Parallax)options[8];
            this.misc = (Misc)options[9];
            this.wetness = (Wetness)options[10];

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public ShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Alpha_Test sAlphaTest = (Shared.Alpha_Test)Enum.Parse(typeof(Shared.Alpha_Test), alpha_test.ToString());
            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());
            Shader.Misc sMisc = (Shader.Misc)Enum.Parse(typeof(Shader.Misc), misc.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Custom, entryPoint, sBlendMode,
                sMisc, sAlphaTest, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes);

            switch (albedo)
            {
                case Albedo.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_default_ps"));
                    break;
                case Albedo.Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_detail_blend_ps"));
                    break;
                case Albedo.Constant_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_constant_color_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_constant_color_ps"));
                    break;
                case Albedo.Two_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_ps"));
                    break;
                case Albedo.Four_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_four_change_color_ps"));
                    break;
                case Albedo.Three_Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_three_detail_blend_ps"));
                    break;
                case Albedo.Two_Detail_Overlay:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_detail_overlay_ps"));
                    break;
                case Albedo.Two_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_detail_ps"));
                    break;
                case Albedo.Color_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_color_mask_ps"));
                    break;
                case Albedo.Two_Detail_Black_Point:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_detail_black_point_ps"));
                    break;
                case Albedo.Waterfall:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_waterfall_ps"));
                    break;
                case Albedo.Multiply_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_multiply_map_ps"));
                    break;
                case Albedo.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_simple_ps"));
                    break;
            }

            switch (bump_mapping)
            {
                case Bump_Mapping.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_off_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_off_ps"));
                    break;
                case Bump_Mapping.Standard:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_default_ps"));
                    break;
                case Bump_Mapping.Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_ps"));
                    break;
            }

            switch (alpha_test)
            {
                case Alpha_Test.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_test", "off"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", "calc_alpha_test_off_ps"));
                    break;
                case Alpha_Test.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_test", "on"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", "calc_alpha_test_on_ps"));
                    break;
                case Alpha_Test.Multiply_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_test", "multmap"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", "calc_alpha_test_multiply_map_ps"));
                    break;
            }

            switch (specular_mask)
            {
                case Specular_Mask.No_Specular_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_no_specular_mask_ps"));
                    break;
                case Specular_Mask.Specular_Mask_From_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_from_diffuse_ps"));
                    break;
                case Specular_Mask.Specular_Mask_From_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_texture_ps"));
                    break;
                case Specular_Mask.Specular_Mask_Mult_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_mult_texture_ps"));
                    break;
                case Specular_Mask.Specular_Mask_From_Color_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_color_texture_ps"));
                    break;
            }

            switch (material_model)
            {
                case Material_Model.Diffuse_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "diffuse_only"));
                    break;
                case Material_Model.Two_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong"));
                    break;
                case Material_Model.Foliage:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "foliage"));
                    break;
                case Material_Model.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "none"));
                    break;
                case Material_Model.Custom_Specular:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "custom_specular"));
                    break;
            }

            switch (environment_mapping)
            {
                case Environment_Mapping.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "none"));
                    break;
                case Environment_Mapping.Per_Pixel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "per_pixel"));
                    break;
                case Environment_Mapping.Dynamic:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "dynamic"));
                    break;
                case Environment_Mapping.From_Flat_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "from_flat_texture"));
                    break;
                case Environment_Mapping.Per_Pixel_Mip:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "per_pixel_mip"));
                    break;
                case Environment_Mapping.Dynamic_Reach:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "dynamic_reach"));
                    break;
            }

            switch (self_illumination)
            {
                case Self_Illumination.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_none_ps"));
                    break;
                case Self_Illumination.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_simple_ps"));
                    break;
                case Self_Illumination._3_Channel_Self_Illum:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_three_channel_ps"));
                    break;
                case Self_Illumination.Plasma:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_plasma_ps"));
                    break;
                case Self_Illumination.From_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_from_albedo_ps"));
                    break;
                case Self_Illumination.Illum_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_detail_ps"));
                    break;
                case Self_Illumination.Meter:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_meter_ps"));
                    break;
                case Self_Illumination.Self_Illum_Times_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_times_diffuse_ps"));
                    break;
                case Self_Illumination.Window_Room:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_window_room_ps"));
                    break;
            }

            switch (blend_mode)
            {
                case Blend_Mode.Opaque:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "opaque"));
                    break;
                case Blend_Mode.Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "additive"));
                    break;
                case Blend_Mode.Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "multiply"));
                    break;
                case Blend_Mode.Alpha_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "alpha_blend"));
                    break;
                case Blend_Mode.Double_Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "double_multiply"));
                    break;
            }

            switch (parallax)
            {
                case Parallax.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_off_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Parallax.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_simple_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
                case Parallax.Interpolated:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_interpolated_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_interpolated_ps"));
                    break;
                case Parallax.Simple_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_simple_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_detail_ps"));
                    break;
            }

            switch (misc)
            {
                case Misc.First_Person_Never:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.First_Person_Sometimes:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.First_Person_Always:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.First_Person_Never_With_Rotating_Bitmaps:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "1"));
                    break;
                case Misc.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.Rotating_Bitmaps_Super_Slow:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "1"));
                    break;
                case Misc.Always_Calc_Albedo:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "2"));
                    break;
            }

            switch (wetness)
            {
                case Wetness.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_default_ps"));
                    break;
                case Wetness.Flood:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_flood_ps"));
                    break;
                case Wetness.Proof:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_proof_ps"));
                    break;
                case Wetness.Ripples:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_ripples_ps"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Custom);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Alpha_Test sAlphaTest = (Shared.Alpha_Test)Enum.Parse(typeof(Shared.Alpha_Test), alpha_test.ToString());
            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());
            Shader.Misc sMisc = (Shader.Misc)Enum.Parse(typeof(Shader.Misc), misc.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Custom, entryPoint, sBlendMode,
                sMisc, sAlphaTest, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes);

            switch (albedo)
            {
                case Albedo.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_default_ps"));
                    break;
                case Albedo.Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_detail_blend_ps"));
                    break;
                case Albedo.Constant_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_constant_color_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_constant_color_ps"));
                    break;
                case Albedo.Two_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_ps"));
                    break;
                case Albedo.Four_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_four_change_color_ps"));
                    break;
                case Albedo.Three_Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_three_detail_blend_ps"));
                    break;
                case Albedo.Two_Detail_Overlay:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_detail_overlay_ps"));
                    break;
                case Albedo.Two_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_detail_ps"));
                    break;
                case Albedo.Color_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_color_mask_ps"));
                    break;
                case Albedo.Two_Detail_Black_Point:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_detail_black_point_ps"));
                    break;
                case Albedo.Waterfall:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_waterfall_ps"));
                    break;
                case Albedo.Multiply_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_multiply_map_ps"));
                    break;
                case Albedo.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_simple_ps"));
                    break;
            }

            switch (bump_mapping)
            {
                case Bump_Mapping.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_off_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_off_ps"));
                    break;
                case Bump_Mapping.Standard:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_default_ps"));
                    break;
                case Bump_Mapping.Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_ps"));
                    break;
            }

            switch (alpha_test)
            {
                case Alpha_Test.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_test", "off"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", "calc_alpha_test_off_ps"));
                    break;
                case Alpha_Test.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_test", "on"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", "calc_alpha_test_on_ps"));
                    break;
                case Alpha_Test.Multiply_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_test", "multmap"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", "calc_alpha_test_multiply_map_ps"));
                    break;
            }

            switch (specular_mask)
            {
                case Specular_Mask.No_Specular_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_no_specular_mask_ps"));
                    break;
                case Specular_Mask.Specular_Mask_From_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_from_diffuse_ps"));
                    break;
                case Specular_Mask.Specular_Mask_From_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_texture_ps"));
                    break;
                case Specular_Mask.Specular_Mask_Mult_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_mult_texture_ps"));
                    break;
                case Specular_Mask.Specular_Mask_From_Color_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_color_texture_ps"));
                    break;
            }

            switch (material_model)
            {
                case Material_Model.Diffuse_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "diffuse_only"));
                    break;
                case Material_Model.Two_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong"));
                    break;
                case Material_Model.Foliage:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "foliage"));
                    break;
                case Material_Model.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "none"));
                    break;
                case Material_Model.Custom_Specular:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "custom_specular"));
                    break;
            }

            switch (environment_mapping)
            {
                case Environment_Mapping.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "none"));
                    break;
                case Environment_Mapping.Per_Pixel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "per_pixel"));
                    break;
                case Environment_Mapping.Dynamic:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "dynamic"));
                    break;
                case Environment_Mapping.From_Flat_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "from_flat_texture"));
                    break;
                case Environment_Mapping.Per_Pixel_Mip:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "per_pixel_mip"));
                    break;
                case Environment_Mapping.Dynamic_Reach:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "dynamic_reach"));
                    break;
            }

            switch (self_illumination)
            {
                case Self_Illumination.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_none_ps"));
                    break;
                case Self_Illumination.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_simple_ps"));
                    break;
                case Self_Illumination._3_Channel_Self_Illum:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_three_channel_ps"));
                    break;
                case Self_Illumination.Plasma:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_plasma_ps"));
                    break;
                case Self_Illumination.From_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_from_albedo_ps"));
                    break;
                case Self_Illumination.Illum_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_detail_ps"));
                    break;
                case Self_Illumination.Meter:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_meter_ps"));
                    break;
                case Self_Illumination.Self_Illum_Times_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_times_diffuse_ps"));
                    break;
                case Self_Illumination.Window_Room:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_window_room_ps"));
                    break;
            }

            switch (blend_mode)
            {
                case Blend_Mode.Opaque:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "opaque"));
                    break;
                case Blend_Mode.Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "additive"));
                    break;
                case Blend_Mode.Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "multiply"));
                    break;
                case Blend_Mode.Alpha_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "alpha_blend"));
                    break;
                case Blend_Mode.Double_Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "double_multiply"));
                    break;
            }

            switch (parallax)
            {
                case Parallax.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_off_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Parallax.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_simple_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
                case Parallax.Interpolated:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_interpolated_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_interpolated_ps"));
                    break;
                case Parallax.Simple_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_simple_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_detail_ps"));
                    break;
            }

            switch (misc)
            {
                case Misc.First_Person_Never:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.First_Person_Sometimes:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.First_Person_Always:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.First_Person_Never_With_Rotating_Bitmaps:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "1"));
                    break;
                case Misc.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.Rotating_Bitmaps_Super_Slow:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "1"));
                    break;
                case Misc.Always_Calc_Albedo:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "2"));
                    break;
            }

            switch (wetness)
            {
                case Wetness.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_default_ps"));
                    break;
                case Wetness.Flood:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_flood_ps"));
                    break;
                case Wetness.Proof:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_proof_ps"));
                    break;
                case Wetness.Ripples:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_ripples_ps"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Custom);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Alpha_Test sAlphaTest = (Shared.Alpha_Test)Enum.Parse(typeof(Shared.Alpha_Test), alpha_test.ToString());
            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());
            Shader.Misc sMisc = (Shader.Misc)Enum.Parse(typeof(Shader.Misc), misc.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Custom, entryPoint,
                sBlendMode, sMisc, sAlphaTest, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes, true, vertexType);

            switch (albedo)
            {
                case Albedo.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_default_ps"));
                    break;
                case Albedo.Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_detail_blend_ps"));
                    break;
                case Albedo.Constant_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_constant_color_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_constant_color_ps"));
                    break;
                case Albedo.Two_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_ps"));
                    break;
                case Albedo.Four_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_four_change_color_ps"));
                    break;
                case Albedo.Three_Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_three_detail_blend_ps"));
                    break;
                case Albedo.Two_Detail_Overlay:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_detail_overlay_ps"));
                    break;
                case Albedo.Two_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_detail_ps"));
                    break;
                case Albedo.Color_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_color_mask_ps"));
                    break;
                case Albedo.Two_Detail_Black_Point:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_detail_black_point_ps"));
                    break;
                case Albedo.Waterfall:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_waterfall_ps"));
                    break;
                case Albedo.Multiply_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_multiply_map_ps"));
                    break;
                case Albedo.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_simple_ps"));
                    break;
            }

            switch (bump_mapping)
            {
                case Bump_Mapping.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_off_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_off_ps"));
                    break;
                case Bump_Mapping.Standard:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_default_ps"));
                    break;
                case Bump_Mapping.Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_ps"));
                    break;
            }

            switch (alpha_test)
            {
                case Alpha_Test.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_test", "off"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", "calc_alpha_test_off_ps"));
                    break;
                case Alpha_Test.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_test", "on"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", "calc_alpha_test_on_ps"));
                    break;
                case Alpha_Test.Multiply_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_test", "multmap"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", "calc_alpha_test_multiply_map_ps"));
                    break;
            }

            switch (specular_mask)
            {
                case Specular_Mask.No_Specular_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_no_specular_mask_ps"));
                    break;
                case Specular_Mask.Specular_Mask_From_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_from_diffuse_ps"));
                    break;
                case Specular_Mask.Specular_Mask_From_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_texture_ps"));
                    break;
                case Specular_Mask.Specular_Mask_Mult_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_mult_texture_ps"));
                    break;
                case Specular_Mask.Specular_Mask_From_Color_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_color_texture_ps"));
                    break;
            }

            switch (material_model)
            {
                case Material_Model.Diffuse_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "diffuse_only"));
                    break;
                case Material_Model.Two_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong"));
                    break;
                case Material_Model.Foliage:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "foliage"));
                    break;
                case Material_Model.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "none"));
                    break;
                case Material_Model.Custom_Specular:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "custom_specular"));
                    break;
            }

            switch (environment_mapping)
            {
                case Environment_Mapping.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "none"));
                    break;
                case Environment_Mapping.Per_Pixel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "per_pixel"));
                    break;
                case Environment_Mapping.Dynamic:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "dynamic"));
                    break;
                case Environment_Mapping.From_Flat_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "from_flat_texture"));
                    break;
                case Environment_Mapping.Per_Pixel_Mip:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "per_pixel_mip"));
                    break;
                case Environment_Mapping.Dynamic_Reach:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "dynamic_reach"));
                    break;
            }

            switch (self_illumination)
            {
                case Self_Illumination.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_none_ps"));
                    break;
                case Self_Illumination.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_simple_ps"));
                    break;
                case Self_Illumination._3_Channel_Self_Illum:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_three_channel_ps"));
                    break;
                case Self_Illumination.Plasma:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_plasma_ps"));
                    break;
                case Self_Illumination.From_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_from_albedo_ps"));
                    break;
                case Self_Illumination.Illum_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_detail_ps"));
                    break;
                case Self_Illumination.Meter:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_meter_ps"));
                    break;
                case Self_Illumination.Self_Illum_Times_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_times_diffuse_ps"));
                    break;
                case Self_Illumination.Window_Room:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_window_room_ps"));
                    break;
            }

            switch (blend_mode)
            {
                case Blend_Mode.Opaque:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "opaque"));
                    break;
                case Blend_Mode.Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "additive"));
                    break;
                case Blend_Mode.Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "multiply"));
                    break;
                case Blend_Mode.Alpha_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "alpha_blend"));
                    break;
                case Blend_Mode.Double_Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "double_multiply"));
                    break;
            }

            switch (parallax)
            {
                case Parallax.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_off_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Parallax.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_simple_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
                case Parallax.Interpolated:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_interpolated_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_interpolated_ps"));
                    break;
                case Parallax.Simple_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_simple_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_detail_ps"));
                    break;
            }

            switch (misc)
            {
                case Misc.First_Person_Never:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.First_Person_Sometimes:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.First_Person_Always:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.First_Person_Never_With_Rotating_Bitmaps:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "1"));
                    break;
                case Misc.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.Rotating_Bitmaps_Super_Slow:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "1"));
                    break;
                case Misc.Always_Calc_Albedo:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "2"));
                    break;
            }

            switch (wetness)
            {
                case Wetness.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_default_ps"));
                    break;
                case Wetness.Flood:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_flood_ps"));
                    break;
                case Wetness.Proof:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_proof_ps"));
                    break;
                case Wetness.Ripples:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_ripples_ps"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Custom);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Alpha_Test sAlphaTest = (Shared.Alpha_Test)Enum.Parse(typeof(Shared.Alpha_Test), alpha_test.ToString());
            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());
            Shader.Misc sMisc = (Shader.Misc)Enum.Parse(typeof(Shader.Misc), misc.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Custom, entryPoint,
                sBlendMode, sMisc, sAlphaTest, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes, true, vertexType);

            switch (albedo)
            {
                case Albedo.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_default_ps"));
                    break;
                case Albedo.Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_detail_blend_ps"));
                    break;
                case Albedo.Constant_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_constant_color_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_constant_color_ps"));
                    break;
                case Albedo.Two_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_ps"));
                    break;
                case Albedo.Four_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_four_change_color_ps"));
                    break;
                case Albedo.Three_Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_three_detail_blend_ps"));
                    break;
                case Albedo.Two_Detail_Overlay:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_detail_overlay_ps"));
                    break;
                case Albedo.Two_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_detail_ps"));
                    break;
                case Albedo.Color_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_color_mask_ps"));
                    break;
                case Albedo.Two_Detail_Black_Point:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_detail_black_point_ps"));
                    break;
                case Albedo.Waterfall:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_waterfall_ps"));
                    break;
                case Albedo.Multiply_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_multiply_map_ps"));
                    break;
                case Albedo.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_simple_ps"));
                    break;
            }

            switch (bump_mapping)
            {
                case Bump_Mapping.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_off_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_off_ps"));
                    break;
                case Bump_Mapping.Standard:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_default_ps"));
                    break;
                case Bump_Mapping.Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_ps"));
                    break;
            }

            switch (alpha_test)
            {
                case Alpha_Test.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_test", "off"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", "calc_alpha_test_off_ps"));
                    break;
                case Alpha_Test.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_test", "on"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", "calc_alpha_test_on_ps"));
                    break;
                case Alpha_Test.Multiply_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_test", "multmap"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", "calc_alpha_test_multiply_map_ps"));
                    break;
            }

            switch (specular_mask)
            {
                case Specular_Mask.No_Specular_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_no_specular_mask_ps"));
                    break;
                case Specular_Mask.Specular_Mask_From_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_from_diffuse_ps"));
                    break;
                case Specular_Mask.Specular_Mask_From_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_texture_ps"));
                    break;
                case Specular_Mask.Specular_Mask_Mult_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_mult_texture_ps"));
                    break;
                case Specular_Mask.Specular_Mask_From_Color_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_color_texture_ps"));
                    break;
            }

            switch (material_model)
            {
                case Material_Model.Diffuse_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "diffuse_only"));
                    break;
                case Material_Model.Two_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong"));
                    break;
                case Material_Model.Foliage:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "foliage"));
                    break;
                case Material_Model.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "none"));
                    break;
                case Material_Model.Custom_Specular:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "custom_specular"));
                    break;
            }

            switch (environment_mapping)
            {
                case Environment_Mapping.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "none"));
                    break;
                case Environment_Mapping.Per_Pixel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "per_pixel"));
                    break;
                case Environment_Mapping.Dynamic:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "dynamic"));
                    break;
                case Environment_Mapping.From_Flat_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "from_flat_texture"));
                    break;
                case Environment_Mapping.Per_Pixel_Mip:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "per_pixel_mip"));
                    break;
                case Environment_Mapping.Dynamic_Reach:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "dynamic_reach"));
                    break;
            }

            switch (self_illumination)
            {
                case Self_Illumination.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_none_ps"));
                    break;
                case Self_Illumination.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_simple_ps"));
                    break;
                case Self_Illumination._3_Channel_Self_Illum:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_three_channel_ps"));
                    break;
                case Self_Illumination.Plasma:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_plasma_ps"));
                    break;
                case Self_Illumination.From_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_from_albedo_ps"));
                    break;
                case Self_Illumination.Illum_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_detail_ps"));
                    break;
                case Self_Illumination.Meter:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_meter_ps"));
                    break;
                case Self_Illumination.Self_Illum_Times_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_times_diffuse_ps"));
                    break;
                case Self_Illumination.Window_Room:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_window_room_ps"));
                    break;
            }

            switch (blend_mode)
            {
                case Blend_Mode.Opaque:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "opaque"));
                    break;
                case Blend_Mode.Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "additive"));
                    break;
                case Blend_Mode.Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "multiply"));
                    break;
                case Blend_Mode.Alpha_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "alpha_blend"));
                    break;
                case Blend_Mode.Double_Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "double_multiply"));
                    break;
            }

            switch (parallax)
            {
                case Parallax.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_off_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Parallax.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_simple_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
                case Parallax.Interpolated:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_interpolated_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_interpolated_ps"));
                    break;
                case Parallax.Simple_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", "calc_parallax_simple_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_detail_ps"));
                    break;
            }

            switch (misc)
            {
                case Misc.First_Person_Never:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.First_Person_Sometimes:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.First_Person_Always:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.First_Person_Never_With_Rotating_Bitmaps:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "1"));
                    break;
                case Misc.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
                case Misc.Rotating_Bitmaps_Super_Slow:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "1"));
                    break;
                case Misc.Always_Calc_Albedo:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "2"));
                    break;
            }

            switch (wetness)
            {
                case Wetness.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_default_ps"));
                    break;
                case Wetness.Flood:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_flood_ps"));
                    break;
                case Wetness.Proof:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_proof_ps"));
                    break;
                case Wetness.Ripples:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_ripples_ps"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Custom);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(CustomMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((CustomMethods)methodIndex)
            {
                case CustomMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case CustomMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping)).Length;
                case CustomMethods.Alpha_Test:
                    return Enum.GetValues(typeof(Alpha_Test)).Length;
                case CustomMethods.Specular_Mask:
                    return Enum.GetValues(typeof(Specular_Mask)).Length;
                case CustomMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model)).Length;
                case CustomMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping)).Length;
                case CustomMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination)).Length;
                case CustomMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
                case CustomMethods.Parallax:
                    return Enum.GetValues(typeof(Parallax)).Length;
                case CustomMethods.Misc:
                    return Enum.GetValues(typeof(Misc)).Length;
                case CustomMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness)).Length;
            }

            return -1;
        }

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((CustomMethods)methodIndex)
            {
                case CustomMethods.Albedo:
                    return (int)albedo;
                case CustomMethods.Bump_Mapping:
                    return (int)bump_mapping;
                case CustomMethods.Alpha_Test:
                    return (int)alpha_test;
                case CustomMethods.Specular_Mask:
                    return (int)specular_mask;
                case CustomMethods.Material_Model:
                    return (int)material_model;
                case CustomMethods.Environment_Mapping:
                    return (int)environment_mapping;
                case CustomMethods.Self_Illumination:
                    return (int)self_illumination;
                case CustomMethods.Blend_Mode:
                    return (int)blend_mode;
                case CustomMethods.Parallax:
                    return (int)parallax;
                case CustomMethods.Misc:
                    return (int)misc;
                case CustomMethods.Wetness:
                    return (int)wetness;
            }

            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Albedo:
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Sh:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Dynamic_Light:
                case ShaderStage.Shadow_Generate:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Static_Per_Vertex_Color:
                case ShaderStage.Dynamic_Light_Cinematic:
                case ShaderStage.Stipple:
                //case ShaderStage.Single_Pass_Per_Pixel:
                //case ShaderStage.Single_Pass_Per_Vertex:
                //case ShaderStage.Single_Pass_Single_Probe:
                //case ShaderStage.Single_Pass_Single_Probe_Ambient:
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
                case 2:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Shadow_Generate:
                    return true;
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
                case ShaderStage.Shadow_Generate:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsVertexFormatSupported(VertexType vertexType)
        {
            switch (vertexType)
            {
                case VertexType.World:
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
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Static_Sh:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Dynamic_Light:
                case ShaderStage.Shadow_Generate:
                case ShaderStage.Static_Per_Vertex_Color:
                case ShaderStage.Dynamic_Light_Cinematic:
                case ShaderStage.Z_Only:
                    return true;
                default:
                    return false;
            }
        }

        public ShaderParameters GetPixelShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;
            var result = new ShaderParameters();

            switch (albedo)
            {
                case Albedo.Default:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddFloat4ColorParameter("albedo_color");
                    break;
                case Albedo.Detail_Blend:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddSamplerWithoutXFormParameter("detail_map2");
                    result.AddFloatParameter("blend_alpha");
                    break;
                case Albedo.Constant_Color:
                    result.AddFloat4ColorParameter("albedo_color");
                    break;
                case Albedo.Two_Change_Color:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddSamplerWithoutXFormParameter("change_color_map");
                    result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                    result.AddSamplerWithoutXFormParameter("camouflage_change_color_map");
                    result.AddFloatParameter("camouflage_scale");
                    break;
                case Albedo.Four_Change_Color:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddSamplerWithoutXFormParameter("change_color_map");
                    result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                    result.AddFloat3ColorParameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary);
                    result.AddFloat3ColorParameter("quaternary_change_color", RenderMethodExtern.object_change_color_quaternary);
                    result.AddSamplerWithoutXFormParameter("camouflage_change_color_map");
                    result.AddFloatParameter("camouflage_scale");
                    break;
                case Albedo.Three_Detail_Blend:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddSamplerWithoutXFormParameter("detail_map2");
                    result.AddSamplerWithoutXFormParameter("detail_map3");
                    result.AddFloatParameter("blend_alpha");
                    break;
                case Albedo.Two_Detail_Overlay:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
                    result.AddSamplerParameter("detail_map_overlay");
                    break;
                case Albedo.Two_Detail:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddSamplerWithoutXFormParameter("detail_map2");
                    break;
                case Albedo.Color_Mask:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddSamplerWithoutXFormParameter("color_mask_map");
                    result.AddFloat4ColorParameter("albedo_color");
                    result.AddFloat4ColorParameter("albedo_color2");
                    result.AddFloat4ColorParameter("albedo_color3");
                    result.AddFloat3ColorParameter("neutral_gray");
                    break;
                case Albedo.Two_Detail_Black_Point:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddSamplerWithoutXFormParameter("detail_map2");
                    break;
                case Albedo.Waterfall:
                    result.AddSamplerParameter("waterfall_base_mask");
                    result.AddSamplerParameter("waterfall_layer0");
                    result.AddSamplerParameter("waterfall_layer1");
                    result.AddSamplerParameter("waterfall_layer2");
                    result.AddFloatParameter("transparency_frothy_weight");
                    result.AddFloatParameter("transparency_base_weight");
                    result.AddFloatParameter("transparency_bias");
                    break;
                case Albedo.Multiply_Map:
                    break;
                case Albedo.Simple:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddFloat4ColorParameter("albedo_color");
                    break;
            }

            switch (bump_mapping)
            {
                case Bump_Mapping.Off:
                    break;
                case Bump_Mapping.Standard:
                    result.AddSamplerParameter("bump_map");
                    break;
                case Bump_Mapping.Detail:
                    result.AddSamplerParameter("bump_map");
                    result.AddSamplerParameter("bump_detail_map");
                    result.AddFloatParameter("bump_detail_coefficient");
                    break;
            }

            switch (alpha_test)
            {
                case Alpha_Test.None:
                    break;
                case Alpha_Test.Simple:
                    result.AddSamplerWithoutXFormParameter("alpha_test_map");
                    break;
                case Alpha_Test.Multiply_Map:
                    result.AddSamplerParameter("alpha_test_map");
                    result.AddSamplerParameter("multiply_map");
                    break;
            }

            switch (specular_mask)
            {
                case Specular_Mask.No_Specular_Mask:
                    break;
                case Specular_Mask.Specular_Mask_From_Diffuse:
                    break;
                case Specular_Mask.Specular_Mask_From_Texture:
                    result.AddSamplerParameter("specular_mask_texture");
                    break;
                case Specular_Mask.Specular_Mask_Mult_Diffuse:
                    result.AddSamplerWithoutXFormParameter("specular_mask_texture");
                    break;
                case Specular_Mask.Specular_Mask_From_Color_Texture:
                    break;
            }

            switch (material_model)
            {
                case Material_Model.Diffuse_Only:
                    break;
                case Material_Model.Two_Lobe_Phong:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloatParameter("normal_specular_power");
                    result.AddFloat3ColorParameter("normal_specular_tint");
                    result.AddFloatParameter("glancing_specular_power");
                    result.AddFloat3ColorParameter("glancing_specular_tint");
                    result.AddFloatParameter("fresnel_curve_steepness");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddBooleanParameter("order3_area_specular");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddFloatParameter("albedo_specular_tint_blend");
                    result.AddFloatParameter("analytical_anti_shadow_control");
                    result.AddFloat3ColorParameter("specular_color_by_angle");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("analytical_roughness");
                    result.AddFloatParameter("approximate_specular_type");
                    result.AddFloatParameter("analytical_power");
                    break;
                case Material_Model.Foliage:
                    break;
                case Material_Model.None:
                    break;
                case Material_Model.Custom_Specular:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddSamplerWithoutXFormParameter("specular_lobe");
                    result.AddSamplerWithoutXFormParameter("glancing_falloff");
                    result.AddSamplerParameter("material_map");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    break;
            }

            switch (environment_mapping)
            {
                case Environment_Mapping.None:
                    break;
                case Environment_Mapping.Per_Pixel:
                    result.AddSamplerWithoutXFormParameter("environment_map");
                    result.AddFloat3ColorParameter("env_tint_color");
                    result.AddFloatParameter("env_roughness_offset");
                    result.AddFloatParameter("env_roughness_scale");
                    break;
                case Environment_Mapping.Dynamic:
                    result.AddFloat3ColorParameter("env_tint_color");
                    result.AddSamplerWithoutXFormParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0);
                    result.AddSamplerWithoutXFormParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1);
                    result.AddFloatParameter("env_roughness_scale");
                    result.AddFloatParameter("env_roughness_offset");
                    break;
                case Environment_Mapping.From_Flat_Texture:
                    result.AddSamplerWithoutXFormParameter("flat_environment_map");
                    result.AddFloat3ColorParameter("env_tint_color");
                    result.AddFloat4ColorParameter("flat_envmap_matrix_x", RenderMethodExtern.flat_envmap_matrix_x);
                    result.AddFloat4ColorParameter("flat_envmap_matrix_y", RenderMethodExtern.flat_envmap_matrix_y);
                    result.AddFloat4ColorParameter("flat_envmap_matrix_z", RenderMethodExtern.flat_envmap_matrix_z);
                    result.AddFloatParameter("hemisphere_percentage");
                    result.AddFloat4ColorParameter("env_bloom_override");
                    result.AddFloatParameter("env_bloom_override_intensity");
                    break;
                case Environment_Mapping.Per_Pixel_Mip:
                    result.AddSamplerWithoutXFormParameter("environment_map");
                    result.AddFloat3ColorParameter("env_tint_color");
                    result.AddFloatParameter("env_roughness_offset");
                    result.AddFloatParameter("env_roughness_scale");
                    break;
                case Environment_Mapping.Dynamic_Reach:
                    result.AddFloat3ColorParameter("env_tint_color");
                    result.AddSamplerWithoutXFormParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0);
                    result.AddSamplerWithoutXFormParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1);
                    result.AddFloatParameter("env_roughness_scale");
                    break;
            }

            switch (self_illumination)
            {
                case Self_Illumination.Off:
                    break;
                case Self_Illumination.Simple:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat3ColorParameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination._3_Channel_Self_Illum:
                    result.AddSamplerWithoutXFormParameter("self_illum_map");
                    result.AddFloat4ColorParameter("channel_a");
                    result.AddFloat4ColorParameter("channel_b");
                    result.AddFloat4ColorParameter("channel_c");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination.Plasma:
                    result.AddSamplerWithoutXFormParameter("noise_map_a");
                    result.AddSamplerWithoutXFormParameter("noise_map_b");
                    result.AddFloat4ColorParameter("color_medium");
                    result.AddFloat4ColorParameter("color_wide");
                    result.AddFloat4ColorParameter("color_sharp");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddSamplerWithoutXFormParameter("alpha_mask_map");
                    result.AddFloatParameter("thinness_medium");
                    result.AddFloatParameter("thinness_wide");
                    result.AddFloatParameter("thinness_sharp");
                    break;
                case Self_Illumination.From_Diffuse:
                    result.AddFloat3ColorParameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination.Illum_Detail:
                    result.AddSamplerWithoutXFormParameter("self_illum_map");
                    result.AddSamplerWithoutXFormParameter("self_illum_detail_map");
                    result.AddFloat3ColorParameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination.Meter:
                    result.AddSamplerWithoutXFormParameter("meter_map");
                    result.AddFloat3ColorParameter("meter_color_off");
                    result.AddFloat3ColorParameter("meter_color_on");
                    result.AddFloatParameter("meter_value");
                    break;
                case Self_Illumination.Self_Illum_Times_Diffuse:
                    result.AddSamplerWithoutXFormParameter("self_illum_map");
                    result.AddFloat3ColorParameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("primary_change_color_blend");
                    break;
                case Self_Illumination.Window_Room:
                    result.AddSamplerParameter("opacity_map");
                    result.AddSamplerParameter("ceiling");
                    result.AddSamplerParameter("walls");
                    result.AddSamplerParameter("floors");
                    result.AddSamplerParameter("transform");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddSamplerWithoutXFormParameter("window_property_map");
                    result.AddFloatParameter("distance_fade_scale");
                    break;
            }

            switch (blend_mode)
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
            }

            switch (parallax)
            {
                case Parallax.Off:
                    break;
                case Parallax.Simple:
                    result.AddSamplerWithoutXFormParameter("height_map");
                    result.AddFloatParameter("height_scale");
                    break;
                case Parallax.Interpolated:
                    result.AddSamplerWithoutXFormParameter("height_map");
                    result.AddFloatParameter("height_scale");
                    break;
                case Parallax.Simple_Detail:
                    result.AddSamplerWithoutXFormParameter("height_map");
                    result.AddFloatParameter("height_scale");
                    result.AddSamplerWithoutXFormParameter("height_scale_map");
                    break;
            }

            switch (misc)
            {
                case Misc.First_Person_Never:
                    break;
                case Misc.First_Person_Sometimes:
                    break;
                case Misc.First_Person_Always:
                    break;
                case Misc.First_Person_Never_With_Rotating_Bitmaps:
                    break;
                case Misc.Default:
                    break;
                case Misc.Rotating_Bitmaps_Super_Slow:
                    break;
                case Misc.Always_Calc_Albedo:
                    break;
            }

            switch (wetness)
            {
                case Wetness.Default:
                    result.AddFloatParameter("wet_material_dim_coefficient");
                    result.AddFloat3ColorParameter("wet_material_dim_tint");
                    break;
                case Wetness.Flood:
                    result.AddFloatParameter("wet_material_dim_coefficient");
                    result.AddFloat3ColorParameter("wet_material_dim_tint");
                    result.AddFloatParameter("wet_sheen_reflection_contribution");
                    result.AddFloat3ColorParameter("wet_sheen_reflection_tint");
                    result.AddFloatParameter("wet_sheen_thickness");
                    result.AddSamplerWithoutXFormParameter("wet_flood_slope_map");
                    result.AddSamplerWithoutXFormParameter("wet_noise_boundary_map");
                    result.AddFloatParameter("specular_mask_tweak_weight");
                    result.AddFloatParameter("surface_tilt_tweak_weight");
                    break;
                case Wetness.Proof:
                    break;
                case Wetness.Ripples:
                    result.AddFloatParameter("wet_material_dim_coefficient");
                    result.AddFloat3ColorParameter("wet_material_dim_tint");
                    result.AddFloatParameter("wet_sheen_reflection_contribution");
                    result.AddFloat3ColorParameter("wet_sheen_reflection_tint");
                    result.AddFloatParameter("wet_sheen_thickness");
                    result.AddSamplerWithoutXFormParameter("wet_noise_boundary_map");
                    result.AddFloatParameter("specular_mask_tweak_weight");
                    result.AddFloatParameter("surface_tilt_tweak_weight");
                    break;
            }

            return result;
        }

        public ShaderParameters GetVertexShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;
            var result = new ShaderParameters();

            switch (albedo)
            {
                case Albedo.Default:
                    break;
                case Albedo.Detail_Blend:
                    break;
                case Albedo.Constant_Color:
                    break;
                case Albedo.Two_Change_Color:
                    break;
                case Albedo.Four_Change_Color:
                    break;
                case Albedo.Three_Detail_Blend:
                    break;
                case Albedo.Two_Detail_Overlay:
                    break;
                case Albedo.Two_Detail:
                    break;
                case Albedo.Color_Mask:
                    break;
                case Albedo.Two_Detail_Black_Point:
                    break;
                case Albedo.Waterfall:
                    break;
                case Albedo.Multiply_Map:
                    break;
                case Albedo.Simple:
                    break;
            }

            switch (bump_mapping)
            {
                case Bump_Mapping.Off:
                    break;
                case Bump_Mapping.Standard:
                    break;
                case Bump_Mapping.Detail:
                    break;
            }

            switch (alpha_test)
            {
                case Alpha_Test.None:
                    break;
                case Alpha_Test.Simple:
                    break;
                case Alpha_Test.Multiply_Map:
                    break;
            }

            switch (specular_mask)
            {
                case Specular_Mask.No_Specular_Mask:
                    break;
                case Specular_Mask.Specular_Mask_From_Diffuse:
                    break;
                case Specular_Mask.Specular_Mask_From_Texture:
                    break;
                case Specular_Mask.Specular_Mask_Mult_Diffuse:
                    break;
                case Specular_Mask.Specular_Mask_From_Color_Texture:
                    break;
            }

            switch (material_model)
            {
                case Material_Model.Diffuse_Only:
                    break;
                case Material_Model.Two_Lobe_Phong:
                    break;
                case Material_Model.Foliage:
                    break;
                case Material_Model.None:
                    break;
                case Material_Model.Custom_Specular:
                    break;
            }

            switch (environment_mapping)
            {
                case Environment_Mapping.None:
                    break;
                case Environment_Mapping.Per_Pixel:
                    break;
                case Environment_Mapping.Dynamic:
                    break;
                case Environment_Mapping.From_Flat_Texture:
                    break;
                case Environment_Mapping.Per_Pixel_Mip:
                    break;
                case Environment_Mapping.Dynamic_Reach:
                    break;
            }

            switch (self_illumination)
            {
                case Self_Illumination.Off:
                    break;
                case Self_Illumination.Simple:
                    break;
                case Self_Illumination._3_Channel_Self_Illum:
                    break;
                case Self_Illumination.Plasma:
                    break;
                case Self_Illumination.From_Diffuse:
                    break;
                case Self_Illumination.Illum_Detail:
                    break;
                case Self_Illumination.Meter:
                    break;
                case Self_Illumination.Self_Illum_Times_Diffuse:
                    break;
                case Self_Illumination.Window_Room:
                    break;
            }

            switch (blend_mode)
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
            }

            switch (parallax)
            {
                case Parallax.Off:
                    break;
                case Parallax.Simple:
                    break;
                case Parallax.Interpolated:
                    break;
                case Parallax.Simple_Detail:
                    break;
            }

            switch (misc)
            {
                case Misc.First_Person_Never:
                    break;
                case Misc.First_Person_Sometimes:
                    break;
                case Misc.First_Person_Always:
                    break;
                case Misc.First_Person_Never_With_Rotating_Bitmaps:
                    break;
                case Misc.Default:
                    break;
                case Misc.Rotating_Bitmaps_Super_Slow:
                    break;
                case Misc.Always_Calc_Albedo:
                    break;
            }

            switch (wetness)
            {
                case Wetness.Default:
                    break;
                case Wetness.Flood:
                    break;
                case Wetness.Proof:
                    break;
                case Wetness.Ripples:
                    break;
            }

            return result;
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();
            result.AddSamplerWithoutXFormParameter("albedo_texture", RenderMethodExtern.texture_global_target_texaccum);
            result.AddSamplerWithoutXFormParameter("normal_texture", RenderMethodExtern.texture_global_target_normal);
            result.AddSamplerWithoutXFormParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture);
            result.AddSamplerWithoutXFormParameter("shadow_depth_map_1", RenderMethodExtern.texture_global_target_shadow_buffer1);
            result.AddSamplerWithoutXFormParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0);
            result.AddFloat3ColorParameter("debug_tint", RenderMethodExtern.debug_tint);
            result.AddSamplerWithoutXFormParameter("active_camo_distortion_texture", RenderMethodExtern.active_camo_distortion_texture);
            result.AddSamplerWithoutXFormParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerWithoutXFormParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture);
            result.AddSamplerWithoutXFormParameter("dominant_light_intensity_map", RenderMethodExtern.texture_dominant_light_intensity_map);
            result.AddSamplerWithoutXFormParameter("g_sample_vmf_phong_specular");
            result.AddSamplerWithoutXFormParameter("g_direction_lut");
            result.AddSamplerWithoutXFormParameter("g_sample_vmf_diffuse");
            result.AddSamplerWithoutXFormParameter("g_diffuse_power_specular");
            result.AddSamplerWithoutXFormParameter("shadow_mask_texture", RenderMethodExtern.none);
            result.AddSamplerWithoutXFormParameter("g_sample_vmf_diffuse_vs");
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
                    case Albedo.Default:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddFloat4ColorParameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_default";
                        break;
                    case Albedo.Detail_Blend:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddSamplerWithoutXFormParameter("detail_map2");
                        result.AddFloatParameter("blend_alpha");
                        rmopName = @"shaders\shader_options\albedo_detail_blend";
                        break;
                    case Albedo.Constant_Color:
                        result.AddFloat4ColorParameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_constant";
                        break;
                    case Albedo.Two_Change_Color:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddSamplerWithoutXFormParameter("change_color_map");
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                        result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                        result.AddSamplerWithoutXFormParameter("camouflage_change_color_map");
                        result.AddFloatParameter("camouflage_scale");
                        rmopName = @"shaders\shader_options\albedo_two_change_color";
                        break;
                    case Albedo.Four_Change_Color:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddSamplerWithoutXFormParameter("change_color_map");
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                        result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                        result.AddFloat3ColorParameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary);
                        result.AddFloat3ColorParameter("quaternary_change_color", RenderMethodExtern.object_change_color_quaternary);
                        result.AddSamplerWithoutXFormParameter("camouflage_change_color_map");
                        result.AddFloatParameter("camouflage_scale");
                        rmopName = @"shaders\shader_options\albedo_four_change_color";
                        break;
                    case Albedo.Three_Detail_Blend:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddSamplerWithoutXFormParameter("detail_map2");
                        result.AddSamplerWithoutXFormParameter("detail_map3");
                        result.AddFloatParameter("blend_alpha");
                        rmopName = @"shaders\shader_options\albedo_three_detail_blend";
                        break;
                    case Albedo.Two_Detail_Overlay:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddSamplerParameter("detail_map2");
                        result.AddSamplerParameter("detail_map_overlay");
                        rmopName = @"shaders\shader_options\albedo_two_detail_overlay";
                        break;
                    case Albedo.Two_Detail:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddSamplerWithoutXFormParameter("detail_map2");
                        rmopName = @"shaders\shader_options\albedo_two_detail";
                        break;
                    case Albedo.Color_Mask:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddSamplerWithoutXFormParameter("color_mask_map");
                        result.AddFloat4ColorParameter("albedo_color");
                        result.AddFloat4ColorParameter("albedo_color2");
                        result.AddFloat4ColorParameter("albedo_color3");
                        result.AddFloat3ColorParameter("neutral_gray");
                        rmopName = @"shaders\shader_options\albedo_color_mask";
                        break;
                    case Albedo.Two_Detail_Black_Point:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddSamplerWithoutXFormParameter("detail_map2");
                        rmopName = @"shaders\shader_options\albedo_two_detail_black_point";
                        break;
                    case Albedo.Waterfall:
                        result.AddSamplerParameter("waterfall_base_mask");
                        result.AddSamplerParameter("waterfall_layer0");
                        result.AddSamplerParameter("waterfall_layer1");
                        result.AddSamplerParameter("waterfall_layer2");
                        result.AddFloatParameter("transparency_frothy_weight");
                        result.AddFloatParameter("transparency_base_weight");
                        result.AddFloatParameter("transparency_bias");
                        rmopName = @"shaders\custom_options\albedo_waterfall";
                        break;
                    case Albedo.Multiply_Map:
                        break;
                    case Albedo.Simple:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddFloat4ColorParameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_simple";
                        break;
                }
            }

            if (methodName == "bump_mapping")
            {
                optionName = ((Bump_Mapping)option).ToString();

                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Off:
                        rmopName = @"shaders\shader_options\bump_off";
                        break;
                    case Bump_Mapping.Standard:
                        result.AddSamplerParameter("bump_map");
                        rmopName = @"shaders\shader_options\bump_default";
                        break;
                    case Bump_Mapping.Detail:
                        result.AddSamplerParameter("bump_map");
                        result.AddSamplerParameter("bump_detail_map");
                        result.AddFloatParameter("bump_detail_coefficient");
                        rmopName = @"shaders\shader_options\bump_detail";
                        break;
                }
            }

            if (methodName == "alpha_test")
            {
                optionName = ((Alpha_Test)option).ToString();

                switch ((Alpha_Test)option)
                {
                    case Alpha_Test.None:
                        rmopName = @"shaders\shader_options\alpha_test_off";
                        break;
                    case Alpha_Test.Simple:
                        result.AddSamplerWithoutXFormParameter("alpha_test_map");
                        rmopName = @"shaders\shader_options\alpha_test_on";
                        break;
                    case Alpha_Test.Multiply_Map:
                        result.AddSamplerParameter("alpha_test_map");
                        result.AddSamplerParameter("multiply_map");
                        rmopName = @"shaders\custom_options\alpha_test_multiply_map";
                        break;
                }
            }

            if (methodName == "specular_mask")
            {
                optionName = ((Specular_Mask)option).ToString();

                switch ((Specular_Mask)option)
                {
                    case Specular_Mask.No_Specular_Mask:
                        break;
                    case Specular_Mask.Specular_Mask_From_Diffuse:
                        break;
                    case Specular_Mask.Specular_Mask_From_Texture:
                        result.AddSamplerParameter("specular_mask_texture");
                        rmopName = @"shaders\shader_options\specular_mask_from_texture";
                        break;
                    case Specular_Mask.Specular_Mask_Mult_Diffuse:
                        result.AddSamplerWithoutXFormParameter("specular_mask_texture");
                        rmopName = @"shaders\shader_options\specular_mask_mult_diffuse";
                        break;
                    case Specular_Mask.Specular_Mask_From_Color_Texture:
                        break;
                }
            }

            if (methodName == "material_model")
            {
                optionName = ((Material_Model)option).ToString();

                switch ((Material_Model)option)
                {
                    case Material_Model.Diffuse_Only:
                        break;
                    case Material_Model.Two_Lobe_Phong:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloatParameter("normal_specular_power");
                        result.AddFloat3ColorParameter("normal_specular_tint");
                        result.AddFloatParameter("glancing_specular_power");
                        result.AddFloat3ColorParameter("glancing_specular_tint");
                        result.AddFloatParameter("fresnel_curve_steepness");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("albedo_specular_tint_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddFloat3ColorParameter("specular_color_by_angle");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("analytical_roughness");
                        result.AddFloatParameter("approximate_specular_type");
                        result.AddFloatParameter("analytical_power");
                        rmopName = @"shaders\shader_options\material_two_lobe_phong_option";
                        break;
                    case Material_Model.Foliage:
                        break;
                    case Material_Model.None:
                        break;
                    case Material_Model.Custom_Specular:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddSamplerWithoutXFormParameter("specular_lobe");
                        result.AddSamplerWithoutXFormParameter("glancing_falloff");
                        result.AddSamplerParameter("material_map");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution");
                        rmopName = @"shaders\custom_options\material_custom_specular";
                        break;
                }
            }

            if (methodName == "environment_mapping")
            {
                optionName = ((Environment_Mapping)option).ToString();

                switch ((Environment_Mapping)option)
                {
                    case Environment_Mapping.None:
                        break;
                    case Environment_Mapping.Per_Pixel:
                        result.AddSamplerWithoutXFormParameter("environment_map");
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddFloatParameter("env_roughness_offset");
                        result.AddFloatParameter("env_roughness_scale");
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.Dynamic:
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddSamplerWithoutXFormParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0);
                        result.AddSamplerWithoutXFormParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1);
                        result.AddFloatParameter("env_roughness_scale");
                        result.AddFloatParameter("env_roughness_offset");
                        rmopName = @"shaders\shader_options\env_map_dynamic";
                        break;
                    case Environment_Mapping.From_Flat_Texture:
                        result.AddSamplerWithoutXFormParameter("flat_environment_map");
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddFloat4ColorParameter("flat_envmap_matrix_x", RenderMethodExtern.flat_envmap_matrix_x);
                        result.AddFloat4ColorParameter("flat_envmap_matrix_y", RenderMethodExtern.flat_envmap_matrix_y);
                        result.AddFloat4ColorParameter("flat_envmap_matrix_z", RenderMethodExtern.flat_envmap_matrix_z);
                        result.AddFloatParameter("hemisphere_percentage");
                        result.AddFloat4ColorParameter("env_bloom_override");
                        result.AddFloatParameter("env_bloom_override_intensity");
                        rmopName = @"shaders\shader_options\env_map_from_flat_texture";
                        break;
                    case Environment_Mapping.Per_Pixel_Mip:
                        result.AddSamplerWithoutXFormParameter("environment_map");
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddFloatParameter("env_roughness_offset");
                        result.AddFloatParameter("env_roughness_scale");
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.Dynamic_Reach:
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddSamplerWithoutXFormParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0);
                        result.AddSamplerWithoutXFormParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1);
                        result.AddFloatParameter("env_roughness_scale");
                        rmopName = @"shaders\shader_options\env_map_dynamic";
                        break;
                }
            }

            if (methodName == "self_illumination")
            {
                optionName = ((Self_Illumination)option).ToString();

                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.Off:
                        break;
                    case Self_Illumination.Simple:
                        result.AddSamplerParameter("self_illum_map");
                        result.AddFloat3ColorParameter("self_illum_color");
                        result.AddFloatParameter("self_illum_intensity");
                        rmopName = @"shaders\shader_options\illum_simple";
                        break;
                    case Self_Illumination._3_Channel_Self_Illum:
                        result.AddSamplerWithoutXFormParameter("self_illum_map");
                        result.AddFloat4ColorParameter("channel_a");
                        result.AddFloat4ColorParameter("channel_b");
                        result.AddFloat4ColorParameter("channel_c");
                        result.AddFloatParameter("self_illum_intensity");
                        rmopName = @"shaders\shader_options\illum_3_channel";
                        break;
                    case Self_Illumination.Plasma:
                        result.AddSamplerWithoutXFormParameter("noise_map_a");
                        result.AddSamplerWithoutXFormParameter("noise_map_b");
                        result.AddFloat4ColorParameter("color_medium");
                        result.AddFloat4ColorParameter("color_wide");
                        result.AddFloat4ColorParameter("color_sharp");
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddSamplerWithoutXFormParameter("alpha_mask_map");
                        result.AddFloatParameter("thinness_medium");
                        result.AddFloatParameter("thinness_wide");
                        result.AddFloatParameter("thinness_sharp");
                        rmopName = @"shaders\shader_options\illum_plasma";
                        break;
                    case Self_Illumination.From_Diffuse:
                        result.AddFloat3ColorParameter("self_illum_color");
                        result.AddFloatParameter("self_illum_intensity");
                        rmopName = @"shaders\shader_options\illum_from_diffuse";
                        break;
                    case Self_Illumination.Illum_Detail:
                        result.AddSamplerWithoutXFormParameter("self_illum_map");
                        result.AddSamplerWithoutXFormParameter("self_illum_detail_map");
                        result.AddFloat3ColorParameter("self_illum_color");
                        result.AddFloatParameter("self_illum_intensity");
                        rmopName = @"shaders\shader_options\illum_detail";
                        break;
                    case Self_Illumination.Meter:
                        result.AddSamplerWithoutXFormParameter("meter_map");
                        result.AddFloat3ColorParameter("meter_color_off");
                        result.AddFloat3ColorParameter("meter_color_on");
                        result.AddFloatParameter("meter_value");
                        rmopName = @"shaders\shader_options\illum_meter";
                        break;
                    case Self_Illumination.Self_Illum_Times_Diffuse:
                        result.AddSamplerWithoutXFormParameter("self_illum_map");
                        result.AddFloat3ColorParameter("self_illum_color");
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddFloatParameter("primary_change_color_blend");
                        rmopName = @"shaders\shader_options\illum_times_diffuse";
                        break;
                    case Self_Illumination.Window_Room:
                        result.AddSamplerParameter("opacity_map");
                        result.AddSamplerParameter("ceiling");
                        result.AddSamplerParameter("walls");
                        result.AddSamplerParameter("floors");
                        result.AddSamplerParameter("transform");
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddSamplerWithoutXFormParameter("window_property_map");
                        result.AddFloatParameter("distance_fade_scale");
                        rmopName = @"shaders\custom_options\window_room_map";
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
                        result.AddSamplerWithoutXFormParameter("height_map");
                        result.AddFloatParameter("height_scale");
                        rmopName = @"shaders\shader_options\parallax_simple";
                        break;
                    case Parallax.Interpolated:
                        result.AddSamplerWithoutXFormParameter("height_map");
                        result.AddFloatParameter("height_scale");
                        rmopName = @"shaders\shader_options\parallax_simple";
                        break;
                    case Parallax.Simple_Detail:
                        result.AddSamplerWithoutXFormParameter("height_map");
                        result.AddFloatParameter("height_scale");
                        result.AddSamplerWithoutXFormParameter("height_scale_map");
                        rmopName = @"shaders\shader_options\parallax_detail";
                        break;
                }
            }

            if (methodName == "misc")
            {
                optionName = ((Misc)option).ToString();

                switch ((Misc)option)
                {
                    case Misc.First_Person_Never:
                        break;
                    case Misc.First_Person_Sometimes:
                        break;
                    case Misc.First_Person_Always:
                        break;
                    case Misc.First_Person_Never_With_Rotating_Bitmaps:
                        break;
                    case Misc.Default:
                        break;
                    case Misc.Rotating_Bitmaps_Super_Slow:
                        break;
                    case Misc.Always_Calc_Albedo:
                        break;
                }
            }

            if (methodName == "wetness")
            {
                optionName = ((Wetness)option).ToString();

                switch ((Wetness)option)
                {
                    case Wetness.Default:
                        result.AddFloatParameter("wet_material_dim_coefficient");
                        result.AddFloat3ColorParameter("wet_material_dim_tint");
                        rmopName = @"shaders\wetness_options\wetness_simple";
                        break;
                    case Wetness.Flood:
                        result.AddFloatParameter("wet_material_dim_coefficient");
                        result.AddFloat3ColorParameter("wet_material_dim_tint");
                        result.AddFloatParameter("wet_sheen_reflection_contribution");
                        result.AddFloat3ColorParameter("wet_sheen_reflection_tint");
                        result.AddFloatParameter("wet_sheen_thickness");
                        result.AddSamplerWithoutXFormParameter("wet_flood_slope_map");
                        result.AddSamplerWithoutXFormParameter("wet_noise_boundary_map");
                        result.AddFloatParameter("specular_mask_tweak_weight");
                        result.AddFloatParameter("surface_tilt_tweak_weight");
                        rmopName = @"shaders\wetness_options\wetness_flood";
                        break;
                    case Wetness.Proof:
                        break;
                    case Wetness.Ripples:
                        result.AddFloatParameter("wet_material_dim_coefficient");
                        result.AddFloat3ColorParameter("wet_material_dim_tint");
                        result.AddFloatParameter("wet_sheen_reflection_contribution");
                        result.AddFloat3ColorParameter("wet_sheen_reflection_tint");
                        result.AddFloatParameter("wet_sheen_thickness");
                        result.AddSamplerWithoutXFormParameter("wet_noise_boundary_map");
                        result.AddFloatParameter("specular_mask_tweak_weight");
                        result.AddFloatParameter("surface_tilt_tweak_weight");
                        rmopName = @"shaders\wetness_options\wetness_ripples";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(CustomMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((CustomMethods)methodIndex)
            {
                case CustomMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case CustomMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping));
                case CustomMethods.Alpha_Test:
                    return Enum.GetValues(typeof(Alpha_Test));
                case CustomMethods.Specular_Mask:
                    return Enum.GetValues(typeof(Specular_Mask));
                case CustomMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model));
                case CustomMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping));
                case CustomMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination));
                case CustomMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
                case CustomMethods.Parallax:
                    return Enum.GetValues(typeof(Parallax));
                case CustomMethods.Misc:
                    return Enum.GetValues(typeof(Misc));
                case CustomMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness));
            }

            return null;
        }

        public byte[] ValidateOptions(byte[] options)
        {
            List<byte> optionList = new List<byte>(options);

            while (optionList.Count < GetMethodCount())
                optionList.Add(0);

            return optionList.ToArray();
        }

        public void GetCategoryFunctions(string methodName, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "albedo")
            {
                vertexFunction = "calc_albedo_vs";
                pixelFunction = "calc_albedo_ps";
            }

            if (methodName == "bump_mapping")
            {
                vertexFunction = "calc_bumpmap_vs";
                pixelFunction = "calc_bumpmap_ps";
            }

            if (methodName == "alpha_test")
            {
                vertexFunction = "alpha_test";
                pixelFunction = "calc_alpha_test_ps";
            }

            if (methodName == "specular_mask")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_specular_mask_ps";
            }

            if (methodName == "material_model")
            {
                vertexFunction = "invalid";
                pixelFunction = "material_type";
            }

            if (methodName == "environment_mapping")
            {
                vertexFunction = "invalid";
                pixelFunction = "envmap_type";
            }

            if (methodName == "self_illumination")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_self_illumination_ps";
            }

            if (methodName == "blend_mode")
            {
                vertexFunction = "invalid";
                pixelFunction = "blend_type";
            }

            if (methodName == "parallax")
            {
                vertexFunction = "calc_parallax_vs";
                pixelFunction = "calc_parallax_ps";
            }

            if (methodName == "misc")
            {
                vertexFunction = "invalid";
                pixelFunction = "bitmap_rotation";
            }

            if (methodName == "wetness")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_wetness_ps";
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
                    case Albedo.Default:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_default_ps";
                        break;
                    case Albedo.Detail_Blend:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_detail_blend_ps";
                        break;
                    case Albedo.Constant_Color:
                        vertexFunction = "calc_albedo_constant_color_vs";
                        pixelFunction = "calc_albedo_constant_color_ps";
                        break;
                    case Albedo.Two_Change_Color:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_change_color_ps";
                        break;
                    case Albedo.Four_Change_Color:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_four_change_color_ps";
                        break;
                    case Albedo.Three_Detail_Blend:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_three_detail_blend_ps";
                        break;
                    case Albedo.Two_Detail_Overlay:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_detail_overlay_ps";
                        break;
                    case Albedo.Two_Detail:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_detail_ps";
                        break;
                    case Albedo.Color_Mask:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_color_mask_ps";
                        break;
                    case Albedo.Two_Detail_Black_Point:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_detail_black_point_ps";
                        break;
                    case Albedo.Waterfall:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_waterfall_ps";
                        break;
                    case Albedo.Multiply_Map:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_multiply_map_ps";
                        break;
                    case Albedo.Simple:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_simple_ps";
                        break;
                }
            }

            if (methodName == "bump_mapping")
            {
                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Off:
                        vertexFunction = "calc_bumpmap_off_vs";
                        pixelFunction = "calc_bumpmap_off_ps";
                        break;
                    case Bump_Mapping.Standard:
                        vertexFunction = "calc_bumpmap_default_vs";
                        pixelFunction = "calc_bumpmap_default_ps";
                        break;
                    case Bump_Mapping.Detail:
                        vertexFunction = "calc_bumpmap_detail_vs";
                        pixelFunction = "calc_bumpmap_detail_ps";
                        break;
                }
            }

            if (methodName == "alpha_test")
            {
                switch ((Alpha_Test)option)
                {
                    case Alpha_Test.None:
                        vertexFunction = "off";
                        pixelFunction = "calc_alpha_test_off_ps";
                        break;
                    case Alpha_Test.Simple:
                        vertexFunction = "on";
                        pixelFunction = "calc_alpha_test_on_ps";
                        break;
                    case Alpha_Test.Multiply_Map:
                        vertexFunction = "multmap";
                        pixelFunction = "calc_alpha_test_multiply_map_ps";
                        break;
                }
            }

            if (methodName == "specular_mask")
            {
                switch ((Specular_Mask)option)
                {
                    case Specular_Mask.No_Specular_Mask:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_no_specular_mask_ps";
                        break;
                    case Specular_Mask.Specular_Mask_From_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_from_diffuse_ps";
                        break;
                    case Specular_Mask.Specular_Mask_From_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_texture_ps";
                        break;
                    case Specular_Mask.Specular_Mask_Mult_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_mult_texture_ps";
                        break;
                    case Specular_Mask.Specular_Mask_From_Color_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_color_texture_ps";
                        break;
                }
            }

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Diffuse_Only:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_only";
                        break;
                    case Material_Model.Two_Lobe_Phong:
                        vertexFunction = "invalid";
                        pixelFunction = "two_lobe_phong";
                        break;
                    case Material_Model.Foliage:
                        vertexFunction = "invalid";
                        pixelFunction = "foliage";
                        break;
                    case Material_Model.None:
                        vertexFunction = "invalid";
                        pixelFunction = "none";
                        break;
                    case Material_Model.Custom_Specular:
                        vertexFunction = "invalid";
                        pixelFunction = "custom_specular";
                        break;
                }
            }

            if (methodName == "environment_mapping")
            {
                switch ((Environment_Mapping)option)
                {
                    case Environment_Mapping.None:
                        vertexFunction = "invalid";
                        pixelFunction = "none";
                        break;
                    case Environment_Mapping.Per_Pixel:
                        vertexFunction = "invalid";
                        pixelFunction = "per_pixel";
                        break;
                    case Environment_Mapping.Dynamic:
                        vertexFunction = "invalid";
                        pixelFunction = "dynamic";
                        break;
                    case Environment_Mapping.From_Flat_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "from_flat_texture";
                        break;
                    case Environment_Mapping.Per_Pixel_Mip:
                        vertexFunction = "invalid";
                        pixelFunction = "per_pixel_mip";
                        break;
                    case Environment_Mapping.Dynamic_Reach:
                        vertexFunction = "invalid";
                        pixelFunction = "dynamic_reach";
                        break;
                }
            }

            if (methodName == "self_illumination")
            {
                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_none_ps";
                        break;
                    case Self_Illumination.Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_simple_ps";
                        break;
                    case Self_Illumination._3_Channel_Self_Illum:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_three_channel_ps";
                        break;
                    case Self_Illumination.Plasma:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_plasma_ps";
                        break;
                    case Self_Illumination.From_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_from_albedo_ps";
                        break;
                    case Self_Illumination.Illum_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_detail_ps";
                        break;
                    case Self_Illumination.Meter:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_meter_ps";
                        break;
                    case Self_Illumination.Self_Illum_Times_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_times_diffuse_ps";
                        break;
                    case Self_Illumination.Window_Room:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_window_room_ps";
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
                    case Parallax.Interpolated:
                        vertexFunction = "calc_parallax_interpolated_vs";
                        pixelFunction = "calc_parallax_interpolated_ps";
                        break;
                    case Parallax.Simple_Detail:
                        vertexFunction = "calc_parallax_simple_vs";
                        pixelFunction = "calc_parallax_simple_detail_ps";
                        break;
                }
            }

            if (methodName == "misc")
            {
                switch ((Misc)option)
                {
                    case Misc.First_Person_Never:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                    case Misc.First_Person_Sometimes:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                    case Misc.First_Person_Always:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                    case Misc.First_Person_Never_With_Rotating_Bitmaps:
                        vertexFunction = "invalid";
                        pixelFunction = "1";
                        break;
                    case Misc.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                    case Misc.Rotating_Bitmaps_Super_Slow:
                        vertexFunction = "invalid";
                        pixelFunction = "1";
                        break;
                    case Misc.Always_Calc_Albedo:
                        vertexFunction = "invalid";
                        pixelFunction = "2";
                        break;
                }
            }

            if (methodName == "wetness")
            {
                switch ((Wetness)option)
                {
                    case Wetness.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_default_ps";
                        break;
                    case Wetness.Flood:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_flood_ps";
                        break;
                    case Wetness.Proof:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_proof_ps";
                        break;
                    case Wetness.Ripples:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_ripples_ps";
                        break;
                }
            }
        }

        public ShaderParameters GetParameterArguments(string methodName, int option)
        {
            ShaderParameters result = new ShaderParameters();
            if (methodName == "albedo")
            {
                switch ((Albedo)option)
                {
                    case Albedo.Default:
                        break;
                    case Albedo.Detail_Blend:
                        break;
                    case Albedo.Constant_Color:
                        break;
                    case Albedo.Two_Change_Color:
                        break;
                    case Albedo.Four_Change_Color:
                        break;
                    case Albedo.Three_Detail_Blend:
                        break;
                    case Albedo.Two_Detail_Overlay:
                        break;
                    case Albedo.Two_Detail:
                        break;
                    case Albedo.Color_Mask:
                        break;
                    case Albedo.Two_Detail_Black_Point:
                        break;
                    case Albedo.Waterfall:
                        break;
                    case Albedo.Multiply_Map:
                        break;
                    case Albedo.Simple:
                        break;
                }
            }

            if (methodName == "bump_mapping")
            {
                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Off:
                        break;
                    case Bump_Mapping.Standard:
                        break;
                    case Bump_Mapping.Detail:
                        break;
                }
            }

            if (methodName == "alpha_test")
            {
                switch ((Alpha_Test)option)
                {
                    case Alpha_Test.None:
                        break;
                    case Alpha_Test.Simple:
                        break;
                    case Alpha_Test.Multiply_Map:
                        break;
                }
            }

            if (methodName == "specular_mask")
            {
                switch ((Specular_Mask)option)
                {
                    case Specular_Mask.No_Specular_Mask:
                        break;
                    case Specular_Mask.Specular_Mask_From_Diffuse:
                        break;
                    case Specular_Mask.Specular_Mask_From_Texture:
                        break;
                    case Specular_Mask.Specular_Mask_Mult_Diffuse:
                        break;
                    case Specular_Mask.Specular_Mask_From_Color_Texture:
                        break;
                }
            }

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Diffuse_Only:
                        break;
                    case Material_Model.Two_Lobe_Phong:
                        break;
                    case Material_Model.Foliage:
                        break;
                    case Material_Model.None:
                        break;
                    case Material_Model.Custom_Specular:
                        break;
                }
            }

            if (methodName == "environment_mapping")
            {
                switch ((Environment_Mapping)option)
                {
                    case Environment_Mapping.None:
                        break;
                    case Environment_Mapping.Per_Pixel:
                        break;
                    case Environment_Mapping.Dynamic:
                        break;
                    case Environment_Mapping.From_Flat_Texture:
                        break;
                    case Environment_Mapping.Per_Pixel_Mip:
                        break;
                    case Environment_Mapping.Dynamic_Reach:
                        break;
                }
            }

            if (methodName == "self_illumination")
            {
                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.Off:
                        break;
                    case Self_Illumination.Simple:
                        break;
                    case Self_Illumination._3_Channel_Self_Illum:
                        break;
                    case Self_Illumination.Plasma:
                        break;
                    case Self_Illumination.From_Diffuse:
                        break;
                    case Self_Illumination.Illum_Detail:
                        break;
                    case Self_Illumination.Meter:
                        break;
                    case Self_Illumination.Self_Illum_Times_Diffuse:
                        break;
                    case Self_Illumination.Window_Room:
                        break;
                }
            }

            if (methodName == "blend_mode")
            {
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
                }
            }

            if (methodName == "parallax")
            {
                switch ((Parallax)option)
                {
                    case Parallax.Off:
                        break;
                    case Parallax.Simple:
                        break;
                    case Parallax.Interpolated:
                        break;
                    case Parallax.Simple_Detail:
                        break;
                }
            }

            if (methodName == "misc")
            {
                switch ((Misc)option)
                {
                    case Misc.First_Person_Never:
                        break;
                    case Misc.First_Person_Sometimes:
                        break;
                    case Misc.First_Person_Always:
                        break;
                    case Misc.First_Person_Never_With_Rotating_Bitmaps:
                        break;
                    case Misc.Default:
                        break;
                    case Misc.Rotating_Bitmaps_Super_Slow:
                        break;
                    case Misc.Always_Calc_Albedo:
                        break;
                }
            }

            if (methodName == "wetness")
            {
                switch ((Wetness)option)
                {
                    case Wetness.Default:
                        break;
                    case Wetness.Flood:
                        break;
                    case Wetness.Proof:
                        break;
                    case Wetness.Ripples:
                        break;
                }
            }
            return result;
        }
    }
}
