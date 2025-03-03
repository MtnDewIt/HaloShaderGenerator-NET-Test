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
        private bool TemplateGenerationValid;
        private bool ApplyFixes;

        Albedo albedo;
        Blend_Mode blend_mode;
        Render_Pass render_pass;
        Specular specular;
        Bump_Mapping bump_mapping;
        Tinting tinting;
        Parallax parallax;
        Interier interier;

        public DecalGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        public DecalGenerator(Albedo albedo, Blend_Mode blend_mode, Render_Pass render_pass, Specular specular, Bump_Mapping bump_mapping, Tinting tinting, Parallax parallax, Interier interier, bool applyFixes = false)
        {
            this.albedo = albedo;
            this.blend_mode = blend_mode;
            this.render_pass = render_pass;
            this.specular = specular;
            this.bump_mapping = bump_mapping;
            this.tinting = tinting;
            this.parallax = parallax;
            this.interier = interier;

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public DecalGenerator(byte[] options, bool applyFixes = false)
        {
            options = ValidateOptions(options);

            this.albedo = (Albedo)options[0];
            this.blend_mode = (Blend_Mode)options[1];
            this.render_pass = (Render_Pass)options[2];
            this.specular = (Specular)options[3];
            this.bump_mapping = (Bump_Mapping)options[4];
            this.tinting = (Tinting)options[5];
            this.parallax = (Parallax)options[6];
            this.interier = (Interier)options[7];

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public ShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Decal, entryPoint, sBlendMode,
                Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes);

            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Albedo>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Blend_Mode>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Render_Pass>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Specular>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Bump_Mapping>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Tinting>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Parallax>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Interier>());

            macros.Add(ShaderGeneratorBase.CreateAutoMacro("albedo", albedo.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("blend_mode", blend_mode.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("render_pass", render_pass.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("specular", specular.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("bump_mapping", bump_mapping.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("tinting", tinting.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("parallax", parallax.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("interier", interier.ToString().ToLower()));

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Decal);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Decal, entryPoint, sBlendMode,
                Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes);

            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Albedo>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Blend_Mode>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Render_Pass>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Specular>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Bump_Mapping>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Tinting>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Parallax>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Interier>());

            macros.Add(ShaderGeneratorBase.CreateAutoMacro("albedo", albedo.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("blend_mode", blend_mode.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("render_pass", render_pass.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("specular", specular.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("bump_mapping", bump_mapping.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("tinting", tinting.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("parallax", parallax.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("interier", interier.ToString().ToLower()));

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Decal);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Decal, entryPoint,
                sBlendMode, Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes, true, vertexType);

            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Albedo>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Blend_Mode>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Render_Pass>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Specular>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Bump_Mapping>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Tinting>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Parallax>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Interier>());

            macros.Add(ShaderGeneratorBase.CreateAutoMacro("albedo", albedo.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("blend_mode", blend_mode.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("render_pass", render_pass.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("specular", specular.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("bump_mapping", bump_mapping.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("tinting", tinting.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("parallax", parallax.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("interier", interier.ToString().ToLower()));

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Decal);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Decal, entryPoint,
                sBlendMode, Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes, true, vertexType);

            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Albedo>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Blend_Mode>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Render_Pass>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Specular>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Bump_Mapping>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Tinting>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Parallax>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Interier>());

            macros.Add(ShaderGeneratorBase.CreateAutoMacro("albedo", albedo.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("blend_mode", blend_mode.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("render_pass", render_pass.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("specular", specular.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("bump_mapping", bump_mapping.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("tinting", tinting.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("parallax", parallax.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("interier", interier.ToString().ToLower()));

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Decal);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

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

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((DecalMethods)methodIndex)
            {
                case DecalMethods.Albedo:
                    return (int)albedo;
                case DecalMethods.Blend_Mode:
                    return (int)blend_mode;
                case DecalMethods.Render_Pass:
                    return (int)render_pass;
                case DecalMethods.Specular:
                    return (int)specular;
                case DecalMethods.Bump_Mapping:
                    return (int)bump_mapping;
                case DecalMethods.Tinting:
                    return (int)tinting;
                case DecalMethods.Parallax:
                    return (int)parallax;
                case DecalMethods.Interier:
                    return (int)interier;
            }

            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Default:
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
                case VertexType.FlatWorld:
                case VertexType.FlatRigid:
                case VertexType.FlatSkinned:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsVertexShaderShared(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Default:
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
                case Albedo.Diffuse_Only:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddFloatParameter("u_tiles");
                    result.AddFloatParameter("v_tiles");
                    break;
                case Albedo.Palettized:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddFloatParameter("u_tiles");
                    result.AddFloatParameter("v_tiles");
                    break;
                case Albedo.Palettized_Plus_Alpha:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    result.AddFloatParameter("u_tiles");
                    result.AddFloatParameter("v_tiles");
                    break;
                case Albedo.Diffuse_Plus_Alpha:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    result.AddFloatParameter("u_tiles");
                    result.AddFloatParameter("v_tiles");
                    break;
                case Albedo.Emblem_Change_Color:
                    result.AddSamplerWithoutXFormParameter("tex0_sampler");
                    result.AddSamplerWithoutXFormParameter("tex1_sampler");
                    result.AddFloat4ColorParameter("emblem_color_background_argb", RenderMethodExtern.none);
                    result.AddFloat4ColorParameter("emblem_color_icon1_argb", RenderMethodExtern.none);
                    result.AddFloat4ColorParameter("emblem_color_icon2_argb", RenderMethodExtern.none);
                    result.AddFloatParameter("u_tiles");
                    result.AddFloatParameter("v_tiles");
                    result.AddSamplerWithoutXFormParameter("foreground0_sampler", RenderMethodExtern.none);
                    break;
                case Albedo.Change_Color:
                    result.AddSamplerWithoutXFormParameter("change_color_map");
                    result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                    result.AddFloat3ColorParameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary);
                    result.AddFloatParameter("u_tiles");
                    result.AddFloatParameter("v_tiles");
                    break;
                case Albedo.Diffuse_Plus_Alpha_Mask:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    result.AddFloatParameter("u_tiles");
                    result.AddFloatParameter("v_tiles");
                    break;
                case Albedo.Palettized_Plus_Alpha_Mask:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    result.AddSamplerWithoutXFormParameter("u_tiles");
                    result.AddSamplerWithoutXFormParameter("v_tiles");
                    break;
                case Albedo.Vector_Alpha:
                    result.AddSamplerParameter("base_map");
                    result.AddFloatParameter("u_tiles");
                    result.AddFloatParameter("v_tiles");
                    result.AddSamplerWithoutXFormParameter("vector_map");
                    result.AddFloatParameter("vector_sharpness");
                    result.AddFloatParameter("antialias_tweak");
                    break;
                case Albedo.Vector_Alpha_Drop_Shadow:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddFloatParameter("u_tiles");
                    result.AddFloatParameter("v_tiles");
                    result.AddSamplerWithoutXFormParameter("vector_map");
                    result.AddFloatParameter("vector_sharpness");
                    result.AddSamplerWithoutXFormParameter("shadow_vector_map");
                    result.AddFloatParameter("shadow_darkness");
                    result.AddFloatParameter("shadow_sharpness");
                    result.AddFloatParameter("antialias_tweak");
                    break;
                case Albedo.Patchy_Emblem:
                    result.AddSamplerWithoutXFormParameter("foreground0_sampler", RenderMethodExtern.none);
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    result.AddFloatParameter("alpha_min");
                    result.AddFloatParameter("alpha_max");
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

            switch (render_pass)
            {
                case Render_Pass.Pre_Lighting:
                    break;
                case Render_Pass.Post_Lighting:
                    break;
                case Render_Pass.Transparent:
                    break;
            }

            switch (specular)
            {
                case Specular.Leave:
                    break;
                case Specular.Modulate:
                    result.AddFloatParameter("specular_multiplier");
                    break;
            }

            switch (bump_mapping)
            {
                case Bump_Mapping.Leave:
                    break;
                case Bump_Mapping.Standard:
                    result.AddSamplerParameter("bump_map");
                    break;
                case Bump_Mapping.Standard_Mask:
                    result.AddSamplerWithoutXFormParameter("bump_map");
                    break;
            }

            switch (tinting)
            {
                case Tinting.None:
                    break;
                case Tinting.Unmodulated:
                    result.AddFloat3ColorParameter("tint_color");
                    result.AddFloatParameter("intensity");
                    break;
                case Tinting.Partially_Modulated:
                    result.AddFloat3ColorParameter("tint_color");
                    result.AddFloatParameter("intensity");
                    result.AddFloatParameter("modulation_factor");
                    break;
                case Tinting.Fully_Modulated:
                    result.AddFloat3ColorParameter("tint_color");
                    result.AddFloatParameter("intensity");
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
                case Parallax.Sphere:
                    result.AddFloatParameter("sphere_radius");
                    result.AddFloatParameter("sphere_height");
                    break;
            }

            switch (interier)
            {
                case Interier.Off:
                    break;
                case Interier.Simple:
                    result.AddSamplerWithoutXFormParameter("interier");
                    result.AddFloatParameter("mask_threshold");
                    break;
                case Interier.Floor:
                    result.AddSamplerParameter("interier");
                    result.AddFloatParameter("mask_threshold");
                    result.AddFloatParameter("thin_shell_height");
                    break;
                case Interier.Hole:
                    result.AddSamplerParameter("interier");
                    result.AddFloatParameter("mask_threshold");
                    result.AddFloatParameter("thin_shell_height");
                    result.AddSamplerParameter("wall_map");
                    result.AddFloatParameter("hole_radius");
                    result.AddFloatParameter("fog_factor");
                    result.AddFloat3ColorParameter("fog_top_color");
                    result.AddFloat3ColorParameter("fog_bottom_color");
                    break;
                case Interier.Box:
                    result.AddSamplerParameter("interier");
                    result.AddFloatParameter("mask_threshold");
                    result.AddFloatParameter("thin_shell_height");
                    result.AddSamplerParameter("wall_map");
                    result.AddFloatParameter("box_size");
                    result.AddFloatParameter("fog_factor");
                    result.AddFloat3ColorParameter("fog_top_color");
                    result.AddFloat3ColorParameter("fog_bottom_color");
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
                case Albedo.Diffuse_Only:
                    result.AddFloatVertexParameter("u_tiles");
                    result.AddFloatVertexParameter("v_tiles");
                    break;
                case Albedo.Palettized:
                    result.AddFloatVertexParameter("u_tiles");
                    result.AddFloatVertexParameter("v_tiles");
                    break;
                case Albedo.Palettized_Plus_Alpha:
                    result.AddFloatVertexParameter("u_tiles");
                    result.AddFloatVertexParameter("v_tiles");
                    break;
                case Albedo.Diffuse_Plus_Alpha:
                    result.AddFloatVertexParameter("u_tiles");
                    result.AddFloatVertexParameter("v_tiles");
                    break;
                case Albedo.Emblem_Change_Color:
                    result.AddFloatVertexParameter("u_tiles");
                    result.AddFloatVertexParameter("v_tiles");
                    break;
                case Albedo.Change_Color:
                    result.AddFloatVertexParameter("u_tiles");
                    result.AddFloatVertexParameter("v_tiles");
                    break;
                case Albedo.Diffuse_Plus_Alpha_Mask:
                    result.AddFloatVertexParameter("u_tiles");
                    result.AddFloatVertexParameter("v_tiles");
                    break;
                case Albedo.Palettized_Plus_Alpha_Mask:
                    result.AddSamplerVertexParameter("u_tiles");
                    result.AddSamplerVertexParameter("v_tiles");
                    break;
                case Albedo.Vector_Alpha:
                    result.AddFloatVertexParameter("u_tiles");
                    result.AddFloatVertexParameter("v_tiles");
                    break;
                case Albedo.Vector_Alpha_Drop_Shadow:
                    result.AddFloatVertexParameter("u_tiles");
                    result.AddFloatVertexParameter("v_tiles");
                    break;
                case Albedo.Patchy_Emblem:
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

            switch (render_pass)
            {
                case Render_Pass.Pre_Lighting:
                    break;
                case Render_Pass.Post_Lighting:
                    break;
                case Render_Pass.Transparent:
                    break;
            }

            switch (specular)
            {
                case Specular.Leave:
                    break;
                case Specular.Modulate:
                    break;
            }

            switch (bump_mapping)
            {
                case Bump_Mapping.Leave:
                    break;
                case Bump_Mapping.Standard:
                    break;
                case Bump_Mapping.Standard_Mask:
                    break;
            }

            switch (tinting)
            {
                case Tinting.None:
                    break;
                case Tinting.Unmodulated:
                    break;
                case Tinting.Partially_Modulated:
                    break;
                case Tinting.Fully_Modulated:
                    break;
            }

            switch (parallax)
            {
                case Parallax.Off:
                    break;
                case Parallax.Simple:
                    break;
                case Parallax.Sphere:
                    break;
            }

            switch (interier)
            {
                case Interier.Off:
                    break;
                case Interier.Simple:
                    break;
                case Interier.Floor:
                    break;
                case Interier.Hole:
                    break;
                case Interier.Box:
                    break;
            }

            return result;
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();
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
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddFloatParameter("u_tiles");
                        result.AddFloatParameter("v_tiles");
                        rmopName = @"shaders\decal_options\albedo_diffuse_only";
                        break;
                    case Albedo.Palettized:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddFloatParameter("u_tiles");
                        result.AddFloatParameter("v_tiles");
                        rmopName = @"shaders\decal_options\albedo_palettized";
                        break;
                    case Albedo.Palettized_Plus_Alpha:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddSamplerWithoutXFormParameter("alpha_map");
                        result.AddFloatParameter("u_tiles");
                        result.AddFloatParameter("v_tiles");
                        rmopName = @"shaders\decal_options\albedo_palettized_plus_alpha";
                        break;
                    case Albedo.Diffuse_Plus_Alpha:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("alpha_map");
                        result.AddFloatParameter("u_tiles");
                        result.AddFloatParameter("v_tiles");
                        rmopName = @"shaders\decal_options\albedo_diffuse_plus_alpha";
                        break;
                    case Albedo.Emblem_Change_Color:
                        result.AddSamplerWithoutXFormParameter("tex0_sampler");
                        result.AddSamplerWithoutXFormParameter("tex1_sampler");
                        result.AddFloat4ColorParameter("emblem_color_background_argb", RenderMethodExtern.none);
                        result.AddFloat4ColorParameter("emblem_color_icon1_argb", RenderMethodExtern.none);
                        result.AddFloat4ColorParameter("emblem_color_icon2_argb", RenderMethodExtern.none);
                        result.AddFloatParameter("u_tiles");
                        result.AddFloatParameter("v_tiles");
                        result.AddSamplerWithoutXFormParameter("foreground0_sampler", RenderMethodExtern.none);
                        rmopName = @"shaders\decal_options\albedo_emblem_change_color";
                        break;
                    case Albedo.Change_Color:
                        result.AddSamplerWithoutXFormParameter("change_color_map");
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                        result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                        result.AddFloat3ColorParameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary);
                        result.AddFloatParameter("u_tiles");
                        result.AddFloatParameter("v_tiles");
                        rmopName = @"shaders\decal_options\albedo_change_color";
                        break;
                    case Albedo.Diffuse_Plus_Alpha_Mask:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("alpha_map");
                        result.AddFloatParameter("u_tiles");
                        result.AddFloatParameter("v_tiles");
                        rmopName = @"shaders\decal_options\albedo_diffuse_plus_alpha_mask";
                        break;
                    case Albedo.Palettized_Plus_Alpha_Mask:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddSamplerWithoutXFormParameter("alpha_map");
                        result.AddSamplerWithoutXFormParameter("u_tiles");
                        result.AddSamplerWithoutXFormParameter("v_tiles");
                        rmopName = @"shaders\decal_options\albedo_palettized_plus_alpha_mask";
                        break;
                    case Albedo.Vector_Alpha:
                        result.AddSamplerParameter("base_map");
                        result.AddFloatParameter("u_tiles");
                        result.AddFloatParameter("v_tiles");
                        result.AddSamplerWithoutXFormParameter("vector_map");
                        result.AddFloatParameter("vector_sharpness");
                        result.AddFloatParameter("antialias_tweak");
                        rmopName = @"shaders\decal_options\albedo_vector_alpha";
                        break;
                    case Albedo.Vector_Alpha_Drop_Shadow:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddFloatParameter("u_tiles");
                        result.AddFloatParameter("v_tiles");
                        result.AddSamplerWithoutXFormParameter("vector_map");
                        result.AddFloatParameter("vector_sharpness");
                        result.AddSamplerWithoutXFormParameter("shadow_vector_map");
                        result.AddFloatParameter("shadow_darkness");
                        result.AddFloatParameter("shadow_sharpness");
                        result.AddFloatParameter("antialias_tweak");
                        rmopName = @"shaders\decal_options\albedo_vector_alpha_drop_shadow";
                        break;
                    case Albedo.Patchy_Emblem:
                        result.AddSamplerWithoutXFormParameter("foreground0_sampler", RenderMethodExtern.none);
                        result.AddSamplerWithoutXFormParameter("alpha_map");
                        result.AddFloatParameter("alpha_min");
                        result.AddFloatParameter("alpha_max");
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
                        result.AddSamplerParameter("bump_map");
                        rmopName = @"shaders\decal_options\bump_mapping_standard";
                        break;
                    case Bump_Mapping.Standard_Mask:
                        result.AddSamplerWithoutXFormParameter("bump_map");
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
                        result.AddFloat3ColorParameter("tint_color");
                        result.AddFloatParameter("intensity");
                        rmopName = @"shaders\decal_options\tinting_unmodulated";
                        break;
                    case Tinting.Partially_Modulated:
                        result.AddFloat3ColorParameter("tint_color");
                        result.AddFloatParameter("intensity");
                        result.AddFloatParameter("modulation_factor");
                        rmopName = @"shaders\decal_options\tinting_partially_modulated";
                        break;
                    case Tinting.Fully_Modulated:
                        result.AddFloat3ColorParameter("tint_color");
                        result.AddFloatParameter("intensity");
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
                        result.AddSamplerParameter("height_map");
                        result.AddFloatParameter("height_scale");
                        rmopName = @"shaders\decal_options\parallax_simple";
                        break;
                    case Parallax.Sphere:
                        result.AddFloatParameter("sphere_radius");
                        result.AddFloatParameter("sphere_height");
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
                        result.AddSamplerWithoutXFormParameter("interier");
                        result.AddFloatParameter("mask_threshold");
                        rmopName = @"shaders\decal_options\interier_shell";
                        break;
                    case Interier.Floor:
                        result.AddSamplerParameter("interier");
                        result.AddFloatParameter("mask_threshold");
                        result.AddFloatParameter("thin_shell_height");
                        rmopName = @"shaders\decal_options\interier_thin_shell";
                        break;
                    case Interier.Hole:
                        result.AddSamplerParameter("interier");
                        result.AddFloatParameter("mask_threshold");
                        result.AddFloatParameter("thin_shell_height");
                        result.AddSamplerParameter("wall_map");
                        result.AddFloatParameter("hole_radius");
                        result.AddFloatParameter("fog_factor");
                        result.AddFloat3ColorParameter("fog_top_color");
                        result.AddFloat3ColorParameter("fog_bottom_color");
                        rmopName = @"shaders\decal_options\interier_hole";
                        break;
                    case Interier.Box:
                        result.AddSamplerParameter("interier");
                        result.AddFloatParameter("mask_threshold");
                        result.AddFloatParameter("thin_shell_height");
                        result.AddSamplerParameter("wall_map");
                        result.AddFloatParameter("box_size");
                        result.AddFloatParameter("fog_factor");
                        result.AddFloat3ColorParameter("fog_top_color");
                        result.AddFloat3ColorParameter("fog_bottom_color");
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

        public ShaderParameters GetParameterArguments(string methodName, int option)
        {
            ShaderParameters result = new ShaderParameters();
            if (methodName == "albedo")
            {
                switch ((Albedo)option)
                {
                    case Albedo.Diffuse_Only:
                        break;
                    case Albedo.Palettized:
                        break;
                    case Albedo.Palettized_Plus_Alpha:
                        break;
                    case Albedo.Diffuse_Plus_Alpha:
                        break;
                    case Albedo.Emblem_Change_Color:
                        break;
                    case Albedo.Change_Color:
                        break;
                    case Albedo.Diffuse_Plus_Alpha_Mask:
                        break;
                    case Albedo.Palettized_Plus_Alpha_Mask:
                        break;
                    case Albedo.Vector_Alpha:
                        break;
                    case Albedo.Vector_Alpha_Drop_Shadow:
                        break;
                    case Albedo.Patchy_Emblem:
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
                switch ((Specular)option)
                {
                    case Specular.Leave:
                        break;
                    case Specular.Modulate:
                        break;
                }
            }

            if (methodName == "bump_mapping")
            {
                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Leave:
                        break;
                    case Bump_Mapping.Standard:
                        break;
                    case Bump_Mapping.Standard_Mask:
                        break;
                }
            }

            if (methodName == "tinting")
            {
                switch ((Tinting)option)
                {
                    case Tinting.None:
                        break;
                    case Tinting.Unmodulated:
                        break;
                    case Tinting.Partially_Modulated:
                        break;
                    case Tinting.Fully_Modulated:
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
                    case Parallax.Sphere:
                        break;
                }
            }

            if (methodName == "interier")
            {
                switch ((Interier)option)
                {
                    case Interier.Off:
                        break;
                    case Interier.Simple:
                        break;
                    case Interier.Floor:
                        break;
                    case Interier.Hole:
                        break;
                    case Interier.Box:
                        break;
                }
            }
            return result;
        }
    }
}
