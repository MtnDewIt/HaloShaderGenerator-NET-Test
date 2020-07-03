#ifndef _ORGANISM_LIGHTING_HLSLI
#define _ORGANISM_LIGHTING_HLSLI

#include "..\material_models\lambert.hlsli"
#include "..\helpers\input_output.hlsli"
#include "..\helpers\sh.hlsli"
#include "..\methods\environment_mapping.hlsli"
#include "..\material_models\organism.hlsli"

void calc_material_organism_transparence_ps(
float3 vertex_world_position,
float3 reflect_dir,
inout float3 diffuse_accumulation)
{
	if (simple_light_count > 0)
	{
		calculate_transparence_diffuse_simple_light(get_simple_light(0), vertex_world_position, reflect_dir, diffuse_accumulation);
		if (simple_light_count > 1)
		{
			calculate_transparence_diffuse_simple_light(get_simple_light(1), vertex_world_position, reflect_dir, diffuse_accumulation);
			if (simple_light_count > 2)
			{
				calculate_transparence_diffuse_simple_light(get_simple_light(2), vertex_world_position, reflect_dir, diffuse_accumulation);
				if (simple_light_count > 3)
				{
					calculate_transparence_diffuse_simple_light(get_simple_light(3), vertex_world_position, reflect_dir, diffuse_accumulation);
					if (simple_light_count > 4)
					{
						calculate_transparence_diffuse_simple_light(get_simple_light(4), vertex_world_position, reflect_dir, diffuse_accumulation);
						if (simple_light_count > 5)
						{
							calculate_transparence_diffuse_simple_light(get_simple_light(5), vertex_world_position, reflect_dir, diffuse_accumulation);
							if (simple_light_count > 6)
							{
								calculate_transparence_diffuse_simple_light(get_simple_light(6), vertex_world_position, reflect_dir, diffuse_accumulation);
								[flatten]
								if (simple_light_count > 7)
								{
									calculate_transparence_diffuse_simple_light(get_simple_light(7), vertex_world_position, reflect_dir, diffuse_accumulation);
								}
							}
						}
					}
				}
			}
		}
	}
}


void calc_dynamic_lighting_organism_ps(SHADER_DYNAMIC_LIGHT_COMMON common_data, out float3 color)
{
	float l_dot_n = dot(common_data.light_direction, common_data.surface_normal);
	color = common_data.light_intensity * l_dot_n * common_data.albedo.rgb;
}

