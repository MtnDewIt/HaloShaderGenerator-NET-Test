using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.LegacyGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.LegacyGenerator.ZOnly
{
    public class LegacyZOnlyGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;
        private bool ApplyFixes;

        Test test;

        /// <summary>
        /// Generator insantiation for shared shaders. Does not require method options.
        /// </summary>
        public LegacyZOnlyGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        /// <summary>
        /// Generator instantiation for method specific shaders.
        /// </summary>
        public LegacyZOnlyGenerator(Test test)
        {
            this.test = test;
            TemplateGenerationValid = true;
        }

        public LegacyZOnlyGenerator(byte[] options, bool applyFixes = false)
        {
            options = ValidateOptions(options);

            this.test = (Test)options[0];

            //ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public LegacyShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.ShaderType>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<Test>());

            //
            // The following code properly names the macros (like in rmdf)
            //

            macros.Add(LegacyShaderGeneratorBase.CreateMacro("shaderstage", entryPoint, "k_shaderstage_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("shadertype", Shared.ShaderType.ZOnly, "k_shadertype_"));

            macros.Add(LegacyShaderGeneratorBase.CreateMacro("test_arg", test, "k_test_"));

            byte[] shaderBytecode = LegacyShaderGeneratorBase.GenerateSource($"pixl_zonly.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new LegacyShaderGeneratorResult(shaderBytecode);
        }

        public LegacyShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderType>());

            byte[] shaderBytecode = LegacyShaderGeneratorBase.GenerateSource($"glps_zonly.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new LegacyShaderGeneratorResult(shaderBytecode);
        }

        public LegacyShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_VERTEX_SHADER_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<VertexType>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.ShaderType>());
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("calc_vertex_transform", vertexType, "calc_vertex_transform_", ""));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("transform_dominant_light", vertexType, "transform_dominant_light_", ""));
            //macros.Add(LegacyShaderGeneratorBase.CreateMacro("calc_distortion", vertexType, "calc_distortion_", ""));
            macros.Add(LegacyShaderGeneratorBase.CreateVertexMacro("input_vertex_format", vertexType));

            macros.Add(LegacyShaderGeneratorBase.CreateMacro("shaderstage", entryPoint, "k_shaderstage_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("vertextype", vertexType, "k_vertextype_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("shadertype", Shared.ShaderType.ZOnly, "shadertype_"));

            byte[] shaderBytecode = LegacyShaderGeneratorBase.GenerateSource(@"glvs_zonly.hlsl", macros, $"entry_{entryPoint.ToString().ToLower()}", "vs_3_0");

            return new LegacyShaderGeneratorResult(shaderBytecode);
        }

        public LegacyShaderGeneratorResult GenerateVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");
            return null;
        }

        public int GetMethodCount()
        {
            return 1;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            return 1;
        }

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((ZOnlyMethods)methodIndex)
            {
                case ZOnlyMethods.Test:
                    return (int)test;
            }
            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            return entryPoint == ShaderStage.Z_Only;
        }

        public bool IsMethodSharedInEntryPoint(ShaderStage entryPoint, int method_index)
        {
            return false;
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
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
            return true;
        }

        public ShaderParameters GetPixelShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;
            var result = new ShaderParameters();

            return result;
        }

        public ShaderParameters GetVertexShaderParameters()
        {
            return new ShaderParameters();
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();
            // technically these are still here at the point of rendering
            //result.AddSamplerWithoutXFormParameter("albedo_texture", RenderMethodExtern.texture_global_target_texaccum);
            //result.AddSamplerWithoutXFormParameter("normal_texture", RenderMethodExtern.texture_global_target_normal);
            //result.AddSamplerWithoutXFormParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture);
            //result.AddSamplerWithoutXFormParameter("shadow_depth_map_1", RenderMethodExtern.texture_global_target_shadow_buffer1);
            //result.AddSamplerWithoutXFormParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0);
            //result.AddFloat4Parameter("debug_tint", RenderMethodExtern.debug_tint);
            //result.AddSamplerWithoutXFormParameter("active_camo_distortion_texture", RenderMethodExtern.active_camo_distortion_texture);
            //result.AddSamplerWithoutXFormParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            //result.AddSamplerWithoutXFormParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture);
            //result.AddSamplerWithoutXFormParameter("dominant_light_intensity_map", RenderMethodExtern.texture_dominant_light_intensity_map);
            return result;
        }

        public ShaderParameters GetParametersInOption(string methodName, int option, out string rmopName, out string optionName)
        {
            ShaderParameters result = new ShaderParameters();
            rmopName = null;
            optionName = null;

            if (methodName == "test")
            {
                optionName = ((Test)option).ToString();
                switch ((Test)option)
                {
                    case Test.Default:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddFloat4Parameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_default";
                        break;
                }
            }

            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(ZOnlyMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((ZOnlyMethods)methodIndex)
            {
                case ZOnlyMethods.Test:
                    return Enum.GetValues(typeof(Test));
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
    }
}
