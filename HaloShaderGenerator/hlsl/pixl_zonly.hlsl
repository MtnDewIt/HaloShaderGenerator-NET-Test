
struct PS_ZONLY_OUTPUT
{
    float4 ldr;
    float4 hdr;
};

PS_ZONLY_OUTPUT main() : COLOR
{
    PS_ZONLY_OUTPUT output;
    output.ldr = float4(0, 0, 0, 1);
    output.hdr = float4(0.5, 0.5, 0.5, 1);
    return output;
}