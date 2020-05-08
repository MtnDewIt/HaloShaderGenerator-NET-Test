using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloShaderGenerator.Globals
{
    public class ShaderParameters
    {
        public List<ShaderParameter> Parameters;

        public ShaderParameters()
        {
            Parameters = new List<ShaderParameter>();
        }

        public void AddSamplerWithoutXFormParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.sampler2D, rmExtern));
        }

        public void AddSamplerParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.sampler2D, rmExtern));
            Parameters.Add(new ShaderParameter(parameterName, parameterName + "_xform", HLSLType.Xform_2d, rmExtern));
        }

        public void AddFloat4Parameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.FLoat4, rmExtern));
        }

        public void AddFloatParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Float, rmExtern));
        }

        public void AddBooleanParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Bool, rmExtern));
        }

        public void AddInteger4Parameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Int4, rmExtern));
        }

        public void AddIntegerParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Int, rmExtern));
        }

        public List<ShaderParameter> GetRealParameters()
        {
            var result = new List<ShaderParameter>();
            foreach(var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Vector && parameter.RenderMethodExtern == RenderMethodExtern.none)
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetBooleanParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Boolean && parameter.RenderMethodExtern == RenderMethodExtern.none)
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetIntegerParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Integer && parameter.RenderMethodExtern == RenderMethodExtern.none)
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetSamplerParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Sampler && parameter.RenderMethodExtern == RenderMethodExtern.none)
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetRealExternParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Vector && parameter.RenderMethodExtern != RenderMethodExtern.none)
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetIntegerExternParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Integer && parameter.RenderMethodExtern != RenderMethodExtern.none)
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetSamplerExternParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Sampler && parameter.RenderMethodExtern != RenderMethodExtern.none)
                    result.Add(parameter);
            }
            return result;
        }
    }


    public class ShaderParameter
    {
        public string ParameterName;
        public string RegisterName;
        public string ExternParameterName;
        public RegisterType RegisterType;
        public HLSLType CodeType;
        public RenderMethodExtern RenderMethodExtern;

        // TODO: add default values

        public ShaderParameter(string parameterName, string registerName, HLSLType type, RenderMethodExtern renderMethodExtern = RenderMethodExtern.none)
        {
            ParameterName = parameterName;
            RegisterName = registerName;
            CodeType = type;
            RegisterType = GetRegisterType(type);
            RenderMethodExtern = renderMethodExtern;
        }

        public string GenerateHLSLCode()
        {
            return $"uniform {CodeType.ToString().ToLower()} {RegisterName};\n";
        }

        public static RegisterType GetRegisterType(HLSLType codeType)
        {
            switch (codeType)
            {
                case HLSLType.Float:
                case HLSLType.Float2:
                case HLSLType.Float3:
                case HLSLType.FLoat4:
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
                    return RegisterType.Boolean;

                default:
                    return RegisterType.Vector;
            }
        }

    }
}
