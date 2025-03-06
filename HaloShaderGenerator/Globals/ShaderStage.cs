using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloShaderGenerator.Globals
{
    public enum ShaderStage
    {
        Default,
        Albedo,
        Static_Default,
        Static_Per_Pixel,
        Static_Per_Vertex,
        Static_Sh,
        Static_Prt_Ambient,
        Static_Prt_Linear,
        Static_Prt_Quadratic,
        Dynamic_Light,
        Shadow_Generate,
        Shadow_Apply,
        Active_Camo,
        Lightmap_Debug_Mode,
        Static_Per_Vertex_Color,
        Water_Tessellation,
        Water_Shading,
        Dynamic_Light_Cinematic,
        Stipple,
        Single_Pass_Per_Pixel,
        Single_Pass_Per_Vertex,
        //Single_Pass_Single_Probe,
        //Single_Pass_Single_Probe_Ambient,
        //Imposter_Static_Sh,
        //Imposter_Static_Prt_Ambient,
        //Dynamic_Light_Hq_Shadows,
        //Dynamic_Light_Hq_Cinematic_Shadows,
        //Static_Nv_Sh,
        //Kernel5_Non_Xenon_Output,
        //Depth_To_Rgba_Pack,
        //Linear_Depth_Downsample,
        Z_Only,
        Sfx_Distort,
    }
}
