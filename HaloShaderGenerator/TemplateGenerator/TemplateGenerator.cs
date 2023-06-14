using HaloShaderGenerator.Custom;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;
using System.Collections.Generic;

namespace HaloShaderGenerator.TemplateGenerator
{
    public class TemplateGenerator
    {
        private bool ApplyFixes;
        private List<OptionInfo> CurrentOptions = null;

        public TemplateGenerator(bool applyFixes = false) => ApplyFixes = applyFixes;

        private Shared.Blend_Mode GetBlendMode()
        {
            var blendIndex = CurrentOptions.FindIndex(x => x.Category == "blend_mode");
            if (blendIndex != -1)
                return (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), CurrentOptions[blendIndex].Option, true);
            return Shared.Blend_Mode.Opaque;
        }

        private Shader.Misc GetMisc()
        {
            var miscIndex = CurrentOptions.FindIndex(x => x.Category == "misc");
            if (miscIndex != -1)
            {
                if (CurrentOptions[miscIndex].Option == @"first_person_never_w/rotating_bitmaps")
                    return Shader.Misc.First_Person_Never_With_rotating_Bitmaps;
                return (Shader.Misc)Enum.Parse(typeof(Misc), CurrentOptions[miscIndex].Option, true);
            }
            return Shader.Misc.First_Person_Never;
        }

        private static bool SsrEnable(ShaderType shaderType) // todo review
        {
            switch (shaderType)
            {
                case ShaderType.Shader:
                case ShaderType.Halogram:
                case ShaderType.Black:
                case ShaderType.Cortana:
                case ShaderType.Terrain:
                case ShaderType.Foliage:
                case ShaderType.Glass:
                case ShaderType.Water:
                case ShaderType.Custom:
                    return true;
                default:
                    return false;
            }
        }

        private string GetSourceFilename(ShaderType shaderType)
        {
            switch (shaderType)
            {
                default:
                    return shaderType.ToString().ToLower() + ".fx";
            }
        }

        private string GetEntryName(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Static_Prt_Ambient:
                    return "static_prt_ps";
                case ShaderStage.Dynamic_Light_Cinematic:
                    return "dynamic_light_cine_ps";
                default:
                    return entryPoint.ToString().ToLower() + "_ps";
            }
        }

        public static void CreateGlobalMacros(List<D3D.SHADER_MACRO> macros, 
            ShaderType shaderType,
            ShaderStage entryPoint, 
            Shared.Blend_Mode blendMode, 
            Shader.Misc misc,
            bool applyFixes)
        {
            macros.Add(ShaderGeneratorBase.CreateMacro("PIXEL_SHADER", "1"));
            macros.Add(ShaderGeneratorBase.CreateMacro("entry_point", entryPoint.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateMacro("vertex_type", "world", "s_", "_vertex"));
            macros.Add(ShaderGeneratorBase.CreateMacro("deform", "world", "deform_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("SHADER_30", "1"));
            macros.Add(ShaderGeneratorBase.CreateMacro("pc", "1"));
            macros.Add(ShaderGeneratorBase.CreateMacro("DX_VERSION", "9"));
            macros.Add(ShaderGeneratorBase.CreateMacro("disable_register_reorder", "1"));

            if (SsrEnable(shaderType))
            {
                switch (entryPoint)
                {
                    case ShaderStage.Static_Per_Pixel:
                    case ShaderStage.Static_Per_Vertex:
                    case ShaderStage.Static_Sh:
                    case ShaderStage.Static_Prt_Ambient:
                    case ShaderStage.Static_Prt_Linear:
                    case ShaderStage.Static_Prt_Quadratic:
                    case ShaderStage.Dynamic_Light:
                    case ShaderStage.Dynamic_Light_Cinematic:
                    case ShaderStage.Active_Camo:
                    case ShaderStage.Lightmap_Debug_Mode:
                    case ShaderStage.Static_Per_Vertex_Color:
                    case ShaderStage.Z_Only:
                    case ShaderStage.Sfx_Distort:
                    case ShaderStage.Water_Tessellation:
                    case ShaderStage.Water_Shading:
                        macros.Add(ShaderGeneratorBase.CreateMacro("SSR_ENABLE", "1"));
                        break;
                }
            }

            if (blendMode != Shared.Blend_Mode.Opaque ||
                (misc != Shader.Misc.First_Person_Never && 
                misc != Shader.Misc.First_Person_Never_With_rotating_Bitmaps))
                macros.Add(ShaderGeneratorBase.CreateMacro("maybe_calc_albedo", "1"));

            if (applyFixes)
                macros.Add(ShaderGeneratorBase.CreateMacro("APPLY_FIXES", "1"));
        }

        private static List<OptionInfo> ValidateOptionInfo(List<OptionInfo> options)
        {
            List<OptionInfo> newOptions = new List<OptionInfo>();
            foreach (var option in options)
            {
                if (option.PsMacro == "invalid" || 
                    option.PsMacroValue == "invalid")
                    continue;
                newOptions.Add(option);
            }
            return newOptions;
        }

        public ShaderGeneratorResult GeneratePixelShader(ShaderType shaderType, ShaderStage entryPoint, List<OptionInfo> options)
        {
            CurrentOptions = ValidateOptionInfo(options);

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            CreateGlobalMacros(macros, shaderType, entryPoint, GetBlendMode(), GetMisc(), ApplyFixes);

            foreach (var option in CurrentOptions)
                macros.Add(ShaderGeneratorBase.CreateMacro(option.PsMacro, option.PsMacroValue));

            string entryName = GetEntryName(entryPoint);
            string filename = GetSourceFilename(shaderType);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            CurrentOptions = null;
            return new ShaderGeneratorResult(bytecode);
        }
    }
}
