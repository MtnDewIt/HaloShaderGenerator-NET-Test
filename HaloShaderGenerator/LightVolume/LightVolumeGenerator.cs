using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;

namespace HaloShaderGenerator.LightVolume
{
    public class LightVolumeGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;
        private bool ApplyFixes;

        Albedo albedo;
        Blend_Mode blend_mode;
        Fog fog;

        /// <summary>
        /// Generator insantiation for shared shaders. Does not require method options.
        /// </summary>
        public LightVolumeGenerator() { TemplateGenerationValid = false; }

        /// <summary>
        /// Generator instantiation for method specific shaders.
        /// </summary>
        /// <param name="albedo"></param>
        /// <param name="blend_mode"></param>
        /// <param name="fog"></param>
        public LightVolumeGenerator(Albedo albedo, Blend_Mode blend_mode, Fog fog, bool applyFixes = false)
        {
            this.albedo = albedo;
            this.blend_mode = blend_mode;
            this.fog = fog;

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public ShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderType>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Albedo>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Blend_Mode>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Fog>());

            macros.Add(ShaderGeneratorBase.CreateMacro("APPLY_HLSL_FIXES", ApplyFixes));

            //
            // The following code properly names the macros (like in rmdf)
            //

            macros.Add(ShaderGeneratorBase.CreateMacro("light_volume_albedo", albedo, "albedo_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("light_volume_blend_mode", blend_mode, "blend_mode_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("light_volume_fog", fog, "fog_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("shaderstage", entryPoint, "k_shaderstage_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("shadertype", entryPoint, "shadertype_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("light_volume_albedo_arg", albedo, "k_light_volume_albedo_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("light_volume_blend_type_arg", blend_mode, "k_light_volume_blend_mode_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("fog_arg", fog, "k_fog_"));


            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"pixl_light_volume.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

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

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"glps_light_volume.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_vertex_transform", vertexType, "calc_vertex_transform_", ""));
            macros.Add(ShaderGeneratorBase.CreateMacro("transform_unknown_vector", vertexType, "transform_unknown_vector_", ""));
            macros.Add(ShaderGeneratorBase.CreateVertexMacro("input_vertex_format", vertexType));

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource(@"glvs_light_volume.hlsl", macros, $"entry_{entryPoint.ToString().ToLower()}", "vs_3_0");

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
            return System.Enum.GetValues(typeof(LightVolumeMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((LightVolumeMethods)methodIndex)
            {
                case LightVolumeMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case LightVolumeMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
                case LightVolumeMethods.Fog:
                    return Enum.GetValues(typeof(Fog)).Length;
            }

            return -1;
        }

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((LightVolumeMethods)methodIndex)
            {
                case LightVolumeMethods.Albedo:
                    return (int)albedo;
                case LightVolumeMethods.Blend_Mode:
                    return (int)blend_mode;
                case LightVolumeMethods.Fog:
                    return (int)fog;
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

        public bool IsPixelShaderShared(ShaderStage entryPoint)
        {
            return false;
        }

        public bool IsVertexFormatSupported(VertexType vertexType)
        {
            return vertexType == VertexType.LightVolume;
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
            }

            return result;
        }

        public ShaderParameters GetVertexShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;

            var result = new ShaderParameters();

            result.AddPrefixedFloat4VertexParameter("blend_mode", "category_");
            result.AddPrefixedFloat4VertexParameter("fog", "category_");

            return result;
        }

        public ShaderParameters GetGlobalParameters()
        {
            return new ShaderParameters();
        }
    }
}