float3 calc_lighting_organism_ps(SHADER_COMMON common_data, out float4 unknown_output)
{
	float3 color = 0;
	float3 analytic_specular;
	float3 area_specular;
	float3 specular;
	float3 diffuse;
	
	diffuse = common_data.diffuse_reflectance;
	float l_dot_r = dot(common_data.dominant_light_direction, common_data.reflect_dir);
	
	float4 specular_map_sample = tex2D(specular_map, common_data.texcoord);
	float c_specular_power = specular_map_sample.a * specular_power;
	
	float specular_coefficient = pow(l_dot_r, c_specular_power);
	specular_coefficient *= (1.0f - c_specular_power) * 0.159154564; // 1 / 2PI
	
	calc_material_analytic_specular_organism_ps(specular_coefficient, common_data.dominant_light_intensity, analytic_specular);
	calc_material_area_specular_organism_ps(common_data.reflect_dir, common_data.sh_312, area_specular);
	
	float3 temp = area_specular_coefficient * area_specular;
	analytic_specular *= analytical_specular_coefficient;
	analytic_specular = l_dot_r > 0 ? analytic_specular : 0;
	temp *= -0.380523592;
	area_specular = area_specular > 0 ? temp : 0;
	specular = analytic_specular + area_specular;
	
	
	float3 diffuse_accumulation = 0;
	float3 specular_accumulation = 0;
	if (!common_data.no_dynamic_lights)
	{
		float dynamic_light_roughness = c_specular_power * dot(specular_map_sample.rgb, specular_map_sample.rgb);
		calc_material_lambert_diffuse_ps(common_data.surface_normal, common_data.world_position, common_data.reflect_dir, dynamic_light_roughness, diffuse_accumulation, specular_accumulation);
		specular_accumulation *= dynamic_light_roughness;
		specular_accumulation *= 0.33;
		
	}
	
	diffuse += diffuse_accumulation;
	diffuse *= diffuse_coefficient;
	
	specular += specular_accumulation * analytical_specular_coefficient;
	
	float3 c_specular_coefficient = specular_map_sample.rgb * specular_tint.rgb;
	
	float4 occlusion_parameters = tex2D(occlusion_parameter_map, common_data.texcoord);
	
	// rim
	
	float3 rim_color;
	float c_rim_power, c_rim_base, c_rim_base_start, c_rim_base_norm;

	c_rim_base_start = saturate(1.0f - common_data.albedo.a) - rim_start;
	c_rim_base_norm = 0.999 - rim_start < 0 ? 1000 : 1 / (1.0 - rim_start);
	c_rim_base = saturate(c_rim_base_start * c_rim_base_norm);
	
	c_rim_power = pow(c_rim_base, rim_power);
	rim_color = c_rim_power * common_data.dominant_light_intensity.rgb;
	rim_color *= rim_tint;
	rim_color *= rim_coefficient;
	
	float3 rim_pos_color = rim_color * rim_maps_transition_ratio;
	rim_pos_color *= specular_map_sample.rgb;
	
	float3 rim_neg_color = rim_color * (1.0 - rim_maps_transition_ratio);
	rim_pos_color = 0.01 - rim_coefficient < 0 ? rim_pos_color : 0;
	rim_neg_color = 0.01 - rim_coefficient < 0 ? rim_neg_color : 0;
	
	// ambient

	float3 ambient_color = occlusion_parameters.x * common_data.sh_0.rgb;
	ambient_color *= ambient_tint.rgb;
	ambient_color *= ambient_coefficient;
	ambient_color = 0.01 - ambient_coefficient < 0 ? ambient_color : 0;
	
	// subsurface scattering
	float3 subsurface_diffuse;
	if (subsurface_coefficient > 0)
	{
		float4 subsurface_map_sample = tex2D(subsurface_map, common_data.texcoord);
		float3 normal_difference = common_data.surface_normal - common_data.normal;
		float3 subsurface_normal = normal_difference * subsurface_normal_detail + common_data.normal;
		float3 light_propagation = common_data.dominant_light_direction * subsurface_propagation_bias;
		float3 subsurface_light_direction = light_propagation * subsurface_map_sample.a + subsurface_normal;
		subsurface_light_direction = normalize(subsurface_light_direction);
		
		float3 subsurface_area_diffuse = float3(dot(subsurface_light_direction, common_data.sh_312[0].xyz),
											  dot(subsurface_light_direction, common_data.sh_312[1].xyz),
											  dot(subsurface_light_direction, common_data.sh_312[2].xyz));
		subsurface_area_diffuse *= -0.380523592;
		subsurface_area_diffuse = max(subsurface_area_diffuse, 0);
			
		float3 subsurface_diffuse_accumulation = 0;
		float3 unused = 0;
		if (!common_data.no_dynamic_lights)
		{
			calc_material_lambert_diffuse_ps(common_data.normal, common_data.world_position, common_data.reflect_dir, 0, subsurface_diffuse_accumulation, unused);
		}
		subsurface_diffuse = subsurface_area_diffuse + subsurface_diffuse_accumulation;
		subsurface_diffuse *= subsurface_tint;
		subsurface_diffuse *= subsurface_coefficient;
		subsurface_diffuse *= subsurface_map_sample.rgb;
		subsurface_diffuse *= occlusion_parameters.x;

	}
	else
		subsurface_diffuse = 0.0f;
	
	// transparence
	float3 transparence_diffuse;
	if (transparence_coefficient > 0)
	{
		float4 transparence_map_sample = tex2D(transparence_map, common_data.texcoord);
		float3 transparence_area_diffuse = float3(dot(-common_data.n_view_dir, common_data.sh_312[0].xyz),
											      dot(-common_data.n_view_dir, common_data.sh_312[1].xyz),
											      dot(-common_data.n_view_dir, common_data.sh_312[2].xyz));
		transparence_area_diffuse *= -0.380523592;
		transparence_area_diffuse = max(transparence_area_diffuse, 0);
		float3 transparence_diffuse_accumulation = 0;
		calc_material_organism_transparence_ps(common_data.world_position, common_data.reflect_dir, transparence_diffuse_accumulation);
		float3 normal_difference = common_data.surface_normal - common_data.normal;
		float3 transparence_normal = normal_difference * transparence_normal_detail + common_data.normal;
		transparence_normal = normalize(transparence_normal);
		float v_dot_n = dot(common_data.n_view_dir, transparence_normal);
		float bias = transparence_normal_bias < 0 ? 1.0 - v_dot_n : v_dot_n;
		bias = saturate(1.0 - abs(transparence_map_sample.a * transparence_normal_bias) * bias);
		transparence_diffuse = transparence_area_diffuse + transparence_diffuse_accumulation;
		transparence_diffuse *= bias;
		transparence_diffuse *= transparence_tint;
		transparence_diffuse *= transparence_coefficient;
		transparence_diffuse *= transparence_map_sample.rgb;
	}
	else
		transparence_diffuse = 0;
	
	color = c_specular_coefficient * specular + rim_pos_color;
	color += subsurface_diffuse;
	color += transparence_diffuse;
	
	float3 surface_diffuse = diffuse * diffuse_tint + rim_neg_color;
	surface_diffuse += ambient_color;
	
	color *= final_tint;
	surface_diffuse *= final_tint;
	
	color = surface_diffuse * common_data.albedo.rgb + color;
	
	unknown_output = 0;
	return color;
}
#endif