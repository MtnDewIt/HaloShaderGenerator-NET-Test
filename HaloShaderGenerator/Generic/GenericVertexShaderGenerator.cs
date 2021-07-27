using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;

namespace HaloShaderGenerator.Shader
{
    public class GenericVertexShaderGenerator
    {
        public static ShaderGeneratorResult GenerateVertexShader(string name, ShaderStage entry, bool chud = false)
        {
            string template = $"{(chud ? "chud" : "explicit")}\\{name}.hlsl";

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();
            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.Add(new D3D.SHADER_MACRO { Name = "VERTEX_SHADER", Definition = "1" });

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource(template, macros, $"vs_{entry.ToString().ToLower()}", "vs_3_0");
            return new ShaderGeneratorResult(shaderBytecode);
        }
    }
}
