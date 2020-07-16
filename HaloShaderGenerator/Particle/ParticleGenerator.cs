using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;

namespace HaloShaderGenerator.Particle
{
    public class ParticleGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;

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
        public ParticleGenerator() { TemplateGenerationValid = false; }

        /// <summary>
        /// Generator instantiation for method specific shaders.
        /// </summary>
        /// <param name="albedo"></param>
        /// <param name="blend_mode"></param>
        /// <param name="specialized_rendering"></param>
        /// <param name="lighting"></param>
        /// <param name="render_targets"></param>
        /// <param name="depth_fade"></param>
        /// <param name="black_point"></param>
        /// <param name="fog"></param>
        /// <param name="frame_blend"></param>
        /// <param name="self_illumination"></param>
        public ParticleGenerator(Albedo albedo, Blend_Mode blend_mode, Specialized_Rendering specialized_rendering, Lighting lighting, Render_Targets render_targets,
            Depth_Fade depth_fade, Black_Point black_point, Fog fog, Frame_Blend frame_blend, Self_Illumination self_illumination)
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

            TemplateGenerationValid = true;
        }

        public ShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderType>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Albedo>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Blend_Mode>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Specialized_Rendering>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Lighting>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Render_Targets>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Depth_Fade>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Black_Point>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Fog>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Frame_Blend>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Self_Illumination>());

            //
            // The following code properly names the macros (like in rmdf)
            //

            macros.Add(ShaderGeneratorBase.CreateMacro("particle_albedo", albedo, "albedo_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("particle_blend_mode", blend_mode, "blend_mode_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("particle_specialized_rendering", specialized_rendering, "specialized_rendering_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("particle_lighting", lighting, "lighting_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("particle_render_targets", render_targets, "render_targets_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("particle_depth_fade", depth_fade, "depth_fade_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("particle_black_point", black_point, "black_point_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("particle_fog", fog, "fog_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("particle_frame_blend", frame_blend, "frame_blend_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("particle_self_illumination", self_illumination, "self_illumination_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("shaderstage", entryPoint, "k_shaderstage_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("shadertype", entryPoint, "shadertype_"));

            //macros.Add(ShaderGeneratorBase.CreateMacro("albedo_arg", albedo, "k_albedo_"));
            //macros.Add(ShaderGeneratorBase.CreateMacro("blend_type_arg", blend_mode, "k_blend_mode_"));
            //macros.Add(ShaderGeneratorBase.CreateMacro("specialized_rendering_arg", specialized_rendering, "k_specialized_rendering_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("lighting_arg", lighting, "k_lighting_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("render_targets_arg", render_targets, "k_render_targets_"));
            //macros.Add(ShaderGeneratorBase.CreateMacro("depth_fade_arg", depth_fade, "k_depth_fade_"));
            //macros.Add(ShaderGeneratorBase.CreateMacro("black_point_arg", black_point, "k_black_point_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("fog_arg", fog, "k_fog_"));
            //macros.Add(ShaderGeneratorBase.CreateMacro("frame_blend_arg", frame_blend, "k_frame_blend_"));
            //macros.Add(ShaderGeneratorBase.CreateMacro("self_illumination_arg", self_illumination, "k_self_illumination_"));


            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"pixl_particle.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderType>());

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"glps_particle.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_vertex_transform", vertexType, "calc_vertex_transform_", ""));
            macros.Add(ShaderGeneratorBase.CreateMacro("transform_unknown_vector", vertexType, "transform_unknown_vector_", ""));
            macros.Add(ShaderGeneratorBase.CreateVertexMacro("input_vertex_format", vertexType));

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource(@"glvs_particle.hlsl", macros, $"entry_{entryPoint.ToString().ToLower()}", "vs_3_0");

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
                    result.AddSamplerParameter("palette");
                    break;
                case Albedo.Palettized_Plus_Billboard_Alpha:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("palette");
                    result.AddSamplerParameter("alpha_map");
                    break;
                case Albedo.Diffuse_Plus_Sprite_Alpha:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("alpha_map");
                    break;
                case Albedo.Palettized_Plus_Sprite_Alpha:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("palette");
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
            switch (frame_blend)
            {
                case Frame_Blend.On:
                    result.AddFloatParameter("starting_uv_scale");
                    result.AddFloatParameter("ending_uv_scale");
                    break;
            }
            switch (self_illumination)
            {
                case Self_Illumination.Constant_Color:
                    result.AddFloat4Parameter("self_illum_color");
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
            result.AddSamplerWithoutXFormParameter("depth_buffer", RenderMethodExtern.texture_global_target_z);
            result.AddFloat4Parameter("screen_constants", RenderMethodExtern.screen_constants);
            return result;
        }
    }
}
