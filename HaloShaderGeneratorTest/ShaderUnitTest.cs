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
using HaloShaderGenerator.Generator;

namespace HaloShaderGenerator
{
    public class ShaderUnitTest : GenericUnitTest
    {
        public ShaderUnitTest(string referencePath) : base(referencePath, new ShaderGenerator(), "shader") { }

        public override string GeneratePixelShader(ShaderStage stage, List<int> shaderOptions)
        {
            var albedo = (Albedo)shaderOptions[0];
            var bump_mapping = (Bump_Mapping)shaderOptions[1];
            var alpha_test = (Alpha_Test)shaderOptions[2];
            var specular_mask = (Specular_Mask)shaderOptions[3];
            var material_model = (Material_Model)shaderOptions[4];
            var environment_mapping = (Environment_Mapping)shaderOptions[5];
            var self_illumination = (Self_Illumination)shaderOptions[6];
            var blend_mode = (Blend_Mode)shaderOptions[7];
            var parallax = (Parallax)shaderOptions[8];
            var misc = (Misc)shaderOptions[9];
            var distortion = (Distortion)shaderOptions[10];
            var gen = new ShaderGenerator(albedo, bump_mapping, alpha_test, specular_mask, material_model, environment_mapping, self_illumination, blend_mode, parallax, misc, distortion);
            var bytecode = gen.GeneratePixelShader(stage).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }

        public override string GenerateSharedPixelShader(ShaderStage stage, int methodIndex, int optionIndex)
        {
            var gen = new ShaderGenerator();
            var bytecode = gen.GenerateSharedPixelShader(stage, methodIndex, optionIndex).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }

        public override string GenerateVertexShader(VertexType vertex, ShaderStage stage)
        {
            var gen = new ShaderGenerator();
            var bytecode = gen.GenerateVertexShader(vertex, stage).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }

        public override string GenerateSharedVertexShader(VertexType vertex, ShaderStage stage)
        {
            var gen = new ShaderGenerator();
            var bytecode = gen.GenerateSharedVertexShader(vertex, stage).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }
    }
    
    public abstract class GenericUnitTest
    {
        private static bool IgnoreD3DX = false;
        private static string ReferencePath;
        private IShaderGenerator ReferenceGenerator;
        private static string ShaderType;

        public GenericUnitTest(string referencePath, IShaderGenerator referenceGenerator, string shaderType)
        {
            ReferencePath = referencePath;
            ReferenceGenerator = referenceGenerator;
            ShaderType = shaderType;
        }

        public static List<ShaderStage> GetAllShaderStages()
        {
            var stages = new List<ShaderStage>();
            foreach (var stage in Enum.GetValues(typeof(ShaderStage)))
                stages.Add((ShaderStage)stage);
            return stages;
        }

        public static List<VertexType> GetAllVertexFormats()
        {
            var vertices = new List<VertexType>();
            foreach (var vertex in Enum.GetValues(typeof(VertexType)))
                vertices.Add((VertexType)vertex);
            return vertices;
        }

        public static string BuildShaderName(List<int> methods)
        {
            string result = "";
            foreach (var m in methods)
            {
                result += $"_{m}";
            }
            return result;
        }

        public static string BuildPixelShaderEntryPointName(ShaderStage stage)
        {
            return $"{stage.ToString().ToLower()}.pixel_shader";
        }

        public static List<List<int>> GetAllTestPixelShaders()
        {
            var pixelShaderPath = Path.Combine(ReferencePath, $"{ShaderType}_templates");
            List<string> availableShaders = Directory.GetDirectories(pixelShaderPath).ToList();
            List<List<int>> availableShaderMethods = new List<List<int>>();
            foreach (var shader in availableShaders)
            {
                var folder = shader.Split('\\').Last();
                var methods = folder.Remove(0, 1).Split('_');
                List<int> methodIndices = new List<int>();
                foreach (var method in methods)
                    methodIndices.Add(Int32.Parse(method));
                availableShaderMethods.Add(methodIndices);
            }
            return availableShaderMethods;
        }

        public static string GetTestSharedVertexShader(VertexType vertex, ShaderStage stage)
        {
            var vertexShaderPath = Path.Combine(ReferencePath, $"{ShaderType}_shared_vertex_shaders.glvs");
            vertexShaderPath = Path.Combine(vertexShaderPath, $"{vertex.ToString().ToLower()}");
            vertexShaderPath = Path.Combine(vertexShaderPath, $"{stage.ToString().ToLower()}.shared_vertex_shader");
            return vertexShaderPath;
        }

