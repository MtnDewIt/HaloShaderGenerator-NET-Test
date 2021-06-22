
#include "..\helpers\explicit_input_output.hlsli"
#include "registers\displacement_motion_blur_registers.hlsli"

#ifndef ODST_SHADER
#define ODST_SHADER 0
#endif

float4 main(VS_OUTPUT_SCREEN input) : COLOR
{
    float distort_constant = ODST_SHADER == 1 ? 0.5000076f : 0.501960814f;
    
    float2 displacement = tex2D(displacement_sampler, input.texcoord.xy).rg - distort_constant;
    float2 displacement_tex = (screen_constants.zw + screen_constants.zw) * displacement;
    
    float disp_magnitute = dot(displacement_tex, displacement_tex);
    clip(-disp_magnitute >= 0.0f ? -1 : 1);
    
    displacement_tex += (input.position.xy + 0.5f) * screen_constants.xy;

    // bound test (snap to safe area)
    displacement_tex = min(max(displacement_tex, window_bounds.xy), window_bounds.zw);

    return tex2D(ldr_buffer, displacement_tex);
}
