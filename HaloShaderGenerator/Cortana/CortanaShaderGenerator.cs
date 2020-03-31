using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Enums;
using HaloShaderGenerator.Generator;

namespace HaloShaderGenerator.Shader
{
    public partial class CortanaShaderGenerator
    {
        public static bool IsShaderStageSupported(ShaderStage stage)
        {
            switch (stage)
            {
                case ShaderStage.Active_Camo:
                    return true;
            }
            return false;
        }

        public static byte[] GenerateShaderCortana(
            ShaderStage stage
            )
        {
            string template = $"shader_cortana.hlsl";

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            switch (stage)
            {
                case ShaderStage.Active_Camo:
                    break;
                default:
                    return null;
            }

            // prevent the definition helper from being included
            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });

            var shader_bytecode = ShaderGeneratorBase.GenerateSource(template, macros, "entry_" + stage.ToString().ToLower(), "ps_3_0");
            return shader_bytecode;
        }
    }
}
