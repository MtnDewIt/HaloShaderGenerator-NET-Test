using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Halogram
{
    public class HalogramGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;
        private bool ApplyFixes;

        Albedo albedo;
        Self_Illumination self_illumination;
        Blend_Mode blend_mode;
        Misc misc;
        Warp warp;
        Overlay overlay;
        Edge_Fade edge_fade;
        Distortion distortion;
        Soft_Fade soft_fade;

        public HalogramGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        public HalogramGenerator(Albedo albedo, Self_Illumination self_illumination, Blend_Mode blend_mode, Misc misc, Warp warp, Overlay overlay, Edge_Fade edge_fade, Distortion distortion, Soft_Fade soft_fade, bool applyFixes = false)
        {
            this.albedo = albedo;
            this.self_illumination = self_illumination;
            this.blend_mode = blend_mode;
            this.misc = misc;
            this.warp = warp;
            this.overlay = overlay;
            this.edge_fade = edge_fade;
            this.distortion = distortion;
            this.soft_fade = soft_fade;

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public HalogramGenerator(byte[] options, bool applyFixes = false)
        {
            options = ValidateOptions(options);

            this.albedo = (Albedo)options[0];
            this.self_illumination = (Self_Illumination)options[1];
            this.blend_mode = (Blend_Mode)options[2];
            this.misc = (Misc)options[3];
            this.warp = (Warp)options[4];
            this.overlay = (Overlay)options[5];
            this.edge_fade = (Edge_Fade)options[6];
            this.distortion = (Distortion)options[7];
            this.soft_fade = (Soft_Fade)options[8];

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public ShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());
            Shader.Misc sMisc = (Shader.Misc)Enum.Parse(typeof(Shader.Misc), misc.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Halogram, entryPoint, sBlendMode,
                sMisc, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes);

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
                case Self_Illumination.Multilayer_Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Scope_Blur:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_scope_blur_ps"));
                    break;
                case Self_Illumination.Ml_Add_Four_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Ml_Add_Five_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Plasma_Wide_And_Sharp_Five_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_plasma_wide_and_sharp_five_change_color_ps"));
                    break;
                case Self_Illumination.Self_Illum_Holograms:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_holograms_ps"));
                    break;
                case Self_Illumination.Palettized_Plasma:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_plasma_ps"));
                    break;
                case Self_Illumination.Palettized_Plasma_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_plasma_ps"));
                    break;
                case Self_Illumination.Palettized_Depth_Fade:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_depth_fade_ps"));
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
                case Misc.Always_Calc_Albedo:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "2"));
                    break;
            }

            switch (warp)
            {
                case Warp.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Warp.From_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_warp_from_texture_ps"));
                    break;
                case Warp.Parallax_Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
            }

            switch (overlay)
            {
                case Overlay.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_none_ps"));
                    break;
                case Overlay.Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_ps"));
                    break;
                case Overlay.Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_detail_ps"));
                    break;
                case Overlay.Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_ps"));
                    break;
                case Overlay.Multiply_And_Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_and_additive_detail_ps"));
                    break;
            }

            switch (edge_fade)
            {
                case Edge_Fade.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_none_ps"));
                    break;
                case Edge_Fade.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_simple_ps"));
                    break;
            }

            switch (distortion)
            {
                case Distortion.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_ps", "distort_off_ps"));
                    break;
                case Distortion.On:
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

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Halogram);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());
            Shader.Misc sMisc = (Shader.Misc)Enum.Parse(typeof(Shader.Misc), misc.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Halogram, entryPoint, sBlendMode,
                sMisc, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes);

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
                case Self_Illumination.Multilayer_Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Scope_Blur:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_scope_blur_ps"));
                    break;
                case Self_Illumination.Ml_Add_Four_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Ml_Add_Five_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Plasma_Wide_And_Sharp_Five_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_plasma_wide_and_sharp_five_change_color_ps"));
                    break;
                case Self_Illumination.Self_Illum_Holograms:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_holograms_ps"));
                    break;
                case Self_Illumination.Palettized_Plasma:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_plasma_ps"));
                    break;
                case Self_Illumination.Palettized_Plasma_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_plasma_ps"));
                    break;
                case Self_Illumination.Palettized_Depth_Fade:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_depth_fade_ps"));
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
                case Misc.Always_Calc_Albedo:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "2"));
                    break;
            }

            switch (warp)
            {
                case Warp.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Warp.From_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_warp_from_texture_ps"));
                    break;
                case Warp.Parallax_Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
            }

            switch (overlay)
            {
                case Overlay.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_none_ps"));
                    break;
                case Overlay.Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_ps"));
                    break;
                case Overlay.Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_detail_ps"));
                    break;
                case Overlay.Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_ps"));
                    break;
                case Overlay.Multiply_And_Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_and_additive_detail_ps"));
                    break;
            }

            switch (edge_fade)
            {
                case Edge_Fade.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_none_ps"));
                    break;
                case Edge_Fade.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_simple_ps"));
                    break;
            }

            switch (distortion)
            {
                case Distortion.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_ps", "distort_off_ps"));
                    break;
                case Distortion.On:
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

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Halogram);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());
            Shader.Misc sMisc = (Shader.Misc)Enum.Parse(typeof(Shader.Misc), misc.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Halogram, entryPoint,
                sBlendMode, sMisc, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes, true, vertexType);

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
                case Self_Illumination.Multilayer_Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Scope_Blur:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_scope_blur_ps"));
                    break;
                case Self_Illumination.Ml_Add_Four_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Ml_Add_Five_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Plasma_Wide_And_Sharp_Five_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_plasma_wide_and_sharp_five_change_color_ps"));
                    break;
                case Self_Illumination.Self_Illum_Holograms:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_holograms_ps"));
                    break;
                case Self_Illumination.Palettized_Plasma:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_plasma_ps"));
                    break;
                case Self_Illumination.Palettized_Plasma_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_plasma_ps"));
                    break;
                case Self_Illumination.Palettized_Depth_Fade:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_depth_fade_ps"));
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
                case Misc.Always_Calc_Albedo:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "2"));
                    break;
            }

            switch (warp)
            {
                case Warp.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Warp.From_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_warp_from_texture_ps"));
                    break;
                case Warp.Parallax_Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
            }

            switch (overlay)
            {
                case Overlay.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_none_ps"));
                    break;
                case Overlay.Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_ps"));
                    break;
                case Overlay.Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_detail_ps"));
                    break;
                case Overlay.Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_ps"));
                    break;
                case Overlay.Multiply_And_Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_and_additive_detail_ps"));
                    break;
            }

            switch (edge_fade)
            {
                case Edge_Fade.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_none_ps"));
                    break;
                case Edge_Fade.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_simple_ps"));
                    break;
            }

            switch (distortion)
            {
                case Distortion.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_ps", "distort_off_ps"));
                    break;
                case Distortion.On:
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

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Halogram);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());
            Shader.Misc sMisc = (Shader.Misc)Enum.Parse(typeof(Shader.Misc), misc.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Halogram, entryPoint,
                sBlendMode, sMisc, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes, true, vertexType);

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
                case Self_Illumination.Multilayer_Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Scope_Blur:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_scope_blur_ps"));
                    break;
                case Self_Illumination.Ml_Add_Four_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Ml_Add_Five_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_multilayer_ps"));
                    break;
                case Self_Illumination.Plasma_Wide_And_Sharp_Five_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_plasma_wide_and_sharp_five_change_color_ps"));
                    break;
                case Self_Illumination.Self_Illum_Holograms:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_holograms_ps"));
                    break;
                case Self_Illumination.Palettized_Plasma:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_plasma_ps"));
                    break;
                case Self_Illumination.Palettized_Plasma_Change_Color:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_plasma_ps"));
                    break;
                case Self_Illumination.Palettized_Depth_Fade:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", "calc_self_illumination_palettized_depth_fade_ps"));
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
                case Misc.Always_Calc_Albedo:
                    macros.Add(ShaderGeneratorBase.CreateMacro("bitmap_rotation", "2"));
                    break;
            }

            switch (warp)
            {
                case Warp.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Warp.From_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_warp_from_texture_ps"));
                    break;
                case Warp.Parallax_Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
            }

            switch (overlay)
            {
                case Overlay.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_none_ps"));
                    break;
                case Overlay.Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_ps"));
                    break;
                case Overlay.Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_detail_ps"));
                    break;
                case Overlay.Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_ps"));
                    break;
                case Overlay.Multiply_And_Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_and_additive_detail_ps"));
                    break;
            }

            switch (edge_fade)
            {
                case Edge_Fade.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_none_ps"));
                    break;
                case Edge_Fade.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_simple_ps"));
                    break;
            }

            switch (distortion)
            {
                case Distortion.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_ps", "distort_off_ps"));
                    break;
                case Distortion.On:
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

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Halogram);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(HalogramMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((HalogramMethods)methodIndex)
            {
                case HalogramMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case HalogramMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination)).Length;
                case HalogramMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
                case HalogramMethods.Misc:
                    return Enum.GetValues(typeof(Misc)).Length;
                case HalogramMethods.Warp:
                    return Enum.GetValues(typeof(Warp)).Length;
                case HalogramMethods.Overlay:
                    return Enum.GetValues(typeof(Overlay)).Length;
                case HalogramMethods.Edge_Fade:
                    return Enum.GetValues(typeof(Edge_Fade)).Length;
                case HalogramMethods.Distortion:
                    return Enum.GetValues(typeof(Distortion)).Length;
                case HalogramMethods.Soft_Fade:
                    return Enum.GetValues(typeof(Soft_Fade)).Length;
            }

            return -1;
        }

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((HalogramMethods)methodIndex)
            {
                case HalogramMethods.Albedo:
                    return (int)albedo;
                case HalogramMethods.Self_Illumination:
                    return (int)self_illumination;
                case HalogramMethods.Blend_Mode:
                    return (int)blend_mode;
                case HalogramMethods.Misc:
                    return (int)misc;
                case HalogramMethods.Warp:
                    return (int)warp;
                case HalogramMethods.Overlay:
                    return (int)overlay;
                case HalogramMethods.Edge_Fade:
                    return (int)edge_fade;
                case HalogramMethods.Distortion:
                    return (int)distortion;
                case HalogramMethods.Soft_Fade:
                    return (int)soft_fade;
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
                //case ShaderStage.Active_Camo:
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
                case ShaderStage.Shadow_Generate:
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
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
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
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
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
                    result.AddSamplerWithoutXFormParameter("meter_map");
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
                case Self_Illumination.Multilayer_Additive:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat3ColorParameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("layer_depth");
                    result.AddFloatParameter("layer_contrast");
                    result.AddIntegerParameter("layers_of_4");
                    result.AddFloatParameter("texcoord_aspect_ratio");
                    result.AddFloatParameter("depth_darken");
                    break;
                case Self_Illumination.Scope_Blur:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat3ColorParameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloat3ColorParameter("self_illum_heat_color");
                    result.AddFloatParameter("z_camera_pixel_size", RenderMethodExtern.z_camera_pixel_size);
                    break;
                case Self_Illumination.Ml_Add_Four_Change_Color:
                    result.AddSamplerWithoutXFormParameter("self_illum_map");
                    result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_quaternary);
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("layer_depth");
                    result.AddFloatParameter("layer_contrast");
                    result.AddIntegerParameter("layers_of_4");
                    result.AddFloatParameter("texcoord_aspect_ratio");
                    result.AddFloatParameter("depth_darken");
                    break;
                case Self_Illumination.Ml_Add_Five_Change_Color:
                    result.AddSamplerWithoutXFormParameter("self_illum_map");
                    result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_quinary);
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("layer_depth");
                    result.AddFloatParameter("layer_contrast");
                    result.AddIntegerParameter("layers_of_4");
                    result.AddFloatParameter("texcoord_aspect_ratio");
                    result.AddFloatParameter("depth_darken");
                    break;
                case Self_Illumination.Plasma_Wide_And_Sharp_Five_Change_Color:
                    result.AddSamplerWithoutXFormParameter("noise_map_a");
                    result.AddSamplerWithoutXFormParameter("noise_map_b");
                    result.AddFloat4ColorParameter("color_medium");
                    result.AddFloat3ColorParameter("color_wide", RenderMethodExtern.object_change_color_quinary);
                    result.AddFloatParameter("color_wide_alpha");
                    result.AddFloat3ColorParameter("color_sharp", RenderMethodExtern.object_change_color_quinary);
                    result.AddFloatParameter("color_sharp_alpha");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddSamplerWithoutXFormParameter("alpha_mask_map");
                    result.AddFloatParameter("thinness_medium");
                    result.AddFloatParameter("thinness_wide");
                    result.AddFloatParameter("thinness_sharp");
                    break;
                case Self_Illumination.Self_Illum_Holograms:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat3ColorParameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination.Palettized_Plasma:
                    result.AddSamplerParameter("noise_map_a");
                    result.AddSamplerParameter("noise_map_b");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerParameter("alpha_mask_map");
                    result.AddFloatParameter("alpha_modulation_factor");
                    result.AddSamplerWithoutXFormParameter("depth_buffer", RenderMethodExtern.texture_global_target_z);
                    result.AddFloatParameter("depth_fade_range");
                    result.AddFloat3ColorParameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("v_coordinate");
                    result.AddFloatParameter("global_depth_constants", RenderMethodExtern.tree_animation_timer);
                    result.AddFloatParameter("global_camera_forward", RenderMethodExtern.global_depth_constants);
                    break;
                case Self_Illumination.Palettized_Plasma_Change_Color:
                    result.AddSamplerWithoutXFormParameter("noise_map_a");
                    result.AddSamplerWithoutXFormParameter("noise_map_b");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerWithoutXFormParameter("alpha_mask_map");
                    result.AddFloatParameter("alpha_modulation_factor");
                    result.AddSamplerWithoutXFormParameter("depth_buffer", RenderMethodExtern.texture_global_target_z);
                    result.AddFloatParameter("depth_fade_range");
                    result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("v_coordinate");
                    result.AddFloatParameter("global_depth_constants", RenderMethodExtern.global_depth_constants);
                    result.AddFloatParameter("global_camera_forward", RenderMethodExtern.global_camera_forward);
                    break;
                case Self_Illumination.Palettized_Depth_Fade:
                    result.AddSamplerWithoutXFormParameter("noise_map_a");
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
                case Misc.Always_Calc_Albedo:
                    break;
            }

            switch (warp)
            {
                case Warp.None:
                    break;
                case Warp.From_Texture:
                    result.AddSamplerParameter("warp_map");
                    result.AddFloatParameter("warp_amount_x");
                    result.AddFloatParameter("warp_amount_y");
                    break;
                case Warp.Parallax_Simple:
                    result.AddSamplerWithoutXFormParameter("height_map");
                    result.AddFloatParameter("height_scale");
                    break;
            }

            switch (overlay)
            {
                case Overlay.None:
                    break;
                case Overlay.Additive:
                    result.AddSamplerParameter("overlay_map");
                    result.AddFloat3ColorParameter("overlay_tint");
                    result.AddFloatParameter("overlay_intensity");
                    break;
                case Overlay.Additive_Detail:
                    result.AddSamplerParameter("overlay_map");
                    result.AddSamplerParameter("overlay_detail_map");
                    result.AddFloat3ColorParameter("overlay_tint");
                    result.AddFloatParameter("overlay_intensity");
                    break;
                case Overlay.Multiply:
                    result.AddSamplerParameter("overlay_map");
                    result.AddFloat3ColorParameter("overlay_tint");
                    result.AddFloatParameter("overlay_intensity");
                    break;
                case Overlay.Multiply_And_Additive_Detail:
                    result.AddSamplerParameter("overlay_multiply_map");
                    result.AddSamplerParameter("overlay_map");
                    result.AddSamplerParameter("overlay_detail_map");
                    result.AddFloat3ColorParameter("overlay_tint");
                    result.AddFloatParameter("overlay_intensity");
                    break;
            }

            switch (edge_fade)
            {
                case Edge_Fade.None:
                    break;
                case Edge_Fade.Simple:
                    result.AddFloat3ColorParameter("edge_fade_edge_tint");
                    result.AddFloat3ColorParameter("edge_fade_center_tint");
                    result.AddFloatParameter("edge_fade_power");
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
                case Self_Illumination.Multilayer_Additive:
                    break;
                case Self_Illumination.Scope_Blur:
                    break;
                case Self_Illumination.Ml_Add_Four_Change_Color:
                    break;
                case Self_Illumination.Ml_Add_Five_Change_Color:
                    break;
                case Self_Illumination.Plasma_Wide_And_Sharp_Five_Change_Color:
                    break;
                case Self_Illumination.Self_Illum_Holograms:
                    break;
                case Self_Illumination.Palettized_Plasma:
                    break;
                case Self_Illumination.Palettized_Plasma_Change_Color:
                    break;
                case Self_Illumination.Palettized_Depth_Fade:
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
                case Misc.Always_Calc_Albedo:
                    break;
            }

            switch (warp)
            {
                case Warp.None:
                    break;
                case Warp.From_Texture:
                    break;
                case Warp.Parallax_Simple:
                    break;
            }

            switch (overlay)
            {
                case Overlay.None:
                    break;
                case Overlay.Additive:
                    break;
                case Overlay.Additive_Detail:
                    break;
                case Overlay.Multiply:
                    break;
                case Overlay.Multiply_And_Additive_Detail:
                    break;
            }

            switch (edge_fade)
            {
                case Edge_Fade.None:
                    break;
                case Edge_Fade.Simple:
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
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddSamplerParameter("detail_map2");
                        rmopName = @"shaders\shader_options\albedo_two_detail_black_point";
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
                        result.AddSamplerWithoutXFormParameter("meter_map");
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
                    case Self_Illumination.Multilayer_Additive:
                        result.AddSamplerParameter("self_illum_map");
                        result.AddFloat3ColorParameter("self_illum_color");
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddFloatParameter("layer_depth");
                        result.AddFloatParameter("layer_contrast");
                        result.AddIntegerParameter("layers_of_4");
                        result.AddFloatParameter("texcoord_aspect_ratio");
                        result.AddFloatParameter("depth_darken");
                        rmopName = @"shaders\shader_options\illum_multilayer";
                        break;
                    case Self_Illumination.Scope_Blur:
                        result.AddSamplerParameter("self_illum_map");
                        result.AddFloat3ColorParameter("self_illum_color");
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddFloat3ColorParameter("self_illum_heat_color");
                        result.AddFloatParameter("z_camera_pixel_size", RenderMethodExtern.z_camera_pixel_size);
                        rmopName = @"shaders\shader_options\illum_scope_blur";
                        break;
                    case Self_Illumination.Ml_Add_Four_Change_Color:
                        result.AddSamplerWithoutXFormParameter("self_illum_map");
                        result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_quaternary);
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddFloatParameter("layer_depth");
                        result.AddFloatParameter("layer_contrast");
                        result.AddIntegerParameter("layers_of_4");
                        result.AddFloatParameter("texcoord_aspect_ratio");
                        result.AddFloatParameter("depth_darken");
                        rmopName = @"shaders\shader_options\illum_multilayer_four_change_color";
                        break;
                    case Self_Illumination.Ml_Add_Five_Change_Color:
                        result.AddSamplerWithoutXFormParameter("self_illum_map");
                        result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_quinary);
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddFloatParameter("layer_depth");
                        result.AddFloatParameter("layer_contrast");
                        result.AddIntegerParameter("layers_of_4");
                        result.AddFloatParameter("texcoord_aspect_ratio");
                        result.AddFloatParameter("depth_darken");
                        rmopName = @"shaders\shader_options\illum_multilayer_five_change_color";
                        break;
                    case Self_Illumination.Plasma_Wide_And_Sharp_Five_Change_Color:
                        result.AddSamplerWithoutXFormParameter("noise_map_a");
                        result.AddSamplerWithoutXFormParameter("noise_map_b");
                        result.AddFloat4ColorParameter("color_medium");
                        result.AddFloat3ColorParameter("color_wide", RenderMethodExtern.object_change_color_quinary);
                        result.AddFloatParameter("color_wide_alpha");
                        result.AddFloat3ColorParameter("color_sharp", RenderMethodExtern.object_change_color_quinary);
                        result.AddFloatParameter("color_sharp_alpha");
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddSamplerWithoutXFormParameter("alpha_mask_map");
                        result.AddFloatParameter("thinness_medium");
                        result.AddFloatParameter("thinness_wide");
                        result.AddFloatParameter("thinness_sharp");
                        rmopName = @"shaders\shader_options\illum_plasma_wide_and_sharp_five_change_color";
                        break;
                    case Self_Illumination.Self_Illum_Holograms:
                        result.AddSamplerParameter("self_illum_map");
                        result.AddFloat3ColorParameter("self_illum_color");
                        result.AddFloatParameter("self_illum_intensity");
                        rmopName = @"shaders\shader_options\illum_holograms";
                        break;
                    case Self_Illumination.Palettized_Plasma:
                        result.AddSamplerParameter("noise_map_a");
                        result.AddSamplerParameter("noise_map_b");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddSamplerParameter("alpha_mask_map");
                        result.AddFloatParameter("alpha_modulation_factor");
                        result.AddSamplerWithoutXFormParameter("depth_buffer", RenderMethodExtern.texture_global_target_z);
                        result.AddFloatParameter("depth_fade_range");
                        result.AddFloat3ColorParameter("self_illum_color");
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddFloatParameter("v_coordinate");
                        result.AddFloatParameter("global_depth_constants", RenderMethodExtern.tree_animation_timer);
                        result.AddFloatParameter("global_camera_forward", RenderMethodExtern.global_depth_constants);
                        rmopName = @"shaders\shader_options\illum_palettized_plasma";
                        break;
                    case Self_Illumination.Palettized_Plasma_Change_Color:
                        result.AddSamplerWithoutXFormParameter("noise_map_a");
                        result.AddSamplerWithoutXFormParameter("noise_map_b");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddSamplerWithoutXFormParameter("alpha_mask_map");
                        result.AddFloatParameter("alpha_modulation_factor");
                        result.AddSamplerWithoutXFormParameter("depth_buffer", RenderMethodExtern.texture_global_target_z);
                        result.AddFloatParameter("depth_fade_range");
                        result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_primary);
                        result.AddFloatParameter("self_illum_intensity");
                        result.AddFloatParameter("v_coordinate");
                        result.AddFloatParameter("global_depth_constants", RenderMethodExtern.global_depth_constants);
                        result.AddFloatParameter("global_camera_forward", RenderMethodExtern.global_camera_forward);
                        rmopName = @"shaders\screen_options\illum_palettized_plasma_change_color";
                        break;
                    case Self_Illumination.Palettized_Depth_Fade:
                        result.AddSamplerWithoutXFormParameter("noise_map_a");
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
                        rmopName = @"shaders\screen_options\illum_palettized_depth_fade";
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
                    case Misc.Always_Calc_Albedo:
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
                        result.AddSamplerParameter("warp_map");
                        result.AddFloatParameter("warp_amount_x");
                        result.AddFloatParameter("warp_amount_y");
                        rmopName = @"shaders\shader_options\warp_from_texture";
                        break;
                    case Warp.Parallax_Simple:
                        result.AddSamplerWithoutXFormParameter("height_map");
                        result.AddFloatParameter("height_scale");
                        rmopName = @"shaders\shader_options\parallax_simple";
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
                        result.AddSamplerParameter("overlay_map");
                        result.AddFloat3ColorParameter("overlay_tint");
                        result.AddFloatParameter("overlay_intensity");
                        rmopName = @"shaders\shader_options\overlay_additive";
                        break;
                    case Overlay.Additive_Detail:
                        result.AddSamplerParameter("overlay_map");
                        result.AddSamplerParameter("overlay_detail_map");
                        result.AddFloat3ColorParameter("overlay_tint");
                        result.AddFloatParameter("overlay_intensity");
                        rmopName = @"shaders\shader_options\overlay_additive_detail";
                        break;
                    case Overlay.Multiply:
                        result.AddSamplerParameter("overlay_map");
                        result.AddFloat3ColorParameter("overlay_tint");
                        result.AddFloatParameter("overlay_intensity");
                        rmopName = @"shaders\shader_options\overlay_additive";
                        break;
                    case Overlay.Multiply_And_Additive_Detail:
                        result.AddSamplerParameter("overlay_multiply_map");
                        result.AddSamplerParameter("overlay_map");
                        result.AddSamplerParameter("overlay_detail_map");
                        result.AddFloat3ColorParameter("overlay_tint");
                        result.AddFloatParameter("overlay_intensity");
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
                        result.AddFloat3ColorParameter("edge_fade_center_tint");
                        result.AddFloatParameter("edge_fade_power");
                        rmopName = @"shaders\shader_options\edge_fade_simple";
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
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(HalogramMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((HalogramMethods)methodIndex)
            {
                case HalogramMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case HalogramMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination));
                case HalogramMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
                case HalogramMethods.Misc:
                    return Enum.GetValues(typeof(Misc));
                case HalogramMethods.Warp:
                    return Enum.GetValues(typeof(Warp));
                case HalogramMethods.Overlay:
                    return Enum.GetValues(typeof(Overlay));
                case HalogramMethods.Edge_Fade:
                    return Enum.GetValues(typeof(Edge_Fade));
                case HalogramMethods.Distortion:
                    return Enum.GetValues(typeof(Distortion));
                case HalogramMethods.Soft_Fade:
                    return Enum.GetValues(typeof(Soft_Fade));
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

            if (methodName == "misc")
            {
                vertexFunction = "invalid";
                pixelFunction = "bitmap_rotation";
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

            if (methodName == "distortion")
            {
                vertexFunction = "invalid";
                pixelFunction = "distort_proc_ps";
            }

            if (methodName == "soft_fade")
            {
                vertexFunction = "invalid";
                pixelFunction = "apply_soft_fade";
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
                    case Self_Illumination.Multilayer_Additive:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_multilayer_ps";
                        break;
                    case Self_Illumination.Scope_Blur:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_scope_blur_ps";
                        break;
                    case Self_Illumination.Ml_Add_Four_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_multilayer_ps";
                        break;
                    case Self_Illumination.Ml_Add_Five_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_multilayer_ps";
                        break;
                    case Self_Illumination.Plasma_Wide_And_Sharp_Five_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_plasma_wide_and_sharp_five_change_color_ps";
                        break;
                    case Self_Illumination.Self_Illum_Holograms:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_holograms_ps";
                        break;
                    case Self_Illumination.Palettized_Plasma:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_palettized_plasma_ps";
                        break;
                    case Self_Illumination.Palettized_Plasma_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_palettized_plasma_ps";
                        break;
                    case Self_Illumination.Palettized_Depth_Fade:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_palettized_depth_fade_ps";
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
                    case Misc.Always_Calc_Albedo:
                        vertexFunction = "invalid";
                        pixelFunction = "2";
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

            if (methodName == "distortion")
            {
                switch ((Distortion)option)
                {
                    case Distortion.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "distort_off_ps";
                        break;
                    case Distortion.On:
                        vertexFunction = "invalid";
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
                    case Self_Illumination.Multilayer_Additive:
                        break;
                    case Self_Illumination.Scope_Blur:
                        break;
                    case Self_Illumination.Ml_Add_Four_Change_Color:
                        break;
                    case Self_Illumination.Ml_Add_Five_Change_Color:
                        break;
                    case Self_Illumination.Plasma_Wide_And_Sharp_Five_Change_Color:
                        break;
                    case Self_Illumination.Self_Illum_Holograms:
                        break;
                    case Self_Illumination.Palettized_Plasma:
                        break;
                    case Self_Illumination.Palettized_Plasma_Change_Color:
                        break;
                    case Self_Illumination.Palettized_Depth_Fade:
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
                    case Misc.Always_Calc_Albedo:
                        break;
                }
            }

            if (methodName == "warp")
            {
                switch ((Warp)option)
                {
                    case Warp.None:
                        break;
                    case Warp.From_Texture:
                        break;
                    case Warp.Parallax_Simple:
                        break;
                }
            }

            if (methodName == "overlay")
            {
                switch ((Overlay)option)
                {
                    case Overlay.None:
                        break;
                    case Overlay.Additive:
                        break;
                    case Overlay.Additive_Detail:
                        break;
                    case Overlay.Multiply:
                        break;
                    case Overlay.Multiply_And_Additive_Detail:
                        break;
                }
            }

            if (methodName == "edge_fade")
            {
                switch ((Edge_Fade)option)
                {
                    case Edge_Fade.None:
                        break;
                    case Edge_Fade.Simple:
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
            return result;
        }
    }
}
