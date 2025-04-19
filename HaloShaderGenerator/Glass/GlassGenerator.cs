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

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                default:
                    return -1;
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

        public bool IsPixelShaderShared(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                default:
                    return false;
            }
        }

        public bool IsAutoMacro()
        {
            return false;
        }

        public ShaderParameters GetGlobalParameters(out string rmopName)
        {
            var result = new ShaderParameters();

            result.AddSamplerExternParameter("albedo_texture", RenderMethodExtern.texture_global_target_texaccum);
            result.AddSamplerExternParameter("normal_texture", RenderMethodExtern.texture_global_target_normal);
            result.AddSamplerExternFilterParameter("lightprobe_texture_array", RenderMethodExtern.texture_lightprobe_texture, ShaderOptionParameter.ShaderFilterMode.Bilinear);
            result.AddSamplerExternFilterAddressParameter("shadow_depth_map_1", RenderMethodExtern.texture_global_target_shadow_buffer1, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp);
            result.AddSamplerExternParameter("dynamic_light_gel_texture", RenderMethodExtern.texture_dynamic_light_gel_0);
            result.AddFloat3ColorExternWithFloatAndIntegerParameter("debug_tint", RenderMethodExtern.debug_tint, 1.0f, 1, new ShaderColor(255, 255, 255, 255));
            result.AddSamplerExternParameter("active_camo_distortion_texture", RenderMethodExtern.active_camo_distortion_texture);
            result.AddSamplerExternParameter("scene_ldr_texture", RenderMethodExtern.scene_ldr_texture);
            result.AddSamplerExternParameter("scene_hdr_texture", RenderMethodExtern.scene_hdr_texture);
            result.AddSamplerExternParameter("dominant_light_intensity_map", RenderMethodExtern.texture_dominant_light_intensity_map);
            //result.AddSamplerExternAddressParameter("g_diffuse_power_specular", RenderMethodExtern.material_diffuse_power, ShaderOptionParameter.ShaderAddressMode.Clamp);
            //result.AddSamplerFilterAddressParameter("g_direction_lut", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp);
            //result.AddSamplerFilterAddressParameter("g_sample_vmf_diffuse_vs", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp);
            //result.AddSamplerFilterAddressParameter("g_sample_vmf_diffuse", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp);
            //result.AddSamplerExternFilterAddressParameter("g_sample_vmf_phong_specular", RenderMethodExtern.material_diffuse_power, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp);
            //result.AddSamplerExternFilterAddressParameter("shadow_mask_texture", RenderMethodExtern.none, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp); // rmExtern - texture_global_target_shadow_mask
            rmopName = @"shaders\shader_options\global_shader_options";

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
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
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
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\bump_default";
                        break;
                    case Bump_Mapping.Detail:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddFloatParameter("bump_detail_coefficient", 1.0f);
                        rmopName = @"shaders\shader_options\bump_detail";
                        break;
                    case Bump_Mapping.Detail_Blend:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map2", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddFloatWithColorParameter("blend_alpha", new ShaderColor(255, 255, 255, 255), 1.0f);
                        rmopName = @"shaders\shader_options\bump_detail_blend";
                        break;
                    case Bump_Mapping.Three_Detail_Blend:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map2", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map3", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddFloatWithColorParameter("blend_alpha", new ShaderColor(255, 255, 255, 255), 1.0f);
                        rmopName = @"shaders\shader_options\bump_three_detail_blend";
                        break;
                    case Bump_Mapping.Standard_Wrinkle:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("wrinkle_normal", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("wrinkle_mask_a", @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddSamplerParameter("wrinkle_mask_b", @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddFloat4ColorExternParameter("wrinkle_weights_a", RenderMethodExtern.none); // rmExtern - wrinkle_weights_a
                        result.AddFloat4ColorExternParameter("wrinkle_weights_b", RenderMethodExtern.none); // rmExtern - wrinkle_weights_b
                        rmopName = @"shaders\shader_options\bump_default_wrinkle";
                        break;
                    case Bump_Mapping.Detail_Wrinkle:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("wrinkle_normal", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("wrinkle_mask_a", @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddSamplerParameter("wrinkle_mask_b", @"shaders\default_bitmaps\bitmaps\color_black_alpha_black");
                        result.AddFloat4ColorExternParameter("wrinkle_weights_a", RenderMethodExtern.none); // rmExtern - wrinkle_weights_a
                        result.AddFloat4ColorExternParameter("wrinkle_weights_b", RenderMethodExtern.none); // rmExtern - wrinkle_weights_b
                        rmopName = @"shaders\shader_options\bump_detail_wrinkle";
                        break;
                }
            }

            if (methodName == "material_model")
            {
                optionName = ((Material_Model)option).ToString();

                switch ((Material_Model)option)
                {
                    case Material_Model.Two_Lobe_Phong_Reach:
                        result.AddFloatParameter("diffuse_coefficient", 1.0f);
                        result.AddFloat3ColorParameter("specular_color_by_angle", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("specular_coefficient", 1.0f);
                        result.AddFloatParameter("normal_specular_power", 10.0f);
                        result.AddFloat3ColorParameter("normal_specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("glancing_specular_power", 10.0f);
                        result.AddFloat3ColorParameter("glancing_specular_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness", 5.0f);
                        result.AddFloatParameter("analytical_specular_contribution", 0.5f);
                        result.AddFloatParameter("environment_map_specular_contribution");
                        result.AddFloatParameter("analytical_roughness", 0.5f);
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("analytical_power", 25.0f);
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
                        result.AddSamplerAddressWithColorParameter("environment_map", ShaderOptionParameter.ShaderAddressMode.Clamp, new ShaderColor(0, 255, 255, 255), @"shaders\default_bitmaps\bitmaps\default_dynamic_cube_map");
                        result.AddFloat3ColorParameter("env_tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("env_roughness_scale", 1.0f);
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.Dynamic:
                        result.AddFloat3ColorParameter("env_tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddSamplerExternAddressParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddSamplerExternAddressParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddFloatParameter("env_roughness_scale", 1.0f);
                        //result.AddFloatParameter("env_roughness_offset", 0.5f); // Breaks shader recompilation
                        rmopName = @"shaders\shader_options\env_map_dynamic";
                        break;
                    case Environment_Mapping.From_Flat_Texture:
                        result.AddSamplerAddressWithColorParameter("flat_environment_map", ShaderOptionParameter.ShaderAddressMode.BlackBorder, new ShaderColor(0, 255, 255, 255), @"shaders\default_bitmaps\bitmaps\color_red");
                        result.AddFloat3ColorParameter("env_tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddFloat4ColorExternParameter("flat_envmap_matrix_x", RenderMethodExtern.flat_envmap_matrix_x);
                        result.AddFloat4ColorExternParameter("flat_envmap_matrix_y", RenderMethodExtern.flat_envmap_matrix_y);
                        result.AddFloat4ColorExternParameter("flat_envmap_matrix_z", RenderMethodExtern.flat_envmap_matrix_z);
                        result.AddFloatParameter("hemisphere_percentage", 1.0f);
                        result.AddFloat4ColorParameter("env_bloom_override", new ShaderColor(255, 0, 0, 0));
                        result.AddFloatParameter("env_bloom_override_intensity", 1.0f);
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
                        result.AddFloat3ColorParameter("wet_material_dim_tint", new ShaderColor(0, 216, 216, 235));
                        result.AddFloatWithColorParameter("wet_material_dim_coefficient", new ShaderColor(0, 149, 149, 149), 1.0f);
                        rmopName = @"shaders\wetness_options\wetness_simple";
                        break;
                    case Wetness.Flood:
                        result.AddFloatWithColorParameter("wet_material_dim_coefficient", new ShaderColor(0, 149, 149, 149), 1.0f);
                        result.AddFloat3ColorParameter("wet_material_dim_tint", new ShaderColor(0, 216, 216, 235));
                        result.AddFloatParameter("wet_sheen_reflection_contribution", 0.3f);
                        result.AddFloat3ColorParameter("wet_sheen_reflection_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("wet_sheen_thickness", 0.9f);
                        result.AddSamplerParameter("wet_flood_slope_map", @"rasterizer\water\static_wave\static_wave_slope_water");
                        result.AddSamplerFilterParameter("wet_noise_boundary_map", ShaderOptionParameter.ShaderFilterMode.Bilinear, @"rasterizer\rain\rain_noise_boundary");
                        result.AddFloatParameter("specular_mask_tweak_weight", 0.5f);
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
                        result.AddFloatParameter("opacity_fresnel_curve_steepness", 3.0f);
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_albedo_alpha";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha:
                        result.AddSamplerParameter("opacity_texture", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness", 3.0f);
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_opacity_map";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Rgb:
                        result.AddSamplerParameter("opacity_texture", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness", 3.0f);
                        result.AddFloatParameter("opacity_fresnel_curve_bias");
                        rmopName = @"shaders\shader_options\blend_source_from_opacity_map";
                        break;
                    case Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha:
                        result.AddSamplerParameter("opacity_texture", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddFloatParameter("opacity_fresnel_coefficient");
                        result.AddFloatParameter("opacity_fresnel_curve_steepness", 3.0f);
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

        public Array GetEntryPointOrder()
        {
            return new ShaderStage[]
            {
                ShaderStage.Static_Sh,
                ShaderStage.Static_Per_Vertex,
                ShaderStage.Static_Prt_Ambient,
                //ShaderStage.Imposter_Static_Sh,
                //ShaderStage.Imposter_Static_Prt_Ambient
            };
        }

        public Array GetVertexTypeOrder()
        {
            return new VertexType[]
            {
                VertexType.World,
                VertexType.Rigid,
                VertexType.Skinned
            };
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
                    case Material_Model.Two_Lobe_Phong_Reach:
                        vertexFunction = "invalid";
                        pixelFunction = "two_lobe_phong_reach";
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
    }
}
