using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;
using System.Collections.Generic;
using System;
using HaloShaderGenerator.DirectX;

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
        Shared.Alpha_Blend_Source alpha_blend_source;

        public GlassGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        public GlassGenerator(Albedo albedo, Bump_Mapping bump_mapping, Material_Model material_model, Environment_Mapping environment_mapping, Wetness wetness, Shared.Alpha_Blend_Source alpha_blend_source, bool applyFixes = false)
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

            // TODO: Add Macros

            string entryName = entryPoint.ToString().ToLower() + "_ps";

            // TODO: Parse entry point

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"glass.fx", macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            // TODO: Parse Option Data

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            // TODO: Add Macros

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Glass);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint) 
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            // TODO: Add Macros

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Glass);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateVertexShader(VertexType vertexType, ShaderStage entryPoint) 
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");
            return null;
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
                case ShaderStage.Albedo:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Static_Per_Vertex_Color:
                case ShaderStage.Active_Camo:
                case ShaderStage.Sfx_Distort:
                case ShaderStage.Dynamic_Light:
                case ShaderStage.Dynamic_Light_Cinematic:
                case ShaderStage.Lightmap_Debug_Mode:
                case ShaderStage.Static_Sh:
                case ShaderStage.Shadow_Generate:
                    return true;

                default:
                case ShaderStage.Default:
                case ShaderStage.Z_Only:
                case ShaderStage.Water_Shading:
                case ShaderStage.Water_Tessellation:
                case ShaderStage.Shadow_Apply:
                case ShaderStage.Static_Default:
                    return false;
            }
        }

        public bool IsMethodSharedInEntryPoint(ShaderStage entryPoint, int method_index)
        {
            return method_index == 2;
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            return entryPoint == ShaderStage.Shadow_Generate;
        }

        public bool IsSharedPixelShaderWithoutMethod(ShaderStage entryPoint)
        {
            return false;
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
            return true;
        }

        public ShaderParameters GetPixelShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;
            var result = new ShaderParameters();

            // TODO: Add Parameters

            return result;
        }

        public ShaderParameters GetVertexShaderParameters()
        {
            return new ShaderParameters();
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();

            // TODO: Add Parameters

            return result;
        }

        public ShaderParameters GetParametersInOption(string methodName, int option, out string rmopName, out string optionName)
        {
            ShaderParameters result = new ShaderParameters();
            rmopName = null;
            optionName = null;

            // TODO: Add Methods

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
    }
}
