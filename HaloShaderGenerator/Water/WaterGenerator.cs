using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Water
{
    public class WaterGenerator : IShaderGenerator
    {
        private bool TemplateGenerationValid;
        private bool ApplyFixes;

        Waveshape waveshape;
        Watercolor watercolor;
        Reflection reflection;
        Refraction refraction;
        Bankalpha bankalpha;
        Appearance appearance;
        Global_Shape global_shape;
        Foam foam;

        public WaterGenerator(bool applyFixes = false) { TemplateGenerationValid = false; ApplyFixes = applyFixes; }

        public WaterGenerator(Waveshape waveshape, Watercolor watercolor, Reflection reflection, Refraction refraction, 
            Bankalpha bankalpha, Appearance appearance, Global_Shape global_shape, Foam foam, bool applyFixes = false)
        {
            this.waveshape = waveshape;
            this.watercolor = watercolor;
            this.reflection = reflection;
            this.refraction = refraction;
            this.bankalpha = bankalpha;
            this.appearance = appearance;
            this.global_shape = global_shape;
            this.foam = foam;

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public WaterGenerator(byte[] options, bool applyFixes = false)
        {
            this.waveshape = (Waveshape)options[0];
            this.watercolor = (Watercolor)options[1];
            this.reflection = (Reflection)options[2];
            this.refraction = (Refraction)options[3];
            this.bankalpha = (Bankalpha)options[4];
            this.appearance = (Appearance)options[5];
            this.global_shape = (Global_Shape)options[6];
            this.foam = (Foam)options[7];

            ApplyFixes = applyFixes;
            TemplateGenerationValid = true;
        }

        public ShaderGeneratorResult GeneratePixelShader(ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Shared.ShaderType>());

            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Waveshape>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Watercolor>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Reflection>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Refraction>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Bankalpha>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Appearance>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Global_Shape>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Foam>());

            macros.Add(ShaderGeneratorBase.CreateMacro("APPLY_HLSL_FIXES", ApplyFixes ? 1 : 0));

            //
            // The following code properly names the macros (like in rmdf)
            //

            macros.Add(ShaderGeneratorBase.CreateMacro("shaderstage", entryPoint, "k_shaderstage_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("shadertype", Shared.ShaderType.Water, "k_shadertype_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("calc_watercolor_ps", watercolor, "calc_watercolor_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_reflection_ps", reflection, "calc_reflection_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_refraction_ps", refraction, "calc_refraction_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_bankalpha_ps", bankalpha, "calc_bankalpha_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_foam_ps", foam, "calc_foam_", "_ps"));

            macros.Add(ShaderGeneratorBase.CreateMacro("waveshape_arg", waveshape, "k_waveshape_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("watercolor_arg", watercolor, "k_watercolor_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("reflection_arg", reflection, "k_reflection_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("refraction_arg", refraction, "k_refraction_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("bankalpha_arg", bankalpha, "k_bankalpha_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("appearance_arg", appearance, "k_appearance_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("global_shape_arg", global_shape, "k_global_shape_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("foam_arg", foam, "k_foam_"));

            byte[] shaderBytecode = ShaderGeneratorBase.GenerateSource($"pixl_water.hlsl", macros, "entry_" + entryPoint.ToString().ToLower(), "ps_3_0");

            return new ShaderGeneratorResult(shaderBytecode);
        }

        public ShaderGeneratorResult GenerateSharedPixelShader(ShaderStage entryPoint, int methodIndex, int optionIndex)
        {
            return null;
        }

        public ShaderGeneratorResult GenerateSharedVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            // TODO
            return null;
        }

        public ShaderGeneratorResult GenerateVertexShader(VertexType vertexType, ShaderStage entryPoint)
        {
            if (!TemplateGenerationValid)
                throw new System.Exception("Generator initialized with shared shader constructor. Use template constructor.");
            return null;
        }

        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(WaterMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((WaterMethods)methodIndex)
            {
                case WaterMethods.Waveshape:
                    return Enum.GetValues(typeof(Waveshape)).Length;
                case WaterMethods.Watercolor:
                    return Enum.GetValues(typeof(Watercolor)).Length;
                case WaterMethods.Reflection:
                    return Enum.GetValues(typeof(Reflection)).Length;
                case WaterMethods.Refraction:
                    return Enum.GetValues(typeof(Refraction)).Length;
                case WaterMethods.Bankalpha:
                    return Enum.GetValues(typeof(Bankalpha)).Length;
                case WaterMethods.Appearance:
                    return Enum.GetValues(typeof(Appearance)).Length;
                case WaterMethods.Global_Shape:
                    return Enum.GetValues(typeof(Global_Shape)).Length;
                case WaterMethods.Foam:
                    return Enum.GetValues(typeof(Foam)).Length;
            }
            return -1;
        }

        public int GetMethodOptionValue(int methodIndex)
        {
            switch ((WaterMethods)methodIndex)
            {
                case WaterMethods.Waveshape:
                    return (int)waveshape;
                case WaterMethods.Watercolor:
                    return (int)watercolor;
                case WaterMethods.Reflection:
                    return (int)reflection;
                case WaterMethods.Refraction:
                    return (int)refraction;
                case WaterMethods.Bankalpha:
                    return (int)bankalpha;
                case WaterMethods.Appearance:
                    return (int)appearance;
                case WaterMethods.Global_Shape:
                    return (int)global_shape;
                case WaterMethods.Foam:
                    return (int)foam;
            }
            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Water_Tesselation:
                    return true;
            }
            return false;
        }

        public bool IsMethodSharedInEntryPoint(ShaderStage entryPoint, int method_index) => false;

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint) => false;

        public bool IsSharedPixelShaderWithoutMethod(ShaderStage entryPoint) => false;

        public bool IsPixelShaderShared(ShaderStage entryPoint) => false;

        public bool IsVertexFormatSupported(VertexType vertexType) => vertexType == VertexType.Water;

        public bool IsVertexShaderShared(ShaderStage entryPoint)
        {
            return IsEntryPointSupported(entryPoint);
        }

        public ShaderParameters GetPixelShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;
            var result = new ShaderParameters();

            switch (waveshape)
            {
                case Waveshape.Default:
                    result.AddFloatParameter("displacement_range_x");
                    result.AddFloatParameter("displacement_range_y");
                    result.AddFloatParameter("displacement_range_z");
                    result.AddFloatParameter("slope_range_x");
                    result.AddFloatParameter("slope_range_y");
                    result.AddSamplerParameter("wave_displacement_array");
                    result.AddFloatParameter("wave_height");
                    result.AddFloatParameter("time_warp");
                    result.AddSamplerParameter("wave_slope_array");
                    result.AddFloatParameter("wave_height_aux");
                    result.AddFloatParameter("time_warp_aux");
                    result.AddFloatParameter("detail_slope_scale_x");
                    result.AddFloatParameter("detail_slope_scale_y");
                    result.AddFloatParameter("detail_slope_scale_z");
                    result.AddFloatParameter("detail_slope_steepness");
                    break;
                case Waveshape.Bump:
                    result.AddSamplerParameter("bump_map");
                    result.AddSamplerParameter("bump_detail_map");
                    break;
            }

            switch (watercolor)
            {
                case Watercolor.Pure:
                    result.AddFloat4Parameter("water_color_pure");
                    break;
                case Watercolor.Texture:
                    result.AddSamplerWithoutXFormParameter("watercolor_texture");
                    result.AddFloatParameter("watercolor_coefficient");
                    break;
            }

            switch (reflection)
            {
                case Reflection.Static:
                    result.AddSamplerWithoutXFormParameter("environment_map");
                    result.AddFloatParameter("reflection_coefficient");
                    result.AddFloatParameter("sunspot_cut");
                    result.AddFloatParameter("shadow_intensity_mark");
                    break;
                case Reflection.Dynamic:
                    result.AddFloatParameter("reflection_coefficient");
                    break;
            }

            switch (refraction)
            {
                case Refraction.Dynamic:
                    result.AddFloatParameter("refraction_texcoord_shift");
                    result.AddFloatParameter("water_murkiness");
                    result.AddFloatParameter("refraction_extinct_distance");
                    result.AddFloatParameter("minimal_wave_disturbance");
                    result.AddFloatParameter("refraction_depth_dominant_ratio");
                    break;
            }

            switch (bankalpha)
            {
                case Bankalpha.Depth:
                    result.AddFloatParameter("bankalpha_infuence_depth");
                    break;
                case Bankalpha.Paint:
                    result.AddSamplerParameter("watercolor_texture");
                    break;
            }

            switch (appearance)
            {
                case Appearance.Default:
                    result.AddFloatParameter("fresnel_coefficient");
                    result.AddFloat4Parameter("water_diffuse");
                    result.AddBooleanParameter("no_dynamic_lights");
                    break;
            }

            switch (global_shape)
            {
                case Global_Shape.Paint:
                    result.AddSamplerWithoutXFormParameter("global_shape_texture"); //v
                    break;
                case Global_Shape.Depth:
                    result.AddFloatParameter("globalshape_infuence_depth"); //v
                    break;
            }

            switch (foam)
            {
                case Foam.Auto:
                    result.AddSamplerParameter("foam_texture");
                    result.AddSamplerParameter("foam_texture_detail");
                    result.AddFloatParameter("foam_height");
                    result.AddFloatParameter("foam_pow");
                    break;
                case Foam.Paint:
                    result.AddSamplerParameter("foam_texture");
                    result.AddSamplerParameter("foam_texture_detail");
                    result.AddSamplerWithoutXFormParameter("global_shape_texture"); //v
                    break;
                case Foam.Both:
                    result.AddSamplerParameter("foam_texture");
                    result.AddSamplerParameter("foam_texture_detail");
                    result.AddSamplerWithoutXFormParameter("global_shape_texture"); //v
                    result.AddFloatParameter("foam_height");
                    result.AddFloatParameter("foam_pow");
                    break;
            }

            return result;
        }

        public ShaderParameters GetVertexShaderParameters()
        {
            if (!TemplateGenerationValid)
                return null;
            var result = new ShaderParameters();

            result.AddPrefixedFloat4VertexParameter("waveshape", "category_");
            result.AddPrefixedFloat4VertexParameter("global_shape", "category_");

            switch (waveshape)
            {
                case Waveshape.Default:
                    result.AddFloatVertexParameter("displacement_range_x");
                    result.AddFloatVertexParameter("displacement_range_y");
                    result.AddFloatVertexParameter("displacement_range_z");
                    result.AddSamplerVertexParameter("wave_displacement_array");
                    result.AddFloatVertexParameter("wave_height");
                    result.AddFloatVertexParameter("time_warp");
                    result.AddSamplerVertexParameter("wave_slope_array");
                    result.AddFloatVertexParameter("wave_height_aux");
                    result.AddFloatVertexParameter("time_warp_aux");
                    result.AddFloatVertexParameter("choppiness_forward");
                    result.AddFloatVertexParameter("choppiness_backward");
                    result.AddFloatVertexParameter("choppiness_side");
                    result.AddFloatVertexParameter("wave_visual_damping_distance");
                    break;
            }

            switch (global_shape)
            {
                case Global_Shape.Paint:
                    result.AddSamplerWithoutXFormVertexParameter("global_shape_texture"); //v
                    break;
                case Global_Shape.Depth:
                    result.AddFloatVertexParameter("globalshape_infuence_depth"); //v
                    break;
            }

            switch (foam)
            {
                case Foam.Paint:
                    result.AddSamplerWithoutXFormVertexParameter("global_shape_texture"); //v
                    break;
                case Foam.Both:
                    result.AddSamplerWithoutXFormVertexParameter("global_shape_texture"); //v
                    break;
            }

            return result;
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();
            result.AddFloat4Parameter("water_memory_export_addr", RenderMethodExtern.water_memory_export_address);
            result.AddSamplerWithoutXFormParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerWithoutXFormParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture);
            result.AddSamplerWithoutXFormParameter("depth_buffer", RenderMethodExtern.texture_global_target_z);
            result.AddSamplerWithoutXFormParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture);
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
            return Enum.GetValues(typeof(WaterMethods));
        }
    }
}
