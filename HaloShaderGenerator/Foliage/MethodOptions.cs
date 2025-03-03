namespace HaloShaderGenerator.Foliage
{
    public enum FoliageMethods
    {
        Albedo,
        Alpha_Test,
        Material_Model,
        Wetness
    }

    public enum Albedo
    {
        Default,
        Simple
    }

    public enum Alpha_Test
    {
        None,
        Simple,
        From_Albedo_Alpha,
        From_Texture
    }

    public enum Material_Model
    {
        Default,
        Flat,
        Specular,
        Translucent
    }

    public enum Wetness
    {
        Simple,
        Proof
    }
}