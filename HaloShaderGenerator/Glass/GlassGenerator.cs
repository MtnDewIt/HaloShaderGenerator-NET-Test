using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Glass
{
    public class GlassGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;
        private bool ApplyFixes;

        Albedo albedo;
        Bump_Mapping bump_mapping;
        Material_Model material_model;
        Environment_Mapping environment_mapping;
        Wetness wetness;
        Alpha_Blend_Source alpha_blend_source;

        public GlassGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        public GlassGenerator(Albedo albedo, Bump_Mapping bump_mapping, Material_Model material_model, Environment_Mapping environment_mapping, Wetness wetness, Alpha_Blend_Source alpha_blend_source, bool applyFixes = false)
        {
            this.albedo = albedo;
            this.bump_mapping = bump_mapping;
            this.material_model = material_model;
            this.environment_mapping = environment_mapping;
            this.wetness = wetness;
            this.alpha_blend_source = alpha_blend_source;

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public GlassGenerator(byte[] options, bool applyFixes = false)
        {
            options = ValidateOptions(options);

            this.albedo = (Albedo)options[0];
            this.bump_mapping = (Bump_Mapping)options[1];
            this.material_model = (Material_Model)options[2];
            this.environment_mapping = (Environment_Mapping)options[3];
            this.wetness = (Wetness)options[4];
            this.alpha_blend_source = (Alpha_Blend_Source)options[5];

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public ShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Alpha_Blend_Source sAlphaBlendSource = (Shared.Alpha_Blend_Source)Enum.Parse(typeof(Shared.Alpha_Blend_Source), alpha_blend_source.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Glass, entryPoint, Shared.Blend_Mode.Opaque,
                Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, sAlphaBlendSource, ApplyFixes);

            switch (albedo)
            {
                case Albedo.Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_default_ps"));
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

            switch (material_model)
            {
                case Material_Model.Two_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong"));
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
            }

            switch (wetness)
            {
                case Wetness.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_simple_ps"));
                    break;
                case Wetness.Flood:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_flood_ps"));
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
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Glass);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Alpha_Blend_Source sAlphaBlendSource = (Shared.Alpha_Blend_Source)Enum.Parse(typeof(Shared.Alpha_Blend_Source), alpha_blend_source.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Glass, entryPoint, Shared.Blend_Mode.Opaque,
                Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, sAlphaBlendSource, ApplyFixes);

            switch (albedo)
            {
                case Albedo.Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_default_ps"));
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

            switch (material_model)
            {
                case Material_Model.Two_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong"));
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
            }

            switch (wetness)
            {
                case Wetness.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_simple_ps"));
                    break;
                case Wetness.Flood:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_flood_ps"));
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
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Glass);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Alpha_Blend_Source sAlphaBlendSource = (Shared.Alpha_Blend_Source)Enum.Parse(typeof(Shared.Alpha_Blend_Source), alpha_blend_source.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Glass, entryPoint,
                Shared.Blend_Mode.Opaque, Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, sAlphaBlendSource, ApplyFixes, true, vertexType);

            switch (albedo)
            {
                case Albedo.Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_default_ps"));
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

            switch (material_model)
            {
                case Material_Model.Two_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong"));
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
            }

            switch (wetness)
            {
                case Wetness.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_simple_ps"));
                    break;
                case Wetness.Flood:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_flood_ps"));
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
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Glass);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Alpha_Blend_Source sAlphaBlendSource = (Shared.Alpha_Blend_Source)Enum.Parse(typeof(Shared.Alpha_Blend_Source), alpha_blend_source.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Glass, entryPoint,
                Shared.Blend_Mode.Opaque, Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, sAlphaBlendSource, ApplyFixes, true, vertexType);

            switch (albedo)
            {
                case Albedo.Map:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", "calc_albedo_default_vs"));
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_default_ps"));
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

            switch (material_model)
            {
                case Material_Model.Two_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "two_lobe_phong"));
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
            }

            switch (wetness)
            {
                case Wetness.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_simple_ps"));
                    break;
                case Wetness.Flood:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_flood_ps"));
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
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Glass);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(GlassMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((GlassMethods)methodIndex)
            {
                case GlassMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case GlassMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping)).Length;
                case GlassMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model)).Length;
                case GlassMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping)).Length;
                case GlassMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness)).Length;
                case GlassMethods.Alpha_Blend_Source:
                    return Enum.GetValues(typeof(Alpha_Blend_Source)).Length;
            }

            return -1;
        }

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((GlassMethods)methodIndex)
            {
                case GlassMethods.Albedo:
                    return (int)albedo;
                case GlassMethods.Bump_Mapping:
                    return (int)bump_mapping;
                case GlassMethods.Material_Model:
                    return (int)material_model;
                case GlassMethods.Environment_Mapping:
                    return (int)environment_mapping;
                case GlassMethods.Wetness:
                    return (int)wetness;
                case GlassMethods.Alpha_Blend_Source:
                    return (int)alpha_blend_source;
            }

            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Default:
                case ShaderStage.Static_Sh:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Static_Prt_Ambient:
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
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Static_Sh:
                case ShaderStage.Static_Prt_Ambient:
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
                case Albedo.Map:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    result.AddFloat4ColorParameter("albedo_color");
                    break;
            }

            switch (bump_mapping)
            {
                case Bump_Mapping.Off:
                    break;
                case Bump_Mapping.Standard:
                    result.AddSamplerWithoutXFormParameter("bump_map");
                    break;
                case Bump_Mapping.Detail:
                    result.AddSamplerWithoutXFormParameter("bump_map");
                    result.AddSamplerWithoutXFormParameter("bump_detail_map");
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

            switch (material_model)
            {
                case Material_Model.Two_Lobe_Phong:
                    result.AddFloatParameter("diffuse_coefficient");
                    result.AddFloat3ColorParameter("specular_color_by_angle");
                    result.AddFloatParameter("specular_coefficient");
                    result.AddFloatParameter("normal_specular_power");
                    result.AddFloat3ColorParameter("normal_specular_tint");
                    result.AddFloatParameter("glancing_specular_power");
                    result.AddFloat3ColorParameter("glancing_specular_tint");
                    result.AddFloatParameter("fresnel_curve_steepness");
                    result.AddFloatParameter("analytical_specular_contribution");
                    result.AddFloatParameter("environment_map_specular_contribution");
                    result.AddFloatParameter("analytical_roughness");
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddFloatParameter("analytical_power");
                    result.AddFloatParameter("albedo_specular_tint_blend");
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
                    break;
                case Environment_Mapping.Dynamic:
                    result.AddFloat3ColorParameter("env_tint_color");
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
            }

            switch (wetness)
            {
                case Wetness.Simple:
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
                case Albedo.Map:
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
                case Bump_Mapping.Detail_Blend:
                    break;
                case Bump_Mapping.Three_Detail_Blend:
                    break;
                case Bump_Mapping.Standard_Wrinkle:
                    break;
                case Bump_Mapping.Detail_Wrinkle:
                    break;
            }

            switch (material_model)
            {
                case Material_Model.Two_Lobe_Phong:
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
            }

            switch (wetness)
            {
                case Wetness.Simple:
                    break;
                case Wetness.Flood:
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
            result.AddSamplerWithoutXFormParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0);
            result.AddFloat3ColorParameter("debug_tint", RenderMethodExtern.debug_tint);
            result.AddSamplerWithoutXFormParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerWithoutXFormParameter("scene_hdr_texture");
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
                    case Albedo.Map:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        result.AddFloat4ColorParameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_default";
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
                        result.AddSamplerWithoutXFormParameter("bump_map");
                        rmopName = @"shaders\shader_options\bump_default";
                        break;
                    case Bump_Mapping.Detail:
                        result.AddSamplerWithoutXFormParameter("bump_map");
                        result.AddSamplerWithoutXFormParameter("bump_detail_map");
                        rmopName = @"shaders\shader_options\bump_detail";
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

            if (methodName == "material_model")
            {
                optionName = ((Material_Model)option).ToString();

                switch ((Material_Model)option)
                {
                    case Material_Model.Two_Lobe_Phong:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloat3ColorParameter("specular_color_by_angle");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloatParameter("normal_specular_power");
                        result.AddFloat3ColorParameter("normal_specular_tint");
                        result.AddFloatParameter("glancing_specular_power");
                        result.AddFloat3ColorParameter("glancing_specular_tint");
                        result.AddFloatParameter("fresnel_curve_steepness");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddFloatParameter("analytical_roughness");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("analytical_power");
                        result.AddFloatParameter("albedo_specular_tint_blend");
                        rmopName = @"shaders\glass_options\glass_specular_option";
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
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.Dynamic:
                        result.AddFloat3ColorParameter("env_tint_color");
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
                }
            }

            if (methodName == "wetness")
            {
                optionName = ((Wetness)option).ToString();

                switch ((Wetness)option)
                {
                    case Wetness.Simple:
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
            return Enum.GetValues(typeof(GlassMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((GlassMethods)methodIndex)
            {
                case GlassMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case GlassMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping));
                case GlassMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model));
                case GlassMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping));
                case GlassMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness));
                case GlassMethods.Alpha_Blend_Source:
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
                    case Albedo.Map:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_default_ps";
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

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Two_Lobe_Phong:
                        vertexFunction = "invalid";
                        pixelFunction = "two_lobe_phong";
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
                }
            }

            if (methodName == "wetness")
            {
                switch ((Wetness)option)
                {
                    case Wetness.Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_simple_ps";
                        break;
                    case Wetness.Flood:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_flood_ps";
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
                    case Albedo.Map:
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

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Two_Lobe_Phong:
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
                }
            }

            if (methodName == "wetness")
            {
                switch ((Wetness)option)
                {
                    case Wetness.Simple:
                        break;
                    case Wetness.Flood:
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
