using System;
using System.Collections.Generic;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shared;

namespace HaloShaderGenerator.Black
{
    public class BlackGenerator : IShaderGenerator
    {
        public int GetMethodCount()
        {
            return Enum.GetValues(typeof(BlackMethods)).Length;
        }

        public int GetMethodOptionCount(int methodIndex)
        {
            return 1;
        }

        public bool IsEntryPointSupported(ShaderStage entryPoint)
        {
            switch (entryPoint)
            {
                case ShaderStage.Albedo:
                case ShaderStage.Stipple:
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
                case ShaderStage.Albedo:
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
            return result;
        }

        public Array GetMethodNames()
        {
            return Enum.GetValues(typeof(BlackMethods));
        }

        public Array GetMethodOptionNames(int methodIndex)
        {
            return null;
        }

        public void GetCategoryFunctions(string methodName, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;
        }

        public void GetOptionFunctions(string methodName, int option, out string vertexFunction, out string pixelFunction)
        {
            vertexFunction = null;
            pixelFunction = null;
        }

        public ShaderParameters GetParameterArguments(string methodName, int option)
        {
            ShaderParameters result = new ShaderParameters();
            return result;
        }
    }
}
