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
    public class ShartedVertexShaderGenerator
    {
        public static bool IsVertexTypeSupported(VertexType type)
        {
            switch (type)
            {
                case VertexType.World:
                case VertexType.Rigid:
                case VertexType.Skinned:
                //case VertexType.DualQuat: //HO only
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsShaderStageSupported(ShaderStage stage)
        {
            switch (stage)
            {
                case ShaderStage.Albedo:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Sfx_Distort:
                    return true;
                case ShaderStage.Active_Camo:
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Static_Sh:
                case ShaderStage.Dynamic_Light:
                case ShaderStage.Shadow_Generate:
                case ShaderStage.Lightmap_Debug_Mode:
                case ShaderStage.Static_Per_Vertex_Color:
                case ShaderStage.Dynamic_Light_Cinematic:
                    Console.Error.WriteLine($"Shader stage {stage} not implemented in generator");
                    return false;

                default:
                case ShaderStage.Z_Only:
                case ShaderStage.Water_Shading:
                case ShaderStage.Water_Tesselation:
                case ShaderStage.Shadow_Apply:
                case ShaderStage.Static_Default:
                case ShaderStage.Default:
                    return false;
            }
        }

        public static byte[] GenerateSharedVertexShader(VertexType vertexType, ShaderStage stage)
        {
            if (!IsVertexTypeSupported(vertexType) || !IsShaderStageSupported(stage))
                return null;



            string template = @"glvs_shader.hlsl";

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            // prevent the definition helper from being included
            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });

            macros.Add(ShaderGeneratorBase.CreateMacro("calc_vertex_transform", vertexType, "calc_vertex_transform_", ""));
            macros.Add(ShaderGeneratorBase.CreateMacro("transform_unknown_vector", vertexType, "transform_unknown_vector_", ""));
            macros.Add(ShaderGeneratorBase.CreateVertexMacro("input_vertex_format", vertexType));

            byte[] shaderBytecode;

            using(FileStream test = new FileInfo("test.hlsl").Create())
            using(StreamWriter writer = new StreamWriter(test))
            {
                shaderBytecode = ShaderGeneratorBase.GenerateSource(template, macros, $"entry_{stage.ToString().ToLower()}", "vs_3_0", writer);
            }

            return shaderBytecode;
        }
    }

    

}
