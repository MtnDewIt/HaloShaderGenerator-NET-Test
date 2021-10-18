using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;

namespace HaloShaderGenerator.Decal
{
    public class DecalGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;
        private bool ApplyFixes;
        private bool DecalIsSimple;

        Albedo albedo;
        Blend_Mode blend_mode;
        Render_Pass render_pass;
        Specular specular;
        Bump_Mapping bump_mapping;
        Tinting tinting;

        /// <summary>
        /// Generator insantiation for shared shaders. Does not require method options.
        /// </summary>
        public DecalGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        /// <summary>
        /// Generator instantiation for method specific shaders.
        /// </summary>
        public DecalGenerator(Albedo albedo, Blend_Mode blend_mode, Render_Pass render_pass, Specular specular, Bump_Mapping bump_mapping, Tinting tinting, bool applyFixes = false)
        {
            this.albedo = albedo;
            this.blend_mode = blend_mode;
            this.render_pass = render_pass;
            this.specular = specular;
            this.bump_mapping = bump_mapping;
            this.tinting = tinting;

            ApplyFixes = applyFixes;
            DecalIsSimple = this.render_pass == Render_Pass.Pre_Lighting && this.bump_mapping == Bump_Mapping.Leave;
            TemplateGenerationValid = true;
        }

        public DecalGenerator(byte[] options, bool applyFixes = false)
        {
            this.albedo = (Albedo)options[0];
            this.blend_mode = (Blend_Mode)options[1];
            this.render_pass = (Render_Pass)options[2];
            this.specular = (Specular)options[3];
            this.bump_mapping = (Bump_Mapping)options[4];
            this.tinting = (Tinting)options[5];

            ApplyFixes = applyFixes;
            DecalIsSimple = this.render_pass == Render_Pass.Pre_Lighting && this.bump_mapping == Bump_Mapping.Leave;
            TemplateGenerationValid = true;
        }

