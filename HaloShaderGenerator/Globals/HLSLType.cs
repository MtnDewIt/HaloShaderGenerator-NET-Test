using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloShaderGenerator.Globals
{
    public enum HLSLType
    {
        Float,
        Float2,
        Float3,
        Float4,

        Xform_2d,
        Xform_3d,

        Sampler,
        sampler2D,
        sampler3D,

        Int,
        Int2,
        Int3,
        Int4,

        Bool,
        Bool2,
        Bool3,
        Bool4
    }
}
