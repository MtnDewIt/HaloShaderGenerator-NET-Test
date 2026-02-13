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

        #region Sampler Functions
        public void AddSamplerParameter(string parameterName, string samplerBitmap = null) 
        {
            Parameters.Add(new ShaderOptionParameter 
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                SamplerBitmap = samplerBitmap
            });
        }

        public void AddSamplerExternParameter(string parameterName, RenderMethodExtern rmExtern, string samplerBitmap = null)
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                RenderMethodExtern = rmExtern,
                SamplerBitmap = samplerBitmap
            });
        }

        public void AddSamplerExternAddressParameter(string parameterName, RenderMethodExtern rmExtern, ShaderOptionParameter.ShaderAddressMode addressMode, string samplerBitmap = null)
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                RenderMethodExtern = rmExtern,
                AddressMode = addressMode,
                SamplerBitmap = samplerBitmap,
            });
        }

        public void AddSamplerExternFilterParameter(string parameterName, RenderMethodExtern rmExtern, ShaderOptionParameter.ShaderFilterMode filterMode, string samplerBitmap = null)
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                RenderMethodExtern = rmExtern,
                FilterMode = filterMode,
                SamplerBitmap = samplerBitmap,
            });
        }

        public void AddSamplerExternFilterAddressParameter(string parameterName, RenderMethodExtern rmExtern, ShaderOptionParameter.ShaderFilterMode filterMode, ShaderOptionParameter.ShaderAddressMode addressMode, string samplerBitmap = null)
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                RenderMethodExtern = rmExtern,
                FilterMode = filterMode,
                AddressMode = addressMode,
                SamplerBitmap = samplerBitmap,
            });
        }

        public void AddSamplerExternWithColorParameter(string parameterName, RenderMethodExtern rmExtern, ShaderColor colorArgument, string samplerBitmap = null) 
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                RenderMethodExtern = rmExtern,
                SamplerBitmap = samplerBitmap,
                ColorArgument = colorArgument,
            });
        }

        public void AddSamplerFilterParameter(string parameterName, ShaderOptionParameter.ShaderFilterMode filterMode, string samplerBitmap = null) 
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                FilterMode = filterMode,
                SamplerBitmap = samplerBitmap,
            });
        }

        public void AddSamplerFilterWithFloatParameter(string parameterName, ShaderOptionParameter.ShaderFilterMode filterMode, float floatArgument, string samplerBitmap = null)
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                FilterMode = filterMode,
                FloatArgument = floatArgument,
                SamplerBitmap = samplerBitmap,
            });
        }

        public void AddSamplerAddressParameter(string parameterName, ShaderOptionParameter.ShaderAddressMode addressMode, string samplerBitmap = null)
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                AddressMode = addressMode,
                SamplerBitmap = samplerBitmap,
            });
        }

        public void AddSamplerAddressWithColorParameter(string parameterName, ShaderOptionParameter.ShaderAddressMode addressMode, ShaderColor colorArgument, string samplerBitmap = null) 
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                AddressMode = addressMode,
                ColorArgument = colorArgument,
                SamplerBitmap = samplerBitmap,
            });
        }

        public void AddSamplerFilterAddressParameter(string parameterName, ShaderOptionParameter.ShaderFilterMode filterMode, ShaderOptionParameter.ShaderAddressMode addressMode, string samplerBitmap = null) 
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                FilterMode = filterMode,
                AddressMode = addressMode,
                SamplerBitmap = samplerBitmap,
            });
        }

        public void AddSamplerWithFloatParameter(string parameterName, float floatArgument, string samplerBitmap = null) 
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                FloatArgument = floatArgument,
                SamplerBitmap = samplerBitmap,
            });
        }

        public void AddSamplerWithScaleParameter(string parameterName, float bitmapScale, string samplerBitmap = null)
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                BitmapScale = bitmapScale,
                SamplerBitmap = samplerBitmap,
            });
        }

        public void AddSamplerWithFloatAndColorParameter(string parameterName, float floatArgument, ShaderColor colorArgument, string samplerBitmap = null) 
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                FloatArgument = floatArgument,
                ColorArgument = colorArgument,
                SamplerBitmap = samplerBitmap,
            });
        }

        public void AddSamplerFilterWithScaleParameter(string parameterName, ShaderOptionParameter.ShaderFilterMode filterMode, float bitmapScale, string samplerBitmap = null)
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.sampler2D,
                FilterMode = filterMode,
                BitmapScale = bitmapScale,
                SamplerBitmap = samplerBitmap,
            });
        }
        #endregion

        #region Float Functions
        public void AddFloatParameter(string parameterName, float floatArgument = 0.0f)
        {
            Parameters.Add(new ShaderOptionParameter 
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Float,
                FloatArgument = floatArgument,
            });
        }

        public void AddFloatWithIntegerParameter(string parameterName, uint intBoolArgument, float floatArgument = 0.0f)
        {
            Parameters.Add(new ShaderOptionParameter 
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Float,
                IntBoolArgument = intBoolArgument,
                FloatArgument = floatArgument,
            });
        }

        public void AddFloatExternParameter(string parameterName, RenderMethodExtern rmExtern, float floatArgument = 0.0f) 
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Float,
                RenderMethodExtern = rmExtern,
                FloatArgument = floatArgument,
            });
        }

        public void AddFloatWithColorParameter(string parameterName, ShaderColor colorArgument, float floatArgument = 0.0f)
        {
            Parameters.Add(new ShaderOptionParameter 
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Float,
                FloatArgument = floatArgument,
                ColorArgument = colorArgument,
            });
        }
        #endregion

        #region Boolean Functions
        public void AddBooleanParameter(string parameterName, uint intBoolArgument = 0) 
        {
            Parameters.Add(new ShaderOptionParameter 
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Bool,
                IntBoolArgument = intBoolArgument,
            });
        }

        public void AddBooleanWithFloatParameter(string parameterName, float floatArgument, uint intBoolArgument = 0) 
        {
            Parameters.Add(new ShaderOptionParameter 
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Bool,
                FloatArgument = floatArgument,
                IntBoolArgument = intBoolArgument,
            });
        }
        #endregion

        #region Integer Functions
        public void AddIntegerParameter(string parameterName, uint intBoolArgument = 0) 
        {
            Parameters.Add(new ShaderOptionParameter 
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Int,
                IntBoolArgument = intBoolArgument,
            });
        }

        public void AddIntegerWithFloatParameter(string parameterName, float floatArgument, uint intBoolArgument = 0) 
        {
            Parameters.Add(new ShaderOptionParameter 
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Int,
                FloatArgument = floatArgument,
                IntBoolArgument = intBoolArgument,
            });
        }
        #endregion

        #region ARGB Color Functions
        public void AddFloat4ColorParameter(string parameterName, ShaderColor colorArgument = new ShaderColor()) 
        {
            Parameters.Add(new ShaderOptionParameter 
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Float4,
                ColorArgument = colorArgument,
                Flags = ShaderOptionParameter.ShaderParameterFlags.IsColor,
            });
        }

        public void AddFloat4ColorExternParameter(string parameterName, RenderMethodExtern rmExtern, ShaderColor colorArgument = new ShaderColor())
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Float4,
                RenderMethodExtern = rmExtern,
                ColorArgument = colorArgument,
                Flags = ShaderOptionParameter.ShaderParameterFlags.IsColor,
            });
        }

        public void AddFloat4ColorWithFloatParameter(string parameterName, float floatArgument, ShaderColor colorArgument = new ShaderColor())
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Float4,
                FloatArgument = floatArgument,
                ColorArgument = colorArgument,
                Flags = ShaderOptionParameter.ShaderParameterFlags.IsColor,
            });
        }

        public void AddFloat4ColorWithFloatAndIntegerParameter(string parameterName, float floatArgument, uint intBoolArgument, ShaderColor colorArgument = new ShaderColor())
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Float4,
                FloatArgument = floatArgument,
                IntBoolArgument = intBoolArgument,
                ColorArgument = colorArgument,
                Flags = ShaderOptionParameter.ShaderParameterFlags.IsColor,
            });
        }
        #endregion

        #region RGB Color Functions
        public void AddFloat3ColorParameter(string parameterName, ShaderColor colorArgument = new ShaderColor()) 
        {
            Parameters.Add(new ShaderOptionParameter 
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Float3,
                ColorArgument = colorArgument,
                Flags = ShaderOptionParameter.ShaderParameterFlags.IsColor,
            });
        }

        public void AddFloat3ColorExternParameter(string parameterName, RenderMethodExtern rmExtern, ShaderColor colorArgument = new ShaderColor())
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Float3,
                RenderMethodExtern = rmExtern,
                ColorArgument = colorArgument,
                Flags = ShaderOptionParameter.ShaderParameterFlags.IsColor,
            });
        }

        public void AddFloat3ColorExternWithSamplerParameter(string parameterName, RenderMethodExtern rmExtern, string samplerBitmap, ShaderColor colorArgument = new ShaderColor())
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Float3,
                RenderMethodExtern = rmExtern,
                SamplerBitmap = samplerBitmap,
                ColorArgument = colorArgument,
                Flags = ShaderOptionParameter.ShaderParameterFlags.IsColor,
            });
        }

        public void AddFloat3ColorExternWithFloatParameter(string parameterName, RenderMethodExtern rmExtern, float floatArgument, ShaderColor colorArgument = new ShaderColor())
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Float3,
                RenderMethodExtern = rmExtern,
                FloatArgument = floatArgument,
                ColorArgument = colorArgument,
                Flags = ShaderOptionParameter.ShaderParameterFlags.IsColor,
            });
        }

        public void AddFloat3ColorExternWithFloatAndIntegerParameter(string parameterName, RenderMethodExtern rmExtern, float floatArgument, uint intBoolArgument, ShaderColor colorArgument = new ShaderColor())
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Float3,
                RenderMethodExtern = rmExtern,
                FloatArgument = floatArgument,
                IntBoolArgument = intBoolArgument,
                ColorArgument = colorArgument,
                Flags = ShaderOptionParameter.ShaderParameterFlags.IsColor,
            });
        }

        public void AddFloat3ColorWithFloatParameter(string parameterName, float floatArgument, ShaderColor colorArgument = new ShaderColor())
        {
            Parameters.Add(new ShaderOptionParameter
            {
                ParameterName = parameterName,
                CodeType = HLSLType.Float3,
                FloatArgument = floatArgument,
                ColorArgument = colorArgument,
                Flags = ShaderOptionParameter.ShaderParameterFlags.IsColor,
            });
        }
        #endregion
    }

    public class ShaderOptionParameter
    {
        public string ParameterName;
        public HLSLType CodeType;
        public RegisterType RegisterType => GetRegisterType(CodeType);
        public RenderMethodExtern RenderMethodExtern = RenderMethodExtern.none;
        public ShaderFilterMode FilterMode = ShaderFilterMode.Trilinear;
        public ShaderAddressMode AddressMode = ShaderAddressMode.Wrap;
        public float BitmapScale = 0.0f;
        public string SamplerBitmap = null;
        public float FloatArgument = 0.0f;
        public uint IntBoolArgument = 0;
        public ShaderColor ColorArgument = new ShaderColor(0, 0, 0, 0);
        public ShaderParameterFlags Flags = ShaderParameterFlags.None;

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
