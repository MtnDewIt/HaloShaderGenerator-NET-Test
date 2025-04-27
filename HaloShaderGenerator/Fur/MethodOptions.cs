namespace HaloShaderGenerator.Fur
{
    public enum FurMethods
    {
        Albedo,
        Warp,
        Overlay,
        Edge_Fade,
        Blend_Mode
    }

    public enum Albedo
    {
        Fur_Multilayer
    }

    public enum Warp
    {
        None,
        From_Texture,
        Parallax_Simple
    }

    public enum Overlay
    {
        None,
        Additive,
        Additive_Detail,
        Multiply,
        Multiply_And_Additive_Detail
    }

    public enum Edge_Fade
    {
        None,
        Simple
    }

    public enum Blend_Mode
    {
        Opaque,
        Alpha_Blend
    }
}