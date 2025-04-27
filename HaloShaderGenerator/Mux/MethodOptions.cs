namespace HaloShaderGenerator.Mux
{
    public enum MuxMethods
    {
        Blending,
        Albedo,
        Bump,
        Materials,
        Environment_Mapping,
        Parallax,
        Wetness
    }

    public enum Blending
    {
        Standard
    }

    public enum Albedo
    {
        Base_Only,
        Base_And_Detail
    }

    public enum Bump
    {
        Base_Only,
        Base_And_Detail
    }

    public enum Materials
    {
        Diffuse_Only,
        Single_Lobe_Phong
    }

    public enum Environment_Mapping
    {
        None,
        Per_Pixel,
        Dynamic
    }

    public enum Parallax
    {
        Off,
        Simple
    }

    public enum Wetness
    {
        Default,
        Flood,
        Proof,
        Ripples
    }
}