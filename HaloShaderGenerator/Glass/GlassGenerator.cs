using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Glass
{
    public class GlassGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(GlassMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (GlassMethods)methodIndex switch
            {
                GlassMethods.Albedo => Enum.GetValues(typeof(Albedo)).Length,
                GlassMethods.Bump_Mapping => Enum.GetValues(typeof(Bump_Mapping)).Length,
                GlassMethods.Material_Model => Enum.GetValues(typeof(Material_Model)).Length,
                GlassMethods.Environment_Mapping => Enum.GetValues(typeof(Environment_Mapping)).Length,
                GlassMethods.Wetness => Enum.GetValues(typeof(Wetness)).Length,
                GlassMethods.Alpha_Blend_Source => Enum.GetValues(typeof(Alpha_Blend_Source)).Length,
                _ => -1,
            };
        }

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint) => -1;

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint) => false;

        public bool IsPixelShaderShared(ShaderStage entryPoint) => false;

        public bool IsAutoMacro() => false;

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
            //result.AddSamplerFilterAddressParameter("g_direction_lut", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\direction_lut_1002");
            //result.AddSamplerFilterAddressParameter("g_sample_vmf_diffuse", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\diffusetable");
            //result.AddSamplerFilterAddressParameter("g_sample_vmf_diffuse_vs", ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, @"rasterizer\diffusetable");
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
                        result.AddFloatParameter("env_roughness_offset", 0.5f);
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

        public Array GetMethodNames() => Enum.GetValues(typeof(GlassMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (GlassMethods)methodIndex switch
            {
                GlassMethods.Albedo => Enum.GetValues(typeof(Albedo)),
                GlassMethods.Bump_Mapping => Enum.GetValues(typeof(Bump_Mapping)),
                GlassMethods.Material_Model => Enum.GetValues(typeof(Material_Model)),
                GlassMethods.Environment_Mapping => Enum.GetValues(typeof(Environment_Mapping)),
                GlassMethods.Wetness => Enum.GetValues(typeof(Wetness)),
                GlassMethods.Alpha_Blend_Source => Enum.GetValues(typeof(Alpha_Blend_Source)),
                _ => null,
            };
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

        public string GetCategoryPixelFunction(int category)
        {
            return (GlassMethods)category switch
            {
                GlassMethods.Albedo => "calc_albedo_ps",
                GlassMethods.Bump_Mapping => "calc_bumpmap_ps",
                GlassMethods.Material_Model => "material_type",
                GlassMethods.Environment_Mapping => "envmap_type",
                GlassMethods.Wetness => "calc_wetness_ps",
                GlassMethods.Alpha_Blend_Source => "alpha_blend_source",
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (GlassMethods)category switch
            {
                GlassMethods.Albedo => "calc_albedo_vs",
                GlassMethods.Bump_Mapping => "calc_bumpmap_vs",
                GlassMethods.Material_Model => string.Empty,
                GlassMethods.Environment_Mapping => string.Empty,
                GlassMethods.Wetness => string.Empty,
                GlassMethods.Alpha_Blend_Source => string.Empty,
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (GlassMethods)category switch
            {
                GlassMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Map => "calc_albedo_default_ps",
                    _ => null,
                },
                GlassMethods.Bump_Mapping => (Bump_Mapping)option switch
                {
                    Bump_Mapping.Off => "calc_bumpmap_off_ps",
                    Bump_Mapping.Standard => "calc_bumpmap_default_ps",
                    Bump_Mapping.Detail => "calc_bumpmap_detail_ps",
                    Bump_Mapping.Detail_Blend => "calc_bumpmap_detail_blend_ps",
                    Bump_Mapping.Three_Detail_Blend => "calc_bumpmap_three_detail_blend_ps",
                    Bump_Mapping.Standard_Wrinkle => "calc_bumpmap_default_wrinkle_ps",
                    Bump_Mapping.Detail_Wrinkle => "calc_bumpmap_detail_wrinkle_ps",
                    _ => null,
                },
                GlassMethods.Material_Model => (Material_Model)option switch
                {
                    Material_Model.Two_Lobe_Phong_Reach => "two_lobe_phong_reach",
                    _ => null,
                },
                GlassMethods.Environment_Mapping => (Environment_Mapping)option switch
                {
                    Environment_Mapping.None => "none",
                    Environment_Mapping.Per_Pixel => "per_pixel",
                    Environment_Mapping.Dynamic => "dynamic",
                    Environment_Mapping.From_Flat_Texture => "from_flat_texture",
                    _ => null,
                },
                GlassMethods.Wetness => (Wetness)option switch
                {
                    Wetness.Simple => "calc_wetness_simple_ps",
                    Wetness.Flood => "calc_wetness_flood_ps",
                    _ => null,
                },
                GlassMethods.Alpha_Blend_Source => (Alpha_Blend_Source)option switch
                {
                    Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel => "albedo_alpha_without_fresnel",
                    Alpha_Blend_Source.From_Albedo_Alpha => "albedo_alpha",
                    Alpha_Blend_Source.From_Opacity_Map_Alpha => "opacity_map_alpha",
                    Alpha_Blend_Source.From_Opacity_Map_Rgb => "opacity_map_rgb",
                    Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha => "opacity_map_alpha_and_albedo_alpha",
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (GlassMethods)category switch
            {
                GlassMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Map => "calc_albedo_default_vs",
                    _ => null,
                },
                GlassMethods.Bump_Mapping => (Bump_Mapping)option switch
                {
                    Bump_Mapping.Off => "calc_bumpmap_off_vs",
                    Bump_Mapping.Standard => "calc_bumpmap_default_vs",
                    Bump_Mapping.Detail => "calc_bumpmap_detail_vs",
                    Bump_Mapping.Detail_Blend => "calc_bumpmap_detail_blend_vs",
                    Bump_Mapping.Three_Detail_Blend => "calc_bumpmap_three_detail_blend_vs",
                    Bump_Mapping.Standard_Wrinkle => "calc_bumpmap_default_wrinkle_vs",
                    Bump_Mapping.Detail_Wrinkle => "calc_bumpmap_detail_wrinkle_vs",
                    _ => null,
                },
                GlassMethods.Material_Model => (Material_Model)option switch
                {
                    Material_Model.Two_Lobe_Phong_Reach => string.Empty,
                    _ => null,
                },
                GlassMethods.Environment_Mapping => (Environment_Mapping)option switch
                {
                    Environment_Mapping.None => string.Empty,
                    Environment_Mapping.Per_Pixel => string.Empty,
                    Environment_Mapping.Dynamic => string.Empty,
                    Environment_Mapping.From_Flat_Texture => string.Empty,
                    _ => null,
                },
                GlassMethods.Wetness => (Wetness)option switch
                {
                    Wetness.Simple => string.Empty,
                    Wetness.Flood => string.Empty,
                    _ => null,
                },
                GlassMethods.Alpha_Blend_Source => (Alpha_Blend_Source)option switch
                {
                    Alpha_Blend_Source.From_Albedo_Alpha_Without_Fresnel => string.Empty,
                    Alpha_Blend_Source.From_Albedo_Alpha => string.Empty,
                    Alpha_Blend_Source.From_Opacity_Map_Alpha => string.Empty,
                    Alpha_Blend_Source.From_Opacity_Map_Rgb => string.Empty,
                    Alpha_Blend_Source.From_Opacity_Map_Alpha_And_Albedo_Alpha => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
