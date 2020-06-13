using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shader;
using HaloShaderGenerator.Black;
using System;
using System.Collections.Generic;
using System.IO;

namespace HaloShaderGenerator
{
    class Application
    {
        static readonly bool UnitTest = true;
        static readonly bool TestSpecificShader = false;


        static int Main()
        {
            if (TestSpecificShader)
            {
                List<ShaderStage> stages_to_gen_prt = new List<ShaderStage> { ShaderStage.Static_Sh, ShaderStage.Static_Prt_Ambient, ShaderStage.Static_Prt_Linear, ShaderStage.Static_Prt_Quadratic };
                List<ShaderStage> stages_to_gen = new List<ShaderStage> {ShaderStage.Albedo};

                List<List<int>> shaders_to_gen = new List<List<int>> {
                    new List<int> { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                    new List<int> {0,0,0,0,0,0,0,0,0,0,0 },
                //new List<int> {0,0,1,0,0,0,0,0,0,2,0 },
                };


                foreach (var stage in stages_to_gen)
                {
                    foreach (var methods in shaders_to_gen)
                    {
                        TestPixelShader(stage, (Albedo)methods[0], (Bump_Mapping)methods[1], (Alpha_Test)methods[2], (Specular_Mask)methods[3], (Material_Model)methods[4],
                            (Environment_Mapping)methods[5], (Self_Illumination)methods[6], (Blend_Mode)methods[7], (Parallax)methods[8], (Misc)methods[9], (Distortion)methods[10]);
                    }

                }
            }

            if (UnitTest)
            {
                PixelShaderUnitTest.RunTests();
                Console.ReadLine();
            }

            return 0;
        }

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
