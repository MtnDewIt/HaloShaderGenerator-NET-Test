using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Particle
{
    public class ParticleGenerator : IShaderGenerator
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
        Warp warp;

        public ParticleGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        public ParticleGenerator(Albedo albedo, Blend_Mode blend_mode, Specialized_Rendering specialized_rendering, Lighting lighting, Render_Targets render_targets, Depth_Fade depth_fade, Black_Point black_point, Fog fog, Frame_Blend frame_blend, Self_Illumination self_illumination, Warp warp, bool applyFixes = false)
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
            this.warp = warp;

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public ParticleGenerator(byte[] options, bool applyFixes = false)
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
            this.warp = (Warp)options[10];

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public ShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Particle, entryPoint, sBlendMode,
                Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes);

            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Albedo>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Blend_Mode>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Specialized_Rendering>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Lighting>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Render_Targets>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Depth_Fade>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Black_Point>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Fog>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Frame_Blend>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Self_Illumination>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Warp>());

            macros.Add(ShaderGeneratorBase.CreateAutoMacro("albedo", albedo.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("blend_mode", blend_mode.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("specialized_rendering", specialized_rendering.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("lighting", lighting.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("render_targets", render_targets.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("depth_fade", depth_fade.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("black_point", black_point.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("fog", fog.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("frame_blend", frame_blend.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("self_illumination", self_illumination.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("warp", warp.ToString().ToLower()));

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Particle);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            if (!IsEntryPointSupported(entryPoint) || !IsPixelShaderShared(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Particle, entryPoint, sBlendMode,
                Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes);

            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Albedo>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Blend_Mode>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Specialized_Rendering>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Lighting>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Render_Targets>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Depth_Fade>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Black_Point>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Fog>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Frame_Blend>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Self_Illumination>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Warp>());

            macros.Add(ShaderGeneratorBase.CreateAutoMacro("albedo", albedo.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("blend_mode", blend_mode.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("specialized_rendering", specialized_rendering.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("lighting", lighting.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("render_targets", render_targets.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("depth_fade", depth_fade.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("black_point", black_point.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("fog", fog.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("frame_blend", frame_blend.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("self_illumination", self_illumination.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("warp", warp.ToString().ToLower()));

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Particle);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "ps_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!IsVertexFormatSupported(vertexType) || !IsEntryPointSupported(entryPoint))
                return null;

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Particle, entryPoint,
                sBlendMode, Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes, true, vertexType);

            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Albedo>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Blend_Mode>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Specialized_Rendering>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Lighting>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Render_Targets>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Depth_Fade>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Black_Point>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Fog>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Frame_Blend>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Self_Illumination>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Warp>());

            macros.Add(ShaderGeneratorBase.CreateAutoMacro("albedo", albedo.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("blend_mode", blend_mode.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("specialized_rendering", specialized_rendering.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("lighting", lighting.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("render_targets", render_targets.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("depth_fade", depth_fade.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("black_point", black_point.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("fog", fog.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("frame_blend", frame_blend.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("self_illumination", self_illumination.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("warp", warp.ToString().ToLower()));

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Particle);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public ShaderGeneratorResult GenerateVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            Shared.Blend_Mode sBlendMode = (Shared.Blend_Mode)Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            TemplateGenerator.TemplateGenerator.CreateGlobalMacros(macros, Globals.ShaderType.Particle, entryPoint,
                sBlendMode, Shader.Misc.First_Person_Never, Shared.Alpha_Test.None, Shared.Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel, ApplyFixes, true, vertexType);

            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Albedo>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Blend_Mode>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Specialized_Rendering>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Lighting>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Render_Targets>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Depth_Fade>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Black_Point>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Fog>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Frame_Blend>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Self_Illumination>());
            macros.AddRange(ShaderGeneratorBase.CreateAutoMacroMethodEnumDefinitions<Warp>());

            macros.Add(ShaderGeneratorBase.CreateAutoMacro("albedo", albedo.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("blend_mode", blend_mode.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("specialized_rendering", specialized_rendering.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("lighting", lighting.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("render_targets", render_targets.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("depth_fade", depth_fade.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("black_point", black_point.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("fog", fog.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("frame_blend", frame_blend.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("self_illumination", self_illumination.ToString().ToLower()));
            macros.Add(ShaderGeneratorBase.CreateAutoMacro("warp", warp.ToString().ToLower()));

            string entryName = TemplateGenerator.TemplateGenerator.GetEntryName(entryPoint, true);
            string filename = TemplateGenerator.TemplateGenerator.GetSourceFilename(Globals.ShaderType.Particle);
            byte[] bytecode = ShaderGeneratorBase.GenerateSource(filename, macros, entryName, "vs_3_0");

            return new ShaderGeneratorResult(bytecode);
        }

        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(ParticleMethods)).Length;
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
                case ParticleMethods.Warp:
                    return Enum.GetValues(typeof(Warp)).Length;
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
                case ParticleMethods.Warp:
                    return (int)warp;
            }

            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Default:
                case ShaderStage.Static_Default:
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
                case VertexType.Particle:
                case VertexType.ParticleModel:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsVertexShaderShared(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Default:
                case ShaderStage.Static_Default:
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
                case Albedo.Diffuse_Only:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    break;
                case Albedo.Diffuse_Plus_Billboard_Alpha:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    break;
                case Albedo.Palettized:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("palette");
                    break;
                case Albedo.Palettized_Plus_Billboard_Alpha:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    break;
                case Albedo.Diffuse_Plus_Sprite_Alpha:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    break;
                case Albedo.Palettized_Plus_Sprite_Alpha:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    break;
                case Albedo.Diffuse_Modulated:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddFloat3ColorParameter("tint_color");
                    result.AddFloatParameter("modulation_factor");
                    break;
                case Albedo.Palettized_Glow:
                    result.AddSamplerWithoutXFormParameter("base_map");
                    result.AddFloat3ColorParameter("tint_color");
                    break;
                case Albedo.Palettized_Plasma:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("base_map2");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    result.AddFloatParameter("alpha_modulation_factor");
                    break;
                case Albedo.Palettized_2d_Plasma:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("base_map2");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerWithoutXFormParameter("alpha_map");
                    result.AddFloatParameter("alpha_modulation_factor");
                    break;
            }

            switch (blend_mode)
            {
                case Blend_Mode.Opaque:
                    break;
                case Blend_Mode.Additive:
                    break;
                case Blend_Mode.Multiply:
                    break;
                case Blend_Mode.Alpha_Blend:
                    break;
                case Blend_Mode.Double_Multiply:
                    break;
                case Blend_Mode.Maximum:
                    break;
                case Blend_Mode.Multiply_Add:
                    break;
                case Blend_Mode.Add_Src_Times_Dstalpha:
                    break;
                case Blend_Mode.Add_Src_Times_Srcalpha:
                    break;
                case Blend_Mode.Inv_Alpha_Blend:
                    break;
                case Blend_Mode.Pre_Multiplied_Alpha:
                    break;
            }

            switch (specialized_rendering)
            {
                case Specialized_Rendering.None:
                    break;
                case Specialized_Rendering.Distortion:
                    result.AddFloatParameter("distortion_scale");
                    break;
                case Specialized_Rendering.Distortion_Expensive:
                    result.AddFloatParameter("distortion_scale");
                    break;
                case Specialized_Rendering.Distortion_Diffuse:
                    result.AddFloatParameter("distortion_scale");
                    break;
                case Specialized_Rendering.Distortion_Expensive_Diffuse:
                    result.AddFloatParameter("distortion_scale");
                    break;
            }

            switch (lighting)
            {
                case Lighting.None:
                    break;
                case Lighting.Per_Pixel_Ravi_Order_3:
                    break;
                case Lighting.Per_Vertex_Ravi_Order_0:
                    break;
                case Lighting.Per_Pixel_Smooth:
                    result.AddFloatParameter("contrast_scale");
                    result.AddFloatParameter("contrast_offset");
                    break;
                case Lighting.Per_Vertex_Ambient:
                    break;
                case Lighting.Smoke_Lighting:
                    result.AddFloatParameter("bump_contrast");
                    result.AddFloatParameter("bump_randomness");
                    break;
            }

            switch (render_targets)
            {
                case Render_Targets.Ldr_And_Hdr:
                    break;
                case Render_Targets.Ldr_Only:
                    break;
            }

            switch (depth_fade)
            {
                case Depth_Fade.Off:
                    break;
                case Depth_Fade.On:
                    result.AddFloatParameter("depth_fade_range");
                    break;
                case Depth_Fade.Low_Res:
                    result.AddFloatParameter("depth_fade_range");
                    break;
                case Depth_Fade.Palette_Shift:
                    result.AddFloatParameter("depth_fade_range");
                    result.AddFloatParameter("palette_shift_amount");
                    break;
            }

            switch (black_point)
            {
                case Black_Point.Off:
                    break;
                case Black_Point.On:
                    break;
            }

            switch (fog)
            {
                case Fog.Off:
                    break;
                case Fog.On:
                    break;
            }

            switch (frame_blend)
            {
                case Frame_Blend.Off:
                    break;
                case Frame_Blend.On:
                    result.AddFloatParameter("starting_uv_scale");
                    result.AddFloatParameter("ending_uv_scale");
                    break;
            }

            switch (self_illumination)
            {
                case Self_Illumination.None:
                    break;
                case Self_Illumination.Constant_Color:
                    result.AddFloat3ColorParameter("self_illum_color");
                    break;
            }

            switch (warp)
            {
                case Warp.None:
                    break;
                case Warp.Sphere:
                    result.AddFloatParameter("sphere_warp_scale");
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
            result.AddPrefixedFloat4VertexParameter("lighting", "category_");
            result.AddPrefixedFloat4VertexParameter("fog", "category_");
            result.AddPrefixedFloat4VertexParameter("specialized_rendering", "category_");
            result.AddPrefixedFloat4VertexParameter("frame_blend", "category_");
            result.AddPrefixedFloat4VertexParameter("self_illumination", "category_");

            switch (albedo)
            {
                case Albedo.Diffuse_Only:
                    break;
                case Albedo.Diffuse_Plus_Billboard_Alpha:
                    break;
                case Albedo.Palettized:
                    break;
                case Albedo.Palettized_Plus_Billboard_Alpha:
                    break;
                case Albedo.Diffuse_Plus_Sprite_Alpha:
                    break;
                case Albedo.Palettized_Plus_Sprite_Alpha:
                    break;
                case Albedo.Diffuse_Modulated:
                    break;
                case Albedo.Palettized_Glow:
                    break;
                case Albedo.Palettized_Plasma:
                    break;
                case Albedo.Palettized_2d_Plasma:
                    break;
            }

            switch (blend_mode)
            {
                case Blend_Mode.Opaque:
                    break;
                case Blend_Mode.Additive:
                    break;
                case Blend_Mode.Multiply:
                    break;
                case Blend_Mode.Alpha_Blend:
                    break;
                case Blend_Mode.Double_Multiply:
                    break;
                case Blend_Mode.Maximum:
                    break;
                case Blend_Mode.Multiply_Add:
                    break;
                case Blend_Mode.Add_Src_Times_Dstalpha:
                    break;
                case Blend_Mode.Add_Src_Times_Srcalpha:
                    break;
                case Blend_Mode.Inv_Alpha_Blend:
                    break;
                case Blend_Mode.Pre_Multiplied_Alpha:
                    break;
            }

            switch (specialized_rendering)
            {
                case Specialized_Rendering.None:
                    break;
                case Specialized_Rendering.Distortion:
                    break;
                case Specialized_Rendering.Distortion_Expensive:
                    break;
                case Specialized_Rendering.Distortion_Diffuse:
                    break;
                case Specialized_Rendering.Distortion_Expensive_Diffuse:
                    break;
            }

            switch (lighting)
            {
                case Lighting.None:
                    break;
                case Lighting.Per_Pixel_Ravi_Order_3:
                    break;
                case Lighting.Per_Vertex_Ravi_Order_0:
                    break;
                case Lighting.Per_Pixel_Smooth:
                    break;
                case Lighting.Per_Vertex_Ambient:
                    break;
                case Lighting.Smoke_Lighting:
                    break;
            }

            switch (render_targets)
            {
                case Render_Targets.Ldr_And_Hdr:
                    break;
                case Render_Targets.Ldr_Only:
                    break;
            }

            switch (depth_fade)
            {
                case Depth_Fade.Off:
                    break;
                case Depth_Fade.On:
                    break;
                case Depth_Fade.Low_Res:
                    break;
                case Depth_Fade.Palette_Shift:
                    break;
            }

            switch (black_point)
            {
                case Black_Point.Off:
                    break;
                case Black_Point.On:
                    break;
            }

            switch (fog)
            {
                case Fog.Off:
                    break;
                case Fog.On:
                    break;
            }

            switch (frame_blend)
            {
                case Frame_Blend.Off:
                    break;
                case Frame_Blend.On:
                    result.AddFloatVertexParameter("starting_uv_scale");
                    result.AddFloatVertexParameter("ending_uv_scale");
                    break;
            }

            switch (self_illumination)
            {
                case Self_Illumination.None:
                    break;
                case Self_Illumination.Constant_Color:
                    result.AddFloat3ColorVertexParameter("self_illum_color");
                    break;
            }

            switch (warp)
            {
                case Warp.None:
                    break;
                case Warp.Sphere:
                    break;
            }

            return result;
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();
            result.AddSamplerWithoutXFormParameter("depth_buffer", RenderMethodExtern.texture_global_target_z);
            result.AddFloat3ColorParameter("screen_constants", RenderMethodExtern.screen_constants);
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
                        result.AddSamplerWithoutXFormParameter("base_map");
                        rmopName = @"shaders\particle_options\albedo_diffuse_only";
                        break;
                    case Albedo.Diffuse_Plus_Billboard_Alpha:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("alpha_map");
                        rmopName = @"shaders\particle_options\albedo_diffuse_plus_billboard_alpha";
                        break;
                    case Albedo.Palettized:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("palette");
                        rmopName = @"shaders\particle_options\albedo_palettized";
                        break;
                    case Albedo.Palettized_Plus_Billboard_Alpha:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddSamplerWithoutXFormParameter("alpha_map");
                        rmopName = @"shaders\particle_options\albedo_palettized_plus_billboard_alpha";
                        break;
                    case Albedo.Diffuse_Plus_Sprite_Alpha:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("alpha_map");
                        rmopName = @"shaders\particle_options\albedo_diffuse_plus_sprite_alpha";
                        break;
                    case Albedo.Palettized_Plus_Sprite_Alpha:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddSamplerWithoutXFormParameter("alpha_map");
                        rmopName = @"shaders\particle_options\albedo_palettized_plus_sprite_alpha";
                        break;
                    case Albedo.Diffuse_Modulated:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddFloat3ColorParameter("tint_color");
                        result.AddFloatParameter("modulation_factor");
                        rmopName = @"shaders\particle_options\albedo_diffuse_modulated";
                        break;
                    case Albedo.Palettized_Glow:
                        result.AddSamplerWithoutXFormParameter("base_map");
                        result.AddFloat3ColorParameter("tint_color");
                        rmopName = @"shaders\particle_options\albedo_palettized_glow";
                        break;
                    case Albedo.Palettized_Plasma:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("base_map2");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddSamplerWithoutXFormParameter("alpha_map");
                        result.AddFloatParameter("alpha_modulation_factor");
                        rmopName = @"shaders\particle_options\albedo_palettized_plasma";
                        break;
                    case Albedo.Palettized_2d_Plasma:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("base_map2");
                        result.AddSamplerWithoutXFormParameter("palette");
                        result.AddSamplerWithoutXFormParameter("alpha_map");
                        result.AddFloatParameter("alpha_modulation_factor");
                        rmopName = @"shaders\particle_options\albedo_palettized_plasma";
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
                    case Blend_Mode.Additive:
                        break;
                    case Blend_Mode.Multiply:
                        break;
                    case Blend_Mode.Alpha_Blend:
                        break;
                    case Blend_Mode.Double_Multiply:
                        break;
                    case Blend_Mode.Maximum:
                        break;
                    case Blend_Mode.Multiply_Add:
                        break;
                    case Blend_Mode.Add_Src_Times_Dstalpha:
                        break;
                    case Blend_Mode.Add_Src_Times_Srcalpha:
                        break;
                    case Blend_Mode.Inv_Alpha_Blend:
                        break;
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        break;
                }
            }

            if (methodName == "specialized_rendering")
            {
                optionName = ((Specialized_Rendering)option).ToString();

                switch ((Specialized_Rendering)option)
                {
                    case Specialized_Rendering.None:
                        break;
                    case Specialized_Rendering.Distortion:
                        result.AddFloatParameter("distortion_scale");
                        rmopName = @"shaders\particle_options\specialized_rendering_distortion";
                        break;
                    case Specialized_Rendering.Distortion_Expensive:
                        result.AddFloatParameter("distortion_scale");
                        rmopName = @"shaders\particle_options\specialized_rendering_distortion";
                        break;
                    case Specialized_Rendering.Distortion_Diffuse:
                        result.AddFloatParameter("distortion_scale");
                        rmopName = @"shaders\particle_options\specialized_rendering_distortion";
                        break;
                    case Specialized_Rendering.Distortion_Expensive_Diffuse:
                        result.AddFloatParameter("distortion_scale");
                        rmopName = @"shaders\particle_options\specialized_rendering_distortion";
                        break;
                }
            }

            if (methodName == "lighting")
            {
                optionName = ((Lighting)option).ToString();

                switch ((Lighting)option)
                {
                    case Lighting.None:
                        break;
                    case Lighting.Per_Pixel_Ravi_Order_3:
                        break;
                    case Lighting.Per_Vertex_Ravi_Order_0:
                        break;
                    case Lighting.Per_Pixel_Smooth:
                        result.AddFloatParameter("contrast_scale");
                        result.AddFloatParameter("contrast_offset");
                        rmopName = @"shaders\particle_options\smooth_lighting";
                        break;
                    case Lighting.Per_Vertex_Ambient:
                        break;
                    case Lighting.Smoke_Lighting:
                        result.AddFloatParameter("bump_contrast");
                        result.AddFloatParameter("bump_randomness");
                        rmopName = @"shaders\particle_options\smoke_lighting";
                        break;
                }
            }

            if (methodName == "render_targets")
            {
                optionName = ((Render_Targets)option).ToString();

                switch ((Render_Targets)option)
                {
                    case Render_Targets.Ldr_And_Hdr:
                        break;
                    case Render_Targets.Ldr_Only:
                        break;
                }
            }

            if (methodName == "depth_fade")
            {
                optionName = ((Depth_Fade)option).ToString();

                switch ((Depth_Fade)option)
                {
                    case Depth_Fade.Off:
                        break;
                    case Depth_Fade.On:
                        result.AddFloatParameter("depth_fade_range");
                        rmopName = @"shaders\particle_options\depth_fade_on";
                        break;
                    case Depth_Fade.Low_Res:
                        result.AddFloatParameter("depth_fade_range");
                        rmopName = @"shaders\particle_options\depth_fade_on";
                        break;
                    case Depth_Fade.Palette_Shift:
                        result.AddFloatParameter("depth_fade_range");
                        result.AddFloatParameter("palette_shift_amount");
                        rmopName = @"shaders\particle_options\depth_fade_palette_shift";
                        break;
                }
            }

            if (methodName == "black_point")
            {
                optionName = ((Black_Point)option).ToString();

                switch ((Black_Point)option)
                {
                    case Black_Point.Off:
                        break;
                    case Black_Point.On:
                        break;
                }
            }

            if (methodName == "fog")
            {
                optionName = ((Fog)option).ToString();

                switch ((Fog)option)
                {
                    case Fog.Off:
                        break;
                    case Fog.On:
                        break;
                }
            }

            if (methodName == "frame_blend")
            {
                optionName = ((Frame_Blend)option).ToString();

                switch ((Frame_Blend)option)
                {
                    case Frame_Blend.Off:
                        break;
                    case Frame_Blend.On:
                        result.AddFloatParameter("starting_uv_scale");
                        result.AddFloatParameter("ending_uv_scale");
                        rmopName = @"shaders\particle_options\frame_blend_on";
                        break;
                }
            }

            if (methodName == "self_illumination")
            {
                optionName = ((Self_Illumination)option).ToString();

                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.None:
                        break;
                    case Self_Illumination.Constant_Color:
                        result.AddFloat3ColorParameter("self_illum_color");
                        rmopName = @"shaders\particle_options\self_illumination_constant_color";
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
                    case Warp.Sphere:
                        result.AddFloatParameter("sphere_warp_scale");
                        rmopName = @"shaders\particle_options\warp_sphere";
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
                case ParticleMethods.Warp:
                    return Enum.GetValues(typeof(Warp));
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
                pixelFunction = "invalid";
            }

            if (methodName == "blend_mode")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "specialized_rendering")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "lighting")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "render_targets")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "depth_fade")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "black_point")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "fog")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "frame_blend")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "self_illumination")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
            }

            if (methodName == "warp")
            {
                vertexFunction = "invalid";
                pixelFunction = "invalid";
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
                    case Albedo.Diffuse_Only:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Diffuse_Plus_Billboard_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Palettized:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Palettized_Plus_Billboard_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Diffuse_Plus_Sprite_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Palettized_Plus_Sprite_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Diffuse_Modulated:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Palettized_Glow:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Palettized_Plasma:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Albedo.Palettized_2d_Plasma:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "blend_mode")
            {
                switch ((Blend_Mode)option)
                {
                    case Blend_Mode.Opaque:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Additive:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Multiply:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Alpha_Blend:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Double_Multiply:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Maximum:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Multiply_Add:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Add_Src_Times_Dstalpha:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Add_Src_Times_Srcalpha:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Inv_Alpha_Blend:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "specialized_rendering")
            {
                switch ((Specialized_Rendering)option)
                {
                    case Specialized_Rendering.None:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Specialized_Rendering.Distortion:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Specialized_Rendering.Distortion_Expensive:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Specialized_Rendering.Distortion_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Specialized_Rendering.Distortion_Expensive_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "lighting")
            {
                switch ((Lighting)option)
                {
                    case Lighting.None:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Lighting.Per_Pixel_Ravi_Order_3:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Lighting.Per_Vertex_Ravi_Order_0:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Lighting.Per_Pixel_Smooth:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Lighting.Per_Vertex_Ambient:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Lighting.Smoke_Lighting:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "render_targets")
            {
                switch ((Render_Targets)option)
                {
                    case Render_Targets.Ldr_And_Hdr:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Render_Targets.Ldr_Only:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "depth_fade")
            {
                switch ((Depth_Fade)option)
                {
                    case Depth_Fade.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Depth_Fade.On:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Depth_Fade.Low_Res:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Depth_Fade.Palette_Shift:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "black_point")
            {
                switch ((Black_Point)option)
                {
                    case Black_Point.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Black_Point.On:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "fog")
            {
                switch ((Fog)option)
                {
                    case Fog.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Fog.On:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "frame_blend")
            {
                switch ((Frame_Blend)option)
                {
                    case Frame_Blend.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Frame_Blend.On:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "self_illumination")
            {
                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.None:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Self_Illumination.Constant_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                }
            }

            if (methodName == "warp")
            {
                switch ((Warp)option)
                {
                    case Warp.None:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
                        break;
                    case Warp.Sphere:
                        vertexFunction = "invalid";
                        pixelFunction = "invalid";
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
                    case Albedo.Diffuse_Only:
                        break;
                    case Albedo.Diffuse_Plus_Billboard_Alpha:
                        break;
                    case Albedo.Palettized:
                        break;
                    case Albedo.Palettized_Plus_Billboard_Alpha:
                        break;
                    case Albedo.Diffuse_Plus_Sprite_Alpha:
                        break;
                    case Albedo.Palettized_Plus_Sprite_Alpha:
                        break;
                    case Albedo.Diffuse_Modulated:
                        break;
                    case Albedo.Palettized_Glow:
                        break;
                    case Albedo.Palettized_Plasma:
                        break;
                    case Albedo.Palettized_2d_Plasma:
                        break;
                }
            }

            if (methodName == "blend_mode")
            {
                switch ((Blend_Mode)option)
                {
                    case Blend_Mode.Opaque:
                        break;
                    case Blend_Mode.Additive:
                        break;
                    case Blend_Mode.Multiply:
                        break;
                    case Blend_Mode.Alpha_Blend:
                        break;
                    case Blend_Mode.Double_Multiply:
                        break;
                    case Blend_Mode.Maximum:
                        break;
                    case Blend_Mode.Multiply_Add:
                        break;
                    case Blend_Mode.Add_Src_Times_Dstalpha:
                        break;
                    case Blend_Mode.Add_Src_Times_Srcalpha:
                        break;
                    case Blend_Mode.Inv_Alpha_Blend:
                        break;
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        break;
                }
            }

            if (methodName == "specialized_rendering")
            {
                switch ((Specialized_Rendering)option)
                {
                    case Specialized_Rendering.None:
                        break;
                    case Specialized_Rendering.Distortion:
                        break;
                    case Specialized_Rendering.Distortion_Expensive:
                        break;
                    case Specialized_Rendering.Distortion_Diffuse:
                        break;
                    case Specialized_Rendering.Distortion_Expensive_Diffuse:
                        break;
                }
            }

            if (methodName == "lighting")
            {
                switch ((Lighting)option)
                {
                    case Lighting.None:
                        break;
                    case Lighting.Per_Pixel_Ravi_Order_3:
                        break;
                    case Lighting.Per_Vertex_Ravi_Order_0:
                        break;
                    case Lighting.Per_Pixel_Smooth:
                        break;
                    case Lighting.Per_Vertex_Ambient:
                        break;
                    case Lighting.Smoke_Lighting:
                        break;
                }
            }

            if (methodName == "render_targets")
            {
                switch ((Render_Targets)option)
                {
                    case Render_Targets.Ldr_And_Hdr:
                        break;
                    case Render_Targets.Ldr_Only:
                        break;
                }
            }

            if (methodName == "depth_fade")
            {
                switch ((Depth_Fade)option)
                {
                    case Depth_Fade.Off:
                        break;
                    case Depth_Fade.On:
                        break;
                    case Depth_Fade.Low_Res:
                        break;
                    case Depth_Fade.Palette_Shift:
                        break;
                }
            }

            if (methodName == "black_point")
            {
                switch ((Black_Point)option)
                {
                    case Black_Point.Off:
                        break;
                    case Black_Point.On:
                        break;
                }
            }

            if (methodName == "fog")
            {
                switch ((Fog)option)
                {
                    case Fog.Off:
                        break;
                    case Fog.On:
                        break;
                }
            }

            if (methodName == "frame_blend")
            {
                switch ((Frame_Blend)option)
                {
                    case Frame_Blend.Off:
                        break;
                    case Frame_Blend.On:
                        break;
                }
            }

            if (methodName == "self_illumination")
            {
                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.None:
                        break;
                    case Self_Illumination.Constant_Color:
                        break;
                }
            }

            if (methodName == "warp")
            {
                switch ((Warp)option)
                {
                    case Warp.None:
                        break;
                    case Warp.Sphere:
                        break;
                }
            }
            return result;
        }
    }
}
