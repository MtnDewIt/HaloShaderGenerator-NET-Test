using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shader;
using HaloShaderGenerator.Black;
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
        static void WriteShaderFile(string name, string disassembly)
        {
            using (FileStream test = new FileInfo(name).Create())
            using (StreamWriter writer = new StreamWriter(test))
            {
                writer.WriteLine(disassembly);
            }
        }

        static void TestPixelShader(ShaderStage stage, Albedo albedo, Bump_Mapping bump_mapping, Alpha_Test alpha_test, Specular_Mask specular_mask, Material_Model material_model,
            Environment_Mapping environment_mapping, Self_Illumination self_illumination, Blend_Mode blend_mode, Parallax parallax, Misc misc, Distortion distortion)
        {
            var gen = new ShaderGenerator(albedo, bump_mapping, alpha_test, specular_mask, material_model, environment_mapping, self_illumination, blend_mode, parallax, misc, distortion);
            var bytecode = gen.GeneratePixelShader(stage).Bytecode;
            var parameters = gen.GetPixelShaderParameters();

            foreach(var param in parameters.GetRealParameters())
            {
                Console.WriteLine(param.ParameterName);
            }

            var disassembly = D3DCompiler.Disassemble(bytecode);
            string filename = $"generated_{stage.ToString().ToLower()}_{(int)albedo}_{(int)bump_mapping}_{(int)alpha_test}_{(int)specular_mask}_{(int)material_model}_{(int)environment_mapping}_{(int)self_illumination}_{(int)blend_mode}_{(int)parallax}_{(int)misc}_{(int)distortion}.pixl";
            WriteShaderFile(filename, disassembly);
        }

        static void TestSharedVertexShader(VertexType vertexType, ShaderStage stage)
        {
            var gen = new ShaderGenerator();
            var bytecode = gen.GenerateSharedVertexShader(vertexType, stage).Bytecode;

            var disassembly = D3DCompiler.Disassemble(bytecode);
            WriteShaderFile($"generated_{stage.ToString().ToLower()}_{vertexType.ToString().ToLower()}.glvs", disassembly);
        }

        static void TestSharedPixelShader(ShaderStage stage, int methodIndex, int optionIndex)
        {
            var gen = new ShaderGenerator();
            var bytecode = gen.GenerateSharedPixelShader(stage, methodIndex, optionIndex).Bytecode;
            var disassembly = D3DCompiler.Disassemble(bytecode);
            WriteShaderFile($"generated_{stage.ToString().ToLower()}_{methodIndex}_{optionIndex}.glps", disassembly);
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
            using (FileStream test = new FileInfo($"generated_{name}.pixl").Create())
            using (StreamWriter writer = new StreamWriter(test))
            {
                writer.WriteLine(str);
            }

            Console.WriteLine(str);
        }

        static int Main()
        {
            //TestVertexShader("chud_cortana_composite");
            //TestPixelShader("chud_cortana_composite");
  
            
            var stage = ShaderStage.Albedo;

            TestPixelShader(stage, Albedo.Two_Change_Color_Anim_Overlay, Bump_Mapping.Standard, Alpha_Test.Off, Specular_Mask.No_Specular_Mask, 
                Material_Model.Diffuse_Only, Environment_Mapping.None, Self_Illumination.Off, Blend_Mode.Opaque, Parallax.Off, Misc.First_Person_Never, Distortion.Off);
            //TestSharedVertexShader(VertexType.World, stage);
            //TestSharedVertexShader(VertexType.Rigid, stage);
            //TestSharedVertexShader(VertexType.Skinned, stage);
            //TestSharedPixelShader(stage, (int)Shader.ShaderMethods.Alpha_Test, (int)Shader.Alpha_Test.Off);
            //TestSharedPixelShader(stage, (int)Shader.ShaderMethods.Alpha_Test, (int)Shader.Alpha_Test.On);

            /*
            TestPixelBlack(ShaderStage.Albedo);
            TestSharedVertexBlack(VertexType.World, ShaderStage.Albedo);
            TestSharedVertexBlack(VertexType.Rigid, ShaderStage.Albedo);
            TestSharedVertexBlack(VertexType.Skinned, ShaderStage.Albedo);
            */
            return 0;
        }

        static void TestPixelBlack(ShaderStage stage)
        {
            var gen = new ShaderBlackGenerator();
            var bytecode = gen.GeneratePixelShader(stage).Bytecode;
            WriteShaderFile($"generated_shader_black_{stage.ToString().ToLower()}_0.pixl", D3DCompiler.Disassemble(bytecode));
        }

        static void TestSharedVertexBlack(VertexType vertexType, ShaderStage stage)
        {
            var gen = new ShaderBlackGenerator();
            var bytecode = gen.GenerateSharedVertexShader(vertexType, stage).Bytecode;
            WriteShaderFile($"generated_shader_black_{stage.ToString().ToLower()}_{vertexType.ToString().ToLower()}.glvs", D3DCompiler.Disassemble(bytecode));
        }
    }
}
