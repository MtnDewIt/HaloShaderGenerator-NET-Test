using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;

namespace HaloShaderGenerator.Shader
{
    public class GenericPixelShaderGenerator
    {
        public static ShaderGeneratorResult GeneratePixelShader(string name, ShaderStage entry, bool chud = false)
        {
            string template = $"{(chud ? "chud" : "explicit")}\\{name}.hlsl";

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();
            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.Add(new D3D.SHADER_MACRO { Name = "PIXEL_SHADER", Definition = "1" });

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource(template, macros, $"ps_{entry.ToString().ToLower()}", "ps_3_0");
            return new ShaderGeneratorResult(shaderBytecode);
        }
    }
}
