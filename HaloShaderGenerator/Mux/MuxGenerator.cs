using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Mux
{
    public class MuxGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(MuxMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (MuxMethods)methodIndex switch
            {
                MuxMethods.Blending => Enum.GetValues(typeof(Blending)).Length,
                MuxMethods.Albedo => Enum.GetValues(typeof(Albedo)).Length,
                MuxMethods.Bump => Enum.GetValues(typeof(Bump)).Length,
                MuxMethods.Materials => Enum.GetValues(typeof(Materials)).Length,
                MuxMethods.Environment_Mapping => Enum.GetValues(typeof(Environment_Mapping)).Length,
                MuxMethods.Parallax => Enum.GetValues(typeof(Parallax)).Length,
                MuxMethods.Wetness => Enum.GetValues(typeof(Wetness)).Length,
                _ => -1,
            };
        }

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint) => -1;

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint) => false;

        public bool IsPixelShaderShared(ShaderStage entryPoint)
        {
            return entryPoint switch
            {
                ShaderStage.Shadow_Generate => true,
                _ => false,
            };
        }

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

            if (methodName == "blending")
            {
                optionName = ((Blending)option).ToString();

                switch ((Blending)option)
                {
                    case Blending.Standard:
                        result.AddSamplerWithScaleParameter("material_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatParameter("blend_material_scale", 1.0f);
                        result.AddFloatParameter("blend_material_offset");
                        result.AddFloatParameter("pc_atlas_scale_x", 1.0f);
                        result.AddFloatParameter("pc_atlas_scale_y", 1.0f);
                        result.AddFloatParameter("pc_atlas_transform_x", 1.0f);
                        result.AddFloatParameter("pc_atlas_transform_y", 1.0f);
                        result.AddFloatParameter("blend_material_count");
                        rmopName = @"shaders\shader_options\mux_blend_standard";
                        break;
                }
            }

            if (methodName == "albedo")
            {
                optionName = ((Albedo)option).ToString();

                switch ((Albedo)option)
                {
                    case Albedo.Base_Only:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\mux_albedo_base_only";
                        break;
                    case Albedo.Base_And_Detail:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\mux_albedo";
                        break;
                }
            }

            if (methodName == "bump")
            {
                optionName = ((Bump)option).ToString();

                switch ((Bump)option)
                {
                    case Bump.Base_Only:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\bump_default";
                        break;
                    case Bump.Base_And_Detail:
                        result.AddSamplerParameter("bump_map", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithScaleParameter("bump_detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddFloatParameter("bump_detail_coefficient", 1.0f);
                        rmopName = @"shaders\shader_options\bump_detail";
                        break;
                }
            }

            if (methodName == "materials")
            {
                optionName = ((Materials)option).ToString();

                switch ((Materials)option)
                {
                    case Materials.Diffuse_Only:
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("approximate_specular_type");
                        rmopName = @"shaders\shader_options\material_diffuse_only";
                        break;
                    case Materials.Single_Lobe_Phong:
                        result.AddSamplerFilterWithFloatParameter("material_property0_map", ShaderOptionParameter.ShaderFilterMode.Bilinear, 1.0f, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerParameter("material_property1_map", @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddBooleanParameter("no_dynamic_lights");
                        rmopName = @"shaders\shader_options\mux_single_lobe_phong";
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
                }
            }

            if (methodName == "parallax")
            {
                optionName = ((Parallax)option).ToString();

                switch ((Parallax)option)
                {
                    case Parallax.Off:
                        break;
                    case Parallax.Simple:
                        result.AddSamplerParameter("height_map", @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        result.AddFloatParameter("height_scale", 0.1f);
                        rmopName = @"shaders\shader_options\parallax_simple";
                        break;
                }
            }

            if (methodName == "wetness")
            {
                optionName = ((Wetness)option).ToString();

                switch ((Wetness)option)
                {
                    case Wetness.Default:
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
                    case Wetness.Proof:
                        break;
                    case Wetness.Ripples:
                        result.AddFloatWithColorParameter("wet_material_dim_coefficient", new ShaderColor(0, 149, 149, 149), 1.0f);
                        result.AddFloat3ColorParameter("wet_material_dim_tint", new ShaderColor(0, 216, 216, 235));
                        result.AddFloatParameter("wet_sheen_reflection_contribution", 0.37f);
                        result.AddFloat3ColorParameter("wet_sheen_reflection_tint", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("wet_sheen_thickness", 0.4f);
                        result.AddSamplerFilterParameter("wet_noise_boundary_map", ShaderOptionParameter.ShaderFilterMode.Bilinear, @"rasterizer\rain\rain_noise_boundary");
                        result.AddFloatParameter("specular_mask_tweak_weight", 0.5f);
                        result.AddFloatParameter("surface_tilt_tweak_weight", 0.3f);
                        rmopName = @"shaders\wetness_options\wetness_ripples";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames() => Enum.GetValues(typeof(MuxMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (MuxMethods)methodIndex switch
            {
                MuxMethods.Blending => Enum.GetValues(typeof(Blending)),
                MuxMethods.Albedo => Enum.GetValues(typeof(Albedo)),
                MuxMethods.Bump => Enum.GetValues(typeof(Bump)),
                MuxMethods.Materials => Enum.GetValues(typeof(Materials)),
                MuxMethods.Environment_Mapping => Enum.GetValues(typeof(Environment_Mapping)),
                MuxMethods.Parallax => Enum.GetValues(typeof(Parallax)),
                MuxMethods.Wetness => Enum.GetValues(typeof(Wetness)),
                _ => null,
            };
        }

        public Array GetEntryPointOrder()
        {
            return new ShaderStage[]
            {
                ShaderStage.Albedo,
                ShaderStage.Static_Per_Pixel,
                ShaderStage.Static_Per_Vertex,
                ShaderStage.Static_Sh,
                ShaderStage.Dynamic_Light,
                ShaderStage.Lightmap_Debug_Mode,
                ShaderStage.Shadow_Generate,
                ShaderStage.Dynamic_Light_Cinematic
                //case ShaderStage.Imposter_Static_Prt_Ambient
            };
        }

        public Array GetVertexTypeOrder()
        {
            return new VertexType[]
            {
                VertexType.World,
                VertexType.Rigid,
                VertexType.Skinned,
            };
        }

        public string GetCategoryPixelFunction(int category)
        {
            return (MuxMethods)category switch
            {
                MuxMethods.Blending => "material_blend",
                MuxMethods.Albedo => "calc_albedo_ps",
                MuxMethods.Bump => "calc_bumpmap_ps",
                MuxMethods.Materials => "material_type",
                MuxMethods.Environment_Mapping => "envmap_type",
                MuxMethods.Parallax => "calc_parallax_ps",
                MuxMethods.Wetness => "calc_wetness_ps",
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (MuxMethods)category switch
            {
                MuxMethods.Blending => string.Empty,
                MuxMethods.Albedo => string.Empty,
                MuxMethods.Bump => string.Empty,
                MuxMethods.Materials => string.Empty,
                MuxMethods.Environment_Mapping => string.Empty,
                MuxMethods.Parallax => string.Empty,
                MuxMethods.Wetness => string.Empty,
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (MuxMethods)category switch
            {
                MuxMethods.Blending => (Blending)option switch
                {
                    Blending.Standard => "standard",
                    _ => null,
                },
                MuxMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Base_Only => "calc_albedo_base_ps",
                    Albedo.Base_And_Detail => "calc_albedo_detail_ps",
                    _ => null,
                },
                MuxMethods.Bump => (Bump)option switch
                {
                    Bump.Base_Only => "calc_bumpmap_default_ps",
                    Bump.Base_And_Detail => "calc_bumpmap_detail_ps",
                    _ => null,
                },
                MuxMethods.Materials => (Materials)option switch
                {
                    Materials.Diffuse_Only => "diffuse_only",
                    Materials.Single_Lobe_Phong => "single_lobe_phong",
                    _ => null,
                },
                MuxMethods.Environment_Mapping => (Environment_Mapping)option switch
                {
                    Environment_Mapping.None => "none",
                    Environment_Mapping.Per_Pixel => "per_pixel",
                    Environment_Mapping.Dynamic => "dynamic",
                    _ => null,
                },
                MuxMethods.Parallax => (Parallax)option switch
                {
                    Parallax.Off => "calc_parallax_off_ps",
                    Parallax.Simple => "calc_parallax_simple_ps",
                    _ => null,
                },
                MuxMethods.Wetness => (Wetness)option switch
                {
                    Wetness.Default => "calc_wetness_default_ps",
                    Wetness.Flood => "calc_wetness_flood_ps",
                    Wetness.Proof => "calc_wetness_proof_ps",
                    Wetness.Ripples => "calc_wetness_ripples_ps",
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (MuxMethods)category switch
            {
                MuxMethods.Blending => (Blending)option switch
                {
                    Blending.Standard => string.Empty,
                    _ => null,
                },
                MuxMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Base_Only => string.Empty,
                    Albedo.Base_And_Detail => string.Empty,
                    _ => null,
                },
                MuxMethods.Bump => (Bump)option switch
                {
                    Bump.Base_Only => string.Empty,
                    Bump.Base_And_Detail => string.Empty,
                    _ => null,
                },
                MuxMethods.Materials => (Materials)option switch
                {
                    Materials.Diffuse_Only => string.Empty,
                    Materials.Single_Lobe_Phong => string.Empty,
                    _ => null,
                },
                MuxMethods.Environment_Mapping => (Environment_Mapping)option switch
                {
                    Environment_Mapping.None => string.Empty,
                    Environment_Mapping.Per_Pixel => string.Empty,
                    Environment_Mapping.Dynamic => string.Empty,
                    _ => null,
                },
                MuxMethods.Parallax => (Parallax)option switch
                {
                    Parallax.Off => string.Empty,
                    Parallax.Simple => string.Empty,
                    _ => null,
                },
                MuxMethods.Wetness => (Wetness)option switch
                {
                    Wetness.Default => string.Empty,
                    Wetness.Flood => string.Empty,
                    Wetness.Proof => string.Empty,
                    Wetness.Ripples => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
