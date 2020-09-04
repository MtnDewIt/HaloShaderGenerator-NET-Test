using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shader;
using HaloShaderGenerator.Black;
using System;
using System.Collections.Generic;
using System.IO;
using HaloShaderGenerator.Generator;

namespace HaloShaderGenerator
{
    class Application
    {
        static readonly string ShaderReferencePath = @"D:\Halo\Repositories\TagTool\TagTool\bin\x64\Debug\Shaders";

        static readonly bool UnitTest = false;
        static readonly bool TestSpecificShader = true;
        static readonly string TestShaderType = "terrain";
        static readonly string TestStageType = "shared_vertex"; //shared_vertex, shader_pixel, vertex or pixel

        static readonly List<ShaderStage> StageOverrides = new List<ShaderStage> {ShaderStage.Albedo};

        #region Shader
        static readonly List<VertexType> VertexOverrides = new List<VertexType> { VertexType.World, VertexType.Rigid };

        static readonly List<int> ShaderAlbedoOverrides = new List<int> { };
        static readonly List<int> ShaderBumpOverrides = new List<int> { };
        static readonly List<int> ShaderAlphaOverrides = new List<int> { };
        static readonly List<int> ShaderSpecularOverrides = new List<int> { };
        static readonly List<int> ShaderMaterialOverrides = new List<int> { 7 };
        static readonly List<int> ShaderEnvOverrides = new List<int> { };
        static readonly List<int> ShaderSelfIllumOverrides = new List<int> { };
        static readonly List<int> ShaderBlendModeOverrides = new List<int> { };
        static readonly List<int> ShaderParallaxOverrides = new List<int> { };
        static readonly List<int> ShaderMiscOverrides = new List<int> { };

        static readonly List<List<int>> ShaderOverrides = new List<List<int>>
        {
            new List<int> { 0, 2, 0, 1, 7, 2, 0, 0, 0, 0, 0 },
            new List<int> { 0, 2, 0, 1, 7, 0, 0, 0, 0, 1, 0 },
        };
        #endregion

        #region particle

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
        #endregion

        #region terrain
        static readonly List<int> TerrainBlendingOverride = new List<int> { };
        static readonly List<int> TerrainEnvMapOverride = new List<int> { };
        static readonly List<int> TerrainMaterial0Override = new List<int> { };
        static readonly List<int> TerrainMaterial1Override = new List<int> { };
        static readonly List<int> TerrainMaterial2Override = new List<int> { };
        static readonly List<int> TerrainMaterial3Override = new List<int> { };

        static readonly List<List<int>> TerrainOverrides = new List<List<int>>
        {
            new List<int> { 0, 0, 0, 0, 0, 0},
            new List<int> { 0, 0, 0, 0, 0, 1},
            new List<int> { 0, 0, 1, 0, 2, 2},
        };
        #endregion

        #region contrail

        static readonly List<int> ContrailAlbedoOverrides = new List<int> { };
        static readonly List<int> ContrailBlendModeOverrides = new List<int> { };
        static readonly List<int> ContrailBlackPointOverrides = new List<int> { };
        static readonly List<int> ContrailFogOverrides = new List<int> { };

        static readonly List<List<int>> ContrailOverrides = new List<List<int>>
        {

        };
        #endregion

        #region beam

        static readonly List<int> BeamAlbedoOverrides = new List<int> { };
        static readonly List<int> BeamBlendModeOverrides = new List<int> { };
        static readonly List<int> BeamBlackPointOverrides = new List<int> { };
        static readonly List<int> BeamFogOverrides = new List<int> { };

        static readonly List<List<int>> BeamOverrides = new List<List<int>>
        {

        };
        #endregion

        #region light_volume

        static readonly List<int> LightVolumeAlbedoOverrides = new List<int> { };
        static readonly List<int> LightVolumeBlendModeOverrides = new List<int> { };
        static readonly List<int> LightVolumeFogOverrides = new List<int> { };

        static readonly List<List<int>> LightVolumeOverrides = new List<List<int>>
        {

        };
        #endregion

