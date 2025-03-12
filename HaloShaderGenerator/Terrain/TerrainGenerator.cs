using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Terrain
{
    public class TerrainGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(TerrainMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((TerrainMethods)methodIndex)
            {
                case TerrainMethods.Blending:
                    return Enum.GetValues(typeof(Blending)).Length;
                case TerrainMethods.Environment_Map:
                    return Enum.GetValues(typeof(Environment_Map)).Length;
                case TerrainMethods.Material_0:
                    return Enum.GetValues(typeof(Material_0)).Length;
                case TerrainMethods.Material_1:
                    return Enum.GetValues(typeof(Material_1)).Length;
                case TerrainMethods.Material_2:
                    return Enum.GetValues(typeof(Material_2)).Length;
                case TerrainMethods.Material_3:
                    return Enum.GetValues(typeof(Material_3)).Length;
                case TerrainMethods.Wetness:
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
                case ShaderStage.Static_Per_Vertex:
                case ShaderStage.Static_Sh:
                case ShaderStage.Dynamic_Light:
                case ShaderStage.Lightmap_Debug_Mode:
                case ShaderStage.Shadow_Generate:
                case ShaderStage.Dynamic_Light_Cinematic:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Stipple:
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
                case ShaderStage.Shadow_Generate:
                    return true;
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
                case ShaderStage.Dynamic_Light:
                case ShaderStage.Shadow_Generate:
                case ShaderStage.Lightmap_Debug_Mode:
                case ShaderStage.Dynamic_Light_Cinematic:
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

            if (methodName == "blending")
            {
                optionName = ((Blending)option).ToString();

                switch ((Blending)option)
                {
                    case Blending.Morph:
                        result.AddSamplerParameter("blend_map");
                        result.AddFloatParameter("global_albedo_tint");
                        rmopName = @"shaders\terrain_options\default_blending";
                        break;
                    case Blending.Dynamic_Morph:
                        result.AddSamplerParameter("blend_map");
                        result.AddFloatParameter("global_albedo_tint");
                        result.AddFloat4ColorParameter("dynamic_material");
                        result.AddFloatParameter("transition_sharpness");
                        result.AddFloatParameter("transition_threshold");
                        rmopName = @"shaders\terrain_options\dynamic_blending";
                        break;
                    case Blending.Distance_Blend_Base:
                        result.AddSamplerParameter("blend_map");
                        result.AddFloatParameter("global_albedo_tint");
                        result.AddFloat4ColorParameter("blend_target_0");
                        result.AddFloat4ColorParameter("blend_target_1");
                        result.AddFloat4ColorParameter("blend_target_2");
                        result.AddFloat4ColorParameter("blend_target_3");
                        result.AddFloatParameter("blend_offset");
                        result.AddFloatParameter("blend_slope");
                        result.AddFloatParameter("blend_max_0");
                        result.AddFloatParameter("blend_max_1");
                        result.AddFloatParameter("blend_max_2");
                        result.AddFloatParameter("blend_max_3");
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
                        result.AddSamplerParameter("environment_map", default, default, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddFloatParameter("env_roughness_offset");
                        result.AddFloatParameter("env_roughness_scale");
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Map.Dynamic:
                        result.AddFloat3ColorParameter("env_tint_color");
                        result.AddSamplerParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0, default, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddSamplerParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1, default, ShaderOptionParameter.ShaderAddressMode.Clamp);
                        result.AddFloatParameter("env_roughness_scale");
                        result.AddFloatParameter("env_roughness_offset");
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
                        result.AddSamplerParameter("base_map_m_0");
                        result.AddSamplerParameter("detail_map_m_0");
                        result.AddSamplerParameter("bump_map_m_0");
                        result.AddSamplerParameter("detail_bump_m_0");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_0";
                        break;
                    case Material_0.Diffuse_Plus_Specular:
                        result.AddSamplerParameter("base_map_m_0");
                        result.AddSamplerParameter("detail_map_m_0");
                        result.AddSamplerParameter("bump_map_m_0");
                        result.AddSamplerParameter("detail_bump_m_0");
                        result.AddFloatParameter("diffuse_coefficient_m_0");
                        result.AddFloatParameter("specular_coefficient_m_0");
                        result.AddFloatParameter("specular_power_m_0");
                        result.AddFloat3ColorParameter("specular_tint_m_0");
                        result.AddFloatParameter("fresnel_curve_steepness_m_0");
                        result.AddFloatParameter("area_specular_contribution_m_0");
                        result.AddFloatParameter("analytical_specular_contribution_m_0");
                        result.AddFloatParameter("environment_specular_contribution_m_0");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_0");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_m_0";
                        break;
                    case Material_0.Off:
                        break;
                    case Material_0.Diffuse_Only_Plus_Self_Illum:
                        result.AddSamplerParameter("base_map_m_0");
                        result.AddSamplerParameter("bump_map_m_0");
                        result.AddSamplerParameter("self_illum_map_m_0");
                        result.AddSamplerParameter("self_illum_detail_map_m_0");
                        result.AddFloat3ColorParameter("self_illum_color_m_0");
                        result.AddFloatParameter("self_illum_intensity_m_0");
                        rmopName = @"shaders\terrain_options\diffuse_only_plus_sefl_illum_m_0";
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Self_Illum:
                        result.AddSamplerParameter("base_map_m_0");
                        result.AddSamplerParameter("bump_map_m_0");
                        result.AddSamplerParameter("self_illum_map_m_0");
                        result.AddSamplerParameter("self_illum_detail_map_m_0");
                        result.AddFloat3ColorParameter("self_illum_color_m_0");
                        result.AddFloatParameter("self_illum_intensity_m_0");
                        result.AddFloatParameter("diffuse_coefficient_m_0");
                        result.AddFloatParameter("specular_coefficient_m_0");
                        result.AddFloatParameter("specular_power_m_0");
                        result.AddFloat3ColorParameter("specular_tint_m_0");
                        result.AddFloatParameter("fresnel_curve_steepness_m_0");
                        result.AddFloatParameter("area_specular_contribution_m_0");
                        result.AddFloatParameter("analytical_specular_contribution_m_0");
                        result.AddFloatParameter("environment_specular_contribution_m_0");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_0");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_self_illumm_0";
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Heightmap:
                        result.AddSamplerParameter("base_map_m_0");
                        result.AddSamplerParameter("detail_map_m_0");
                        result.AddSamplerParameter("bump_map_m_0");
                        result.AddSamplerParameter("detail_bump_m_0");
                        result.AddSamplerParameter("heightmap_m_0");
                        result.AddBooleanParameter("heightmap_invert_m_0");
                        result.AddFloatParameter("diffuse_coefficient_m_0");
                        result.AddFloatParameter("specular_coefficient_m_0");
                        result.AddFloatParameter("specular_power_m_0");
                        result.AddFloat3ColorParameter("specular_tint_m_0");
                        result.AddFloatParameter("fresnel_curve_steepness_m_0");
                        result.AddFloatParameter("area_specular_contribution_m_0");
                        result.AddFloatParameter("analytical_specular_contribution_m_0");
                        result.AddFloatParameter("environment_specular_contribution_m_0");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_0");
                        result.AddFloatParameter("smooth_zone_m_0");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_heightmap_m_0";
                        break;
                    case Material_0.Diffuse_Plus_Two_Detail:
                        result.AddSamplerParameter("base_map_m_0");
                        result.AddSamplerParameter("detail_map_m_0");
                        result.AddSamplerParameter("detail_map2_m_0");
                        result.AddSamplerParameter("bump_map_m_0");
                        result.AddSamplerParameter("detail_bump_m_0");
                        rmopName = @"shaders\terrain_options\diffuse_plus_two_detail_m_0";
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap:
                        result.AddSamplerParameter("base_map_m_0");
                        result.AddSamplerParameter("detail_map_m_0");
                        result.AddSamplerParameter("bump_map_m_0");
                        result.AddSamplerParameter("detail_bump_m_0");
                        result.AddSamplerParameter("heightmap_m_0");
                        result.AddBooleanParameter("heightmap_invert_m_0");
                        result.AddFloatParameter("diffuse_coefficient_m_0");
                        result.AddFloatParameter("specular_coefficient_m_0");
                        result.AddFloatParameter("specular_power_m_0");
                        result.AddFloat3ColorParameter("specular_tint_m_0");
                        result.AddFloatParameter("fresnel_curve_steepness_m_0");
                        result.AddFloatParameter("area_specular_contribution_m_0");
                        result.AddFloatParameter("analytical_specular_contribution_m_0");
                        result.AddFloatParameter("environment_specular_contribution_m_0");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_0");
                        result.AddFloatParameter("smooth_zone_m_0");
                        result.AddFloatParameter("up_vector_scale_m_0");
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
                        result.AddSamplerParameter("base_map_m_1");
                        result.AddSamplerParameter("detail_map_m_1");
                        result.AddSamplerParameter("bump_map_m_1");
                        result.AddSamplerParameter("detail_bump_m_1");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_1";
                        break;
                    case Material_1.Diffuse_Plus_Specular:
                        result.AddSamplerParameter("base_map_m_1");
                        result.AddSamplerParameter("detail_map_m_1");
                        result.AddSamplerParameter("bump_map_m_1");
                        result.AddSamplerParameter("detail_bump_m_1");
                        result.AddFloatParameter("diffuse_coefficient_m_1");
                        result.AddFloatParameter("specular_coefficient_m_1");
                        result.AddFloatParameter("specular_power_m_1");
                        result.AddFloat3ColorParameter("specular_tint_m_1");
                        result.AddFloatParameter("fresnel_curve_steepness_m_1");
                        result.AddFloatParameter("area_specular_contribution_m_1");
                        result.AddFloatParameter("analytical_specular_contribution_m_1");
                        result.AddFloatParameter("environment_specular_contribution_m_1");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_1");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_m_1";
                        break;
                    case Material_1.Off:
                        break;
                    case Material_1.Diffuse_Only_Plus_Self_Illum:
                        result.AddSamplerParameter("base_map_m_1");
                        result.AddSamplerParameter("bump_map_m_1");
                        result.AddSamplerParameter("self_illum_map_m_1");
                        result.AddSamplerParameter("self_illum_detail_map_m_1");
                        result.AddFloat3ColorParameter("self_illum_color_m_1");
                        result.AddFloatParameter("self_illum_intensity_m_1");
                        rmopName = @"shaders\terrain_options\diffuse_only_plus_sefl_illum_m_1";
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Self_Illum:
                        result.AddSamplerParameter("base_map_m_1");
                        result.AddSamplerParameter("bump_map_m_1");
                        result.AddSamplerParameter("self_illum_map_m_1");
                        result.AddSamplerParameter("self_illum_detail_map_m_1");
                        result.AddFloat3ColorParameter("self_illum_color_m_1");
                        result.AddFloatParameter("self_illum_intensity_m_1");
                        result.AddFloatParameter("diffuse_coefficient_m_1");
                        result.AddFloatParameter("specular_coefficient_m_1");
                        result.AddFloatParameter("specular_power_m_1");
                        result.AddFloat3ColorParameter("specular_tint_m_1");
                        result.AddFloatParameter("fresnel_curve_steepness_m_1");
                        result.AddFloatParameter("area_specular_contribution_m_1");
                        result.AddFloatParameter("analytical_specular_contribution_m_1");
                        result.AddFloatParameter("environment_specular_contribution_m_1");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_1");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_self_illumm_1";
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Heightmap:
                        result.AddSamplerParameter("base_map_m_1");
                        result.AddSamplerParameter("detail_map_m_1");
                        result.AddSamplerParameter("bump_map_m_1");
                        result.AddSamplerParameter("detail_bump_m_1");
                        result.AddSamplerParameter("heightmap_m_1");
                        result.AddBooleanParameter("heightmap_invert_m_1");
                        result.AddFloatParameter("diffuse_coefficient_m_1");
                        result.AddFloatParameter("specular_coefficient_m_1");
                        result.AddFloatParameter("specular_power_m_1");
                        result.AddFloat3ColorParameter("specular_tint_m_1");
                        result.AddFloatParameter("fresnel_curve_steepness_m_1");
                        result.AddFloatParameter("area_specular_contribution_m_1");
                        result.AddFloatParameter("analytical_specular_contribution_m_1");
                        result.AddFloatParameter("environment_specular_contribution_m_1");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_1");
                        result.AddFloatParameter("smooth_zone_m_1");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_heightmap_m_1";
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap:
                        result.AddSamplerParameter("base_map_m_1");
                        result.AddSamplerParameter("detail_map_m_1");
                        result.AddSamplerParameter("bump_map_m_1");
                        result.AddSamplerParameter("detail_bump_m_1");
                        result.AddSamplerParameter("heightmap_m_1");
                        result.AddBooleanParameter("heightmap_invert_m_1");
                        result.AddFloatParameter("diffuse_coefficient_m_1");
                        result.AddFloatParameter("specular_coefficient_m_1");
                        result.AddFloatParameter("specular_power_m_1");
                        result.AddFloat3ColorParameter("specular_tint_m_1");
                        result.AddFloatParameter("fresnel_curve_steepness_m_1");
                        result.AddFloatParameter("area_specular_contribution_m_1");
                        result.AddFloatParameter("analytical_specular_contribution_m_1");
                        result.AddFloatParameter("environment_specular_contribution_m_1");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_1");
                        result.AddFloatParameter("smooth_zone_m_1");
                        result.AddFloatParameter("up_vector_scale_m_1");
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
                        result.AddSamplerParameter("base_map_m_2");
                        result.AddSamplerParameter("detail_map_m_2");
                        result.AddSamplerParameter("bump_map_m_2");
                        result.AddSamplerParameter("detail_bump_m_2");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_2";
                        break;
                    case Material_2.Diffuse_Plus_Specular:
                        result.AddSamplerParameter("base_map_m_2");
                        result.AddSamplerParameter("detail_map_m_2");
                        result.AddSamplerParameter("bump_map_m_2");
                        result.AddSamplerParameter("detail_bump_m_2");
                        result.AddFloatParameter("diffuse_coefficient_m_2");
                        result.AddFloatParameter("specular_coefficient_m_2");
                        result.AddFloatParameter("specular_power_m_2");
                        result.AddFloat3ColorParameter("specular_tint_m_2");
                        result.AddFloatParameter("fresnel_curve_steepness_m_2");
                        result.AddFloatParameter("area_specular_contribution_m_2");
                        result.AddFloatParameter("analytical_specular_contribution_m_2");
                        result.AddFloatParameter("environment_specular_contribution_m_2");
                        result.AddFloatParameter("albedo_specular_tint_blend_m_2");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_m_2";
                        break;
                    case Material_2.Off:
                        break;
                    case Material_2.Diffuse_Only_Plus_Self_Illum:
                        result.AddSamplerParameter("base_map_m_2");
                        result.AddSamplerParameter("bump_map_m_2");
                        result.AddSamplerParameter("self_illum_map_m_2");
                        result.AddSamplerParameter("self_illum_detail_map_m_2");
                        result.AddFloat3ColorParameter("self_illum_color_m_2");
                        result.AddFloatParameter("self_illum_intensity_m_2");
                        rmopName = @"shaders\terrain_options\diffuse_only_plus_sefl_illum_m_2";
                        break;
                    case Material_2.Diffuse_Plus_Specular_Plus_Self_Illum:
                        result.AddSamplerParameter("base_map_m_2");
                        result.AddSamplerParameter("bump_map_m_2");
                        result.AddSamplerParameter("self_illum_map_m_2");
                        result.AddSamplerParameter("self_illum_detail_map_m_2");
                        result.AddFloat3ColorParameter("self_illum_color_m_2");
                        result.AddFloatParameter("self_illum_intensity_m_2");
                        result.AddFloatParameter("diffuse_coefficient_m_2");
                        result.AddFloatParameter("specular_coefficient_m_2");
                        result.AddFloatParameter("specular_power_m_2");
                        result.AddFloat3ColorParameter("specular_tint_m_2");
                        result.AddFloatParameter("fresnel_curve_steepness_m_2");
                        result.AddFloatParameter("area_specular_contribution_m_2");
                        result.AddFloatParameter("analytical_specular_contribution_m_2");
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
                        result.AddSamplerParameter("base_map_m_3");
                        result.AddSamplerParameter("detail_map_m_3");
                        result.AddSamplerParameter("bump_map_m_3");
                        result.AddSamplerParameter("detail_bump_m_3");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_3";
                        break;
                    case Material_3.Diffuse_Plus_Specular_Four_Material_Shaders_Disable_Detail_Bump:
                        result.AddSamplerParameter("base_map_m_3");
                        result.AddSamplerParameter("detail_map_m_3");
                        result.AddSamplerParameter("bump_map_m_3");
                        result.AddSamplerParameter("detail_bump_m_3");
                        result.AddFloatParameter("diffuse_coefficient_m_3");
                        result.AddFloatParameter("specular_coefficient_m_3");
                        result.AddFloatParameter("specular_power_m_3");
                        result.AddFloat3ColorParameter("specular_tint_m_3");
                        result.AddFloatParameter("fresnel_curve_steepness_m_3");
                        result.AddFloatParameter("area_specular_contribution_m_3");
                        result.AddFloatParameter("analytical_specular_contribution_m_3");
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
                        result.AddFloatParameter("wet_material_dim_coefficient");
                        result.AddFloat3ColorParameter("wet_material_dim_tint");
                        rmopName = @"shaders\wetness_options\wetness_simple";
                        break;
                    case Wetness.Proof:
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
                    case Wetness.Ripples:
                        result.AddFloatParameter("wet_material_dim_coefficient");
                        result.AddFloat3ColorParameter("wet_material_dim_tint");
                        result.AddFloatParameter("wet_sheen_reflection_contribution");
                        result.AddFloat3ColorParameter("wet_sheen_reflection_tint");
                        result.AddFloatParameter("wet_sheen_thickness");
                        result.AddSamplerParameter("wet_noise_boundary_map", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, default);
                        result.AddFloatParameter("specular_mask_tweak_weight");
                        result.AddFloatParameter("surface_tilt_tweak_weight");
                        rmopName = @"shaders\wetness_options\wetness_ripples";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(TerrainMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((TerrainMethods)methodIndex)
            {
                case TerrainMethods.Blending:
                    return Enum.GetValues(typeof(Blending));
                case TerrainMethods.Environment_Map:
                    return Enum.GetValues(typeof(Environment_Map));
                case TerrainMethods.Material_0:
                    return Enum.GetValues(typeof(Material_0));
                case TerrainMethods.Material_1:
                    return Enum.GetValues(typeof(Material_1));
                case TerrainMethods.Material_2:
                    return Enum.GetValues(typeof(Material_2));
                case TerrainMethods.Material_3:
                    return Enum.GetValues(typeof(Material_3));
                case TerrainMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness));
            }

            return null;
        }

        public void GetCategoryFunctions(string methodName, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;

            if (methodName == "blending")
            {
                vertexFunction = "invalid";
                pixelFunction = "blend_type";
            }

            if (methodName == "environment_map")
            {
                vertexFunction = "invalid";
                pixelFunction = "envmap_type";
            }

            if (methodName == "material_0")
            {
                vertexFunction = "invalid";
                pixelFunction = "material_0_type";
            }

            if (methodName == "material_1")
            {
                vertexFunction = "invalid";
                pixelFunction = "material_1_type";
            }

            if (methodName == "material_2")
            {
                vertexFunction = "invalid";
                pixelFunction = "material_2_type";
            }

            if (methodName == "material_3")
            {
                vertexFunction = "invalid";
                pixelFunction = "material_3_type";
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

            if (methodName == "blending")
            {
                switch ((Blending)option)
                {
                    case Blending.Morph:
                        vertexFunction = "invalid";
                        pixelFunction = "morph";
                        break;
                    case Blending.Dynamic_Morph:
                        vertexFunction = "invalid";
                        pixelFunction = "dynamic";
                        break;
                    case Blending.Distance_Blend_Base:
                        vertexFunction = "invalid";
                        pixelFunction = "distance_blend_base";
                        break;
                }
            }

            if (methodName == "environment_map")
            {
                switch ((Environment_Map)option)
                {
                    case Environment_Map.None:
                        vertexFunction = "invalid";
                        pixelFunction = "none";
                        break;
                    case Environment_Map.Per_Pixel:
                        vertexFunction = "invalid";
                        pixelFunction = "per_pixel";
                        break;
                    case Environment_Map.Dynamic:
                        vertexFunction = "invalid";
                        pixelFunction = "dynamic";
                        break;
                    case Environment_Map.Dynamic_Reach:
                        vertexFunction = "invalid";
                        pixelFunction = "dynamic_reach";
                        break;
                }
            }

            if (methodName == "material_0")
            {
                switch ((Material_0)option)
                {
                    case Material_0.Diffuse_Only:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_only";
                        break;
                    case Material_0.Diffuse_Plus_Specular:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_plus_specular";
                        break;
                    case Material_0.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "off";
                        break;
                    case Material_0.Diffuse_Only_Plus_Self_Illum:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_only_plus_self_illum";
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Self_Illum:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_plus_specular_plus_self_illum";
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Heightmap:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_plus_specular_plus_heightmap";
                        break;
                    case Material_0.Diffuse_Plus_Two_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_plus_two_detail";
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_plus_specular_plus_up_vector_plus_heightmap";
                        break;
                }
            }

            if (methodName == "material_1")
            {
                switch ((Material_1)option)
                {
                    case Material_1.Diffuse_Only:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_only";
                        break;
                    case Material_1.Diffuse_Plus_Specular:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_plus_specular";
                        break;
                    case Material_1.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "off";
                        break;
                    case Material_1.Diffuse_Only_Plus_Self_Illum:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_only_plus_self_illum";
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Self_Illum:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_plus_specular_plus_self_illum";
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Heightmap:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_plus_specular_plus_heightmap";
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_plus_specular_plus_up_vector_plus_heightmap";
                        break;
                }
            }

            if (methodName == "material_2")
            {
                switch ((Material_2)option)
                {
                    case Material_2.Diffuse_Only:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_only";
                        break;
                    case Material_2.Diffuse_Plus_Specular:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_plus_specular";
                        break;
                    case Material_2.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "off";
                        break;
                    case Material_2.Diffuse_Only_Plus_Self_Illum:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_only_plus_self_illum";
                        break;
                    case Material_2.Diffuse_Plus_Specular_Plus_Self_Illum:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_plus_specular_plus_self_illum";
                        break;
                }
            }

            if (methodName == "material_3")
            {
                switch ((Material_3)option)
                {
                    case Material_3.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "off";
                        break;
                    case Material_3.Diffuse_Only_Four_Material_Shaders_Disable_Detail_Bump:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_only";
                        break;
                    case Material_3.Diffuse_Plus_Specular_Four_Material_Shaders_Disable_Detail_Bump:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_plus_specular";
                        break;
                }
            }

            if (methodName == "wetness")
            {
                switch ((Wetness)option)
                {
                    case Wetness.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_default_ps";
                        break;
                    case Wetness.Proof:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_proof_ps";
                        break;
                    case Wetness.Flood:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_flood_ps";
                        break;
                    case Wetness.Ripples:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_ripples_ps";
                        break;
                }
            }
        }

        public ShaderParameters GetParameterArguments(string methodName, int option)
        {
            ShaderParameters result = new ShaderParameters();
            if (methodName == "blending")
            {
                switch ((Blending)option)
                {
                    case Blending.Morph:
                        break;
                    case Blending.Dynamic_Morph:
                        break;
                    case Blending.Distance_Blend_Base:
                        break;
                }
            }

            if (methodName == "environment_map")
            {
                switch ((Environment_Map)option)
                {
                    case Environment_Map.None:
                        break;
                    case Environment_Map.Per_Pixel:
                        break;
                    case Environment_Map.Dynamic:
                        break;
                    case Environment_Map.Dynamic_Reach:
                        break;
                }
            }

            if (methodName == "material_0")
            {
                switch ((Material_0)option)
                {
                    case Material_0.Diffuse_Only:
                        break;
                    case Material_0.Diffuse_Plus_Specular:
                        break;
                    case Material_0.Off:
                        break;
                    case Material_0.Diffuse_Only_Plus_Self_Illum:
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Self_Illum:
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Heightmap:
                        break;
                    case Material_0.Diffuse_Plus_Two_Detail:
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap:
                        break;
                }
            }

            if (methodName == "material_1")
            {
                switch ((Material_1)option)
                {
                    case Material_1.Diffuse_Only:
                        break;
                    case Material_1.Diffuse_Plus_Specular:
                        break;
                    case Material_1.Off:
                        break;
                    case Material_1.Diffuse_Only_Plus_Self_Illum:
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Self_Illum:
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Heightmap:
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap:
                        break;
                }
            }

            if (methodName == "material_2")
            {
                switch ((Material_2)option)
                {
                    case Material_2.Diffuse_Only:
                        break;
                    case Material_2.Diffuse_Plus_Specular:
                        break;
                    case Material_2.Off:
                        break;
                    case Material_2.Diffuse_Only_Plus_Self_Illum:
                        break;
                    case Material_2.Diffuse_Plus_Specular_Plus_Self_Illum:
                        break;
                }
            }

            if (methodName == "material_3")
            {
                switch ((Material_3)option)
                {
                    case Material_3.Off:
                        break;
                    case Material_3.Diffuse_Only_Four_Material_Shaders_Disable_Detail_Bump:
                        break;
                    case Material_3.Diffuse_Plus_Specular_Four_Material_Shaders_Disable_Detail_Bump:
                        break;
                }
            }

            if (methodName == "wetness")
            {
                switch ((Wetness)option)
                {
                    case Wetness.Default:
                        break;
                    case Wetness.Proof:
                        break;
                    case Wetness.Flood:
                        break;
                    case Wetness.Ripples:
                        break;
                }
            }
            return result;
        }
    }
}
