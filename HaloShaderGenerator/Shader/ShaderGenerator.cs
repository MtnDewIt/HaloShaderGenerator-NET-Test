using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Shader
{
    public class ShaderGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(ShaderMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            switch ((ShaderMethods)methodIndex)
            {
                case ShaderMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo)).Length;
                case ShaderMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping)).Length;
                case ShaderMethods.Alpha_Test:
                    return Enum.GetValues(typeof(Alpha_Test)).Length;
                case ShaderMethods.Specular_Mask:
                    return Enum.GetValues(typeof(Specular_Mask)).Length;
                case ShaderMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model)).Length;
                case ShaderMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping)).Length;
                case ShaderMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination)).Length;
                case ShaderMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode)).Length;
                case ShaderMethods.Parallax:
                    return Enum.GetValues(typeof(Parallax)).Length;
                case ShaderMethods.Misc:
                    return Enum.GetValues(typeof(Misc)).Length;
                case ShaderMethods.Distortion:
                    return Enum.GetValues(typeof(Distortion)).Length;
                case ShaderMethods.Soft_Fade:
                    return Enum.GetValues(typeof(Soft_Fade)).Length;
                case ShaderMethods.Misc_Attr_Animation:
                    return Enum.GetValues(typeof(Misc_Attr_Animation)).Length;
                case ShaderMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness)).Length;
                case ShaderMethods.Alpha_Blend_Source:
                    return Enum.GetValues(typeof(Alpha_Blend_Source)).Length;
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
                case ShaderStage.Dynamic_Light:
                case ShaderStage.Shadow_Generate:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Active_Camo:
                case ShaderStage.Static_Per_Vertex_Color:
                case ShaderStage.Lightmap_Debug_Mode:
                case ShaderStage.Dynamic_Light_Cinematic:
                case ShaderStage.Stipple:
                case ShaderStage.Single_Pass_Per_Pixel:
                case ShaderStage.Single_Pass_Per_Vertex:
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
                case 2:
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
                case ShaderStage.Active_Camo:
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
                case VertexType.DualQuat:
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
                case ShaderStage.Active_Camo:
                case ShaderStage.Lightmap_Debug_Mode:
                case ShaderStage.Static_Per_Vertex_Color:
                case ShaderStage.Dynamic_Light_Cinematic:
                case ShaderStage.Z_Only:
                case ShaderStage.Sfx_Distort:
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
                    case Albedo.Default:
                        result.AddFloat4ColorParameter("albedo_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_default";
                        break;
                    case Albedo.Detail_Blend:
                        result.AddFloatParameter("blend_alpha", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map2", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_detail_blend";
                        break;
                    case Albedo.Constant_Color:
                        result.AddFloat4ColorParameter("albedo_color");
                        rmopName = @"shaders\shader_options\albedo_constant";
                        break;
                    case Albedo.Two_Change_Color:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddSamplerParameter("change_color_map");
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary);
                        result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary);
                        result.AddSamplerParameter("camouflage_change_color_map");
                        result.AddFloatParameter("camouflage_scale");
                        rmopName = @"shaders\shader_options\albedo_two_change_color";
                        break;
                    case Albedo.Four_Change_Color:
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, default, default, default, default, default);
                        result.AddFloat3ColorParameter("quaternary_change_color", RenderMethodExtern.object_change_color_quaternary, default, default, default, default, default);
                        result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary, default, default, default, default, default);
                        result.AddFloat3ColorParameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary, default, default, default, default, default);
                        result.AddFloatParameter("camouflage_scale", default, default, default, default, default, default);
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("camouflage_change_color_map", default, default, default, default, default, default);
                        result.AddSamplerParameter("change_color_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_four_change_color";
                        break;
                    case Albedo.Three_Detail_Blend:
                        result.AddFloatParameter("blend_alpha", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map2", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map3", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_three_detail_blend";
                        break;
                    case Albedo.Two_Detail_Overlay:
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map_overlay", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map2", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_detail_overlay";
                        break;
                    case Albedo.Two_Detail:
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("detail_map2", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_detail";
                        break;
                    case Albedo.Color_Mask:
                        result.AddFloat3ColorParameter("neutral_gray", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("albedo_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("albedo_color2", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("albedo_color3", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("color_mask_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\reference_grids");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_color_mask";
                        break;
                    case Albedo.Two_Detail_Black_Point:
                        result.AddSamplerParameter("base_map");
                        result.AddSamplerParameter("detail_map");
                        result.AddSamplerParameter("detail_map2");
                        rmopName = @"shaders\shader_options\albedo_two_detail_black_point";
                        break;
                    case Albedo.Two_Change_Color_Anim_Overlay:
                        result.AddFloat3ColorParameter("primary_change_color_anim", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, default, default, default, default, default);
                        result.AddFloat3ColorParameter("secondary_change_color_anim", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary, default, default, default, default, default);
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("change_color_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_change_color_anim";
                        break;
                    case Albedo.Chameleon:
                        result.AddFloat3ColorParameter("chameleon_color0", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("chameleon_color1", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("chameleon_color2", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("chameleon_color3", default, default, default, default, default, default);
                        result.AddFloatParameter("chameleon_color_offset1", default, default, default, default, default, 0.3333f);
                        result.AddFloatParameter("chameleon_color_offset2", default, default, default, default, default, 0.6666f);
                        result.AddFloatParameter("chameleon_fresnel_power", default, default, default, default, default, 2f);
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_chameleon";
                        break;
                    case Albedo.Two_Change_Color_Chameleon:
                        result.AddFloat3ColorParameter("chameleon_color0", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("chameleon_color1", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("chameleon_color2", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("chameleon_color3", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("primary_change_color_anim", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, default, default, default, default, default);
                        result.AddFloat3ColorParameter("secondary_change_color_anim", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary, default, default, default, default, default);
                        result.AddFloatParameter("chameleon_color_offset1", default, default, default, default, default, 0.3333f);
                        result.AddFloatParameter("chameleon_color_offset2", default, default, default, default, default, 0.6666f);
                        result.AddFloatParameter("chameleon_fresnel_power", default, default, default, default, default, 2f);
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("change_color_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_change_color_chameleon";
                        break;
                    case Albedo.Chameleon_Masked:
                        result.AddFloat3ColorParameter("chameleon_color0", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("chameleon_color1", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("chameleon_color2", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("chameleon_color3", default, default, default, default, default, default);
                        result.AddFloatParameter("chameleon_color_offset1", default, default, default, default, default, 0.3333f);
                        result.AddFloatParameter("chameleon_color_offset2", default, default, default, default, default, 0.6666f);
                        result.AddFloatParameter("chameleon_fresnel_power", default, default, default, default, default, 2f);
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("chameleon_mask_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerParameter("detail_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_chameleon_masked";
                        break;
                    case Albedo.Color_Mask_Hard_Light:
                        result.AddFloat4ColorParameter("albedo_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("color_mask_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\reference_grids");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_color_mask_hard_light";
                        break;
                    case Albedo.Two_Change_Color_Tex_Overlay:
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, default, default, default, default, default);
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("change_color_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("secondary_change_color_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\albedo_two_change_color_tex_overlay";
                        break;
                    case Albedo.Chameleon_Albedo_Masked:
                        result.AddFloat3ColorParameter("chameleon_color0", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("chameleon_color1", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("chameleon_color2", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("chameleon_color3", default, default, default, default, default, default);
                        result.AddFloat4ColorParameter("albedo_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("albedo_masked_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("chameleon_color_offset1", default, default, default, default, default, 0.3333f);
                        result.AddFloatParameter("chameleon_color_offset2", default, default, default, default, default, 0.6666f);
                        result.AddFloatParameter("chameleon_fresnel_power", default, default, default, default, default, 2f);
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("base_masked_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("chameleon_mask_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\shader_options\albedo_chameleon_albedo_masked";
                        break;
                    case Albedo.Custom_Cube:
                        result.AddFloat4ColorParameter("albedo_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("custom_cube", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\albedo_custom_cube";
                        break;
                    case Albedo.Two_Color:
                        result.AddFloat4ColorParameter("albedo_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("albedo_second_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("blend_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_two_color";
                        break;
                    case Albedo.Scrolling_Cube_Mask:
                        result.AddFloat4ColorParameter("albedo_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("albedo_second_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("color_blend_mask_cubemap", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_scrolling_cube_mask";
                        break;
                    case Albedo.Scrolling_Cube:
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("color_cubemap", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_scrolling_cube";
                        break;
                    case Albedo.Scrolling_Texture_Uv:
                        result.AddFloatParameter("u_speed", default, default, default, default, default, 1f);
                        result.AddFloatParameter("v_speed", default, default, default, default, default, default);
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("color_texture", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\albedo_scrolling_texture_uv";
                        break;
                    case Albedo.Texture_From_Misc:
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("color_texture", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\albedo_texture_from_misc";
                        break;
                    case Albedo.Four_Change_Color_Applying_To_Specular:
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, default, default, default, default, default);
                        result.AddFloat3ColorParameter("quaternary_change_color", RenderMethodExtern.object_change_color_quaternary, default, default, default, default, default);
                        result.AddFloat3ColorParameter("secondary_change_color", RenderMethodExtern.object_change_color_secondary, default, default, default, default, default);
                        result.AddFloat3ColorParameter("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary, default, default, default, default, default);
                        result.AddFloatParameter("camouflage_scale", default, default, default, default, default, default);
                        result.AddSamplerParameter("base_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("camouflage_change_color_map", default, default, default, default, default, default);
                        result.AddSamplerParameter("change_color_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_detail");
                        rmopName = @"shaders\shader_options\albedo_four_change_color";
                        break;
                    case Albedo.Simple:
                        result.AddFloat4ColorParameter("albedo_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddSamplerParameter("base_map", default, default, default, default, 1, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\albedo_simple";
                        break;
                    case Albedo.Emblem:
                        result.AddSamplerParameter("emblem_map", RenderMethodExtern.emblem_player_shoulder_texture);
                        rmopName = @"shaders\shader_options\albedo_emblem";
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
                    case Bump_Mapping.Detail_Masked:
                        result.AddBooleanParameter("invert_mask", default, default, default, default, default, default);
                        result.AddFloatParameter("bump_detail_coefficient", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("bump_detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_detail_mask_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerParameter("bump_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\bump_detail_masked";
                        break;
                    case Bump_Mapping.Detail_Plus_Detail_Masked:
                        result.AddFloatParameter("bump_detail_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("bump_detail_masked_coefficient", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("bump_detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_detail_mask_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerParameter("bump_detail_masked_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\bump_detail_plus_detail_masked";
                        break;
                    case Bump_Mapping.Detail_Unorm:
                        result.AddFloatParameter("bump_detail_coefficient", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("bump_detail_map", default, default, default, default, 16, @"shaders\default_bitmaps\bitmaps\default_vector");
                        result.AddSamplerParameter("bump_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\bump_detail_unorm";
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

            if (methodName == "alpha_test")
            {
                optionName = ((Alpha_Test)option).ToString();

                switch ((Alpha_Test)option)
                {
                    case Alpha_Test.None:
                        rmopName = @"shaders\shader_options\alpha_test_off";
                        break;
                    case Alpha_Test.Simple:
                        result.AddSamplerParameter("alpha_test_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_alpha_test");
                        rmopName = @"shaders\shader_options\alpha_test_on";
                        break;
                }
            }

            if (methodName == "specular_mask")
            {
                optionName = ((Specular_Mask)option).ToString();

                switch ((Specular_Mask)option)
                {
                    case Specular_Mask.No_Specular_Mask:
                        break;
                    case Specular_Mask.Specular_Mask_From_Diffuse:
                        break;
                    case Specular_Mask.Specular_Mask_From_Texture:
                        result.AddSamplerParameter("specular_mask_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\shader_options\specular_mask_from_texture";
                        break;
                    case Specular_Mask.Specular_Mask_From_Color_Texture:
                        result.AddSamplerParameter("specular_mask_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\shader_options\specular_mask_from_texture";
                        break;
                    case Specular_Mask.Specular_Mask_Mult_Diffuse:
                        result.AddSamplerParameter("specular_mask_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\shader_options\specular_mask_mult_diffuse";
                        break;
                }
            }

            if (methodName == "material_model")
            {
                optionName = ((Material_Model)option).ToString();

                switch ((Material_Model)option)
                {
                    case Material_Model.Diffuse_Only:
                        result.AddBooleanParameter("no_dynamic_lights");
                        result.AddFloatParameter("approximate_specular_type");
                        rmopName = @"shaders\shader_options\material_diffuse_only";
                        break;
                    case Material_Model.Cook_Torrance:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular", default, default, default, default, default, default);
                        result.AddBooleanParameter("use_material_texture", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color", default, default, default, default, default, new ShaderColor(1, 128, 128, 128));
                        result.AddFloat3ColorParameter("specular_color_by_angle", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_anti_shadow_control", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_roughness", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("approximate_specular_type", default, default, default, default, default, default);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness", default, default, default, default, default, 5f);
                        result.AddFloatParameter("material_texture_black_roughness", default, default, default, default, default, 1f);
                        result.AddFloatParameter("material_texture_black_specular_multiplier", default, default, default, default, default, 1f);
                        result.AddFloatParameter("roughness", default, default, default, default, default, 0.04f);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236, default, default, default, default, default);
                        result.AddSamplerParameter("material_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_cook_torrance_option";
                        break;
                    case Material_Model.Two_Lobe_Phong:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("glancing_specular_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("normal_specular_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_color_by_angle", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_anti_shadow_control", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_power", default, default, default, default, default, 25f);
                        result.AddFloatParameter("analytical_roughness", default, default, default, default, default, 0.02f);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("approximate_specular_type", default, default, default, default, default, default);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("fresnel_curve_steepness", default, default, default, default, default, 5f);
                        result.AddFloatParameter("glancing_specular_power", default, default, default, default, default, 10f);
                        result.AddFloatParameter("normal_specular_power", default, default, default, default, default, 10f);
                        result.AddFloatParameter("roughness", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, 1f);
                        rmopName = @"shaders\shader_options\material_two_lobe_phong_option";
                        break;
                    case Material_Model.Foliage:
                        result.AddBooleanParameter("no_dynamic_lights");
                        rmopName = @"shaders\shader_options\material_foliage";
                        break;
                    case Material_Model.None:
                        break;
                    case Material_Model.Glass:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("fresnel_coefficient", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("fresnel_curve_bias", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness", default, default, default, default, default, 5f);
                        result.AddFloatParameter("roughness", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, default);
                        rmopName = @"shaders\shader_options\glass_material";
                        break;
                    case Material_Model.Organism:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("use_material_texture", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("ambient_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("diffuse_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("environment_map_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("final_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color", default, default, default, default, default, new ShaderColor(1, 128, 128, 128));
                        result.AddFloat3ColorParameter("rim_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_color_by_angle", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("subsurface_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("transparence_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("ambient_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_roughness", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("analytical_specular_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("approximate_specular_type", default, default, default, default, default, default);
                        result.AddFloatParameter("area_specular_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, 0.16f);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness", default, default, default, default, default, 5f);
                        result.AddFloatParameter("material_texture_black_roughness", default, default, default, default, default, 1f);
                        result.AddFloatParameter("material_texture_black_specular_multiplier", default, default, default, default, default, 1f);
                        result.AddFloatParameter("rim_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("rim_maps_transition_ratio", default, default, default, default, default, default);
                        result.AddFloatParameter("rim_power", default, default, default, default, default, 2f);
                        result.AddFloatParameter("rim_start", default, default, default, default, default, 0.7f);
                        result.AddFloatParameter("rim_width", default, default, default, default, default, 0.3f);
                        result.AddFloatParameter("roughness", default, default, default, default, default, 0.04f);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("specular_power", default, default, default, default, default, 10f);
                        result.AddFloatParameter("subsurface_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("subsurface_normal_detail", default, default, default, default, default, default);
                        result.AddFloatParameter("subsurface_propagation_bias", default, default, default, default, default, default);
                        result.AddFloatParameter("transparence_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("transparence_normal_bias", default, default, default, default, default, default);
                        result.AddFloatParameter("transparence_normal_detail", default, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_cooktorran_array", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
                        result.AddSamplerParameter("material_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("occlusion_parameter_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerParameter("specular_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerParameter("subsurface_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerParameter("transparence_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white");
                        rmopName = @"shaders\shader_options\material_organism_option";
                        break;
                    case Material_Model.Single_Lobe_Phong:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_anti_shadow_control", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("roughness", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, default);
                        rmopName = @"shaders\shader_options\single_lobe_phong";
                        break;
                    case Material_Model.Car_Paint:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular0", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular1", default, default, default, default, default, default);
                        result.AddBooleanParameter("use_material_texture0", default, default, default, default, default, default);
                        result.AddBooleanParameter("use_material_texture1", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color_environment1", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color0", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color1", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("rim_fresnel_color1", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint0", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint1", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_blend0", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_blend1", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution0", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution1", default, default, default, default, default, 0.2f);
                        result.AddFloatParameter("area_specular_contribution0", default, default, default, default, default, 0.3f);
                        result.AddFloatParameter("area_specular_contribution1", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("bump_detail_map0_blend_factor", default, default, default, default, default, 0.75f);
                        result.AddFloatParameter("diffuse_coefficient0", default, default, default, default, default, 0.25f);
                        result.AddFloatParameter("diffuse_coefficient1", default, default, default, default, default, default);
                        result.AddFloatParameter("environment_map_specular_contribution1", default, default, default, default, default, 0.3f);
                        result.AddFloatParameter("fresnel_power0", default, default, default, default, default, 1f);
                        result.AddFloatParameter("fresnel_power1", default, default, default, default, default, 1f);
                        result.AddFloatParameter("rim_fresnel_albedo_blend1", default, default, default, default, default, default);
                        result.AddFloatParameter("rim_fresnel_coefficient1", default, default, default, default, default, default);
                        result.AddFloatParameter("rim_fresnel_power1", default, default, default, default, default, 2f);
                        result.AddFloatParameter("roughness0", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("roughness1", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("specular_coefficient0", default, default, default, default, default, 1f);
                        result.AddFloatParameter("specular_coefficient1", default, default, default, default, default, 0.15f);
                        result.AddSamplerParameter("bump_detail_map0", default, ShaderOptionParameter.ShaderFilterMode.Anisotropic4Expensive, default, default, 16, @"shaders\default_bitmaps\bitmaps\sparklenoisemap");
                        result.AddSamplerParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236, default, default, default, default, default);
                        result.AddSamplerParameter("material_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_car_paint_option";
                        break;
                    case Material_Model.Cook_Torrance_Custom_Cube:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular", default, default, default, default, default, default);
                        result.AddBooleanParameter("use_material_texture", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color", default, default, default, default, default, new ShaderColor(1, 128, 128, 128));
                        result.AddFloat3ColorParameter("specular_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_anti_shadow_control", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("roughness", default, default, default, default, default, 0.4f);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, default);
                        result.AddSamplerParameter("custom_cube", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236, default, default, default, default, default);
                        result.AddSamplerParameter("material_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_cook_torrance_custom_cube_option";
                        break;
                    case Material_Model.Cook_Torrance_Pbr_Maps:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular", default, default, default, default, default, default);
                        result.AddBooleanParameter("use_material_texture", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color", default, default, default, default, default, new ShaderColor(1, 128, 128, 128));
                        result.AddFloat3ColorParameter("specular_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_anti_shadow_control", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("roughness", default, default, default, default, default, 0.4f);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236, default, default, default, default, default);
                        result.AddSamplerParameter("material_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("spec_tint_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_cook_torrance_pbr_maps_option";
                        break;
                    case Material_Model.Cook_Torrance_Two_Color_Spec_Tint:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular", default, default, default, default, default, default);
                        result.AddBooleanParameter("use_material_texture", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color", default, default, default, default, default, new ShaderColor(1, 128, 128, 128));
                        result.AddFloat3ColorParameter("specular_second_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_anti_shadow_control", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("roughness", default, default, default, default, default, 0.4f);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236, default, default, default, default, default);
                        result.AddSamplerParameter("material_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("spec_blend_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_cook_torrance_two_color_spec_tint";
                        break;
                    case Material_Model.Two_Lobe_Phong_Tint_Map:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("glancing_specular_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("normal_specular_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_anti_shadow_control", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness", default, default, default, default, default, 5f);
                        result.AddFloatParameter("glancing_specular_power", default, default, default, default, default, 10f);
                        result.AddFloatParameter("normal_specular_power", default, default, default, default, default, 10f);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, default);
                        result.AddSamplerParameter("glancing_specular_tint_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("normal_specular_tint_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_two_lobe_phong_tint_map_option";
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube_Mask:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular", default, default, default, default, default, default);
                        result.AddBooleanParameter("use_material_texture", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color", default, default, default, default, default, new ShaderColor(1, 128, 128, 128));
                        result.AddFloat3ColorParameter("specular_second_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_anti_shadow_control", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("roughness", default, default, default, default, default, 0.4f);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236, default, default, default, default, default);
                        result.AddSamplerParameter("material_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("tint_blend_mask_cubemap", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_cook_torrance_scrolling_cube_mask";
                        break;
                    case Material_Model.Cook_Torrance_Rim_Fresnel:
                        result.AddBooleanParameter("albedo_blend_with_specular_tint", default, default, default, default, default, default);
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular", default, default, default, default, default, default);
                        result.AddBooleanParameter("use_fresnel_color_environment", default, default, default, default, default, default);
                        result.AddBooleanParameter("use_material_texture", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color_environment", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color", default, default, default, default, default, new ShaderColor(1, 128, 128, 128));
                        result.AddFloat3ColorParameter("rim_fresnel_color", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_anti_shadow_control", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_power", default, default, default, default, default, 1f);
                        result.AddFloatParameter("rim_fresnel_albedo_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("rim_fresnel_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("rim_fresnel_power", default, default, default, default, default, 2f);
                        result.AddFloatParameter("roughness", default, default, default, default, default, 0.4f);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236, default, default, default, default, default);
                        result.AddSamplerParameter("material_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_cook_torrance_rim_fresnel";
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular", default, default, default, default, default, default);
                        result.AddBooleanParameter("use_material_texture", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color", default, default, default, default, default, new ShaderColor(1, 128, 128, 128));
                        result.AddFloatParameter("albedo_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_anti_shadow_control", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("roughness", default, default, default, default, default, 0.4f);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236, default, default, default, default, default);
                        result.AddSamplerParameter("material_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("spec_tint_cubemap", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_cook_torrance_scrolling_cube";
                        break;
                    case Material_Model.Cook_Torrance_From_Albedo:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("order3_area_specular", default, default, default, default, default, default);
                        result.AddBooleanParameter("use_material_texture", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color", default, default, default, default, default, new ShaderColor(1, 128, 128, 128));
                        result.AddFloatParameter("albedo_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_anti_shadow_control", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("roughness", default, default, default, default, default, 0.4f);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_c78d78", RenderMethodExtern.texture_cook_torrance_c78d78, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_cc0236", RenderMethodExtern.texture_cook_torrance_cc0236, default, default, default, default, default);
                        result.AddSamplerParameter("g_sampler_dd0236", RenderMethodExtern.texture_cook_torrance_dd0236, default, default, default, default, default);
                        result.AddSamplerParameter("material_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_cook_torrance_from_albedo";
                        break;
                    case Material_Model.Hair:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("diffuse_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("environment_map_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("final_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_specular_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("area_specular_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("environment_map_coefficient", default, default, default, default, default, default);
                        result.AddFloatParameter("roughness", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("specular_power", default, default, default, default, default, 10f);
                        result.AddSamplerParameter("specular_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\color_white");
                        result.AddSamplerParameter("specular_noise_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        result.AddSamplerParameter("specular_shift_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        rmopName = @"shaders\shader_options\material_hair_option";
                        break;
                    case Material_Model.Cook_Torrance_Reach:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddBooleanParameter("use_material_texture", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("fresnel_color", default, default, default, default, default, new ShaderColor(1, 128, 128, 128));
                        result.AddFloat3ColorParameter("specular_color_by_angle", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_roughness", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("approximate_specular_type", default, default, default, default, default, default);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("fresnel_curve_steepness", default, default, default, default, default, 5f);
                        result.AddFloatParameter("material_texture_black_roughness", default, default, default, default, default, 1f);
                        result.AddFloatParameter("material_texture_black_specular_multiplier", default, default, default, default, default, 1f);
                        result.AddFloatParameter("roughness", default, default, default, default, default, 0.04f);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("material_texture", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\material_cook_torrance_option_reach";
                        break;
                    case Material_Model.Two_Lobe_Phong_Reach:
                        result.AddBooleanParameter("no_dynamic_lights", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("glancing_specular_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("normal_specular_tint", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("specular_color_by_angle", default, default, default, default, default, default);
                        result.AddFloatParameter("albedo_specular_tint_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("analytical_power", default, default, default, default, default, 25f);
                        result.AddFloatParameter("analytical_roughness", default, default, default, default, default, 0.02f);
                        result.AddFloatParameter("analytical_specular_contribution", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("approximate_specular_type", default, default, default, default, default, default);
                        result.AddFloatParameter("area_specular_contribution", default, default, default, default, default, default);
                        result.AddFloatParameter("diffuse_coefficient", default, default, default, default, default, 1f);
                        result.AddFloatParameter("environment_map_specular_contribution", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("fresnel_curve_steepness", default, default, default, default, default, 5f);
                        result.AddFloatParameter("glancing_specular_power", default, default, default, default, default, 10f);
                        result.AddFloatParameter("normal_specular_power", default, default, default, default, default, 10f);
                        result.AddFloatParameter("roughness", default, default, default, default, default, default);
                        result.AddFloatParameter("specular_coefficient", default, default, default, default, default, 1f);
                        rmopName = @"shaders\shader_options\material_two_lobe_phong_option_reach";
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
                    case Environment_Mapping.Custom_Map:
                        result.AddFloat3ColorParameter("env_tint_color", default, default, default, default, default, default);
                        result.AddFloatParameter("env_roughness_offset", default, default, default, default, default, 0.5f);
                        result.AddSamplerParameter("environment_map", default, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"shaders\default_bitmaps\bitmaps\default_dynamic_cube_map");
                        rmopName = @"shaders\shader_options\env_map_per_pixel";
                        break;
                    case Environment_Mapping.From_Flat_Texture_As_Cubemap:
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
                    case Environment_Mapping.Dynamic_Reach:
                        result.AddFloat3ColorParameter("env_tint_color", default, default, default, default, default, default);
                        result.AddFloatParameter("env_roughness_offset", default, default, default, default, default, 0.5f);
                        result.AddFloatParameter("env_roughness_scale", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("dynamic_environment_map_0", RenderMethodExtern.texture_dynamic_environment_map_0, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
                        result.AddSamplerParameter("dynamic_environment_map_1", RenderMethodExtern.texture_dynamic_environment_map_1, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
                        rmopName = @"shaders\shader_options\env_map_dynamic";
                        break;
                }
            }

            if (methodName == "self_illumination")
            {
                optionName = ((Self_Illumination)option).ToString();

                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.Off:
                        break;
                    case Self_Illumination.Simple:
                        result.AddFloat3ColorParameter("self_illum_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_simple";
                        break;
                    case Self_Illumination._3_Channel_Self_Illum:
                        result.AddFloat4ColorParameter("channel_a", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("channel_b", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("channel_c", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_3_channel";
                        break;
                    case Self_Illumination.Plasma:
                        result.AddFloat4ColorParameter("color_medium", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("color_sharp", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("color_wide", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddFloatParameter("thinness_medium", default, default, default, default, default, 16f);
                        result.AddFloatParameter("thinness_sharp", default, default, default, default, default, 32f);
                        result.AddFloatParameter("thinness_wide", default, default, default, default, default, 4f);
                        result.AddSamplerParameter("alpha_mask_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\alpha_white");
                        result.AddSamplerParameter("noise_map_a", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("noise_map_b", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_plasma";
                        break;
                    case Self_Illumination.From_Diffuse:
                        result.AddFloat3ColorParameter("self_illum_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        rmopName = @"shaders\shader_options\illum_from_diffuse";
                        break;
                    case Self_Illumination.Illum_Detail:
                        result.AddFloat3ColorParameter("self_illum_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("self_illum_detail_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_detail";
                        break;
                    case Self_Illumination.Meter:
                        result.AddFloat3ColorParameter("meter_color_off", default, default, default, default, default, default);
                        result.AddFloat3ColorParameter("meter_color_on", default, default, default, default, default, default);
                        result.AddFloatParameter("meter_value", default, default, default, default, default, 0.5f);
                        result.AddSamplerParameter("meter_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\monochrome_alpha_grid");
                        rmopName = @"shaders\shader_options\illum_meter";
                        break;
                    case Self_Illumination.Self_Illum_Times_Diffuse:
                        result.AddFloat3ColorParameter("self_illum_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("primary_change_color_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_times_diffuse";
                        break;
                    case Self_Illumination.Simple_With_Alpha_Mask:
                        result.AddFloat3ColorParameter("self_illum_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_simple";
                        break;
                    case Self_Illumination.Simple_Four_Change_Color:
                        result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_quaternary, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_simple_four_change_color";
                        break;
                    case Self_Illumination.Illum_Detail_World_Space_Four_Cc:
                        result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_quaternary, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloat4ColorParameter("self_illum_obj_bounding_sphere", default, default, default, default, default, default);
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("self_illum_detail_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_detail_world_space_four_cc";
                        break;
                    case Self_Illumination.Illum_Change_Color:
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, default, default, default, default, default);
                        result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_primary, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("primary_change_color_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_change_color";
                        break;
                    case Self_Illumination.Multilayer_Additive:
                        result.AddFloat3ColorParameter("self_illum_color", default, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("depth_darken", default, default, default, default, default, 1f);
                        result.AddFloatParameter("layer_contrast", default, default, default, default, default, 4f);
                        result.AddFloatParameter("layer_depth", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddFloatParameter("texcoord_aspect_ratio", default, default, default, default, default, 1f);
                        result.AddIntegerParameter("layers_of_4", default, default, default, default, default, 4);
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_multilayer";
                        break;
                    case Self_Illumination.Palettized_Plasma:
                        result.AddFloat3ColorParameter("self_illum_color", default, default, default, default, default, new ShaderColor(1, 255, 255, 255));
                        result.AddFloatParameter("alpha_modulation_factor", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("depth_fade_range", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("global_camera_forward", RenderMethodExtern.global_camera_forward, default, default, default, default, default);
                        result.AddFloatParameter("global_depth_constants", RenderMethodExtern.global_depth_constants, default, default, default, default, default);
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddFloatParameter("v_coordinate", default, default, default, default, default, 0.5f);
                        result.AddSamplerParameter("alpha_mask_map", default, default, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"shaders\default_bitmaps\bitmaps\alpha_grey50");
                        result.AddSamplerParameter("depth_buffer", RenderMethodExtern.texture_global_target_z, ShaderOptionParameter.ShaderFilterMode.Point, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, default);
                        result.AddSamplerParameter("noise_map_a", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("noise_map_b", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerParameter("palette", default, ShaderOptionParameter.ShaderFilterMode.Bilinear, ShaderOptionParameter.ShaderAddressMode.Clamp, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_palettized_plasma";
                        break;
                    case Self_Illumination.Change_Color:
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, default, default, default, default, default);
                        result.AddFloat3ColorParameter("self_illum_color", RenderMethodExtern.object_change_color_primary, default, default, default, default, new ShaderColor(255, 255, 255, 255));
                        result.AddFloatParameter("primary_change_color_blend", default, default, default, default, default, default);
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_change_color";
                        break;
                    case Self_Illumination.Change_Color_Detail:
                        result.AddFloat3ColorParameter("primary_change_color", RenderMethodExtern.object_change_color_primary, default, default, default, default, default);
                        result.AddFloatParameter("self_illum_intensity", default, default, default, default, default, 1f);
                        result.AddSamplerParameter("self_illum_detail_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddSamplerParameter("self_illum_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        rmopName = @"shaders\shader_options\illum_change_color_detail";
                        break;
                }
            }

            if (methodName == "blend_mode")
            {
                optionName = ((Blend_Mode)option).ToString();

                switch ((Blend_Mode)option)
                {
                    case Blend_Mode.Opaque:
                        break;
                    case Blend_Mode.Additive:
                        break;
                    case Blend_Mode.Multiply:
                        break;
                    case Blend_Mode.Alpha_Blend:
                        break;
                    case Blend_Mode.Double_Multiply:
                        break;
                    case Blend_Mode.Pre_Multiplied_Alpha:
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
                        result.AddFloatParameter("height_scale", default, default, default, default, default, 0.1f);
                        result.AddSamplerParameter("height_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        rmopName = @"shaders\shader_options\parallax_simple";
                        break;
                    case Parallax.Interpolated:
                        result.AddFloatParameter("height_scale", default, default, default, default, default, 0.1f);
                        result.AddSamplerParameter("height_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        rmopName = @"shaders\shader_options\parallax_simple";
                        break;
                    case Parallax.Simple_Detail:
                        result.AddFloatParameter("height_scale", default, default, default, default, default, 0.1f);
                        result.AddSamplerParameter("height_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        result.AddSamplerParameter("height_scale_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\gray_50_percent_linear");
                        rmopName = @"shaders\shader_options\parallax_detail";
                        break;
                }
            }

            if (methodName == "misc")
            {
                optionName = ((Misc)option).ToString();

                switch ((Misc)option)
                {
                    case Misc.First_Person_Never:
                        break;
                    case Misc.First_Person_Sometimes:
                        break;
                    case Misc.First_Person_Always:
                        break;
                    case Misc.First_Person_Never_With_Rotating_Bitmaps:
                        break;
                    case Misc.Default:
                        break;
                    case Misc.Rotating_Bitmaps_Super_Slow:
                        break;
                    case Misc.Always_Calc_Albedo:
                        break;
                }
            }

            if (methodName == "distortion")
            {
                optionName = ((Distortion)option).ToString();

                switch ((Distortion)option)
                {
                    case Distortion.Off:
                        break;
                    case Distortion.On:
                        result.AddBooleanParameter("distort_selfonly", default, default, default, default, default, default);
                        result.AddFloatParameter("distort_fadeoff", default, default, default, default, default, 10f);
                        result.AddFloatParameter("distort_scale", default, default, default, default, default, 0.1f);
                        result.AddSamplerParameter("distort_map", default, default, default, default, default, @"shaders\default_bitmaps\bitmaps\default_vector");
                        rmopName = @"shaders\shader_options\sfx_distort";
                        break;
                }
            }

            if (methodName == "soft_fade")
            {
                optionName = ((Soft_Fade)option).ToString();

                switch ((Soft_Fade)option)
                {
                    case Soft_Fade.Off:
                        break;
                    case Soft_Fade.On:
                        result.AddSamplerParameter("depth_map", RenderMethodExtern.texture_global_target_z, ShaderOptionParameter.ShaderFilterMode.Point, default);
                        result.AddBooleanParameter("use_soft_fresnel");
                        result.AddFloatParameter("soft_fresnel_power");
                        result.AddBooleanParameter("use_soft_z");
                        result.AddFloatParameter("soft_z_range");
                        result.AddFloatParameter("screen_params", RenderMethodExtern.screen_constants);
                        rmopName = @"shaders\shader_options\soft_fade";
                        break;
                }
            }

            if (methodName == "misc_attr_animation")
            {
                optionName = ((Misc_Attr_Animation)option).ToString();

                switch ((Misc_Attr_Animation)option)
                {
                    case Misc_Attr_Animation.Off:
                        break;
                    case Misc_Attr_Animation.Scrolling_Cube:
                        result.AddFloatParameter("scrolling_axis_x", default, default, default, default, default, default);
                        result.AddFloatParameter("scrolling_axis_y", default, default, default, default, default, 1f);
                        result.AddFloatParameter("scrolling_axis_z", default, default, default, default, default, default);
                        result.AddFloatParameter("scrolling_speed", default, default, default, default, default, 1f);
                        result.AddIntegerParameter("misc_attr_animation_option", default, default, default, default, default, 1);
                        rmopName = @"shaders\shader_options\misc_attr_scrolling_cube";
                        break;
                    case Misc_Attr_Animation.Scrolling_Projected:
                        result.AddFloatParameter("object_center_x", default, default, default, default, default, default);
                        result.AddFloatParameter("object_center_y", default, default, default, default, default, default);
                        result.AddFloatParameter("object_center_z", default, default, default, default, default, 0.65f);
                        result.AddFloatParameter("plane_u_x", default, default, default, default, default, default);
                        result.AddFloatParameter("plane_u_y", default, default, default, default, default, 1f);
                        result.AddFloatParameter("plane_u_z", default, default, default, default, default, default);
                        result.AddFloatParameter("plane_v_x", default, default, default, default, default, default);
                        result.AddFloatParameter("plane_v_y", default, default, default, default, default, default);
                        result.AddFloatParameter("plane_v_z", default, default, default, default, default, 1f);
                        result.AddFloatParameter("scale_u", default, default, default, default, default, 1f);
                        result.AddFloatParameter("scale_v", default, default, default, default, default, 1f);
                        result.AddFloatParameter("speed_u", default, default, default, default, default, 0.1f);
                        result.AddFloatParameter("speed_v", default, default, default, default, default, default);
                        result.AddFloatParameter("translate_u", default, default, default, default, default, default);
                        result.AddFloatParameter("translate_v", default, default, default, default, default, default);
                        result.AddIntegerParameter("misc_attr_animation_option", default, default, default, default, default, 2);
                        rmopName = @"shaders\shader_options\misc_attr_scrolling_projected";
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
                    case Wetness.Proof:
                        break;
                    case Wetness.Simple:
                        result.AddFloat3ColorParameter("wet_material_dim_tint", default, default, default, default, default, default);
                        result.AddFloatParameter("wet_material_dim_coefficient", default, default, default, default, default, 1f);
                        rmopName = @"shaders\wetness_options\wetness_simple";
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
            return Enum.GetValues(typeof(ShaderMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            switch ((ShaderMethods)methodIndex)
            {
                case ShaderMethods.Albedo:
                    return Enum.GetValues(typeof(Albedo));
                case ShaderMethods.Bump_Mapping:
                    return Enum.GetValues(typeof(Bump_Mapping));
                case ShaderMethods.Alpha_Test:
                    return Enum.GetValues(typeof(Alpha_Test));
                case ShaderMethods.Specular_Mask:
                    return Enum.GetValues(typeof(Specular_Mask));
                case ShaderMethods.Material_Model:
                    return Enum.GetValues(typeof(Material_Model));
                case ShaderMethods.Environment_Mapping:
                    return Enum.GetValues(typeof(Environment_Mapping));
                case ShaderMethods.Self_Illumination:
                    return Enum.GetValues(typeof(Self_Illumination));
                case ShaderMethods.Blend_Mode:
                    return Enum.GetValues(typeof(Blend_Mode));
                case ShaderMethods.Parallax:
                    return Enum.GetValues(typeof(Parallax));
                case ShaderMethods.Misc:
                    return Enum.GetValues(typeof(Misc));
                case ShaderMethods.Distortion:
                    return Enum.GetValues(typeof(Distortion));
                case ShaderMethods.Soft_Fade:
                    return Enum.GetValues(typeof(Soft_Fade));
                case ShaderMethods.Misc_Attr_Animation:
                    return Enum.GetValues(typeof(Misc_Attr_Animation));
                case ShaderMethods.Wetness:
                    return Enum.GetValues(typeof(Wetness));
                case ShaderMethods.Alpha_Blend_Source:
                    return Enum.GetValues(typeof(Alpha_Blend_Source));
            }

            return null;
        }

        public byte[] ValidateOptions(byte[] options)
        {
            List<byte> optionList = new List<byte>(options);

            while (optionList.Count < GetMethodCount())
                optionList.Add(0);

            return optionList.ToArray();
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

            if (methodName == "alpha_test")
            {
                vertexFunction = "alpha_test";
                pixelFunction = "calc_alpha_test_ps";
            }

            if (methodName == "specular_mask")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_specular_mask_ps";
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

            if (methodName == "self_illumination")
            {
                vertexFunction = "invalid";
                pixelFunction = "calc_self_illumination_ps";
            }

            if (methodName == "blend_mode")
            {
                vertexFunction = "invalid";
                pixelFunction = "blend_type";
            }

            if (methodName == "parallax")
            {
                vertexFunction = "calc_parallax_vs";
                pixelFunction = "calc_parallax_ps";
            }

            if (methodName == "misc")
            {
                vertexFunction = "invalid";
                pixelFunction = "bitmap_rotation";
            }

            if (methodName == "distortion")
            {
                vertexFunction = "distort_proc_vs";
                pixelFunction = "distort_proc_ps";
            }

            if (methodName == "soft_fade")
            {
                vertexFunction = "invalid";
                pixelFunction = "apply_soft_fade";
            }

            if (methodName == "misc_attr_animation")
            {
                vertexFunction = "invalid"; // misc_attr_define (We ran out of output registers :/)
                pixelFunction = "invalid";
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
                    case Albedo.Default:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_default_ps";
                        break;
                    case Albedo.Detail_Blend:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_detail_blend_ps";
                        break;
                    case Albedo.Constant_Color:
                        vertexFunction = "calc_albedo_constant_color_vs";
                        pixelFunction = "calc_albedo_constant_color_ps";
                        break;
                    case Albedo.Two_Change_Color:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_change_color_ps";
                        break;
                    case Albedo.Four_Change_Color:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_four_change_color_ps";
                        break;
                    case Albedo.Three_Detail_Blend:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_three_detail_blend_ps";
                        break;
                    case Albedo.Two_Detail_Overlay:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_detail_overlay_ps";
                        break;
                    case Albedo.Two_Detail:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_detail_ps";
                        break;
                    case Albedo.Color_Mask:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_color_mask_ps";
                        break;
                    case Albedo.Two_Detail_Black_Point:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_detail_black_point_ps";
                        break;
                    case Albedo.Two_Change_Color_Anim_Overlay:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_change_color_anim_ps";
                        break;
                    case Albedo.Chameleon:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_chameleon_ps";
                        break;
                    case Albedo.Two_Change_Color_Chameleon:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_change_color_chameleon_ps";
                        break;
                    case Albedo.Chameleon_Masked:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_chameleon_masked_ps";
                        break;
                    case Albedo.Color_Mask_Hard_Light:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_color_mask_hard_light_ps";
                        break;
                    case Albedo.Two_Change_Color_Tex_Overlay:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_change_color_tex_overlay_ps";
                        break;
                    case Albedo.Chameleon_Albedo_Masked:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_chameleon_albedo_masked_ps";
                        break;
                    case Albedo.Custom_Cube:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_custom_cube_ps";
                        break;
                    case Albedo.Two_Color:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_two_color_ps";
                        break;
                    case Albedo.Scrolling_Cube_Mask:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_scrolling_cube_mask_ps";
                        break;
                    case Albedo.Scrolling_Cube:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_scrolling_cube_ps";
                        break;
                    case Albedo.Scrolling_Texture_Uv:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_scrolling_texture_uv_ps";
                        break;
                    case Albedo.Texture_From_Misc:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_texture_from_misc_ps";
                        break;
                    case Albedo.Four_Change_Color_Applying_To_Specular:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_four_change_color_applying_to_specular_ps";
                        break;
                    case Albedo.Simple:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_simple_ps";
                        break;
                    case Albedo.Emblem:
                        vertexFunction = "calc_albedo_default_vs";
                        pixelFunction = "calc_albedo_emblem_ps";
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
                    case Bump_Mapping.Detail_Masked:
                        vertexFunction = "calc_bumpmap_detail_vs";
                        pixelFunction = "calc_bumpmap_detail_masked_ps";
                        break;
                    case Bump_Mapping.Detail_Plus_Detail_Masked:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_bumpmap_detail_plus_detail_masked_ps";
                        break;
                    case Bump_Mapping.Detail_Unorm:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_bumpmap_detail_unorm_ps";
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

            if (methodName == "alpha_test")
            {
                switch ((Alpha_Test)option)
                {
                    case Alpha_Test.None:
                        vertexFunction = "off";
                        pixelFunction = "calc_alpha_test_off_ps";
                        break;
                    case Alpha_Test.Simple:
                        vertexFunction = "on";
                        pixelFunction = "calc_alpha_test_on_ps";
                        break;
                }
            }

            if (methodName == "specular_mask")
            {
                switch ((Specular_Mask)option)
                {
                    case Specular_Mask.No_Specular_Mask:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_no_specular_mask_ps";
                        break;
                    case Specular_Mask.Specular_Mask_From_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_from_diffuse_ps";
                        break;
                    case Specular_Mask.Specular_Mask_From_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_texture_ps";
                        break;
                    case Specular_Mask.Specular_Mask_From_Color_Texture:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_color_texture_ps";
                        break;
                    case Specular_Mask.Specular_Mask_Mult_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_specular_mask_mult_texture_ps";
                        break;
                }
            }

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Diffuse_Only:
                        vertexFunction = "invalid";
                        pixelFunction = "diffuse_only";
                        break;
                    case Material_Model.Cook_Torrance:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance";
                        break;
                    case Material_Model.Two_Lobe_Phong:
                        vertexFunction = "invalid";
                        pixelFunction = "two_lobe_phong";
                        break;
                    case Material_Model.Foliage:
                        vertexFunction = "invalid";
                        pixelFunction = "foliage";
                        break;
                    case Material_Model.None:
                        vertexFunction = "invalid";
                        pixelFunction = "none";
                        break;
                    case Material_Model.Glass:
                        vertexFunction = "invalid";
                        pixelFunction = "glass";
                        break;
                    case Material_Model.Organism:
                        vertexFunction = "invalid";
                        pixelFunction = "organism";
                        break;
                    case Material_Model.Single_Lobe_Phong:
                        vertexFunction = "invalid";
                        pixelFunction = "single_lobe_phong";
                        break;
                    case Material_Model.Car_Paint:
                        vertexFunction = "invalid";
                        pixelFunction = "car_paint";
                        break;
                    case Material_Model.Cook_Torrance_Custom_Cube:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_custom_cube";
                        break;
                    case Material_Model.Cook_Torrance_Pbr_Maps:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_pbr_maps";
                        break;
                    case Material_Model.Cook_Torrance_Two_Color_Spec_Tint:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_two_color_spec_tint";
                        break;
                    case Material_Model.Two_Lobe_Phong_Tint_Map:
                        vertexFunction = "invalid";
                        pixelFunction = "two_lobe_phong_tint_map";
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube_Mask:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_scrolling_cube_mask";
                        break;
                    case Material_Model.Cook_Torrance_Rim_Fresnel:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_rim_fresnel";
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_scrolling_cube";
                        break;
                    case Material_Model.Cook_Torrance_From_Albedo:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_from_albedo";
                        break;
                    case Material_Model.Hair:
                        vertexFunction = "invalid";
                        pixelFunction = "hair";
                        break;
                    case Material_Model.Cook_Torrance_Reach:
                        vertexFunction = "invalid";
                        pixelFunction = "cook_torrance_reach";
                        break;
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
                    case Environment_Mapping.Custom_Map:
                        vertexFunction = "invalid";
                        pixelFunction = "custom_map";
                        break;
                    case Environment_Mapping.From_Flat_Texture_As_Cubemap:
                        vertexFunction = "invalid";
                        pixelFunction = "from_flat_texture_as_cubemap";
                        break;
                    case Environment_Mapping.Dynamic_Reach:
                        vertexFunction = "invalid";
                        pixelFunction = "dynamic_reach";
                        break;
                }
            }

            if (methodName == "self_illumination")
            {
                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_none_ps";
                        break;
                    case Self_Illumination.Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_simple_ps";
                        break;
                    case Self_Illumination._3_Channel_Self_Illum:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_three_channel_ps";
                        break;
                    case Self_Illumination.Plasma:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_plasma_ps";
                        break;
                    case Self_Illumination.From_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_from_albedo_ps";
                        break;
                    case Self_Illumination.Illum_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_detail_ps";
                        break;
                    case Self_Illumination.Meter:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_meter_ps";
                        break;
                    case Self_Illumination.Self_Illum_Times_Diffuse:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_times_diffuse_ps";
                        break;
                    case Self_Illumination.Simple_With_Alpha_Mask:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_simple_with_alpha_mask_ps";
                        break;
                    case Self_Illumination.Simple_Four_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_simple_ps";
                        break;
                    case Self_Illumination.Illum_Detail_World_Space_Four_Cc:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_detail_world_space_ps";
                        break;
                    case Self_Illumination.Illum_Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_change_color_ps";
                        break;
                    case Self_Illumination.Multilayer_Additive:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_multilayer_ps";
                        break;
                    case Self_Illumination.Palettized_Plasma:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_palettized_plasma_ps";
                        break;
                    case Self_Illumination.Change_Color:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_change_color_ps";
                        break;
                    case Self_Illumination.Change_Color_Detail:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_self_illumination_change_color_detail_ps";
                        break;
                }
            }

            if (methodName == "blend_mode")
            {
                switch ((Blend_Mode)option)
                {
                    case Blend_Mode.Opaque:
                        vertexFunction = "invalid";
                        pixelFunction = "opaque";
                        break;
                    case Blend_Mode.Additive:
                        vertexFunction = "invalid";
                        pixelFunction = "additive";
                        break;
                    case Blend_Mode.Multiply:
                        vertexFunction = "invalid";
                        pixelFunction = "multiply";
                        break;
                    case Blend_Mode.Alpha_Blend:
                        vertexFunction = "invalid";
                        pixelFunction = "alpha_blend";
                        break;
                    case Blend_Mode.Double_Multiply:
                        vertexFunction = "invalid";
                        pixelFunction = "double_multiply";
                        break;
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        vertexFunction = "invalid";
                        pixelFunction = "pre_multiplied_alpha";
                        break;
                }
            }

            if (methodName == "parallax")
            {
                switch ((Parallax)option)
                {
                    case Parallax.Off:
                        vertexFunction = "calc_parallax_off_vs";
                        pixelFunction = "calc_parallax_off_ps";
                        break;
                    case Parallax.Simple:
                        vertexFunction = "calc_parallax_simple_vs";
                        pixelFunction = "calc_parallax_simple_ps";
                        break;
                    case Parallax.Interpolated:
                        vertexFunction = "calc_parallax_interpolated_vs";
                        pixelFunction = "calc_parallax_interpolated_ps";
                        break;
                    case Parallax.Simple_Detail:
                        vertexFunction = "calc_parallax_simple_vs";
                        pixelFunction = "calc_parallax_simple_detail_ps";
                        break;
                }
            }

            if (methodName == "misc")
            {
                switch ((Misc)option)
                {
                    case Misc.First_Person_Never:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                    case Misc.First_Person_Sometimes:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                    case Misc.First_Person_Always:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                    case Misc.First_Person_Never_With_Rotating_Bitmaps:
                        vertexFunction = "invalid";
                        pixelFunction = "1";
                        break;
                    case Misc.Default:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                    case Misc.Rotating_Bitmaps_Super_Slow:
                        vertexFunction = "invalid";
                        pixelFunction = "1";
                        break;
                    case Misc.Always_Calc_Albedo:
                        vertexFunction = "invalid";
                        pixelFunction = "0";
                        break;
                }
            }

            if (methodName == "distortion")
            {
                switch ((Distortion)option)
                {
                    case Distortion.Off:
                        vertexFunction = "distort_nocolor_vs";
                        pixelFunction = "distort_off_ps";
                        break;
                    case Distortion.On:
                        vertexFunction = "distort_nocolor_vs";
                        pixelFunction = "distort_on_ps";
                        break;
                }
            }

            if (methodName == "soft_fade")
            {
                switch ((Soft_Fade)option)
                {
                    case Soft_Fade.Off:
                        vertexFunction = "invalid";
                        pixelFunction = "apply_soft_fade_off";
                        break;
                    case Soft_Fade.On:
                        vertexFunction = "invalid";
                        pixelFunction = "apply_soft_fade_on";
                        break;
                }
            }

            if (methodName == "misc_attr_animation")
            {
                switch ((Misc_Attr_Animation)option)
                {
                    case Misc_Attr_Animation.Off:
                        vertexFunction = "invalid"; // off (We ran out of output registers :/)
                        pixelFunction = "invalid";
                        break;
                    case Misc_Attr_Animation.Scrolling_Cube:
                        vertexFunction = "invalid"; // misc_attr_exist (We ran out of output registers :/)
                        pixelFunction = "invalid";
                        break;
                    case Misc_Attr_Animation.Scrolling_Projected:
                        vertexFunction = "invalid"; // misc_attr_exist (We ran out of output registers :/)
                        pixelFunction = "invalid";
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
                    case Wetness.Flood:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_flood_ps";
                        break;
                    case Wetness.Proof:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_proof_ps";
                        break;
                    case Wetness.Simple:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_simple_ps";
                        break;
                    case Wetness.Ripples:
                        vertexFunction = "invalid";
                        pixelFunction = "calc_wetness_ripples_ps";
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
                    case Albedo.Default:
                        break;
                    case Albedo.Detail_Blend:
                        break;
                    case Albedo.Constant_Color:
                        break;
                    case Albedo.Two_Change_Color:
                        break;
                    case Albedo.Four_Change_Color:
                        break;
                    case Albedo.Three_Detail_Blend:
                        break;
                    case Albedo.Two_Detail_Overlay:
                        break;
                    case Albedo.Two_Detail:
                        break;
                    case Albedo.Color_Mask:
                        break;
                    case Albedo.Two_Detail_Black_Point:
                        break;
                    case Albedo.Two_Change_Color_Anim_Overlay:
                        break;
                    case Albedo.Chameleon:
                        break;
                    case Albedo.Two_Change_Color_Chameleon:
                        break;
                    case Albedo.Chameleon_Masked:
                        break;
                    case Albedo.Color_Mask_Hard_Light:
                        break;
                    case Albedo.Two_Change_Color_Tex_Overlay:
                        break;
                    case Albedo.Chameleon_Albedo_Masked:
                        break;
                    case Albedo.Custom_Cube:
                        break;
                    case Albedo.Two_Color:
                        break;
                    case Albedo.Scrolling_Cube_Mask:
                        break;
                    case Albedo.Scrolling_Cube:
                        break;
                    case Albedo.Scrolling_Texture_Uv:
                        break;
                    case Albedo.Texture_From_Misc:
                        break;
                    case Albedo.Four_Change_Color_Applying_To_Specular:
                        break;
                    case Albedo.Simple:
                        break;
                    case Albedo.Emblem:
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
                    case Bump_Mapping.Detail_Masked:
                        break;
                    case Bump_Mapping.Detail_Plus_Detail_Masked:
                        break;
                    case Bump_Mapping.Detail_Unorm:
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

            if (methodName == "alpha_test")
            {
                switch ((Alpha_Test)option)
                {
                    case Alpha_Test.None:
                        break;
                    case Alpha_Test.Simple:
                        break;
                }
            }

            if (methodName == "specular_mask")
            {
                switch ((Specular_Mask)option)
                {
                    case Specular_Mask.No_Specular_Mask:
                        break;
                    case Specular_Mask.Specular_Mask_From_Diffuse:
                        break;
                    case Specular_Mask.Specular_Mask_From_Texture:
                        break;
                    case Specular_Mask.Specular_Mask_From_Color_Texture:
                        break;
                    case Specular_Mask.Specular_Mask_Mult_Diffuse:
                        break;
                }
            }

            if (methodName == "material_model")
            {
                switch ((Material_Model)option)
                {
                    case Material_Model.Diffuse_Only:
                        break;
                    case Material_Model.Cook_Torrance:
                        break;
                    case Material_Model.Two_Lobe_Phong:
                        break;
                    case Material_Model.Foliage:
                        break;
                    case Material_Model.None:
                        break;
                    case Material_Model.Glass:
                        break;
                    case Material_Model.Organism:
                        break;
                    case Material_Model.Single_Lobe_Phong:
                        break;
                    case Material_Model.Car_Paint:
                        break;
                    case Material_Model.Cook_Torrance_Custom_Cube:
                        break;
                    case Material_Model.Cook_Torrance_Pbr_Maps:
                        break;
                    case Material_Model.Cook_Torrance_Two_Color_Spec_Tint:
                        break;
                    case Material_Model.Two_Lobe_Phong_Tint_Map:
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube_Mask:
                        break;
                    case Material_Model.Cook_Torrance_Rim_Fresnel:
                        break;
                    case Material_Model.Cook_Torrance_Scrolling_Cube:
                        break;
                    case Material_Model.Cook_Torrance_From_Albedo:
                        break;
                    case Material_Model.Hair:
                        break;
                    case Material_Model.Cook_Torrance_Reach:
                        break;
                    case Material_Model.Two_Lobe_Phong_Reach:
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
                    case Environment_Mapping.Custom_Map:
                        break;
                    case Environment_Mapping.From_Flat_Texture_As_Cubemap:
                        break;
                    case Environment_Mapping.Dynamic_Reach:
                        break;
                }
            }

            if (methodName == "self_illumination")
            {
                switch ((Self_Illumination)option)
                {
                    case Self_Illumination.Off:
                        break;
                    case Self_Illumination.Simple:
                        break;
                    case Self_Illumination._3_Channel_Self_Illum:
                        break;
                    case Self_Illumination.Plasma:
                        break;
                    case Self_Illumination.From_Diffuse:
                        break;
                    case Self_Illumination.Illum_Detail:
                        break;
                    case Self_Illumination.Meter:
                        break;
                    case Self_Illumination.Self_Illum_Times_Diffuse:
                        break;
                    case Self_Illumination.Simple_With_Alpha_Mask:
                        break;
                    case Self_Illumination.Simple_Four_Change_Color:
                        break;
                    case Self_Illumination.Illum_Detail_World_Space_Four_Cc:
                        break;
                    case Self_Illumination.Illum_Change_Color:
                        break;
                    case Self_Illumination.Multilayer_Additive:
                        break;
                    case Self_Illumination.Palettized_Plasma:
                        break;
                    case Self_Illumination.Change_Color:
                        break;
                    case Self_Illumination.Change_Color_Detail:
                        break;
                }
            }

            if (methodName == "blend_mode")
            {
                switch ((Blend_Mode)option)
                {
                    case Blend_Mode.Opaque:
                        break;
                    case Blend_Mode.Additive:
                        break;
                    case Blend_Mode.Multiply:
                        break;
                    case Blend_Mode.Alpha_Blend:
                        break;
                    case Blend_Mode.Double_Multiply:
                        break;
                    case Blend_Mode.Pre_Multiplied_Alpha:
                        break;
                }
            }

            if (methodName == "parallax")
            {
                switch ((Parallax)option)
                {
                    case Parallax.Off:
                        break;
                    case Parallax.Simple:
                        break;
                    case Parallax.Interpolated:
                        break;
                    case Parallax.Simple_Detail:
                        break;
                }
            }

            if (methodName == "misc")
            {
                switch ((Misc)option)
                {
                    case Misc.First_Person_Never:
                        break;
                    case Misc.First_Person_Sometimes:
                        break;
                    case Misc.First_Person_Always:
                        break;
                    case Misc.First_Person_Never_With_Rotating_Bitmaps:
                        break;
                    case Misc.Default:
                        break;
                    case Misc.Rotating_Bitmaps_Super_Slow:
                        break;
                    case Misc.Always_Calc_Albedo:
                        break;
                }
            }

            if (methodName == "distortion")
            {
                switch ((Distortion)option)
                {
                    case Distortion.Off:
                        break;
                    case Distortion.On:
                        break;
                }
            }

            if (methodName == "soft_fade")
            {
                switch ((Soft_Fade)option)
                {
                    case Soft_Fade.Off:
                        break;
                    case Soft_Fade.On:
                        break;
                }
            }

            if (methodName == "misc_attr_animation")
            {
                switch ((Misc_Attr_Animation)option)
                {
                    case Misc_Attr_Animation.Off:
                        break;
                    case Misc_Attr_Animation.Scrolling_Cube:
                        break;
                    case Misc_Attr_Animation.Scrolling_Projected:
                        break;
                }
            }

            if (methodName == "wetness")
            {
                switch ((Wetness)option)
                {
                    case Wetness.Default:
                        break;
                    case Wetness.Flood:
                        break;
                    case Wetness.Proof:
                        break;
                    case Wetness.Simple:
                        break;
                    case Wetness.Ripples:
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
