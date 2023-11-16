using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.LegacyGenerator.Generator;
using HaloShaderGenerator.Globals;

namespace HaloShaderGenerator.LegacyGenerator.Particle
{
    public class LegacyParticleGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;
        private bool ApplyFixes;

        Albedo albedo;
        Blend_Mode blend_mode;
        Specialized_Rendering specialized_rendering;
        Lighting lighting;
        Render_Targets render_targets;
        Depth_Fade depth_fade;
        Black_Point black_point;
        Fog fog;
        Frame_Blend frame_blend;
        Self_Illumination self_illumination;

        /// <summary>
        /// Generator insantiation for shared shaders. Does not require method options.
        /// </summary>
        public LegacyParticleGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        /// <summary>
        /// Generator instantiation for method specific shaders.
        /// </summary>
        public LegacyParticleGenerator(Albedo albedo, Blend_Mode blend_mode, Specialized_Rendering specialized_rendering, Lighting lighting, Render_Targets render_targets,
            Depth_Fade depth_fade, Black_Point black_point, Fog fog, Frame_Blend frame_blend, Self_Illumination self_illumination, bool applyFixes = false)
        {
            this.albedo = albedo;
            this.blend_mode = blend_mode;
            this.specialized_rendering = specialized_rendering;
            this.lighting = lighting;
            this.render_targets = render_targets;
            this.depth_fade = depth_fade;
            this.black_point = black_point;
            this.fog = fog;
            this.frame_blend = frame_blend;
            this.self_illumination = self_illumination;

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public LegacyParticleGenerator(byte[] options, bool applyFixes = false)
        {
            options = ValidateOptions(options);

            this.albedo = (Albedo)options[0];
            this.blend_mode = (Blend_Mode)options[1];
            this.specialized_rendering = (Specialized_Rendering)options[2];
            this.lighting = (Lighting)options[3];
            this.render_targets = (Render_Targets)options[4];
            this.depth_fade = (Depth_Fade)options[5];
            this.black_point = (Black_Point)options[6];
            this.fog = (Fog)options[7];
            this.frame_blend = (Frame_Blend)options[8];
            this.self_illumination = (Self_Illumination)options[9];

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public LegacyShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.ShaderType>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Albedo>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Blend_Mode>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<Specialized_Rendering>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<Lighting>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<Render_Targets>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Depth_Fade>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Black_Point>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Fog>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<Frame_Blend>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Self_Illumination>());

            //
            // Convert to shared enum
            //

            var sAlbedo = Enum.Parse(typeof(Shared.Albedo), albedo.ToString());
            var sBlendMode = Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());
            var sSelfIllumination = Enum.Parse(typeof(Shared.Self_Illumination), self_illumination.ToString());
            var sDepthFade = Enum.Parse(typeof(Shared.Depth_Fade), depth_fade.ToString());
            var sBlackPoint = Enum.Parse(typeof(Shared.Black_Point), black_point.ToString());
            var sFog = Enum.Parse(typeof(Shared.Fog), fog.ToString());

            //
            // The following code properly names the macros (like in rmdf)
            //

