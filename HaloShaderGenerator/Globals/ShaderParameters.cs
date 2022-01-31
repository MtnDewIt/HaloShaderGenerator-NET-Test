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

        //
        // Pixel shader parameters easy add methods
        //

        public void AddSamplerWithoutXFormParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.sampler2D, rmExtern));
        }

        public void AddSamplerParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.sampler2D, rmExtern));
            Parameters.Add(new ShaderParameter(parameterName, parameterName + "_xform", HLSLType.Xform_2d, rmExtern));
        }

        public void AddXFormOnlyParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName + "_xform", HLSLType.Xform_2d, rmExtern, ShaderParameterFlags.IsXFormOnly));
        }

        public void AddFloat4ColorParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Float4, rmExtern, ShaderParameterFlags.IsColor));
        }

        public void AddFloat3ColorParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Float3, rmExtern, ShaderParameterFlags.IsColor));
        }

        public void AddFloat4Parameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Float4, rmExtern));
        }

        public void AddFloat3Parameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Float3, rmExtern));
        }

        public void AddFloat2Parameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Float2, rmExtern));
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

        //
        // Vertex shader parameters easy add methods
        //

        public void AddSamplerWithoutXFormVertexParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.sampler2D, rmExtern, ShaderParameterFlags.IsVertexShader));
        }

        public void AddSamplerVertexParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.sampler2D, rmExtern, ShaderParameterFlags.IsVertexShader));
            Parameters.Add(new ShaderParameter(parameterName, parameterName + "_xform", HLSLType.Xform_2d, rmExtern, ShaderParameterFlags.IsVertexShader));
        }

        public void AddFloat4ColorVertexParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Float4, rmExtern, (ShaderParameterFlags.IsVertexShader | ShaderParameterFlags.IsColor)));
        }

        public void AddFloat3ColorVertexParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Float3, rmExtern, (ShaderParameterFlags.IsVertexShader | ShaderParameterFlags.IsColor)));
        }

        public void AddFloat4VertexParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Float4, rmExtern, ShaderParameterFlags.IsVertexShader));
        }
        public void AddPrefixedFloat4VertexParameter(string parameterName, string prefix, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, prefix + parameterName, HLSLType.Float4, rmExtern, ShaderParameterFlags.IsVertexShader));
        }

        public void AddCategoryVertexParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, "category_" + parameterName, HLSLType.Float4, rmExtern, ShaderParameterFlags.IsVertexShader | ShaderParameterFlags.IsCategory));
        }

        public void AddFloat3VertexParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Float3, rmExtern, ShaderParameterFlags.IsVertexShader));
        }

        public void AddFloat2VertexParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Float2, rmExtern, ShaderParameterFlags.IsVertexShader));
        }

        public void AddFloatVertexParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Float, rmExtern, ShaderParameterFlags.IsVertexShader));
        }

        public void AddBooleanVertexParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Bool, rmExtern, ShaderParameterFlags.IsVertexShader));
        }

        public void AddInteger4VertexParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Int4, rmExtern, ShaderParameterFlags.IsVertexShader));
        }

        public void AddIntegerVertexParameter(string parameterName, RenderMethodExtern rmExtern = RenderMethodExtern.none)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.Int, rmExtern, ShaderParameterFlags.IsVertexShader));
        }

        //
        // Getters for specific types of paramaters for pixel shaders
        //

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

        public List<ShaderParameter> GetRealPixelParameters()
        {
            var result = new List<ShaderParameter>();
            foreach(var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Vector && parameter.RenderMethodExtern == RenderMethodExtern.none && !parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader))
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetBooleanPixelParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Boolean && parameter.RenderMethodExtern == RenderMethodExtern.none && !parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader))
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetIntegerPixelParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Integer && parameter.RenderMethodExtern == RenderMethodExtern.none && !parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader))
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetSamplerPixelParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Sampler && parameter.RenderMethodExtern == RenderMethodExtern.none && !parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader))
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetRealExternPixelParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Vector && parameter.RenderMethodExtern != RenderMethodExtern.none && !parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader))
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetIntegerExternPixelParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Integer && parameter.RenderMethodExtern != RenderMethodExtern.none && !parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader))
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetSamplerExternPixelParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Sampler && parameter.RenderMethodExtern != RenderMethodExtern.none && !parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader))
                    result.Add(parameter);
            }
            return result;
        }

        //
        // Getters for specific types of paramaters for vertex shaders
        //

        public List<ShaderParameter> GetRealVertexParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Vector && parameter.RenderMethodExtern == RenderMethodExtern.none && parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader))
                    result.Add(parameter);
                else if (parameter.RegisterType == RegisterType.Integer && 
                    parameter.RenderMethodExtern == RenderMethodExtern.none && 
                    parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader) &&
                    parameter.Flags.HasFlag(ShaderParameterFlags.IsCategory))
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetBooleanVertexParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Boolean && parameter.RenderMethodExtern == RenderMethodExtern.none && parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader))
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetIntegerVertexParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Integer && parameter.RenderMethodExtern == RenderMethodExtern.none && parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader))
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetSamplerVertexParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Sampler && parameter.RenderMethodExtern == RenderMethodExtern.none && parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader))
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetRealExternVertexParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Vector && parameter.RenderMethodExtern != RenderMethodExtern.none && parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader))
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetIntegerExternVertexParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Integer && parameter.RenderMethodExtern != RenderMethodExtern.none && parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader))
                    result.Add(parameter);
            }
            return result;
        }

        public List<ShaderParameter> GetSamplerExternVertexParameters()
        {
            var result = new List<ShaderParameter>();
            foreach (var parameter in Parameters)
            {
                if (parameter.RegisterType == RegisterType.Sampler && parameter.RenderMethodExtern != RenderMethodExtern.none && parameter.Flags.HasFlag(ShaderParameterFlags.IsVertexShader))
                    result.Add(parameter);
            }
            return result;
        }
    }

    public enum ShaderParameterFlags
    {
        None = 0,
        IsVertexShader = 1 << 0,
        IsColor = 1 << 1,
        IsXFormOnly = 1 << 2,
        IsVertexAndPixelSampler = 1 << 3,
        IsCategory = 1 << 4
    }

    public class ShaderParameter
    {
        public string ParameterName;
        public string RegisterName;
        public string ExternParameterName;
        public RegisterType RegisterType;
        public HLSLType CodeType;
        public RenderMethodExtern RenderMethodExtern;
        //public bool IsVertexShader = false;
        //public bool IsColor = false;
        //public bool IsXForm = false;
        public ShaderParameterFlags Flags = ShaderParameterFlags.None;

        // TODO: add default values

        public ShaderParameter(string parameterName, string registerName, HLSLType type, RenderMethodExtern renderMethodExtern = RenderMethodExtern.none, ShaderParameterFlags flags = ShaderParameterFlags.None)
        {
            ParameterName = parameterName;
            RegisterName = registerName;
            CodeType = type;
            RegisterType = GetRegisterType(type);
            RenderMethodExtern = renderMethodExtern;
            Flags = flags;
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
