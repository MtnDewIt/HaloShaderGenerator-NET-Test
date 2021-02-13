using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;

namespace HaloShaderGenerator.Shader
{
    public class GenericPixelShaderGenerator
    {
        public static ShaderGeneratorResult GeneratePixelShader(string name)
        {
            string template = $"explicit\\pixl_{name}.hlsl";

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();
            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource(template, macros, "main", "ps_3_0");
            return new ShaderGeneratorResult(shaderBytecode);
        }
    }
}
