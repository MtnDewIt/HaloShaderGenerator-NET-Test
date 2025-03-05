using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Fur
{
    public class FurGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;
        private bool ApplyFixes;

        Albedo albedo;
        Warp warp;
        Overlay overlay;
        Edge_Fade edge_fade;
        Blend_Mode blend_mode;

        public FurGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        public FurGenerator(Albedo albedo, Warp warp, Overlay overlay, Edge_Fade edge_fade, Blend_Mode blend_mode, bool applyFixes = false)
        {
            this.albedo = albedo;
            this.warp = warp;
            this.overlay = overlay;
            this.edge_fade = edge_fade;
            this.blend_mode = blend_mode;

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public FurGenerator(byte[] options, bool applyFixes = false)
        {
            options = ValidateOptions(options);

            this.albedo = (Albedo)options[0];
            this.warp = (Warp)options[1];
            this.overlay = (Overlay)options[2];
            this.edge_fade = (Edge_Fade)options[3];
            this.blend_mode = (Blend_Mode)options[4];

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public ShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Fur, entryPoint, sBlendMode,
                Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes);

            switch (albedo)
            {
                case Albedo.Fur_Multilayer:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_multilayer_ps"));
                    break;
            }

            switch (warp)
            {
                case Warp.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Warp.From_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_warp_from_texture_ps"));
                    break;
                case Warp.Parallax_Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
            }

            switch (overlay)
            {
                case Overlay.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_none_ps"));
                    break;
                case Overlay.Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_ps"));
                    break;
                case Overlay.Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_detail_ps"));
                    break;
                case Overlay.Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_ps"));
                    break;
                case Overlay.Multiply_And_Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_and_additive_detail_ps"));
                    break;
            }

            switch (edge_fade)
            {
                case Edge_Fade.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_none_ps"));
                    break;
                case Edge_Fade.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_simple_ps"));
                    break;
            }

            switch (blend_mode)
            {
                case Blend_Mode.Opaque:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "opaque"));
                    break;
                case Blend_Mode.Alpha_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "alpha_blend"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Fur);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Fur, entryPoint, sBlendMode,
                Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes);

            switch (albedo)
            {
                case Albedo.Fur_Multilayer:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_multilayer_ps"));
                    break;
            }

            switch (warp)
            {
                case Warp.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Warp.From_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_warp_from_texture_ps"));
                    break;
                case Warp.Parallax_Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
            }

            switch (overlay)
            {
                case Overlay.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_none_ps"));
                    break;
                case Overlay.Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_ps"));
                    break;
                case Overlay.Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_detail_ps"));
                    break;
                case Overlay.Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_ps"));
                    break;
                case Overlay.Multiply_And_Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_and_additive_detail_ps"));
                    break;
            }

            switch (edge_fade)
            {
                case Edge_Fade.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_none_ps"));
                    break;
                case Edge_Fade.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_simple_ps"));
                    break;
            }

            switch (blend_mode)
            {
                case Blend_Mode.Opaque:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "opaque"));
                    break;
                case Blend_Mode.Alpha_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "alpha_blend"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Fur);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Fur, entryPoint,
                sBlendMode, Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes, true, vertexType);

            switch (albedo)
            {
                case Albedo.Fur_Multilayer:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_multilayer_ps"));
                    break;
            }

            switch (warp)
            {
                case Warp.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Warp.From_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_warp_from_texture_ps"));
                    break;
                case Warp.Parallax_Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
            }

            switch (overlay)
            {
                case Overlay.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_none_ps"));
                    break;
                case Overlay.Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_ps"));
                    break;
                case Overlay.Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_detail_ps"));
                    break;
                case Overlay.Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_ps"));
                    break;
                case Overlay.Multiply_And_Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_and_additive_detail_ps"));
                    break;
            }

            switch (edge_fade)
            {
                case Edge_Fade.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_none_ps"));
                    break;
                case Edge_Fade.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_simple_ps"));
                    break;
            }

            switch (blend_mode)
            {
                case Blend_Mode.Opaque:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "opaque"));
                    break;
                case Blend_Mode.Alpha_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "alpha_blend"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Fur);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Fur, entryPoint,
                sBlendMode, Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes, true, vertexType);

            switch (albedo)
            {
                case Albedo.Fur_Multilayer:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", "calc_albedo_multilayer_ps"));
                    break;
            }

            switch (warp)
            {
                case Warp.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_off_ps"));
                    break;
                case Warp.From_Texture:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_warp_from_texture_ps"));
                    break;
                case Warp.Parallax_Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", "calc_parallax_simple_ps"));
                    break;
            }

            switch (overlay)
            {
                case Overlay.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_none_ps"));
                    break;
                case Overlay.Additive:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_ps"));
                    break;
                case Overlay.Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_additive_detail_ps"));
                    break;
                case Overlay.Multiply:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_ps"));
                    break;
                case Overlay.Multiply_And_Additive_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", "calc_overlay_multiply_and_additive_detail_ps"));
                    break;
            }

            switch (edge_fade)
            {
                case Edge_Fade.None:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_none_ps"));
                    break;
                case Edge_Fade.Simple:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", "calc_edge_fade_simple_ps"));
                    break;
            }

            switch (blend_mode)
            {
                case Blend_Mode.Opaque:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "opaque"));
                    break;
                case Blend_Mode.Alpha_Blend:
                    macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", "alpha_blend"));
                    break;
            }

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Fur);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(FurMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((FurMethods)methodIndex)
            {
                case FurMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case FurMethods.Warp:
                    return Enum.GetValues(typeof(Warp)).Length;
                case FurMethods.Overlay:
                    return Enum.GetValues(typeof(Overlay)).Length;
                case FurMethods.Edge_Fade:
                    return Enum.GetValues(typeof(Edge_Fade)).Length;
                case FurMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
            }

            return -1;
        }

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((FurMethods)methodIndex)
            {
                case FurMethods.Albedo:
                    return (int)albedo;
                case FurMethods.Warp:
                    return (int)warp;
                case FurMethods.Overlay:
                    return (int)overlay;
                case FurMethods.Edge_Fade:
                    return (int)edge_fade;
                case FurMethods.Blend_Mode:
                    return (int)blend_mode;
            }

            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Albedo:
                case ShaderStage.Static_Sh:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Dynamic_Light:
                //case ShaderStage.Imposter_Static_Sh:
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
                default:
                    return false;
            }
        }

        public bool IsPixelShaderShared(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                default:
                    return false;
            }
        }

        public bool IsVertexFormatSupported(VertexType vertexType)
        {
            switch (vertexType)
            {
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
                case ShaderStage.Static_Sh:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Dynamic_Light:
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

            switch (albedo)
            {
                case Albedo.Fur_Multilayer:
                    result.AddSamplerWithoutXFormParameter("fur_hairs_map");
                    result.AddSamplerWithoutXFormParameter("fur_tint_map");
                    result.AddFloat3ColorParameter("fur_deep_color");
                    result.AddFloat3ColorParameter("fur_tint_color");
                    result.AddFloatParameter("fur_intensity");
                    result.AddFloatParameter("fur_alpha_scale");
                    result.AddFloatParameter("fur_shear_x");
                    result.AddFloatParameter("fur_shear_y");
                    result.AddFloatParameter("fur_fix");
                    result.AddFloatParameter("layer_depth");
                    result.AddIntegerParameter("layers_of_4");
                    result.AddFloatParameter("texcoord_aspect_ratio");
                    result.AddFloatParameter("depth_darken");
                    break;
            }

            switch (warp)
            {
                case Warp.None:
                    break;
                case Warp.From_Texture:
                    result.AddSamplerWithoutXFormParameter("warp_map");
                    result.AddFloatParameter("warp_amount_x");
                    result.AddFloatParameter("warp_amount_y");
                    break;
                case Warp.Parallax_Simple:
                    break;
            }

            switch (overlay)
            {
                case Overlay.None:
                    break;
                case Overlay.Additive:
                    result.AddSamplerWithoutXFormParameter("overlay_map");
                    result.AddFloat3ColorParameter("overlay_tint");
                    result.AddFloatParameter("overlay_intensity");
                    break;
                case Overlay.Additive_Detail:
                    result.AddSamplerWithoutXFormParameter("overlay_map");
                    result.AddSamplerWithoutXFormParameter("overlay_detail_map");
                    result.AddFloat3ColorParameter("overlay_tint");
                    result.AddFloatParameter("overlay_intensity");
                    break;
                case Overlay.Multiply:
                    result.AddSamplerWithoutXFormParameter("overlay_map");
                    result.AddFloat3ColorParameter("overlay_tint");
                    result.AddFloatParameter("overlay_intensity");
                    break;
                case Overlay.Multiply_And_Additive_Detail:
                    result.AddSamplerWithoutXFormParameter("overlay_multiply_map");
                    result.AddSamplerWithoutXFormParameter("overlay_map");
                    result.AddSamplerWithoutXFormParameter("overlay_detail_map");
                    result.AddFloat3ColorParameter("overlay_tint");
                    result.AddFloatParameter("overlay_intensity");
                    break;
            }

            switch (edge_fade)
            {
                case Edge_Fade.None:
                    break;
                case Edge_Fade.Simple:
                    result.AddFloat3ColorParameter("edge_fade_edge_tint");
                    result.AddFloat3ColorParameter("edge_fade_center_tint");
                    result.AddFloatParameter("edge_fade_power");
                    break;
            }

            switch (blend_mode)
            {
                case Blend_Mode.Opaque:
                    break;
                case Blend_Mode.Alpha_Blend:
                    break;
            }

            return result;
        }

        public ShaderParameters GetVertexShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;
            var result = new ShaderParameters();

            switch (albedo)
            {
                case Albedo.Fur_Multilayer:
                    break;
            }

            switch (warp)
            {
                case Warp.None:
                    break;
                case Warp.From_Texture:
                    break;
                case Warp.Parallax_Simple:
                    break;
            }

            switch (overlay)
            {
                case Overlay.None:
                    break;
                case Overlay.Additive:
                    break;
                case Overlay.Additive_Detail:
                    break;
                case Overlay.Multiply:
                    break;
                case Overlay.Multiply_And_Additive_Detail:
                    break;
            }

            switch (edge_fade)
            {
                case Edge_Fade.None:
                    break;
                case Edge_Fade.Simple:
                    break;
            }

            switch (blend_mode)
            {
                case Blend_Mode.Opaque:
                    break;
                case Blend_Mode.Alpha_Blend:
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

            if (methodName == "albedo")
            {
                optionName = ((Albedo)option).ToString();

                switch ((Albedo)option)
                {
                    case Albedo.Fur_Multilayer:
                        result.AddSamplerWithoutXFormParameter("fur_hairs_map");
                        result.AddSamplerWithoutXFormParameter("fur_tint_map");
                        result.AddFloat3ColorParameter("fur_deep_color");
                        result.AddFloat3ColorParameter("fur_tint_color");
                        result.AddFloatParameter("fur_intensity");
                        result.AddFloatParameter("fur_alpha_scale");
                        result.AddFloatParameter("fur_shear_x");
                        result.AddFloatParameter("fur_shear_y");
                        result.AddFloatParameter("fur_fix");
                        result.AddFloatParameter("layer_depth");
                        result.AddIntegerParameter("layers_of_4");
                        result.AddFloatParameter("texcoord_aspect_ratio");
                        result.AddFloatParameter("depth_darken");
                        rmopName = @"shaders\fur_options\fur_multilayer";
                        break;
                }
            }

            if (methodName == "warp")
            {
                optionName = ((Warp)option).ToString();

                switch ((Warp)option)
                {
                    case Warp.None:
                        break;
                    case Warp.From_Texture:
                        result.AddSamplerWithoutXFormParameter("warp_map");
                        result.AddFloatParameter("warp_amount_x");
                        result.AddFloatParameter("warp_amount_y");
                        rmopName = @"shaders\shader_options\warp_from_texture";
                        break;
                    case Warp.Parallax_Simple:
                        break;
                }
            }

            if (methodName == "overlay")
            {
                optionName = ((Overlay)option).ToString();

                switch ((Overlay)option)
                {
                    case Overlay.None:
                        break;
                    case Overlay.Additive:
                        result.AddSamplerWithoutXFormParameter("overlay_map");
                        result.AddFloat3ColorParameter("overlay_tint");
                        result.AddFloatParameter("overlay_intensity");
                        rmopName = @"shaders\shader_options\overlay_additive";
                        break;
                    case Overlay.Additive_Detail:
                        result.AddSamplerWithoutXFormParameter("overlay_map");
                        result.AddSamplerWithoutXFormParameter("overlay_detail_map");
                        result.AddFloat3ColorParameter("overlay_tint");
                        result.AddFloatParameter("overlay_intensity");
                        rmopName = @"shaders\shader_options\overlay_additive_detail";
                        break;
                    case Overlay.Multiply:
                        result.AddSamplerWithoutXFormParameter("overlay_map");
                        result.AddFloat3ColorParameter("overlay_tint");
                        result.AddFloatParameter("overlay_intensity");
                        rmopName = @"shaders\shader_options\overlay_additive";
                        break;
                    case Overlay.Multiply_And_Additive_Detail:
                        result.AddSamplerWithoutXFormParameter("overlay_multiply_map");
                        result.AddSamplerWithoutXFormParameter("overlay_map");
                        result.AddSamplerWithoutXFormParameter("overlay_detail_map");
                        result.AddFloat3ColorParameter("overlay_tint");
                        result.AddFloatParameter("overlay_intensity");
                        rmopName = @"shaders\shader_options\overlay_multiply_additive_detail";
                        break;
                }
            }

            if (methodName == "edge_fade")
            {
                optionName = ((Edge_Fade)option).ToString();

                switch ((Edge_Fade)option)
                {
                    case Edge_Fade.None:
                        break;
                    case Edge_Fade.Simple:
                        result.AddFloat3ColorParameter("edge_fade_edge_tint");
                        result.AddFloat3ColorParameter("edge_fade_center_tint");
                        result.AddFloatParameter("edge_fade_power");
                        rmopName = @"shaders\shader_options\edge_fade_simple";
                        break;
                }
            }

            if (methodName == "blend_mode")
            {
                optionName = ((Blend_Mode)option).ToString();

                switch ((Blend_Mode)option)
                {
                    case Blend_Mode.Opaque:
                        break;
                    case Blend_Mode.Alpha_Blend:
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(FurMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((FurMethods)methodIndex)
            {
                case FurMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case FurMethods.Warp:
                    return Enum.GetValues(typeof(Warp));
                case FurMethods.Overlay:
                    return Enum.GetValues(typeof(Overlay));
                case FurMethods.Edge_Fade:
                    return Enum.GetValues(typeof(Edge_Fade));
                case FurMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
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

            if (methodName == "albedo")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_albedo_ps";
            }

            if (methodName == "warp")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_parallax_ps";
            }

            if (methodName == "overlay")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_overlay_ps";
            }

            if (methodName == "edge_fade")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_edge_fade_ps";
            }

            if (methodName == "blend_mode")
            {
                vertexFunction = "invalid";
                pixelFunction = "blend_type";
            }
        }

        public void GetOptionFunctions(string methodName, int option, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "albedo")
            {
                switch ((Albedo)option)
                {
                    case Albedo.Fur_Multilayer:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_albedo_multilayer_ps";
                        break;
                }
            }

            if (methodName == "warp")
            {
                switch ((Warp)option)
                {
                    case Warp.None:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_parallax_off_ps";
                        break;
                    case Warp.From_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_warp_from_texture_ps";
                        break;
                    case Warp.Parallax_Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_parallax_simple_ps";
                        break;
                }
            }

            if (methodName == "overlay")
            {
                switch ((Overlay)option)
                {
                    case Overlay.None:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_none_ps";
                        break;
                    case Overlay.Additive:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_additive_ps";
                        break;
                    case Overlay.Additive_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_additive_detail_ps";
                        break;
                    case Overlay.Multiply:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_multiply_ps";
                        break;
                    case Overlay.Multiply_And_Additive_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_overlay_multiply_and_additive_detail_ps";
                        break;
                }
            }

            if (methodName == "edge_fade")
            {
                switch ((Edge_Fade)option)
                {
                    case Edge_Fade.None:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_edge_fade_none_ps";
                        break;
                    case Edge_Fade.Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_edge_fade_simple_ps";
                        break;
                }
            }

            if (methodName == "blend_mode")
            {
                switch ((Blend_Mode)option)
                {
                    case Blend_Mode.Opaque:
                        vertexFunction = "invalid";
                        pixelFunction = "opaque";
                        break;
                    case Blend_Mode.Alpha_Blend:
                        vertexFunction = "invalid";
                        pixelFunction = "alpha_blend";
                        break;
                }
            }
        }

        public ShaderParameters GetParameterArguments(string methodName, int option)
        {
            ShaderParameters result = new ShaderParameters();
            if (methodName == "albedo")
            {
                switch ((Albedo)option)
                {
                    case Albedo.Fur_Multilayer:
                        break;
                }
            }

            if (methodName == "warp")
            {
                switch ((Warp)option)
                {
                    case Warp.None:
                        break;
                    case Warp.From_Texture:
                        break;
                    case Warp.Parallax_Simple:
                        break;
                }
            }

            if (methodName == "overlay")
            {
                switch ((Overlay)option)
                {
                    case Overlay.None:
                        break;
                    case Overlay.Additive:
                        break;
                    case Overlay.Additive_Detail:
                        break;
                    case Overlay.Multiply:
                        break;
                    case Overlay.Multiply_And_Additive_Detail:
                        break;
                }
            }

            if (methodName == "edge_fade")
            {
                switch ((Edge_Fade)option)
                {
                    case Edge_Fade.None:
                        break;
                    case Edge_Fade.Simple:
                        break;
                }
            }

            if (methodName == "blend_mode")
            {
                switch ((Blend_Mode)option)
                {
                    case Blend_Mode.Opaque:
                        break;
                    case Blend_Mode.Alpha_Blend:
                        break;
                }
            }
            return result;
        }
    }
}
