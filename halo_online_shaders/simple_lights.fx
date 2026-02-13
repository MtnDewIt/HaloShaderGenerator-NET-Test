/*
	simple lights are organized as an array of 16 structs consisting of 4 float4's
	
		position (xyz)		size (w)
		direction (xyz)		spherical % (w)
		color (xyz)			smooth (w)
		falloff scale (xy)	falloff offset (zw)
*/


#ifndef SIMPLE_LIGHT_DATA
#define SIMPLE_LIGHT_DATA simple_lights
#define SIMPLE_LIGHT_COUNT simple_light_count
#endif // !SIMPLE_LIGHT_DATA

#define PI 3.14159265358979323846264338327950f

void calculate_simple_light(
		uniform int light_index,
		in float3 fragment_position_world,
		out float3 light_radiance,
		out float3 fragment_to_light)			// return normalized direction to the light
{
#ifdef dynamic_lights_use_array_notation
#define		LIGHT_DATA(offset, registers)	(SIMPLE_LIGHT_DATA[light_index][(offset)].registers)
#elif DX_VERSION == 9
#define		LIGHT_DATA(offset, registers)	(SIMPLE_LIGHT_DATA[(light_index * 5) + (offset)].registers)
#elif DX_VERSION == 11
#define		LIGHT_DATA(offset, registers)	(SIMPLE_LIGHT_DATA[(light_index * 5) + (offset)].registers)
#endif

#define		LIGHT_POSITION			LIGHT_DATA(0, xyz)
#define		LIGHT_DIRECTION			LIGHT_DATA(1, xyz)
#define		LIGHT_COLOR				LIGHT_DATA(2, xyz)
#define		LIGHT_SIZE				LIGHT_DATA(0, w)
#define		LIGHT_SPHERE			LIGHT_DATA(1, w)
#define		LIGHT_SMOOTH			LIGHT_DATA(2, w)
#define		LIGHT_FALLOFF_SCALE		LIGHT_DATA(3, xy)
#define		LIGHT_FALLOFF_OFFSET	LIGHT_DATA(3, zw)
#define		LIGHT_BOUNDING_RADIUS	LIGHT_DATA(4, x)

	// calculate direction to light (4 instructions)
	fragment_to_light= LIGHT_POSITION - fragment_position_world;				// vector from fragment to light
	float  light_dist2= dot(fragment_to_light, fragment_to_light);				// distance to the light, squared
	fragment_to_light  *=rsqrt(light_dist2);									// normalized vector pointing to the light
		
	float2 falloff;
	falloff.x= 1 / (LIGHT_SIZE + light_dist2);									// distance based falloff				(2 instructions)
	falloff.y= dot(fragment_to_light, LIGHT_DIRECTION);							// angle based falloff (spot-light)		(1 instruction)
	falloff= max(0.0001f, falloff * LIGHT_FALLOFF_SCALE + LIGHT_FALLOFF_OFFSET);	// scale, offset, clamp result			(2 instructions)
	falloff.y= pow(falloff.y, LIGHT_SMOOTH) + LIGHT_SPHERE;						// smooth and add ambient				(4 instructions)
	float combined_falloff= saturate(falloff.x) * saturate(falloff.y);								//										(1 instruction)

	light_radiance= LIGHT_COLOR * combined_falloff;								//										(1 instruction)
}

#if DX_VERSION == 9

