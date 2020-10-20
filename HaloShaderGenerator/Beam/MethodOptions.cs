
namespace HaloShaderGenerator.Beam
{
    public enum BeamMethods
    {
        Albedo,
        Blend_Mode,
        Black_Point,
        Fog
    }

    public enum Albedo
    {
        Diffuse_Only,
        Palettized,
        Palettized_Plus_Alpha,
        Palettized_Plasma,
        Palettized_2d_Plasma
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
        Pre_Multiplied_Alpha,
    }

    public enum Black_Point
    {
        Off,
        On,
    }

    public enum Fog
    {
        Off,
        On,
    }
}
