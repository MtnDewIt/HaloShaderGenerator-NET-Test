
sampler2D specular_lobe;
sampler1D glancing_falloff;
uniform sampler2D material_map;
uniform float4 material_map_xform;

uniform float diffuse_coefficient;
uniform float specular_coefficient;
uniform float area_specular_contribution;
uniform float analytical_specular_contribution;
uniform float environment_map_specular_contribution;
