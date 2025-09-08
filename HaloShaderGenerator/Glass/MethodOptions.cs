namespace HaloShaderGenerator.Glass
{
    public enum GlassMethods
    {
        Albedo,
        Bump_Mapping,
        Material_Model,
        Environment_Mapping,
        Wetness,
        Alpha_Blend_Source
    }

    public enum Albedo
    {
        Map
    }

    public enum Bump_Mapping
    {
        Off,
        Standard,
        Detail,
        Detail_Blend,
        Three_Detail_Blend,
        Standard_Wrinkle,
        Detail_Wrinkle
    }

    public enum Material_Model
    {
        Two_Lobe_Phong_Reach
    }

    public enum Environment_Mapping
    {
        None,
        Per_Pixel,
        Dynamic,
        From_Flat_Texture
    }

    public enum Wetness
    {
        Simple,
        Flood
    }

    public enum Alpha_Blend_Source
    {
        From_Albedo_Alpha_Without_Fresnel,
        From_Albedo_Alpha,
        From_Opacity_Map_Alpha,
        From_Opacity_Map_Rgb,
        From_Opacity_Map_Alpha_And_Albedo_Alpha
    }
}