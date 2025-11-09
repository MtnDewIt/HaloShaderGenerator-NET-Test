using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.ZOnly
{
    public class ZOnlyGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(ZOnlyMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (ZOnlyMethods)methodIndex switch
            {
                ZOnlyMethods.Test => Enum.GetValues(typeof(Test)).Length,
                _ => -1,
            };
        }

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint) => -1;

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint) => false;

        public bool IsPixelShaderShared(ShaderStage entryPoint) => false;

        public bool IsAutoMacro() => true;

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

            if (methodName == "test")
            {
                optionName = ((Test)option).ToString();

                switch ((Test)option)
                {
                    case Test.Default:
                        result.AddSamplerWithScaleParameter("base_map", 1.0f, @"shaders\default_bitmaps\bitmaps\gray_50_percent");
                        result.AddSamplerWithScaleParameter("detail_map", 16.0f, @"shaders\default_bitmaps\bitmaps\default_detail");
                        result.AddFloat4ColorWithFloatAndIntegerParameter("albedo_color", 1.0f, 1, new ShaderColor(255, 255, 255, 255));
                        rmopName = @"shaders\shader_options\albedo_default";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames() => Enum.GetValues(typeof(ZOnlyMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (ZOnlyMethods)methodIndex switch
            {
                ZOnlyMethods.Test => Enum.GetValues(typeof(Test)),
                _ => null,
            };
        }

        public Array GetEntryPointOrder()
        {
            return new ShaderStage[]
            {
                ShaderStage.Z_Only
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
            return (ZOnlyMethods)category switch
            {
                ZOnlyMethods.Test => "test_ps",
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (ZOnlyMethods)category switch
            {
                ZOnlyMethods.Test => "test_vs",
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (ZOnlyMethods)category switch
            {
                ZOnlyMethods.Test => (Test)option switch
                {
                    Test.Default => "calc_albedo_default_ps",
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (ZOnlyMethods)category switch
            {
                ZOnlyMethods.Test => (Test)option switch
                {
                    Test.Default => "calc_albedo_default_vs",
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
