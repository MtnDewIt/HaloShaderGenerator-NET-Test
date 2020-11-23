using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;

namespace HaloShaderGenerator.Halogram
{
    public class HalogramGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;
        private bool ApplyFixes;

        Albedo albedo;
        Self_Illumination self_illumination;
        Blend_Mode blend_mode;
        Misc misc;
        Warp warp;
        Overlay overlay;
        Edge_Fade edge_fade;

        /// <summary>
        /// Generator insantiation for shared shaders. Does not require method options.
        /// </summary>
        public HalogramGenerator() { TemplateGenerationValid = false; }

        /// <summary>
        /// Generator instantiation for method specific shaders.
        /// </summary>
        public HalogramGenerator(Albedo albedo, Self_Illumination self_illumination, Blend_Mode blend_mode, Misc misc, Warp warp, Overlay overlay, Edge_Fade edge_fade, bool applyFixes = false)
        {
            this.albedo = albedo;
            this.self_illumination = self_illumination;
            this.blend_mode = blend_mode;
            this.misc = misc;
            this.warp = warp;
            this.overlay = overlay;
            this.edge_fade = edge_fade;

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public HalogramGenerator(byte[] options, bool applyFixes = false)
        {
            this.albedo = (Albedo)options[0];
            this.self_illumination = (Self_Illumination)options[1];
            this.blend_mode = (Blend_Mode)options[2];
            this.misc = (Misc)options[3];
            this.warp = (Warp)options[4];
            this.overlay = (Overlay)options[5];
            this.edge_fade = (Edge_Fade)options[6];

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public ShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            //macros.Add(new D3D.SHADER_MACRO { Name = "_HALOGRAM_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.ShaderType>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Albedo>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Self_Illumination>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.Blend_Mode>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Misc>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Warp>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Overlay>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Edge_Fade>());

            //
            // Convert to shared enum
            //

            var sAlbedo = Enum.Parse(typeof(Shared.Albedo), albedo.ToString());
            var sSelfIllumination = Enum.Parse(typeof(Shared.Self_Illumination), self_illumination.ToString());
            var sBlendMode = Enum.Parse(typeof(Shared.Blend_Mode), blend_mode.ToString());

            //
            // The following code properly names the macros (like in rmdf)
            //

            macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", sAlbedo, "calc_albedo_", "_ps"));
            if (albedo == Albedo.Constant_Color)
                macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", sAlbedo, "calc_albedo_", "_vs"));

            switch (self_illumination)
            {
                case Self_Illumination.Ml_Add_Four_Change_Color:
                case Self_Illumination.Ml_Add_Five_Change_Color:
                    // ml_add_four_change_color and ml_add_five_change_color use multilayer_additive ps macro
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", Shared.Self_Illumination.Multilayer_Additive, "calc_self_illumination_", "_ps"));
                    break;
                case Self_Illumination.Palettized_Plasma_Change_Color:
                    // Palettized_Plasma_Change_Color use Palettized_Plasma ps code
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", Shared.Self_Illumination.Palettized_Plasma, "calc_self_illumination_", "_ps"));
                    break;
                default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", sSelfIllumination, "calc_self_illumination_", "_ps"));
                    break;
            }

            macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", sBlendMode, "blend_type_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_warp", warp, "calc_warp_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_overlay_ps", overlay, "calc_overlay_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_edge_fade_ps", edge_fade, "calc_edge_fade_", "_ps"));

            macros.Add(ShaderGeneratorBase.CreateMacro("shaderstage", entryPoint, "k_shaderstage_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("shadertype", Shared.ShaderType.Halogram, "k_shadertype_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("albedo_arg", sAlbedo, "k_albedo_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("self_illumination_arg", sSelfIllumination, "k_self_illumination_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("blend_type_arg", sBlendMode, "k_blend_mode_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("misc_arg", misc, "k_misc_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("warp_arg", warp, "k_warp_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("overlay_arg", overlay, "k_overlay_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("edge_fade_arg", edge_fade, "k_edge_fade_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("APPLY_HLSL_FIXES", ApplyFixes));
            macros.Add(ShaderGeneratorBase.CreateMacro("_HALOGRAM", 1));

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"pixl_halogram.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

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

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"glps_halogram.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

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

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource(@"glvs_halogram.hlsl", macros, $"entry_{entryPoint.ToString().ToLower()}", "vs_3_0");

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
            return System.Enum.GetValues(typeof(HalogramMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((HalogramMethods)methodIndex)
            {
                case HalogramMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case HalogramMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination)).Length;
                case HalogramMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
                case HalogramMethods.Misc:
                    return Enum.GetValues(typeof(Misc)).Length;
                case HalogramMethods.Warp:
                    return Enum.GetValues(typeof(Warp)).Length;
                case HalogramMethods.Overlay:
                    return Enum.GetValues(typeof(Overlay)).Length;
                case HalogramMethods.Edge_Fade:
                    return Enum.GetValues(typeof(Edge_Fade)).Length;
            }

            return -1;
        }

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((HalogramMethods)methodIndex)
            {
                case HalogramMethods.Albedo:
                    return (int)albedo;
                case HalogramMethods.Self_Illumination:
                    return (int)self_illumination;
                case HalogramMethods.Blend_Mode:
                    return (int)blend_mode;
                case HalogramMethods.Misc:
                    return (int)misc;
                case HalogramMethods.Warp:
                    return (int)warp;
                case HalogramMethods.Overlay:
                    return (int)overlay;
                case HalogramMethods.Edge_Fade:
                    return (int)edge_fade;
            }
            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Albedo:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Static_Per_Vertex_Color:
                case ShaderStage.Dynamic_Light:
                case ShaderStage.Dynamic_Light_Cinematic:
                case ShaderStage.Static_Sh:
                    return true;
            }

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

        public bool IsPixelShaderShared(ShaderStage entryPoint)
        {
            return false;
        }

        public bool IsVertexFormatSupported(VertexType vertexType)
        {
            switch (vertexType)
            {
                case VertexType.World:
                case VertexType.Rigid:
                case VertexType.Skinned:
                    return true;
            }

            return false;
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
                case Albedo.Default:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddFloat4Parameter("albedo_color");
                    break;
                case Albedo.Detail_Blend:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
                    break;
                case Albedo.Constant_Color:
                    result.AddFloat4Parameter("albedo_color");
                    break;
                case Albedo.Two_Change_Color:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("change_color_map");
                    result.AddFloat4Parameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloat4Parameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                    break;
                case Albedo.Four_Change_Color:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("change_color_map");
                    result.AddFloat4Parameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloat4Parameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                    result.AddFloat4Parameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary);
                    result.AddFloat4Parameter("quaternary_change_color", RenderMethodExtern.object_change_color_quaternary);
                    break;
                case Albedo.Three_Detail_Blend:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
                    result.AddSamplerParameter("detail_map3");
                    break;
                case Albedo.Two_Detail_Overlay:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
                    result.AddSamplerParameter("detail_map_overlay");
                    break;
                case Albedo.Two_Detail:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
                    break;
                case Albedo.Color_Mask:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("color_mask_map");
                    result.AddFloat4Parameter("albedo_color");
                    result.AddFloat4Parameter("albedo_color2");
                    result.AddFloat4Parameter("albedo_color3");
                    result.AddFloat4Parameter("neutral_gray");
                    break;
                case Albedo.Two_Detail_Black_Point:
                    result.AddSamplerParameter("base_map");
                    result.AddSamplerParameter("detail_map");
                    result.AddSamplerParameter("detail_map2");
                    break;
            }
            switch (self_illumination)
            {
                case Self_Illumination.Simple:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat4Parameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination._3_Channel_Self_Illum:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat4Parameter("channel_a");
                    result.AddFloat4Parameter("channel_b");
                    result.AddFloat4Parameter("channel_c");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination.Plasma:
                    result.AddSamplerParameter("noise_map_a");
                    result.AddSamplerParameter("noise_map_b");
                    result.AddFloat4Parameter("color_medium");
                    result.AddFloat4Parameter("color_wide");
                    result.AddFloat4Parameter("color_sharp");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddSamplerParameter("alpha_mask_map");
                    result.AddFloatParameter("thinness_medium");
                    result.AddFloatParameter("thinness_wide");
                    result.AddFloatParameter("thinness_sharp");
                    break;
                case Self_Illumination.From_Diffuse:
                    result.AddFloat4Parameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination.Illum_Detail:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddSamplerParameter("self_illum_detail_map");
                    result.AddFloat4Parameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    break;
                case Self_Illumination.Meter:
                    result.AddSamplerParameter("meter_map");
                    result.AddFloat4Parameter("meter_color_off");
                    result.AddFloat4Parameter("meter_color_on");
                    result.AddFloatParameter("meter_value");
                    break;
                case Self_Illumination.Self_Illum_Times_Diffuse:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat4Parameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("primary_change_color_blend");
                    break;
                case Self_Illumination.Multilayer_Additive:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat4Parameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("layer_depth");
                    result.AddFloatParameter("layer_contrast");
                    result.AddFloatParameter("layers_of_4");
                    result.AddIntegerParameter("layers_of_4");
                    result.AddFloatParameter("texcoord_aspect_ratio");
                    result.AddFloatParameter("depth_darken");
                    break;
                case Self_Illumination.Ml_Add_Four_Change_Color:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat4Parameter("self_illum_color", RenderMethodExtern.object_change_color_quaternary);
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("layer_depth");
                    result.AddFloatParameter("layer_contrast");
                    result.AddFloatParameter("layers_of_4");
                    result.AddIntegerParameter("layers_of_4");
                    result.AddFloatParameter("texcoord_aspect_ratio");
                    result.AddFloatParameter("depth_darken");
                    break;
                case Self_Illumination.Ml_Add_Five_Change_Color:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat4Parameter("self_illum_color", RenderMethodExtern.object_change_color_quinary);
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("layer_depth");
                    result.AddFloatParameter("layer_contrast");
                    result.AddFloatParameter("layers_of_4");
                    result.AddIntegerParameter("layers_of_4");
                    result.AddFloatParameter("texcoord_aspect_ratio");
                    result.AddFloatParameter("depth_darken");
                    break;
                case Self_Illumination.Scope_Blur:
                    result.AddSamplerParameter("self_illum_map");
                    result.AddFloat4Parameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloat4Parameter("self_illum_heat_color");
                    break;
                case Self_Illumination.Palettized_Plasma:
                    result.AddSamplerParameter("noise_map_a");
                    result.AddSamplerParameter("noise_map_b");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerParameter("alpha_mask_map");
                    result.AddFloatParameter("alpha_modulation_factor");
                    result.AddSamplerParameter("depth_buffer", RenderMethodExtern.texture_global_target_z);
                    result.AddFloatParameter("depth_fade_range");
                    result.AddFloat4Parameter("self_illum_color");
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("v_coordinate");
                    result.AddFloat3Parameter("global_depth_constants", RenderMethodExtern.global_depth_constants);
                    result.AddFloat3Parameter("global_camera_forward", RenderMethodExtern.global_camera_forward);
                    break;
                case Self_Illumination.Palettized_Plasma_Change_Color:
                    result.AddSamplerParameter("noise_map_a");
                    result.AddSamplerParameter("noise_map_b");
                    result.AddSamplerWithoutXFormParameter("palette");
                    result.AddSamplerParameter("alpha_mask_map");
                    result.AddFloatParameter("alpha_modulation_factor");
                    result.AddSamplerParameter("depth_buffer", RenderMethodExtern.texture_global_target_z);
                    result.AddFloatParameter("depth_fade_range");
                    result.AddFloat4Parameter("self_illum_color", RenderMethodExtern.object_change_color_primary);
                    result.AddFloatParameter("self_illum_intensity");
                    result.AddFloatParameter("v_coordinate");
                    result.AddFloat3Parameter("global_depth_constants", RenderMethodExtern.global_depth_constants);
                    result.AddFloat3Parameter("global_camera_forward", RenderMethodExtern.global_camera_forward);
                    break;
            }
            switch (warp)
            {
                case Warp.From_Texture:
                    result.AddSamplerParameter("warp_map");
                    result.AddFloatParameter("warp_amount_x");
                    result.AddFloatParameter("warp_amount_y");
                    break;
            }
            switch (overlay)
            {
                case Overlay.Additive:
                    result.AddSamplerParameter("overlay_map");
                    result.AddFloat4Parameter("overlay_tint");
                    result.AddFloatParameter("overlay_intensity");
                    break;
                case Overlay.Additive_Detail:
                    result.AddSamplerParameter("overlay_map");
                    result.AddSamplerParameter("overlay_detail_map");
                    result.AddFloat4Parameter("overlay_tint");
                    result.AddFloatParameter("overlay_intensity");
                    break;
                case Overlay.Multiply:
                    result.AddSamplerParameter("overlay_map");
                    result.AddFloat4Parameter("overlay_tint");
                    result.AddFloatParameter("overlay_intensity");
                    break;
                case Overlay.Multiply_And_Additive_Detail:
                    result.AddSamplerParameter("overlay_multiply_map");
                    result.AddSamplerParameter("overlay_map");
                    result.AddSamplerParameter("overlay_detail_map");
                    result.AddFloat4Parameter("overlay_tint");
                    result.AddFloatParameter("overlay_intensity");
                    break;
            }
            switch (edge_fade)
            {
                case Edge_Fade.Simple:
                    result.AddFloat4Parameter("edge_fade_edge_tint");
                    result.AddFloat4Parameter("edge_fade_center_tint");
                    result.AddFloatParameter("edge_fade_power");
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
            rmopName = "";
            optionName = "";
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(HalogramMethods));
        }
    }
}
