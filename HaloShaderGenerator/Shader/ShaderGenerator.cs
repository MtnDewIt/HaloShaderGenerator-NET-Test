using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Shader
{
    public class ShaderGenerator : IShaderGenerator
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
        Distortion distortion;
        Soft_Fade soft_fade;
        Misc_Attr_Animation misc_attr_animation;
        Wetness wetness;
        Alpha_Blend_Source alpha_blend_source;

        public ShaderGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        public ShaderGenerator(Albedo albedo, Bump_Mapping bump_mapping, Alpha_Test alpha_test, Specular_Mask specular_mask, Material_Model material_model, Environment_Mapping environment_mapping, Self_Illumination self_illumination, Blend_Mode blend_mode, Parallax parallax, Misc misc, Distortion distortion, Soft_Fade soft_fade, Misc_Attr_Animation misc_attr_animation, Wetness wetness, Alpha_Blend_Source alpha_blend_source, bool applyFixes = false)
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
            this.distortion = distortion;
            this.soft_fade = soft_fade;
            this.misc_attr_animation = misc_attr_animation;
            this.wetness = wetness;
            this.alpha_blend_source = alpha_blend_source;

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public ShaderGenerator(byte[] options, bool applyFixes = false)
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
            this.distortion = (Distortion)options[10];
            this.soft_fade = (Soft_Fade)options[11];
            this.misc_attr_animation = (Misc_Attr_Animation)options[12];
            this.wetness = (Wetness)options[13];
            this.alpha_blend_source = (Alpha_Blend_Source)options[14];

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
            Shared.Alpha_Blend_Source sAlphaBlendSource = (Shared.Alpha_Blend_Source)Enum.Parse(typeof(Shared.Alpha_Blend_Source), alpha_blend_source.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Shader, entryPoint, sBlendMode,
                sMisc, sAlphaTest, sAlphaBlendSource, ApplyFixes);

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
                case Albedo.Two_Change_Color_Anim_Overlay:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_anim_ps"));
                    break;
                case Albedo.Chameleon:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_chameleon_ps"));
                    break;
                case Albedo.Two_Change_Color_Chameleon:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_chameleon_ps"));
                    break;
                case Albedo.Chameleon_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_chameleon_masked_ps"));
                    break;
                case Albedo.Color_Mask_Hard_Light:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_color_mask_hard_light_ps"));
                    break;
                case Albedo.Two_Change_Color_Tex_Overlay:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_tex_overlay_ps"));
                    break;
                case Albedo.Chameleon_Albedo_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_chameleon_albedo_masked_ps"));
                    break;
                case Albedo.Custom_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_custom_cube_ps"));
                    break;
                case Albedo.Two_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_color_ps"));
                    break;
                case Albedo.Scrolling_Cube_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_scrolling_cube_mask_ps"));
                    break;
                case Albedo.Scrolling_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_scrolling_cube_ps"));
                    break;
                case Albedo.Scrolling_Texture_Uv:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_scrolling_texture_uv_ps"));
                    break;
                case Albedo.Texture_From_Misc:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_texture_from_misc_ps"));
                    break;
                case Albedo.Four_Change_Color_Applying_To_Specular:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_four_change_color_applying_to_specular_ps"));
                    break;
                case Albedo.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_simple_ps"));
                    break;
                case Albedo.Emblem:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_emblem_ps"));
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
                case Bump_Mapping.Detail_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_masked_ps"));
                    break;
                case Bump_Mapping.Detail_Plus_Detail_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "invalid"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_plus_detail_masked_ps"));
                    break;
                case Bump_Mapping.Detail_Unorm:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "invalid"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_unorm_ps"));
                    break;
                case Bump_Mapping.Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_blend_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_blend_ps"));
                    break;
                case Bump_Mapping.Three_Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_three_detail_blend_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_three_detail_blend_ps"));
                    break;
                case Bump_Mapping.Standard_Wrinkle:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_default_wrinkle_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_default_wrinkle_ps"));
                    break;
                case Bump_Mapping.Detail_Wrinkle:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_wrinkle_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_wrinkle_ps"));
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
                case Specular_Mask.Specular_Mask_From_Color_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_color_texture_ps"));
                    break;
                case Specular_Mask.Specular_Mask_Mult_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_mult_texture_ps"));
                    break;
            }

            switch (material_model)
            {
                case Material_Model.Diffuse_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "diffuse_only"));
                    break;
                case Material_Model.Cook_Torrance:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance"));
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
                case Material_Model.Glass:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "glass"));
                    break;
                case Material_Model.Organism:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "organism"));
                    break;
                case Material_Model.Single_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "single_lobe_phong"));
                    break;
                case Material_Model.Car_Paint:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "car_paint"));
                    break;
                case Material_Model.Cook_Torrance_Custom_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_custom_cube"));
                    break;
                case Material_Model.Cook_Torrance_Pbr_Maps:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_pbr_maps"));
                    break;
                case Material_Model.Cook_Torrance_Two_Color_Spec_Tint:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_two_color_spec_tint"));
                    break;
                case Material_Model.Two_Lobe_Phong_Tint_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong_tint_map"));
                    break;
                case Material_Model.Cook_Torrance_Scrolling_Cube_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_scrolling_cube_mask"));
                    break;
                case Material_Model.Cook_Torrance_Rim_Fresnel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_rim_fresnel"));
                    break;
                case Material_Model.Cook_Torrance_Scrolling_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_scrolling_cube"));
                    break;
                case Material_Model.Cook_Torrance_From_Albedo:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_from_albedo"));
                    break;
                case Material_Model.Hair:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "hair"));
                    break;
                case Material_Model.Cook_Torrance_Reach:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_reach"));
                    break;
                case Material_Model.Two_Lobe_Phong_Reach:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong_reach"));
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
                case Environment_Mapping.Custom_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "custom_map"));
                    break;
                case Environment_Mapping.From_Flat_Texture_As_Cubemap:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "from_flat_texture_as_cubemap"));
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
                case Self_Illumination.Simple_With_Alpha_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_simple_with_alpha_mask_ps"));
                    break;
                case Self_Illumination.Simple_Four_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_simple_ps"));
                    break;
                case Self_Illumination.Illum_Detail_World_Space_Four_Cc:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_detail_world_space_ps"));
                    break;
                case Self_Illumination.Illum_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_change_color_ps"));
                    break;
                case Self_Illumination.Multilayer_Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Palettized_Plasma:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_plasma_ps"));
                    break;
                case Self_Illumination.Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_change_color_ps"));
                    break;
                case Self_Illumination.Change_Color_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_change_color_detail_ps"));
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
                case Blend_Mode.Pre_Multiplied_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "pre_multiplied_alpha"));
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
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
            }

            switch (distortion)
            {
                case Distortion.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_vs", "distort_nocolor_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_ps", "distort_off_ps"));
                    break;
                case Distortion.On:
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_vs", "distort_nocolor_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_ps", "distort_on_ps"));
                    break;
            }

            switch (soft_fade)
            {
                case Soft_Fade.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("apply_soft_fade", "apply_soft_fade_off"));
                    break;
                case Soft_Fade.On:
                    macros.Add(ShaderGeneratorBase.CreateMacro("apply_soft_fade", "apply_soft_fade_on"));
                    break;
            }

            switch (misc_attr_animation)
            {
                case Misc_Attr_Animation.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("misc_attr_define", "invalid"));
                    break;
                case Misc_Attr_Animation.Scrolling_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("misc_attr_define", "misc_attr_exist"));
                    break;
                case Misc_Attr_Animation.Scrolling_Projected:
                    macros.Add(ShaderGeneratorBase.CreateMacro("misc_attr_define", "misc_attr_exist"));
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
                case Wetness.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_simple_ps"));
                    break;
                case Wetness.Ripples:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_ripples_ps"));
                    break;
            }

            switch (alpha_blend_source)
            {
                case Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "albedo_alpha_without_fresnel"));
                    break;
                case Alpha_Blend_Source.From_Albedo_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "albedo_alpha"));
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "opacity_map_alpha"));
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "opacity_map_rgb"));
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "opacity_map_alpha_and_albedo_alpha"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Shader);
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
            Shared.Alpha_Blend_Source sAlphaBlendSource = (Shared.Alpha_Blend_Source)Enum.Parse(typeof(Shared.Alpha_Blend_Source), alpha_blend_source.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Shader, entryPoint, sBlendMode,
                sMisc, sAlphaTest, sAlphaBlendSource, ApplyFixes);

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
                case Albedo.Two_Change_Color_Anim_Overlay:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_anim_ps"));
                    break;
                case Albedo.Chameleon:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_chameleon_ps"));
                    break;
                case Albedo.Two_Change_Color_Chameleon:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_chameleon_ps"));
                    break;
                case Albedo.Chameleon_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_chameleon_masked_ps"));
                    break;
                case Albedo.Color_Mask_Hard_Light:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_color_mask_hard_light_ps"));
                    break;
                case Albedo.Two_Change_Color_Tex_Overlay:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_tex_overlay_ps"));
                    break;
                case Albedo.Chameleon_Albedo_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_chameleon_albedo_masked_ps"));
                    break;
                case Albedo.Custom_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_custom_cube_ps"));
                    break;
                case Albedo.Two_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_color_ps"));
                    break;
                case Albedo.Scrolling_Cube_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_scrolling_cube_mask_ps"));
                    break;
                case Albedo.Scrolling_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_scrolling_cube_ps"));
                    break;
                case Albedo.Scrolling_Texture_Uv:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_scrolling_texture_uv_ps"));
                    break;
                case Albedo.Texture_From_Misc:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_texture_from_misc_ps"));
                    break;
                case Albedo.Four_Change_Color_Applying_To_Specular:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_four_change_color_applying_to_specular_ps"));
                    break;
                case Albedo.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_simple_ps"));
                    break;
                case Albedo.Emblem:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_emblem_ps"));
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
                case Bump_Mapping.Detail_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_masked_ps"));
                    break;
                case Bump_Mapping.Detail_Plus_Detail_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "invalid"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_plus_detail_masked_ps"));
                    break;
                case Bump_Mapping.Detail_Unorm:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "invalid"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_unorm_ps"));
                    break;
                case Bump_Mapping.Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_blend_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_blend_ps"));
                    break;
                case Bump_Mapping.Three_Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_three_detail_blend_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_three_detail_blend_ps"));
                    break;
                case Bump_Mapping.Standard_Wrinkle:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_default_wrinkle_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_default_wrinkle_ps"));
                    break;
                case Bump_Mapping.Detail_Wrinkle:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_wrinkle_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_wrinkle_ps"));
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
                case Specular_Mask.Specular_Mask_From_Color_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_color_texture_ps"));
                    break;
                case Specular_Mask.Specular_Mask_Mult_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_mult_texture_ps"));
                    break;
            }

            switch (material_model)
            {
                case Material_Model.Diffuse_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "diffuse_only"));
                    break;
                case Material_Model.Cook_Torrance:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance"));
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
                case Material_Model.Glass:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "glass"));
                    break;
                case Material_Model.Organism:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "organism"));
                    break;
                case Material_Model.Single_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "single_lobe_phong"));
                    break;
                case Material_Model.Car_Paint:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "car_paint"));
                    break;
                case Material_Model.Cook_Torrance_Custom_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_custom_cube"));
                    break;
                case Material_Model.Cook_Torrance_Pbr_Maps:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_pbr_maps"));
                    break;
                case Material_Model.Cook_Torrance_Two_Color_Spec_Tint:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_two_color_spec_tint"));
                    break;
                case Material_Model.Two_Lobe_Phong_Tint_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong_tint_map"));
                    break;
                case Material_Model.Cook_Torrance_Scrolling_Cube_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_scrolling_cube_mask"));
                    break;
                case Material_Model.Cook_Torrance_Rim_Fresnel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_rim_fresnel"));
                    break;
                case Material_Model.Cook_Torrance_Scrolling_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_scrolling_cube"));
                    break;
                case Material_Model.Cook_Torrance_From_Albedo:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_from_albedo"));
                    break;
                case Material_Model.Hair:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "hair"));
                    break;
                case Material_Model.Cook_Torrance_Reach:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_reach"));
                    break;
                case Material_Model.Two_Lobe_Phong_Reach:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong_reach"));
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
                case Environment_Mapping.Custom_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "custom_map"));
                    break;
                case Environment_Mapping.From_Flat_Texture_As_Cubemap:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "from_flat_texture_as_cubemap"));
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
                case Self_Illumination.Simple_With_Alpha_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_simple_with_alpha_mask_ps"));
                    break;
                case Self_Illumination.Simple_Four_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_simple_ps"));
                    break;
                case Self_Illumination.Illum_Detail_World_Space_Four_Cc:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_detail_world_space_ps"));
                    break;
                case Self_Illumination.Illum_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_change_color_ps"));
                    break;
                case Self_Illumination.Multilayer_Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Palettized_Plasma:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_plasma_ps"));
                    break;
                case Self_Illumination.Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_change_color_ps"));
                    break;
                case Self_Illumination.Change_Color_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_change_color_detail_ps"));
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
                case Blend_Mode.Pre_Multiplied_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "pre_multiplied_alpha"));
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
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
            }

            switch (distortion)
            {
                case Distortion.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_vs", "distort_nocolor_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_ps", "distort_off_ps"));
                    break;
                case Distortion.On:
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_vs", "distort_nocolor_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_ps", "distort_on_ps"));
                    break;
            }

            switch (soft_fade)
            {
                case Soft_Fade.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("apply_soft_fade", "apply_soft_fade_off"));
                    break;
                case Soft_Fade.On:
                    macros.Add(ShaderGeneratorBase.CreateMacro("apply_soft_fade", "apply_soft_fade_on"));
                    break;
            }

            switch (misc_attr_animation)
            {
                case Misc_Attr_Animation.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("misc_attr_define", "invalid"));
                    break;
                case Misc_Attr_Animation.Scrolling_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("misc_attr_define", "misc_attr_exist"));
                    break;
                case Misc_Attr_Animation.Scrolling_Projected:
                    macros.Add(ShaderGeneratorBase.CreateMacro("misc_attr_define", "misc_attr_exist"));
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
                case Wetness.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_simple_ps"));
                    break;
                case Wetness.Ripples:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_ripples_ps"));
                    break;
            }

            switch (alpha_blend_source)
            {
                case Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "albedo_alpha_without_fresnel"));
                    break;
                case Alpha_Blend_Source.From_Albedo_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "albedo_alpha"));
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "opacity_map_alpha"));
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "opacity_map_rgb"));
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "opacity_map_alpha_and_albedo_alpha"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Shader);
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
            Shared.Alpha_Blend_Source sAlphaBlendSource = (Shared.Alpha_Blend_Source)Enum.Parse(typeof(Shared.Alpha_Blend_Source), alpha_blend_source.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Shader, entryPoint,
                sBlendMode, sMisc, sAlphaTest, sAlphaBlendSource, ApplyFixes, true, vertexType);

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
                case Albedo.Two_Change_Color_Anim_Overlay:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_anim_ps"));
                    break;
                case Albedo.Chameleon:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_chameleon_ps"));
                    break;
                case Albedo.Two_Change_Color_Chameleon:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_chameleon_ps"));
                    break;
                case Albedo.Chameleon_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_chameleon_masked_ps"));
                    break;
                case Albedo.Color_Mask_Hard_Light:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_color_mask_hard_light_ps"));
                    break;
                case Albedo.Two_Change_Color_Tex_Overlay:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_tex_overlay_ps"));
                    break;
                case Albedo.Chameleon_Albedo_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_chameleon_albedo_masked_ps"));
                    break;
                case Albedo.Custom_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_custom_cube_ps"));
                    break;
                case Albedo.Two_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_color_ps"));
                    break;
                case Albedo.Scrolling_Cube_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_scrolling_cube_mask_ps"));
                    break;
                case Albedo.Scrolling_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_scrolling_cube_ps"));
                    break;
                case Albedo.Scrolling_Texture_Uv:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_scrolling_texture_uv_ps"));
                    break;
                case Albedo.Texture_From_Misc:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_texture_from_misc_ps"));
                    break;
                case Albedo.Four_Change_Color_Applying_To_Specular:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_four_change_color_applying_to_specular_ps"));
                    break;
                case Albedo.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_simple_ps"));
                    break;
                case Albedo.Emblem:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_emblem_ps"));
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
                case Bump_Mapping.Detail_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_masked_ps"));
                    break;
                case Bump_Mapping.Detail_Plus_Detail_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "invalid"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_plus_detail_masked_ps"));
                    break;
                case Bump_Mapping.Detail_Unorm:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "invalid"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_unorm_ps"));
                    break;
                case Bump_Mapping.Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_blend_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_blend_ps"));
                    break;
                case Bump_Mapping.Three_Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_three_detail_blend_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_three_detail_blend_ps"));
                    break;
                case Bump_Mapping.Standard_Wrinkle:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_default_wrinkle_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_default_wrinkle_ps"));
                    break;
                case Bump_Mapping.Detail_Wrinkle:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_wrinkle_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_wrinkle_ps"));
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
                case Specular_Mask.Specular_Mask_From_Color_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_color_texture_ps"));
                    break;
                case Specular_Mask.Specular_Mask_Mult_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_mult_texture_ps"));
                    break;
            }

            switch (material_model)
            {
                case Material_Model.Diffuse_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "diffuse_only"));
                    break;
                case Material_Model.Cook_Torrance:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance"));
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
                case Material_Model.Glass:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "glass"));
                    break;
                case Material_Model.Organism:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "organism"));
                    break;
                case Material_Model.Single_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "single_lobe_phong"));
                    break;
                case Material_Model.Car_Paint:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "car_paint"));
                    break;
                case Material_Model.Cook_Torrance_Custom_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_custom_cube"));
                    break;
                case Material_Model.Cook_Torrance_Pbr_Maps:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_pbr_maps"));
                    break;
                case Material_Model.Cook_Torrance_Two_Color_Spec_Tint:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_two_color_spec_tint"));
                    break;
                case Material_Model.Two_Lobe_Phong_Tint_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong_tint_map"));
                    break;
                case Material_Model.Cook_Torrance_Scrolling_Cube_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_scrolling_cube_mask"));
                    break;
                case Material_Model.Cook_Torrance_Rim_Fresnel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_rim_fresnel"));
                    break;
                case Material_Model.Cook_Torrance_Scrolling_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_scrolling_cube"));
                    break;
                case Material_Model.Cook_Torrance_From_Albedo:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_from_albedo"));
                    break;
                case Material_Model.Hair:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "hair"));
                    break;
                case Material_Model.Cook_Torrance_Reach:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_reach"));
                    break;
                case Material_Model.Two_Lobe_Phong_Reach:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong_reach"));
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
                case Environment_Mapping.Custom_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "custom_map"));
                    break;
                case Environment_Mapping.From_Flat_Texture_As_Cubemap:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "from_flat_texture_as_cubemap"));
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
                case Self_Illumination.Simple_With_Alpha_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_simple_with_alpha_mask_ps"));
                    break;
                case Self_Illumination.Simple_Four_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_simple_ps"));
                    break;
                case Self_Illumination.Illum_Detail_World_Space_Four_Cc:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_detail_world_space_ps"));
                    break;
                case Self_Illumination.Illum_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_change_color_ps"));
                    break;
                case Self_Illumination.Multilayer_Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Palettized_Plasma:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_plasma_ps"));
                    break;
                case Self_Illumination.Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_change_color_ps"));
                    break;
                case Self_Illumination.Change_Color_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_change_color_detail_ps"));
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
                case Blend_Mode.Pre_Multiplied_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "pre_multiplied_alpha"));
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
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
            }

            switch (distortion)
            {
                case Distortion.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_vs", "distort_nocolor_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_ps", "distort_off_ps"));
                    break;
                case Distortion.On:
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_vs", "distort_nocolor_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_ps", "distort_on_ps"));
                    break;
            }

            switch (soft_fade)
            {
                case Soft_Fade.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("apply_soft_fade", "apply_soft_fade_off"));
                    break;
                case Soft_Fade.On:
                    macros.Add(ShaderGeneratorBase.CreateMacro("apply_soft_fade", "apply_soft_fade_on"));
                    break;
            }

            switch (misc_attr_animation)
            {
                case Misc_Attr_Animation.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("misc_attr_define", "invalid"));
                    break;
                case Misc_Attr_Animation.Scrolling_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("misc_attr_define", "misc_attr_exist"));
                    break;
                case Misc_Attr_Animation.Scrolling_Projected:
                    macros.Add(ShaderGeneratorBase.CreateMacro("misc_attr_define", "misc_attr_exist"));
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
                case Wetness.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_simple_ps"));
                    break;
                case Wetness.Ripples:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_ripples_ps"));
                    break;
            }

            switch (alpha_blend_source)
            {
                case Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "albedo_alpha_without_fresnel"));
                    break;
                case Alpha_Blend_Source.From_Albedo_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "albedo_alpha"));
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "opacity_map_alpha"));
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "opacity_map_rgb"));
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "opacity_map_alpha_and_albedo_alpha"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Shader);
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
            Shared.Alpha_Blend_Source sAlphaBlendSource = (Shared.Alpha_Blend_Source)Enum.Parse(typeof(Shared.Alpha_Blend_Source), alpha_blend_source.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Shader, entryPoint,
                sBlendMode, sMisc, sAlphaTest, sAlphaBlendSource, ApplyFixes, true, vertexType);

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
                case Albedo.Two_Change_Color_Anim_Overlay:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_anim_ps"));
                    break;
                case Albedo.Chameleon:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_chameleon_ps"));
                    break;
                case Albedo.Two_Change_Color_Chameleon:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_chameleon_ps"));
                    break;
                case Albedo.Chameleon_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_chameleon_masked_ps"));
                    break;
                case Albedo.Color_Mask_Hard_Light:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_color_mask_hard_light_ps"));
                    break;
                case Albedo.Two_Change_Color_Tex_Overlay:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_change_color_tex_overlay_ps"));
                    break;
                case Albedo.Chameleon_Albedo_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_chameleon_albedo_masked_ps"));
                    break;
                case Albedo.Custom_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_custom_cube_ps"));
                    break;
                case Albedo.Two_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_two_color_ps"));
                    break;
                case Albedo.Scrolling_Cube_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_scrolling_cube_mask_ps"));
                    break;
                case Albedo.Scrolling_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_scrolling_cube_ps"));
                    break;
                case Albedo.Scrolling_Texture_Uv:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_scrolling_texture_uv_ps"));
                    break;
                case Albedo.Texture_From_Misc:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_texture_from_misc_ps"));
                    break;
                case Albedo.Four_Change_Color_Applying_To_Specular:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_four_change_color_applying_to_specular_ps"));
                    break;
                case Albedo.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_simple_ps"));
                    break;
                case Albedo.Emblem:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_emblem_ps"));
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
                case Bump_Mapping.Detail_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_masked_ps"));
                    break;
                case Bump_Mapping.Detail_Plus_Detail_Masked:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "invalid"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_plus_detail_masked_ps"));
                    break;
                case Bump_Mapping.Detail_Unorm:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "invalid"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_unorm_ps"));
                    break;
                case Bump_Mapping.Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_blend_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_blend_ps"));
                    break;
                case Bump_Mapping.Three_Detail_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_three_detail_blend_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_three_detail_blend_ps"));
                    break;
                case Bump_Mapping.Standard_Wrinkle:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_default_wrinkle_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_default_wrinkle_ps"));
                    break;
                case Bump_Mapping.Detail_Wrinkle:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", "calc_bumpmap_detail_wrinkle_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_wrinkle_ps"));
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
                case Specular_Mask.Specular_Mask_From_Color_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_color_texture_ps"));
                    break;
                case Specular_Mask.Specular_Mask_Mult_Diffuse:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", "calc_specular_mask_mult_texture_ps"));
                    break;
            }

            switch (material_model)
            {
                case Material_Model.Diffuse_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "diffuse_only"));
                    break;
                case Material_Model.Cook_Torrance:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance"));
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
                case Material_Model.Glass:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "glass"));
                    break;
                case Material_Model.Organism:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "organism"));
                    break;
                case Material_Model.Single_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "single_lobe_phong"));
                    break;
                case Material_Model.Car_Paint:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "car_paint"));
                    break;
                case Material_Model.Cook_Torrance_Custom_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_custom_cube"));
                    break;
                case Material_Model.Cook_Torrance_Pbr_Maps:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_pbr_maps"));
                    break;
                case Material_Model.Cook_Torrance_Two_Color_Spec_Tint:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_two_color_spec_tint"));
                    break;
                case Material_Model.Two_Lobe_Phong_Tint_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong_tint_map"));
                    break;
                case Material_Model.Cook_Torrance_Scrolling_Cube_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_scrolling_cube_mask"));
                    break;
                case Material_Model.Cook_Torrance_Rim_Fresnel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_rim_fresnel"));
                    break;
                case Material_Model.Cook_Torrance_Scrolling_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_scrolling_cube"));
                    break;
                case Material_Model.Cook_Torrance_From_Albedo:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_from_albedo"));
                    break;
                case Material_Model.Hair:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "hair"));
                    break;
                case Material_Model.Cook_Torrance_Reach:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "cook_torrance_reach"));
                    break;
                case Material_Model.Two_Lobe_Phong_Reach:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong_reach"));
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
                case Environment_Mapping.Custom_Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "custom_map"));
                    break;
                case Environment_Mapping.From_Flat_Texture_As_Cubemap:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "from_flat_texture_as_cubemap"));
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
                case Self_Illumination.Simple_With_Alpha_Mask:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_simple_with_alpha_mask_ps"));
                    break;
                case Self_Illumination.Simple_Four_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_simple_ps"));
                    break;
                case Self_Illumination.Illum_Detail_World_Space_Four_Cc:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_detail_world_space_ps"));
                    break;
                case Self_Illumination.Illum_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_change_color_ps"));
                    break;
                case Self_Illumination.Multilayer_Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Palettized_Plasma:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_plasma_ps"));
                    break;
                case Self_Illumination.Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_change_color_ps"));
                    break;
                case Self_Illumination.Change_Color_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_change_color_detail_ps"));
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
                case Blend_Mode.Pre_Multiplied_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "pre_multiplied_alpha"));
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
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "0"));
                    break;
            }

            switch (distortion)
            {
                case Distortion.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_vs", "distort_nocolor_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_ps", "distort_off_ps"));
                    break;
                case Distortion.On:
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_vs", "distort_nocolor_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_ps", "distort_on_ps"));
                    break;
            }

            switch (soft_fade)
            {
                case Soft_Fade.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("apply_soft_fade", "apply_soft_fade_off"));
                    break;
                case Soft_Fade.On:
                    macros.Add(ShaderGeneratorBase.CreateMacro("apply_soft_fade", "apply_soft_fade_on"));
                    break;
            }

            switch (misc_attr_animation)
            {
                case Misc_Attr_Animation.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("misc_attr_define", "invalid"));
                    break;
                case Misc_Attr_Animation.Scrolling_Cube:
                    macros.Add(ShaderGeneratorBase.CreateMacro("misc_attr_define", "misc_attr_exist"));
                    break;
                case Misc_Attr_Animation.Scrolling_Projected:
                    macros.Add(ShaderGeneratorBase.CreateMacro("misc_attr_define", "misc_attr_exist"));
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
                case Wetness.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_simple_ps"));
                    break;
                case Wetness.Ripples:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_ripples_ps"));
                    break;
            }

            switch (alpha_blend_source)
            {
                case Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "albedo_alpha_without_fresnel"));
                    break;
                case Alpha_Blend_Source.From_Albedo_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "albedo_alpha"));
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "opacity_map_alpha"));
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "opacity_map_rgb"));
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                    macros.Add(ShaderGeneratorBase.CreateMacro("alpha_blend_source", "opacity_map_alpha_and_albedo_alpha"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Shader);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(ShaderMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((ShaderMethods)methodIndex)
            {
                case ShaderMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case ShaderMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping)).Length;
                case ShaderMethods.Alpha_Test:
                    return Enum.GetValues(typeof(Alpha_Test)).Length;
                case ShaderMethods.Specular_Mask:
                    return Enum.GetValues(typeof(Specular_Mask)).Length;
                case ShaderMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model)).Length;
                case ShaderMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping)).Length;
                case ShaderMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination)).Length;
                case ShaderMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
                case ShaderMethods.Parallax:
                    return Enum.GetValues(typeof(Parallax)).Length;
                case ShaderMethods.Misc:
                    return Enum.GetValues(typeof(Misc)).Length;
                case ShaderMethods.Distortion:
                    return Enum.GetValues(typeof(Distortion)).Length;
                case ShaderMethods.Soft_Fade:
                    return Enum.GetValues(typeof(Soft_Fade)).Length;
                case ShaderMethods.Misc_Attr_Animation:
                    return Enum.GetValues(typeof(Misc_Attr_Animation)).Length;
                case ShaderMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness)).Length;
                case ShaderMethods.Alpha_Blend_Source:
                    return Enum.GetValues(typeof(Alpha_Blend_Source)).Length;
            }

            return -1;
        }

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((ShaderMethods)methodIndex)
            {
                case ShaderMethods.Albedo:
                    return (int)albedo;
                case ShaderMethods.Bump_Mapping:
                    return (int)bump_mapping;
                case ShaderMethods.Alpha_Test:
                    return (int)alpha_test;
                case ShaderMethods.Specular_Mask:
                    return (int)specular_mask;
                case ShaderMethods.Material_Model:
                    return (int)material_model;
                case ShaderMethods.Environment_Mapping:
                    return (int)environment_mapping;
                case ShaderMethods.Self_Illumination:
                    return (int)self_illumination;
                case ShaderMethods.Blend_Mode:
                    return (int)blend_mode;
                case ShaderMethods.Parallax:
                    return (int)parallax;
                case ShaderMethods.Misc:
                    return (int)misc;
                case ShaderMethods.Distortion:
                    return (int)distortion;
                case ShaderMethods.Soft_Fade:
                    return (int)soft_fade;
                case ShaderMethods.Misc_Attr_Animation:
                    return (int)misc_attr_animation;
                case ShaderMethods.Wetness:
                    return (int)wetness;
                case ShaderMethods.Alpha_Blend_Source:
                    return (int)alpha_blend_source;
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
                case ShaderStage.Active_Camo:
                case ShaderStage.Static_Per_Vertex_Color:
                case ShaderStage.Lightmap_Debug_Mode:
                case ShaderStage.Dynamic_Light_Cinematic:
                case ShaderStage.Stipple:
                case ShaderStage.Single_Pass_Per_Pixel:
                case ShaderStage.Single_Pass_Per_Vertex:
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
                case ShaderStage.Active_Camo:
                    return true;
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
                case VertexType.DualQuat:
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
                case ShaderStage.Active_Camo:
                case ShaderStage.Lightmap_Debug_Mode:
                case ShaderStage.Static_Per_Vertex_Color:
                case ShaderStage.Dynamic_Light_Cinematic:
                case ShaderStage.Z_Only:
                case ShaderStage.Sfx_Distort:
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
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
                    result.AddFloatParameter("blend_alpha");
                    break;
                case Albedo.Constant_Color:
                    result.AddFloat4ColorParameter("albedo_color");
                    break;
                case Albedo.Two_Change_Color:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("change_color_map");
                    result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                    result.AddSamplerWithoutXFormParameter("camouflage_change_color_map");
                    result.AddFloatParameter("camouflage_scale");
                    break;
                case Albedo.Four_Change_Color:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("change_color_map");
                    result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                    result.AddFloat3ColorParameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary);
                    result.AddFloat3ColorParameter("quaternary_change_color", RenderMethodExtern.object_change_color_quaternary);
                    result.AddSamplerWithoutXFormParameter("camouflage_change_color_map");
                    result.AddFloatParameter("camouflage_scale");
                    break;
                case Albedo.Three_Detail_Blend:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
                    result.AddSamplerParameter("detail_map3");
                    result.AddFloatParameter("blend_alpha");
                    break;
                case Albedo.Two_Detail_Overlay:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
                    result.AddSamplerParameter("detail_map_overlay");
                    break;
                case Albedo.Two_Detail:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
                    break;
                case Albedo.Color_Mask:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("color_mask_map");
                    result.AddFloat4ColorParameter("albedo_color");
                    result.AddFloat4ColorParameter("albedo_color2");
                    result.AddFloat4ColorParameter("albedo_color3");
                    result.AddFloat3ColorParameter("neutral_gray");
                    break;
                case Albedo.Two_Detail_Black_Point:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
                    break;
                case Albedo.Two_Change_Color_Anim_Overlay:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddSamplerWithoutXFormParameter("change_color_map");
                    result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                    result.AddFloat3ColorParameter("primary_change_color_anim");
                    result.AddFloat3ColorParameter("secondary_change_color_anim");
                    break;
                case Albedo.Chameleon:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddFloat3ColorParameter("chameleon_color0");
                    result.AddFloat3ColorParameter("chameleon_color1");
                    result.AddFloat3ColorParameter("chameleon_color2");
                    result.AddFloat3ColorParameter("chameleon_color3");
                    result.AddFloatParameter("chameleon_color_offset1");
                    result.AddFloatParameter("chameleon_color_offset2");
                    result.AddFloatParameter("chameleon_fresnel_power");
                    break;
                case Albedo.Two_Change_Color_Chameleon:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddSamplerWithoutXFormParameter("change_color_map");
                    result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                    result.AddFloat3ColorParameter("primary_change_color_anim");
                    result.AddFloat3ColorParameter("secondary_change_color_anim");
                    result.AddFloat3ColorParameter("chameleon_color0");
                    result.AddFloat3ColorParameter("chameleon_color1");
                    result.AddFloat3ColorParameter("chameleon_color2");
                    result.AddFloat3ColorParameter("chameleon_color3");
                    result.AddFloatParameter("chameleon_color_offset1");
                    result.AddFloatParameter("chameleon_color_offset2");
                    result.AddFloatParameter("chameleon_fresnel_power");
                    break;
                case Albedo.Chameleon_Masked:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("chameleon_mask_map");
                    result.AddFloat3ColorParameter("chameleon_color0");
                    result.AddFloat3ColorParameter("chameleon_color1");
                    result.AddFloat3ColorParameter("chameleon_color2");
                    result.AddFloat3ColorParameter("chameleon_color3");
                    result.AddFloatParameter("chameleon_color_offset1");
                    result.AddFloatParameter("chameleon_color_offset2");
                    result.AddFloatParameter("chameleon_fresnel_power");
                    break;
                case Albedo.Color_Mask_Hard_Light:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddSamplerWithoutXFormParameter("color_mask_map");
                    result.AddFloat4ColorParameter("albedo_color");
                    break;
                case Albedo.Two_Change_Color_Tex_Overlay:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddSamplerWithoutXFormParameter("change_color_map");
                    result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddSamplerWithoutXFormParameter("secondary_change_color_map");
                    break;
                case Albedo.Chameleon_Albedo_Masked:
                    result.AddSamplerParameter("base_map");
                    result.AddFloat4ColorParameter("albedo_color");
                    result.AddSamplerParameter("base_masked_map");
                    result.AddFloat4ColorParameter("albedo_masked_color");
                    result.AddSamplerParameter("chameleon_mask_map");
                    result.AddFloat3ColorParameter("chameleon_color0");
                    result.AddFloat3ColorParameter("chameleon_color1");
                    result.AddFloat3ColorParameter("chameleon_color2");
                    result.AddFloat3ColorParameter("chameleon_color3");
                    result.AddFloatParameter("chameleon_color_offset1");
                    result.AddFloatParameter("chameleon_color_offset2");
                    result.AddFloatParameter("chameleon_fresnel_power");
                    break;
                case Albedo.Custom_Cube:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("custom_cube");
                    result.AddFloat4ColorParameter("albedo_color");
                    break;
                case Albedo.Two_Color:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddFloat4ColorParameter("albedo_color");
                    result.AddSamplerWithoutXFormParameter("blend_map");
                    result.AddFloat4ColorParameter("albedo_second_color");
                    break;
                case Albedo.Scrolling_Cube_Mask:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddFloat4ColorParameter("albedo_color");
                    result.AddSamplerWithoutXFormParameter("color_blend_mask_cubemap");
                    result.AddFloat4ColorParameter("albedo_second_color");
                    break;
                case Albedo.Scrolling_Cube:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddSamplerWithoutXFormParameter("color_cubemap");
                    break;
                case Albedo.Scrolling_Texture_Uv:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("color_texture");
                    result.AddFloatParameter("u_speed");
                    result.AddFloatParameter("v_speed");
                    break;
                case Albedo.Texture_From_Misc:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("color_texture");
                    break;
                case Albedo.Four_Change_Color_Applying_To_Specular:
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
                case Albedo.Simple:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddFloat4ColorParameter("albedo_color");
                    break;
                case Albedo.Emblem:
                    result.AddSamplerWithoutXFormParameter("emblem_map", RenderMethodExtern.emblem_player_shoulder_texture);
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
                case Bump_Mapping.Detail_Masked:
                    result.AddSamplerParameter("bump_map");
                    result.AddSamplerParameter("bump_detail_map");
                    result.AddSamplerParameter("bump_detail_mask_map");
                    result.AddFloatParameter("bump_detail_coefficient");
                    result.AddBooleanParameter("invert_mask");
                    break;
                case Bump_Mapping.Detail_Plus_Detail_Masked:
                    result.AddSamplerWithoutXFormParameter("bump_map");
                    result.AddSamplerWithoutXFormParameter("bump_detail_map");
                    result.AddSamplerWithoutXFormParameter("bump_detail_mask_map");
                    result.AddSamplerWithoutXFormParameter("bump_detail_masked_map");
                    result.AddFloatParameter("bump_detail_coefficient");
                    result.AddFloatParameter("bump_detail_masked_coefficient");
                    break;
                case Bump_Mapping.Detail_Unorm:
                    result.AddSamplerParameter("bump_map");
                    result.AddSamplerParameter("bump_detail_map");
                    result.AddFloatParameter("bump_detail_coefficient");
                    break;
                case Bump_Mapping.Detail_Blend:
                    result.AddSamplerWithoutXFormParameter("bump_map");
                    result.AddSamplerWithoutXFormParameter("bump_detail_map");
                    result.AddSamplerWithoutXFormParameter("bump_detail_map2");
                    result.AddFloatParameter("blend_alpha");
                    break;
                case Bump_Mapping.Three_Detail_Blend:
                    result.AddSamplerWithoutXFormParameter("bump_map");
                    result.AddSamplerWithoutXFormParameter("bump_detail_map");
                    result.AddSamplerWithoutXFormParameter("bump_detail_map2");
                    result.AddSamplerWithoutXFormParameter("bump_detail_map3");
                    result.AddFloatParameter("blend_alpha");
                    break;
                case Bump_Mapping.Standard_Wrinkle:
                    result.AddSamplerWithoutXFormParameter("bump_map");
                    result.AddSamplerWithoutXFormParameter("wrinkle_normal");
                    result.AddSamplerWithoutXFormParameter("wrinkle_mask_a");
                    result.AddSamplerWithoutXFormParameter("wrinkle_mask_b");
                    result.AddFloat4ColorParameter("wrinkle_weights_a", RenderMethodExtern.none);
                    result.AddFloat4ColorParameter("wrinkle_weights_b", RenderMethodExtern.none);
                    break;
                case Bump_Mapping.Detail_Wrinkle:
                    result.AddSamplerWithoutXFormParameter("bump_map");
                    result.AddSamplerWithoutXFormParameter("bump_detail_map");
                    result.AddSamplerWithoutXFormParameter("wrinkle_normal");
                    result.AddSamplerWithoutXFormParameter("wrinkle_mask_a");
                    result.AddSamplerWithoutXFormParameter("wrinkle_mask_b");
                    result.AddFloat4ColorParameter("wrinkle_weights_a", RenderMethodExtern.none);
                    result.AddFloat4ColorParameter("wrinkle_weights_b", RenderMethodExtern.none);
                    break;
            }

            switch (alpha_test)
            {
                case Alpha_Test.None:
                    break;
                case Alpha_Test.Simple:
                    result.AddSamplerParameter("alpha_test_map");
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
                case Specular_Mask.Specular_Mask_From_Color_Texture:
                    result.AddSamplerParameter("specular_mask_texture");
                    break;
                case Specular_Mask.Specular_Mask_Mult_Diffuse:
                    result.AddSamplerWithoutXFormParameter("specular_mask_texture");
                    break;
            }

            switch (material_model)
            {
                case Material_Model.Diffuse_Only:
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddFloatParameter("approximate_specular_type");
                    break;
                case Material_Model.Cook_Torrance:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloat3ColorParameter("specular_tint");
                    result.AddFloat3ColorParameter("fresnel_color");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddBooleanParameter("order3_area_specular");
                    result.AddBooleanParameter("use_material_texture");
                    result.AddSamplerParameter("material_texture");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                    result.AddFloatParameter("albedo_blend");
                    result.AddFloatParameter("analytical_anti_shadow_control");
                    result.AddFloat3ColorParameter("specular_color_by_angle");
                    result.AddFloatParameter("fresnel_curve_steepness");
                    result.AddFloatParameter("analytical_roughness");
                    result.AddFloatParameter("material_texture_black_specular_multiplier");
                    result.AddFloatParameter("material_texture_black_roughness");
                    result.AddFloatParameter("approximate_specular_type");
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
                    result.AddBooleanParameter("no_dynamic_lights");
                    break;
                case Material_Model.None:
                    break;
                case Material_Model.Glass:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloatParameter("fresnel_coefficient");
                    result.AddFloatParameter("fresnel_curve_steepness");
                    result.AddFloatParameter("fresnel_curve_bias");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddBooleanParameter("no_dynamic_lights");
                    break;
                case Material_Model.Organism:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloat3ColorParameter("diffuse_tint");
                    result.AddFloatParameter("analytical_specular_coefficient");
                    result.AddFloatParameter("area_specular_coefficient");
                    result.AddFloat3ColorParameter("specular_tint");
                    result.AddFloatParameter("specular_power");
                    result.AddSamplerWithoutXFormParameter("specular_map");
                    result.AddFloatParameter("environment_map_coefficient");
                    result.AddFloat3ColorParameter("environment_map_tint");
                    result.AddFloatParameter("fresnel_curve_steepness");
                    result.AddFloatParameter("rim_coefficient");
                    result.AddFloat3ColorParameter("rim_tint");
                    result.AddFloatParameter("rim_power");
                    result.AddFloatParameter("rim_start");
                    result.AddFloatParameter("rim_maps_transition_ratio");
                    result.AddFloatParameter("ambient_coefficient");
                    result.AddFloat3ColorParameter("ambient_tint");
                    result.AddSamplerWithoutXFormParameter("occlusion_parameter_map");
                    result.AddFloatParameter("subsurface_coefficient");
                    result.AddFloat3ColorParameter("subsurface_tint");
                    result.AddFloatParameter("subsurface_propagation_bias");
                    result.AddFloatParameter("subsurface_normal_detail");
                    result.AddSamplerWithoutXFormParameter("subsurface_map");
                    result.AddFloatParameter("transparence_coefficient");
                    result.AddFloat3ColorParameter("transparence_tint");
                    result.AddFloatParameter("transparence_normal_bias");
                    result.AddFloatParameter("transparence_normal_detail");
                    result.AddSamplerWithoutXFormParameter("transparence_map");
                    result.AddFloat3ColorParameter("final_tint");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddFloat3ColorParameter("specular_color_by_angle");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloat3ColorParameter("fresnel_color");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("analytical_roughness");
                    result.AddBooleanParameter("use_material_texture");
                    result.AddSamplerWithoutXFormParameter("material_texture");
                    result.AddFloatParameter("material_texture_black_specular_multiplier");
                    result.AddFloatParameter("material_texture_black_roughness");
                    result.AddFloatParameter("albedo_blend");
                    result.AddSamplerWithoutXFormParameter("g_sampler_cooktorran_array");
                    result.AddFloatParameter("approximate_specular_type");
                    result.AddFloatParameter("rim_width");
                    break;
                case Material_Model.Single_Lobe_Phong:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddFloat3ColorParameter("specular_tint");
                    result.AddBooleanParameter("order3_area_specular");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddFloatParameter("analytical_anti_shadow_control");
                    break;
                case Material_Model.Car_Paint:
                    result.AddBooleanParameter("use_material_texture0");
                    result.AddBooleanParameter("use_material_texture1");
                    result.AddSamplerWithoutXFormParameter("material_texture");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                    result.AddSamplerWithoutXFormParameter("bump_detail_map0");
                    result.AddFloatParameter("bump_detail_map0_blend_factor");
                    result.AddFloatParameter("diffuse_coefficient0");
                    result.AddFloatParameter("specular_coefficient0");
                    result.AddFloat3ColorParameter("specular_tint0");
                    result.AddFloat3ColorParameter("fresnel_color0");
                    result.AddFloatParameter("fresnel_power0");
                    result.AddFloatParameter("albedo_blend0");
                    result.AddFloatParameter("roughness0");
                    result.AddFloatParameter("area_specular_contribution0");
                    result.AddFloatParameter("analytical_specular_contribution0");
                    result.AddBooleanParameter("order3_area_specular0");
                    result.AddFloatParameter("diffuse_coefficient1");
                    result.AddFloatParameter("specular_coefficient1");
                    result.AddFloat3ColorParameter("specular_tint1");
                    result.AddFloat3ColorParameter("fresnel_color1");
                    result.AddFloat3ColorParameter("fresnel_color_environment1");
                    result.AddFloatParameter("fresnel_power1");
                    result.AddFloatParameter("albedo_blend1");
                    result.AddFloatParameter("roughness1");
                    result.AddFloatParameter("area_specular_contribution1");
                    result.AddFloatParameter("analytical_specular_contribution1");
                    result.AddFloatParameter("environment_map_specular_contribution1");
                    result.AddBooleanParameter("order3_area_specular1");
                    result.AddFloatParameter("rim_fresnel_coefficient1");
                    result.AddFloat3ColorParameter("rim_fresnel_color1");
                    result.AddFloatParameter("rim_fresnel_power1");
                    result.AddFloatParameter("rim_fresnel_albedo_blend1");
                    break;
                case Material_Model.Cook_Torrance_Custom_Cube:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloat3ColorParameter("specular_tint");
                    result.AddFloat3ColorParameter("fresnel_color");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddBooleanParameter("order3_area_specular");
                    result.AddBooleanParameter("use_material_texture");
                    result.AddSamplerParameter("material_texture");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                    result.AddFloatParameter("albedo_blend");
                    result.AddFloatParameter("analytical_anti_shadow_control");
                    result.AddSamplerWithoutXFormParameter("custom_cube");
                    break;
                case Material_Model.Cook_Torrance_Pbr_Maps:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloat3ColorParameter("specular_tint");
                    result.AddFloat3ColorParameter("fresnel_color");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddBooleanParameter("order3_area_specular");
                    result.AddBooleanParameter("use_material_texture");
                    result.AddSamplerParameter("material_texture");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                    result.AddFloatParameter("albedo_blend");
                    result.AddFloatParameter("analytical_anti_shadow_control");
                    result.AddSamplerWithoutXFormParameter("spec_tint_map");
                    break;
                case Material_Model.Cook_Torrance_Two_Color_Spec_Tint:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloat3ColorParameter("specular_tint");
                    result.AddFloat3ColorParameter("fresnel_color");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddBooleanParameter("order3_area_specular");
                    result.AddBooleanParameter("use_material_texture");
                    result.AddSamplerParameter("material_texture");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                    result.AddFloatParameter("albedo_blend");
                    result.AddFloatParameter("analytical_anti_shadow_control");
                    result.AddSamplerWithoutXFormParameter("spec_blend_map");
                    result.AddFloat3ColorParameter("specular_second_tint");
                    break;
                case Material_Model.Two_Lobe_Phong_Tint_Map:
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
                    result.AddSamplerWithoutXFormParameter("normal_specular_tint_map");
                    result.AddSamplerWithoutXFormParameter("glancing_specular_tint_map");
                    break;
                case Material_Model.Cook_Torrance_Scrolling_Cube_Mask:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloat3ColorParameter("specular_tint");
                    result.AddFloat3ColorParameter("fresnel_color");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddBooleanParameter("order3_area_specular");
                    result.AddBooleanParameter("use_material_texture");
                    result.AddSamplerWithoutXFormParameter("material_texture");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                    result.AddFloatParameter("albedo_blend");
                    result.AddFloatParameter("analytical_anti_shadow_control");
                    result.AddSamplerWithoutXFormParameter("tint_blend_mask_cubemap");
                    result.AddFloat3ColorParameter("specular_second_tint");
                    break;
                case Material_Model.Cook_Torrance_Rim_Fresnel:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloat3ColorParameter("specular_tint");
                    result.AddFloat3ColorParameter("fresnel_color");
                    result.AddBooleanParameter("use_fresnel_color_environment");
                    result.AddFloat3ColorParameter("fresnel_color_environment");
                    result.AddFloatParameter("fresnel_power");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddBooleanParameter("order3_area_specular");
                    result.AddBooleanParameter("use_material_texture");
                    result.AddSamplerParameter("material_texture");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                    result.AddBooleanParameter("albedo_blend_with_specular_tint");
                    result.AddFloatParameter("albedo_blend");
                    result.AddFloatParameter("analytical_anti_shadow_control");
                    result.AddFloatParameter("rim_fresnel_coefficient");
                    result.AddFloat3ColorParameter("rim_fresnel_color");
                    result.AddFloatParameter("rim_fresnel_power");
                    result.AddFloatParameter("rim_fresnel_albedo_blend");
                    break;
                case Material_Model.Cook_Torrance_Scrolling_Cube:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloat3ColorParameter("fresnel_color");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddBooleanParameter("order3_area_specular");
                    result.AddBooleanParameter("use_material_texture");
                    result.AddSamplerWithoutXFormParameter("material_texture");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                    result.AddFloatParameter("albedo_blend");
                    result.AddFloatParameter("analytical_anti_shadow_control");
                    result.AddSamplerWithoutXFormParameter("spec_tint_cubemap");
                    break;
                case Material_Model.Cook_Torrance_From_Albedo:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloat3ColorParameter("fresnel_color");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddBooleanParameter("order3_area_specular");
                    result.AddBooleanParameter("use_material_texture");
                    result.AddSamplerParameter("material_texture");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                    result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                    result.AddFloatParameter("albedo_blend");
                    result.AddFloatParameter("analytical_anti_shadow_control");
                    break;
                case Material_Model.Hair:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloat3ColorParameter("diffuse_tint");
                    result.AddFloatParameter("analytical_specular_coefficient");
                    result.AddFloatParameter("area_specular_coefficient");
                    result.AddFloat3ColorParameter("specular_tint");
                    result.AddFloatParameter("specular_power");
                    result.AddSamplerWithoutXFormParameter("specular_map");
                    result.AddSamplerWithoutXFormParameter("specular_shift_map");
                    result.AddSamplerWithoutXFormParameter("specular_noise_map");
                    result.AddFloatParameter("environment_map_coefficient");
                    result.AddFloat3ColorParameter("environment_map_tint");
                    result.AddFloat3ColorParameter("final_tint");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddFloatParameter("roughness");
                    break;
                case Material_Model.Cook_Torrance_Reach:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloat3ColorParameter("specular_color_by_angle");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloat3ColorParameter("specular_tint");
                    result.AddFloat3ColorParameter("fresnel_color");
                    result.AddFloatParameter("fresnel_curve_steepness");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("analytical_roughness");
                    result.AddBooleanParameter("use_material_texture");
                    result.AddSamplerWithoutXFormParameter("material_texture");
                    result.AddFloatParameter("material_texture_black_specular_multiplier");
                    result.AddFloatParameter("material_texture_black_roughness");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddFloatParameter("albedo_blend");
                    result.AddFloatParameter("approximate_specular_type");
                    break;
                case Material_Model.Two_Lobe_Phong_Reach:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloat3ColorParameter("specular_color_by_angle");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloatParameter("normal_specular_power");
                    result.AddFloat3ColorParameter("normal_specular_tint");
                    result.AddFloatParameter("glancing_specular_power");
                    result.AddFloat3ColorParameter("glancing_specular_tint");
                    result.AddFloatParameter("fresnel_curve_steepness");
                    result.AddFloatParameter("area_specular_contribution");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddFloatParameter("roughness");
                    result.AddFloatParameter("analytical_roughness");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddFloatParameter("albedo_specular_tint_blend");
                    result.AddFloatParameter("approximate_specular_type");
                    result.AddFloatParameter("analytical_power");
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
                case Environment_Mapping.Custom_Map:
                    result.AddSamplerWithoutXFormParameter("environment_map");
                    result.AddFloat3ColorParameter("env_tint_color");
                    result.AddFloatParameter("env_roughness_scale");
                    break;
                case Environment_Mapping.From_Flat_Texture_As_Cubemap:
                    result.AddSamplerWithoutXFormParameter("flat_environment_map");
                    result.AddFloat3ColorParameter("env_tint_color");
                    result.AddFloat4ColorParameter("flat_envmap_matrix_x", RenderMethodExtern.flat_envmap_matrix_x);
                    result.AddFloat4ColorParameter("flat_envmap_matrix_y", RenderMethodExtern.flat_envmap_matrix_y);
                    result.AddFloat4ColorParameter("flat_envmap_matrix_z", RenderMethodExtern.flat_envmap_matrix_z);
                    result.AddFloatParameter("hemisphere_percentage");
                    result.AddFloat4ColorParameter("env_bloom_override");
                    result.AddFloatParameter("env_bloom_override_intensity");
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
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat4ColorParameter("channel_a");
                    result.AddFloat4ColorParameter("channel_b");
                    result.AddFloat4ColorParameter("channel_c");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination.Plasma:
                    result.AddSamplerParameter("noise_map_a");
                    result.AddSamplerParameter("noise_map_b");
                    result.AddFloat4ColorParameter("color_medium");
                    result.AddFloat4ColorParameter("color_wide");
                    result.AddFloat4ColorParameter("color_sharp");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddSamplerParameter("alpha_mask_map");
                    result.AddFloatParameter("thinness_medium");
                    result.AddFloatParameter("thinness_wide");
                    result.AddFloatParameter("thinness_sharp");
                    break;
                case Self_Illumination.From_Diffuse:
                    result.AddFloat3ColorParameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination.Illum_Detail:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddSamplerParameter("self_illum_detail_map");
                    result.AddFloat3ColorParameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination.Meter:
                    result.AddSamplerParameter("meter_map");
                    result.AddFloat3ColorParameter("meter_color_off");
                    result.AddFloat3ColorParameter("meter_color_on");
                    result.AddFloatParameter("meter_value");
                    break;
                case Self_Illumination.Self_Illum_Times_Diffuse:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat3ColorParameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("primary_change_color_blend");
                    break;
                case Self_Illumination.Simple_With_Alpha_Mask:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat3ColorParameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination.Simple_Four_Change_Color:
                    result.AddSamplerWithoutXFormParameter("self_illum_map");
                    result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_quaternary);
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination.Illum_Detail_World_Space_Four_Cc:
                    result.AddSamplerWithoutXFormParameter("self_illum_map");
                    result.AddSamplerWithoutXFormParameter("self_illum_detail_map");
                    result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_quaternary);
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloat4ColorParameter("self_illum_obj_bounding_sphere");
                    break;
                case Self_Illumination.Illum_Change_Color:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("primary_change_color_blend");
                    break;
                case Self_Illumination.Multilayer_Additive:
                    result.AddSamplerWithoutXFormParameter("self_illum_map");
                    result.AddFloat3ColorParameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("layer_depth");
                    result.AddFloatParameter("layer_contrast");
                    result.AddIntegerParameter("layers_of_4");
                    result.AddFloatParameter("texcoord_aspect_ratio");
                    result.AddFloatParameter("depth_darken");
                    break;
                case Self_Illumination.Palettized_Plasma:
                    result.AddSamplerWithoutXFormParameter("noise_map_a");
                    result.AddSamplerWithoutXFormParameter("noise_map_b");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerWithoutXFormParameter("alpha_mask_map");
                    result.AddFloatParameter("alpha_modulation_factor");
                    result.AddSamplerWithoutXFormParameter("depth_buffer", RenderMethodExtern.texture_global_target_z);
                    result.AddFloatParameter("depth_fade_range");
                    result.AddFloat3ColorParameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("v_coordinate");
                    result.AddFloatParameter("global_depth_constants", RenderMethodExtern.global_depth_constants);
                    result.AddFloatParameter("global_camera_forward", RenderMethodExtern.global_camera_forward);
                    break;
                case Self_Illumination.Change_Color:
                    result.AddSamplerWithoutXFormParameter("self_illum_map");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    break;
                case Self_Illumination.Change_Color_Detail:
                    result.AddSamplerWithoutXFormParameter("self_illum_map");
                    result.AddSamplerWithoutXFormParameter("self_illum_detail_map");
                    result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_primary);
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
                case Blend_Mode.Pre_Multiplied_Alpha:
                    break;
            }

            switch (parallax)
            {
                case Parallax.Off:
                    break;
                case Parallax.Simple:
                    result.AddSamplerParameter("height_map");
                    result.AddFloatParameter("height_scale");
                    break;
                case Parallax.Interpolated:
                    result.AddSamplerParameter("height_map");
                    result.AddFloatParameter("height_scale");
                    break;
                case Parallax.Simple_Detail:
                    result.AddSamplerParameter("height_map");
                    result.AddFloatParameter("height_scale");
                    result.AddSamplerParameter("height_scale_map");
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

            switch (distortion)
            {
                case Distortion.Off:
                    break;
                case Distortion.On:
                    result.AddSamplerWithoutXFormParameter("distort_map");
                    result.AddFloatParameter("distort_scale");
                    result.AddFloatParameter("distort_fadeoff");
                    result.AddBooleanParameter("distort_selfonly");
                    break;
            }

            switch (soft_fade)
            {
                case Soft_Fade.Off:
                    break;
                case Soft_Fade.On:
                    result.AddSamplerWithoutXFormParameter("depth_map", RenderMethodExtern.texture_global_target_z);
                    result.AddBooleanParameter("use_soft_fresnel");
                    result.AddFloatParameter("soft_fresnel_power");
                    result.AddBooleanParameter("use_soft_z");
                    result.AddFloatParameter("soft_z_range");
                    result.AddFloatParameter("screen_params", RenderMethodExtern.screen_constants);
                    break;
            }

            switch (misc_attr_animation)
            {
                case Misc_Attr_Animation.Off:
                    break;
                case Misc_Attr_Animation.Scrolling_Cube:
                    result.AddIntegerParameter("misc_attr_animation_option");
                    result.AddFloatParameter("scrolling_axis_x");
                    result.AddFloatParameter("scrolling_axis_y");
                    result.AddFloatParameter("scrolling_axis_z");
                    result.AddFloatParameter("scrolling_speed");
                    break;
                case Misc_Attr_Animation.Scrolling_Projected:
                    result.AddIntegerParameter("misc_attr_animation_option");
                    result.AddFloatParameter("object_center_x");
                    result.AddFloatParameter("object_center_y");
                    result.AddFloatParameter("object_center_z");
                    result.AddFloatParameter("plane_u_x");
                    result.AddFloatParameter("plane_u_y");
                    result.AddFloatParameter("plane_u_z");
                    result.AddFloatParameter("plane_v_x");
                    result.AddFloatParameter("plane_v_y");
                    result.AddFloatParameter("plane_v_z");
                    result.AddFloatParameter("scale_u");
                    result.AddFloatParameter("scale_v");
                    result.AddFloatParameter("translate_u");
                    result.AddFloatParameter("translate_v");
                    result.AddFloatParameter("speed_u");
                    result.AddFloatParameter("speed_v");
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
                case Wetness.Simple:
                    result.AddFloatParameter("wet_material_dim_coefficient");
                    result.AddFloat3ColorParameter("wet_material_dim_tint");
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

            switch (alpha_blend_source)
            {
                case Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel:
                    break;
                case Alpha_Blend_Source.From_Albedo_Alpha:
                    result.AddFloatParameter("opacity_fresnel_coefficient");
                    result.AddFloatParameter("opacity_fresnel_curve_steepness");
                    result.AddFloatParameter("opacity_fresnel_curve_bias");
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                    result.AddSamplerWithoutXFormParameter("opacity_texture");
                    result.AddFloatParameter("opacity_fresnel_coefficient");
                    result.AddFloatParameter("opacity_fresnel_curve_steepness");
                    result.AddFloatParameter("opacity_fresnel_curve_bias");
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                    result.AddSamplerWithoutXFormParameter("opacity_texture");
                    result.AddFloatParameter("opacity_fresnel_coefficient");
                    result.AddFloatParameter("opacity_fresnel_curve_steepness");
                    result.AddFloatParameter("opacity_fresnel_curve_bias");
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                    result.AddSamplerWithoutXFormParameter("opacity_texture");
                    result.AddFloatParameter("opacity_fresnel_coefficient");
                    result.AddFloatParameter("opacity_fresnel_curve_steepness");
                    result.AddFloatParameter("opacity_fresnel_curve_bias");
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
                case Albedo.Two_Change_Color_Anim_Overlay:
                    break;
                case Albedo.Chameleon:
                    break;
                case Albedo.Two_Change_Color_Chameleon:
                    break;
                case Albedo.Chameleon_Masked:
                    break;
                case Albedo.Color_Mask_Hard_Light:
                    break;
                case Albedo.Two_Change_Color_Tex_Overlay:
                    break;
                case Albedo.Chameleon_Albedo_Masked:
                    break;
                case Albedo.Custom_Cube:
                    break;
                case Albedo.Two_Color:
                    break;
                case Albedo.Scrolling_Cube_Mask:
                    break;
                case Albedo.Scrolling_Cube:
                    break;
                case Albedo.Scrolling_Texture_Uv:
                    break;
                case Albedo.Texture_From_Misc:
                    break;
                case Albedo.Four_Change_Color_Applying_To_Specular:
                    break;
                case Albedo.Simple:
                    break;
                case Albedo.Emblem:
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
                case Bump_Mapping.Detail_Masked:
                    break;
                case Bump_Mapping.Detail_Plus_Detail_Masked:
                    break;
                case Bump_Mapping.Detail_Unorm:
                    break;
                case Bump_Mapping.Detail_Blend:
                    break;
                case Bump_Mapping.Three_Detail_Blend:
                    break;
                case Bump_Mapping.Standard_Wrinkle:
                    break;
                case Bump_Mapping.Detail_Wrinkle:
                    break;
            }

            switch (alpha_test)
            {
                case Alpha_Test.None:
                    break;
                case Alpha_Test.Simple:
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
                case Specular_Mask.Specular_Mask_From_Color_Texture:
                    break;
                case Specular_Mask.Specular_Mask_Mult_Diffuse:
                    break;
            }

            switch (material_model)
            {
                case Material_Model.Diffuse_Only:
                    break;
                case Material_Model.Cook_Torrance:
                    break;
                case Material_Model.Two_Lobe_Phong:
                    break;
                case Material_Model.Foliage:
                    break;
                case Material_Model.None:
                    break;
                case Material_Model.Glass:
                    break;
                case Material_Model.Organism:
                    break;
                case Material_Model.Single_Lobe_Phong:
                    break;
                case Material_Model.Car_Paint:
                    break;
                case Material_Model.Cook_Torrance_Custom_Cube:
                    break;
                case Material_Model.Cook_Torrance_Pbr_Maps:
                    break;
                case Material_Model.Cook_Torrance_Two_Color_Spec_Tint:
                    break;
                case Material_Model.Two_Lobe_Phong_Tint_Map:
                    break;
                case Material_Model.Cook_Torrance_Scrolling_Cube_Mask:
                    break;
                case Material_Model.Cook_Torrance_Rim_Fresnel:
                    break;
                case Material_Model.Cook_Torrance_Scrolling_Cube:
                    break;
                case Material_Model.Cook_Torrance_From_Albedo:
                    break;
                case Material_Model.Hair:
                    break;
                case Material_Model.Cook_Torrance_Reach:
                    break;
                case Material_Model.Two_Lobe_Phong_Reach:
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
                case Environment_Mapping.Custom_Map:
                    break;
                case Environment_Mapping.From_Flat_Texture_As_Cubemap:
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
                case Self_Illumination.Simple_With_Alpha_Mask:
                    break;
                case Self_Illumination.Simple_Four_Change_Color:
                    break;
                case Self_Illumination.Illum_Detail_World_Space_Four_Cc:
                    break;
                case Self_Illumination.Illum_Change_Color:
                    break;
                case Self_Illumination.Multilayer_Additive:
                    break;
                case Self_Illumination.Palettized_Plasma:
                    break;
                case Self_Illumination.Change_Color:
                    break;
                case Self_Illumination.Change_Color_Detail:
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
                case Blend_Mode.Pre_Multiplied_Alpha:
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

            switch (distortion)
            {
                case Distortion.Off:
                    break;
                case Distortion.On:
                    break;
            }

            switch (soft_fade)
            {
                case Soft_Fade.Off:
                    break;
                case Soft_Fade.On:
                    break;
            }

            switch (misc_attr_animation)
            {
                case Misc_Attr_Animation.Off:
                    break;
                case Misc_Attr_Animation.Scrolling_Cube:
                    result.AddIntegerVertexParameter("misc_attr_animation_option");
                    result.AddFloatVertexParameter("scrolling_axis_x");
                    result.AddFloatVertexParameter("scrolling_axis_y");
                    result.AddFloatVertexParameter("scrolling_axis_z");
                    result.AddFloatVertexParameter("scrolling_speed");
                    break;
                case Misc_Attr_Animation.Scrolling_Projected:
                    result.AddIntegerVertexParameter("misc_attr_animation_option");
                    result.AddFloatVertexParameter("object_center_x");
                    result.AddFloatVertexParameter("object_center_y");
                    result.AddFloatVertexParameter("object_center_z");
                    result.AddFloatVertexParameter("plane_u_x");
                    result.AddFloatVertexParameter("plane_u_y");
                    result.AddFloatVertexParameter("plane_u_z");
                    result.AddFloatVertexParameter("plane_v_x");
                    result.AddFloatVertexParameter("plane_v_y");
                    result.AddFloatVertexParameter("plane_v_z");
                    result.AddFloatVertexParameter("scale_u");
                    result.AddFloatVertexParameter("scale_v");
                    result.AddFloatVertexParameter("translate_u");
                    result.AddFloatVertexParameter("translate_v");
                    result.AddFloatVertexParameter("speed_u");
                    result.AddFloatVertexParameter("speed_v");
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
                case Wetness.Simple:
                    break;
                case Wetness.Ripples:
                    break;
            }

            switch (alpha_blend_source)
            {
                case Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel:
                    break;
                case Alpha_Blend_Source.From_Albedo_Alpha:
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                    break;
                case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
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
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddSamplerParameter("detail_map2");
                        result.AddFloatParameter("blend_alpha");
                        rmopName = @"shaders\shader_options\albedo_detail_blend";
                        break;
                    case Albedo.Constant_Color:
                        result.AddFloat4ColorParameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_constant";
                        break;
                    case Albedo.Two_Change_Color:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddSamplerParameter("change_color_map");
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                        result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                        result.AddSamplerWithoutXFormParameter("camouflage_change_color_map");
                        result.AddFloatParameter("camouflage_scale");
                        rmopName = @"shaders\shader_options\albedo_two_change_color";
                        break;
                    case Albedo.Four_Change_Color:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddSamplerParameter("change_color_map");
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                        result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                        result.AddFloat3ColorParameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary);
                        result.AddFloat3ColorParameter("quaternary_change_color", RenderMethodExtern.object_change_color_quaternary);
                        result.AddSamplerWithoutXFormParameter("camouflage_change_color_map");
                        result.AddFloatParameter("camouflage_scale");
                        rmopName = @"shaders\shader_options\albedo_four_change_color";
                        break;
                    case Albedo.Three_Detail_Blend:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddSamplerParameter("detail_map2");
                        result.AddSamplerParameter("detail_map3");
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
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddSamplerParameter("detail_map2");
                        rmopName = @"shaders\shader_options\albedo_two_detail";
                        break;
                    case Albedo.Color_Mask:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddSamplerParameter("color_mask_map");
                        result.AddFloat4ColorParameter("albedo_color");
                        result.AddFloat4ColorParameter("albedo_color2");
                        result.AddFloat4ColorParameter("albedo_color3");
                        result.AddFloat3ColorParameter("neutral_gray");
                        rmopName = @"shaders\shader_options\albedo_color_mask";
                        break;
                    case Albedo.Two_Detail_Black_Point:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddSamplerParameter("detail_map2");
                        rmopName = @"shaders\shader_options\albedo_two_detail_black_point";
                        break;
                    case Albedo.Two_Change_Color_Anim_Overlay:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddSamplerWithoutXFormParameter("change_color_map");
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                        result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                        result.AddFloat3ColorParameter("primary_change_color_anim");
                        result.AddFloat3ColorParameter("secondary_change_color_anim");
                        rmopName = @"shaders\shader_options\albedo_two_change_color_anim";
                        break;
                    case Albedo.Chameleon:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddFloat3ColorParameter("chameleon_color0");
                        result.AddFloat3ColorParameter("chameleon_color1");
                        result.AddFloat3ColorParameter("chameleon_color2");
                        result.AddFloat3ColorParameter("chameleon_color3");
                        result.AddFloatParameter("chameleon_color_offset1");
                        result.AddFloatParameter("chameleon_color_offset2");
                        result.AddFloatParameter("chameleon_fresnel_power");
                        rmopName = @"shaders\shader_options\albedo_chameleon";
                        break;
                    case Albedo.Two_Change_Color_Chameleon:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddSamplerWithoutXFormParameter("change_color_map");
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                        result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                        result.AddFloat3ColorParameter("primary_change_color_anim");
                        result.AddFloat3ColorParameter("secondary_change_color_anim");
                        result.AddFloat3ColorParameter("chameleon_color0");
                        result.AddFloat3ColorParameter("chameleon_color1");
                        result.AddFloat3ColorParameter("chameleon_color2");
                        result.AddFloat3ColorParameter("chameleon_color3");
                        result.AddFloatParameter("chameleon_color_offset1");
                        result.AddFloatParameter("chameleon_color_offset2");
                        result.AddFloatParameter("chameleon_fresnel_power");
                        rmopName = @"shaders\shader_options\albedo_two_change_color_chameleon";
                        break;
                    case Albedo.Chameleon_Masked:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddSamplerParameter("chameleon_mask_map");
                        result.AddFloat3ColorParameter("chameleon_color0");
                        result.AddFloat3ColorParameter("chameleon_color1");
                        result.AddFloat3ColorParameter("chameleon_color2");
                        result.AddFloat3ColorParameter("chameleon_color3");
                        result.AddFloatParameter("chameleon_color_offset1");
                        result.AddFloatParameter("chameleon_color_offset2");
                        result.AddFloatParameter("chameleon_fresnel_power");
                        rmopName = @"shaders\shader_options\albedo_chameleon_masked";
                        break;
                    case Albedo.Color_Mask_Hard_Light:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddSamplerWithoutXFormParameter("color_mask_map");
                        result.AddFloat4ColorParameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_color_mask_hard_light";
                        break;
                    case Albedo.Two_Change_Color_Tex_Overlay:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddSamplerWithoutXFormParameter("change_color_map");
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                        result.AddSamplerWithoutXFormParameter("secondary_change_color_map");
                        rmopName = @"shaders\shader_options\albedo_two_change_color_tex_overlay";
                        break;
                    case Albedo.Chameleon_Albedo_Masked:
                        result.AddSamplerParameter("base_map");
                        result.AddFloat4ColorParameter("albedo_color");
                        result.AddSamplerParameter("base_masked_map");
                        result.AddFloat4ColorParameter("albedo_masked_color");
                        result.AddSamplerParameter("chameleon_mask_map");
                        result.AddFloat3ColorParameter("chameleon_color0");
                        result.AddFloat3ColorParameter("chameleon_color1");
                        result.AddFloat3ColorParameter("chameleon_color2");
                        result.AddFloat3ColorParameter("chameleon_color3");
                        result.AddFloatParameter("chameleon_color_offset1");
                        result.AddFloatParameter("chameleon_color_offset2");
                        result.AddFloatParameter("chameleon_fresnel_power");
                        rmopName = @"shaders\shader_options\albedo_chameleon_albedo_masked";
                        break;
                    case Albedo.Custom_Cube:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("custom_cube");
                        result.AddFloat4ColorParameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_custom_cube";
                        break;
                    case Albedo.Two_Color:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddFloat4ColorParameter("albedo_color");
                        result.AddSamplerWithoutXFormParameter("blend_map");
                        result.AddFloat4ColorParameter("albedo_second_color");
                        rmopName = @"shaders\shader_options\albedo_two_color";
                        break;
                    case Albedo.Scrolling_Cube_Mask:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddFloat4ColorParameter("albedo_color");
                        result.AddSamplerWithoutXFormParameter("color_blend_mask_cubemap");
                        result.AddFloat4ColorParameter("albedo_second_color");
                        rmopName = @"shaders\shader_options\albedo_scrolling_cube_mask";
                        break;
                    case Albedo.Scrolling_Cube:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddSamplerWithoutXFormParameter("color_cubemap");
                        rmopName = @"shaders\shader_options\albedo_scrolling_cube";
                        break;
                    case Albedo.Scrolling_Texture_Uv:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("color_texture");
                        result.AddFloatParameter("u_speed");
                        result.AddFloatParameter("v_speed");
                        rmopName = @"shaders\shader_options\albedo_scrolling_texture_uv";
                        break;
                    case Albedo.Texture_From_Misc:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("color_texture");
                        rmopName = @"shaders\shader_options\albedo_texture_from_misc";
                        break;
                    case Albedo.Four_Change_Color_Applying_To_Specular:
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
                    case Albedo.Simple:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddFloat4ColorParameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_simple";
                        break;
                    case Albedo.Emblem:
                        result.AddSamplerWithoutXFormParameter("emblem_map", RenderMethodExtern.emblem_player_shoulder_texture);
                        rmopName = @"shaders\shader_options\albedo_emblem";
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
                    case Bump_Mapping.Detail_Masked:
                        result.AddSamplerParameter("bump_map");
                        result.AddSamplerParameter("bump_detail_map");
                        result.AddSamplerParameter("bump_detail_mask_map");
                        result.AddFloatParameter("bump_detail_coefficient");
                        result.AddBooleanParameter("invert_mask");
                        rmopName = @"shaders\shader_options\bump_detail_masked";
                        break;
                    case Bump_Mapping.Detail_Plus_Detail_Masked:
                        result.AddSamplerWithoutXFormParameter("bump_map");
                        result.AddSamplerWithoutXFormParameter("bump_detail_map");
                        result.AddSamplerWithoutXFormParameter("bump_detail_mask_map");
                        result.AddSamplerWithoutXFormParameter("bump_detail_masked_map");
                        result.AddFloatParameter("bump_detail_coefficient");
                        result.AddFloatParameter("bump_detail_masked_coefficient");
                        rmopName = @"shaders\shader_options\bump_detail_plus_detail_masked";
                        break;
                    case Bump_Mapping.Detail_Unorm:
                        result.AddSamplerParameter("bump_map");
                        result.AddSamplerParameter("bump_detail_map");
                        result.AddFloatParameter("bump_detail_coefficient");
                        rmopName = @"shaders\shader_options\bump_detail_unorm";
                        break;
                    case Bump_Mapping.Detail_Blend:
                        result.AddSamplerWithoutXFormParameter("bump_map");
                        result.AddSamplerWithoutXFormParameter("bump_detail_map");
                        result.AddSamplerWithoutXFormParameter("bump_detail_map2");
                        result.AddFloatParameter("blend_alpha");
                        rmopName = @"shaders\shader_options\bump_detail_blend";
                        break;
                    case Bump_Mapping.Three_Detail_Blend:
                        result.AddSamplerWithoutXFormParameter("bump_map");
                        result.AddSamplerWithoutXFormParameter("bump_detail_map");
                        result.AddSamplerWithoutXFormParameter("bump_detail_map2");
                        result.AddSamplerWithoutXFormParameter("bump_detail_map3");
                        result.AddFloatParameter("blend_alpha");
                        rmopName = @"shaders\shader_options\bump_three_detail_blend";
                        break;
                    case Bump_Mapping.Standard_Wrinkle:
                        result.AddSamplerWithoutXFormParameter("bump_map");
                        result.AddSamplerWithoutXFormParameter("wrinkle_normal");
                        result.AddSamplerWithoutXFormParameter("wrinkle_mask_a");
                        result.AddSamplerWithoutXFormParameter("wrinkle_mask_b");
                        result.AddFloat4ColorParameter("wrinkle_weights_a", RenderMethodExtern.none);
                        result.AddFloat4ColorParameter("wrinkle_weights_b", RenderMethodExtern.none);
                        rmopName = @"shaders\shader_options\bump_default_wrinkle";
                        break;
                    case Bump_Mapping.Detail_Wrinkle:
                        result.AddSamplerWithoutXFormParameter("bump_map");
                        result.AddSamplerWithoutXFormParameter("bump_detail_map");
                        result.AddSamplerWithoutXFormParameter("wrinkle_normal");
                        result.AddSamplerWithoutXFormParameter("wrinkle_mask_a");
                        result.AddSamplerWithoutXFormParameter("wrinkle_mask_b");
                        result.AddFloat4ColorParameter("wrinkle_weights_a", RenderMethodExtern.none);
                        result.AddFloat4ColorParameter("wrinkle_weights_b", RenderMethodExtern.none);
                        rmopName = @"shaders\shader_options\bump_detail_wrinkle";
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
                        result.AddSamplerParameter("alpha_test_map");
                        rmopName = @"shaders\shader_options\alpha_test_on";
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
                    case Specular_Mask.Specular_Mask_From_Color_Texture:
                        result.AddSamplerParameter("specular_mask_texture");
                        rmopName = @"shaders\shader_options\specular_mask_from_texture";
                        break;
                    case Specular_Mask.Specular_Mask_Mult_Diffuse:
                        result.AddSamplerWithoutXFormParameter("specular_mask_texture");
                        rmopName = @"shaders\shader_options\specular_mask_mult_diffuse";
                        break;
                }
            }

            if (methodName == "material_model")
            {
                optionName = ((Material_Model)option).ToString();

                switch ((Material_Model)option)
                {
                    case Material_Model.Diffuse_Only:
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("approximate_specular_type");
                        rmopName = @"shaders\shader_options\material_diffuse_only";
                        break;
                    case Material_Model.Cook_Torrance:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint");
                        result.AddFloat3ColorParameter("fresnel_color");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddFloat3ColorParameter("specular_color_by_angle");
                        result.AddFloatParameter("fresnel_curve_steepness");
                        result.AddFloatParameter("analytical_roughness");
                        result.AddFloatParameter("material_texture_black_specular_multiplier");
                        result.AddFloatParameter("material_texture_black_roughness");
                        result.AddFloatParameter("approximate_specular_type");
                        rmopName = @"shaders\shader_options\material_cook_torrance_option";
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
                        result.AddBooleanParameter("no_dynamic_lights");
                        rmopName = @"shaders\shader_options\material_foliage";
                        break;
                    case Material_Model.None:
                        break;
                    case Material_Model.Glass:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloatParameter("fresnel_coefficient");
                        result.AddFloatParameter("fresnel_curve_steepness");
                        result.AddFloatParameter("fresnel_curve_bias");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddBooleanParameter("no_dynamic_lights");
                        rmopName = @"shaders\shader_options\glass_material";
                        break;
                    case Material_Model.Organism:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloat3ColorParameter("diffuse_tint");
                        result.AddFloatParameter("analytical_specular_coefficient");
                        result.AddFloatParameter("area_specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint");
                        result.AddFloatParameter("specular_power");
                        result.AddSamplerWithoutXFormParameter("specular_map");
                        result.AddFloatParameter("environment_map_coefficient");
                        result.AddFloat3ColorParameter("environment_map_tint");
                        result.AddFloatParameter("fresnel_curve_steepness");
                        result.AddFloatParameter("rim_coefficient");
                        result.AddFloat3ColorParameter("rim_tint");
                        result.AddFloatParameter("rim_power");
                        result.AddFloatParameter("rim_start");
                        result.AddFloatParameter("rim_maps_transition_ratio");
                        result.AddFloatParameter("ambient_coefficient");
                        result.AddFloat3ColorParameter("ambient_tint");
                        result.AddSamplerWithoutXFormParameter("occlusion_parameter_map");
                        result.AddFloatParameter("subsurface_coefficient");
                        result.AddFloat3ColorParameter("subsurface_tint");
                        result.AddFloatParameter("subsurface_propagation_bias");
                        result.AddFloatParameter("subsurface_normal_detail");
                        result.AddSamplerWithoutXFormParameter("subsurface_map");
                        result.AddFloatParameter("transparence_coefficient");
                        result.AddFloat3ColorParameter("transparence_tint");
                        result.AddFloatParameter("transparence_normal_bias");
                        result.AddFloatParameter("transparence_normal_detail");
                        result.AddSamplerWithoutXFormParameter("transparence_map");
                        result.AddFloat3ColorParameter("final_tint");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloat3ColorParameter("specular_color_by_angle");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("fresnel_color");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("analytical_roughness");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerWithoutXFormParameter("material_texture");
                        result.AddFloatParameter("material_texture_black_specular_multiplier");
                        result.AddFloatParameter("material_texture_black_roughness");
                        result.AddFloatParameter("albedo_blend");
                        result.AddSamplerWithoutXFormParameter("g_sampler_cooktorran_array");
                        result.AddFloatParameter("approximate_specular_type");
                        result.AddFloatParameter("rim_width");
                        rmopName = @"shaders\shader_options\material_organism_option";
                        break;
                    case Material_Model.Single_Lobe_Phong:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddFloat3ColorParameter("specular_tint");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        rmopName = @"shaders\shader_options\single_lobe_phong";
                        break;
                    case Material_Model.Car_Paint:
                        result.AddBooleanParameter("use_material_texture0");
                        result.AddBooleanParameter("use_material_texture1");
                        result.AddSamplerWithoutXFormParameter("material_texture");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddSamplerWithoutXFormParameter("bump_detail_map0");
                        result.AddFloatParameter("bump_detail_map0_blend_factor");
                        result.AddFloatParameter("diffuse_coefficient0");
                        result.AddFloatParameter("specular_coefficient0");
                        result.AddFloat3ColorParameter("specular_tint0");
                        result.AddFloat3ColorParameter("fresnel_color0");
                        result.AddFloatParameter("fresnel_power0");
                        result.AddFloatParameter("albedo_blend0");
                        result.AddFloatParameter("roughness0");
                        result.AddFloatParameter("area_specular_contribution0");
                        result.AddFloatParameter("analytical_specular_contribution0");
                        result.AddBooleanParameter("order3_area_specular0");
                        result.AddFloatParameter("diffuse_coefficient1");
                        result.AddFloatParameter("specular_coefficient1");
                        result.AddFloat3ColorParameter("specular_tint1");
                        result.AddFloat3ColorParameter("fresnel_color1");
                        result.AddFloat3ColorParameter("fresnel_color_environment1");
                        result.AddFloatParameter("fresnel_power1");
                        result.AddFloatParameter("albedo_blend1");
                        result.AddFloatParameter("roughness1");
                        result.AddFloatParameter("area_specular_contribution1");
                        result.AddFloatParameter("analytical_specular_contribution1");
                        result.AddFloatParameter("environment_map_specular_contribution1");
                        result.AddBooleanParameter("order3_area_specular1");
                        result.AddFloatParameter("rim_fresnel_coefficient1");
                        result.AddFloat3ColorParameter("rim_fresnel_color1");
                        result.AddFloatParameter("rim_fresnel_power1");
                        result.AddFloatParameter("rim_fresnel_albedo_blend1");
                        rmopName = @"shaders\shader_options\material_car_paint_option";
                        break;
                    case Material_Model.Cook_Torrance_Custom_Cube:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint");
                        result.AddFloat3ColorParameter("fresnel_color");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddSamplerWithoutXFormParameter("custom_cube");
                        rmopName = @"shaders\shader_options\material_cook_torrance_custom_cube_option";
                        break;
                    case Material_Model.Cook_Torrance_Pbr_Maps:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint");
                        result.AddFloat3ColorParameter("fresnel_color");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddSamplerWithoutXFormParameter("spec_tint_map");
                        rmopName = @"shaders\shader_options\material_cook_torrance_pbr_maps_option";
                        break;
                    case Material_Model.Cook_Torrance_Two_Color_Spec_Tint:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint");
                        result.AddFloat3ColorParameter("fresnel_color");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddSamplerWithoutXFormParameter("spec_blend_map");
                        result.AddFloat3ColorParameter("specular_second_tint");
                        rmopName = @"shaders\shader_options\material_cook_torrance_two_color_spec_tint";
                        break;
                    case Material_Model.Two_Lobe_Phong_Tint_Map:
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
                        result.AddSamplerWithoutXFormParameter("normal_specular_tint_map");
                        result.AddSamplerWithoutXFormParameter("glancing_specular_tint_map");
                        rmopName = @"shaders\shader_options\material_two_lobe_phong_tint_map_option";
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube_Mask:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint");
                        result.AddFloat3ColorParameter("fresnel_color");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerWithoutXFormParameter("material_texture");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddSamplerWithoutXFormParameter("tint_blend_mask_cubemap");
                        result.AddFloat3ColorParameter("specular_second_tint");
                        rmopName = @"shaders\shader_options\material_cook_torrance_scrolling_cube_mask";
                        break;
                    case Material_Model.Cook_Torrance_Rim_Fresnel:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint");
                        result.AddFloat3ColorParameter("fresnel_color");
                        result.AddBooleanParameter("use_fresnel_color_environment");
                        result.AddFloat3ColorParameter("fresnel_color_environment");
                        result.AddFloatParameter("fresnel_power");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddBooleanParameter("albedo_blend_with_specular_tint");
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddFloatParameter("rim_fresnel_coefficient");
                        result.AddFloat3ColorParameter("rim_fresnel_color");
                        result.AddFloatParameter("rim_fresnel_power");
                        result.AddFloatParameter("rim_fresnel_albedo_blend");
                        rmopName = @"shaders\shader_options\material_cook_torrance_rim_fresnel";
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("fresnel_color");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerWithoutXFormParameter("material_texture");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        result.AddSamplerWithoutXFormParameter("spec_tint_cubemap");
                        rmopName = @"shaders\shader_options\material_cook_torrance_scrolling_cube";
                        break;
                    case Material_Model.Cook_Torrance_From_Albedo:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("fresnel_color");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddBooleanParameter("order3_area_specular");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerParameter("material_texture");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddSamplerWithoutXFormParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236);
                        result.AddSamplerWithoutXFormParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78);
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("analytical_anti_shadow_control");
                        rmopName = @"shaders\shader_options\material_cook_torrance_from_albedo";
                        break;
                    case Material_Model.Hair:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloat3ColorParameter("diffuse_tint");
                        result.AddFloatParameter("analytical_specular_coefficient");
                        result.AddFloatParameter("area_specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint");
                        result.AddFloatParameter("specular_power");
                        result.AddSamplerWithoutXFormParameter("specular_map");
                        result.AddSamplerWithoutXFormParameter("specular_shift_map");
                        result.AddSamplerWithoutXFormParameter("specular_noise_map");
                        result.AddFloatParameter("environment_map_coefficient");
                        result.AddFloat3ColorParameter("environment_map_tint");
                        result.AddFloat3ColorParameter("final_tint");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("roughness");
                        rmopName = @"shaders\shader_options\material_hair_option";
                        break;
                    case Material_Model.Cook_Torrance_Reach:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloat3ColorParameter("specular_color_by_angle");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloat3ColorParameter("specular_tint");
                        result.AddFloat3ColorParameter("fresnel_color");
                        result.AddFloatParameter("fresnel_curve_steepness");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("analytical_roughness");
                        result.AddBooleanParameter("use_material_texture");
                        result.AddSamplerWithoutXFormParameter("material_texture");
                        result.AddFloatParameter("material_texture_black_specular_multiplier");
                        result.AddFloatParameter("material_texture_black_roughness");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("albedo_blend");
                        result.AddFloatParameter("approximate_specular_type");
                        rmopName = @"shaders\shader_options\material_cook_torrance_option_reach";
                        break;
                    case Material_Model.Two_Lobe_Phong_Reach:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloat3ColorParameter("specular_color_by_angle");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloatParameter("normal_specular_power");
                        result.AddFloat3ColorParameter("normal_specular_tint");
                        result.AddFloatParameter("glancing_specular_power");
                        result.AddFloat3ColorParameter("glancing_specular_tint");
                        result.AddFloatParameter("fresnel_curve_steepness");
                        result.AddFloatParameter("area_specular_contribution");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddFloatParameter("roughness");
                        result.AddFloatParameter("analytical_roughness");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("albedo_specular_tint_blend");
                        result.AddFloatParameter("approximate_specular_type");
                        result.AddFloatParameter("analytical_power");
                        rmopName = @"shaders\shader_options\material_two_lobe_phong_option_reach";
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
                    case Environment_Mapping.Custom_Map:
                        result.AddSamplerWithoutXFormParameter("environment_map");
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddFloatParameter("env_roughness_scale");
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.From_Flat_Texture_As_Cubemap:
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
                        result.AddSamplerParameter("self_illum_map");
                        result.AddFloat4ColorParameter("channel_a");
                        result.AddFloat4ColorParameter("channel_b");
                        result.AddFloat4ColorParameter("channel_c");
                        result.AddFloatParameter("self_illum_intensity");
                        rmopName = @"shaders\shader_options\illum_3_channel";
                        break;
                    case Self_Illumination.Plasma:
                        result.AddSamplerParameter("noise_map_a");
                        result.AddSamplerParameter("noise_map_b");
                        result.AddFloat4ColorParameter("color_medium");
                        result.AddFloat4ColorParameter("color_wide");
                        result.AddFloat4ColorParameter("color_sharp");
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddSamplerParameter("alpha_mask_map");
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
                        result.AddSamplerParameter("self_illum_map");
                        result.AddSamplerParameter("self_illum_detail_map");
                        result.AddFloat3ColorParameter("self_illum_color");
                        result.AddFloatParameter("self_illum_intensity");
                        rmopName = @"shaders\shader_options\illum_detail";
                        break;
                    case Self_Illumination.Meter:
                        result.AddSamplerParameter("meter_map");
                        result.AddFloat3ColorParameter("meter_color_off");
                        result.AddFloat3ColorParameter("meter_color_on");
                        result.AddFloatParameter("meter_value");
                        rmopName = @"shaders\shader_options\illum_meter";
                        break;
                    case Self_Illumination.Self_Illum_Times_Diffuse:
                        result.AddSamplerParameter("self_illum_map");
                        result.AddFloat3ColorParameter("self_illum_color");
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddFloatParameter("primary_change_color_blend");
                        rmopName = @"shaders\shader_options\illum_times_diffuse";
                        break;
                    case Self_Illumination.Simple_With_Alpha_Mask:
                        result.AddSamplerParameter("self_illum_map");
                        result.AddFloat3ColorParameter("self_illum_color");
                        result.AddFloatParameter("self_illum_intensity");
                        rmopName = @"shaders\shader_options\illum_simple";
                        break;
                    case Self_Illumination.Simple_Four_Change_Color:
                        result.AddSamplerWithoutXFormParameter("self_illum_map");
                        result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_quaternary);
                        result.AddFloatParameter("self_illum_intensity");
                        rmopName = @"shaders\shader_options\illum_simple_four_change_color";
                        break;
                    case Self_Illumination.Illum_Detail_World_Space_Four_Cc:
                        result.AddSamplerWithoutXFormParameter("self_illum_map");
                        result.AddSamplerWithoutXFormParameter("self_illum_detail_map");
                        result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_quaternary);
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddFloat4ColorParameter("self_illum_obj_bounding_sphere");
                        rmopName = @"shaders\shader_options\illum_detail_world_space_four_cc";
                        break;
                    case Self_Illumination.Illum_Change_Color:
                        result.AddSamplerParameter("self_illum_map");
                        result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_primary);
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddFloatParameter("primary_change_color_blend");
                        rmopName = @"shaders\shader_options\illum_change_color";
                        break;
                    case Self_Illumination.Multilayer_Additive:
                        result.AddSamplerWithoutXFormParameter("self_illum_map");
                        result.AddFloat3ColorParameter("self_illum_color");
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddFloatParameter("layer_depth");
                        result.AddFloatParameter("layer_contrast");
                        result.AddIntegerParameter("layers_of_4");
                        result.AddFloatParameter("texcoord_aspect_ratio");
                        result.AddFloatParameter("depth_darken");
                        rmopName = @"shaders\shader_options\illum_multilayer";
                        break;
                    case Self_Illumination.Palettized_Plasma:
                        result.AddSamplerWithoutXFormParameter("noise_map_a");
                        result.AddSamplerWithoutXFormParameter("noise_map_b");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddSamplerWithoutXFormParameter("alpha_mask_map");
                        result.AddFloatParameter("alpha_modulation_factor");
                        result.AddSamplerWithoutXFormParameter("depth_buffer", RenderMethodExtern.texture_global_target_z);
                        result.AddFloatParameter("depth_fade_range");
                        result.AddFloat3ColorParameter("self_illum_color");
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddFloatParameter("v_coordinate");
                        result.AddFloatParameter("global_depth_constants", RenderMethodExtern.global_depth_constants);
                        result.AddFloatParameter("global_camera_forward", RenderMethodExtern.global_camera_forward);
                        rmopName = @"shaders\screen_options\illum_palettized_plasma";
                        break;
                    case Self_Illumination.Change_Color:
                        result.AddSamplerWithoutXFormParameter("self_illum_map");
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                        rmopName = @"shaders\shader_options\illum_change_color";
                        break;
                    case Self_Illumination.Change_Color_Detail:
                        result.AddSamplerWithoutXFormParameter("self_illum_map");
                        result.AddSamplerWithoutXFormParameter("self_illum_detail_map");
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_primary);
                        rmopName = @"shaders\shader_options\illum_change_color_detail";
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
                    case Blend_Mode.Pre_Multiplied_Alpha:
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
                        result.AddSamplerParameter("height_map");
                        result.AddFloatParameter("height_scale");
                        rmopName = @"shaders\shader_options\parallax_simple";
                        break;
                    case Parallax.Interpolated:
                        result.AddSamplerParameter("height_map");
                        result.AddFloatParameter("height_scale");
                        rmopName = @"shaders\shader_options\parallax_simple";
                        break;
                    case Parallax.Simple_Detail:
                        result.AddSamplerParameter("height_map");
                        result.AddFloatParameter("height_scale");
                        result.AddSamplerParameter("height_scale_map");
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

            if (methodName == "distortion")
            {
                optionName = ((Distortion)option).ToString();

                switch ((Distortion)option)
                {
                    case Distortion.Off:
                        break;
                    case Distortion.On:
                        result.AddSamplerWithoutXFormParameter("distort_map");
                        result.AddFloatParameter("distort_scale");
                        result.AddFloatParameter("distort_fadeoff");
                        result.AddBooleanParameter("distort_selfonly");
                        rmopName = @"shaders\shader_options\sfx_distort";
                        break;
                }
            }

            if (methodName == "soft_fade")
            {
                optionName = ((Soft_Fade)option).ToString();

                switch ((Soft_Fade)option)
                {
                    case Soft_Fade.Off:
                        break;
                    case Soft_Fade.On:
                        result.AddSamplerWithoutXFormParameter("depth_map", RenderMethodExtern.texture_global_target_z);
                        result.AddBooleanParameter("use_soft_fresnel");
                        result.AddFloatParameter("soft_fresnel_power");
                        result.AddBooleanParameter("use_soft_z");
                        result.AddFloatParameter("soft_z_range");
                        result.AddFloatParameter("screen_params", RenderMethodExtern.screen_constants);
                        rmopName = @"shaders\shader_options\soft_fade";
                        break;
                }
            }

            if (methodName == "misc_attr_animation")
            {
                optionName = ((Misc_Attr_Animation)option).ToString();

                switch ((Misc_Attr_Animation)option)
                {
                    case Misc_Attr_Animation.Off:
                        break;
                    case Misc_Attr_Animation.Scrolling_Cube:
                        result.AddIntegerParameter("misc_attr_animation_option");
                        result.AddFloatParameter("scrolling_axis_x");
                        result.AddFloatParameter("scrolling_axis_y");
                        result.AddFloatParameter("scrolling_axis_z");
                        result.AddFloatParameter("scrolling_speed");
                        rmopName = @"shaders\shader_options\misc_attr_scrolling_cube";
                        break;
                    case Misc_Attr_Animation.Scrolling_Projected:
                        result.AddIntegerParameter("misc_attr_animation_option");
                        result.AddFloatParameter("object_center_x");
                        result.AddFloatParameter("object_center_y");
                        result.AddFloatParameter("object_center_z");
                        result.AddFloatParameter("plane_u_x");
                        result.AddFloatParameter("plane_u_y");
                        result.AddFloatParameter("plane_u_z");
                        result.AddFloatParameter("plane_v_x");
                        result.AddFloatParameter("plane_v_y");
                        result.AddFloatParameter("plane_v_z");
                        result.AddFloatParameter("scale_u");
                        result.AddFloatParameter("scale_v");
                        result.AddFloatParameter("translate_u");
                        result.AddFloatParameter("translate_v");
                        result.AddFloatParameter("speed_u");
                        result.AddFloatParameter("speed_v");
                        rmopName = @"shaders\shader_options\misc_attr_scrolling_projected";
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
                    case Wetness.Simple:
                        result.AddFloatParameter("wet_material_dim_coefficient");
                        result.AddFloat3ColorParameter("wet_material_dim_tint");
                        rmopName = @"shaders\wetness_options\wetness_simple";
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

            if (methodName == "alpha_blend_source")
            {
                optionName = ((Alpha_Blend_Source)option).ToString();

                switch ((Alpha_Blend_Source)option)
                {
                    case Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel:
                        break;
                    case Alpha_Blend_Source.From_Albedo_Alpha:
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness");
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_albedo_alpha";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                        result.AddSamplerWithoutXFormParameter("opacity_texture");
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness");
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_opacity_map";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                        result.AddSamplerWithoutXFormParameter("opacity_texture");
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness");
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_opacity_map";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                        result.AddSamplerWithoutXFormParameter("opacity_texture");
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness");
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_opacity_map";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(ShaderMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((ShaderMethods)methodIndex)
            {
                case ShaderMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case ShaderMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping));
                case ShaderMethods.Alpha_Test:
                    return Enum.GetValues(typeof(Alpha_Test));
                case ShaderMethods.Specular_Mask:
                    return Enum.GetValues(typeof(Specular_Mask));
                case ShaderMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model));
                case ShaderMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping));
                case ShaderMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination));
                case ShaderMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
                case ShaderMethods.Parallax:
                    return Enum.GetValues(typeof(Parallax));
                case ShaderMethods.Misc:
                    return Enum.GetValues(typeof(Misc));
                case ShaderMethods.Distortion:
                    return Enum.GetValues(typeof(Distortion));
                case ShaderMethods.Soft_Fade:
                    return Enum.GetValues(typeof(Soft_Fade));
                case ShaderMethods.Misc_Attr_Animation:
                    return Enum.GetValues(typeof(Misc_Attr_Animation));
                case ShaderMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness));
                case ShaderMethods.Alpha_Blend_Source:
                    return Enum.GetValues(typeof(Alpha_Blend_Source));
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

            if (methodName == "distortion")
            {
                vertexFunction = "distort_proc_vs";
                pixelFunction = "distort_proc_ps";
            }

            if (methodName == "soft_fade")
            {
                vertexFunction = "invalid";
                pixelFunction = "apply_soft_fade";
            }

            if (methodName == "misc_attr_animation")
            {
                vertexFunction = "invalid"; // misc_attr_define (We ran out of output registers :/)
                pixelFunction = "invalid";
            }

            if (methodName == "wetness")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_wetness_ps";
            }

            if (methodName == "alpha_blend_source")
            {
                vertexFunction = "invalid";
                pixelFunction = "alpha_blend_source";
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
                    case Albedo.Two_Change_Color_Anim_Overlay:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_change_color_anim_ps";
                        break;
                    case Albedo.Chameleon:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_chameleon_ps";
                        break;
                    case Albedo.Two_Change_Color_Chameleon:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_change_color_chameleon_ps";
                        break;
                    case Albedo.Chameleon_Masked:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_chameleon_masked_ps";
                        break;
                    case Albedo.Color_Mask_Hard_Light:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_color_mask_hard_light_ps";
                        break;
                    case Albedo.Two_Change_Color_Tex_Overlay:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_change_color_tex_overlay_ps";
                        break;
                    case Albedo.Chameleon_Albedo_Masked:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_chameleon_albedo_masked_ps";
                        break;
                    case Albedo.Custom_Cube:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_custom_cube_ps";
                        break;
                    case Albedo.Two_Color:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_color_ps";
                        break;
                    case Albedo.Scrolling_Cube_Mask:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_scrolling_cube_mask_ps";
                        break;
                    case Albedo.Scrolling_Cube:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_scrolling_cube_ps";
                        break;
                    case Albedo.Scrolling_Texture_Uv:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_scrolling_texture_uv_ps";
                        break;
                    case Albedo.Texture_From_Misc:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_texture_from_misc_ps";
                        break;
                    case Albedo.Four_Change_Color_Applying_To_Specular:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_four_change_color_applying_to_specular_ps";
                        break;
                    case Albedo.Simple:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_simple_ps";
                        break;
                    case Albedo.Emblem:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_emblem_ps";
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
                    case Bump_Mapping.Detail_Masked:
                        vertexFunction = "calc_bumpmap_detail_vs";
                        pixelFunction = "calc_bumpmap_detail_masked_ps";
                        break;
                    case Bump_Mapping.Detail_Plus_Detail_Masked:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_bumpmap_detail_plus_detail_masked_ps";
                        break;
                    case Bump_Mapping.Detail_Unorm:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_bumpmap_detail_unorm_ps";
                        break;
                    case Bump_Mapping.Detail_Blend:
                        vertexFunction = "calc_bumpmap_detail_blend_vs";
                        pixelFunction = "calc_bumpmap_detail_blend_ps";
                        break;
                    case Bump_Mapping.Three_Detail_Blend:
                        vertexFunction = "calc_bumpmap_three_detail_blend_vs";
                        pixelFunction = "calc_bumpmap_three_detail_blend_ps";
                        break;
                    case Bump_Mapping.Standard_Wrinkle:
                        vertexFunction = "calc_bumpmap_default_wrinkle_vs";
                        pixelFunction = "calc_bumpmap_default_wrinkle_ps";
                        break;
                    case Bump_Mapping.Detail_Wrinkle:
                        vertexFunction = "calc_bumpmap_detail_wrinkle_vs";
                        pixelFunction = "calc_bumpmap_detail_wrinkle_ps";
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
                    case Specular_Mask.Specular_Mask_From_Color_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_color_texture_ps";
                        break;
                    case Specular_Mask.Specular_Mask_Mult_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_mult_texture_ps";
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
                    case Material_Model.Cook_Torrance:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance";
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
                    case Material_Model.Glass:
                        vertexFunction = "invalid";
                        pixelFunction = "glass";
                        break;
                    case Material_Model.Organism:
                        vertexFunction = "invalid";
                        pixelFunction = "organism";
                        break;
                    case Material_Model.Single_Lobe_Phong:
                        vertexFunction = "invalid";
                        pixelFunction = "single_lobe_phong";
                        break;
                    case Material_Model.Car_Paint:
                        vertexFunction = "invalid";
                        pixelFunction = "car_paint";
                        break;
                    case Material_Model.Cook_Torrance_Custom_Cube:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_custom_cube";
                        break;
                    case Material_Model.Cook_Torrance_Pbr_Maps:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_pbr_maps";
                        break;
                    case Material_Model.Cook_Torrance_Two_Color_Spec_Tint:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_two_color_spec_tint";
                        break;
                    case Material_Model.Two_Lobe_Phong_Tint_Map:
                        vertexFunction = "invalid";
                        pixelFunction = "two_lobe_phong_tint_map";
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube_Mask:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_scrolling_cube_mask";
                        break;
                    case Material_Model.Cook_Torrance_Rim_Fresnel:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_rim_fresnel";
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_scrolling_cube";
                        break;
                    case Material_Model.Cook_Torrance_From_Albedo:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_from_albedo";
                        break;
                    case Material_Model.Hair:
                        vertexFunction = "invalid";
                        pixelFunction = "hair";
                        break;
                    case Material_Model.Cook_Torrance_Reach:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_reach";
                        break;
                    case Material_Model.Two_Lobe_Phong_Reach:
                        vertexFunction = "invalid";
                        pixelFunction = "two_lobe_phong_reach";
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
                    case Environment_Mapping.Custom_Map:
                        vertexFunction = "invalid";
                        pixelFunction = "custom_map";
                        break;
                    case Environment_Mapping.From_Flat_Texture_As_Cubemap:
                        vertexFunction = "invalid";
                        pixelFunction = "from_flat_texture_as_cubemap";
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
                    case Self_Illumination.Simple_With_Alpha_Mask:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_simple_with_alpha_mask_ps";
                        break;
                    case Self_Illumination.Simple_Four_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_simple_ps";
                        break;
                    case Self_Illumination.Illum_Detail_World_Space_Four_Cc:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_detail_world_space_ps";
                        break;
                    case Self_Illumination.Illum_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_change_color_ps";
                        break;
                    case Self_Illumination.Multilayer_Additive:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_multilayer_ps";
                        break;
                    case Self_Illumination.Palettized_Plasma:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_palettized_plasma_ps";
                        break;
                    case Self_Illumination.Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_change_color_ps";
                        break;
                    case Self_Illumination.Change_Color_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_change_color_detail_ps";
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
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "pre_multiplied_alpha";
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
                        pixelFunction = "0";
                        break;
                }
            }

            if (methodName == "distortion")
            {
                switch ((Distortion)option)
                {
                    case Distortion.Off:
                        vertexFunction = "distort_nocolor_vs";
                        pixelFunction = "distort_off_ps";
                        break;
                    case Distortion.On:
                        vertexFunction = "distort_nocolor_vs";
                        pixelFunction = "distort_on_ps";
                        break;
                }
            }

            if (methodName == "soft_fade")
            {
                switch ((Soft_Fade)option)
                {
                    case Soft_Fade.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "apply_soft_fade_off";
                        break;
                    case Soft_Fade.On:
                        vertexFunction = "invalid";
                        pixelFunction = "apply_soft_fade_on";
                        break;
                }
            }

            if (methodName == "misc_attr_animation")
            {
                switch ((Misc_Attr_Animation)option)
                {
                    case Misc_Attr_Animation.Off:
                        vertexFunction = "invalid"; // off (We ran out of output registers :/)
                        pixelFunction = "invalid";
                        break;
                    case Misc_Attr_Animation.Scrolling_Cube:
                        vertexFunction = "invalid"; // misc_attr_exist (We ran out of output registers :/)
                        pixelFunction = "invalid";
                        break;
                    case Misc_Attr_Animation.Scrolling_Projected:
                        vertexFunction = "invalid"; // misc_attr_exist (We ran out of output registers :/)
                        pixelFunction = "invalid";
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
                    case Wetness.Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_simple_ps";
                        break;
                    case Wetness.Ripples:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_ripples_ps";
                        break;
                }
            }

            if (methodName == "alpha_blend_source")
            {
                switch ((Alpha_Blend_Source)option)
                {
                    case Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel:
                        vertexFunction = "invalid";
                        pixelFunction = "albedo_alpha_without_fresnel";
                        break;
                    case Alpha_Blend_Source.From_Albedo_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "albedo_alpha";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "opacity_map_alpha";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                        vertexFunction = "invalid";
                        pixelFunction = "opacity_map_rgb";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "opacity_map_alpha_and_albedo_alpha";
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
                    case Albedo.Two_Change_Color_Anim_Overlay:
                        break;
                    case Albedo.Chameleon:
                        break;
                    case Albedo.Two_Change_Color_Chameleon:
                        break;
                    case Albedo.Chameleon_Masked:
                        break;
                    case Albedo.Color_Mask_Hard_Light:
                        break;
                    case Albedo.Two_Change_Color_Tex_Overlay:
                        break;
                    case Albedo.Chameleon_Albedo_Masked:
                        break;
                    case Albedo.Custom_Cube:
                        break;
                    case Albedo.Two_Color:
                        break;
                    case Albedo.Scrolling_Cube_Mask:
                        break;
                    case Albedo.Scrolling_Cube:
                        break;
                    case Albedo.Scrolling_Texture_Uv:
                        break;
                    case Albedo.Texture_From_Misc:
                        break;
                    case Albedo.Four_Change_Color_Applying_To_Specular:
                        break;
                    case Albedo.Simple:
                        break;
                    case Albedo.Emblem:
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
                    case Bump_Mapping.Detail_Masked:
                        break;
                    case Bump_Mapping.Detail_Plus_Detail_Masked:
                        break;
                    case Bump_Mapping.Detail_Unorm:
                        break;
                    case Bump_Mapping.Detail_Blend:
                        break;
                    case Bump_Mapping.Three_Detail_Blend:
                        break;
                    case Bump_Mapping.Standard_Wrinkle:
                        break;
                    case Bump_Mapping.Detail_Wrinkle:
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
                    case Specular_Mask.Specular_Mask_From_Color_Texture:
                        break;
                    case Specular_Mask.Specular_Mask_Mult_Diffuse:
                        break;
                }
            }

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Diffuse_Only:
                        break;
                    case Material_Model.Cook_Torrance:
                        break;
                    case Material_Model.Two_Lobe_Phong:
                        break;
                    case Material_Model.Foliage:
                        break;
                    case Material_Model.None:
                        break;
                    case Material_Model.Glass:
                        break;
                    case Material_Model.Organism:
                        break;
                    case Material_Model.Single_Lobe_Phong:
                        break;
                    case Material_Model.Car_Paint:
                        break;
                    case Material_Model.Cook_Torrance_Custom_Cube:
                        break;
                    case Material_Model.Cook_Torrance_Pbr_Maps:
                        break;
                    case Material_Model.Cook_Torrance_Two_Color_Spec_Tint:
                        break;
                    case Material_Model.Two_Lobe_Phong_Tint_Map:
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube_Mask:
                        break;
                    case Material_Model.Cook_Torrance_Rim_Fresnel:
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube:
                        break;
                    case Material_Model.Cook_Torrance_From_Albedo:
                        break;
                    case Material_Model.Hair:
                        break;
                    case Material_Model.Cook_Torrance_Reach:
                        break;
                    case Material_Model.Two_Lobe_Phong_Reach:
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
                    case Environment_Mapping.Custom_Map:
                        break;
                    case Environment_Mapping.From_Flat_Texture_As_Cubemap:
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
                    case Self_Illumination.Simple_With_Alpha_Mask:
                        break;
                    case Self_Illumination.Simple_Four_Change_Color:
                        break;
                    case Self_Illumination.Illum_Detail_World_Space_Four_Cc:
                        break;
                    case Self_Illumination.Illum_Change_Color:
                        break;
                    case Self_Illumination.Multilayer_Additive:
                        break;
                    case Self_Illumination.Palettized_Plasma:
                        break;
                    case Self_Illumination.Change_Color:
                        break;
                    case Self_Illumination.Change_Color_Detail:
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
                    case Blend_Mode.Pre_Multiplied_Alpha:
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

            if (methodName == "distortion")
            {
                switch ((Distortion)option)
                {
                    case Distortion.Off:
                        break;
                    case Distortion.On:
                        break;
                }
            }

            if (methodName == "soft_fade")
            {
                switch ((Soft_Fade)option)
                {
                    case Soft_Fade.Off:
                        break;
                    case Soft_Fade.On:
                        break;
                }
            }

            if (methodName == "misc_attr_animation")
            {
                switch ((Misc_Attr_Animation)option)
                {
                    case Misc_Attr_Animation.Off:
                        break;
                    case Misc_Attr_Animation.Scrolling_Cube:
                        break;
                    case Misc_Attr_Animation.Scrolling_Projected:
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
                    case Wetness.Simple:
                        break;
                    case Wetness.Ripples:
                        break;
                }
            }

            if (methodName == "alpha_blend_source")
            {
                switch ((Alpha_Blend_Source)option)
                {
                    case Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel:
                        break;
                    case Alpha_Blend_Source.From_Albedo_Alpha:
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                        break;
                }
            }
            return result;
        }
    }
}
