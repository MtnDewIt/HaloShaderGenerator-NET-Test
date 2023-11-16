#define cortana_template

#include "helpers\definition_helper.hlsli"
#include "helpers\input_output.hlsli"

#if shaderstage == k_shaderstage_albedo
#include "cortana/entry_albedo.hlsli"
PS_OUTPUT_ALBEDO entry_albedo(VS_OUTPUT_ALBEDO input) : COLOR
{	
	return cortana_entry_albedo(input);
}
#endif

#if shaderstage == k_shaderstage_active_camo
#include "cortana/entry_active_camo.hlsli"
PS_OUTPUT_DEFAULT entry_active_camo(VS_OUTPUT_CORTANA_PRT input) : COLOR
{
	return cortana_entry_active_camo(input);
}
#endif

#if shaderstage == k_shaderstage_static_sh || shaderstage == k_shaderstage_static_prt_ambient
#include "cortana/entry_prt.hlsli"
PS_OUTPUT_DEFAULT entry_static_sh(VS_OUTPUT_STATIC_SH input) : COLOR
{
	return cortana_entry_static_sh(input);
}

PS_OUTPUT_DEFAULT entry_static_prt_ambient(VS_OUTPUT_CORTANA_PRT input) : COLOR
{
	return cortana_entry_static_prt(input);
}
#endif
