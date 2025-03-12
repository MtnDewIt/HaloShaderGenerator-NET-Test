using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            ShaderOptionParameter.ShaderAddressMode addressMode = ShaderOptionParameter.ShaderAddressMode.Wrap
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, parameterName, HLSLType.sampler2D, rmExtern, filterMode, addressMode));
        }

        public void AddFloat4ColorParameter
        (
            string parameterName, 
            RenderMethodExtern rmExtern = RenderMethodExtern.none,
            ShaderOptionParameter.ShaderFilterMode filterMode = ShaderOptionParameter.ShaderFilterMode.Trilinear,
            ShaderOptionParameter.ShaderAddressMode addressMode = ShaderOptionParameter.ShaderAddressMode.Wrap
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, parameterName, HLSLType.Float4, rmExtern, filterMode, addressMode, ShaderOptionParameter.ShaderParameterFlags.IsColor));
        }

        public void AddFloat3ColorParameter
        (
            string parameterName,
            RenderMethodExtern rmExtern = RenderMethodExtern.none,
            ShaderOptionParameter.ShaderFilterMode filterMode = ShaderOptionParameter.ShaderFilterMode.Trilinear,
            ShaderOptionParameter.ShaderAddressMode addressMode = ShaderOptionParameter.ShaderAddressMode.Wrap
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, parameterName, HLSLType.Float3, rmExtern, filterMode, addressMode, ShaderOptionParameter.ShaderParameterFlags.IsColor));
        }

        public void AddFloat4Parameter
        (
            string parameterName,
            RenderMethodExtern rmExtern = RenderMethodExtern.none
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, parameterName, HLSLType.Float4, rmExtern));
        }

        public void AddFloat3Parameter
        (
            string parameterName,
            RenderMethodExtern rmExtern = RenderMethodExtern.none
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, parameterName, HLSLType.Float3, rmExtern));
        }

        public void AddFloat2Parameter
        (
            string parameterName,
            RenderMethodExtern rmExtern = RenderMethodExtern.none
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, parameterName, HLSLType.Float2, rmExtern));
        }

        public void AddFloatParameter
        (
            string parameterName,
            RenderMethodExtern rmExtern = RenderMethodExtern.none
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, parameterName, HLSLType.Float, rmExtern));
        }

        public void AddBooleanParameter
        (
            string parameterName,
            RenderMethodExtern rmExtern = RenderMethodExtern.none
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, parameterName, HLSLType.Bool, rmExtern));
        }

        public void AddInteger4Parameter
        (
            string parameterName,
            RenderMethodExtern rmExtern = RenderMethodExtern.none
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, parameterName, HLSLType.Int4, rmExtern));
        }

        public void AddIntegerParameter
        (
            string parameterName,
            RenderMethodExtern rmExtern = RenderMethodExtern.none
        )
        {
            Parameters.Add(new ShaderOptionParameter(parameterName, parameterName, HLSLType.Int, rmExtern));
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
            Trilinear,
            Point,
            Bilinear,
            Anisotropic1,
            Anisotropic2Expensive,
            Anisotropic3Expensive,
            Anisotropic4Expensive,
            LightprobeTextureArray,
            ComparisonPoint,
            ComparisonBilinear
        }

        public enum ShaderAddressMode 
        {
            Wrap,
            Clamp,
            Mirror,
            BlackBorder,
            MirrorOnce,
            MirrorOnceBorder
        }

        public ShaderOptionParameter
        (
            string parameterName, 
            string registerName, 
            HLSLType type, 
            RenderMethodExtern renderMethodExtern = RenderMethodExtern.none, 
            ShaderFilterMode filterMode = ShaderFilterMode.Trilinear,
            ShaderAddressMode addressMode = ShaderAddressMode.Wrap,
            ShaderParameterFlags flags = ShaderParameterFlags.None
        )
        {
            ParameterName = parameterName;
            RegisterName = registerName;
            CodeType = type;
            RegisterType = GetRegisterType(type);
            RenderMethodExtern = renderMethodExtern;
            FilterMode = filterMode;
            AddressMode = addressMode;
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
