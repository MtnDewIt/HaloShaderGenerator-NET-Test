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
        Dynamic,
        Dynamic_Reach
    }

    public enum Material
    {
        Diffuse_Only,
        Diffuse_Plus_Specular,
        Off,
        Diffuse_Only_Plus_Self_Illum,
        Diffuse_Plus_Specular_Plus_Self_Illum,
        Diffuse_Plus_Specular_Plus_Heightmap,
        Diffuse_Plus_Two_Detail,
        Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap
    }

    public enum Material1
    {
        Diffuse_Only,
        Diffuse_Plus_Specular,
        Off,
        Diffuse_Only_Plus_Self_Illum,
        Diffuse_Plus_Specular_Plus_Self_Illum,
        Diffuse_Plus_Specular_Plus_Heightmap,
        Diffuse_Plus_Specular_Plus_Up_Vector_Plus_Heightmap
    }

    public enum Material2
    {
        Diffuse_Only,
        Diffuse_Plus_Specular,
        Off,
        Diffuse_Only_Plus_Self_Illum,
        Diffuse_Plus_Specular_Plus_Self_Illum,
    }

    public enum Material_No_Detail_Bump
    {
        Off,
        Diffuse_Only,
        Diffuse_Plus_Specular
    }


}
