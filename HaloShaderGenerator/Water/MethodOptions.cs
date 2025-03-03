namespace HaloShaderGenerator.Water
{
    public enum WaterMethods
    {
        Waveshape,
        Watercolor,
        Reflection,
        Refraction,
        Bankalpha,
        Appearance,
        Global_Shape,
        Foam,
        Detail,
        Reach_Compatibility
    }

    public enum Waveshape
    {
        Default,
        None,
        Bump
    }

    public enum Watercolor
    {
        Pure,
        Texture
    }

    public enum Reflection
    {
        None,
        Static,
        Dynamic,
        Static_Ssr
    }

    public enum Refraction
    {
        None,
        Dynamic
    }

    public enum Bankalpha
    {
        None,
        Depth,
        Paint,
        From_Shape_Texture_Alpha
    }

    public enum Appearance
    {
        Default
    }

    public enum Global_Shape
    {
        None,
        Paint,
        Depth
    }

    public enum Foam
    {
        None,
        Auto,
        Paint,
        Both
    }

    public enum Detail
    {
        None,
        Repeat
    }

    public enum Reach_Compatibility
    {
        Disabled,
        Enabled,
        Enabled_Detail_Repeat
    }
}