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

        static readonly bool UnitTest = false;
        static readonly bool TestSpecificShader = true;
        static readonly string TestShaderType = "shader";
        static readonly string TestStageType = "shared_vertex"; //shared_vertex, shader_pixel, vertex or pixel
        static readonly List<ShaderStage> StageOverrides = new List<ShaderStage> {ShaderStage.Albedo, ShaderStage.Active_Camo, ShaderStage.Shadow_Generate, ShaderStage.Lightmap_Debug_Mode,
            ShaderStage.Dynamic_Light, ShaderStage.Dynamic_Light_Cinematic, ShaderStage.Sfx_Distort, ShaderStage.Static_Sh, ShaderStage.Static_Per_Vertex_Color,
            ShaderStage.Static_Per_Pixel, ShaderStage.Static_Per_Vertex, ShaderStage.Static_Prt_Ambient, ShaderStage.Static_Prt_Linear, ShaderStage.Static_Prt_Quadratic};

        // Shader
        static readonly List<VertexType> VertexOverrides = new List<VertexType> {VertexType.World, VertexType.Rigid };

        static readonly List<int> ShaderAlbedoOverrides =         new List<int> {};
        static readonly List<int> ShaderBumpOverrides =           new List<int> { };
        static readonly List<int> ShaderAlphaOverrides =          new List<int> { };
        static readonly List<int> ShaderSpecularOverrides =       new List<int> {};
        static readonly List<int> ShaderMaterialOverrides =       new List<int> {7 };
        static readonly List<int> ShaderEnvOverrides =            new List<int> {};
        static readonly List<int> ShaderSelfIllumOverrides =      new List<int> {};
        static readonly List<int> ShaderBlendModeOverrides =      new List<int> { };
        static readonly List<int> ShaderParallaxOverrides =       new List<int> {};
        static readonly List<int> ShaderMiscOverrides =           new List<int> { };

        static readonly List<List<int>> ShaderOverrides = new List<List<int>>
        {
            new List<int> { 0, 2, 0, 1, 7, 2, 0, 0, 0, 0, 0 },
            new List<int> { 0, 2, 0, 1, 7, 0, 0, 0, 0, 1, 0 },
        };

        // Particle

        static readonly List<int> ParticleAlbedoOverrides =                 new List<int> { };
        static readonly List<int> ParticleBlendModeOverrides =              new List<int> { };
        static readonly List<int> ParticleSpecializedRenderingOverrides =   new List<int> { };
        static readonly List<int> ParticleLightingOverrides =               new List<int> { };
        static readonly List<int> ParticleRenderTargetsOverrides =          new List<int> { };
        static readonly List<int> ParticleDepthFadeOverrides =              new List<int> { };
        static readonly List<int> ParticleBlackPointOverrides =             new List<int> { };
        static readonly List<int> ParticleFogOverrides =                    new List<int> { };
        static readonly List<int> ParticleFrameBlendOverrides =             new List<int> { };
        static readonly List<int> ParticleSelfIlluminationOverrides =       new List<int> { };

        static readonly List<List<int>> ParticleOverrides = new List<List<int>>
        {
            //new List<int> { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 },
        };

        static void RunSharedVertexShaderUnitTest()
        {
            ShaderUnitTest shaderTests = new ShaderUnitTest(ShaderReferencePath);

            if (TestSpecificShader)
            {
                shaderTests.TestAllSharedVertexShaders(VertexOverrides, StageOverrides);

                var stages = (StageOverrides.Count > 0) ? StageOverrides : ShaderUnitTest.GetAllShaderStages();
                var vertices = (VertexOverrides.Count > 0) ? VertexOverrides : ShaderUnitTest.GetAllVertexFormats();
                foreach (var vertex in vertices)
                {
                    foreach (var stage in stages)
                    {
                        TestSharedVertexShader(vertex, stage);
                    }
                }
            }

            if (UnitTest)
            {
                shaderTests.TestAllSharedVertexShaders(null, null);
            }
        }

        static void RunShaderUnitTest()
        {
            ShaderUnitTest shaderTests = new ShaderUnitTest(ShaderReferencePath);

            if (TestSpecificShader)
            {
                var methodOverrides = new List<List<int>> { ShaderAlbedoOverrides, ShaderBumpOverrides, ShaderAlphaOverrides, ShaderSpecularOverrides, ShaderMaterialOverrides,
                ShaderEnvOverrides, ShaderSelfIllumOverrides, ShaderBlendModeOverrides, ShaderParallaxOverrides, ShaderMiscOverrides, new List<int>() };

                shaderTests.TestAllPixelShaders(ShaderOverrides, StageOverrides, methodOverrides);

                var stages = (StageOverrides.Count > 0) ? StageOverrides : ShaderUnitTest.GetAllShaderStages();

                foreach (var stage in stages)
                {
                    foreach (var methods in ShaderOverrides)
                    {
                        TestPixelShader(stage, methods);
                    }
                }
            }

            if (UnitTest)
            {
                shaderTests.TestAllPixelShaders(ShaderTests, null, null);
            }
        }

        static void RunParticleUnitTest()
        {
            ParticleUnitTest shaderTests = new ParticleUnitTest(ShaderReferencePath);

            if (TestSpecificShader)
            {
                var methodOverrides = new List<List<int>> { ParticleAlbedoOverrides, ParticleBlendModeOverrides, ParticleSpecializedRenderingOverrides, ParticleLightingOverrides,
                    ParticleRenderTargetsOverrides, ParticleDepthFadeOverrides, ParticleBlackPointOverrides, ParticleFogOverrides, ParticleFrameBlendOverrides, ParticleSelfIlluminationOverrides };

                shaderTests.TestAllPixelShaders(ParticleOverrides, StageOverrides, methodOverrides);

                var stages = (StageOverrides.Count > 0) ? StageOverrides : ParticleUnitTest.GetAllShaderStages();

                foreach (var stage in stages)
                {
                    foreach (var methods in ParticleOverrides)
                    {
                        TestPixelShader(stage, methods);
                    }
                }
            }

            if (UnitTest)
            {
                shaderTests.TestAllPixelShaders(ShaderTests, null, null);
            }
        }

        static int Main()
        {
            Console.WriteLine($"TESTING {TestShaderType.ToUpper()}");

            switch (TestShaderType)
            {
                case "shader":
                    switch (TestStageType)
                    {
                        case "shared_vertex":
                            RunSharedVertexShaderUnitTest(); break;
                        case "pixel":
                            RunShaderUnitTest(); break;
                    }
                    break;
                case "particle": RunParticleUnitTest(); break;
            }

            Console.ReadLine();
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

        static void TestPixelShader(ShaderStage stage, List<int> methods)
        {
            if (TestShaderType == "shader")
            {
                var albedo = (Albedo)methods[0]; 
                var bump_mapping = (Bump_Mapping)methods[1]; 
                var alpha_test = (Alpha_Test)methods[2]; 
                var specular_mask = (Specular_Mask)methods[3]; 
                var material_model = (Material_Model)methods[4];
                var environment_mapping = (Environment_Mapping)methods[5]; 
                var self_illumination = (Self_Illumination)methods[6]; 
                var blend_mode = (Blend_Mode)methods[7]; 
                var parallax = (Parallax)methods[8]; 
                var misc = (Misc)methods[9];
                var distortion = (Distortion)methods[10];

                var gen = new ShaderGenerator(albedo, bump_mapping, alpha_test, specular_mask, material_model, environment_mapping, self_illumination, blend_mode, parallax, misc, distortion);
                if (gen.IsEntryPointSupported(stage) && !gen.IsPixelShaderShared(stage))
                {
                    var bytecode = gen.GeneratePixelShader(stage).Bytecode;
                    var parameters = gen.GetPixelShaderParameters();

                    var disassembly = D3DCompiler.Disassemble(bytecode);
                    string filename = $"generated_{stage.ToString().ToLower()}_{string.Join("_", methods)}.pixl";
                    WriteShaderFile(filename, disassembly);
                }
            }
            else if (TestShaderType == "particle")
            {
                var albedo = (Particle.Albedo)methods[0];
                var blend_mode = (Particle.Blend_Mode)methods[1];
                var specialized_rendering = (Particle.Specialized_Rendering)methods[2];
                var lighting = (Particle.Lighting)methods[3];
                var render_targets = (Particle.Render_Targets)methods[4];
                var depth_fade = (Particle.Depth_Fade)methods[5];
                var black_point = (Particle.Black_Point)methods[6];
                var fog = (Particle.Fog)methods[7];
                var frame_blend = (Particle.Frame_Blend)methods[8];
                var self_illumination = (Particle.Self_Illumination)methods[9];

                var gen = new Particle.ParticleGenerator(albedo, blend_mode, specialized_rendering, lighting, render_targets, depth_fade, black_point, fog, frame_blend, self_illumination);
                if (gen.IsEntryPointSupported(stage) && !gen.IsPixelShaderShared(stage))
                {
                    var bytecode = gen.GeneratePixelShader(stage).Bytecode;
                    var parameters = gen.GetPixelShaderParameters();

                    var disassembly = D3DCompiler.Disassemble(bytecode);
                    string filename = $"generated_{stage.ToString().ToLower()}_{string.Join("_", methods)}.pixl";
                    WriteShaderFile(filename, disassembly);
                }
            }
        }

        static void TestSharedVertexShader(VertexType vertexType, ShaderStage stage)
        {
            var gen = new ShaderGenerator();
            if(gen.IsEntryPointSupported(stage) && gen.IsVertexShaderShared(stage) && gen.IsVertexFormatSupported(vertexType))
            {
                var bytecode = gen.GenerateSharedVertexShader(vertexType, stage).Bytecode;
                var disassembly = D3DCompiler.Disassemble(bytecode);
                WriteShaderFile($"generated_{stage.ToString().ToLower()}_{vertexType.ToString().ToLower()}.glvs", disassembly);
            }
           
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

        static readonly List<List<int>> ShaderTests = new List<List<int>>
        {
            new List<int> { 0, 0, 0, 0, 0, 1, 0, 3, 0, 0, 0 },

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
