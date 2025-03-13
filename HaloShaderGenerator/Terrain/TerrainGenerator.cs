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

            if (methodName == "blending")
            {
                optionName = ((Blending)option).ToString();

                switch ((Blending)option)
                {
                    case Blending.Morph:
                        result.AddFloatParameter("global_albedo_tint", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("blend_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\terrain_options\default_blending";
                        break;
                    case Blending.Dynamic_Morph:
                        result.AddFloat4ColorParameter("dynamic_material", default, default, default, default, default, default);
                        result.AddFloatParameter("global_albedo_tint", default, default, default, default, default, 1f);
                        result.AddFloatParameter("transition_sharpness", default, default, default, default, default, 4f);
                        result.AddFloatParameter("transition_threshold", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("blend_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\terrain_options\dynamic_blending";
                        break;
                    case Blending.Distance_Blend_Base:
                        result.AddFloat4ColorParameter("blend_target_0", default, default, default, default, default, default);
                        result.AddFloat4ColorParameter("blend_target_1", default, default, default, default, default, default);
                        result.AddFloat4ColorParameter("blend_target_2", default, default, default, default, default, default);
                        result.AddFloat4ColorParameter("blend_target_3", default, default, default, default, default, default);
                        result.AddFloatParameter("blend_max_0", default, default, default, default, default, 0.8f);
                        result.AddFloatParameter("blend_max_1", default, default, default, default, default, 0.8f);
                        result.AddFloatParameter("blend_max_2", default, default, default, default, default, 0.8f);
                        result.AddFloatParameter("blend_max_3", default, default, default, default, default, 0.8f);
                        result.AddFloatParameter("blend_offset", default, default, default, default, default, -1f);
                        result.AddFloatParameter("blend_slope", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("global_albedo_tint", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("blend_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
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
                        result.AddFloat3ColorParameter("env_tint_color", default, default, default, default, default, default);
                        result.AddFloatParameter("env_roughness_offset", default, default, default, default, default, 0.5f);
                        result.AddSamplerParameter("environment_map", default, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"shaders\default_bitmaps\bitmaps\default_dynamic_cube_map");
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Map.Dynamic:
                        result.AddFloat3ColorParameter("env_tint_color", default, default, default, default, default, default);
                        result.AddFloatParameter("env_roughness_offset", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("env_roughness_scale", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
                        result.AddSamplerParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
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
                        result.AddSamplerParameter("base_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_0";
                        break;
                    case Material_0.Diffuse_Plus_Specular:
                        result.AddFloat3ColorParameter("specular_tint_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution_m_0", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution_m_0", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient_m_0", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_specular_contribution_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness_m_0", default, default, default, default, default, 5f);
                        result.AddFloatParameter("specular_coefficient_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_power_m_0", default, default, default, default, default, 10f);
                        result.AddSamplerParameter("base_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_m_0";
                        break;
                    case Material_0.Off:
                        break;
                    case Material_0.Diffuse_Only_Plus_Self_Illum:
                        result.AddFloat3ColorParameter("self_illum_color_m_0", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity_m_0", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("base_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("self_illum_detail_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("self_illum_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\terrain_options\diffuse_only_plus_sefl_illum_m_0";
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Self_Illum:
                        result.AddFloat3ColorParameter("self_illum_color_m_0", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat3ColorParameter("specular_tint_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution_m_0", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution_m_0", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient_m_0", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_specular_contribution_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness_m_0", default, default, default, default, default, 5f);
                        result.AddFloatParameter("self_illum_intensity_m_0", default, default, default, default, default, 1f);
                        result.AddFloatParameter("specular_coefficient_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_power_m_0", default, default, default, default, default, 10f);
                        result.AddSamplerParameter("base_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("self_illum_detail_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("self_illum_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_self_illumm_0";
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Heightmap:
                        result.AddBooleanParameter("heightmap_invert_m_0", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution_m_0", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution_m_0", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient_m_0", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_specular_contribution_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness_m_0", default, default, default, default, default, 5f);
                        result.AddFloatParameter("smooth_zone_m_0", default, default, default, default, default, 10f);
                        result.AddFloatParameter("specular_coefficient_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_power_m_0", default, default, default, default, default, 10f);
                        result.AddSamplerParameter("base_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("heightmap_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_heightmap_m_0";
                        break;
                    case Material_0.Diffuse_Plus_Two_Detail:
                        result.AddSamplerParameter("base_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map2_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\terrain_options\diffuse_plus_two_detail_m_0";
                        break;
                    case Material_0.Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap:
                        result.AddBooleanParameter("heightmap_invert_m_0", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution_m_0", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution_m_0", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient_m_0", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_specular_contribution_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness_m_0", default, default, default, default, default, 5f);
                        result.AddFloatParameter("smooth_zone_m_0", default, default, default, default, default, 10f);
                        result.AddFloatParameter("specular_coefficient_m_0", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_power_m_0", default, default, default, default, default, 10f);
                        result.AddFloatParameter("up_vector_scale_m_0", default, default, default, default, default, 1f);
                        result.AddFloatParameter("up_vector_shift_m_0", default, default, default, default, default, default);
                        result.AddSamplerParameter("base_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_map_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("heightmap_m_0", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
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
                        result.AddSamplerParameter("base_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_1";
                        break;
                    case Material_1.Diffuse_Plus_Specular:
                        result.AddFloat3ColorParameter("specular_tint_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution_m_1", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution_m_1", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient_m_1", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_specular_contribution_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness_m_1", default, default, default, default, default, 5f);
                        result.AddFloatParameter("specular_coefficient_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_power_m_1", default, default, default, default, default, 10f);
                        result.AddSamplerParameter("base_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_m_1";
                        break;
                    case Material_1.Off:
                        break;
                    case Material_1.Diffuse_Only_Plus_Self_Illum:
                        result.AddFloat3ColorParameter("self_illum_color_m_1", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity_m_1", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("base_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("self_illum_detail_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("self_illum_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\terrain_options\diffuse_only_plus_sefl_illum_m_1";
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Self_Illum:
                        result.AddFloat3ColorParameter("self_illum_color_m_1", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat3ColorParameter("specular_tint_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution_m_1", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution_m_1", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient_m_1", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_specular_contribution_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness_m_1", default, default, default, default, default, 5f);
                        result.AddFloatParameter("self_illum_intensity_m_1", default, default, default, default, default, 1f);
                        result.AddFloatParameter("specular_coefficient_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_power_m_1", default, default, default, default, default, 10f);
                        result.AddSamplerParameter("base_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("self_illum_detail_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("self_illum_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_self_illumm_1";
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Heightmap:
                        result.AddBooleanParameter("heightmap_invert_m_1", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution_m_1", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution_m_1", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient_m_1", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_specular_contribution_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness_m_1", default, default, default, default, default, 5f);
                        result.AddFloatParameter("smooth_zone_m_1", default, default, default, default, default, 10f);
                        result.AddFloatParameter("specular_coefficient_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_power_m_1", default, default, default, default, default, 10f);
                        result.AddSamplerParameter("base_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("heightmap_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_plus_heightmap_m_1";
                        break;
                    case Material_1.Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap:
                        result.AddBooleanParameter("heightmap_invert_m_1", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution_m_1", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution_m_1", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient_m_1", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_specular_contribution_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness_m_1", default, default, default, default, default, 5f);
                        result.AddFloatParameter("smooth_zone_m_1", default, default, default, default, default, 10f);
                        result.AddFloatParameter("specular_coefficient_m_1", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_power_m_1", default, default, default, default, default, 10f);
                        result.AddFloatParameter("up_vector_scale_m_1", default, default, default, default, default, 1f);
                        result.AddFloatParameter("up_vector_shift_m_1", default, default, default, default, default, default);
                        result.AddSamplerParameter("base_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_map_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("heightmap_m_1", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
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
                        result.AddSamplerParameter("base_map_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_map_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_2";
                        break;
                    case Material_2.Diffuse_Plus_Specular:
                        result.AddFloat3ColorParameter("specular_tint_m_2", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend_m_2", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution_m_2", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution_m_2", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient_m_2", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_specular_contribution_m_2", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness_m_2", default, default, default, default, default, 5f);
                        result.AddFloatParameter("specular_coefficient_m_2", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_power_m_2", default, default, default, default, default, 10f);
                        result.AddSamplerParameter("base_map_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_map_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\terrain_options\diffuse_plus_specular_m_2";
                        break;
                    case Material_2.Off:
                        break;
                    case Material_2.Diffuse_Only_Plus_Self_Illum:
                        result.AddFloat3ColorParameter("self_illum_color_m_2", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity_m_2", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("base_map_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("self_illum_detail_map_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("self_illum_map_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\terrain_options\diffuse_only_plus_sefl_illum_m_2";
                        break;
                    case Material_2.Diffuse_Plus_Specular_Plus_Self_Illum:
                        result.AddFloat3ColorParameter("self_illum_color_m_2", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat3ColorParameter("specular_tint_m_2", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend_m_2", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution_m_2", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution_m_2", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient_m_2", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_specular_contribution_m_2", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness_m_2", default, default, default, default, default, 5f);
                        result.AddFloatParameter("self_illum_intensity_m_2", default, default, default, default, default, 1f);
                        result.AddFloatParameter("specular_coefficient_m_2", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_power_m_2", default, default, default, default, default, 10f);
                        result.AddSamplerParameter("base_map_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("self_illum_detail_map_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("self_illum_map_m_2", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
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
                        result.AddSamplerParameter("base_map_m_3", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_3", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_3", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_map_m_3", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\terrain_options\diffuse_only_m_3";
                        break;
                    case Material_3.Diffuse_Plus_Specular_Four_Material_Shaders_Disable_Detail_Bump:
                        result.AddFloat3ColorParameter("specular_tint_m_3", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend_m_3", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution_m_3", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution_m_3", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient_m_3", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_specular_contribution_m_3", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness_m_3", default, default, default, default, default, 5f);
                        result.AddFloatParameter("specular_coefficient_m_3", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_power_m_3", default, default, default, default, default, 10f);
                        result.AddSamplerParameter("base_map_m_3", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("bump_map_m_3", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_bump_m_3", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("detail_map_m_3", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
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
                        result.AddFloat3ColorParameter("wet_material_dim_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("wet_material_dim_coefficient", default, default, default, default, default, 1f);
                        rmopName = @"shaders\wetness_options\wetness_simple";
                        break;
                    case Wetness.Proof:
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
                    case Wetness.Ripples:
                        result.AddFloat3ColorParameter("wet_material_dim_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("wet_sheen_reflection_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_mask_tweak_weight", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("surface_tilt_tweak_weight", default, default, default, default, default, 0.3f);
                        result.AddFloatParameter("wet_material_dim_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("wet_sheen_reflection_contribution", default, default, default, default, default, 0.37f);
                        result.AddFloatParameter("wet_sheen_thickness", default, default, default, default, default, 0.4f);
                        result.AddSamplerParameter("wet_noise_boundary_map", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, default, default, default, @"rasterizer\rain\rain_noise_boundary");
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
