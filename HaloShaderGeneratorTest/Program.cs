using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shader;
using HaloShaderGenerator.Black;
using System;
using System.Collections.Generic;
using System.IO;
using HaloShaderGenerator.Generator;
using System.Linq;

namespace HaloShaderGenerator
{
    class Application
    {
        static readonly ShaderTypes.ShaderSubtype TestStageType = ShaderTypes.ShaderSubtype.Pixel; //shared_vertex, shared_pixel, vertex or pixel

        static readonly bool ExplicitTest = true;
        static readonly string ExplicitReferencePath = @"C:\REPOS\TagTool\TagTool\bin\x64\Debug\HaloOnline106708\explicit";
        static readonly bool ExplicitTestSingle = true;
        static readonly ShaderTypes.ExplicitShader ExplicitShader = ShaderTypes.ExplicitShader.debug2d;

        static readonly bool ChudTest = false;
        static readonly string ChudReferencePath = @"C:\REPOS\TagTool\TagTool\bin\x64\Debug\HaloOnline106708\chud";
        static readonly bool ChudTestSingle = true;
        static readonly ShaderTypes.ChudShader ChudShader = ShaderTypes.ChudShader.simple;

        static readonly bool TemplateTest = true;
        static readonly string ShaderReferencePath = @"C:\REPOS\TagTool\TagTool\bin\x64\Debug\HaloOnline106708\Shaders";
        static readonly bool UnitTest = false;
        static readonly bool TestSpecificShader = true;
        static readonly ShaderType TestShaderType = ShaderType.Particle;

        static readonly List<ShaderStage> StageOverrides = new List<ShaderStage> { ShaderStage.Default };

        #region Shader
        static readonly List<VertexType> VertexOverrides = new List<VertexType> { };

        static readonly List<int> ShaderAlbedoOverrides = new List<int> {  };
        static readonly List<int> ShaderBumpOverrides = new List<int> { };
        static readonly List<int> ShaderAlphaOverrides = new List<int> { };
        static readonly List<int> ShaderSpecularOverrides = new List<int> { };
        static readonly List<int> ShaderMaterialOverrides = new List<int> { };
        static readonly List<int> ShaderEnvOverrides = new List<int> { };
        static readonly List<int> ShaderSelfIllumOverrides = new List<int> { };
        static readonly List<int> ShaderBlendModeOverrides = new List<int> { };
        static readonly List<int> ShaderParallaxOverrides = new List<int> { };
        static readonly List<int> ShaderMiscOverrides = new List<int> { };

        static readonly List<List<int>> ShaderOverrides = new List<List<int>>
        {
            //new List<int> { 0, 2, 0, 1, 7, 2, 0, 0, 0, 0, 0 },
            //new List<int> { 0, 2, 0, 1, 7, 0, 0, 0, 0, 1, 0 },
            //new List<int> { 0, 1, 1, 0, 1, 0, 0, 3, 0, 0, 0 },
        };

        static readonly List<List<int>> ShaderMethodOverrides = new List<List<int>> { ShaderAlbedoOverrides, ShaderBumpOverrides, ShaderAlphaOverrides, ShaderSpecularOverrides, ShaderMaterialOverrides,
                ShaderEnvOverrides, ShaderSelfIllumOverrides, ShaderBlendModeOverrides, ShaderParallaxOverrides, ShaderMiscOverrides, new List<int>() };
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

        static readonly List<List<int>> ParticleMS26 = new List<List<int>>
        {
            new List<int> { 3, 3, 0, 1, 0, 1, 0, 1, 0, 1 },
        };

        static readonly List<List<int>> ParticleMethodOverrides = new List<List<int>> { ParticleAlbedoOverrides, ParticleBlendModeOverrides, ParticleSpecializedRenderingOverrides, ParticleLightingOverrides,
                    ParticleRenderTargetsOverrides, ParticleDepthFadeOverrides, ParticleBlackPointOverrides, ParticleFogOverrides, ParticleFrameBlendOverrides, ParticleSelfIlluminationOverrides };
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
            new List<int> { 0, 0, 2, 0, 0, 2},
            new List<int> { 0, 0, 0, 0, 0, 2},
        };

        static readonly List<List<int>> TerrainMethodOverrides = new List<List<int>> { TerrainBlendingOverride, TerrainEnvMapOverride, TerrainMaterial0Override, TerrainMaterial1Override,
                    TerrainMaterial2Override, TerrainMaterial3Override};
        #endregion

