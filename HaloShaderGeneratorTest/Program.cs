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

        static void TestVertexShader(string name)
        {
            var bytecode = GenericVertexShaderGenerator.GenerateVertexShader(name);
            var str = D3DCompiler.Disassemble(bytecode);
            using (FileStream test = new FileInfo($"generated_{name}.vtsh").Create())
            using (StreamWriter writer = new StreamWriter(test))
            {
                writer.WriteLine(str);
            }

            Console.WriteLine(str);
        }

        static void TestPixelShader(string name)
        {
            var bytecode = GenericPixelShaderGenerator.GeneratePixelShader(name);
            var str = D3DCompiler.Disassemble(bytecode);
            using (FileStream test = new FileInfo($"generated_{name}.vtsh").Create())
            using (StreamWriter writer = new StreamWriter(test))
            {
                writer.WriteLine(str);
            }

            Console.WriteLine(str);
        }

        static int Main()
        {
            //TestVertexShader("chud_cortana_composite");
            TestPixelShader("chud_cortana_composite");
            /*
            var stage = ShaderStage.Static_Sh;

            TestPixelShader(stage);
            TestSharedVertexShader(VertexType.World, stage);
            TestSharedVertexShader(VertexType.Rigid, stage);
            TestSharedVertexShader(VertexType.Skinned, stage);
            */
            return 0;
        }
    }
}