            macros.Add(LegacyShaderGeneratorBase.CreateMacro("calc_albedo_ps", sAlbedo, "calc_albedo_", "_ps"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("blend_type", sBlendMode, "blend_type_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("particle_specialized_rendering", specialized_rendering, "specialized_rendering_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("particle_lighting", lighting, "lighting_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("particle_render_targets", render_targets, "render_targets_"));
            //macros.Add(LegacyShaderGeneratorBase.CreateMacro("calc_depth_fade", sDepthFade, "calc_depth_fade_"));
            //macros.Add(LegacyShaderGeneratorBase.CreateMacro("calc_black_point", sBlackPoint, "calc_black_point_"));
            //macros.Add(LegacyShaderGeneratorBase.CreateMacro("calc_fog", sFog, "calc_fog_"));
            //macros.Add(LegacyShaderGeneratorBase.CreateMacro("calc_frame_blend", frame_blend, "calc_frame_blend_"));
            //macros.Add(LegacyShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", sSelfIllumination, "calc_self_illumination_"));

            macros.Add(LegacyShaderGeneratorBase.CreateMacro("shaderstage", entryPoint, "k_shaderstage_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("shadertype", Shared.ShaderType.Particle, "k_shadertype_"));

            macros.Add(LegacyShaderGeneratorBase.CreateMacro("albedo_arg", sAlbedo, "k_albedo_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("blend_type_arg", sBlendMode, "k_blend_mode_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("specialized_rendering_arg", specialized_rendering, "k_specialized_rendering_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("lighting_arg", lighting, "k_lighting_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("render_targets_arg", render_targets, "k_render_targets_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("depth_fade_arg", sDepthFade, "k_depth_fade_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("black_point_arg", sBlackPoint, "k_black_point_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("fog_arg", sFog, "k_fog_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("frame_blend_arg", frame_blend, "k_frame_blend_"));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("self_illumination_arg", sSelfIllumination, "k_self_illumination_"));

            macros.Add(LegacyShaderGeneratorBase.CreateMacro("APPLY_HLSL_FIXES", ApplyFixes ? 1 : 0));

            byte[] shaderBytecode = LegacyShaderGeneratorBase.GenerateSource($"pixl_particle.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new LegacyShaderGeneratorResult(shaderBytecode);
        }

        public LegacyShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(LegacyShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderType>());

            byte[] shaderBytecode = LegacyShaderGeneratorBase.GenerateSource($"glps_particle.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new LegacyShaderGeneratorResult(shaderBytecode);
        }

        public LegacyShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("calc_vertex_transform", vertexType, "calc_vertex_transform_", ""));
            macros.Add(LegacyShaderGeneratorBase.CreateMacro("transform_unknown_vector", vertexType, "transform_unknown_vector_", ""));
            macros.Add(LegacyShaderGeneratorBase.CreateVertexMacro("input_vertex_format", vertexType));

            byte[] shaderBytecode = LegacyShaderGeneratorBase.GenerateSource(@"glvs_particle.hlsl", macros, $"entry_{entryPoint.ToString().ToLower()}", "vs_3_0");

            return new LegacyShaderGeneratorResult(shaderBytecode);
        }

        public LegacyShaderGeneratorResult GenerateVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");
            return null;
        }

        public int GetMethodCount()
        {
            return System.Enum.GetValues(typeof(ParticleMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((ParticleMethods)methodIndex)
            {
                case ParticleMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case ParticleMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
                case ParticleMethods.Specialized_Rendering:
                    return Enum.GetValues(typeof(Specialized_Rendering)).Length;
                case ParticleMethods.Lighting:
                    return Enum.GetValues(typeof(Lighting)).Length;
                case ParticleMethods.Render_Targets:
                    return Enum.GetValues(typeof(Render_Targets)).Length;
                case ParticleMethods.Depth_Fade:
                    return Enum.GetValues(typeof(Depth_Fade)).Length;
                case ParticleMethods.Black_Point:
                    return Enum.GetValues(typeof(Black_Point)).Length;
                case ParticleMethods.Fog:
                    return Enum.GetValues(typeof(Fog)).Length;
                case ParticleMethods.Frame_Blend:
                    return Enum.GetValues(typeof(Frame_Blend)).Length;
                case ParticleMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination)).Length;
            }

            return -1;
        }

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((ParticleMethods)methodIndex)
            {
                case ParticleMethods.Albedo:
                    return (int)albedo;
                case ParticleMethods.Blend_Mode:
                    return (int)blend_mode;
                case ParticleMethods.Specialized_Rendering:
                    return (int)specialized_rendering;
                case ParticleMethods.Lighting:
                    return (int)lighting;
                case ParticleMethods.Render_Targets:
                    return (int)render_targets;
                case ParticleMethods.Depth_Fade:
                    return (int)depth_fade;
                case ParticleMethods.Black_Point:
                    return (int)black_point;
                case ParticleMethods.Fog:
                    return (int)fog;
                case ParticleMethods.Frame_Blend:
                    return (int)frame_blend;
                case ParticleMethods.Self_Illumination:
                    return (int)self_illumination;
            }
            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            if (entryPoint == ShaderStage.Default)
                return true;
            return false;
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
            return false;
        }

        public bool IsPixelShaderShared(ShaderStage entryPoint)
        {
            return false;
        }

        public bool IsVertexFormatSupported(VertexType vertexType)
        {
            switch (vertexType)
            {
                case VertexType.Particle:
                case VertexType.ParticleModel:
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

            switch (albedo)
            {
                case Albedo.Diffuse_Only:
                    result.AddSamplerParameter("base_map");
                    break;
                case Albedo.Diffuse_Plus_Billboard_Alpha:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("alpha_map");
                    break;
                case Albedo.Palettized:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("palette");
                    break;
                case Albedo.Palettized_Plus_Billboard_Alpha:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerParameter("alpha_map");
                    break;
                case Albedo.Diffuse_Plus_Sprite_Alpha:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("alpha_map");
                    break;
                case Albedo.Palettized_Plus_Sprite_Alpha:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerParameter("alpha_map");
                    break;
                case Albedo.Diffuse_Modulated:
                    result.AddSamplerParameter("base_map");
                    result.AddFloat4Parameter("tint_color");
                    result.AddFloatParameter("modulation_factor");
                    break;
                case Albedo.Palettized_Plasma:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("base_map2");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerParameter("alpha_map");
                    result.AddFloatParameter("alpha_modulation_factor");
                    break;
                case Albedo.Palettized_2d_Plasma:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("base_map2");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerParameter("alpha_map");
                    break;
            }
            switch (specialized_rendering)
            {
                case Specialized_Rendering.Distortion:
                case Specialized_Rendering.Distortion_Expensive:
                case Specialized_Rendering.Distortion_Diffuse:
                case Specialized_Rendering.Distortion_Expensive_Diffuse:
                    result.AddFloatParameter("distortion_scale");
                    break;
            }
            switch (depth_fade)
            {
                case Depth_Fade.On:
                    result.AddFloatParameter("depth_fade_range");
                    break;
            }

            return result;
        }

        public ShaderParameters GetVertexShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;
            var result = new ShaderParameters();

            result.AddPrefixedFloat4VertexParameter("albedo", "category_");
            result.AddPrefixedFloat4VertexParameter("blend_mode", "category_");
            result.AddPrefixedFloat4VertexParameter("specialized_rendering", "category_");
            result.AddPrefixedFloat4VertexParameter("lighting", "category_");
            result.AddPrefixedFloat4VertexParameter("fog", "category_");
            switch (frame_blend)
            {
                case Frame_Blend.On:
                    result.AddFloatVertexParameter("starting_uv_scale");
                    result.AddFloatVertexParameter("ending_uv_scale");
                    break;
            }
            result.AddPrefixedFloat4VertexParameter("frame_blend", "category_");
            switch (self_illumination)
            {
                case Self_Illumination.Constant_Color:
                    result.AddFloat4VertexParameter("self_illum_color");
                    break;
            }
            result.AddPrefixedFloat4VertexParameter("self_illumination", "category_");

            return result;
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();
            result.AddSamplerWithoutXFormParameter("depth_buffer", RenderMethodExtern.texture_global_target_z);
            result.AddFloat4Parameter("screen_constants", RenderMethodExtern.screen_constants);
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
                    case Albedo.Diffuse_Only:
                        result.AddSamplerParameter("base_map");
                        rmopName = @"shaders\particle_options\albedo_diffuse_only";
                        break;
                    case Albedo.Diffuse_Plus_Billboard_Alpha:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("alpha_map");
                        rmopName = @"shaders\particle_options\albedo_diffuse_plus_billboard_alpha";
                        break;
                    case Albedo.Palettized:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("palette");
                        rmopName = @"shaders\particle_options\albedo_palettized";
                        break;
                    case Albedo.Palettized_Plus_Billboard_Alpha:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddSamplerParameter("alpha_map");
                        rmopName = @"shaders\particle_options\albedo_palettized_plus_billboard_alpha";
                        break;
                    case Albedo.Diffuse_Plus_Sprite_Alpha:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("alpha_map");
                        rmopName = @"shaders\particle_options\albedo_diffuse_plus_sprite_alpha";
                        break;
                    case Albedo.Palettized_Plus_Sprite_Alpha:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddSamplerParameter("alpha_map");
                        rmopName = @"shaders\particle_options\albedo_palettized_plus_sprite_alpha";
                        break;
                    case Albedo.Diffuse_Modulated:
                        result.AddSamplerParameter("base_map");
                        result.AddFloat4Parameter("tint_color");
                        result.AddFloatParameter("modulation_factor");
                        rmopName = @"shaders\particle_options\albedo_diffuse_modulated";
                        break;
                    case Albedo.Palettized_Glow:
                        result.AddSamplerParameter("base_map");
                        result.AddFloat4Parameter("tint_color");
                        rmopName = @"shaders\particle_options\albedo_palettized_glow";
                        break;
                    case Albedo.Palettized_Plasma:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("base_map2");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddSamplerParameter("alpha_map");
                        result.AddFloatParameter("alpha_modulation_factor");
                        rmopName = @"shaders\particle_options\albedo_palettized_plasma";
                        break;
                    case Albedo.Palettized_2d_Plasma:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("base_map2");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddSamplerParameter("alpha_map");
                        rmopName = @"shaders\particle_options\albedo_palettized_plasma";
                        break;
                }
            }
            if (methodName == "blend_mode")
            {
                optionName = ((Blend_Mode)option).ToString();
            }
            if (methodName == "specialized_rendering")
            {
                optionName = ((Specialized_Rendering)option).ToString();
                switch ((Specialized_Rendering)option)
                {
                    case Specialized_Rendering.Distortion:
                    case Specialized_Rendering.Distortion_Expensive:
                    case Specialized_Rendering.Distortion_Diffuse:
                    case Specialized_Rendering.Distortion_Expensive_Diffuse:
                        result.AddFloatParameter("distortion_scale");
                        rmopName = @"shaders\particle_options\distortion_diffuse";
                        break;
                }
            }
            if (methodName == "lighting")
            {
                optionName = ((Lighting)option).ToString();
            }
            if (methodName == "render_targets")
            {
                optionName = ((Render_Targets)option).ToString();
            }
            if (methodName == "depth_fade")
            {
                optionName = ((Depth_Fade)option).ToString();
                switch ((Depth_Fade)option)
                {
                    case Depth_Fade.On:
                        result.AddFloatParameter("depth_fade_range");
                        rmopName = @"shaders\particle_options\depth_fade_on";
                        break;
                }
            }
            if (methodName == "black_point")
            {
                optionName = ((Black_Point)option).ToString();
            }
            if (methodName == "fog")
            {
                optionName = ((Fog)option).ToString();
            }
            if (methodName == "frame_blend")
            {
                optionName = ((Frame_Blend)option).ToString();
                switch ((Frame_Blend)option)
                {
                    case Frame_Blend.On:
                        result.AddFloatVertexParameter("starting_uv_scale");
                        result.AddFloatVertexParameter("ending_uv_scale");
                        rmopName = @"shaders\particle_options\frame_blend_on";
                        break;
                }
            }
            if (methodName == "self_illumination")
            {
                optionName = ((Self_Illumination)option).ToString();
                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.Constant_Color:
                        result.AddFloat4VertexParameter("self_illum_color");
                        rmopName = @"shaders\particle_options\self_illumination_constant_color";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(ParticleMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((ParticleMethods)methodIndex)
            {
                case ParticleMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case ParticleMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
                case ParticleMethods.Specialized_Rendering:
                    return Enum.GetValues(typeof(Specialized_Rendering));
                case ParticleMethods.Lighting:
                    return Enum.GetValues(typeof(Lighting));
                case ParticleMethods.Render_Targets:
                    return Enum.GetValues(typeof(Render_Targets));
                case ParticleMethods.Depth_Fade:
                    return Enum.GetValues(typeof(Depth_Fade));
                case ParticleMethods.Black_Point:
                    return Enum.GetValues(typeof(Black_Point));
                case ParticleMethods.Fog:
                    return Enum.GetValues(typeof(Fog));
                case ParticleMethods.Frame_Blend:
                    return Enum.GetValues(typeof(Frame_Blend));
                case ParticleMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination));
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
    }
}
