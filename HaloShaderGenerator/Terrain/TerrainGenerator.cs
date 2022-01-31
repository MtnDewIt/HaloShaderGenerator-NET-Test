using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Terrain
{
    public class TerrainGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;
        private bool ApplyFixes;

        Blending blending;
        Environment_Mapping environment_mapping;
        Material material_0;
        Material material_1;
        Material material_2;
        Material_No_Detail_Bump material_3;

        /// <summary>
        /// Generator insantiation for shared shaders. Does not require method options.
        /// </summary>
        public TerrainGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        /// <summary>
        /// Generator instantiation for method specific shaders.
        /// </summary>
        public TerrainGenerator(Blending blending, Environment_Mapping environment_mapping, Material material_0, Material material_1, Material material_2, Material_No_Detail_Bump material_3)
        {
            this.blending = blending;
            this.environment_mapping = environment_mapping;
            this.material_0 = material_0;
            this.material_1 = material_1;
            this.material_2 = material_2;
            this.material_3 = material_3;
            TemplateGenerationValid = true;
        }

        public TerrainGenerator(byte[] options, bool applyFixes = false)
        {
            this.blending = (Blending)options[0];
            this.environment_mapping = (Environment_Mapping)options[1];
            this.material_0 = (Material)options[2];
            this.material_1 = (Material)options[3];
            this.material_2 = (Material)options[4];
            this.material_3 = (Material_No_Detail_Bump)options[5];

            //ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }


        public ShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.Add(new D3D.SHADER_MACRO { Name = "_TERRAIN_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.ShaderType>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Blending>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Environment_Mapping>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Material>());

            //
            // Convert to shared enum
            //

            var sEnvironmentMapping = Enum.Parse(typeof(Shared.Environment_Mapping), environment_mapping.ToString());

            //
            // The following code properly names the macros (like in rmdf)
            //

            Material material_3_translated = material_3 == Material_No_Detail_Bump.Off ? Material.Off : material_3 == Material_No_Detail_Bump.Diffuse_Only ? Material.Diffuse_Only : Material.Diffuse_Plus_Specular;

            macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", blending));
            macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", sEnvironmentMapping, "envmap_type_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("shaderstage", entryPoint, "k_shaderstage_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("shadertype", Shared.ShaderType.Terrain, "k_shadertype_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("blend_type_arg", blending, "k_blend_type_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type_arg", sEnvironmentMapping, "k_environment_mapping_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("material_type_0_arg", material_0, "k_material_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("material_type_1_arg", material_1, "k_material_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("material_type_2_arg", material_2, "k_material_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("material_type_3_arg", material_3_translated, "k_material_"));

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"pixl_terrain.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.Add(new D3D.SHADER_MACRO { Name = "_TERRAIN_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderType>());

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"glps_terrain.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_VERTEX_SHADER_HELPER_HLSLI", Definition = "1" });
            macros.Add(new D3D.SHADER_MACRO { Name = "_TERRAIN_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<VertexType>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.ShaderType>());
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_vertex_transform", vertexType, "calc_vertex_transform_", ""));
            macros.Add(ShaderGeneratorBase.CreateMacro("transform_dominant_light", vertexType, "transform_dominant_light_", ""));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_distortion", vertexType, "calc_distortion_", ""));
            macros.Add(ShaderGeneratorBase.CreateVertexMacro("input_vertex_format", vertexType));

            macros.Add(ShaderGeneratorBase.CreateMacro("shaderstage", entryPoint, "k_shaderstage_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("vertextype", vertexType, "k_vertextype_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("shadertype", Shared.ShaderType.Terrain, "shadertype_"));

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource(@"glvs_terrain.hlsl", macros, $"entry_{entryPoint.ToString().ToLower()}", "vs_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");
            return null;
        }

        public int GetMethodCount()
        {
            return 5;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            //TODO
            return 2;
        }

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((TerrainMethods)methodIndex)
            {
                case TerrainMethods.Blending:
                    return (int)blending;
                case TerrainMethods.Environment_Map:
                    return (int)environment_mapping;
                case TerrainMethods.Material_0:
                    return (int)material_0;
                case TerrainMethods.Material_1:
                    return (int)material_1;
                case TerrainMethods.Material_2:
                    return (int)material_2;
                case TerrainMethods.Material_3:
                    return (int)material_3;
                
            }
            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Albedo:
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Static_Sh:
                case ShaderStage.Dynamic_Light:
                case ShaderStage.Lightmap_Debug_Mode:
                case ShaderStage.Shadow_Generate:
                case ShaderStage.Dynamic_Light_Cinematic:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Ambient:
                    return true;
                    
                default:
                case ShaderStage.Default:
                case ShaderStage.Z_Only:
                case ShaderStage.Water_Shading:
                case ShaderStage.Water_Tesselation:
                case ShaderStage.Shadow_Apply:
                case ShaderStage.Static_Default:
                case ShaderStage.Static_Per_Vertex_Color:
                case ShaderStage.Active_Camo:
                case ShaderStage.Sfx_Distort:
                    return false;
            }
        }

        public bool IsMethodSharedInEntryPoint(ShaderStage entryPoint, int method_index)
        {
            return false;
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            return false;
        }

        public bool IsSharedPixelShaderWithoutMethod(ShaderStage entryPoint)
        {
            return entryPoint == ShaderStage.Shadow_Generate;
        }

        public bool IsPixelShaderShared(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Shadow_Generate:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsVertexFormatSupported(VertexType vertexType)
        {
            switch (vertexType)
            {
                case VertexType.World:
                case VertexType.Rigid:
                case VertexType.Skinned:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsVertexShaderShared(ShaderStage entryPoint)
        {
            return true;
        }

        public ShaderParameters GetPixelShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;
            var result = new ShaderParameters();

            result.AddSamplerParameter("blend_map");
            result.AddFloatParameter("global_albedo_tint");
            switch (blending)
            {
                case Blending.Dynamic_Morph:
                    result.AddFloatParameter("dynamic_material");
                    result.AddFloatParameter("transition_sharpness");
                    result.AddFloatParameter("transition_threshold");
                    break;
            }

            switch (environment_mapping)
            {
                case Environment_Mapping.None:
                    break;
                case Environment_Mapping.Per_Pixel:
                    result.AddSamplerWithoutXFormParameter("environment_map");
                    result.AddFloat3ColorParameter("env_tint_color");
                    result.AddFloatParameter("env_roughness_scale");
                    break;
                case Environment_Mapping.Dynamic:
                    result.AddFloat3ColorParameter("env_tint_color");
                    result.AddSamplerParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0);
                    result.AddSamplerParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1);
                    result.AddFloatParameter("env_roughness_scale");
                    break;
            }

            switch (material_0)
            {
                case Material.Off:
                    break;

                case Material.Diffuse_Only:
                    result.AddSamplerParameter("base_map_m_0");
                    result.AddSamplerParameter("detail_map_m_0");
                    result.AddSamplerParameter("bump_map_m_0");
                    result.AddSamplerParameter("detail_bump_m_0");
                    break;

                case Material.Diffuse_Plus_Specular:
                    result.AddSamplerParameter("base_map_m_0");
                    result.AddSamplerParameter("detail_map_m_0");
                    result.AddSamplerParameter("bump_map_m_0");
                    result.AddSamplerParameter("detail_bump_m_0");

                    result.AddFloatParameter("diffuse_coefficient_m_0");
                    result.AddFloatParameter("specular_coefficient_m_0");
                    result.AddFloatParameter("specular_power_m_0");
                    result.AddFloat3Parameter("specular_tint_m_0");
                    result.AddFloatParameter("fresnel_curve_steepness_m_0");
                    result.AddFloatParameter("area_specular_contribution_m_0");
                    result.AddFloatParameter("analytical_specular_contribution_m_0");
                    result.AddFloatParameter("environment_specular_contribution_m_0");
                    result.AddFloatParameter("albedo_specular_tint_blend_m_0");
                    break;
            }

            switch (material_1)
            {
                case Material.Off:
                    break;

                case Material.Diffuse_Only:
                    result.AddSamplerParameter("base_map_m_1");
                    result.AddSamplerParameter("detail_map_m_1");
                    result.AddSamplerParameter("bump_map_m_1");
                    result.AddSamplerParameter("detail_bump_m_1");
                    break;

                case Material.Diffuse_Plus_Specular:
                    result.AddSamplerParameter("base_map_m_1");
                    result.AddSamplerParameter("detail_map_m_1");
                    result.AddSamplerParameter("bump_map_m_1");
                    result.AddSamplerParameter("detail_bump_m_1");

                    result.AddFloatParameter("diffuse_coefficient_m_1");
                    result.AddFloatParameter("specular_coefficient_m_1");
                    result.AddFloatParameter("specular_power_m_1");
                    result.AddFloat3Parameter("specular_tint_m_1");
                    result.AddFloatParameter("fresnel_curve_steepness_m_1");
                    result.AddFloatParameter("area_specular_contribution_m_1");
                    result.AddFloatParameter("analytical_specular_contribution_m_1");
                    result.AddFloatParameter("environment_specular_contribution_m_1");
                    result.AddFloatParameter("albedo_specular_tint_blend_m_1");
                    break;
            }

            switch (material_2)
            {
                case Material.Off:
                    break;

                case Material.Diffuse_Only:
                    result.AddSamplerParameter("base_map_m_2");
                    result.AddSamplerParameter("detail_map_m_2");
                    result.AddSamplerParameter("bump_map_m_2");
                    result.AddSamplerParameter("detail_bump_m_2");
                    break;

                case Material.Diffuse_Plus_Specular:
                    result.AddSamplerParameter("base_map_m_2");
                    result.AddSamplerParameter("detail_map_m_2");
                    result.AddSamplerParameter("bump_map_m_2");
                    result.AddSamplerParameter("detail_bump_m_2");

                    result.AddFloatParameter("diffuse_coefficient_m_2");
                    result.AddFloatParameter("specular_coefficient_m_2");
                    result.AddFloatParameter("specular_power_m_2");
                    result.AddFloat3Parameter("specular_tint_m_2");
                    result.AddFloatParameter("fresnel_curve_steepness_m_2");
                    result.AddFloatParameter("area_specular_contribution_m_2");
                    result.AddFloatParameter("analytical_specular_contribution_m_2");
                    result.AddFloatParameter("environment_specular_contribution_m_2");
                    result.AddFloatParameter("albedo_specular_tint_blend_m_2");
                    break;
            }

            switch (material_3)
            {
                case Material_No_Detail_Bump.Off:
                    break;

                case Material_No_Detail_Bump.Diffuse_Only:
                    result.AddSamplerParameter("base_map_m_3");
                    result.AddSamplerParameter("detail_map_m_3");
                    result.AddSamplerParameter("bump_map_m_3");
                    result.AddSamplerParameter("detail_bump_m_3");
                    break;

                case Material_No_Detail_Bump.Diffuse_Plus_Specular:
                    result.AddSamplerParameter("base_map_m_3");
                    result.AddSamplerParameter("detail_map_m_3");
                    result.AddSamplerParameter("bump_map_m_3");
                    result.AddSamplerParameter("detail_bump_m_3");

                    result.AddFloatParameter("diffuse_coefficient_m_3");
                    result.AddFloatParameter("specular_coefficient_m_3");
                    result.AddFloatParameter("specular_power_m_3");
                    result.AddFloat3Parameter("specular_tint_m_3");
                    result.AddFloatParameter("fresnel_curve_steepness_m_3");
                    result.AddFloatParameter("area_specular_contribution_m_3");
                    result.AddFloatParameter("analytical_specular_contribution_m_3");
                    result.AddFloatParameter("environment_specular_contribution_m_3");
                    result.AddFloatParameter("albedo_specular_tint_blend_m_3");
                    break;
            }

            return result;
        }

        public ShaderParameters GetVertexShaderParameters()
        {
            return new ShaderParameters();
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();
            result.AddSamplerWithoutXFormParameter("albedo_texture", RenderMethodExtern.texture_global_target_texaccum);
            result.AddSamplerWithoutXFormParameter("normal_texture", RenderMethodExtern.texture_global_target_normal);
            result.AddSamplerWithoutXFormParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture);
            result.AddSamplerWithoutXFormParameter("shadow_depth_map_1", RenderMethodExtern.texture_global_target_shadow_buffer1);
            result.AddSamplerWithoutXFormParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0);
            result.AddFloat4Parameter("debug_tint", RenderMethodExtern.debug_tint);
            result.AddSamplerWithoutXFormParameter("active_camo_distortion_texture", RenderMethodExtern.active_camo_distortion_texture);
            result.AddSamplerWithoutXFormParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerWithoutXFormParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture);
            result.AddSamplerWithoutXFormParameter("dominant_light_intensity_map", RenderMethodExtern.texture_dominant_light_intensity_map);
            return result;
        }

        public ShaderParameters GetParametersInOption(string methodName, int option, out string rmopName, out string optionName)
        {
            ShaderParameters result = new ShaderParameters();
            rmopName = null;
            optionName = null;

            if (methodName == "blending")
            {
                optionName = ((Blending)option).ToString();

                switch ((Blending)option)
                {
                    case Blending.Morph:
                        result.AddSamplerParameter("blend_map");
                        result.AddFloatParameter("global_albedo_tint");
                        rmopName = @"shaders\terrain_options\default_blending";
                        break;
                    case Blending.Dynamic_Morph:
                        result.AddSamplerParameter("blend_map");
                        result.AddFloatParameter("global_albedo_tint");
                        result.AddFloatParameter("dynamic_material");
                        result.AddFloatParameter("transition_sharpness");
                        result.AddFloatParameter("transition_threshold");
                        rmopName = @"shaders\terrain_options\dynamic_blending";
                        break;
                }
            }
            if (methodName == "environment_mapping")
            {
                optionName = ((Environment_Mapping)option).ToString();

                switch ((Environment_Mapping)option)
                {
                    case Environment_Mapping.Per_Pixel:
                        result.AddSamplerWithoutXFormParameter("environment_map");
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddFloatParameter("env_roughness_scale");
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.Dynamic:
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddSamplerParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0);
                        result.AddSamplerParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1);
                        result.AddFloatParameter("env_roughness_scale");
                        rmopName = @"shaders\shader_options\env_map_dynamic";
                        break;
                }
            }
            if (methodName == "material_0")
            {
                optionName = ((Material)option).ToString();

                switch ((Material)option)
                {
                    case Material.Diffuse_Only:
                        result.AddSamplerParameter("base_map_m_0");
                        result.AddSamplerParameter("detail_map_m_0");
                        result.AddSamplerParameter("bump_map_m_0");
                        result.AddSamplerParameter("detail_bump_m_0");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_0";
                        break;
                    case Material.Diffuse_Plus_Specular:
                        result.AddSamplerParameter("base_map_m_0");
                        result.AddSamplerParameter("detail_map_m_0");
                        result.AddSamplerParameter("bump_map_m_0");
                        result.AddSamplerParameter("detail_bump_m_0");
                        result.AddFloatParameter("diffuse_coefficient_m_0");
                        result.AddFloatParameter("specular_coefficient_m_0");
                        result.AddFloatParameter("specular_power_m_0");
                        result.AddFloat3Parameter("specular_tint_m_0");
                        result.AddFloatParameter("fresnel_curve_steepness_m_0");
                        result.AddFloatParameter("area_specular_contribution_m_0");
                        result.AddFloatParameter("analytical_specular_contribution_m_0");
                        result.AddFloatParameter("environment_specular_contribution_m_0");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_0");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_m_0";
                        break;
                }
            }
            if (methodName == "material_1")
            {
                optionName = ((Material)option).ToString();

                switch ((Material)option)
                {
                    case Material.Diffuse_Only:
                        result.AddSamplerParameter("base_map_m_1");
                        result.AddSamplerParameter("detail_map_m_1");
                        result.AddSamplerParameter("bump_map_m_1");
                        result.AddSamplerParameter("detail_bump_m_1");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_1";
                        break;
                    case Material.Diffuse_Plus_Specular:
                        result.AddSamplerParameter("base_map_m_1");
                        result.AddSamplerParameter("detail_map_m_1");
                        result.AddSamplerParameter("bump_map_m_1");
                        result.AddSamplerParameter("detail_bump_m_1");
                        result.AddFloatParameter("diffuse_coefficient_m_1");
                        result.AddFloatParameter("specular_coefficient_m_1");
                        result.AddFloatParameter("specular_power_m_1");
                        result.AddFloat3Parameter("specular_tint_m_1");
                        result.AddFloatParameter("fresnel_curve_steepness_m_1");
                        result.AddFloatParameter("area_specular_contribution_m_1");
                        result.AddFloatParameter("analytical_specular_contribution_m_1");
                        result.AddFloatParameter("environment_specular_contribution_m_1");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_1");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_m_1";
                        break;
                }
            }
            if (methodName == "material_2")
            {
                optionName = ((Material)option).ToString();

                switch ((Material)option)
                {
                    case Material.Diffuse_Only:
                        result.AddSamplerParameter("base_map_m_2");
                        result.AddSamplerParameter("detail_map_m_2");
                        result.AddSamplerParameter("bump_map_m_2");
                        result.AddSamplerParameter("detail_bump_m_2");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_2";
                        break;
                    case Material.Diffuse_Plus_Specular:
                        result.AddSamplerParameter("base_map_m_2");
                        result.AddSamplerParameter("detail_map_m_2");
                        result.AddSamplerParameter("bump_map_m_2");
                        result.AddSamplerParameter("detail_bump_m_2");
                        result.AddFloatParameter("diffuse_coefficient_m_2");
                        result.AddFloatParameter("specular_coefficient_m_2");
                        result.AddFloatParameter("specular_power_m_2");
                        result.AddFloat3Parameter("specular_tint_m_2");
                        result.AddFloatParameter("fresnel_curve_steepness_m_2");
                        result.AddFloatParameter("area_specular_contribution_m_2");
                        result.AddFloatParameter("analytical_specular_contribution_m_2");
                        result.AddFloatParameter("environment_specular_contribution_m_2");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_2");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_m_2";
                        break;
                }
            }
            if (methodName == "material_3")
            {
                optionName = ((Material_No_Detail_Bump)option).ToString();

                switch ((Material_No_Detail_Bump)option)
                {
                    case Material_No_Detail_Bump.Diffuse_Only:
                        result.AddSamplerParameter("base_map_m_3");
                        result.AddSamplerParameter("detail_map_m_3");
                        result.AddSamplerParameter("bump_map_m_3");
                        result.AddSamplerParameter("detail_bump_m_3");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_3";
                        break;
                    case Material_No_Detail_Bump.Diffuse_Plus_Specular:
                        result.AddSamplerParameter("base_map_m_3");
                        result.AddSamplerParameter("detail_map_m_3");
                        result.AddSamplerParameter("bump_map_m_3");
                        result.AddSamplerParameter("detail_bump_m_3");
                        result.AddFloatParameter("diffuse_coefficient_m_3");
                        result.AddFloatParameter("specular_coefficient_m_3");
                        result.AddFloatParameter("specular_power_m_3");
                        result.AddFloat3Parameter("specular_tint_m_3");
                        result.AddFloatParameter("fresnel_curve_steepness_m_3");
                        result.AddFloatParameter("area_specular_contribution_m_3");
                        result.AddFloatParameter("analytical_specular_contribution_m_3");
                        result.AddFloatParameter("environment_specular_contribution_m_3");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_3");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_m_3";
                        break;
                }
            }

            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(TerrainMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((TerrainMethods)methodIndex)
            {
                case TerrainMethods.Blending:
                    return Enum.GetValues(typeof(Blending));
                case TerrainMethods.Environment_Map:
                    return Enum.GetValues(typeof(Environment_Mapping));
                case TerrainMethods.Material_0:
                case TerrainMethods.Material_1:
                case TerrainMethods.Material_2:
                    return Enum.GetValues(typeof(Material));
                case TerrainMethods.Material_3:
                    return Enum.GetValues(typeof(Material_No_Detail_Bump));
            }

            return null;
        }
    }
}
