using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Foliage
{
    public class FoliageGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(FoliageMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((FoliageMethods)methodIndex)
            {
                case FoliageMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case FoliageMethods.Alpha_Test:
                    return Enum.GetValues(typeof(Alpha_Test)).Length;
                case FoliageMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model)).Length;
                case FoliageMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness)).Length;
            }

            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Albedo:
                case ShaderStage.Static_Per_Pixel:
                case ShaderStage.Static_Sh:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Shadow_Generate:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Stipple:
                //case ShaderStage.Single_Pass_Per_Pixel:
                //case ShaderStage.Single_Pass_Per_Vertex:
                //case ShaderStage.Single_Pass_Single_Probe:
                //case ShaderStage.Single_Pass_Single_Probe_Ambient:
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
                case 1:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Shadow_Generate:
                    return true;
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
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Shadow_Generate:
                case ShaderStage.Z_Only:
                    return true;
                default:
                    return false;
            }
        }

        public ShaderParameters GetGlobalParameters()
        {
            var result = new ShaderParameters();
            result.AddSamplerParameter("albedo_texture", RenderMethodExtern.texture_global_target_texaccum);
            result.AddSamplerParameter("normal_texture", RenderMethodExtern.texture_global_target_normal);
            result.AddSamplerParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture);
            result.AddSamplerParameter("shadow_depth_map_1", RenderMethodExtern.texture_global_target_shadow_buffer1);
            result.AddSamplerParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0);
            result.AddFloat3ColorParameter("debug_tint", RenderMethodExtern.debug_tint);
            result.AddSamplerParameter("active_camo_distortion_texture", RenderMethodExtern.active_camo_distortion_texture);
            result.AddSamplerParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture);
            result.AddSamplerParameter("dominant_light_intensity_map", RenderMethodExtern.texture_dominant_light_intensity_map);
            result.AddSamplerParameter("g_sample_vmf_phong_specular");
            result.AddSamplerParameter("g_direction_lut");
            result.AddSamplerParameter("g_sample_vmf_diffuse");
            result.AddSamplerParameter("g_diffuse_power_specular");
            result.AddSamplerParameter("shadow_mask_texture", RenderMethodExtern.none);
            result.AddSamplerParameter("g_sample_vmf_diffuse_vs");
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
                    case Albedo.Default:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddFloat4ColorParameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_default";
                        break;
                    case Albedo.Simple:
                        result.AddSamplerParameter("base_map");
                        result.AddFloat4ColorParameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_simple";
                        break;
                }
            }

            if (methodName == "alpha_test")
            {
                optionName = ((Alpha_Test)option).ToString();

                switch ((Alpha_Test)option)
                {
                    case Alpha_Test.None:
                        rmopName = @"shaders\shader_options\alpha_test_off";
                        break;
                    case Alpha_Test.Simple:
                        result.AddSamplerParameter("alpha_test_map");
                        rmopName = @"shaders\shader_options\alpha_test_on";
                        break;
                    case Alpha_Test.From_Albedo_Alpha:
                        rmopName = @"shaders\shader_options\alpha_test_off";
                        break;
                    case Alpha_Test.From_Texture:
                        result.AddSamplerParameter("alpha_test_map");
                        rmopName = @"shaders\shader_options\alpha_test_on";
                        break;
                }
            }

            if (methodName == "material_model")
            {
                optionName = ((Material_Model)option).ToString();

                switch ((Material_Model)option)
                {
                    case Material_Model.Default:
                        result.AddFloat3ColorParameter("back_light");
                        result.AddFloatParameter("g_tree_animation_coeff", RenderMethodExtern.tree_animation_timer);
                        result.AddFloatParameter("animation_amplitude_horizontal");
                        rmopName = @"shaders\foliage_options\material_default";
                        break;
                    case Material_Model.Flat:
                        result.AddFloatParameter("g_tree_animation_coeff", RenderMethodExtern.tree_animation_timer);
                        result.AddFloatParameter("animation_amplitude_horizontal");
                        rmopName = @"shaders\foliage_options\material_flat";
                        break;
                    case Material_Model.Specular:
                        result.AddFloatParameter("g_tree_animation_coeff", RenderMethodExtern.tree_animation_timer);
                        result.AddFloatParameter("animation_amplitude_horizontal");
                        result.AddFloatParameter("foliage_translucency");
                        result.AddFloat3ColorParameter("foliage_specular_color");
                        result.AddFloatParameter("foliage_specular_intensity");
                        result.AddFloatParameter("foliage_specular_power");
                        rmopName = @"shaders\foliage_options\material_specular";
                        break;
                    case Material_Model.Translucent:
                        result.AddFloatParameter("g_tree_animation_coeff", RenderMethodExtern.tree_animation_timer);
                        result.AddFloatParameter("animation_amplitude_horizontal");
                        result.AddFloatParameter("foliage_translucency");
                        rmopName = @"shaders\foliage_options\material_translucent";
                        break;
                }
            }

            if (methodName == "wetness")
            {
                optionName = ((Wetness)option).ToString();

                switch ((Wetness)option)
                {
                    case Wetness.Simple:
                        result.AddFloatParameter("wet_material_dim_coefficient");
                        result.AddFloat3ColorParameter("wet_material_dim_tint");
                        rmopName = @"shaders\wetness_options\wetness_simple";
                        break;
                    case Wetness.Proof:
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(FoliageMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((FoliageMethods)methodIndex)
            {
                case FoliageMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case FoliageMethods.Alpha_Test:
                    return Enum.GetValues(typeof(Alpha_Test));
                case FoliageMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model));
                case FoliageMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness));
            }

            return null;
        }

        public void GetCategoryFunctions(string methodName, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "albedo")
            {
                vertexFunction = "calc_albedo_vs";
                pixelFunction = "calc_albedo_ps";
            }

            if (methodName == "alpha_test")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_alpha_test_ps";
            }

            if (methodName == "material_model")
            {
                vertexFunction = "invalid";
                pixelFunction = "calculate_material";
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

            if (methodName == "albedo")
            {
                switch ((Albedo)option)
                {
                    case Albedo.Default:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_default_ps";
                        break;
                    case Albedo.Simple:
                        vertexFunction = "calc_albedo_simple_vs";
                        pixelFunction = "calc_albedo_simple_ps";
                        break;
                }
            }

            if (methodName == "alpha_test")
            {
                switch ((Alpha_Test)option)
                {
                    case Alpha_Test.None:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_alpha_test_off_ps";
                        break;
                    case Alpha_Test.Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_alpha_test_on_ps";
                        break;
                    case Alpha_Test.From_Albedo_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_alpha_test_from_albedo_ps";
                        break;
                    case Alpha_Test.From_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_alpha_test_texture_ps";
                        break;
                }
            }

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "calculate_material_default";
                        break;
                    case Material_Model.Flat:
                        vertexFunction = "invalid";
                        pixelFunction = "flat";
                        break;
                    case Material_Model.Specular:
                        vertexFunction = "invalid";
                        pixelFunction = "specular";
                        break;
                    case Material_Model.Translucent:
                        vertexFunction = "invalid";
                        pixelFunction = "translucent";
                        break;
                }
            }

            if (methodName == "wetness")
            {
                switch ((Wetness)option)
                {
                    case Wetness.Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_simple_ps";
                        break;
                    case Wetness.Proof:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_proof_ps";
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
                    case Albedo.Default:
                        break;
                    case Albedo.Simple:
                        break;
                }
            }

            if (methodName == "alpha_test")
            {
                switch ((Alpha_Test)option)
                {
                    case Alpha_Test.None:
                        break;
                    case Alpha_Test.Simple:
                        break;
                    case Alpha_Test.From_Albedo_Alpha:
                        break;
                    case Alpha_Test.From_Texture:
                        break;
                }
            }

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Default:
                        break;
                    case Material_Model.Flat:
                        break;
                    case Material_Model.Specular:
                        break;
                    case Material_Model.Translucent:
                        break;
                }
            }

            if (methodName == "wetness")
            {
                switch ((Wetness)option)
                {
                    case Wetness.Simple:
                        break;
                    case Wetness.Proof:
                        break;
                }
            }
            return result;
        }
    }
}
