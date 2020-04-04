#define shader_template

#include "registers/vertex_shader.hlsli"
#include "../helpers/input_output.hlsli"


// temporary definition, will have to macro the hell out of this
VS_OUTPUT_STATIC_PTR_AMBIENT global_entry_rigid_static_prt_ambient(VS_INPUT_RIGID_VERTEX_AMBIENT_PRT input)
{
	VS_OUTPUT_STATIC_PTR_AMBIENT output;
	return output;
}