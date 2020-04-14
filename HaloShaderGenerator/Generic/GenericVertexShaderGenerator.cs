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
    public class GenericVertexShaderGenerator
    {
        public static byte[] GenerateVertexShader(string name)
        {
            // TODO: find the stages in the executable and make an enum for them, not the same as templated shaders

            string template = $"vtsh_{name}.hlsl";

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            // prevent the definition helper from being included
            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });

            byte[] shaderBytecode;

            shaderBytecode = ShaderGeneratorBase.GenerateSource(template, macros, "main", "vs_3_0");

            return shaderBytecode;
        }
    }

    

}
