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
    public partial class PixelShaderBlackGenerator
    {
        public static byte[] GeneratePixelShader(ShaderStage stage)
        {
            string template = $"pixl_shader_black.hlsl";

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            if (!Globals.IsShaderStageSupported(stage))
                return null;

            //
            // Create all the macros required
            //

            // prevent the definition helper from being included
            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });

            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderType>());
           
            
            byte[] shaderBytecode;

            using(FileStream test = new FileInfo("test.hlsl").Create())
            using(StreamWriter writer = new StreamWriter(test))
            {
                shaderBytecode = ShaderGeneratorBase.GenerateSource(template, macros, "entry_" + stage.ToString().ToLower(), "ps_3_0", writer);
            }

            return shaderBytecode;
        }
    }
}