void simple_lights_analytical_single(
		in int light_index,
		in float3 fragment_position_world,
		in float3 surface_normal,
		in float3 view_reflect_dir_world,							// view direction = fragment to camera,   reflected around fragment normal
		in float specular_power,
		inout float3 diffusely_reflected_light,						// diffusely reflected light (not including diffuse surface color)
		inout float3 specularly_reflected_light)						// specularly reflected light (not including specular surface color)
{
	// Compute distance squared to light, to see if we can skip this light.
	// Note: This is also computed in calculate_simple_light below, but the shader
	// compiler will remove the second computation and share the results of this
	// computation.
	float3 fragment_to_light_test= LIGHT_POSITION - fragment_position_world;				// vector from fragment to light
	float  light_dist2_test= dot(fragment_to_light_test, fragment_to_light_test);				// distance to the light, squared
	if( light_dist2_test < LIGHT_BOUNDING_RADIUS )
	{
        float3 fragment_to_light;
        float3 light_radiance;
        calculate_simple_light(
			light_index, fragment_position_world, light_radiance, fragment_to_light);
		
		// calculate diffuse cosine lobe (diffuse surface N dot L)
        float cosine_lobe = dot(surface_normal, fragment_to_light);
		
        diffusely_reflected_light += light_radiance * max(0.05f, cosine_lobe); // add light with cosine lobe (add ambient 5% light)
//		diffusely_reflected_light  += light_radiance * saturate(cosine_lobe);			// add light with cosine lobe (clamped positive)
		
		// step(0.0f, cosine_lobe)
        specularly_reflected_light += light_radiance * safe_pow(max(0.0f, dot(fragment_to_light, view_reflect_dir_world)), specular_power);
//		specularly_reflected_light += light_radiance * pow(saturate(dot(fragment_to_light, view_reflect_dir_world)), specular_power);
    }
	else
    {
		// debug: use a strong green tint to highlight area outside of the light's radius
		//diffusely_reflected_light += float3( 0, 1, 0 );
		//specularly_reflected_light += float3( 0, 1, 0 );
    }
}

