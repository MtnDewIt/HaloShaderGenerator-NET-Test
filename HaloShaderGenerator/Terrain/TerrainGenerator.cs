using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.Terrain
{
    public class TerrainGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(TerrainMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (TerrainMethods)methodIndex switch
            {
                TerrainMethods.Blending => Enum.GetValues(typeof(Blending)).Length,
                TerrainMethods.Environment_Map => Enum.GetValues(typeof(Environment_Map)).Length,
                TerrainMethods.Material_0 => Enum.GetValues(typeof(Material_0)).Length,
                TerrainMethods.Material_1 => Enum.GetValues(typeof(Material_1)).Length,
                TerrainMethods.Material_2 => Enum.GetValues(typeof(Material_2)).Length,
                TerrainMethods.Material_3 => Enum.GetValues(typeof(Material_3)).Length,
                TerrainMethods.Wetness => Enum.GetValues(typeof(Wetness)).Length,
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
                    case Blending.Morph:
                        result.AddSamplerWithFloatAndColorParameter("blend_map", 1.0f, new ShaderColor(255, 255, 255, 255), @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatWithColorParameter("global_albedo_tint", new ShaderColor(255, 255, 255, 255), 1.0f);
                        rmopName = @"shaders\terrain_options\default_blending";
                        break;
                    case Blending.Dynamic_Morph:
                        result.AddSamplerWithFloatAndColorParameter("blend_map", 1.0f, new ShaderColor(255, 255, 255, 255), @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatWithColorParameter("global_albedo_tint", new ShaderColor(255, 255, 255, 255), 1.0f);
                        result.AddFloat4ColorParameter("dynamic_material");
                        result.AddFloatParameter("transition_sharpness", 4f);
                        result.AddFloatParameter("transition_threshold", 1.0f);
                        rmopName = @"shaders\terrain_options\dynamic_blending";
                        break;
                    case Blending.Distance_Blend_Base:                        
                        result.AddSamplerWithFloatAndColorParameter("blend_map", 1.0f, new ShaderColor(255, 255, 255, 255), @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddFloatWithColorParameter("global_albedo_tint", new ShaderColor(255, 255, 255, 255), 1.0f);
                        result.AddFloat4ColorParameter("blend_target_0", new ShaderColor(0, 188, 188, 188));
                        result.AddFloat4ColorParameter("blend_target_1", new ShaderColor(0, 188, 188, 188));
                        result.AddFloat4ColorParameter("blend_target_2", new ShaderColor(0, 188, 188, 188));
                        result.AddFloat4ColorParameter("blend_target_3", new ShaderColor(0, 188, 188, 188));
                        result.AddFloatParameter("blend_offset", -1.0f);
                        result.AddFloatParameter("blend_slope", 0.1f);
                        result.AddFloatParameter("blend_max_0", 0.8f);
                        result.AddFloatParameter("blend_max_1", 0.8f);
                        result.AddFloatParameter("blend_max_2", 0.8f);
                        result.AddFloatParameter("blend_max_3", 0.8f);
                        rmopName = @"shaders\terrain_options\distance_blend_base";
                        break;
                }
            }

            if (methodName == "environment_map")
            {
                optionName = ((Environment_Map)option).ToString();

                switch ((Environment_Map)option)
                {
                    case Environment_Map.None:
                        break;
                    case Environment_Map.Per_Pixel:
                        result.AddSamplerAddressWithColorParameter("environment_map", ShaderOptionParameter.ShaderAddressMode.Clamp, new ShaderColor(0, 255, 255, 255), @"shaders\default_bitmaps\bitmaps\default_dynamic_cube_map");
                        result.AddFloat3ColorParameter("env_tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("env_roughness_scale", 1.0f);
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Map.Dynamic:
                        result.AddFloat3ColorParameter("env_tint_color", new ShaderColor(0, 255, 255, 255));
                        result.AddSamplerExternAddressParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddSamplerExternAddressParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddFloatParameter("env_roughness_scale", 1.0f);
                        result.AddFloatParameter("env_roughness_offset", 0.5f);
                        rmopName = @"shaders\shader_options\env_map_dynamic";
                        break;
                    case Environment_Map.Dynamic_Reach:
                        break;
                }
            }

            if (methodName == "material_0")
            {
                optionName = ((Material_0)option).ToString();

                switch ((Material_0)option)
                {
                    case Material_0.Diffuse_Only:
                        result.AddSamplerParameter("base_map_m_0", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_m_0", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("bump_map_m_0", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_0", @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_0";
                        break;
                    case Material_0.Diffuse_Plus_Specular:
                        result.AddSamplerParameter("base_map_m_0", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_m_0", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("bump_map_m_0", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithFloatParameter("detail_bump_m_0", 1.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddFloatParameter("diffuse_coefficient_m_0", 1.0f);
                        result.AddFloatParameter("specular_coefficient_m_0");
                        result.AddFloatParameter("specular_power_m_0", 10.0f);
                        result.AddFloat3ColorParameter("specular_tint_m_0", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness_m_0", 5.0f);
                        result.AddFloatParameter("area_specular_contribution_m_0", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution_m_0", 0.5f);
                        result.AddFloatParameter("environment_specular_contribution_m_0");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_0");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_m_0";
                        break;
                    case Material_0.Off:
                        break;
                    case Material_0.Diffuse_Only_Plus_Self_Illum:
                        result.AddSamplerParameter("base_map_m_0", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_0", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("self_illum_map_m_0", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("self_illum_detail_map_m_0", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat3ColorParameter("self_illum_color_m_0", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity_m_0", 1.0f);
                        rmopName = @"shaders\terrain_options\diffuse_only_plus_sefl_illum_m_0";
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Self_Illum:
                        result.AddSamplerParameter("base_map_m_0", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_0", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("self_illum_map_m_0", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("self_illum_detail_map_m_0", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat3ColorParameter("self_illum_color_m_0", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity_m_0", 1.0f);
                        result.AddFloatParameter("diffuse_coefficient_m_0", 1.0f);
                        result.AddFloatParameter("specular_coefficient_m_0");
                        result.AddFloatParameter("specular_power_m_0", 10.0f);
                        result.AddFloat3ColorParameter("specular_tint_m_0", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness_m_0", 5.0f);
                        result.AddFloatParameter("area_specular_contribution_m_0", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution_m_0", 0.5f);
                        result.AddFloatParameter("environment_specular_contribution_m_0");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_0");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_self_illumm_0";
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Heightmap:
                        result.AddSamplerParameter("base_map_m_0", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_m_0", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("bump_map_m_0", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithFloatParameter("detail_bump_m_0", 1.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("heightmap_m_0", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddBooleanParameter("heightmap_invert_m_0");
                        result.AddFloatParameter("diffuse_coefficient_m_0", 1.0f);
                        result.AddFloatParameter("specular_coefficient_m_0");
                        result.AddFloatParameter("specular_power_m_0", 10.0f);
                        result.AddFloat3ColorParameter("specular_tint_m_0", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness_m_0", 5.0f);
                        result.AddFloatParameter("area_specular_contribution_m_0", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution_m_0", 0.5f);
                        result.AddFloatParameter("environment_specular_contribution_m_0");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_0");
                        result.AddFloatParameter("smooth_zone_m_0", 10.0f);
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_heightmap_m_0";
                        break;
                    case Material_0.Diffuse_Plus_Two_Detail:
                        result.AddSamplerParameter("base_map_m_0", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_m_0", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map2_m_0", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("bump_map_m_0", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_0", @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\terrain_options\diffuse_plus_two_detail_m_0";
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap:
                        result.AddSamplerParameter("base_map_m_0", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_m_0", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("bump_map_m_0", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithFloatParameter("detail_bump_m_0", 1.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("heightmap_m_0", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddBooleanParameter("heightmap_invert_m_0");
                        result.AddFloatParameter("diffuse_coefficient_m_0", 1.0f);
                        result.AddFloatParameter("specular_coefficient_m_0");
                        result.AddFloatParameter("specular_power_m_0", 10.0f);
                        result.AddFloat3ColorParameter("specular_tint_m_0", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness_m_0", 5.0f);
                        result.AddFloatParameter("area_specular_contribution_m_0", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution_m_0", 0.5f);
                        result.AddFloatParameter("environment_specular_contribution_m_0");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_0");
                        result.AddFloatParameter("smooth_zone_m_0", 10.0f);
                        result.AddFloatParameter("up_vector_scale_m_0", 1.0f);
                        result.AddFloatParameter("up_vector_shift_m_0");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_up_vector_plus_heightmap_m_0";
                        break;
                }
            }

            if (methodName == "material_1")
            {
                optionName = ((Material_1)option).ToString();

                switch ((Material_1)option)
                {
                    case Material_1.Diffuse_Only:
                        result.AddSamplerParameter("base_map_m_1", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_m_1", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("bump_map_m_1", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_1", @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_1";
                        break;
                    case Material_1.Diffuse_Plus_Specular:
                        result.AddSamplerParameter("base_map_m_1", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_m_1", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("bump_map_m_1", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithFloatParameter("detail_bump_m_1", 1.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddFloatParameter("diffuse_coefficient_m_1", 1.0f);
                        result.AddFloatParameter("specular_coefficient_m_1");
                        result.AddFloatParameter("specular_power_m_1", 10.0f);
                        result.AddFloat3ColorParameter("specular_tint_m_1", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness_m_1", 5.0f);
                        result.AddFloatParameter("area_specular_contribution_m_1", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution_m_1", 0.5f);
                        result.AddFloatParameter("environment_specular_contribution_m_1");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_1");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_m_1";
                        break;
                    case Material_1.Off:
                        break;
                    case Material_1.Diffuse_Only_Plus_Self_Illum:
                        result.AddSamplerParameter("base_map_m_1", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_1", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("self_illum_map_m_1", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("self_illum_detail_map_m_1", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat3ColorParameter("self_illum_color_m_1", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity_m_1", 1.0f);
                        rmopName = @"shaders\terrain_options\diffuse_only_plus_sefl_illum_m_1";
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Self_Illum:
                        result.AddSamplerParameter("base_map_m_1", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_1", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("self_illum_map_m_1", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("self_illum_detail_map_m_1", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat3ColorParameter("self_illum_color_m_1", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity_m_1", 1.0f);
                        result.AddFloatParameter("diffuse_coefficient_m_1", 1.0f);
                        result.AddFloatParameter("specular_coefficient_m_1");
                        result.AddFloatParameter("specular_power_m_1", 10.0f);
                        result.AddFloat3ColorParameter("specular_tint_m_1", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness_m_1", 5.0f);
                        result.AddFloatParameter("area_specular_contribution_m_1", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution_m_1", 0.5f);
                        result.AddFloatParameter("environment_specular_contribution_m_1");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_1");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_self_illumm_1";
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Heightmap:
                        result.AddSamplerParameter("base_map_m_1", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_m_1", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("bump_map_m_1", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithFloatParameter("detail_bump_m_1", 1.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("heightmap_m_1", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddBooleanParameter("heightmap_invert_m_1");
                        result.AddFloatParameter("diffuse_coefficient_m_1", 1.0f);
                        result.AddFloatParameter("specular_coefficient_m_1");
                        result.AddFloatParameter("specular_power_m_1", 10.0f);
                        result.AddFloat3ColorParameter("specular_tint_m_1", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness_m_1", 5.0f);
                        result.AddFloatParameter("area_specular_contribution_m_1", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution_m_1", 0.5f);
                        result.AddFloatParameter("environment_specular_contribution_m_1");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_1");
                        result.AddFloatParameter("smooth_zone_m_1", 10.0f);
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_heightmap_m_1";
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap:
                        result.AddSamplerParameter("base_map_m_1", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_m_1", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("bump_map_m_1", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithFloatParameter("detail_bump_m_1", 1.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("heightmap_m_1", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddBooleanParameter("heightmap_invert_m_1");
                        result.AddFloatParameter("diffuse_coefficient_m_1", 1.0f);
                        result.AddFloatParameter("specular_coefficient_m_1");
                        result.AddFloatParameter("specular_power_m_1", 10.0f);
                        result.AddFloat3ColorParameter("specular_tint_m_1", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness_m_1", 5.0f);
                        result.AddFloatParameter("area_specular_contribution_m_1", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution_m_1", 0.5f);
                        result.AddFloatParameter("environment_specular_contribution_m_1");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_1");
                        result.AddFloatParameter("smooth_zone_m_1", 10.0f);
                        result.AddFloatParameter("up_vector_scale_m_1", 1.0f);
                        result.AddFloatParameter("up_vector_shift_m_1");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_up_vector_plus_heightmap_m_1";
                        break;
                }
            }

            if (methodName == "material_2")
            {
                optionName = ((Material_2)option).ToString();

                switch ((Material_2)option)
                {
                    case Material_2.Diffuse_Only:
                        result.AddSamplerParameter("base_map_m_2", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_m_2", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("bump_map_m_2", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_2", @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_2";
                        break;
                    case Material_2.Diffuse_Plus_Specular:
                        result.AddSamplerParameter("base_map_m_2", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_m_2", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("bump_map_m_2", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithFloatParameter("detail_bump_m_2", 1.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddFloatParameter("diffuse_coefficient_m_2", 1.0f);
                        result.AddFloatParameter("specular_coefficient_m_2");
                        result.AddFloatParameter("specular_power_m_2", 10.0f);
                        result.AddFloat3ColorParameter("specular_tint_m_2", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness_m_2", 5.0f);
                        result.AddFloatParameter("area_specular_contribution_m_2", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution_m_2", 0.5f);
                        result.AddFloatParameter("environment_specular_contribution_m_2");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_2");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_m_2";
                        break;
                    case Material_2.Off:
                        break;
                    case Material_2.Diffuse_Only_Plus_Self_Illum:
                        result.AddSamplerParameter("base_map_m_2", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_2", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("self_illum_map_m_2", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("self_illum_detail_map_m_2", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat3ColorParameter("self_illum_color_m_2", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity_m_2", 1.0f);
                        rmopName = @"shaders\terrain_options\diffuse_only_plus_sefl_illum_m_2";
                        break;
                    case Material_2.Diffuse_Plus_Specular_Plus_Self_Illum:
                        result.AddSamplerParameter("base_map_m_2", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_2", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("self_illum_map_m_2", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("self_illum_detail_map_m_2", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat3ColorParameter("self_illum_color_m_2", new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity_m_2", 1.0f);
                        result.AddFloatParameter("diffuse_coefficient_m_2", 1.0f);
                        result.AddFloatParameter("specular_coefficient_m_2");
                        result.AddFloatParameter("specular_power_m_2", 10.0f);
                        result.AddFloat3ColorParameter("specular_tint_m_2", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness_m_2", 5.0f);
                        result.AddFloatParameter("area_specular_contribution_m_2", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution_m_2", 0.5f);
                        result.AddFloatParameter("environment_specular_contribution_m_2");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_2");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_self_illumm_2";
                        break;
                }
            }

            if (methodName == "material_3")
            {
                optionName = ((Material_3)option).ToString();

                switch ((Material_3)option)
                {
                    case Material_3.Off:
                        break;
                    case Material_3.Diffuse_Only_Four_Material_Shaders_Disable_Detail_Bump:
                        result.AddSamplerParameter("base_map_m_3", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_m_3", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("bump_map_m_3", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_3", @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_3";
                        break;
                    case Material_3.Diffuse_Plus_Specular_Four_Material_Shaders_Disable_Detail_Bump:
                        result.AddSamplerParameter("base_map_m_3", @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_m_3", @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("bump_map_m_3", @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerWithFloatParameter("detail_bump_m_3", 1.0f, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddFloatParameter("diffuse_coefficient_m_3", 1.0f);
                        result.AddFloatParameter("specular_coefficient_m_3");
                        result.AddFloatParameter("specular_power_m_3", 10.0f);
                        result.AddFloat3ColorParameter("specular_tint_m_3", new ShaderColor(0, 255, 255, 255));
                        result.AddFloatParameter("fresnel_curve_steepness_m_3", 5.0f);
                        result.AddFloatParameter("area_specular_contribution_m_3", 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution_m_3", 0.5f);
                        result.AddFloatParameter("environment_specular_contribution_m_3");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_3");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_m_3";
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
                    case Wetness.Proof:
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

        public Array GetMethodNames() => Enum.GetValues(typeof(TerrainMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (TerrainMethods)methodIndex switch
            {
                TerrainMethods.Blending => Enum.GetValues(typeof(Blending)),
                TerrainMethods.Environment_Map => Enum.GetValues(typeof(Environment_Map)),
                TerrainMethods.Material_0 => Enum.GetValues(typeof(Material_0)),
                TerrainMethods.Material_1 => Enum.GetValues(typeof(Material_1)),
                TerrainMethods.Material_2 => Enum.GetValues(typeof(Material_2)),
                TerrainMethods.Material_3 => Enum.GetValues(typeof(Material_3)),
                TerrainMethods.Wetness => Enum.GetValues(typeof(Wetness)),
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
                ShaderStage.Dynamic_Light_Cinematic,
                ShaderStage.Static_Prt_Quadratic,
                ShaderStage.Static_Prt_Linear,
                ShaderStage.Static_Prt_Ambient
                //ShaderStage.Stipple,
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
            return (TerrainMethods)category switch
            {
                TerrainMethods.Blending => "blend_type",
                TerrainMethods.Environment_Map => "envmap_type",
                TerrainMethods.Material_0 => "material_0_type",
                TerrainMethods.Material_1 => "material_1_type",
                TerrainMethods.Material_2 => "material_2_type",
                TerrainMethods.Material_3 => "material_3_type",
                TerrainMethods.Wetness => "calc_wetness_ps",
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (TerrainMethods)category switch
            {
                TerrainMethods.Blending => string.Empty,
                TerrainMethods.Environment_Map => string.Empty,
                TerrainMethods.Material_0 => string.Empty,
                TerrainMethods.Material_1 => string.Empty,
                TerrainMethods.Material_2 => string.Empty,
                TerrainMethods.Material_3 => string.Empty,
                TerrainMethods.Wetness => string.Empty,
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (TerrainMethods)category switch
            {
                TerrainMethods.Blending => (Blending)option switch
                {
                    Blending.Morph => "morph",
                    Blending.Dynamic_Morph => "dynamic",
                    Blending.Distance_Blend_Base => "distance_blend_base",
                    _ => null,
                },
                TerrainMethods.Environment_Map => (Environment_Map)option switch
                {
                    Environment_Map.None => "none",
                    Environment_Map.Per_Pixel => "per_pixel",
                    Environment_Map.Dynamic => "dynamic",
                    Environment_Map.Dynamic_Reach => "dynamic_reach",
                    _ => null,
                },
                TerrainMethods.Material_0 => (Material_0)option switch
                {
                    Material_0.Diffuse_Only => "diffuse_only",
                    Material_0.Diffuse_Plus_Specular => "diffuse_plus_specular",
                    Material_0.Off => "off",
                    Material_0.Diffuse_Only_Plus_Self_Illum => "diffuse_only_plus_self_illum",
                    Material_0.Diffuse_Plus_Specular_Plus_Self_Illum => "diffuse_plus_specular_plus_self_illum",
                    Material_0.Diffuse_Plus_Specular_Plus_Heightmap => "diffuse_plus_specular_plus_heightmap",
                    Material_0.Diffuse_Plus_Two_Detail => "diffuse_plus_two_detail",
                    Material_0.Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap => "diffuse_plus_specular_plus_up_vector_plus_heightmap",
                    _ => null,
                },
                TerrainMethods.Material_1 => (Material_1)option switch
                {
                    Material_1.Diffuse_Only => "diffuse_only",
                    Material_1.Diffuse_Plus_Specular => "diffuse_plus_specular",
                    Material_1.Off => "off",
                    Material_1.Diffuse_Only_Plus_Self_Illum => "diffuse_only_plus_self_illum",
                    Material_1.Diffuse_Plus_Specular_Plus_Self_Illum => "diffuse_plus_specular_plus_self_illum",
                    Material_1.Diffuse_Plus_Specular_Plus_Heightmap => "diffuse_plus_specular_plus_heightmap",
                    Material_1.Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap => "diffuse_plus_specular_plus_up_vector_plus_heightmap",
                    _ => null,
                },
                TerrainMethods.Material_2 => (Material_2)option switch
                {
                    Material_2.Diffuse_Only => "diffuse_only",
                    Material_2.Diffuse_Plus_Specular => "diffuse_plus_specular",
                    Material_2.Off => "off",
                    Material_2.Diffuse_Only_Plus_Self_Illum => "diffuse_only_plus_self_illum",
                    Material_2.Diffuse_Plus_Specular_Plus_Self_Illum => "diffuse_plus_specular_plus_self_illum",
                    _ => null,
                },
                TerrainMethods.Material_3 => (Material_3)option switch
                {
                    Material_3.Off => "off",
                    Material_3.Diffuse_Only_Four_Material_Shaders_Disable_Detail_Bump => "diffuse_only",
                    Material_3.Diffuse_Plus_Specular_Four_Material_Shaders_Disable_Detail_Bump => "diffuse_plus_specular",
                    _ => null,
                },
                TerrainMethods.Wetness => (Wetness)option switch
                {
                    Wetness.Default => "calc_wetness_default_ps",
                    Wetness.Proof => "calc_wetness_proof_ps",
                    Wetness.Flood => "calc_wetness_flood_ps",
                    Wetness.Ripples => "calc_wetness_ripples_ps",
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (TerrainMethods)category switch
            {
                TerrainMethods.Blending => (Blending)option switch
                {
                    Blending.Morph => string.Empty,
                    Blending.Dynamic_Morph => string.Empty,
                    Blending.Distance_Blend_Base => string.Empty,
                    _ => null,
                },
                TerrainMethods.Environment_Map => (Environment_Map)option switch
                {
                    Environment_Map.None => string.Empty,
                    Environment_Map.Per_Pixel => string.Empty,
                    Environment_Map.Dynamic => string.Empty,
                    Environment_Map.Dynamic_Reach => string.Empty,
                    _ => null,
                },
                TerrainMethods.Material_0 => (Material_0)option switch
                {
                    Material_0.Diffuse_Only => string.Empty,
                    Material_0.Diffuse_Plus_Specular => string.Empty,
                    Material_0.Off => string.Empty,
                    Material_0.Diffuse_Only_Plus_Self_Illum => string.Empty,
                    Material_0.Diffuse_Plus_Specular_Plus_Self_Illum => string.Empty,
                    Material_0.Diffuse_Plus_Specular_Plus_Heightmap => string.Empty,
                    Material_0.Diffuse_Plus_Two_Detail => string.Empty,
                    Material_0.Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap => string.Empty,
                    _ => null,
                },
                TerrainMethods.Material_1 => (Material_1)option switch
                {
                    Material_1.Diffuse_Only => string.Empty,
                    Material_1.Diffuse_Plus_Specular => string.Empty,
                    Material_1.Off => string.Empty,
                    Material_1.Diffuse_Only_Plus_Self_Illum => string.Empty,
                    Material_1.Diffuse_Plus_Specular_Plus_Self_Illum => string.Empty,
                    Material_1.Diffuse_Plus_Specular_Plus_Heightmap => string.Empty,
                    Material_1.Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap => string.Empty,
                    _ => null,
                },
                TerrainMethods.Material_2 => (Material_2)option switch
                {
                    Material_2.Diffuse_Only => string.Empty,
                    Material_2.Diffuse_Plus_Specular => string.Empty,
                    Material_2.Off => string.Empty,
                    Material_2.Diffuse_Only_Plus_Self_Illum => string.Empty,
                    Material_2.Diffuse_Plus_Specular_Plus_Self_Illum => string.Empty,
                    _ => null,
                },
                TerrainMethods.Material_3 => (Material_3)option switch
                {
                    Material_3.Off => string.Empty,
                    Material_3.Diffuse_Only_Four_Material_Shaders_Disable_Detail_Bump => string.Empty,
                    Material_3.Diffuse_Plus_Specular_Four_Material_Shaders_Disable_Detail_Bump => string.Empty,
                    _ => null,
                },
                TerrainMethods.Wetness => (Wetness)option switch
                {
                    Wetness.Default => string.Empty,
                    Wetness.Proof => string.Empty,
                    Wetness.Flood => string.Empty,
                    Wetness.Ripples => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
