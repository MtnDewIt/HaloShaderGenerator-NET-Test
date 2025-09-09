using HaloShaderGenerator.DirectX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HaloShaderGenerator.Generator
{
    public static class ShaderGeneratorBase
    {
        private class IncludeManager(string root_directory = "") : Include(root_directory)
        {
            private readonly string ShaderDirectory = Path.GetDirectoryName(typeof(IncludeManager).Assembly.Location) + "\\halo_online_shaders\\";

            public string ReadResource(string filepath, string _parent_directory = null)
            {
                string parent_directory = _parent_directory ?? base.DirectoryMap[IntPtr.Zero];
                string filePath = ShaderDirectory + Path.Combine(parent_directory, filepath);

                if (!File.Exists(filePath))
                    throw new Exception($"Couldn't find file {filePath}");

                using (StreamReader reader = new(filePath))
                    return reader.ReadToEnd();
            }

            protected override int Open(D3D.INCLUDE_TYPE includeType, string filepath, string directory, ref string hlsl_source)
            {
                switch (includeType)
                {
                    case D3D.INCLUDE_TYPE.D3D_INCLUDE_LOCAL:
                        hlsl_source = ReadResource(filepath, directory);
                        return 0;
                    case D3D.INCLUDE_TYPE.D3D_INCLUDE_SYSTEM:
                        hlsl_source = ReadResource(filepath, base.DirectoryMap[IntPtr.Zero]);
                        return 0;
                    default:
                        throw new Exception("Unimplemented include type");
                }
            }
        }

        public static string GetSourceFile(string template)
        {
            string fileName = template.Split('\\').Last();

            IncludeManager include = new IncludeManager(template.Replace(fileName, ""));

            return include.ReadResource(fileName);
        }

        public static byte[] GenerateSource(string template, IEnumerable<D3D.SHADER_MACRO> macros, string entry, string version)
        { 
            // Macros should never be duplicated
            for (var i = 0; i < macros.Count(); i++)
            {
                for (var j = 0; j < macros.Count(); j++)
                {
                    if (i == j) continue;
                    if (macros.ElementAt(i).Name == macros.ElementAt(j).Name)
                    {
                        throw new Exception($"Macro {macros.ElementAt(i).Name} is defined multiple times");
                    }
                }
            }

            string fileName = template.Split('\\').Last();

            IncludeManager include = new IncludeManager(template.Replace(fileName, ""));

            string shader_source = include.ReadResource(fileName);

            D3DCompiler.D3DCOMPILE flags = 0;
#if DEBUG
            //flags |= D3DCompiler.D3DCOMPILE.D3DCOMPILE_WARNINGS_ARE_ERRORS;
#endif
            //flags |= D3DCompiler.D3DCOMPILE.D3DCOMPILE_SKIP_VALIDATION;
            //flags |= D3DCompiler.D3DCOMPILE.D3DCOMPILE_DEBUG;
            flags |= D3DCompiler.D3DCOMPILE.D3DCOMPILE_OPTIMIZATION_LEVEL3; // if can't get shader to compile 1-1 add or remove this line
            byte[] shader_code = D3DCompiler.Compile(
                shader_source,
                entry,
                version,
                macros.ToArray(),
                flags,
                0,
                template,
                include
            );

            GC.KeepAlive(include?.NativePointer);
            return shader_code;
        }

        public static string CreateMethodDefinition(object method, string prefix = "", string suffix = "")
        {
            var method_type_name = method.GetType().Name.ToLower();
            var method_name = method.ToString().ToLower();

            return $"{prefix.ToLower()}{method_name}{suffix.ToLower()}";
        }

        public static D3D.SHADER_MACRO CreateMacro(string name, string definition)
        {
            return new D3D.SHADER_MACRO { Name = name, Definition = definition };
        }

        public static D3D.SHADER_MACRO CreateMacro(string name, object method, string prefix = "", string suffix = "")
        {
            return new D3D.SHADER_MACRO { Name = name, Definition = CreateMethodDefinition(method, prefix, suffix) };
        }
    }
}
