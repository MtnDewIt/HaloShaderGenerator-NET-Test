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
        static void TestPixelShader()
        {
            var bytecode = PixelShaderGenerator.GeneratePixelShader(ShaderStage.Albedo,
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

            using (FileStream test = new FileInfo($"generated_{ShaderStage.Albedo.ToString()}_0_0_0_0_0_0_0_0_0_0_0.pixel_shader").Create())
            using (StreamWriter writer = new StreamWriter(test))
            {
                writer.WriteLine(str);
            }

            Console.WriteLine(str);
        }

        static void TestSharedVertexShader()
        {
            var bytecode = ShartedVertexShaderGenerator.GenerateSharedVertexShader(VertexType.Rigid, ShaderStage.Albedo);
            var str = D3DCompiler.Disassemble(bytecode);

            using (FileStream test = new FileInfo($"generated_{ShaderStage.Albedo.ToString().ToLower()}_rigid.glvs").Create())
            using (StreamWriter writer = new StreamWriter(test))
            {
                writer.WriteLine(str);
            }

            Console.WriteLine(str);
        }

        static int Main()
        {
            //TestPixelShader();
            TestSharedVertexShader();

            return 0;
        }
    }
}
