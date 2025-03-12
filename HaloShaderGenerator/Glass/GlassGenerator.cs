using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Glass
{
    public class GlassGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(GlassMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((GlassMethods)methodIndex)
            {
                case GlassMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case GlassMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping)).Length;
                case GlassMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model)).Length;
                case GlassMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping)).Length;
                case GlassMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness)).Length;
                case GlassMethods.Alpha_Blend_Source:
                    return Enum.GetValues(typeof(Alpha_Blend_Source)).Length;
            }

            return -1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Static_Sh:
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Static_Prt_Ambient:
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
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Static_Sh:
                case ShaderStage.Static_Prt_Ambient:
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
            result.AddSamplerParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0);
            result.AddFloat3ColorParameter("debug_tint", RenderMethodExtern.debug_tint);
            result.AddSamplerParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerParameter("scene_hdr_texture");
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
                    case Albedo.Map:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddFloat4ColorParameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_default";
                        break;
                }
            }

            if (methodName == "bump_mapping")
            {
                optionName = ((Bump_Mapping)option).ToString();

                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Off:
                        rmopName = @"shaders\shader_options\bump_off";
                        break;
                    case Bump_Mapping.Standard:
                        result.AddSamplerParameter("bump_map");
                        rmopName = @"shaders\shader_options\bump_default";
                        break;
                    case Bump_Mapping.Detail:
                        result.AddSamplerParameter("bump_map");
                        result.AddSamplerParameter("bump_detail_map");
                        rmopName = @"shaders\shader_options\bump_detail";
                        break;
                    case Bump_Mapping.Detail_Blend:
                        result.AddSamplerParameter("bump_map");
                        result.AddSamplerParameter("bump_detail_map");
                        result.AddSamplerParameter("bump_detail_map2");
                        result.AddFloatParameter("blend_alpha");
                        rmopName = @"shaders\shader_options\bump_detail_blend";
                        break;
                    case Bump_Mapping.Three_Detail_Blend:
                        result.AddSamplerParameter("bump_map");
                        result.AddSamplerParameter("bump_detail_map");
                        result.AddSamplerParameter("bump_detail_map2");
                        result.AddSamplerParameter("bump_detail_map3");
                        result.AddFloatParameter("blend_alpha");
                        rmopName = @"shaders\shader_options\bump_three_detail_blend";
                        break;
                    case Bump_Mapping.Standard_Wrinkle:
                        result.AddSamplerParameter("bump_map");
                        result.AddSamplerParameter("wrinkle_normal");
                        result.AddSamplerParameter("wrinkle_mask_a");
                        result.AddSamplerParameter("wrinkle_mask_b");
                        result.AddFloat4ColorParameter("wrinkle_weights_a", RenderMethodExtern.none);
                        result.AddFloat4ColorParameter("wrinkle_weights_b", RenderMethodExtern.none);
                        rmopName = @"shaders\shader_options\bump_default_wrinkle";
                        break;
                    case Bump_Mapping.Detail_Wrinkle:
                        result.AddSamplerParameter("bump_map");
                        result.AddSamplerParameter("bump_detail_map");
                        result.AddSamplerParameter("wrinkle_normal");
                        result.AddSamplerParameter("wrinkle_mask_a");
                        result.AddSamplerParameter("wrinkle_mask_b");
                        result.AddFloat4ColorParameter("wrinkle_weights_a", RenderMethodExtern.none);
                        result.AddFloat4ColorParameter("wrinkle_weights_b", RenderMethodExtern.none);
                        rmopName = @"shaders\shader_options\bump_detail_wrinkle";
                        break;
                }
            }

            if (methodName == "material_model")
            {
                optionName = ((Material_Model)option).ToString();

                switch ((Material_Model)option)
                {
                    case Material_Model.Two_Lobe_Phong:
                        result.AddFloatParameter("diffuse_coefficient");
                        result.AddFloat3ColorParameter("specular_color_by_angle");
                        result.AddFloatParameter("specular_coefficient");
                        result.AddFloatParameter("normal_specular_power");
                        result.AddFloat3ColorParameter("normal_specular_tint");
                        result.AddFloatParameter("glancing_specular_power");
                        result.AddFloat3ColorParameter("glancing_specular_tint");
                        result.AddFloatParameter("fresnel_curve_steepness");
                        result.AddFloatParameter("analytical_specular_contribution");
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddFloatParameter("analytical_roughness");
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("analytical_power");
                        result.AddFloatParameter("albedo_specular_tint_blend");
                        rmopName = @"shaders\glass_options\glass_specular_option";
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
                        result.AddSamplerParameter("environment_map", default, default, ShaderOptionParameter.ShaderAddressMode.Clamp);
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
                    case Environment_Mapping.From_Flat_Texture:
                        result.AddSamplerParameter("flat_environment_map", default, default, ShaderOptionParameter.ShaderAddressMode.BlackBorder);
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddFloat4ColorParameter("flat_envmap_matrix_x", RenderMethodExtern.flat_envmap_matrix_x);
                        result.AddFloat4ColorParameter("flat_envmap_matrix_y", RenderMethodExtern.flat_envmap_matrix_y);
                        result.AddFloat4ColorParameter("flat_envmap_matrix_z", RenderMethodExtern.flat_envmap_matrix_z);
                        result.AddFloatParameter("hemisphere_percentage");
                        result.AddFloat4ColorParameter("env_bloom_override");
                        result.AddFloatParameter("env_bloom_override_intensity");
                        rmopName = @"shaders\shader_options\env_map_from_flat_texture";
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
                    case Wetness.Flood:
                        result.AddFloatParameter("wet_material_dim_coefficient");
                        result.AddFloat3ColorParameter("wet_material_dim_tint");
                        result.AddFloatParameter("wet_sheen_reflection_contribution");
                        result.AddFloat3ColorParameter("wet_sheen_reflection_tint");
                        result.AddFloatParameter("wet_sheen_thickness");
                        result.AddSamplerParameter("wet_flood_slope_map");
                        result.AddSamplerParameter("wet_noise_boundary_map", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, default);
                        result.AddFloatParameter("specular_mask_tweak_weight");
                        result.AddFloatParameter("surface_tilt_tweak_weight");
                        rmopName = @"shaders\wetness_options\wetness_flood";
                        break;
                }
            }

            if (methodName == "alpha_blend_source")
            {
                optionName = ((Alpha_Blend_Source)option).ToString();

                switch ((Alpha_Blend_Source)option)
                {
                    case Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel:
                        break;
                    case Alpha_Blend_Source.From_Albedo_Alpha:
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness");
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_albedo_alpha";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                        result.AddSamplerParameter("opacity_texture");
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness");
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_opacity_map";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                        result.AddSamplerParameter("opacity_texture");
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness");
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_opacity_map";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                        result.AddSamplerParameter("opacity_texture");
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness");
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_opacity_map";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(GlassMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((GlassMethods)methodIndex)
            {
                case GlassMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case GlassMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping));
                case GlassMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model));
                case GlassMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping));
                case GlassMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness));
                case GlassMethods.Alpha_Blend_Source:
                    return Enum.GetValues(typeof(Alpha_Blend_Source));
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

            if (methodName == "bump_mapping")
            {
                vertexFunction = "calc_bumpmap_vs";
                pixelFunction = "calc_bumpmap_ps";
            }

            if (methodName == "material_model")
            {
                vertexFunction = "invalid";
                pixelFunction = "material_type";
            }

            if (methodName == "environment_mapping")
            {
                vertexFunction = "invalid";
                pixelFunction = "envmap_type";
            }

            if (methodName == "wetness")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_wetness_ps";
            }

            if (methodName == "alpha_blend_source")
            {
                vertexFunction = "invalid";
                pixelFunction = "alpha_blend_source";
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
                    case Albedo.Map:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_default_ps";
                        break;
                }
            }

            if (methodName == "bump_mapping")
            {
                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Off:
                        vertexFunction = "calc_bumpmap_off_vs";
                        pixelFunction = "calc_bumpmap_off_ps";
                        break;
                    case Bump_Mapping.Standard:
                        vertexFunction = "calc_bumpmap_default_vs";
                        pixelFunction = "calc_bumpmap_default_ps";
                        break;
                    case Bump_Mapping.Detail:
                        vertexFunction = "calc_bumpmap_detail_vs";
                        pixelFunction = "calc_bumpmap_detail_ps";
                        break;
                    case Bump_Mapping.Detail_Blend:
                        vertexFunction = "calc_bumpmap_detail_blend_vs";
                        pixelFunction = "calc_bumpmap_detail_blend_ps";
                        break;
                    case Bump_Mapping.Three_Detail_Blend:
                        vertexFunction = "calc_bumpmap_three_detail_blend_vs";
                        pixelFunction = "calc_bumpmap_three_detail_blend_ps";
                        break;
                    case Bump_Mapping.Standard_Wrinkle:
                        vertexFunction = "calc_bumpmap_default_wrinkle_vs";
                        pixelFunction = "calc_bumpmap_default_wrinkle_ps";
                        break;
                    case Bump_Mapping.Detail_Wrinkle:
                        vertexFunction = "calc_bumpmap_detail_wrinkle_vs";
                        pixelFunction = "calc_bumpmap_detail_wrinkle_ps";
                        break;
                }
            }

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Two_Lobe_Phong:
                        vertexFunction = "invalid";
                        pixelFunction = "two_lobe_phong";
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
                    case Environment_Mapping.From_Flat_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "from_flat_texture";
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
                    case Wetness.Flood:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_flood_ps";
                        break;
                }
            }

            if (methodName == "alpha_blend_source")
            {
                switch ((Alpha_Blend_Source)option)
                {
                    case Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel:
                        vertexFunction = "invalid";
                        pixelFunction = "albedo_alpha_without_fresnel";
                        break;
                    case Alpha_Blend_Source.From_Albedo_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "albedo_alpha";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "opacity_map_alpha";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                        vertexFunction = "invalid";
                        pixelFunction = "opacity_map_rgb";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "opacity_map_alpha_and_albedo_alpha";
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
                    case Albedo.Map:
                        break;
                }
            }

            if (methodName == "bump_mapping")
            {
                switch ((Bump_Mapping)option)
                {
                    case Bump_Mapping.Off:
                        break;
                    case Bump_Mapping.Standard:
                        break;
                    case Bump_Mapping.Detail:
                        break;
                    case Bump_Mapping.Detail_Blend:
                        break;
                    case Bump_Mapping.Three_Detail_Blend:
                        break;
                    case Bump_Mapping.Standard_Wrinkle:
                        break;
                    case Bump_Mapping.Detail_Wrinkle:
                        break;
                }
            }

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Two_Lobe_Phong:
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
                    case Environment_Mapping.From_Flat_Texture:
                        break;
                }
            }

            if (methodName == "wetness")
            {
                switch ((Wetness)option)
                {
                    case Wetness.Simple:
                        break;
                    case Wetness.Flood:
                        break;
                }
            }

            if (methodName == "alpha_blend_source")
            {
                switch ((Alpha_Blend_Source)option)
                {
                    case Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel:
                        break;
                    case Alpha_Blend_Source.From_Albedo_Alpha:
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                        break;
                }
            }
            return result;
        }
    }
}
