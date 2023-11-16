#define light_volume_template

#include "light_volume/entry_default.hlsli"

PS_OUTPUT_DEFAULT entry_default(VS_OUTPUT_FX input) : COLOR
{
    return light_volume_entry_default(input);
}