using System.Collections.Generic;
using HaloShaderGenerator.Globals;

namespace HaloShaderGenerator.Generic
{
    public class GenericShaderStage
    {
        // This needs to be eliminated -- there are entry point / vertex types defines in the shader files themselves

        static public List<ShaderStage> GetExplicitEntryPoints(ExplicitShader explicitShader)
        {
            switch (explicitShader)
            {
                case ExplicitShader.shadow_apply:
                case ExplicitShader.shadow_apply2:
                    return new List<ShaderStage> { ShaderStage.Default, ShaderStage.Albedo };
                case ExplicitShader.water_ripple:
                    return new List<ShaderStage> { ShaderStage.Default, ShaderStage.Albedo, ShaderStage.Dynamic_Light, ShaderStage.Shadow_Apply, ShaderStage.Active_Camo };
                default:
                    return new List<ShaderStage> { ShaderStage.Default };
            }
        }

        static public List<ShaderStage> GetChudEntryPoints(ChudShader explicitShader)
        {
            switch (explicitShader)
            {
                case ChudShader.chud_turbulence:
                    return new List<ShaderStage> { ShaderStage.Default, ShaderStage.Albedo, ShaderStage.Dynamic_Light };
                default:
                    return new List<ShaderStage> { ShaderStage.Default };
            }
        }

        static public List<VertexType> GetChudVertexTypes(ChudShader explicitShader)
        {
            switch (explicitShader)
            {
                case ChudShader.chud_sensor:
                    return new List<VertexType> { VertexType.FancyChud };
                default:
                    return new List<VertexType> { VertexType.SimpleChud };
            }
        }
    }
}
