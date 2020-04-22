using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;

namespace HaloShaderGenerator.Shader
{
    public class GenericPixelShaderGenerator
    {
        public static byte[] GeneratePixelShader(string name)
        {
            string template = $"pixl_{name}.hlsl";

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });

            byte[] shaderBytecode;

            shaderBytecode = ShaderGeneratorBase.GenerateSource(template, macros, "main", "ps_3_0");

            return shaderBytecode;
        }
    }

    

}
