#ifndef _CHUD_INPUT_OUTPUT_HLSLI
#define _CHUD_INPUT_OUTPUT_HLSLI

struct s_chud_vertex_output
{
    float4 position : SV_Position;
    float2 micro_texcoord : TEXCOORD;
    float2 texcoord : TEXCOORD1;
};

struct s_chud_vertex // could just be screen vertex?
{
    float4 position : POSITION;
    float4 texcoord : TEXCOORD;
    //float4 color : COLOR;
};

#endif
