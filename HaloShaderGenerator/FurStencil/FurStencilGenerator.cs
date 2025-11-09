using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;
using System;

namespace HaloShaderGenerator.FurStencil
{
    public class FurStencilGenerator : IShaderGenerator
    {
        public int GetMethodCount() => Enum.GetValues(typeof(FurStencilMethods)).Length;

        public int GetMethodOptionCount(int methodIndex)
        {
            return (FurStencilMethods)methodIndex switch
            {
                FurStencilMethods.Alpha_Test => Enum.GetValues(typeof(Alpha_Test)).Length,
                _ => -1,
            };
        }

        public int GetSharedPixelShaderCategory(ShaderStage entryPoint)
        {
            return entryPoint switch
            {
                ShaderStage.Default or 
                ShaderStage.Shadow_Generate => 0,
                _ => -1,
            };
        }

        public bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint)
        {
            return entryPoint switch
            {
                ShaderStage.Default or 
                ShaderStage.Shadow_Generate => true,
                _ => false,
            };
        }

        public bool IsPixelShaderShared(ShaderStage entryPoint)
        {
            return entryPoint switch
            {
                ShaderStage.Default or 
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

            if (methodName == "alpha_test")
            {
                optionName = ((Alpha_Test)option).ToString();

                switch ((Alpha_Test)option)
                {
                    case Alpha_Test.Off:
                        break;
                    case Alpha_Test.On:
                        result.AddSamplerParameter("alpha_test_map", @"shaders\default_bitmaps\bitmaps\default_alpha_test");
                        rmopName = @"shaders\shader_options\alpha_test_on";
                        break;
                }
            }
            return result;
        }

        public Array GetMethodNames() => Enum.GetValues(typeof(FurStencilMethods));

        public Array GetMethodOptionNames(int methodIndex)
        {
            return (FurStencilMethods)methodIndex switch
            {
                FurStencilMethods.Alpha_Test => Enum.GetValues(typeof(Alpha_Test)),
                _ => null,
            };
        }

        public Array GetEntryPointOrder()
        {
            return new ShaderStage[]
            {
                ShaderStage.Default,
                ShaderStage.Shadow_Generate
            };
        }

        public Array GetVertexTypeOrder()
        {
            return new VertexType[]
            {
                VertexType.Rigid,
                VertexType.Skinned
            };
        }

        public string GetCategoryPixelFunction(int category)
        {
            return (FurStencilMethods)category switch
            {
                FurStencilMethods.Alpha_Test => "calc_alpha_test_ps",
                _ => null,
            };
        }

        public string GetCategoryVertexFunction(int category)
        {
            return (FurStencilMethods)category switch
            {
                FurStencilMethods.Alpha_Test => string.Empty,
                _ => null,
            };
        }

        public string GetOptionPixelFunction(int category, int option)
        {
            return (FurStencilMethods)category switch
            {
                FurStencilMethods.Alpha_Test => (Alpha_Test)option switch
                {
                    Alpha_Test.Off => "calc_alpha_test_off_ps",
                    Alpha_Test.On => "calc_alpha_test_on_ps",
                    _ => null,
                },
                _ => null,
            };
        }

        public string GetOptionVertexFunction(int category, int option)
        {
            return (FurStencilMethods)category switch
            {
                FurStencilMethods.Alpha_Test => (Alpha_Test)option switch
                {
                    Alpha_Test.Off => string.Empty,
                    Alpha_Test.On => string.Empty,
                    _ => null,
                },
                _ => null,
            };
        }
    }
}
