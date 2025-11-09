using HaloShaderGenerator.Globals;
using System;

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

        public string GetCategoryPixelFunction(int category);

        public string GetCategoryVertexFunction(int category);

        public string GetOptionPixelFunction(int category, int option);

        public string GetOptionVertexFunction(int category, int option);
    }

}
