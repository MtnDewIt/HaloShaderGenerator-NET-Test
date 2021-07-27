using HaloShaderGenerator.DirectX;
using HaloShaderGenerator.Globals;
using HaloShaderGenerator.Shader;
using System.Collections.Generic;

namespace HaloShaderGenerator
{
    public class ExplicitUnitTest : GenericUnitTest
    {
        public ExplicitUnitTest(string referencePath) : base(referencePath, null, "explicit") { }

        public override string GeneratePixelShader(ShaderStage stage, List<int> shaderOptions)
        {
            throw new System.NotImplementedException();
        }

        public override string GenerateSharedPixelShader(ShaderStage stage, int methodIndex, int optionIndex)
        {
            throw new System.NotImplementedException();
        }

        public override string GenerateVertexShader(VertexType vertex, ShaderStage stage)
        {
            throw new System.NotImplementedException();
        }

        public override string GenerateSharedVertexShader(VertexType vertex, ShaderStage stage)
        {
            throw new System.NotImplementedException();
        }

        public override string GenerateExplicitPixelShader(ShaderTypes.ExplicitShader explicitShader)
        {
            var bytecode = GenericPixelShaderGenerator.GeneratePixelShader(explicitShader.ToString(), false).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }

        public override string GenerateExplicitVertexShader(ShaderTypes.ExplicitShader explicitShader)
        {
            var bytecode = GenericVertexShaderGenerator.GenerateVertexShader(explicitShader.ToString(), false).Bytecode;
            return D3DCompiler.Disassemble(bytecode);
        }
    }
}