        #region halogram
        static readonly List<int> HalogramAlbedoOverride = new List<int> {  };
        static readonly List<int> HalogramSelfIllumOverride = new List<int> {  };
        static readonly List<int> HalogramBlendModeOverride = new List<int> { };
        static readonly List<int> HalogramMiscOverride = new List<int> { };
        static readonly List<int> HalogramWarpOverride = new List<int> { };
        static readonly List<int> HalogramOverlayOverride = new List<int> { };
        static readonly List<int> HalogramEdgeFadeOverride = new List<int> { };

        static readonly List<List<int>> HalogramOverrides = new List<List<int>>
        {
            //new List<int> { 6, 1, 3, 0, 0, 0, 1},
            //new List<int> { 0, 9, 0, 1, 0, 2, 0},
            //new List<int> { 0, 1, 0, 0, 0, 0, 1},
            //new List<int> { 0, 1, 1, 0, 0, 0, 0},
            //new List<int> { 0, 1, 1, 0, 0, 0, 1},
            //new List<int> { 0, 1, 1, 1, 0, 0, 0},
            //new List<int> { 2, 1, 1, 0, 0, 0, 0},
            //new List<int> { 6, 1, 3, 0, 0, 0, 1},
            //new List<int> { 0, 8, 1, 0, 1, 0, 0},
            //new List<int> { 0, 11, 0, 1, 0, 2, 0},
            //new List<int> { 0, 1, 1, 0, 0, 0, 1},
            //new List<int> { 0, 3, 1, 0, 0, 0, 1},
            //new List<int> { 0, 3, 1, 0, 0, 2, 1},
            //new List<int> { 0, 4, 1, 0, 0, 3, 0},
            //new List<int> { 0, 4, 1, 0, 0, 0, 0},
            //new List<int> { 0, 5, 1, 0, 0, 1, 1},
            //new List<int> { 0, 7, 1, 0, 0, 0, 0},
            //new List<int> { 2, 5, 1, 0, 1, 2, 1},
            //new List<int> { 0, 8, 1, 0, 0, 0, 0},
            //new List<int> { 0, 8, 1, 0, 0, 1, 0},
            //new List<int> { 0, 8, 1, 0, 0, 1, 1},
            //new List<int> { 0, 8, 1, 0, 0, 3, 1},
            //new List<int> { 0, 8, 1, 0, 0, 4, 1},
            //new List<int> { 0, 8, 1, 0, 1, 0, 0},
            //new List<int> { 0, 9, 0, 1, 0, 2, 0},
            //new List<int> { 2, 9, 1, 1, 0, 2, 0},
        };

        static readonly List<List<int>> HalogramMS25 = new List<List<int>>
        {
            new List<int> { 0,5,1,1,0,0,0 },
            new List<int> { 2,3,1,0,0,0,1 },
            new List<int> { 0,1,0,0,0,0,0 },
            new List<int> { 0,1,3,0,0,0,0 },
            new List<int> { 0,5,3,0,0,0,0 },
            new List<int> { 0,4,3,0,0,0,0 },
            new List<int> { 0,5,1,0,0,0,1 },
            new List<int> { 0,4,0,0,0,0,0 },
            new List<int> { 9,5,3,0,0,0,0 },
        };

        static readonly List<List<int>> HalogramMethodOverrides = new List<List<int>> { HalogramAlbedoOverride, HalogramSelfIllumOverride, 
            HalogramBlendModeOverride, HalogramMiscOverride, HalogramWarpOverride, HalogramOverlayOverride, HalogramEdgeFadeOverride };

        #endregion

        #region contrail

        static readonly List<int> ContrailAlbedoOverrides = new List<int> { };
        static readonly List<int> ContrailBlendModeOverrides = new List<int> { };
        static readonly List<int> ContrailBlackPointOverrides = new List<int> { };
        static readonly List<int> ContrailFogOverrides = new List<int> { };

        static readonly List<List<int>> ContrailOverrides = new List<List<int>>
        {
            //new List<int> { 2, 10, 1, 0 },
        };

        static readonly List<List<int>> ContrailMethodOverrides = new List<List<int>> { ContrailAlbedoOverrides, 
            ContrailBlendModeOverrides, ContrailBlackPointOverrides, ContrailFogOverrides };

        #endregion

        #region beam

