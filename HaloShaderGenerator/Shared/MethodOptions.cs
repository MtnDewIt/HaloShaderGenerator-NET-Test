﻿
namespace HaloShaderGenerator.Shared
{
    public enum ShaderType
    {
        Shader,
        Beam,
        Contrail,
        Decal,
        Halogram,
        Light_Volume,
        Particle,
        Terrain,
        Black,
        Custom,
        Water,
        Foliage,
        Glass,
        Cortana,
        Screen
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
        Waterfall,
        Multiply_Map,
        Two_Change_Color_Anim_Overlay,
        Chameleon,
        Two_Change_Color_Chameleon,
        Chameleon_Masked,
        Color_Mask_Hard_Light,
        Two_Change_Color_Tex_Overlay,
        Chameleon_Albedo_Masked,
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
        Circular,
        Palettized_Plus_Alpha,
        Diffuse_Plus_Alpha,
        Emblem_Change_Color,
        Change_Color,
        Diffuse_Plus_Alpha_Mask,
        Palettized_Plus_Alpha_Mask,
        Vector_Alpha,
        Vector_Alpha_Drop_Shadow,
    }

    public enum Alpha_Test
    {
        None,
        Simple,
        Multiply_Map,
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
        Hair,
        Custom_Specular,
    }

    public enum Environment_Mapping
    {
        None,
        Per_Pixel,
        Dynamic,
        From_Flat_Texture,
        Custom_Map,
        Per_Pixel_Mip,
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
        None,
        Multilayer_Additive,
        Ml_Add_Four_Change_Color,
        Ml_Add_Five_Change_Color,
        Scope_Blur,
        Palettized_Plasma,
        Palettized_Plasma_Change_Color,
        Constant_Color,
        Window_Room
    }

    public enum Blend_Mode
    {
        Opaque,
        Additive,
        Multiply,
        Alpha_Blend,
        Double_Multiply,
        Maximum,
        Multiply_Add,
        Add_Src_Times_Dstalpha,
        Add_Src_Times_Srcalpha,
        Inv_Alpha_Blend,
        Pre_Multiplied_Alpha,
    }

    public enum Depth_Fade
    {
        Off,
        On,
        Low_Res,
        Palette_Shift,
    }

    public enum Black_Point
    {
        Off,
        On
    }

    public enum Fog
    {
        Off,
        On,
    }
}