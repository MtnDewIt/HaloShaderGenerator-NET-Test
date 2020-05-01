using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloShaderGenerator.Globals
{
    public class ShaderParameters
    {
        public List<ShaderParameter> Parameters = new List<ShaderParameter>();


        public void AddSamplerParameters(string parameterName)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.sampler2D));
            Parameters.Add(new ShaderParameter(parameterName, parameterName + "_xform", HLSLType.Xform_2d));
        }

        public void AddVectorParameters(string parameterName)
        {
            Parameters.Add(new ShaderParameter(parameterName, parameterName, HLSLType.FLoat4));
        }
    }


    public class ShaderParameter
    {
        public string ParameterName;
        public string RegisterName;
        public RegisterType RegisterType;
        public HLSLType CodeType;
        // TODO: add default values

        public ShaderParameter(string parameterName, string registerName, HLSLType type)
        {
            ParameterName = parameterName;
            RegisterName = registerName;
            CodeType = type;
            RegisterType = GetRegisterType(type);
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


        public static string GetParameterNameFromRegister(string registerName)
        {
            switch (registerName)
            {
                case "category_albedo":
                case "albedo":
                    return "albedo";

                case "base_map":
                case "base_map_xform":
                    return "base_map";
            }


            return "";
        }



    }
}