void calc_simple_lights_analytical(
		in float3 fragment_position_world,
		in float3 surface_normal,
		in float3 view_reflect_dir_world,							// view direction = fragment to camera,   reflected around fragment normal
		in float specular_power,
		out float3 diffusely_reflected_light,						// diffusely reflected light (not including diffuse surface color)
		out float3 specularly_reflected_light)						// specularly reflected light (not including specular surface color)
{
	diffusely_reflected_light= float3(0.0f, 0.0f, 0.0f);
	specularly_reflected_light= float3(0.0f, 0.0f, 0.0f);

    if (SIMPLE_LIGHT_COUNT > 0)
    {
        simple_lights_analytical_single(0, fragment_position_world, surface_normal, view_reflect_dir_world, specular_power, diffusely_reflected_light, specularly_reflected_light);
        if (SIMPLE_LIGHT_COUNT > 1)
        {
            simple_lights_analytical_single(1, fragment_position_world, surface_normal, view_reflect_dir_world, specular_power, diffusely_reflected_light, specularly_reflected_light);
            if (SIMPLE_LIGHT_COUNT > 2)
            {
                simple_lights_analytical_single(2, fragment_position_world, surface_normal, view_reflect_dir_world, specular_power, diffusely_reflected_light, specularly_reflected_light);
                if (SIMPLE_LIGHT_COUNT > 3)
                {
                    simple_lights_analytical_single(3, fragment_position_world, surface_normal, view_reflect_dir_world, specular_power, diffusely_reflected_light, specularly_reflected_light);
                    if (SIMPLE_LIGHT_COUNT > 4)
                    {
                        simple_lights_analytical_single(4, fragment_position_world, surface_normal, view_reflect_dir_world, specular_power, diffusely_reflected_light, specularly_reflected_light);
                        if (SIMPLE_LIGHT_COUNT > 5)
                        {
                            simple_lights_analytical_single(5, fragment_position_world, surface_normal, view_reflect_dir_world, specular_power, diffusely_reflected_light, specularly_reflected_light);
                            if (SIMPLE_LIGHT_COUNT > 6)
                            {
                                simple_lights_analytical_single(6, fragment_position_world, surface_normal, view_reflect_dir_world, specular_power, diffusely_reflected_light, specularly_reflected_light);
								[flatten]
                                if (SIMPLE_LIGHT_COUNT > 7)
                                {
                                    simple_lights_analytical_single(7, fragment_position_world, surface_normal, view_reflect_dir_world, specular_power, diffusely_reflected_light, specularly_reflected_light);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
	
    specularly_reflected_light *= specular_power;
}

void calc_simple_lights_analytical_reach(
		in float3 fragment_position_world,
		in float3 surface_normal,
		in float3 view_reflect_dir_world,							// view direction = fragment to camera,   reflected around fragment normal
		in float specular_power,
		out float3 diffusely_reflected_light,						// diffusely reflected light (not including diffuse surface color)
		out float3 specularly_reflected_light)						// specularly reflected light (not including specular surface color)
{
	diffusely_reflected_light= float3(0.0f, 0.0f, 0.0f);
	specularly_reflected_light= float3(0.0f, 0.0f, 0.0f);
	
	// add in simple lights
#ifndef pc	
	[loop]
#endif
	for (int light_index= 0; light_index < SIMPLE_LIGHT_COUNT; light_index++)
	{
		// Compute distance squared to light, to see if we can skip this light.
		// Note: This is also computed in calculate_simple_light below, but the shader
		// compiler will remove the second computation and share the results of this
		// computation.
		float3 fragment_to_light_test= LIGHT_POSITION - fragment_position_world;				// vector from fragment to light
		float  light_dist2_test= dot(fragment_to_light_test, fragment_to_light_test);				// distance to the light, squared
		if( light_dist2_test >= LIGHT_BOUNDING_RADIUS )
		{
			// debug: use a strong green tint to highlight area outside of the light's radius
			//diffusely_reflected_light += float3( 0, 1, 0 );
			//specularly_reflected_light += float3( 0, 1, 0 );
			continue;
		}
		
		float3 fragment_to_light;
		float3 light_radiance;
		calculate_simple_light(
			light_index, fragment_position_world, light_radiance, fragment_to_light);
		
		// calculate diffuse cosine lobe (diffuse surface N dot L)
		float cosine_lobe= dot(surface_normal, fragment_to_light) + 0.06f;  // + 0.05 so that the grenade on the ground can work well.
		
		diffusely_reflected_light  += light_radiance * saturate(cosine_lobe) / 3.14159265358979323846;			// add light with cosine lobe (clamped positive)
		
		// step(0.0f, cosine_lobe)
		//specularly_reflected_light += light_radiance * pow(max(0.0f, dot(fragment_to_light, view_reflect_dir_world)), specular_power);
		float specular_cosine_lobe= saturate(dot(fragment_to_light, view_reflect_dir_world));
		specularly_reflected_light += light_radiance * pow(specular_cosine_lobe, specular_power);
#ifdef pc
		if (light_index >= 7)		// god damn PC compiler likes to unroll these loops - only support 8 lights or so (:P)
		{
			light_index= 100;
		}
#endif // pc
	}
	specularly_reflected_light *= (1+specular_power);
}

void calc_simple_lights_uma(
		in float3 fragment_position_world,
		in float3 surface_normal,
		in float3 view_reflect_dir_world,							// view direction = fragment to camera,   reflected around fragment normal
        in float3 view_dir,
		in float  spec_mask,
        in float3 albedo,
		out float3 diffusely_reflected_light,						// diffusely reflected light (not including diffuse surface color)
		out float3 specularly_reflected_light)
{
	

	diffusely_reflected_light= float3(0.0f, 0.0f, 0.0f);
	specularly_reflected_light= float3(0.0f, 0.0f, 0.0f);

#ifndef pc
	[loop]
#endif
	for (int light_index= 0; light_index < SIMPLE_LIGHT_COUNT; light_index++)
	{
		// Compute distance squared to light, to see if we can skip this light.
		// Note: This is also computed in calculate_simple_light below, but the shader
		// compiler will remove the second computation and share the results of this
		// computation.
		float3 fragment_to_light_test= LIGHT_POSITION - fragment_position_world;				// vector from fragment to light
		float  light_dist2_test= dot(fragment_to_light_test, fragment_to_light_test);				// distance to the light, squared
		if( light_dist2_test >= LIGHT_BOUNDING_RADIUS )
		{
			// debug: use a strong green tint to highlight area outside of the light's radius
			//diffusely_reflected_light += float3( 0, 1, 0 );
			//specularly_reflected_light += float3( 0, 1, 0 );
			continue;
		}

		float3 fragment_to_light;
		float3 light_radiance;
		calculate_simple_light(
			light_index, fragment_position_world, light_radiance, fragment_to_light);

        float NdotL = smoothstep(0.0, 0.03f, dot(surface_normal, fragment_to_light));
		float NdotV = dot(surface_normal, view_dir);

		specularly_reflected_light += saturate(spec_mask * 50 * (NdotV - 0.6)) * (albedo / PI) * NdotL * light_radiance;
		
		diffusely_reflected_light	+= NdotL * light_radiance * (albedo / PI);
	}
}

#else

void calc_simple_lights_analytical(
		in float3 fragment_position_world,
		in float3 surface_normal,
		in float3 view_reflect_dir_world,							// view direction = fragment to camera,   reflected around fragment normal
		in float specular_power,
		out float3 diffusely_reflected_light,						// diffusely reflected light (not including diffuse surface color)
		out float3 specularly_reflected_light)						// specularly reflected light (not including specular surface color)
{
	diffusely_reflected_light= float3(0.0f, 0.0f, 0.0f);
	specularly_reflected_light= float3(0.0f, 0.0f, 0.0f);
	
	// add in simple lights
//#ifndef pc	
	[loop]
//#endif
	for (int light_index= 0; light_index < SIMPLE_LIGHT_COUNT; light_index++)
	{
		// Compute distance squared to light, to see if we can skip this light.
		// Note: This is also computed in calculate_simple_light below, but the shader
		// compiler will remove the second computation and share the results of this
		// computation.
		float3 fragment_to_light_test= LIGHT_POSITION - fragment_position_world;				// vector from fragment to light
		float  light_dist2_test= dot(fragment_to_light_test, fragment_to_light_test);				// distance to the light, squared
		if( light_dist2_test >= LIGHT_BOUNDING_RADIUS )
		{
			// debug: use a strong green tint to highlight area outside of the light's radius
			//diffusely_reflected_light += float3( 0, 1, 0 );
			//specularly_reflected_light += float3( 0, 1, 0 );
			continue;
		}
		
		float3 fragment_to_light;
		float3 light_radiance;
		calculate_simple_light(
			light_index, fragment_position_world, light_radiance, fragment_to_light);
		
		// calculate diffuse cosine lobe (diffuse surface N dot L)
		float cosine_lobe= dot(surface_normal, fragment_to_light);
		
		diffusely_reflected_light  += light_radiance * max(0.05f, cosine_lobe);			// add light with cosine lobe (add ambient 5% light)
//		diffusely_reflected_light  += light_radiance * saturate(cosine_lobe);			// add light with cosine lobe (clamped positive)
		
		// step(0.0f, cosine_lobe)
		specularly_reflected_light += light_radiance * safe_pow(max(0.0f, dot(fragment_to_light, view_reflect_dir_world)), specular_power);
//		specularly_reflected_light += light_radiance * pow(saturate(dot(fragment_to_light, view_reflect_dir_world)), specular_power);
// #ifdef pc
// 		if (light_index >= 7)		// god damn PC compiler likes to unroll these loops - only support 8 lights or so (:P)
// 		{
// 			light_index= 100;
// 		}
// #endif // pc
	}
	specularly_reflected_light *= specular_power;
}
#endif // DX_VERSION == 9

void calc_toon_lights_analytical(
		in float3 fragment_position_world,
		in float3 normal_dir,
		in float3 view_dir,							// view direction = fragment to camera,   reflected around fragment normal
		in float roughness,
		in float3 effective_reflectance,
		in float3 specular_glancing_color,
		out float ndotl_sum,
		out float3 diffusely_reflected_light,						// diffusely reflected light (not including diffuse surface color)
		out float3 specularly_reflected_light)						// specularly reflected light (not including specular surface color)
{
	diffusely_reflected_light= float3(0.0f, 0.0f, 0.0f);
	specularly_reflected_light= float3(0.0f, 0.0f, 0.0f);
	ndotl_sum = 0.0f;
	float blinn_power = max(2/pow(max(roughness, 0.06),4)-2, 0.00001);
	float blinn_power_sharp = max(2/pow(max(roughness - (0.6 * roughness * roughness), 0.06),4)-2, 0.00001);
	float blinn_normalize = ((blinn_power + 2) / 8);
	// add in simple lights
//#ifndef pc	
	[loop]
//#endif
	for (int light_index= 0; light_index < SIMPLE_LIGHT_COUNT; light_index++)
	{
		// Compute distance squared to light, to see if we can skip this light.
		// Note: This is also computed in calculate_simple_light below, but the shader
		// compiler will remove the second computation and share the results of this
		// computation.
		float3 fragment_to_light_test= LIGHT_POSITION - fragment_position_world;				// vector from fragment to light
		float  light_dist2_test= dot(fragment_to_light_test, fragment_to_light_test);				// distance to the light, squared
		if( light_dist2_test >= LIGHT_BOUNDING_RADIUS )
		{
			// debug: use a strong green tint to highlight area outside of the light's radius
			//diffusely_reflected_light += float3( 0, 1, 0 );
			//specularly_reflected_light += float3( 0, 1, 0 );
			continue;
		}
		
		float3 fragment_to_light;
		float3 light_radiance;
		calculate_simple_light(
			light_index, fragment_position_world, light_radiance, fragment_to_light);
		

		float3 H = normalize(fragment_to_light + view_dir);
		float NdotH = saturate(dot(normal_dir, H));
		float NdotL = saturate(dot(normal_dir, fragment_to_light));
		float NdotV = saturate(dot(normal_dir, view_dir));
		float VdotH = saturate(dot(view_dir, H));

		float NdotL_stepped = smoothstep(0, 0.01f, NdotL);

		float3 fresnel = effective_reflectance + (specular_glancing_color - effective_reflectance) * pow(1 - VdotH, 5.0f);

		float blinn_phong = blinn_normalize * pow(NdotH, blinn_power);
		float blinn_phong_sharp = blinn_normalize * pow(NdotH, blinn_power_sharp);

		float sharp_highlight = smoothstep(0.005f, 0.01f, saturate(blinn_phong_sharp))  * (1 - smoothstep(0.2, 0.8, roughness));
		float widest_highlight = min(blinn_phong, 0.175f) * smoothstep(0.2, 0.8, roughness) * (1 - sharp_highlight);
		float scale_factor = max(8 * roughness, 1);
		float toon_blinn = sharp_highlight + widest_highlight;

		diffusely_reflected_light  += (1 / PI) * (1 - fresnel) * NdotL_stepped * light_radiance;	
		
		specularly_reflected_light += toon_blinn * fresnel * NdotL * light_radiance;

		ndotl_sum += (NdotL_stepped + toon_blinn) * light_radiance ;
	}
	ndotl_sum = saturate(ndotl_sum);
}

float calc_diffuse_lobe(
	in float3 fragment_normal,
	in float3 fragment_to_light,
	in float3 translucency)
{
	// calculate diffuse cosine lobe (diffuse surface N dot L)
	float cosine_lobe= dot(fragment_normal, fragment_to_light);
	return saturate(cosine_lobe);
}


float calc_diffuse_translucent_lobe(
	in float3 fragment_normal,
	in float3 fragment_to_light,
	in float3 translucency)
{
	// calculate diffuse cosine lobe (diffuse surface N dot L)
	float cosine_lobe= dot(fragment_normal, fragment_to_light);
	float translucent_cosine_lobe= (cosine_lobe * translucency.x + translucency.y) * cosine_lobe + translucency.z;
	return translucent_cosine_lobe;
}

#if DX_VERSION == 9

void simple_lights_analytical_diffuse_translucent_single(
		in int light_index,
		in float3 fragment_position_world,
		in float3 surface_normal,
		inout float3 diffusely_reflected_light)						// diffusely reflected light (not including diffuse surface color)
{
	// Compute distance squared to light, to see if we can skip this light.
	// Note: This is also computed in calculate_simple_light below, but the shader
	// compiler will remove the second computation and share the results of this
	// computation.
	float3 fragment_to_light_test= LIGHT_POSITION - fragment_position_world;				// vector from fragment to light
	float  light_dist2_test= dot(fragment_to_light_test, fragment_to_light_test);				// distance to the light, squared
	if( light_dist2_test < LIGHT_BOUNDING_RADIUS )
	{
        float3 fragment_to_light;
		float3 light_radiance;
		calculate_simple_light(
			light_index, fragment_position_world, light_radiance, fragment_to_light);
				
		diffusely_reflected_light  += light_radiance;
    }
	else
    {
		// debug: use a strong green tint to highlight area outside of the light's radius
		//diffusely_reflected_light += float3( 0, 1, 0 );
    }
}

void calc_simple_lights_analytical_diffuse_translucent(
		in float3 fragment_position_world,
		in float3 surface_normal,
		in float3 translucency,
		out float3 diffusely_reflected_light)						// specularly reflected light (not including specular surface color)
{
	diffusely_reflected_light= float3(0.0f, 0.0f, 0.0f);
	
	// add in simple lights
	if (SIMPLE_LIGHT_COUNT > 0)
    {
        simple_lights_analytical_diffuse_translucent_single(0, fragment_position_world, surface_normal, diffusely_reflected_light);
        if (SIMPLE_LIGHT_COUNT > 1)
        {
            simple_lights_analytical_diffuse_translucent_single(1, fragment_position_world, surface_normal, diffusely_reflected_light);
            if (SIMPLE_LIGHT_COUNT > 2)
            {
                simple_lights_analytical_diffuse_translucent_single(2, fragment_position_world, surface_normal, diffusely_reflected_light);
                if (SIMPLE_LIGHT_COUNT > 3)
                {
                    simple_lights_analytical_diffuse_translucent_single(3, fragment_position_world, surface_normal, diffusely_reflected_light);
                    if (SIMPLE_LIGHT_COUNT > 4)
                    {
                        simple_lights_analytical_diffuse_translucent_single(4, fragment_position_world, surface_normal, diffusely_reflected_light);
                        if (SIMPLE_LIGHT_COUNT > 5)
                        {
                            simple_lights_analytical_diffuse_translucent_single(5, fragment_position_world, surface_normal, diffusely_reflected_light);
                            if (SIMPLE_LIGHT_COUNT > 6)
                            {
                                simple_lights_analytical_diffuse_translucent_single(6, fragment_position_world, surface_normal, diffusely_reflected_light);
								[flatten]
                                if (SIMPLE_LIGHT_COUNT > 7)
                                {
                                    simple_lights_analytical_diffuse_translucent_single(7, fragment_position_world, surface_normal, diffusely_reflected_light);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

#else // DX_VERSION 11 || XENON

void calc_simple_lights_analytical_diffuse_translucent(
		in float3 fragment_position_world,
		in float3 surface_normal,
		in float3 translucency,
		out float3 diffusely_reflected_light)						// specularly reflected light (not including specular surface color)
{
	diffusely_reflected_light= float3(0.0f, 0.0f, 0.0f);
	
	// add in simple lights
//#ifndef pc	
	[loop]
//#endif
	for (int light_index= 0; light_index < SIMPLE_LIGHT_COUNT; light_index++)
	{
		// Compute distance squared to light, to see if we can skip this light.
		// Note: This is also computed in calculate_simple_light below, but the shader
		// compiler will remove the second computation and share the results of this
		// computation.
		float3 fragment_to_light_test= LIGHT_POSITION - fragment_position_world;				// vector from fragment to light
		float  light_dist2_test= dot(fragment_to_light_test, fragment_to_light_test);				// distance to the light, squared
		if( light_dist2_test >= LIGHT_BOUNDING_RADIUS )
		{
			// debug: use a strong green tint to highlight area outside of the light's radius
			//diffusely_reflected_light += float3( 0, 1, 0 );
			//specularly_reflected_light += float3( 0, 1, 0 );
			continue;
		}
		
		float3 fragment_to_light;
		float3 light_radiance;
		calculate_simple_light(
			light_index, fragment_position_world, light_radiance, fragment_to_light);
				
		diffusely_reflected_light  += light_radiance;// * calc_diffuse_translucent_lobe(surface_normal, fragment_to_light, translucency);
		
// #ifdef pc
// 		if (light_index >= 7)		// god damn PC compiler likes to unroll these loops - only support 8 lights or so (:P)
// 		{
// 			light_index= 100;
// 		}
// #endif // pc
	}
}
#endif // DX_VERSION == 9