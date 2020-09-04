#define beam_template

#include "beam/entry_default.hlsli"

PS_OUTPUT_DEFAULT entry_default(VS_OUTPUT_FX input) : COLOR
{
    return beam_entry_default(input);
}