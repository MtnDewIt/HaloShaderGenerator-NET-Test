using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Foliage
{
    public class FoliageGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(FoliageMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (FoliageMethods)methodIndex switch
            {
                FoliageMethods.Albedo => Enum.GetValues(typeof(Albedo)).Length,
                FoliageMethods.Alpha_Test => Enum.GetValues(typeof(Alpha_Test)).Length,
                FoliageMethods.Material_Model => Enum.GetValues(typeof(Material_Model)).Length,
                FoliageMethods.Wetness => Enum.GetValues(typeof(Wetness)).Length,
                _ => -1,
            };
        }

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint)
        {
            return entryPoint switch
            {
                ShaderStage.Shadow_Generate => 1,
                _ => -1,
            };
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            return entryPoint switch
            {
                ShaderStage.Shadow_Generate => true,
                _ => false,
            };
        }

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

            if (methodName == "albedo")
            {
                optionName = ((Albedo)option).ToString();

                switch ((Albedo)option)
                {
                    case Albedo.Simple:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        rmopName = @"shaders\shader_options\albedo_simple";
                        break;
                    case Albedo.Default:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        rmopName = @"shaders\shader_options\albedo_default";
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
                        result.AddSamplerParameter("alpha_test_map", @"shaders\default_bitmaps\bitmaps\default_alpha_test");
                        rmopName = @"shaders\shader_options\alpha_test_on";
                        break;
                    case Alpha_Test.From_Albedo_Alpha:
                        rmopName = @"shaders\shader_options\alpha_test_off";
                        break;
                    case Alpha_Test.From_Texture:
                        result.AddSamplerParameter("alpha_test_map", @"shaders\default_bitmaps\bitmaps\default_alpha_test");
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
                        result.AddFloatExternParameter("g_tree_animation_coeff", RenderMethodExtern.tree_animation_timer);
                        result.AddFloatParameter("animation_amplitude_horizontal", 0.04f);
                        rmopName = @"shaders\foliage_options\material_default";
                        break;
                    case Material_Model.Flat:
                        result.AddFloatExternParameter("g_tree_animation_coeff", RenderMethodExtern.tree_animation_timer);
                        result.AddFloatParameter("animation_amplitude_horizontal", 0.04f);
                        rmopName = @"shaders\foliage_options\material_flat";
                        break;
                    case Material_Model.Specular:
                        result.AddFloatExternParameter("g_tree_animation_coeff", RenderMethodExtern.tree_animation_timer);
                        result.AddFloatParameter("animation_amplitude_horizontal", 0.04f);
                        result.AddFloatParameter("foliage_translucency", 0.3f);
                        result.AddFloat3ColorParameter("foliage_specular_color", new ShaderColor(1, 255, 255, 255));
                        result.AddFloatParameter("foliage_specular_intensity", 1.0f);
                        result.AddFloatParameter("foliage_specular_power", 4.0f);
                        rmopName = @"shaders\foliage_options\material_specular";
                        break;
                    case Material_Model.Translucent:
                        result.AddFloatExternParameter("g_tree_animation_coeff", RenderMethodExtern.tree_animation_timer);
                        result.AddFloatParameter("animation_amplitude_horizontal", 0.04f);
                        result.AddFloatParameter("foliage_translucency", 0.3f);
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
                        result.AddFloat3ColorParameter("wet_material_dim_tint", new ShaderColor(0, 216, 216, 235));
                        result.AddFloatWithColorParameter("wet_material_dim_coefficient", new ShaderColor(0, 149, 149, 149), 1.0f);
                        rmopName = @"shaders\wetness_options\wetness_simple";
                        break;
                    case Wetness.Proof:
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames() => Enum.GetValues(typeof(FoliageMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (FoliageMethods)methodIndex switch
            {
                FoliageMethods.Albedo => Enum.GetValues(typeof(Albedo)),
                FoliageMethods.Alpha_Test => Enum.GetValues(typeof(Alpha_Test)),
                FoliageMethods.Material_Model => Enum.GetValues(typeof(Material_Model)),
                FoliageMethods.Wetness => Enum.GetValues(typeof(Wetness)),
                _ => null,
            };
        }

        public Array GetEntryPointOrder()
        {
            return new ShaderStage[]
            {
                ShaderStage.Albedo,
                ShaderStage.Static_Per_Pixel,
                ShaderStage.Static_Sh,
                ShaderStage.Static_Per_Vertex,
                ShaderStage.Shadow_Generate,
                ShaderStage.Static_Prt_Ambient,
                ShaderStage.Static_Prt_Linear,
                ShaderStage.Static_Prt_Quadratic
                //ShaderStage.Stipple,
                //ShaderStage.Single_Pass_Per_Pixel,
                //ShaderStage.Single_Pass_Per_Vertex,
                //ShaderStage.Single_Pass_Single_Probe,
                //ShaderStage.Single_Pass_Single_Probe_Ambient,
                //ShaderStage.Imposter_Static_Sh,
                //ShaderStage.Imposter_Static_Prt_Ambient,
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
            return (FoliageMethods)category switch
            {
                FoliageMethods.Albedo => "calc_albedo_ps",
                FoliageMethods.Alpha_Test => "calc_alpha_test_ps",
                FoliageMethods.Material_Model => "calculate_material",
                FoliageMethods.Wetness => "calc_wetness_ps",
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (FoliageMethods)category switch
            {
                FoliageMethods.Albedo => "calc_albedo_vs",
                FoliageMethods.Alpha_Test => string.Empty,
                FoliageMethods.Material_Model => string.Empty,
                FoliageMethods.Wetness => string.Empty,
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (FoliageMethods)category switch
            {
                FoliageMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Simple => "calc_albedo_simple_ps",
                    Albedo.Default => "calc_albedo_default_ps",
                    _ => null,
                },
                FoliageMethods.Alpha_Test => (Alpha_Test)option switch
                {
                    Alpha_Test.None => "calc_alpha_test_off_ps",
                    Alpha_Test.Simple => "calc_alpha_test_on_ps",
                    Alpha_Test.From_Albedo_Alpha => "calc_alpha_test_from_albedo_ps",
                    Alpha_Test.From_Texture => "calc_alpha_test_texture_ps",
                    _ => null,
                },
                FoliageMethods.Material_Model => (Material_Model)option switch
                {
                    Material_Model.Default => "calculate_material_default",
                    Material_Model.Flat => "flat",
                    Material_Model.Specular => "specular",
                    Material_Model.Translucent => "translucent",
                    _ => null,
                },
                FoliageMethods.Wetness => (Wetness)option switch
                {
                    Wetness.Simple => "calc_wetness_simple_ps",
                    Wetness.Proof => "calc_wetness_proof_ps",
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (FoliageMethods)category switch
            {
                FoliageMethods.Albedo => (Albedo)option switch
                {
                    Albedo.Simple => "calc_albedo_simple_vs",
                    Albedo.Default => "calc_albedo_default_vs",
                    _ => null,
                },
                FoliageMethods.Alpha_Test => (Alpha_Test)option switch
                {
                    Alpha_Test.None => string.Empty,
                    Alpha_Test.Simple => string.Empty,
                    Alpha_Test.From_Albedo_Alpha => string.Empty,
                    Alpha_Test.From_Texture => string.Empty,
                    _ => null,
                },
                FoliageMethods.Material_Model => (Material_Model)option switch
                {
                    Material_Model.Default => string.Empty,
                    Material_Model.Flat => string.Empty,
                    Material_Model.Specular => string.Empty,
                    Material_Model.Translucent => string.Empty,
                    _ => null,
                },
                FoliageMethods.Wetness => (Wetness)option switch
                {
                    Wetness.Simple => string.Empty,
                    Wetness.Proof => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
