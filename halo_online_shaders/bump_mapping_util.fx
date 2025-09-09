
#ifndef DXT5_NORMALS
#if defined(pc) && (DX_VERSION == 9)
#define DXT5_NORMALS
#endif
#endif

#ifndef BUMP_CONVERT
#if defined(pc) && (DX_VERSION == 9)
#define BUMP_CONVERT(x)  ((x) * (255.0f / 127.f) - (128.0f / 127.f))
#else
#define BUMP_CONVERT(x)  (x)
#endif
#endif

#ifndef BUMP_CONVERT_DXT5NM
#if defined(pc) && (DX_VERSION == 9)
#define BUMP_CONVERT_DXT5NM(x, y)  float2((x) * (255.0f / 127.0f) - (128.0f / 127.0f), (y) * (63.0f / 31.0f) - (32.0f / 31.0f))
#else
#define BUMP_CONVERT_DXT5NM(x, y)  float2(x, g)
#endif
#endif

#ifndef BUMPMAP_UTIL
#define BUMPMAP_UTIL

// enable if storing X in the green channel
#define DXT5NM_AXES_SWAP false

// do not change: use DXT5NM_AXES_SWAP to flip channels
#ifndef BUMP_GET_DXT5NM_CHANNELS
#define BUMP_GET_DXT5NM_CHANNELS(x)  (x).ag
#endif

float bump_reconstruct_z(float2 bump)
{
    float2 bump2 = bump.xy * bump.xy;
    return sqrt(1 - min(bump2.x + bump2.y, 1.0f));
}

float2 bump_convert_dxt5(float2 channels)
{
    if (DXT5NM_AXES_SWAP)
        return float2(channels.y * (63.0f / 31.0f) - (32.0f / 31.0f), channels.x * (255.0f / 127.0f) - (128.0f / 127.0f));
    else
        return float2(channels.x * (255.0f / 127.0f) - (128.0f / 127.0f), channels.y * (63.0f / 31.0f) - (32.0f / 31.0f));
}

float2 sample_dxt5nm(in sampler2D tex, in float2 texcoord)
{
    return bump_convert_dxt5(BUMP_GET_DXT5NM_CHANNELS(tex2D(tex, texcoord)));
}

float3 bump_sample_dxt5nm(in sampler2D tex, in float2 texcoord)
{
    float2 bump5nm = sample_dxt5nm(tex, texcoord);
    return normalize(float3(bump5nm.xy, bump_reconstruct_z(bump5nm)));
}

float3 bump_sample_dxt5nm_unnormalized(in sampler2D tex, in float2 texcoord)
{
    float2 bump5nm = sample_dxt5nm(tex, texcoord);
    return float3(bump5nm.xy, bump_reconstruct_z(bump5nm));
}

float3 bump_sample_dxn_unnormalized(in sampler2D tex, in float2 texcoord)
{
    float2 bump = BUMP_CONVERT(tex2D(tex, texcoord).xy);
    return float3(bump.xy, bump_reconstruct_z(bump));
}

float3 bump_sample_unnormalized(in sampler2D tex, in float2 texcoord)
{
#ifdef DXT5_NORMALS
    return bump_sample_dxt5nm_unnormalized(tex, texcoord);
#else
    return bump_sample_dxn_unnormalized(tex, texcoord);
#endif
}

// this function replicates a h3 content bug. it is only used for certain decals
float3 sample_diffuse_as_bumpmap(in sampler2D tex, in float2 texcoord)
{
    float2 bump = tex2D(tex, texcoord).xy; // match xenon - no sign bias as the texture is a diffuse
    return normalize(float3(bump.xy, bump_reconstruct_z(bump)));
}
#endif
