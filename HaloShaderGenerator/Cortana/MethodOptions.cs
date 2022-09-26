
namespace HaloShaderGenerator.Cortana
{
    public enum CortanaMethods
    {
        Albedo,
        Bump_Mapping,
        Alpha_Test,
        Material_Model,
        Environment_Mapping,
        Warp,
        Lighting,
        Scanlines,
        Transparency
    }

    public enum Albedo
    {
        Default
    }

    public enum Bump_Mapping
    {
        Standard
    }

    public enum Alpha_Test
    {
        None,
        Simple
    }

    public enum Material_Model
    {
        Cook_Torrance
    }

    public enum Environment_Mapping
    {
        None,
        Per_Pixel,
        Dynamic,
        Dynamic_Reach
    }

    public enum Warp
    {
        Default
    }

    public enum Lighting
    {
        Default
    }

    public enum Scanlines
    {
        Default
    }

    public enum Transparency
    {
        Default
    }
}
