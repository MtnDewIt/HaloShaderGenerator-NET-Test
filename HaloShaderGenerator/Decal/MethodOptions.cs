namespace HaloShaderGenerator.Decal
{
    public enum DecalMethods
    {
        Albedo,
        Blend_Mode,
        Render_Pass,
        Specular,
        Bump_Mapping,
        Tinting,
        Parallax,
        Interier
    }

    public enum Albedo
    {
        Diffuse_Only,
        Palettized,
        Palettized_Plus_Alpha,
        Diffuse_Plus_Alpha,
        Emblem_Change_Color,
        Change_Color,
        Diffuse_Plus_Alpha_Mask,
        Palettized_Plus_Alpha_Mask,
        Vector_Alpha,
        Vector_Alpha_Drop_Shadow,
        Patchy_Emblem
    }

    public enum Blend_Mode
    {
        Opaque,
        Additive,
        Multiply,
        Alpha_Blend,
        Double_Multiply,
        Maximum,
        Multiply_Add,
        Add_Src_Times_Dstalpha,
        Add_Src_Times_Srcalpha,
        Inv_Alpha_Blend,
        Pre_Multiplied_Alpha
    }

    public enum Render_Pass
    {
        Pre_Lighting,
        Post_Lighting,
        Transparent
    }

    public enum Specular
    {
        Leave,
        Modulate
    }

    public enum Bump_Mapping
    {
        Leave,
        Standard,
        Standard_Mask
    }

    public enum Tinting
    {
        None,
        Unmodulated,
        Partially_Modulated,
        Fully_Modulated
    }

    public enum Parallax
    {
        Off,
        Simple,
        Sphere
    }

    public enum Interier
    {
        Off,
        Simple,
        Floor,
        Hole,
        Box
    }
}