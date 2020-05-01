using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloShaderGenerator.Globals
{
    public static class RegisterScopes
    {
        public static RegisterScope GetVertexShaderRegisterScope(string name)
        {
            return RegisterScope.Global_Arguments;
        }

        public static RegisterScope GetPixelShaderRegisterScope(string name)
        {
            return RegisterScope.Global_Arguments;
        }
    }


    public enum RegisterScope
    {
        TextureSampler_Arguments,
        WaterVector,
        UnknownA,
        UnknownB,
        Vector_Arguments,
        Integer_Arguments,
        Global_Arguments,
        RenderMethodExtern_Arguments,
        UnknownD,
        UnknownE,
        RenderMethodExternVector_Arguments,
        UnknownF,
        UnknownG,
        UnknownH,
        Indirect
    }
}
