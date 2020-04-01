using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;

namespace HaloShaderGenerator.Shader
{
    public partial class ShaderGenerator
    {
        public static byte[] GenerateShader(
            ShaderStage stage,
            Albedo albedo,
            Bump_Mapping bump_mapping,
            Alpha_Test alpha_test,
            Specular_Mask specular_mask,
            Material_Model material_model,
            Environment_Mapping environment_mapping,
            Self_Illumination self_illumination,
            Blend_Mode blend_mode,
            Parallax parallax,
            Misc misc,
            Distortion distortion,
            Soft_fade soft_fade
            )
        {
            string template = $"shader.hlsl";

            List<D3D.SHADER_MACRO> macros = new List<D3D.SHADER_MACRO>();

            switch (stage)
            {
                
                case ShaderStage.Albedo:
                case ShaderStage.Static_Prt_Ambient:
                case ShaderStage.Static_Prt_Linear:
                case ShaderStage.Static_Prt_Quadratic:
                case ShaderStage.Active_Camo:
                    break;
                default:
                    case ShaderStage.Default:
                    case ShaderStage.Static_Per_Pixel:
                    case ShaderStage.Static_Per_Vertex:
                    case ShaderStage.Static_Sh:
                    case ShaderStage.Dynamic_Light:
                    case ShaderStage.Shadow_Generate:
                    case ShaderStage.Lightmap_Debug_Mode:
                    case ShaderStage.Static_Per_Vertex_Color:
                    case ShaderStage.Dynamic_Light_Cinematic:
                    case ShaderStage.Sfx_Distort:
                    Console.Error.WriteLine($"Shader stage {stage} not supported in Shader generator");
                    return null;
            }

            //
            // Create all the macros required
            //

            // prevent the definition helper from being included
            macros.Add(new D3D.SHADER_MACRO { Name = "_DEFINITION_HELPER_HLSLI", Definition = "1" });

            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderStage>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<ShaderType>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Albedo>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Bump_Mapping>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Alpha_Test>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Specular_Mask>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Material_Model>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Environment_Mapping>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Self_Illumination>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Blend_Mode>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Parallax>());
            macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Misc>());
            //macros.AddRange(ShaderGeneratorBase.CreateMethodEnumDefinitions<Distortion>());

            //
            // The following code properly names the macros (like in rmdf)
            //
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_ps", albedo, "calc_albedo_", "_ps"));

            if (albedo == Albedo.Constant_Color)
            {
                macros.Add(ShaderGeneratorBase.CreateMacro("calc_albedo_vs", albedo, "calc_albedo_", "_vs"));
            }

            macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_ps", bump_mapping, "calc_bumpmap_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_bumpmap_vs", bump_mapping, "calc_bumpmap_", "_vs"));

            macros.Add(ShaderGeneratorBase.CreateMacro("calc_alpha_test_ps", alpha_test, "calc_alpha_test_", "_ps"));
            macros.Add(ShaderGeneratorBase.CreateMacro("calc_specular_mask_ps", specular_mask, "calc_specular_mask_", "_ps"));

            macros.Add(ShaderGeneratorBase.CreateMacro("calc_self_illumination_ps", self_illumination, "calc_self_illumination_", "_ps"));

            macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_ps", parallax, "calc_parallax_", "_ps"));
            switch (parallax)
            {
                case Parallax.Simple_Detail:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", Parallax.Simple, "calc_parallax_", "_vs"));
                    break;
                default:
                    macros.Add(ShaderGeneratorBase.CreateMacro("calc_parallax_vs", parallax, "calc_parallax_", "_vs"));
                    break;
            }

            macros.Add(ShaderGeneratorBase.CreateMacro("material_type", material_model, "material_type_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type", environment_mapping, "envmap_type_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("blend_type", blend_mode, "blend_type_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("shaderstage", stage, "shaderstage_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("shadertype", stage, "shadertype_"));

            macros.Add(ShaderGeneratorBase.CreateMacro("albedo_arg", albedo, "k_albedo_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("self_illumination_arg", self_illumination, "k_self_illumination_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("material_type_arg", material_model, "k_material_model_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("envmap_type_arg", environment_mapping, "k_environment_mapping_"));
            macros.Add(ShaderGeneratorBase.CreateMacro("blend_type_arg", blend_mode, "k_blend_mode_"));

            // add misc option macros here

            //macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_ps", distortion, "distort_", "_ps"));
            //macros.Add(ShaderGeneratorBase.CreateMacro("distort_proc_vs", "nocolor", "distort_", "_vs"));


            byte[] shaderBytecode;

            using(FileStream test = new FileInfo("test.hlsl").Create())
            using(StreamWriter writer = new StreamWriter(test))
            {
                shaderBytecode = ShaderGeneratorBase.GenerateSource(template, macros, "entry_" + stage.ToString().ToLower(), "ps_3_0", writer);
            }

            return shaderBytecode;
        }
    }

    public enum ShaderMethods
    {
        Albedo,
        Bump_Mapping,
        Alpha_Test,
        Specular_Mask,
        Material_Model,
        Environment_Mapping,
        Self_Illumination,
        Blend_Mode,
        Parallax,
        Misc,
        Distortion,
        //Soft_Fade
    }

    public enum Albedo
    {
        Default,
        Detail_Blend,
        Constant_Color,
        Two_Change_Color,
        Four_Change_Color,
        Three_Detail_Blend,
        Two_Detail_Overlay,
        Two_Detail,
        Color_Mask,
        Two_Detail_Black_Point,
        Two_Change_Color_Anim_Overlay,
        Chameleon,
        Two_Change_Color_Chameleon,
        Chameleon_Masked,
        Color_Mask_Hard_Light,
        //Two_Change_Color_Tex_Overlay,
        //Chameleon_Albedo_Masked
    }

    public enum Bump_Mapping
    {
        Off,
        Standard,
        Detail,
        Detail_Masked,
        //Detail_Plus_Detail_Masked
    }

    public enum Alpha_Test
    {
        None,
        Simple
    }

    public enum Specular_Mask
    {
        No_Specular_Mask,
        Specular_Mask_From_Diffuse,
        Specular_Mask_From_Texture,
        Specular_Mask_From_Color_Texture
    }

    public enum Material_Model
    {
        Diffuse_Only,
        Cook_Torrance,
        Two_Lobe_Phong,
        Foliage,
        None,
        Glass,
        Organism,
        Single_Lobe_Phong,
        Car_Paint,
        //Hair
    }

    public enum Environment_Mapping
    {
        None,
        Per_Pixel,
        Dynamic,
        From_Flat_Texture,
        Custom_Map
    }

    public enum Self_Illumination
    {
        Off,
        Simple,
        _3_Channel_Self_Illum,
        Plasma,
        From_Diffuse,
        Illum_Detail,
        Meter,
        Self_Illum_Times_Diffuse,
        Simple_With_Alpha_Mask,
        Simple_Four_Change_Color,
        //Illum_Detail_World_Space_Four_Cc
    }

    public enum Blend_Mode
    {
        Opaque,
        Additive,
        Multiply,
        Alpha_Blend,
        Double_Multiply,
        Pre_Multiplied_Alpha
    }

    public enum Parallax
    {
        Off,
        Simple,
        Interpolated,
        Simple_Detail
    }

    public enum Misc
    {
        First_Person_Never,
        First_Person_Sometimes,
        First_Person_Always,
        First_Person_Never_With_rotating_Bitmaps
    }

    public enum Distortion
    {
        Off,
        On
    }

    public enum Soft_fade
    {
        Off,
        On
    }

}
