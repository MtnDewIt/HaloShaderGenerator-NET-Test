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
        static readonly string ShaderReferencePath = @"D:\Halo\Repositories\TagTool\TagTool\bin\x64\Debug\Shaders";

        static readonly bool UnitTest = true;
        static readonly bool TestSpecificShader = true;

        static readonly List<ShaderStage> StageOverrides = new List<ShaderStage> { ShaderStage.Static_Per_Pixel };

        static readonly List<int> AlbedoOverrides = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
        static readonly List<int> BumpOverrides =       new List<int> { };
        static readonly List<int> AlphaOverrides =      new List<int> { 1};
        static readonly List<int> SpecularOverrides =   new List<int> { };
        static readonly List<int> MaterialOverrides =   new List<int> { 0};
        static readonly List<int> EnvOverrides =        new List<int> { };
        static readonly List<int> SelfIllumOverrides =  new List<int> { };
        static readonly List<int> BlendModeOverrides =  new List<int> { };
        static readonly List<int> ParallaxOverrides =   new List<int> { };
        static readonly List<int> MiscOverrides =       new List<int> { };

        static readonly List<List<int>> ShaderOverrides = new List<List<int>> 
        {
            new List<int> { 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0 },
        };

        //new List<int> { 11, 1, 0, 2, 1, 4, 0, 0, 0, 1, 0 },

        static int Main()
        {
            if (TestSpecificShader)
            {
                foreach (var stage in StageOverrides)
                {
                    foreach (var methods in ShaderOverrides)
                    {
                        TestPixelShader(stage, (Albedo)methods[0], (Bump_Mapping)methods[1], (Alpha_Test)methods[2], (Specular_Mask)methods[3], (Material_Model)methods[4],
                            (Environment_Mapping)methods[5], (Self_Illumination)methods[6], (Blend_Mode)methods[7], (Parallax)methods[8], (Misc)methods[9], (Distortion)methods[10]);
                    }
                }
            }

            if (UnitTest)
            {
                ShaderUnitTest shaderTests = new ShaderUnitTest(ShaderReferencePath);
                var methodOverrides = new List<List<int>> { AlbedoOverrides, BumpOverrides, AlphaOverrides, SpecularOverrides, MaterialOverrides,
                EnvOverrides, SelfIllumOverrides, BlendModeOverrides, ParallaxOverrides, MiscOverrides, new List<int>() };

                shaderTests.TestAllPixelShaders(ShaderOverrides, StageOverrides, methodOverrides);
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


        static void TestPixelBlack()
        {
            var gen = new ShaderBlackGenerator();
            var bytecode = gen.GeneratePixelShader(ShaderStage.Albedo).Bytecode;
            WriteShaderFile($"generated_shader_black_{ShaderStage.Albedo.ToString().ToLower()}_0.pixl", D3DCompiler.Disassemble(bytecode));
        }

        static void TestSharedVertexBlack(VertexType vertexType, ShaderStage stage)
        {
            var gen = new ShaderBlackGenerator();
            var bytecode = gen.GenerateSharedVertexShader(vertexType, stage).Bytecode;
            WriteShaderFile($"generated_shader_black_{stage.ToString().ToLower()}_{vertexType.ToString().ToLower()}.glvs", D3DCompiler.Disassemble(bytecode));
        }
    }
}
