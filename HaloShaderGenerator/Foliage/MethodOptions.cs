
namespace HaloShaderGenerator.Foliage
{
    public enum FoliageMethods
    {
        Albedo,
        Alpha_Test,
        Material_Model
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
        From_Albedo_Alpha
    }

    public enum Material_Model
    {
        Default
    }
}
