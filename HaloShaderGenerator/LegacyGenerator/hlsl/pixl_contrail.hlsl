#define contrail_template

#include "contrail/entry_default.hlsli"

PS_OUTPUT_DEFAULT entry_default(VS_OUTPUT_FX input) : COLOR
{
    return contrail_entry_default(input);
}