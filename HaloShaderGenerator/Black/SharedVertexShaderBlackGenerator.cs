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

namespace HaloShaderGenerator.Black
{
    public class SharedVertexBlackGenerator
    {
        public static byte[] GenerateSharedVertexShaderBlack(VertexType vertexType, ShaderStage stage)
        {
            if (!Globals.IsVertexTypeSupported(vertexType) || !Globals.IsShaderStageSupported(stage))
                return null;

            string template = @"glvs_shader_black.hlsl";

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            // prevent the definition helper from being included
            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });

            macros.Add(ShaderGeneratorBase.CreateMacro("calc_vertex_transform", vertexType, "calc_vertex_transform_", ""));
            macros.Add(ShaderGeneratorBase.CreateVertexMacro("input_vertex_format", vertexType));

            byte[] shaderBytecode;

            shaderBytecode = ShaderGeneratorBase.GenerateSource(template, macros, $"entry_{stage.ToString().ToLower()}", "vs_3_0");

            return shaderBytecode;
        }
    }

    

}
