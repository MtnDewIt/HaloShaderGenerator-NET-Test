#define particle_template

#include "helpers\definition_helper.hlsli"
#include "helpers\input_output.hlsli"

#if shaderstage == k_shaderstage_default
#include "particle/entry_default.hlsli"

#if render_targets_arg == k_render_targets_ldr_and_hdr
PS_OUTPUT_DEFAULT entry_default(VS_OUTPUT_PARTICLE input) : COLOR
{	
    return particle_entry_default_ldr_and_hdr(input);
}
#endif
#if render_targets_arg == k_render_targets_ldr_only
PS_OUTPUT_DEFAULT_LDR_ONLY entry_default(VS_OUTPUT_PARTICLE input) : COLOR
{
    return particle_entry_default_ldr_only(input);
}
#endif // render_targets_arg == k_render_targets_ldr_only

#endif // shaderstage == k_shaderstage_default