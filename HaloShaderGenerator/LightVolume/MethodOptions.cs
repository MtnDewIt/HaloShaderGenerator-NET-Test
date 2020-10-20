
namespace HaloShaderGenerator.LightVolume
{
    public enum LightVolumeMethods
    {
        Albedo,
        Blend_Mode,
        Fog
    }

    public enum Albedo
    {
        Diffuse_Only,
        Circular
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

    public enum Fog
    {
        Off,
        On,
    }
}
