#ifndef _STIPPLE_FX_
#define _STIPPLE_FX_

void stipple_test(in float2 screen_position)
{
	float stipple = sample2D(stipple_texture, frac(screen_position.xy / 8.0)).r;
	clip(stipple_threshold - stipple);
}

#endif
