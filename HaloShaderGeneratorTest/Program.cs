using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloShaderGenerator
{
    class Application
    {
        static void TestPixelShader(ShaderStage stage)
        {
            var bytecode = PixelShaderGenerator.GeneratePixelShader(stage,
                Albedo.Default,
                Bump_Mapping.Off,
                Alpha_Test.None,
                Specular_Mask.No_Specular_Mask,
                Material_Model.Diffuse_Only,
                Environment_Mapping.None,
                Self_Illumination.Off,
                Blend_Mode.Opaque,
                Parallax.Off,
                Misc.First_Person_Never,
                Distortion.Off);

            var str = D3DCompiler.Disassemble(bytecode);

            using (FileStream test = new FileInfo($"generated_{stage.ToString().ToLower()}_0_0_0_0_0_0_0_0_0_0_0.pixl").Create())
            using (StreamWriter writer = new StreamWriter(test))
            {
                writer.WriteLine(str);
            }

            Console.WriteLine(str);
        }

        static void TestSharedVertexShader(VertexType vertexType, ShaderStage stage)
        {
            var bytecode = ShartedVertexShaderGenerator.GenerateSharedVertexShader(vertexType, stage);
            var str = D3DCompiler.Disassemble(bytecode);

            using (FileStream test = new FileInfo($"generated_{stage.ToString().ToLower()}_{vertexType.ToString().ToLower()}.glvs").Create())
            using (StreamWriter writer = new StreamWriter(test))
            {
                writer.WriteLine(str);
            }

            Console.WriteLine(str);
        }

        static int Main()
        {
            
            TestPixelShader(ShaderStage.Albedo);
            TestPixelShader(ShaderStage.Static_Prt_Ambient);
            TestPixelShader(ShaderStage.Static_Prt_Linear);
            TestPixelShader(ShaderStage.Static_Prt_Quadratic);
            
            TestSharedVertexShader(VertexType.Rigid, ShaderStage.Albedo);
            TestSharedVertexShader(VertexType.Rigid, ShaderStage.Static_Prt_Ambient);

            return 0;
        }
    }
}