        static readonly List<int> BeamAlbedoOverrides = new List<int> { };
        static readonly List<int> BeamBlendModeOverrides = new List<int> { };
        static readonly List<int> BeamBlackPointOverrides = new List<int> { };
        static readonly List<int> BeamFogOverrides = new List<int> { };

        static readonly List<List<int>> BeamOverrides = new List<List<int>>
        {

        };

        static readonly List<List<int>> BeamMethodOverrides = new List<List<int>> { BeamAlbedoOverrides, 
            BeamBlendModeOverrides, BeamBlackPointOverrides, BeamFogOverrides };
        #endregion

        #region light_volume

        static readonly List<int> LightVolumeAlbedoOverrides = new List<int> { };
        static readonly List<int> LightVolumeBlendModeOverrides = new List<int> { };
        static readonly List<int> LightVolumeFogOverrides = new List<int> { };

        static readonly List<List<int>> LightVolumeOverrides = new List<List<int>>
        {

        };

        static readonly List<List<int>> LightVolumeMethodOverrides = new List<List<int>> { LightVolumeAlbedoOverrides, 
            LightVolumeBlendModeOverrides, LightVolumeFogOverrides };
        #endregion

        #region decal

        static readonly List<int> DecalAlbedoOverrides = new List<int> { };
        static readonly List<int> DecalBlendModeOverrides = new List<int> { };
        static readonly List<int> DecalRenderPassOverrides = new List<int> { };
        static readonly List<int> DecalSpecularOverrides = new List<int> { };
        static readonly List<int> DecalBumpMappingOverrides = new List<int> { };
        static readonly List<int> DecalTintingOverrides = new List<int> { };

        static readonly List<List<int>> DecalOverrides = new List<List<int>>
        {
            //new List<int> { 0, 1, 0, 0, 0, 0 },
            //new List<int> { 0, 3, 0, 0, 0, 0 },
            //new List<int> { 0, 2, 1, 0, 0, 0 },
            //new List<int> { 0, 2, 0, 0, 0, 0 },
            //new List<int> { 0, 8, 1, 0, 0, 0 },
            //new List<int> { 0, 8, 1, 0, 0, 1 },
            //new List<int> { 0, 1, 0, 1, 0, 0 },
            //new List<int> { 0, 1, 1, 0, 0, 1 },
            //new List<int> { 0, 1, 1, 0, 0, 2 },
            //new List<int> { 0, 2, 0, 1, 0, 0 },
            //new List<int> { 0, 10, 0, 0, 1, 0 },
            //new List<int> { 0, 4, 0, 1, 0, 0 },
            //new List<int> { 0, 2, 0, 0, 0, 3 },
            //new List<int> { 0, 3, 0, 0, 1, 0 },
            //new List<int> { 0, 3, 0, 0, 1, 1 },
            //new List<int> { 2, 2, 0, 0, 0, 0 },
            //new List<int> { 2, 10, 1, 1, 0, 3 },
            //new List<int> { 2, 10, 1, 1, 1, 3 },
            //new List<int> { 4, 3, 0, 0, 0, 0 },
            //new List<int> { 5, 3, 0, 0, 0, 3 },
            //new List<int> { 8, 10, 0, 1, 0, 2 },
        };

        static readonly List<List<int>> DecalMethodOverrides = new List<List<int>> { DecalAlbedoOverrides, DecalBlendModeOverrides, 
            DecalRenderPassOverrides, DecalSpecularOverrides, DecalBumpMappingOverrides, DecalTintingOverrides };
        #endregion

        #region screen

        static readonly List<int> ScreenWarpOverrides = new List<int> { };
        static readonly List<int> ScreenBaseOverrides = new List<int> { };
        static readonly List<int> ScreenOverlayAOverrides = new List<int> { };
        static readonly List<int> ScreenOverlayBOverrides = new List<int> { };
        static readonly List<int> ScreenBlendModeOverrides = new List<int> { };

        static readonly List<List<int>> ScreenOverrides = new List<List<int>>
        {
            new List<int> { 2, 0, 4, 1, 0 },
            new List<int> { 2, 1, 2, 0, 0 },
        };
        
        static readonly List<List<int>> ScreenMethodOverrides = new List<List<int>> { ScreenWarpOverrides, ScreenBaseOverrides, ScreenOverlayAOverrides, ScreenOverlayBOverrides, ScreenBlendModeOverrides };
        #endregion

