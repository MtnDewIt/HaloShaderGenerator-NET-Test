
namespace HaloShaderGenerator.Particle
{
    public enum ParticleMethods
    {
        Albedo,
        Blend_Mode,
        Specialized_Rendering,
        Lighting,
        Render_Targets,
        Depth_Fade,
        Black_Point,
        Fog,
        Frame_Blend,
        Self_Illumination,
    }

    public enum Albedo
    {
        Diffuse_Only,
        Diffuse_Plus_Billboard_Alpha,
        Palettized,
        Palettized_Plus_Billboard_Alpha,
        Diffuse_Plus_Sprite_Alpha,
        Palettized_Plus_Sprite_Alpha,
        Diffuse_Modulated, // odst
        Palettized_Glow, // odst (unsupported)
        Palettized_Plasma, // odst
        Palettized_2d_Plasma // reach
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

    public enum Specialized_Rendering
    {
        None,
        Distortion,
        Distortion_Expensive,
        Distortion_Diffuse,
        Distortion_Expensive_Diffuse,
    }

    public enum Lighting
    {
        None,
        Per_Pixel_Ravi_Order_3,
        Per_Vertex_Ravi_Order_0,
    }

    public enum Render_Targets
    {
        Ldr_And_Hdr,
        Ldr_Only,
    }

    public enum Depth_Fade
    {
        Off,
        On,
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

    public enum Frame_Blend
    {
        Off,
        On,
    }

    public enum Self_Illumination
    {
        None,
        Constant_Color,
    }
}