        public static string GetTestSharedPixelShader(ShaderStage stage, int methodIndex = -1, int optionIndex = -1)
        {
            var vertexShaderPath = Path.Combine(ReferencePath, $"{ShaderType}_shared_pixel_shaders.glps");
            var filename = $"{stage.ToString().ToLower()}";
            if(methodIndex != -1 && optionIndex != -1)
            {
                filename += $"_{methodIndex}_{optionIndex}";
            }

            vertexShaderPath = Path.Combine(vertexShaderPath, $"{filename}.shared_pixel_shader");

            return vertexShaderPath;
        }

        public static void DisplayPixelShaderTestResults(bool success, List<int> testShader, ShaderStage stage, bool usesD3DX)
        {
            if (IgnoreD3DX && usesD3DX)
                return;

            if (!success)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Generated shader for {BuildShaderName(testShader)} at {stage.ToString().ToLower()} is not identical to reference." + (usesD3DX ? " USES D3DX." : ""));
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Generated shader for {BuildShaderName(testShader)} at {stage.ToString().ToLower()} is identical to reference.");
                Console.ResetColor();
            }
        }

        public static void DisplayVertexShaderTestResults(bool success, VertexType vertex, ShaderStage stage, bool usesD3DX)
        {
            if (IgnoreD3DX && usesD3DX)
                return;

            if (!success)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Generated shader for {stage.ToString().ToLower()} vertex format {vertex.ToString().ToLower()} is not identical to reference." + (usesD3DX ? " USES D3DX." : ""));
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Generated shader for {stage.ToString().ToLower()} vertex format {vertex.ToString().ToLower()} is identical to reference.");
                Console.ResetColor();
            }
        }

        public static void DisplaySharedPixelShaderTestResults(bool success, int methodIndex, int optionIndex, ShaderStage stage, bool usesD3DX)
        {
            if (IgnoreD3DX && usesD3DX)
                return;

            if (!success)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Generated shader for {stage.ToString().ToLower()}_{methodIndex}_{optionIndex} is not identical to reference." + (usesD3DX ? " USES D3DX." : ""));
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Generated shader for {stage.ToString().ToLower()}_{methodIndex}_{optionIndex} is identical to reference.");
                Console.ResetColor();
            }
        }

        public static bool CompareShaders(string generatedDissassembly, string filePath, string version, out bool usesD3DX)
        {
            var generatedShaderFile = new FileInfo("unittest.shader");
            using (var genStream = generatedShaderFile.Create())
            using (StreamWriter writer = new StreamWriter(genStream))
            {
                writer.WriteLine(generatedDissassembly);
            }
            var referenceDissasembly = File.ReadAllText(filePath);
            generatedDissassembly = File.ReadAllText("unittest.shader");

            bool equal = string.Equals(generatedDissassembly, referenceDissasembly);
            generatedShaderFile.Delete();

            if (!equal)
            {
                equal = ReorderConstantsAndTest(generatedDissassembly, referenceDissasembly, version);
            }

            usesD3DX = referenceDissasembly.Contains("Generated by Microsoft (R) D3DX9 Shader Compiler");

            return equal;
        }

        public struct DissasemblyConstants
        {
            public string Name;
            public int Index;
            public string X;
            public string Y;
            public string Z;
            public string W;

            public DissasemblyConstants(string name, int index, string x, string y, string z, string w)
            {
                Name = name;
                Index = index;
                X = x;
                Y = y;
                Z = z;
                W = w;
            }
        }

        public static Dictionary<string, DissasemblyConstants> GetConstants(string data, string version)
        {
            var startIndex = data.IndexOf(version) + 7;
            var trimmedString = data.Substring(startIndex);
            var endIndex = trimmedString.IndexOf("dcl_") - 5;
            if (endIndex < 0)
                return null;
            var constantsBlock = trimmedString.Substring(0, endIndex);
            constantsBlock = constantsBlock.Replace("    def ", "");
            List<string> registerConstants = constantsBlock.Split('\n').ToList();
            Dictionary<string, DissasemblyConstants> constantsMapping = new Dictionary<string, DissasemblyConstants>();
            for(int i = 0; i < registerConstants.Count; i++)
            {
                var register = registerConstants[i];
                var regConstants = register.Split(',').ToList();
                if (regConstants.Count == 0)
                    continue;
                var name = regConstants[0];
                regConstants.RemoveAt(0);
                constantsMapping[name] = new DissasemblyConstants(name, i, regConstants[0], regConstants[1], regConstants[2], regConstants[3]);
            }

            return constantsMapping;
        }

        public static string RebuildConstants(DissasemblyConstants constant)
        {
            return $"{constant.X}, {constant.Y}, {constant.Z}, {constant.W}";
        }

        public static string ReplaceConstants(DissasemblyConstants genConstants, DissasemblyConstants refConstants, string genData)
        {
            genData = genData.Replace($"{genConstants.Name} ", $"shadergenprefix{refConstants.Name}shadergensuffix ");
            genData = genData.Replace($"{genConstants.Name}.", $"shadergenprefix{refConstants.Name}shadergensuffix.");
            genData = genData.Replace($"{genConstants.Name}\n", $"shadergenprefix{refConstants.Name}shadergensuffix\n");
            return genData;
        }

        public static bool ReorderConstantsAndTest(string genData, string refData, string version)
        {
            var refConstants = GetConstants(refData, version);
            var genConstants = GetConstants(genData, version);

            if (refConstants == null || genConstants == null)
                return false;

            if (refConstants.Count != genConstants.Count)
                return false;

            Dictionary<DissasemblyConstants, DissasemblyConstants> swaps = new Dictionary<DissasemblyConstants, DissasemblyConstants>();

            foreach(var refConstantName in refConstants.Keys)
            {
                if (!genConstants.ContainsKey(refConstantName))
                    return false;
                var refConstant = refConstants[refConstantName];
                var genConstant = genConstants[refConstantName];
                var refConstantString = RebuildConstants(refConstant);
                if (RebuildConstants(genConstant).Equals(refConstantString))
                    continue;

                // different constants, replace lines properly
                foreach(var genConstantName in genConstants.Keys)
                {
                    // add per component here if needed
                    if (RebuildConstants(genConstants[genConstantName]).Equals(refConstantString))
                    {
                        // found matching line
                        genData = ReplaceConstants(genConstants[genConstantName], refConstant, genData);
                        //swaps[genConstants[genConstantName]] = refConstant;
                        break;
                    }
                }
            }

            genData = genData.Replace("shadergenprefix", "");
            genData = genData.Replace("shadergensuffix", "");
            var startIndex = genData.IndexOf("ps_3_0") + 7;
            var sourceConstantBlock = genData.Substring(startIndex);
            var endIndex = sourceConstantBlock.IndexOf("dcl_") - 5;
            sourceConstantBlock = sourceConstantBlock.Substring(0, endIndex);

            startIndex = refData.IndexOf("ps_3_0") + 7;
            var destConstantBlock = refData.Substring(startIndex);
            endIndex = destConstantBlock.IndexOf("dcl_") - 5;
            destConstantBlock = destConstantBlock.Substring(0, endIndex);

            genData = genData.Replace(sourceConstantBlock, destConstantBlock);
            return string.Equals(genData, refData);
        }


        public bool TestAllPixelShaders(List<List<int>> shaderOverrides, List<ShaderStage> stageOverrides, List<List<int>> methodOverrides)
        {
            bool success = true;

            List<List<int>> shaders;
            if (shaderOverrides != null && shaderOverrides.Count > 0)
                shaders = shaderOverrides;
            else
                shaders = GetAllTestPixelShaders();

            foreach (var testShader in shaders)
            {
                List<ShaderStage> stages;
                if (stageOverrides != null && stageOverrides.Count > 0)
                    stages = stageOverrides;
                else
                    stages = GetAllShaderStages();

                foreach (var stage in stages)
                {
                    if (ReferenceGenerator.IsEntryPointSupported(stage) && !ReferenceGenerator.IsPixelShaderShared(stage))
                    {
                        if (methodOverrides != null && methodOverrides.Count == ReferenceGenerator.GetMethodCount())
                        {
                            bool validOptions = true;
                            for (int i = 0; i < ReferenceGenerator.GetMethodCount(); i++)
                            {
                                var optionOverrides = methodOverrides[i];
                                if(optionOverrides != null && optionOverrides.Count > 0)
                                    validOptions &= optionOverrides.Contains(testShader[i]);
                            }
                                

                            if (!validOptions)
                                continue;
                        }

                        string filePath = Path.Combine(Path.Combine(ReferencePath, $"{ShaderType.ToLower()}_templates"), BuildShaderName(testShader));
                        filePath = Path.Combine(filePath, BuildPixelShaderEntryPointName(stage));
                        var file = new FileInfo(filePath);

                        if (file.Exists == false)
                        {
                            Console.WriteLine($"No reference shader for {BuildShaderName(testShader)} at {stage.ToString().ToLower()}");
                            success = false;
                            continue;
                        }
                        bool equal = CompareShaders(GeneratePixelShader(stage, testShader), filePath, "ps_3_0",  out bool usesD3DX);
                        success &= equal;
                        DisplayPixelShaderTestResults(equal, testShader, stage, usesD3DX);
                    }
                }
            }

            if (success)
                Console.WriteLine("All unit tests passed sucessfully!");
            else
                Console.WriteLine("Failed unit tests. See above for more details.");

            return success;
        }

        public bool TestAllSharedPixelShaders(List<ShaderStage> stageOverrides)
        {
            bool success = true;

            List<ShaderStage> stages;
            if (stageOverrides != null && stageOverrides.Count > 0)
                stages = stageOverrides;
            else
                stages = GetAllShaderStages();

            foreach (var stage in stages)
            {
                if (ReferenceGenerator.IsEntryPointSupported(stage) && ReferenceGenerator.IsPixelShaderShared(stage))
                {
                    if (ReferenceGenerator.IsSharedPixelShaderUsingMethods(stage))
                    {
                        for (int i = 0; i < ReferenceGenerator.GetMethodCount(); i++)
                        {
                            for (int j = 0; j < ReferenceGenerator.GetMethodOptionCount(i); j++)
                            {
                                if (ReferenceGenerator.IsMethodSharedInEntryPoint(stage, i))
                                {
                                    string filePath = GetTestSharedPixelShader(stage, i, j);
                                    var file = new FileInfo(filePath);

                                    if (file.Exists == false)
                                    {
                                        Console.WriteLine($"No reference shader for {stage}_{i}_{j} at {stage.ToString().ToLower()}");
                                        success = false;
                                        continue;
                                    }
                                    bool equal = CompareShaders(GenerateSharedPixelShader(stage, i, j), filePath, "ps_3_0", out bool usesD3DX);
                                    success &= equal;
                                    DisplaySharedPixelShaderTestResults(equal, i, j, stage, usesD3DX);
                                }
                            }

                        }
                    }
                    else
                    {
                        string filePath = GetTestSharedPixelShader(stage, -1, -1);
                        var file = new FileInfo(filePath);

                        if (file.Exists == false)
                        {
                            Console.WriteLine($"No reference shader for {stage} at {stage.ToString().ToLower()}");
                            success = false;
                            continue;
                        }
                        bool equal = CompareShaders(GenerateSharedPixelShader(stage, -1, -1), filePath, "ps_3_0", out bool usesD3DX);
                        success &= equal;
                        DisplaySharedPixelShaderTestResults(equal, -1, -1, stage, usesD3DX);
                    }

                    
                }
            }

            if (success)
                Console.WriteLine("All unit tests passed sucessfully!");
            else
                Console.WriteLine("Failed unit tests. See above for more details.");

            return success;
        }

        public bool TestAllSharedVertexShaders(List<VertexType> vertexOverrides, List<ShaderStage> stageOverrides)
        {
            bool success = true;

            List<VertexType> vertices;
            if (vertexOverrides != null && vertexOverrides.Count > 0)
                vertices = vertexOverrides;
            else
                vertices = GetAllVertexFormats();

            foreach (var vertex in vertices)
            {
                if (!ReferenceGenerator.IsVertexFormatSupported(vertex))
                    continue;

                List<ShaderStage> stages;
                if (stageOverrides != null && stageOverrides.Count > 0)
                    stages = stageOverrides;
                else
                    stages = GetAllShaderStages();

                foreach (var stage in stages)
                {
                    if (ReferenceGenerator.IsEntryPointSupported(stage) && ReferenceGenerator.IsVertexShaderShared(stage))
                    {

                        string filePath = GetTestSharedVertexShader(vertex, stage);
                        var file = new FileInfo(filePath);

                        if (file.Exists == false)
                        {
                            Console.WriteLine($"No reference shader for {stage.ToString().ToLower()} vertex format {vertex.ToString().ToLower()}");
                            success = false;
                            continue;
                        }

                        bool equal = CompareShaders(GenerateSharedVertexShader(vertex, stage), filePath, "vs_3_0",  out bool usesD3DX);
                        success &= equal;
                        DisplayVertexShaderTestResults(equal, vertex, stage, usesD3DX);
                    }
                }
            }

            if (success)
                Console.WriteLine("All unit tests passed sucessfully!");
            else
                Console.WriteLine("Failed unit tests. See above for more details.");

            return success;
        }

        public abstract string GeneratePixelShader(ShaderStage stage, List<int> shaderOptions);

        public abstract string GenerateSharedPixelShader(ShaderStage stage, int methodIndex, int optionIndex);

        public abstract string GenerateVertexShader(VertexType vertex, ShaderStage stage);

        public abstract string GenerateSharedVertexShader(VertexType vertex, ShaderStage stage);
    }
}
