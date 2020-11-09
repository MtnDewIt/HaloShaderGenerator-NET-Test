using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;

namespace HaloShaderGenerator.Screen
{
    public class ScreenGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;
        private bool ApplyFixes;

        Warp warp;
        Base _base;
        Overlay_A overlay_a;
        Overlay_B overlay_b;
        Blend_Mode blend_mode;

        /// <summary>
        /// Generator insantiation for shared shaders. Does not require method options.
        /// </summary>
        public ScreenGenerator() { TemplateGenerationValid = false; }

        /// <summary>
        /// Generator instantiation for method specific shaders.
        /// </summary>
        public ScreenGenerator(Warp warp, Base _base, Overlay_A overlay_a, Overlay_B overlay_b, Blend_Mode blend_mode, bool applyFixes = false)
        {
            this.warp = warp;
            this._base = _base;
            this.overlay_a = overlay_a;
            this.overlay_b = overlay_b;
            this.blend_mode = blend_mode;

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public ScreenGenerator(byte[] options, bool applyFixes = false)
        {
            this.warp = (Warp)options[0];
            this._base = (Base)options[1];
            this.overlay_a = (Overlay_A)options[2];
            this.overlay_b = (Overlay_B)options[3];
            this.blend_mode = (Blend_Mode)options[4];

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
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.ShaderType>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Warp>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Base>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Overlay_A>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Overlay_B>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Blend_Mode>());

            //
            // Convert to shared enum
            //

            var sBlendMode = Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            //
            // The following code properly names the macros (like in rmdf)
            //

            macros.Add(ShaderGeneratorBase.CreateMacro("calc_screen_warp", warp, "calc_screen_warp_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_base", _base, "calc_base_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("overlay_type_a", overlay_a, "overlay_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("overlay_type_b", overlay_b, "overlay_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", sBlendMode, "blend_type_")); 

            macros.Add(ShaderGeneratorBase.CreateMacro("shaderstage", entryPoint, "k_shaderstage_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("shadertype", Shared.ShaderType.Screen, "k_shadertype_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("blend_type_arg", sBlendMode, "k_blend_mode_"));

            //macros.Add(ShaderGeneratorBase.CreateMacro("APPLY_HLSL_FIXES", ApplyFixes));

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"pixl_screen.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.ShaderType>());

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"glps_screen.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

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

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource(@"glvs_screen.hlsl", macros, $"entry_{entryPoint.ToString().ToLower()}", "vs_3_0");

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
            return System.Enum.GetValues(typeof(ScreenMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((ScreenMethods)methodIndex)
            {
                case ScreenMethods.Warp:
                    return Enum.GetValues(typeof(Warp)).Length;
                case ScreenMethods.Base:
                    return Enum.GetValues(typeof(Base)).Length;
                case ScreenMethods.Overlay_A:
                    return Enum.GetValues(typeof(Overlay_A)).Length;
                case ScreenMethods.Overlay_B:
                    return Enum.GetValues(typeof(Overlay_B)).Length;
                case ScreenMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
            }

            return -1;
        }

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((ScreenMethods)methodIndex)
            {
                case ScreenMethods.Warp:
                    return (int)warp;
                case ScreenMethods.Base:
                    return (int)_base;
                case ScreenMethods.Overlay_A:
                    return (int)overlay_a;
                case ScreenMethods.Overlay_B:
                    return (int)overlay_b;
                case ScreenMethods.Blend_Mode:
                    return (int)blend_mode;
            }
            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            if (entryPoint == ShaderStage.Default)
                return true;
            return false;
        }

        public bool IsMethodSharedInEntryPoint(ShaderStage entryPoint, int method_index)
        {
            return false;
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            return false;
        }

        public bool IsPixelShaderShared(ShaderStage entryPoint)
        {
            return false;
        }

        public bool IsVertexFormatSupported(VertexType vertexType)
        {
            return vertexType == VertexType.Screen;
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

            switch (warp)
            {
                case Warp.Pixel_Space:
                case Warp.Screen_Space:
                    result.AddSamplerParameter("warp_map");
                    result.AddFloatParameter("warp_amount");
                    break;
            }
            switch (_base)
            {
                case Base.Single_Screen_Space:
                case Base.Single_Pixel_Space:
                    result.AddSamplerParameter("base_map");
                    break;
            }
            switch (overlay_a)
            {
                case Overlay_A.Tint_Add_Color:
                    result.AddFloat4ColorParameter("tint_color");
                    result.AddFloat4ColorParameter("add_color");
                    break;
                case Overlay_A.Detail_Screen_Space:
                case Overlay_A.Detail_Pixel_Space:
                    result.AddSamplerParameter("detail_map_a");
                    result.AddFloatParameter("detail_fade_a");
                    result.AddFloatParameter("detail_multiplier_a");
                    break;
                case Overlay_A.Detail_Masked_Screen_Space:
                    result.AddSamplerParameter("detail_map_a");
                    result.AddSamplerParameter("detail_mask_a");
                    result.AddFloatParameter("detail_fade_a");
                    result.AddFloatParameter("detail_multiplier_a");
                    break;
            }
            switch (overlay_b)
            {
                case Overlay_B.Tint_Add_Color when overlay_a != Overlay_A.Tint_Add_Color:
                    result.AddFloat4ColorParameter("tint_color");
                    result.AddFloat4ColorParameter("add_color");
                    break;
            }
            switch (blend_mode)
            {
                case Blend_Mode.Opaque:
                    break;
                default:
                    result.AddFloatParameter("fade");
                    break;
            }

            return result;
        }

        public ShaderParameters GetVertexShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;
            var result = new ShaderParameters();

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
            rmopName = "";
            optionName = "";



            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(ScreenMethods));
        }
    }
}
