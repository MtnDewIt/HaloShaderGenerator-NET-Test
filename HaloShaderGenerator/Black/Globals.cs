using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Generator;
using HaloShaderGenerator.Globals;

namespace HaloShaderGenerator.Black
{
    public static class Globals
    {
        public static bool IsVertexTypeSupported(VertexType type)
        {
            switch (type)
            {
                case VertexType.World:
                case VertexType.Rigid:
                case VertexType.Skinned:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsShaderStageSupported(ShaderStage stage)
        {
            switch (stage)
            {
                case ShaderStage.Albedo:
                    return true;
                default:
                    return false;
            }
        }
    }

    

}
