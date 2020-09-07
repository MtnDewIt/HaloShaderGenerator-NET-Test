#define decal_template

#include "helpers\definition_helper.hlsli"
#include "helpers\input_output.hlsli"
#include "decal\entry_default.hlsli"
#include "helpers\decal_helper.hlsli"

// TODO: setup *another* function for bumpmapping (code in decal_entry_default right now)

#if shaderstage == k_shaderstage_default

#if DECAL_IS_SIMPLE == 1
PS_OUTPUT_DECAL_SIMPLE entry_default(VS_OUTPUT_DECAL input) : COLOR
{
    return decal_entry_default_simple(input);
}
#else
PS_OUTPUT_DECAL entry_default(VS_OUTPUT_DECAL input) : COLOR
{
    return decal_entry_default(input);
}
#endif
#endif