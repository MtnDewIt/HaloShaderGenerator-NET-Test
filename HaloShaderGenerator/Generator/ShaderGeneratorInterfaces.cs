using HaloShaderGenerator.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloShaderGenerator.Generator
{
    public interface IShaderGenerator
    {
        bool IsPixelShaderShared(ShaderStage entryPoint);

        bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint);

        bool IsAutoMacro();

        int GetSharedPixelShaderCategory(ShaderStage entryPoint);

        int GetMethodCount();

        int GetMethodOptionCount(int methodIndex);

        ShaderParameters GetGlobalParameters(out string rmopName);

        ShaderParameters GetParametersInOption(string methodName, int option, out string rmopName, out string optionName);

        Array GetMethodNames();

        Array GetMethodOptionNames(int methodIndex);

        Array GetEntryPointOrder();

        Array GetVertexTypeOrder();

        void GetCategoryFunctions(string methodName, out string vertexFunction, out string pixelFunction);

        void GetOptionFunctions(string methodName, int option, out string vertexFunction, out string pixelFunction);
    }

}
