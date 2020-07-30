using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloShaderGenerator.Terrain
{
    public enum TerrainMethods
    {
        Blending,
        Environment_Map,
        Material_0,
        Material_1,
        Material_2,
        Material_3
    }

    public enum Blending
    {
        Morph,
        Dynamic_Morph
    }

    public enum Environment_Mapping
    {
        None,
        Per_Pixel,
        Dynamic
    }

    public enum Material
    {
        Diffuse_Only,
        Diffuse_Plus_Specular,
        Off,
    }

    public enum Material_No_Detail_Bump
    {
        Off,
        Diffuse_Only,
        Diffuse_Plus_Specular
    }


}
