
namespace HaloShaderGenerator.Custom
{
    public enum CustomMethods
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
        Waterfall,
        Multiply_Map,
    }

    public enum Bump_Mapping
    {
        Off,
        Standard,
        Detail,
    }

    public enum Alpha_Test
    {
        None,
        Simple,
        Multiply_Map,
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
        Two_Lobe_Phong,
        Foliage,
        None,
        Custom_Specular
    }

    public enum Environment_Mapping
    {
        None,
        Per_Pixel,
        Dynamic,
        From_Flat_Texture,
        Per_Pixel_Mip,
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
        Window_Room
    }

    public enum Blend_Mode
    {
        Opaque,
        Additive,
        Multiply,
        Alpha_Blend,
        Double_Multiply,
    }

    public enum Parallax
    {
        Off,
        Simple,
        Interpolated,
        Simple_Detail,
    }

    public enum Misc
    {
        First_Person_Never,
        First_Person_Sometimes,
        First_Person_Always,
        First_Person_Never_With_rotating_Bitmaps,
        Always_Calc_Albedo
    }
}