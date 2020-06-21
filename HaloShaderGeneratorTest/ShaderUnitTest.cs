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
    public static class PixelShaderUnitTest
    {
        static readonly string PathToReferenceShaders = @"D:\Halo\Repositories\TagTool\TagTool\bin\x64\Debug\Shaders\shader_templates";

        static string BuildShaderName(List<int> methods)
        {
            string result = "";
            foreach(var m in methods)
            {
                result += $"_{m}";
            }
            return result;
        }

        static string BuildPixelShaderEntryPointName(ShaderStage stage)
        {
            return $"{stage.ToString().ToLower()}.pixel_shader";
        }

        static void WriteShaderFile(string name, string disassembly)
        {
            using (FileStream test = new FileInfo(name).Create())
            using (StreamWriter writer = new StreamWriter(test))
            {
                writer.WriteLine(disassembly);
            }
        }

        static public void RunTests()
        {
            bool success = true;

            foreach(var testShader in TestMethods)
            {
                foreach(ShaderStage stage in Enum.GetValues(typeof(ShaderStage)))
                {
                    if (stage == ShaderStage.Default || stage == ShaderStage.Shadow_Apply || stage == ShaderStage.Shadow_Generate || stage == ShaderStage.Static_Default || stage == ShaderStage.Water_Shading || stage == ShaderStage.Water_Tesselation || stage == ShaderStage.Z_Only)
                        continue;

                    /*
                    if (stage != ShaderStage.Dynamic_Light && stage != ShaderStage.Dynamic_Light_Cinematic)
                        continue;
                    */

                    string filePath = Path.Combine(PathToReferenceShaders, BuildShaderName(testShader));
                    filePath = Path.Combine(filePath, BuildPixelShaderEntryPointName(stage));
                    var file = new FileInfo(filePath);

                    if(file.Exists == false)
                    {
                        Console.WriteLine($"No reference shader for {BuildShaderName(testShader)} at {stage.ToString().ToLower()}");
                        success = false;
                        continue;
                    }
                    var generatedDissassembly = TestPixelShader(stage, (Albedo)testShader[0], (Bump_Mapping)testShader[1], (Alpha_Test)testShader[2], (Specular_Mask)testShader[3], (Material_Model)testShader[4],
                        (Environment_Mapping)testShader[5], (Self_Illumination)testShader[6], (Blend_Mode)testShader[7], (Parallax)testShader[8], (Misc)testShader[9], (Distortion)testShader[10]);

                    

                    var generatedShaderFile = new FileInfo("unittest.shader");
                    using(var genStream = generatedShaderFile.Create())
                    using (StreamWriter writer = new StreamWriter(genStream))
                    {
                        writer.WriteLine(generatedDissassembly);
                    }
                    var referenceDissasembly = File.ReadAllText(filePath);
                    generatedDissassembly = File.ReadAllText("unittest.shader");

                    bool equal = string.Equals(generatedDissassembly, referenceDissasembly);

                    if (!equal)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Generated shader for {BuildShaderName(testShader)} at {stage.ToString().ToLower()} is not equal to reference.");
                        Console.ResetColor();
                        string filename = $"unit_test_failed_{stage.ToString().ToLower()}{BuildShaderName(testShader)}.pixl";
                        WriteShaderFile(filename, generatedDissassembly);
                    }


                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Generated shader for {BuildShaderName(testShader)} at {stage.ToString().ToLower()} is identical to reference.");
                        Console.ResetColor();
                    }
                        

                    success &= equal;
                    generatedShaderFile.Delete();
                }
            }

            if (success)
            {
                Console.WriteLine("All unit tests passed sucessfully!");
            }
            else
            {
                Console.WriteLine("Failed unit tests. See above for more details.");
            }
        }

        static string TestPixelShader(ShaderStage stage, Albedo albedo, Bump_Mapping bump_mapping, Alpha_Test alpha_test, Specular_Mask specular_mask, Material_Model material_model,
            Environment_Mapping environment_mapping, Self_Illumination self_illumination, Blend_Mode blend_mode, Parallax parallax, Misc misc, Distortion distortion)
        {
            var gen = new ShaderGenerator(albedo, bump_mapping, alpha_test, specular_mask, material_model, environment_mapping, self_illumination, blend_mode, parallax, misc, distortion);
            var bytecode = gen.GeneratePixelShader(stage).Bytecode;
            var parameters = gen.GetPixelShaderParameters();
            var result = new ShaderGeneratorResult(bytecode);
            return D3DCompiler.Disassemble(bytecode);
        }

        static readonly List<List<int>> TestMethods = new List<List<int>> {

            //new List<int> { 0, 0, 0, 1, 2, 0, 0, 3, 0, 0, 0 },
            //new List<int> { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },

            /*
            

            new List<int> { 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0 },
            new List<int> { 0, 1, 0, 2, 1, 0, 0, 0, 0, 0, 0 },*/

            //new List<int> { 0, 0, 0, 0, 0, 2, 0, 1, 0, 0, 0 },

            // not passing but fine anyway
            /*
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 3, 1, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 1, 0, 3, 0, 0, 0 },
            new List<int> { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 4, 3, 2, 0, 0 },
            new List<int> { 6, 0, 0, 0, 3, 0, 0, 1, 0, 0, 0 },
            */





            
            // material_none test, all passing
            new List<int> { 2, 0, 0, 0, 4, 0, 1, 0, 0, 1, 0 },
            new List<int> { 2, 0, 0, 0, 4, 0, 1, 1, 0, 1, 0 },
            new List<int> { 2, 0, 0, 0, 4, 0, 1, 3, 0, 1, 0 },
            new List<int> { 2, 0, 0, 0, 4, 0, 3, 1, 0, 0, 0 },
            new List<int> { 2, 0, 0, 0, 4, 0, 5, 1, 0, 0, 0 },
            new List<int> { 2, 0, 0, 0, 4, 0, 6, 1, 0, 0, 0 },
            new List<int> { 2, 0, 0, 0, 4, 0, 6, 1, 0, 1, 0 },
            new List<int> { 0, 0, 0, 0, 4, 0, 1, 1, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 4, 0, 6, 1, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 4, 0, 6, 1, 0, 1, 0 },

            // passing everything
            
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 2, 0, 1, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 3, 0, 1, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 3, 0, 2, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 4, 0, 1, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0 },

            new List<int> { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 1, 3, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 1, 3, 0, 1, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 3, 1, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 3, 1, 0, 1, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 4, 1, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 4, 3, 0, 0, 0 },

            new List<int> { 0, 0, 0, 0, 0, 0, 5, 1, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 5, 3, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 5, 3, 0, 1, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 7, 0, 0, 1, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 7, 1, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0, 0, 8, 3, 0, 0, 0 },

            new List<int> { 0, 0, 1, 0, 0, 0, 0, 0, 0, 2, 0 },
            new List<int> { 5, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            
            new List<int> { 0, 0, 0, 0, 3, 0, 3, 1, 0, 0, 0 },



        };
    }
}
