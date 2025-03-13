using System.Collections.Generic;

namespace HaloShaderGenerator.Globals
{
    public class ShaderParameters
    {
        public List<ShaderOptionParameter> Parameters;

        public ShaderParameters()
        {
            Parameters = new List<ShaderOptionParameter>();
        }

        public void AddSamplerParameter
        (
            string parameterName, 
            RenderMethodExtern rmExtern = RenderMethodExtern.none, 
            ShaderOptionParameter.ShaderFilterMode filterMode = ShaderOptionParameter.ShaderFilterMode.Trilinear, 
            ShaderOptionParameter.ShaderAddressMode addressMode = ShaderOptionParameter.ShaderAddressMode.Wrap,
            short anisotropyAmount = 0,
            float bitmapScale = 0.0f,
            string SamplerBitmap = null
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, HLSLType.sampler2D, rmExtern, filterMode, addressMode, anisotropyAmount, bitmapScale, SamplerBitmap));
        }

        public void AddFloat4ColorParameter
        (
            string parameterName, 
            RenderMethodExtern rmExtern = RenderMethodExtern.none,
            ShaderOptionParameter.ShaderFilterMode filterMode = ShaderOptionParameter.ShaderFilterMode.Trilinear,
            ShaderOptionParameter.ShaderAddressMode addressMode = ShaderOptionParameter.ShaderAddressMode.Wrap,
            short anisotropyAmount = 0,
            float bitmapScale = 0.0f,
            ShaderColor colorArgument = new ShaderColor()
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, HLSLType.Float4, rmExtern, filterMode, addressMode, anisotropyAmount, bitmapScale, colorArgument, ShaderOptionParameter.ShaderParameterFlags.IsColor));
        }

        public void AddFloat3ColorParameter
        (
            string parameterName,
            RenderMethodExtern rmExtern = RenderMethodExtern.none,
            ShaderOptionParameter.ShaderFilterMode filterMode = ShaderOptionParameter.ShaderFilterMode.Trilinear,
            ShaderOptionParameter.ShaderAddressMode addressMode = ShaderOptionParameter.ShaderAddressMode.Wrap,
            short anisotropyAmount = 0,
            float bitmapScale = 0.0f,
            ShaderColor colorArgument = new ShaderColor()
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, HLSLType.Float3, rmExtern, filterMode, addressMode, anisotropyAmount, bitmapScale, colorArgument, ShaderOptionParameter.ShaderParameterFlags.IsColor));
        }

        public void AddFloatParameter
        (
            string parameterName,
            RenderMethodExtern rmExtern = RenderMethodExtern.none,
            ShaderOptionParameter.ShaderFilterMode filterMode = ShaderOptionParameter.ShaderFilterMode.Trilinear,
            ShaderOptionParameter.ShaderAddressMode addressMode = ShaderOptionParameter.ShaderAddressMode.Wrap,
            short anisotropyAmount = 0,
            float bitmapScale = 0.0f,
            float floatArgument = 0.0f
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, HLSLType.Float, rmExtern, filterMode, addressMode, anisotropyAmount, bitmapScale, floatArgument));
        }

        public void AddBooleanParameter
        (
            string parameterName,
            RenderMethodExtern rmExtern = RenderMethodExtern.none,
            ShaderOptionParameter.ShaderFilterMode filterMode = ShaderOptionParameter.ShaderFilterMode.Trilinear,
            ShaderOptionParameter.ShaderAddressMode addressMode = ShaderOptionParameter.ShaderAddressMode.Wrap,
            short anisotropyAmount = 0,
            float bitmapScale = 0.0f,
            uint intBoolArgument = 0
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, HLSLType.Bool, rmExtern, filterMode, addressMode, anisotropyAmount, bitmapScale, intBoolArgument));
        }

        public void AddIntegerParameter
        (
            string parameterName,
            RenderMethodExtern rmExtern = RenderMethodExtern.none,
            ShaderOptionParameter.ShaderFilterMode filterMode = ShaderOptionParameter.ShaderFilterMode.Trilinear,
            ShaderOptionParameter.ShaderAddressMode addressMode = ShaderOptionParameter.ShaderAddressMode.Wrap,
            short anisotropyAmount = 0,
            float bitmapScale = 0.0f,
            uint intBoolArgument = 0
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, HLSLType.Int, rmExtern, filterMode, addressMode, anisotropyAmount, bitmapScale, intBoolArgument));
        }
    }

    public class ShaderOptionParameter
    {
        public string ParameterName;
        public string RegisterName;
        public RegisterType RegisterType;
        public HLSLType CodeType;
        public RenderMethodExtern RenderMethodExtern;
        public ShaderFilterMode FilterMode;
        public ShaderAddressMode AddressMode;
        public short AnisotropyAmount;
        public float BitmapScale;
        public string SamplerBitmap;
        public float FloatArgument;
        public uint IntBoolArgument;
        public ShaderColor ColorArgument;
        public ShaderParameterFlags Flags;

        public enum ShaderParameterFlags
        {
            None = 0,
            IsVertexShader = 1 << 0,
            IsColor = 1 << 1,
            IsXFormOnly = 1 << 2,
        }

        public enum ShaderFilterMode 
        {
            Trilinear = 0,
            Point = 1,
            Bilinear = 2,
            Anisotropic1 = 3,
            Anisotropic2Expensive = 4,
            Anisotropic3Expensive = 5,
            Anisotropic4Expensive = 6,
            LightprobeTextureArray = 7,
            ComparisonPoint = 9,
            ComparisonBilinear = 10
        }

        public enum ShaderAddressMode 
        {
            Wrap = 0,
            Clamp = 1,
            Mirror = 2,
            BlackBorder = 3,
            MirrorOnce = 4,
            MirrorOnceBorder = 5
        }

        public ShaderOptionParameter
        (
            string parameterName,
            HLSLType type,
            RenderMethodExtern renderMethodExtern = RenderMethodExtern.none,
            ShaderFilterMode filterMode = ShaderFilterMode.Trilinear,
            ShaderAddressMode addressMode = ShaderAddressMode.Wrap,
            short anisotropyAmount = 0,
            float bitmapScale = 0.0f,
            ShaderParameterFlags flags = ShaderParameterFlags.None
        )
        {
            ParameterName = parameterName;
            CodeType = type;
            RegisterType = GetRegisterType(type);
            RenderMethodExtern = renderMethodExtern;
            FilterMode = filterMode;
            AddressMode = addressMode;
            AnisotropyAmount = anisotropyAmount;
            BitmapScale = bitmapScale;
            Flags = flags;
        }

        public ShaderOptionParameter
        (
            string parameterName,
            HLSLType type,
            RenderMethodExtern renderMethodExtern = RenderMethodExtern.none,
            ShaderFilterMode filterMode = ShaderFilterMode.Trilinear,
            ShaderAddressMode addressMode = ShaderAddressMode.Wrap,
            short anisotropyAmount = 0,
            float bitmapScale = 0.0f,
            string samplerBitmap = null,
            ShaderParameterFlags flags = ShaderParameterFlags.None
        ) 
        {
            ParameterName = parameterName;
            CodeType = type;
            RegisterType = GetRegisterType(type);
            RenderMethodExtern = renderMethodExtern;
            FilterMode = filterMode;
            AddressMode = addressMode;
            AnisotropyAmount = anisotropyAmount;
            BitmapScale = bitmapScale;
            SamplerBitmap = samplerBitmap;
            Flags = flags;
        }

        public ShaderOptionParameter
        (
            string parameterName,
            HLSLType type,
            RenderMethodExtern renderMethodExtern = RenderMethodExtern.none,
            ShaderFilterMode filterMode = ShaderFilterMode.Trilinear,
            ShaderAddressMode addressMode = ShaderAddressMode.Wrap,
            short anisotropyAmount = 0,
            float bitmapScale = 0.0f,
            float floatArgument = 0.0f,
            ShaderParameterFlags flags = ShaderParameterFlags.None
        )
        {
            ParameterName = parameterName;
            CodeType = type;
            RegisterType = GetRegisterType(type);
            RenderMethodExtern = renderMethodExtern;
            FilterMode = filterMode;
            AddressMode = addressMode;
            AnisotropyAmount = anisotropyAmount;
            BitmapScale = bitmapScale;
            FloatArgument = floatArgument;
            Flags = flags;
        }

        public ShaderOptionParameter
        (
            string parameterName,
            HLSLType type,
            RenderMethodExtern renderMethodExtern = RenderMethodExtern.none,
            ShaderFilterMode filterMode = ShaderFilterMode.Trilinear,
            ShaderAddressMode addressMode = ShaderAddressMode.Wrap,
            short anisotropyAmount = 0,
            float bitmapScale = 0.0f,
            uint intBoolArgument = 0,
            ShaderParameterFlags flags = ShaderParameterFlags.None
        )
        {
            ParameterName = parameterName;
            CodeType = type;
            RegisterType = GetRegisterType(type);
            RenderMethodExtern = renderMethodExtern;
            FilterMode = filterMode;
            AddressMode = addressMode;
            AnisotropyAmount = anisotropyAmount;
            BitmapScale = bitmapScale;
            IntBoolArgument = intBoolArgument;
            Flags = flags;
        }

        public ShaderOptionParameter
        (
            string parameterName,
            HLSLType type,
            RenderMethodExtern renderMethodExtern = RenderMethodExtern.none,
            ShaderFilterMode filterMode = ShaderFilterMode.Trilinear,
            ShaderAddressMode addressMode = ShaderAddressMode.Wrap,
            short anisotropyAmount = 0,
            float bitmapScale = 0.0f,
            ShaderColor colorArgument = new ShaderColor(),
            ShaderParameterFlags flags = ShaderParameterFlags.None
        )
        {
            ParameterName = parameterName;
            CodeType = type;
            RegisterType = GetRegisterType(type);
            RenderMethodExtern = renderMethodExtern;
            FilterMode = filterMode;
            AddressMode = addressMode;
            AnisotropyAmount = anisotropyAmount;
            BitmapScale = bitmapScale;
            ColorArgument = colorArgument;
            Flags = flags;
        }

        public static RegisterType GetRegisterType(HLSLType codeType)
        {
            switch (codeType)
            {
                case HLSLType.Float:
                case HLSLType.Float2:
                case HLSLType.Float3:
                case HLSLType.Float4:
                case HLSLType.Xform_2d:
                case HLSLType.Xform_3d:
                    return RegisterType.Vector;

                case HLSLType.Sampler:
                case HLSLType.sampler2D:
                case HLSLType.sampler3D:
                    return RegisterType.Sampler;

                case HLSLType.Bool:
                case HLSLType.Bool2:
                case HLSLType.Bool3:
                case HLSLType.Bool4:
                    return RegisterType.Boolean;

                case HLSLType.Int:
                case HLSLType.Int2:
                case HLSLType.Int3:
                case HLSLType.Int4:
                    return RegisterType.Integer;

                default:
                    return RegisterType.Vector;
            }
        }
    }
}
