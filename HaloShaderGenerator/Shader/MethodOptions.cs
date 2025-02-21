using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloShaderGenerator.Shader
{
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
        Soft_Fade,
        Misc_Attr_Animation,
        Wetness,
        Alpha_Blend_Source
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
        Two_Change_Color_Tex_Overlay,
        Chameleon_Albedo_Masked,
        Custom_Cube,
        Two_Color,
        Scrolling_Cube_Mask,
        Scrolling_Cube,
        Scrolling_Texture_Uv,
        Texture_From_Misc,
        Four_Change_Color_Applying_To_Specular,
        Simple,
        Emblem
    }

    public enum Bump_Mapping
    {
        Off,
        Standard,
        Detail,
        Detail_Masked,
        Detail_Plus_Detail_Masked,
        Detail_Unorm,
        Detail_Blend,
        Three_Detail_Blend,
        Standard_Wrinkle,
        Detail_Wrinkle,
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
        Specular_Mask_From_Color_Texture,
        Specular_Mask_Mult_Diffuse
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
        Cook_Torrance_Custom_Cube,
        Cook_Torrance_Pbr_Maps,
        Cook_Torrance_Two_Color_Spec_Tint,
        Two_Lobe_Phong_Tint_Map,
        Cook_Torrance_Scrolling_Cube_Mask,
        Cook_Torrance_Rim_Fresnel,
        Cook_Torrance_Scrolling_Cube,
        Cook_Torrance_From_Albedo,
        Hair,
        Cook_Torrance_Reach,
        Two_Lobe_Phong_Reach
    }

    public enum Environment_Mapping
    {
        None,
        Per_Pixel,
        Dynamic,
        From_Flat_Texture,
        Custom_Map,
        From_Flat_exture_As_Cubemap,
        Dynamic_Reach
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
        Illum_Detail_World_Space_Four_Cc,
        Illum_Change_Color,
        Multilayer_Additive,
        Palettized_Plasma,
        Change_Color,
        Change_Color_Detail
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
        First_Person_Never_With_rotating_Bitmaps,
        Default,
        Rotating_Bitmaps_Super_Slow,
        Always_Calc_Albedo
    }

    public enum Misc_Attr_Animation 
    {
        Off,
        Scrolling_Cube,
        Scrolling_Projected
    }

    public enum Wetness
    {
        Default,
        Flood,
        Proof,
        Simple,
        Ripples
    }
}
