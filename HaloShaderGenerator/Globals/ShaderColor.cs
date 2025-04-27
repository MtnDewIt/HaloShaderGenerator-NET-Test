namespace HaloShaderGenerator.Globals
{
    public struct ShaderColor
    {
        public byte Alpha;
        public byte Red;
        public byte Green;
        public byte Blue;

        public ShaderColor(byte alpha, byte red, byte green, byte blue)
        {
            Alpha = alpha;
            Red = red;
            Green = green;
            Blue = blue;
        }
    }
}
