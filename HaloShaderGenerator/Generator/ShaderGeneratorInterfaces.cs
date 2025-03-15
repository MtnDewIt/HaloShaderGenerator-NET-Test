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
        /// <summary>
        /// Returns true when entry point is supported in the shading pipeline. The pixel shader and vertex shader are either in the shared tag or in the pixl,vtsh.
        /// </summary>
        /// <param name="entryPoint"></param>
        /// <returns></returns>
        bool IsEntryPointSupported(ShaderStage entryPoint);

        /// <summary>
        /// Returns true if the pixel shader should be stored in the global pixel shader tag.
        /// </summary>
        /// <param name="entryPoint"></param>
        /// <returns></returns>
        bool IsPixelShaderShared(ShaderStage entryPoint);

        /// <summary>
        /// Returns true if the pixel shader uses the method at method_index. 
        /// </summary>
        /// <param name="entryPoint"></param>
        /// <param name="method_index"></param>
        /// <returns></returns>
        bool IsMethodSharedInEntryPoint(ShaderStage entryPoint, int methodIndex);

        /// <summary>
        /// Returns true if the vertex shader supports the vertex format
        /// </summary>
        /// <returns></returns>
        bool IsVertexFormatSupported(VertexType vertexType);

        /// <summary>
        /// Returns true if the vertex shader is in the shared vertex shader tag.
        /// </summary>
        /// <param name="entryPoint"></param>
        /// <returns></returns>
        bool IsVertexShaderShared(ShaderStage entryPoint);

        /// <summary>
        /// Returns true if shared pixel shader has multiple methods
        /// </summary>
        /// <param name="entryPoint"></param>
        /// <returns></returns>
        bool IsSharedPixelShaderUsingMethods(ShaderStage entryPoint);

        /// <summary>
        /// Returns true if the entry point has a shared pixel shader independent of methods\options
        /// </summary>
        /// <param name="entryPoint"></param>
        /// <returns></returns>
        bool IsSharedPixelShaderWithoutMethod(ShaderStage entryPoint);

        int GetMethodCount();

        int GetMethodOptionCount(int methodIndex);

        ShaderParameters GetGlobalParameters();

        ShaderParameters GetParametersInOption(string methodName, int option, out string rmopName, out string optionName);

        Array GetMethodNames();

        Array GetMethodOptionNames(int methodIndex);

        void GetCategoryFunctions(string methodName, out string vertexFunction, out string pixelFunction);

        void GetOptionFunctions(string methodName, int option, out string vertexFunction, out string pixelFunction);
    }

}
