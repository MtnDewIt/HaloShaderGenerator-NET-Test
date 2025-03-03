namespace HaloShaderGenerator.Shared
{
    public enum ShaderType
    {
        Shader,
        Particle,
        Decal,
        Light_Volume,
        Contrail,
        Halogram,
        Cortana,
        Terrain,
        Water,
        Foliage,
        Beam,
        Custom,
        Screen,
        Black,
        Glass,
        Fur_Stencil,
        Fur,
        Mux,
        ZOnly
    }

    public enum Albedo
    {
        Default,
        Detail_Blend,
        Constant_Color,
        Two_Change_Color,
        Four_Change_Color,
        Three_Detail_Blend,
        Two_Detail_Overlay,
        Two_Detail,
        Color_Mask,
        Two_Detail_Black_Point,
        Two_Change_Color_Anim_Overlay,
        Chameleon,
        Two_Change_Color_Chameleon,
        Chameleon_Masked,
        Color_Mask_Hard_Light,
        Two_Change_Color_Tex_Overlay,
        Chameleon_Albedo_Masked,
        Custom_Cube,
        Two_Color,
        Scrolling_Cube_Mask,
        Scrolling_Cube,
        Scrolling_Texture_Uv,
        Texture_From_Misc,
        Four_Change_Color_Applying_To_Specular,
        Simple,
        Emblem,
        Diffuse_Only,
        Diffuse_Plus_Billboard_Alpha,
        Palettized,
        Palettized_Plus_Billboard_Alpha,
        Diffuse_Plus_Sprite_Alpha,
        Palettized_Plus_Sprite_Alpha,
        Diffuse_Modulated,
        Palettized_Glow,
        Palettized_Plasma,
        Palettized_2d_Plasma,
        Palettized_Plus_Alpha,
        Diffuse_Plus_Alpha,
        Emblem_Change_Color,
        Change_Color,
        Diffuse_Plus_Alpha_Mask,
        Palettized_Plus_Alpha_Mask,
        Vector_Alpha,
        Vector_Alpha_Drop_Shadow,
        Patchy_Emblem,
        Circular,
        Waterfall,
        Multiply_Map,
        Map,
        Fur_Multilayer,
        Base_Only,
        Base_And_Detail
    }

    public enum Alpha_Test
    {
        None,
        Simple,
        From_Albedo_Alpha,
        From_Texture,
        Multiply_Map,
        Off,
        On
    }

    public enum Material_Model
    {
        Diffuse_Only,
        Cook_Torrance,
        Two_Lobe_Phong,
        Foliage,
        None,
        Glass,
        Organism,
        Single_Lobe_Phong,
        Car_Paint,
        Cook_Torrance_Custom_Cube,
        Cook_Torrance_Pbr_Maps,
        Cook_Torrance_Two_Color_Spec_Tint,
        Two_Lobe_Phong_Tint_Map,
        Cook_Torrance_Scrolling_Cube_Mask,
        Cook_Torrance_Rim_Fresnel,
        Cook_Torrance_Scrolling_Cube,
        Cook_Torrance_From_Albedo,
        Hair,
        Cook_Torrance_Reach,
        Two_Lobe_Phong_Reach,
        Default,
        Flat,
        Specular,
        Translucent,
        Custom_Specular
    }

    public enum Environment_Mapping
    {
        None,
        Per_Pixel,
        Dynamic,
        From_Flat_Texture,
        Custom_Map,
        From_Flat_Exture_As_Cubemap,
        Dynamic_Reach,
        From_Flat_Texture_As_Cubemap,
        Per_Pixel_Mip
    }

    public enum Self_Illumination
    {
        Off,
        Simple,
        _3_Channel_Self_Illum,
        Plasma,
        From_Diffuse,
        Illum_Detail,
        Meter,
        Self_Illum_Times_Diffuse,
        Simple_With_Alpha_Mask,
        Simple_Four_Change_Color,
        Illum_Detail_World_Space_Four_Cc,
        Illum_Change_Color,
        Multilayer_Additive,
        Palettized_Plasma,
        Change_Color,
        Change_Color_Detail,
        None,
        Constant_Color,
        Scope_Blur,
        Ml_Add_Four_Change_Color,
        Ml_Add_Five_Change_Color,
        Plasma_Wide_And_Sharp_Five_Change_Color,
        Self_Illum_Holograms,
        Palettized_Plasma_Change_Color,
        Palettized_Depth_Fade,
        Window_Room
    }

    public enum Blend_Mode
    {
        Opaque,
        Additive,
        Multiply,
        Alpha_Blend,
        Double_Multiply,
        Pre_Multiplied_Alpha,
        Maximum,
        Multiply_Add,
        Add_Src_Times_Dstalpha,
        Add_Src_Times_Srcalpha,
        Inv_Alpha_Blend
    }

    public enum Alpha_Blend_Source
    {
        From_Albedo_Alpha_Without_Fresnel,
        From_Albedo_Alpha,
        From_Opacity_Map_Alpha,
        From_Opacity_Map_Rgb,
        From_Opacity_Map_Alpha_And_Albedo_Alpha
    }

    public enum Depth_Fade
    {
        Off,
        On,
        Low_Res,
        Palette_Shift,
        Biased
    }

    public enum Black_Point
    {
        Off,
        On
    }

    public enum Fog
    {
        Off,
        On
    }

    public enum Distortion
    {
        Off,
        On
    }

    public enum Soft_Fade
    {
        Off,
        On
    }
}
