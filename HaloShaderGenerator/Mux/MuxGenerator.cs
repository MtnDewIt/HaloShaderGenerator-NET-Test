using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Mux
{
    public class MuxGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;
        private bool ApplyFixes;

        Blending blending;
        Albedo albedo;
        Bump bump;
        Materials materials;
        Environment_Mapping environment_mapping;
        Parallax parallax;
        Wetness wetness;

        public MuxGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        public MuxGenerator(Blending blending, Albedo albedo, Bump bump, Materials materials, Environment_Mapping environment_mapping, Parallax parallax, Wetness wetness, bool applyFixes = false)
        {
            this.blending = blending;
            this.albedo = albedo;
            this.bump = bump;
            this.materials = materials;
            this.environment_mapping = environment_mapping;
            this.parallax = parallax;
            this.wetness = wetness;

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public MuxGenerator(byte[] options, bool applyFixes = false)
        {
            options = ValidateOptions(options);

            this.blending = (Blending)options[0];
            this.albedo = (Albedo)options[1];
            this.bump = (Bump)options[2];
            this.materials = (Materials)options[3];
            this.environment_mapping = (Environment_Mapping)options[4];
            this.parallax = (Parallax)options[5];
            this.wetness = (Wetness)options[6];

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public ShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Mux, entryPoint, Shared.Blend_Mode.Opaque,
                Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes);

            switch (blending)
            {
                case Blending.Standard:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_blend", "standard"));
                    break;
            }

            switch (albedo)
            {
                case Albedo.Base_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_base_ps"));
                    break;
                case Albedo.Base_And_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_detail_ps"));
                    break;
            }

            switch (bump)
            {
                case Bump.Base_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_default_ps"));
                    break;
                case Bump.Base_And_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_ps"));
                    break;
            }

            switch (materials)
            {
                case Materials.Diffuse_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "diffuse_only"));
                    break;
                case Materials.Single_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "single_lobe_phong"));
                    break;
            }

            switch (environment_mapping)
            {
                case Environment_Mapping.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "none"));
                    break;
                case Environment_Mapping.Per_Pixel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "per_pixel"));
                    break;
                case Environment_Mapping.Dynamic:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "dynamic"));
                    break;
            }

            switch (parallax)
            {
                case Parallax.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Parallax.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
            }

            switch (wetness)
            {
                case Wetness.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_default_ps"));
                    break;
                case Wetness.Flood:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_flood_ps"));
                    break;
                case Wetness.Proof:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_proof_ps"));
                    break;
                case Wetness.Ripples:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_ripples_ps"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Mux);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Mux, entryPoint, Shared.Blend_Mode.Opaque,
                Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes);

            switch (blending)
            {
                case Blending.Standard:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_blend", "standard"));
                    break;
            }

            switch (albedo)
            {
                case Albedo.Base_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_base_ps"));
                    break;
                case Albedo.Base_And_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_detail_ps"));
                    break;
            }

            switch (bump)
            {
                case Bump.Base_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_default_ps"));
                    break;
                case Bump.Base_And_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_ps"));
                    break;
            }

            switch (materials)
            {
                case Materials.Diffuse_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "diffuse_only"));
                    break;
                case Materials.Single_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "single_lobe_phong"));
                    break;
            }

            switch (environment_mapping)
            {
                case Environment_Mapping.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "none"));
                    break;
                case Environment_Mapping.Per_Pixel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "per_pixel"));
                    break;
                case Environment_Mapping.Dynamic:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "dynamic"));
                    break;
            }

            switch (parallax)
            {
                case Parallax.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Parallax.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
            }

            switch (wetness)
            {
                case Wetness.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_default_ps"));
                    break;
                case Wetness.Flood:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_flood_ps"));
                    break;
                case Wetness.Proof:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_proof_ps"));
                    break;
                case Wetness.Ripples:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_ripples_ps"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Mux);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Mux, entryPoint,
                Shared.Blend_Mode.Opaque, Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes, true, vertexType);

            switch (blending)
            {
                case Blending.Standard:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_blend", "standard"));
                    break;
            }

            switch (albedo)
            {
                case Albedo.Base_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_base_ps"));
                    break;
                case Albedo.Base_And_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_detail_ps"));
                    break;
            }

            switch (bump)
            {
                case Bump.Base_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_default_ps"));
                    break;
                case Bump.Base_And_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_ps"));
                    break;
            }

            switch (materials)
            {
                case Materials.Diffuse_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "diffuse_only"));
                    break;
                case Materials.Single_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "single_lobe_phong"));
                    break;
            }

            switch (environment_mapping)
            {
                case Environment_Mapping.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "none"));
                    break;
                case Environment_Mapping.Per_Pixel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "per_pixel"));
                    break;
                case Environment_Mapping.Dynamic:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "dynamic"));
                    break;
            }

            switch (parallax)
            {
                case Parallax.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Parallax.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
            }

            switch (wetness)
            {
                case Wetness.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_default_ps"));
                    break;
                case Wetness.Flood:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_flood_ps"));
                    break;
                case Wetness.Proof:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_proof_ps"));
                    break;
                case Wetness.Ripples:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_ripples_ps"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Mux);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Mux, entryPoint,
                Shared.Blend_Mode.Opaque, Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes, true, vertexType);

            switch (blending)
            {
                case Blending.Standard:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_blend", "standard"));
                    break;
            }

            switch (albedo)
            {
                case Albedo.Base_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_base_ps"));
                    break;
                case Albedo.Base_And_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_detail_ps"));
                    break;
            }

            switch (bump)
            {
                case Bump.Base_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_default_ps"));
                    break;
                case Bump.Base_And_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", "calc_bumpmap_detail_ps"));
                    break;
            }

            switch (materials)
            {
                case Materials.Diffuse_Only:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "diffuse_only"));
                    break;
                case Materials.Single_Lobe_Phong:
                    macros.Add(ShaderGeneratorBase.CreateMacro("material_type", "single_lobe_phong"));
                    break;
            }

            switch (environment_mapping)
            {
                case Environment_Mapping.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "none"));
                    break;
                case Environment_Mapping.Per_Pixel:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "per_pixel"));
                    break;
                case Environment_Mapping.Dynamic:
                    macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", "dynamic"));
                    break;
            }

            switch (parallax)
            {
                case Parallax.Off:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Parallax.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
            }

            switch (wetness)
            {
                case Wetness.Default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_default_ps"));
                    break;
                case Wetness.Flood:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_flood_ps"));
                    break;
                case Wetness.Proof:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_proof_ps"));
                    break;
                case Wetness.Ripples:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_wetness_ps", "calc_wetness_ripples_ps"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Mux);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(MuxMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((MuxMethods)methodIndex)
            {
                case MuxMethods.Blending:
                    return Enum.GetValues(typeof(Blending)).Length;
                case MuxMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case MuxMethods.Bump:
                    return Enum.GetValues(typeof(Bump)).Length;
                case MuxMethods.Materials:
                    return Enum.GetValues(typeof(Materials)).Length;
                case MuxMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping)).Length;
                case MuxMethods.Parallax:
                    return Enum.GetValues(typeof(Parallax)).Length;
                case MuxMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness)).Length;
            }

            return -1;
        }

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((MuxMethods)methodIndex)
            {
                case MuxMethods.Blending:
                    return (int)blending;
                case MuxMethods.Albedo:
                    return (int)albedo;
                case MuxMethods.Bump:
                    return (int)bump;
                case MuxMethods.Materials:
                    return (int)materials;
                case MuxMethods.Environment_Mapping:
                    return (int)environment_mapping;
                case MuxMethods.Parallax:
                    return (int)parallax;
                case MuxMethods.Wetness:
                    return (int)wetness;
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
                //case ShaderStage.Imposter_Static_Prt_Ambient:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsMethodSharedInEntryPoint(ShaderStage entryPoint, int method_index)
        {
            switch (method_index)
            {
                default:
                    return false;
            }
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                default:
                    return false;
            }
        }

        public bool IsSharedPixelShaderWithoutMethod(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Shadow_Generate:
                    return true;
                default:
                    return false;
            }
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
            switch (entryPoint)
            {
                case ShaderStage.Albedo:
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Static_Sh:
                case ShaderStage.Dynamic_Light:
                case ShaderStage.Shadow_Generate:
                case ShaderStage.Lightmap_Debug_Mode:
                case ShaderStage.Dynamic_Light_Cinematic:
                    return true;
                default:
                    return false;
            }
        }

        public ShaderParameters GetPixelShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;
            var result = new ShaderParameters();

            switch (blending)
            {
                case Blending.Standard:
                    result.AddSamplerWithoutXFormParameter("material_map");
                    result.AddFloatParameter("blend_material_scale");
                    result.AddFloatParameter("blend_material_offset");
                    result.AddFloatParameter("pc_atlas_scale_x");
                    result.AddFloatParameter("pc_atlas_scale_y");
                    result.AddFloatParameter("pc_atlas_transform_x");
                    result.AddFloatParameter("pc_atlas_transform_y");
                    result.AddFloatParameter("blend_material_count");
                    break;
            }

            switch (albedo)
            {
                case Albedo.Base_Only:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    break;
                case Albedo.Base_And_Detail:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("detail_map");
                    break;
            }

            switch (bump)
            {
                case Bump.Base_Only:
                    result.AddSamplerWithoutXFormParameter("bump_map");
                    break;
                case Bump.Base_And_Detail:
                    result.AddSamplerWithoutXFormParameter("bump_map");
                    result.AddSamplerWithoutXFormParameter("bump_detail_map");
                    break;
            }

            switch (materials)
            {
                case Materials.Diffuse_Only:
                    result.AddBooleanParameter("no_dynamic_lights");
                    result.AddFloatParameter("approximate_specular_type");
                    break;
                case Materials.Single_Lobe_Phong:
                    result.AddSamplerWithoutXFormParameter("material_property0_map");
                    result.AddSamplerWithoutXFormParameter("material_property1_map");
                    result.AddBooleanParameter("no_dynamic_lights");
                    break;
            }

            switch (environment_mapping)
            {
                case Environment_Mapping.None:
                    break;
                case Environment_Mapping.Per_Pixel:
                    result.AddSamplerWithoutXFormParameter("environment_map");
                    result.AddFloat3ColorParameter("env_tint_color");
                    result.AddFloatParameter("env_roughness_offset");
                    break;
                case Environment_Mapping.Dynamic:
                    result.AddFloat3ColorParameter("env_tint_color");
                    result.AddFloatParameter("env_roughness_scale");
                    result.AddFloatParameter("env_roughness_offset");
                    break;
            }

            switch (parallax)
            {
                case Parallax.Off:
                    break;
                case Parallax.Simple:
                    result.AddSamplerWithoutXFormParameter("height_map");
                    result.AddFloatParameter("height_scale");
                    break;
            }

            switch (wetness)
            {
                case Wetness.Default:
                    result.AddFloatParameter("wet_material_dim_coefficient");
                    result.AddFloat3ColorParameter("wet_material_dim_tint");
                    break;
                case Wetness.Flood:
                    result.AddFloatParameter("wet_material_dim_coefficient");
                    result.AddFloat3ColorParameter("wet_material_dim_tint");
                    result.AddFloatParameter("wet_sheen_reflection_contribution");
                    result.AddFloat3ColorParameter("wet_sheen_reflection_tint");
                    result.AddFloatParameter("wet_sheen_thickness");
                    result.AddSamplerWithoutXFormParameter("wet_flood_slope_map");
                    result.AddSamplerWithoutXFormParameter("wet_noise_boundary_map");
                    result.AddFloatParameter("specular_mask_tweak_weight");
                    result.AddFloatParameter("surface_tilt_tweak_weight");
                    break;
                case Wetness.Proof:
                    break;
                case Wetness.Ripples:
                    result.AddFloatParameter("wet_material_dim_coefficient");
                    result.AddFloat3ColorParameter("wet_material_dim_tint");
                    result.AddFloatParameter("wet_sheen_reflection_contribution");
                    result.AddFloat3ColorParameter("wet_sheen_reflection_tint");
                    result.AddFloatParameter("wet_sheen_thickness");
                    result.AddSamplerWithoutXFormParameter("wet_noise_boundary_map");
                    result.AddFloatParameter("specular_mask_tweak_weight");
                    result.AddFloatParameter("surface_tilt_tweak_weight");
                    break;
            }

            return result;
        }

        public ShaderParameters GetVertexShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;
            var result = new ShaderParameters();

            switch (blending)
            {
                case Blending.Standard:
                    break;
            }

            switch (albedo)
            {
                case Albedo.Base_Only:
                    break;
                case Albedo.Base_And_Detail:
                    break;
            }

            switch (bump)
            {
                case Bump.Base_Only:
                    break;
                case Bump.Base_And_Detail:
                    break;
            }

            switch (materials)
            {
                case Materials.Diffuse_Only:
                    break;
                case Materials.Single_Lobe_Phong:
                    break;
            }

            switch (environment_mapping)
            {
                case Environment_Mapping.None:
                    break;
                case Environment_Mapping.Per_Pixel:
                    break;
                case Environment_Mapping.Dynamic:
                    break;
            }

            switch (parallax)
            {
                case Parallax.Off:
                    break;
                case Parallax.Simple:
                    break;
            }

            switch (wetness)
            {
                case Wetness.Default:
                    break;
                case Wetness.Flood:
                    break;
                case Wetness.Proof:
                    break;
                case Wetness.Ripples:
                    break;
            }

            return result;
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();
            result.AddSamplerWithoutXFormParameter("albedo_texture", RenderMethodExtern.texture_global_target_texaccum);
            result.AddSamplerWithoutXFormParameter("normal_texture", RenderMethodExtern.texture_global_target_normal);
            result.AddSamplerWithoutXFormParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0);
            result.AddFloat3ColorParameter("debug_tint", RenderMethodExtern.debug_tint);
            result.AddSamplerWithoutXFormParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerWithoutXFormParameter("scene_hdr_texture");
            result.AddSamplerWithoutXFormParameter("g_sample_vmf_phong_specular");
            result.AddSamplerWithoutXFormParameter("g_direction_lut");
            result.AddSamplerWithoutXFormParameter("g_sample_vmf_diffuse");
            result.AddSamplerWithoutXFormParameter("g_diffuse_power_specular");
            result.AddSamplerWithoutXFormParameter("shadow_mask_texture", RenderMethodExtern.none);
            result.AddSamplerWithoutXFormParameter("g_sample_vmf_diffuse_vs");
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
                    case Blending.Standard:
                        result.AddSamplerWithoutXFormParameter("material_map");
                        result.AddFloatParameter("blend_material_scale");
                        result.AddFloatParameter("blend_material_offset");
                        result.AddFloatParameter("pc_atlas_scale_x");
                        result.AddFloatParameter("pc_atlas_scale_y");
                        result.AddFloatParameter("pc_atlas_transform_x");
                        result.AddFloatParameter("pc_atlas_transform_y");
                        result.AddFloatParameter("blend_material_count");
                        rmopName = @"shaders\shader_options\mux_blend_standard";
                        break;
                }
            }

            if (methodName == "albedo")
            {
                optionName = ((Albedo)option).ToString();

                switch ((Albedo)option)
                {
                    case Albedo.Base_Only:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        rmopName = @"shaders\shader_options\mux_albedo_base_only";
                        break;
                    case Albedo.Base_And_Detail:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("detail_map");
                        rmopName = @"shaders\shader_options\mux_albedo";
                        break;
                }
            }

            if (methodName == "bump")
            {
                optionName = ((Bump)option).ToString();

                switch ((Bump)option)
                {
                    case Bump.Base_Only:
                        result.AddSamplerWithoutXFormParameter("bump_map");
                        rmopName = @"shaders\shader_options\bump_default";
                        break;
                    case Bump.Base_And_Detail:
                        result.AddSamplerWithoutXFormParameter("bump_map");
                        result.AddSamplerWithoutXFormParameter("bump_detail_map");
                        rmopName = @"shaders\shader_options\bump_detail";
                        break;
                }
            }

            if (methodName == "materials")
            {
                optionName = ((Materials)option).ToString();

                switch ((Materials)option)
                {
                    case Materials.Diffuse_Only:
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("approximate_specular_type");
                        rmopName = @"shaders\shader_options\material_diffuse_only";
                        break;
                    case Materials.Single_Lobe_Phong:
                        result.AddSamplerWithoutXFormParameter("material_property0_map");
                        result.AddSamplerWithoutXFormParameter("material_property1_map");
                        result.AddBooleanParameter("no_dynamic_lights");
                        rmopName = @"shaders\shader_options\mux_single_lobe_phong";
                        break;
                }
            }

            if (methodName == "environment_mapping")
            {
                optionName = ((Environment_Mapping)option).ToString();

                switch ((Environment_Mapping)option)
                {
                    case Environment_Mapping.None:
                        break;
                    case Environment_Mapping.Per_Pixel:
                        result.AddSamplerWithoutXFormParameter("environment_map");
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddFloatParameter("env_roughness_offset");
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.Dynamic:
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddFloatParameter("env_roughness_scale");
                        result.AddFloatParameter("env_roughness_offset");
                        rmopName = @"shaders\shader_options\env_map_dynamic";
                        break;
                }
            }

            if (methodName == "parallax")
            {
                optionName = ((Parallax)option).ToString();

                switch ((Parallax)option)
                {
                    case Parallax.Off:
                        break;
                    case Parallax.Simple:
                        result.AddSamplerWithoutXFormParameter("height_map");
                        result.AddFloatParameter("height_scale");
                        rmopName = @"shaders\shader_options\parallax_simple";
                        break;
                }
            }

            if (methodName == "wetness")
            {
                optionName = ((Wetness)option).ToString();

                switch ((Wetness)option)
                {
                    case Wetness.Default:
                        result.AddFloatParameter("wet_material_dim_coefficient");
                        result.AddFloat3ColorParameter("wet_material_dim_tint");
                        rmopName = @"shaders\wetness_options\wetness_simple";
                        break;
                    case Wetness.Flood:
                        result.AddFloatParameter("wet_material_dim_coefficient");
                        result.AddFloat3ColorParameter("wet_material_dim_tint");
                        result.AddFloatParameter("wet_sheen_reflection_contribution");
                        result.AddFloat3ColorParameter("wet_sheen_reflection_tint");
                        result.AddFloatParameter("wet_sheen_thickness");
                        result.AddSamplerWithoutXFormParameter("wet_flood_slope_map");
                        result.AddSamplerWithoutXFormParameter("wet_noise_boundary_map");
                        result.AddFloatParameter("specular_mask_tweak_weight");
                        result.AddFloatParameter("surface_tilt_tweak_weight");
                        rmopName = @"shaders\wetness_options\wetness_flood";
                        break;
                    case Wetness.Proof:
                        break;
                    case Wetness.Ripples:
                        result.AddFloatParameter("wet_material_dim_coefficient");
                        result.AddFloat3ColorParameter("wet_material_dim_tint");
                        result.AddFloatParameter("wet_sheen_reflection_contribution");
                        result.AddFloat3ColorParameter("wet_sheen_reflection_tint");
                        result.AddFloatParameter("wet_sheen_thickness");
                        result.AddSamplerWithoutXFormParameter("wet_noise_boundary_map");
                        result.AddFloatParameter("specular_mask_tweak_weight");
                        result.AddFloatParameter("surface_tilt_tweak_weight");
                        rmopName = @"shaders\wetness_options\wetness_ripples";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(MuxMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((MuxMethods)methodIndex)
            {
                case MuxMethods.Blending:
                    return Enum.GetValues(typeof(Blending));
                case MuxMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case MuxMethods.Bump:
                    return Enum.GetValues(typeof(Bump));
                case MuxMethods.Materials:
                    return Enum.GetValues(typeof(Materials));
                case MuxMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping));
                case MuxMethods.Parallax:
                    return Enum.GetValues(typeof(Parallax));
                case MuxMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness));
            }

            return null;
        }

        public byte[] ValidateOptions(byte[] options)
        {
            List<byte> optionList = new List<byte>(options);

            while (optionList.Count < GetMethodCount())
                optionList.Add(0);

            return optionList.ToArray();
        }

        public void GetCategoryFunctions(string methodName, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "blending")
            {
                vertexFunction = "invalid";
                pixelFunction = "material_blend";
            }

            if (methodName == "albedo")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_albedo_ps";
            }

            if (methodName == "bump")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_bumpmap_ps";
            }

            if (methodName == "materials")
            {
                vertexFunction = "invalid";
                pixelFunction = "material_type";
            }

            if (methodName == "environment_mapping")
            {
                vertexFunction = "invalid";
                pixelFunction = "envmap_type";
            }

            if (methodName == "parallax")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_parallax_ps";
            }

            if (methodName == "wetness")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_wetness_ps";
            }
        }

        public void GetOptionFunctions(string methodName, int option, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "blending")
            {
                switch ((Blending)option)
                {
                    case Blending.Standard:
                        vertexFunction = "invalid";
                        pixelFunction = "standard";
                        break;
                }
            }

            if (methodName == "albedo")
            {
                switch ((Albedo)option)
                {
                    case Albedo.Base_Only:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_albedo_base_ps";
                        break;
                    case Albedo.Base_And_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_albedo_detail_ps";
                        break;
                }
            }

            if (methodName == "bump")
            {
                switch ((Bump)option)
                {
                    case Bump.Base_Only:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_bumpmap_default_ps";
                        break;
                    case Bump.Base_And_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_bumpmap_detail_ps";
                        break;
                }
            }

            if (methodName == "materials")
            {
                switch ((Materials)option)
                {
                    case Materials.Diffuse_Only:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_only";
                        break;
                    case Materials.Single_Lobe_Phong:
                        vertexFunction = "invalid";
                        pixelFunction = "single_lobe_phong";
                        break;
                }
            }

            if (methodName == "environment_mapping")
            {
                switch ((Environment_Mapping)option)
                {
                    case Environment_Mapping.None:
                        vertexFunction = "invalid";
                        pixelFunction = "none";
                        break;
                    case Environment_Mapping.Per_Pixel:
                        vertexFunction = "invalid";
                        pixelFunction = "per_pixel";
                        break;
                    case Environment_Mapping.Dynamic:
                        vertexFunction = "invalid";
                        pixelFunction = "dynamic";
                        break;
                }
            }

            if (methodName == "parallax")
            {
                switch ((Parallax)option)
                {
                    case Parallax.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_parallax_off_ps";
                        break;
                    case Parallax.Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_parallax_simple_ps";
                        break;
                }
            }

            if (methodName == "wetness")
            {
                switch ((Wetness)option)
                {
                    case Wetness.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_default_ps";
                        break;
                    case Wetness.Flood:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_flood_ps";
                        break;
                    case Wetness.Proof:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_proof_ps";
                        break;
                    case Wetness.Ripples:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_ripples_ps";
                        break;
                }
            }
        }

        public ShaderParameters GetParameterArguments(string methodName, int option)
        {
            ShaderParameters result = new ShaderParameters();
            if (methodName == "blending")
            {
                switch ((Blending)option)
                {
                    case Blending.Standard:
                        break;
                }
            }

            if (methodName == "albedo")
            {
                switch ((Albedo)option)
                {
                    case Albedo.Base_Only:
                        break;
                    case Albedo.Base_And_Detail:
                        break;
                }
            }

            if (methodName == "bump")
            {
                switch ((Bump)option)
                {
                    case Bump.Base_Only:
                        break;
                    case Bump.Base_And_Detail:
                        break;
                }
            }

            if (methodName == "materials")
            {
                switch ((Materials)option)
                {
                    case Materials.Diffuse_Only:
                        break;
                    case Materials.Single_Lobe_Phong:
                        break;
                }
            }

            if (methodName == "environment_mapping")
            {
                switch ((Environment_Mapping)option)
                {
                    case Environment_Mapping.None:
                        break;
                    case Environment_Mapping.Per_Pixel:
                        break;
                    case Environment_Mapping.Dynamic:
                        break;
                }
            }

            if (methodName == "parallax")
            {
                switch ((Parallax)option)
                {
                    case Parallax.Off:
                        break;
                    case Parallax.Simple:
                        break;
                }
            }

            if (methodName == "wetness")
            {
                switch ((Wetness)option)
                {
                    case Wetness.Default:
                        break;
                    case Wetness.Flood:
                        break;
                    case Wetness.Proof:
                        break;
                    case Wetness.Ripples:
                        break;
                }
            }
            return result;
        }
    }
}
