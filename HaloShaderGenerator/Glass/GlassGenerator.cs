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
            result.AddFloat3ColorParameter("debug_tint", RenderMethodExtern.debug_tint, default, default, default, default, new ShaderColor(255, 255, 255, 255));
            result.AddSamplerParameter("active_camo_distortion_texture", RenderMethodExtern.active_camo_distortion_texture, default, default, default, default, default);
            result.AddSamplerParameter("albedo_texture", RenderMethodExtern.texture_global_target_texaccum, default, default, default, default, default);
            result.AddSamplerParameter("dominant_light_intensity_map", RenderMethodExtern.texture_dominant_light_intensity_map, default, default, default, default, default);
            result.AddSamplerParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0, default, default, default, default, default);
            result.AddSamplerParameter("g_diffuse_power_specular", default, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"rasterizer\diffuse_power_specular\diffuse_power");
            result.AddSamplerParameter("g_direction_lut", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"rasterizer\direction_lut_1002");
            result.AddSamplerParameter("g_sample_vmf_diffuse_vs", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"rasterizer\diffusetable");
            result.AddSamplerParameter("g_sample_vmf_diffuse", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"rasterizer\diffusetable");
            result.AddSamplerParameter("g_sample_vmf_phong_specular", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"rasterizer\diffuse_power_specular\diffuse_power");
            result.AddSamplerParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture, ShaderOptionParameter.ShaderFilterMode.Bilinear, default, default, default, default);
            result.AddSamplerParameter("normal_texture", RenderMethodExtern.texture_global_target_normal, default, default, default, default, default);
            result.AddSamplerParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture, default, default, default, default, default);
            result.AddSamplerParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture, default, default, default, default, default);
            result.AddSamplerParameter("shadow_depth_map_1", RenderMethodExtern.texture_global_target_shadow_buffer1, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
            result.AddSamplerParameter("shadow_mask_texture", default, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default); // rmExtern - texture_global_target_shadow_mask
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
                        result.AddFloat4ColorParameter("albedo_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
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
                        result.AddSamplerParameter("bump_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\bump_default";
                        break;
                    case Bump_Mapping.Detail:
                        result.AddFloatParameter("bump_detail_coefficient", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("bump_detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\bump_detail";
                        break;
                    case Bump_Mapping.Detail_Blend:
                        result.AddFloatParameter("blend_alpha", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("bump_detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_detail_map2", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\bump_detail_blend";
                        break;
                    case Bump_Mapping.Three_Detail_Blend:
                        result.AddFloatParameter("blend_alpha", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("bump_detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_detail_map2", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_detail_map3", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\bump_three_detail_blend";
                        break;
                    case Bump_Mapping.Standard_Wrinkle:
                        result.AddFloat4ColorParameter("wrinkle_weights_a", default, default, default, default, default, default); // rmExtern - wrinkle_weights_a
                        result.AddFloat4ColorParameter("wrinkle_weights_b", default, default, default, default, default, default); // rmExtern - wrinkle_weights_b
                        result.AddSamplerParameter("bump_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("wrinkle_mask_a", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddSamplerParameter("wrinkle_mask_b", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddSamplerParameter("wrinkle_normal", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\bump_default_wrinkle";
                        break;
                    case Bump_Mapping.Detail_Wrinkle:
                        result.AddFloat4ColorParameter("wrinkle_weights_a", default, default, default, default, default, default); // rmExtern - wrinkle_weights_a
                        result.AddFloat4ColorParameter("wrinkle_weights_b", default, default, default, default, default, default); // rmExtern - wrinkle_weights_b
                        result.AddSamplerParameter("bump_detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("wrinkle_mask_a", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddSamplerParameter("wrinkle_mask_b", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddSamplerParameter("wrinkle_normal", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
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
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("glancing_specular_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("normal_specular_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_color_by_angle", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_power", default, default, default, default, default, 25f);
                        result.AddFloatParameter("analytical_roughness", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness", default, default, default, default, default, 5f);
                        result.AddFloatParameter("glancing_specular_power", default, default, default, default, default, 10f);
                        result.AddFloatParameter("normal_specular_power", default, default, default, default, default, 10f);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, 1f);
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
                        result.AddFloat3ColorParameter("env_tint_color", default, default, default, default, default, default);
                        result.AddFloatParameter("env_roughness_offset", default, default, default, default, default, 0.5f);
                        result.AddSamplerParameter("environment_map", default, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"shaders\default_bitmaps\bitmaps\default_dynamic_cube_map");
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.Dynamic:
                        result.AddFloat3ColorParameter("env_tint_color", default, default, default, default, default, default);
                        result.AddFloatParameter("env_roughness_offset", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("env_roughness_scale", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
                        result.AddSamplerParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
                        rmopName = @"shaders\shader_options\env_map_dynamic";
                        break;
                    case Environment_Mapping.From_Flat_Texture:
                        result.AddFloat3ColorParameter("env_tint_color", default, default, default, default, default, default);
                        result.AddFloat4ColorParameter("env_bloom_override", default, default, default, default, default, default);
                        result.AddFloat4ColorParameter("flat_envmap_matrix_x", RenderMethodExtern.flat_envmap_matrix_x, default, default, default, default, default);
                        result.AddFloat4ColorParameter("flat_envmap_matrix_y", RenderMethodExtern.flat_envmap_matrix_y, default, default, default, default, default);
                        result.AddFloat4ColorParameter("flat_envmap_matrix_z", RenderMethodExtern.flat_envmap_matrix_z, default, default, default, default, default);
                        result.AddFloatParameter("env_bloom_override_intensity", default, default, default, default, default, 1f);
                        result.AddFloatParameter("hemisphere_percentage", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("flat_environment_map", default, default, ShaderOptionParameter.ShaderAddressMode.BlackBorder, default, default, @"shaders\default_bitmaps\bitmaps\color_red");
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
                        result.AddFloat3ColorParameter("wet_material_dim_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("wet_material_dim_coefficient", default, default, default, default, default, 1f);
                        rmopName = @"shaders\wetness_options\wetness_simple";
                        break;
                    case Wetness.Flood:
                        result.AddFloat3ColorParameter("wet_material_dim_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("wet_sheen_reflection_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_mask_tweak_weight", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("surface_tilt_tweak_weight", default, default, default, default, default, default);
                        result.AddFloatParameter("wet_material_dim_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("wet_sheen_reflection_contribution", default, default, default, default, default, 0.3f);
                        result.AddFloatParameter("wet_sheen_thickness", default, default, default, default, default, 0.9f);
                        result.AddSamplerParameter("wet_flood_slope_map", default, default, default, default, default, @"rasterizer\water\static_wave\static_wave_slope_water");
                        result.AddSamplerParameter("wet_noise_boundary_map", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, default, default, default, @"rasterizer\rain\rain_noise_boundary");
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
                        result.AddFloatParameter("opacity_fresnel_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("opacity_fresnel_curve_bias", default, default, default, default, default, default);
                        result.AddFloatParameter("opacity_fresnel_curve_steepness", default, default, default, default, default, 3f);
                        rmopName = @"shaders\shader_options\blend_source_from_albedo_alpha";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                        result.AddFloatParameter("opacity_fresnel_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("opacity_fresnel_curve_bias", default, default, default, default, default, default);
                        result.AddFloatParameter("opacity_fresnel_curve_steepness", default, default, default, default, default, 3f);
                        result.AddSamplerParameter("opacity_texture", default, default, default, default, default, default);
                        rmopName = @"shaders\shader_options\blend_source_from_opacity_map";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                        result.AddFloatParameter("opacity_fresnel_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("opacity_fresnel_curve_bias", default, default, default, default, default, default);
                        result.AddFloatParameter("opacity_fresnel_curve_steepness", default, default, default, default, default, 3f);
                        result.AddSamplerParameter("opacity_texture", default, default, default, default, default, default);
                        rmopName = @"shaders\shader_options\blend_source_from_opacity_map";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                        result.AddFloatParameter("opacity_fresnel_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("opacity_fresnel_curve_bias", default, default, default, default, default, default);
                        result.AddFloatParameter("opacity_fresnel_curve_steepness", default, default, default, default, default, 3f);
                        result.AddSamplerParameter("opacity_texture", default, default, default, default, default, default);
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