        static GenericUnitTest GetUnitTest(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.Shader:         return new ShaderUnitTest(ShaderReferencePath);
                case ShaderType.Beam:           return new BeamUnitTest(ShaderReferencePath);
                case ShaderType.Contrail:       return new ContrailUnitTest(ShaderReferencePath);
                case ShaderType.Decal:          return new DecalUnitTest(ShaderReferencePath);
                case ShaderType.Halogram:       return new HalogramUnitTest(ShaderReferencePath);
                case ShaderType.LightVolume:    return new LightVolumeUnitTest(ShaderReferencePath);
                case ShaderType.Particle:       return new ParticleUnitTest(ShaderReferencePath);
                case ShaderType.Terrain:        return new TerrainUnitTest(ShaderReferencePath);
                //case ShaderType.Water:          return new WaterUnitTest(ShaderReferencePath);
                case ShaderType.Screen:         return new ScreenUnitTest(ShaderReferencePath);
                //case ShaderType.Foliage:        return new FoliageUnitTest(ShaderReferencePath);
            }

            throw new Exception($"No unit test for \"shaderType\" found.");
        }

        static List<List<int>> GetMethodOverrides(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.Shader:         return ShaderMethodOverrides;
                case ShaderType.Beam:           return BeamMethodOverrides;
                case ShaderType.Contrail:       return ContrailMethodOverrides;
                case ShaderType.Decal:          return DecalMethodOverrides;
                case ShaderType.Halogram:       return HalogramMethodOverrides;
                case ShaderType.LightVolume:    return LightVolumeMethodOverrides;
                case ShaderType.Particle:       return ParticleMethodOverrides;
                case ShaderType.Terrain:        return TerrainMethodOverrides;
                //case ShaderType.Water:          return WaterMethodOverrides;
                case ShaderType.Screen:         return ScreenMethodOverrides;
                //case ShaderType.Foliage:        return FoliageMethodOverrides;
            }

            throw new Exception($"No method overrides for \"shaderType\" found.");
        }

        static List<List<int>> GetOverrides(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.Shader:         return ShaderOverrides;
                case ShaderType.Beam:           return BeamOverrides;
                case ShaderType.Contrail:       return ContrailOverrides;
                case ShaderType.Decal:          return DecalOverrides;
                case ShaderType.Halogram:       return HalogramOverrides;
                case ShaderType.LightVolume:    return LightVolumeOverrides;
                case ShaderType.Particle:       return ParticleOverrides;
                case ShaderType.Terrain:        return TerrainOverrides;
                //case ShaderType.Water:          return WaterOverrides;
                case ShaderType.Screen:         return ScreenOverrides;
                //case ShaderType.Foliage:        return FoliageOverrides;
            }

            throw new Exception($"No overrides for \"shaderType\" found.");
        }

        static void RunPixelUnitTest(GenericUnitTest unitTest, ShaderType shaderType)
        {
            if (TestSpecificShader)
            {
                var methodOverrides = GetMethodOverrides(shaderType);
                var overrides = GetOverrides(shaderType);

                unitTest.TestAllPixelShaders(overrides, StageOverrides, methodOverrides);

                var stages = (StageOverrides.Count > 0) ? StageOverrides : unitTest.GetAllShaderStages();

                foreach (var stage in stages)
                {
                    foreach (var methods in overrides)
                    {
                        TestPixelShader(shaderType, stage, methods);
                    }
                }
            }

            if (UnitTest)
            {
                unitTest.TestAllPixelShaders(ShaderTests, null, null);
            }
        }

        static void RunSharedPixelUnitTest(GenericUnitTest unitTest, ShaderType shaderType)
        {
            if (TestSpecificShader)
            {
                unitTest.TestAllSharedPixelShaders(StageOverrides);

                var stages = (StageOverrides.Count > 0) ? StageOverrides : unitTest.GetAllShaderStages();
                foreach (var stage in stages)
                {
                    TestSharedPixelShader(shaderType, stage, -1, -1);
                }
            }

            if (UnitTest)
            {
                unitTest.TestAllSharedPixelShaders(null);
            }
        }

        static void RunSharedVertexUnitTest(GenericUnitTest unitTest, ShaderType shaderType)
        {
            if (TestSpecificShader)
            {
                unitTest.TestAllSharedVertexShaders(VertexOverrides, StageOverrides);

                var stages = (StageOverrides.Count > 0) ? StageOverrides : unitTest.GetAllShaderStages();
                var vertices = (VertexOverrides.Count > 0) ? VertexOverrides : unitTest.GetAllVertexFormats();
                foreach (var vertex in vertices)
                {
                    foreach (var stage in stages)
                    {
                        TestSharedVertexShader(shaderType, vertex, stage);
                    }
                }
            }

            if (UnitTest)
            {
                unitTest.TestAllSharedVertexShaders(null, null);
            }
        }

        static void RunUnitTest(ShaderType shaderType, ShaderTypes.ShaderSubtype shaderSubtype)
        {
            Console.WriteLine($"TESTING {shaderType.ToString().ToUpper()}");

            var unitTest = GetUnitTest(shaderType);

            switch (shaderSubtype)
            {
                case ShaderTypes.ShaderSubtype.Pixel:
                    RunPixelUnitTest(unitTest, shaderType);
                    break;
                case ShaderTypes.ShaderSubtype.SharedPixel:
                    RunSharedPixelUnitTest(unitTest, shaderType);
                    break;
                case ShaderTypes.ShaderSubtype.SharedVertex:
                    RunSharedVertexUnitTest(unitTest, shaderType);
                    break;
            }
        }

        static void RunExplicitUnitTest()
        {
            Console.WriteLine($"TESTING EXPLICIT");
            bool pixl = TestStageType == ShaderTypes.ShaderSubtype.Pixel;

            var unitTest = new ExplicitUnitTest(ExplicitReferencePath);

            if (ExplicitTestSingle)
            {
                string disassembly = "";
                if (TestStageType == ShaderTypes.ShaderSubtype.Pixel)
                    disassembly = unitTest.TestExplicitPixelShader(ExplicitShader);
                else if (TestStageType == ShaderTypes.ShaderSubtype.Vertex)
                    disassembly = unitTest.TestExplicitVertexShader(ExplicitShader);

                string filename = $"generated_{ExplicitShader}.{(pixl ? "pixel_shader" : "vertex_shader")}";
                WriteShaderFile(filename, disassembly);
            }
            else
            {
                if (TestStageType == ShaderTypes.ShaderSubtype.Pixel)
                    unitTest.TestAllExplicitPixelShaders();
                else if (TestStageType == ShaderTypes.ShaderSubtype.Vertex)
                    unitTest.TestAllExplicitVertexShaders();
            }
        }

        static int Main()
        {
            if (ExplicitTest)
                RunExplicitUnitTest();
            //if (ChudTest)
            //    RunChudUnitTest();
            if (TemplateTest)
                RunUnitTest(TestShaderType, TestStageType);

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
        static IShaderGenerator GetTemplateShaderGenerator(ShaderType shaderType, List<int> methods)
        {
            byte[] bMethods = methods.ToArray().Select(i => (byte)i).ToArray();

            switch (shaderType)
            {
                case ShaderType.Shader:         return new Shader.ShaderGenerator(bMethods);
                case ShaderType.Beam:           return new Beam.BeamGenerator(bMethods);
                case ShaderType.Contrail:       return new Contrail.ContrailGenerator(bMethods);
                case ShaderType.Decal:          return new Decal.DecalGenerator(bMethods);
                case ShaderType.Halogram:       return new Halogram.HalogramGenerator(bMethods);
                case ShaderType.LightVolume:    return new LightVolume.LightVolumeGenerator(bMethods);
                case ShaderType.Particle:       return new Particle.ParticleGenerator(bMethods);
                case ShaderType.Terrain:        return new Terrain.TerrainGenerator(bMethods);
                //case ShaderType.Cortana:        return new Cortana.CortanaGenerator(bMethods);
                //case ShaderType.Water:          return new Water.WaterGenerator(bMethods);
                case ShaderType.Black:          return new Black.ShaderBlackGenerator();
                case ShaderType.Screen:         return new Screen.ScreenGenerator(bMethods);
                case ShaderType.Custom:         return new Custom.CustomGenerator(bMethods);
                //case ShaderType.Foliage:        return new Foliage.FoliageGenerator(bMethods);
                case ShaderType.ZOnly:          return new ZOnly.ZOnlyGenerator(bMethods);
            }

            throw new Exception($"No generator for \"shaderType\" found.");
        }

        static IShaderGenerator GetShaderGenerator(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.Shader:         return new Shader.ShaderGenerator();
                case ShaderType.Beam:           return new Beam.BeamGenerator();
                case ShaderType.Contrail:       return new Contrail.ContrailGenerator();
                case ShaderType.Decal:          return new Decal.DecalGenerator();
                case ShaderType.Halogram:       return new Halogram.HalogramGenerator();
                case ShaderType.LightVolume:    return new LightVolume.LightVolumeGenerator();
                case ShaderType.Particle:       return new Particle.ParticleGenerator();
                case ShaderType.Terrain:        return new Terrain.TerrainGenerator();
                //case ShaderType.Cortana:        return new Cortana.CortanaGenerator();
                //case ShaderType.Water:          return new Water.WaterGenerator();
                case ShaderType.Black:          return new Black.ShaderBlackGenerator();
                case ShaderType.Screen:         return new Screen.ScreenGenerator();
                case ShaderType.Custom:         return new Custom.CustomGenerator();
                //case ShaderType.Foliage:        return new Foliage.FoliageGenerator();
                case ShaderType.ZOnly:          return new ZOnly.ZOnlyGenerator();
            }

            throw new Exception($"No generator for \"shaderType\" found.");
        }

        static void TestPixelShader(ShaderType shaderType, ShaderStage stage, List<int> methods)
        {
            IShaderGenerator generator = GetTemplateShaderGenerator(shaderType, methods);

            if (generator.IsEntryPointSupported(stage) && !generator.IsPixelShaderShared(stage))
            {
                var bytecode = generator.GeneratePixelShader(stage).Bytecode;
                var parameters = generator.GetPixelShaderParameters();

                var disassembly = D3DCompiler.Disassemble(bytecode);
                string filename = $"generated_{stage.ToString().ToLower()}_{string.Join("_", methods)}.pixl";
                WriteShaderFile(filename, disassembly);
            }
        }

        static void TestSharedVertexShader(ShaderType shaderType, VertexType vertexType, ShaderStage stage)
        {
            IShaderGenerator generator = GetShaderGenerator(shaderType);

            if (generator.IsEntryPointSupported(stage) && generator.IsVertexShaderShared(stage) && generator.IsVertexFormatSupported(vertexType))
            {
                var bytecode = generator.GenerateSharedVertexShader(vertexType, stage).Bytecode;
                var disassembly = D3DCompiler.Disassemble(bytecode);
                WriteShaderFile($"generated_{stage.ToString().ToLower()}_{vertexType.ToString().ToLower()}.glvs", disassembly);
            }
           
        }

        static void TestSharedPixelShader(ShaderType shaderType, ShaderStage stage, int methodIndex, int optionIndex)
        {
            IShaderGenerator generator = GetShaderGenerator(shaderType);

            byte[] bytecode;
            string disassembly;

            if (generator.IsSharedPixelShaderUsingMethods(stage))
            {
                if (methodIndex == -1 || optionIndex == -1)
                {
                    for (int i = 0; i < generator.GetMethodCount(); i++)
                    {
                        if (generator.IsMethodSharedInEntryPoint(stage, i) && generator.IsPixelShaderShared(stage))
                        {
                            for (int j = 0; j < generator.GetMethodOptionCount(i); j++)
                            {
                                bytecode = generator.GenerateSharedPixelShader(stage, i, j).Bytecode;
                                disassembly = D3DCompiler.Disassemble(bytecode);
                                WriteShaderFile($"generated_{stage.ToString().ToLower()}_{i}_{j}.glps", disassembly);
                            }
                        }
                    }
                }
                else
                {
                    bytecode = generator.GenerateSharedPixelShader(stage, methodIndex, optionIndex).Bytecode;
                    disassembly = D3DCompiler.Disassemble(bytecode);
                    WriteShaderFile($"generated_{stage.ToString().ToLower()}_{methodIndex}_{optionIndex}.glps", disassembly);
                }
            }
            else
            {
                bytecode = generator.GenerateSharedPixelShader(stage, -1, -1).Bytecode;
                disassembly = D3DCompiler.Disassemble(bytecode);
                WriteShaderFile($"generated_{stage.ToString().ToLower()}.glps", disassembly);
            }
            
        }
        #endregion

        #region test methods independent shaders
        static void TestVertexShader(string name)
        {
            var bytecode = GenericVertexShaderGenerator.GenerateVertexShader(name).Bytecode;
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
            var str = D3DCompiler.Disassemble(bytecode.Bytecode);
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