        public ShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.ShaderType>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Albedo>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Blend_Mode>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Render_Pass>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Specular>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Bump_Mapping>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Tinting>());

            //
            // Convert to shared enum
            //

            var sAlbedo = Enum.Parse(typeof(Shared.Albedo), albedo.ToString());
            var sBlendMode = Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            //
            // The following code properly names the macros (like in rmdf)
            //

            // TODO fix
            //macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", sBlendMode, "blend_type_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("decal_blend_mode", blend_mode, "blend_mode_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", sAlbedo, "calc_albedo_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("decal_render_pass", render_pass, "render_pass_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("decal_specular", specular, "specular_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("decal_bump_mapping", bump_mapping, "bump_mapping_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("decal_tinting", tinting, "tinting_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("shaderstage", entryPoint, "k_shaderstage_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("shadertype", Shared.ShaderType.Decal, "k_shadertype_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("albedo_arg", sAlbedo, "k_albedo_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("blend_type_arg", sBlendMode, "k_blend_mode_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("decal_render_pass_arg", render_pass, "k_decal_render_pass_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("decal_specular_arg", specular, "k_decal_specular_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("decal_bump_mapping_arg", bump_mapping, "k_decal_bump_mapping_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("decal_tinting_arg", tinting, "k_decal_tinting_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("APPLY_HLSL_FIXES", ApplyFixes ? 1 : 0));
            macros.Add(ShaderGeneratorBase.CreateMacro("DECAL_IS_SIMPLE", DecalIsSimple ? 1 : 0));

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"pixl_decal.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderType>());

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"glps_decal.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<VertexType>());
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_vertex_transform", vertexType, "calc_vertex_transform_", ""));
            macros.Add(ShaderGeneratorBase.CreateMacro("transform_unknown_vector", vertexType, "transform_unknown_vector_", ""));
            macros.Add(ShaderGeneratorBase.CreateVertexMacro("input_vertex_format", vertexType));
            macros.Add(ShaderGeneratorBase.CreateMacro("transform_dominant_light", vertexType, "transform_dominant_light_", ""));

            macros.Add(ShaderGeneratorBase.CreateMacro("shaderstage", entryPoint, "k_shaderstage_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("vertextype", vertexType, "k_vertextype_"));

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource(@"glvs_decal.hlsl", macros, $"entry_{entryPoint.ToString().ToLower()}", "vs_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");
            return null;
        }

        public int GetMethodCount()
        {
            return System.Enum.GetValues(typeof(DecalMethods)).Length;
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
            }
            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            return entryPoint == ShaderStage.Default;
        }

        public bool IsMethodSharedInEntryPoint(ShaderStage entryPoint, int method_index)
        {
            return false;
        }

        public bool IsSharedPixelShaderWithoutMethod(ShaderStage entryPoint)
        {
            return false;
        }

        public bool IsPixelShaderShared(ShaderStage entryPoint)
        {
            return false;
        }

        public bool IsVertexFormatSupported(VertexType vertexType)
        {
            return vertexType == VertexType.World || vertexType == VertexType.Rigid || vertexType == VertexType.Skinned ||
                vertexType == VertexType.FlatWorld || vertexType == VertexType.FlatRigid || vertexType == VertexType.FlatSkinned;
        }

        public bool IsVertexShaderShared(ShaderStage entryPoint)
        {
            return true;
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
                    break;
                case Albedo.Palettized:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("palette");
                    break;
                case Albedo.Palettized_Plus_Alpha:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    break;
                case Albedo.Diffuse_Plus_Alpha:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    break;
                case Albedo.Emblem_Change_Color:
                    result.AddSamplerWithoutXFormParameter("tex0_sampler", RenderMethodExtern.emblem_player_shoulder_texture);
                    result.AddSamplerWithoutXFormParameter("tex1_sampler", RenderMethodExtern.emblem_player_shoulder_texture);
                    result.AddFloat4Parameter("emblem_color_background_argb");
                    result.AddFloat4Parameter("emblem_color_icon1_argb");
                    result.AddFloat4Parameter("emblem_color_icon2_argb");
                    break;
                case Albedo.Change_Color:
                    result.AddSamplerWithoutXFormParameter("change_color_map");
                    result.AddFloat3Parameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloat3Parameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                    result.AddFloat3Parameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary);
                    break;
                case Albedo.Diffuse_Plus_Alpha_Mask:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    break;
                case Albedo.Palettized_Plus_Alpha_Mask:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    break;
                case Albedo.Vector_Alpha:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("vector_map");
                    result.AddFloatParameter("vector_sharpness");
                    result.AddFloatParameter("antialias_tweak");
                    break;
                case Albedo.Vector_Alpha_Drop_Shadow:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("vector_map");
                    result.AddFloatParameter("vector_sharpness");
                    result.AddSamplerWithoutXFormParameter("shadow_vector_map");
                    result.AddFloatParameter("shadow_darkness");
                    result.AddFloatParameter("shadow_sharpness");
                    result.AddFloatParameter("antialias_tweak");
                    break;
            }
            switch (specular)
            {
                case Specular.Modulate:
                    result.AddFloatParameter("specular_multiplier");
                    break;
            }
            switch (bump_mapping)
            {
                case Bump_Mapping.Standard:
                    result.AddSamplerParameter("bump_map");
                    break;
                case Bump_Mapping.Standard_Mask:
                    result.AddSamplerParameter("bump_map");
                    break;
            }
            switch (tinting)
            {
                case Tinting.Unmodulated:
                    result.AddFloat4Parameter("tint_color");
                    result.AddFloatParameter("intensity");
                    break;
                case Tinting.Partially_Modulated:
                    result.AddFloat4Parameter("tint_color");
                    result.AddFloatParameter("intensity");
                    result.AddFloatParameter("modulation_factor");
                    break;
                case Tinting.Fully_Modulated:
                    result.AddFloat4Parameter("tint_color");
                    result.AddFloatParameter("intensity");
                    break;
            }

            return result;
        }

        public ShaderParameters GetVertexShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;

            var result = new ShaderParameters();

            result.AddFloatVertexParameter("u_tiles");
            result.AddFloatVertexParameter("v_tiles");

            return result;
        }

        public ShaderParameters GetGlobalParameters()
        {
            return new ShaderParameters();
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            throw new NotImplementedException();
        }

        public ShaderParameters GetParametersInOption(string methodName, int option, out string rmopName, out string optionName)
        {
            ShaderParameters result = new ShaderParameters();
            rmopName = "";
            optionName = "";
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
            }

            return null;
        }
    }
}