        static void RunTerrainSharedVertexShaderUnitTest()
        {
            TerrainUnitTest shaderTests = new TerrainUnitTest(ShaderReferencePath);

            if (TestSpecificShader)
            {
                shaderTests.TestAllSharedVertexShaders(VertexOverrides, StageOverrides);

                var stages = (StageOverrides.Count > 0) ? StageOverrides : TerrainUnitTest.GetAllShaderStages();
                var vertices = (VertexOverrides.Count > 0) ? VertexOverrides : TerrainUnitTest.GetAllVertexFormats();
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

        static void RunTerrainSharedPixelShaderUnitTest()
        {
            TerrainUnitTest shaderTests = new TerrainUnitTest(ShaderReferencePath);

            if (TestSpecificShader)
            {
                shaderTests.TestAllSharedPixelShaders(StageOverrides);

                var stages = (StageOverrides.Count > 0) ? StageOverrides : TerrainUnitTest.GetAllShaderStages();
                foreach (var stage in stages)
                {
                    TestSharedPixelShader(stage, -1, -1);
                }
            }

            if (UnitTest)
            {
                shaderTests.TestAllSharedPixelShaders(null);
            }
        }


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

        static void RunSharedPixelShaderUnitTest()
        {
            ShaderUnitTest shaderTests = new ShaderUnitTest(ShaderReferencePath);

            if (TestSpecificShader)
            {
                shaderTests.TestAllSharedPixelShaders(StageOverrides);

                var stages = (StageOverrides.Count > 0) ? StageOverrides : ShaderUnitTest.GetAllShaderStages();
                foreach (var stage in stages)
                {
                    TestSharedPixelShader(stage, -1, -1);
                }
            }

            if (UnitTest)
            {
                shaderTests.TestAllSharedPixelShaders(null);
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

        static void RunTerrainUnitTest()
        {
            TerrainUnitTest shaderTests = new TerrainUnitTest(ShaderReferencePath);

            if (TestSpecificShader)
            {
                var methodOverrides = new List<List<int>> { TerrainBlendingOverride, TerrainEnvMapOverride, TerrainMaterial0Override, TerrainMaterial1Override,
                    TerrainMaterial2Override, TerrainMaterial3Override};

                shaderTests.TestAllPixelShaders(TerrainOverrides, StageOverrides, methodOverrides);

                var stages = (StageOverrides.Count > 0) ? StageOverrides : TerrainUnitTest.GetAllShaderStages();

                foreach (var stage in stages)
                {
                    foreach (var methods in TerrainOverrides)
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

        static void RunContrailUnitTest()
        {
            ContrailUnitTest shaderTests = new ContrailUnitTest(ShaderReferencePath);

            if (TestSpecificShader)
            {
                var methodOverrides = new List<List<int>> { ContrailAlbedoOverrides, ContrailBlendModeOverrides, ContrailBlackPointOverrides, ContrailFogOverrides };

                shaderTests.TestAllPixelShaders(ContrailOverrides, StageOverrides, methodOverrides);

                var stages = (StageOverrides.Count > 0) ? StageOverrides : ContrailUnitTest.GetAllShaderStages();

                foreach (var stage in stages)
                {
                    foreach (var methods in ContrailOverrides)
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

        static void RunBeamUnitTest()
        {
            BeamUnitTest shaderTests = new BeamUnitTest(ShaderReferencePath);

            if (TestSpecificShader)
            {
                var methodOverrides = new List<List<int>> { BeamAlbedoOverrides, BeamBlendModeOverrides, BeamBlackPointOverrides, BeamFogOverrides };

                shaderTests.TestAllPixelShaders(BeamOverrides, StageOverrides, methodOverrides);

                var stages = (StageOverrides.Count > 0) ? StageOverrides : BeamUnitTest.GetAllShaderStages();

                foreach (var stage in stages)
                {
                    foreach (var methods in BeamOverrides)
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

        static void RunLightVolumeUnitTest()
        {
            LightVolumeUnitTest shaderTests = new LightVolumeUnitTest(ShaderReferencePath);

            if (TestSpecificShader)
            {
                var methodOverrides = new List<List<int>> { LightVolumeAlbedoOverrides, LightVolumeBlendModeOverrides, LightVolumeFogOverrides };

                shaderTests.TestAllPixelShaders(LightVolumeOverrides, StageOverrides, methodOverrides);

                var stages = (StageOverrides.Count > 0) ? StageOverrides : LightVolumeUnitTest.GetAllShaderStages();

                foreach (var stage in stages)
                {
                    foreach (var methods in LightVolumeOverrides)
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
                        case "shared_pixel":
                            RunSharedPixelShaderUnitTest(); break;
                    }
                    break;
                case "particle": RunParticleUnitTest(); break;
                case "contrail": RunContrailUnitTest(); break;
                case "beam": RunBeamUnitTest(); break;
                case "light_volume": RunLightVolumeUnitTest(); break;
                case "terrain":
                    switch (TestStageType)
                    {
                        case "shared_vertex":
                            RunTerrainSharedVertexShaderUnitTest(); 
                            break;
                        case "pixel":
                            RunTerrainUnitTest(); break;
                        case "shared_pixel":
                            RunTerrainSharedPixelShaderUnitTest(); 
                            break;
                    }
                    break;
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

        #region test methods shader
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
            else if(TestShaderType == "terrain")
            {
                var blend_type = (Terrain.Blending)methods[0];
                var env_map = (Terrain.Environment_Mapping)methods[1];
                var material_0 = (Terrain.Material)methods[2];
                var material_1 = (Terrain.Material)methods[3];
                var material_2 = (Terrain.Material)methods[4];
                var material_3 = (Terrain.Material_No_Detail_Bump)methods[5];

                var gen = new Terrain.TerrainGenerator(blend_type, env_map, material_0, material_1, material_2, material_3);
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
            IShaderGenerator gen = new ShaderGenerator();

            if (TestShaderType == "terrain")
                gen = new Terrain.TerrainGenerator();

            if (gen.IsEntryPointSupported(stage) && gen.IsVertexShaderShared(stage) && gen.IsVertexFormatSupported(vertexType))
            {
                var bytecode = gen.GenerateSharedVertexShader(vertexType, stage).Bytecode;
                var disassembly = D3DCompiler.Disassemble(bytecode);
                WriteShaderFile($"generated_{stage.ToString().ToLower()}_{vertexType.ToString().ToLower()}.glvs", disassembly);
            }
           
        }

        static void TestSharedPixelShader(ShaderStage stage, int methodIndex, int optionIndex)
        {
            IShaderGenerator gen = new ShaderGenerator();

            if (TestShaderType == "terrain")
                gen = new Terrain.TerrainGenerator();

            byte[] bytecode;
            string disassembly;

            if (gen.IsSharedPixelShaderUsingMethods(stage))
            {
                if (methodIndex == -1 || optionIndex == -1)
                {
                    for (int i = 0; i < gen.GetMethodCount(); i++)
                    {
                        if (gen.IsMethodSharedInEntryPoint(stage, i) && gen.IsPixelShaderShared(stage))
                        {
                            for (int j = 0; j < gen.GetMethodOptionCount(i); j++)
                            {
                                bytecode = gen.GenerateSharedPixelShader(stage, i, j).Bytecode;
                                disassembly = D3DCompiler.Disassemble(bytecode);
                                WriteShaderFile($"generated_{stage.ToString().ToLower()}_{i}_{j}.glps", disassembly);
                            }
                        }
                    }
                }
                else
                {
                    bytecode = gen.GenerateSharedPixelShader(stage, methodIndex, optionIndex).Bytecode;
                    disassembly = D3DCompiler.Disassemble(bytecode);
                    WriteShaderFile($"generated_{stage.ToString().ToLower()}_{methodIndex}_{optionIndex}.glps", disassembly);
                }
            }
            else
            {
                bytecode = gen.GenerateSharedPixelShader(stage, -1, -1).Bytecode;
                disassembly = D3DCompiler.Disassemble(bytecode);
                WriteShaderFile($"generated_{stage.ToString().ToLower()}.glps", disassembly);
            }
            
        }
        #endregion

        #region test methods independent shaders
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
        #endregion

        #region test methods shader black
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
        #endregion

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
