using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;

namespace HaloShaderGenerator.Shader
{
    public class GenericVertexShaderGenerator
    {
        public static byte[] GenerateVertexShader(string name)
        {
            string template = $"explicit\\vtsh_{name}.hlsl";

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();
            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource(template, macros, "main", "vs_3_0");
            return shaderBytecode;
        }
    }
}
