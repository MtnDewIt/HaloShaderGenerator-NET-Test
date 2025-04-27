using HaloShaderGenerator.DirectX;
using System;
using System.Collections.Generic;
using System.IO;

namespace HaloShaderGenerator
{
    public class ShaderGeneratorResult
    {
        public List<ShaderRegister> Registers = new List<ShaderRegister>();
        public byte[] Bytecode = null;

        public class ShaderRegister
        {
            public ShaderRegister(string name, ShaderRegisterType type, int register, int size)
            {
                Name = name;
                RegisterType = type;
                Size = size;
                Register = register;
            }

            public string Name { get; }
            public ShaderRegisterType RegisterType { get; }
            public int Size { get; }
            public int Register { get; }

            public enum ShaderRegisterType
            {
                Vector,
                Boolean,
                Integer,
                Sampler
            }
        }

        public List<ShaderRegister> GetRegisters(string input)
        {
            List<ShaderRegister> result = new List<ShaderRegister>();
            bool found_registers = false;
            using (StringReader reader = new StringReader(input))
            {
                while (true)
                {
                    if (!found_registers)
                    {
                        var line = reader.ReadLine();
                        if (line == null)
                            break;
                        found_registers = line.Contains("Registers:");
                        if (found_registers)
                        {
                            reader.ReadLine();
                            reader.ReadLine();
                            reader.ReadLine();
                        }
                    }
                    else
                    {
                        var register_line = reader.ReadLine().Replace("//", "").Trim();
                        if (string.IsNullOrWhiteSpace(register_line)) break;

                        var register_components = register_line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        ShaderRegister.ShaderRegisterType register_type = ShaderRegister.ShaderRegisterType.Vector;

                        switch (register_components[1][0])
                        {
                            case 'b':
                                register_type = ShaderRegister.ShaderRegisterType.Boolean;
                                break;
                            case 'i':
                                register_type = ShaderRegister.ShaderRegisterType.Integer;
                                break;
                            case 'c':
                                register_type = ShaderRegister.ShaderRegisterType.Vector;
                                break;
                            case 's':
                                register_type = ShaderRegister.ShaderRegisterType.Sampler;
                                break;
                        }

                        var register = Int32.Parse(register_components[1].Substring(1));
                        var size = Int32.Parse(register_components[2]);

                        result.Add(new ShaderRegister(register_components[0], register_type, register, size));
                    }
                }
            }
            return result;
        }

        public Dictionary<string, string> GetRegisterParameters(string input)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            bool found_registers = false;
            using (StringReader reader = new StringReader(input))
            {
                while (true)
                {
                    if (!found_registers)
                    {
                        var line = reader.ReadLine();
                        if (line == null)
                            break;
                        found_registers = line.Contains("Parameters:");
                        if (found_registers)
                        {
                            reader.ReadLine();
                        }
                    }
                    else
                    {
                        var register_line = reader.ReadLine().Replace("//", "").Trim();
                        if (string.IsNullOrWhiteSpace(register_line)) break;

                        var register_components = register_line.Split(new char[] { ' ', ';', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

                        result[register_components[1]] = register_components[0];
                    }
                }
            }
            return result;
        }

        public ShaderGeneratorResult(byte[] bytecode)
        {
            Bytecode = bytecode;
            if (bytecode == null) return;
            var result = D3DCompiler.Disassemble(bytecode);
            if (result == null) return;

            var registers = GetRegisters(result);
            var parameters = GetRegisterParameters(result);

            Registers = registers;
            /*
            foreach (var parameters_kp in parameters)
            {
                var name = parameters_kp.Key;
                var typename = parameters_kp.Value;

                ShaderRegister.RegisterType register_type;
                {
                    ShaderRegister.RegisterType? _register_type = null;

                    switch (typename)
                    {
                        case "float4":
                        case "float3":
                        case "float2":
                        case "float":
                            _register_type = ShaderRegister.RegisterType.Vector;
                            break;
                        case "int":
                            _register_type = ShaderRegister.RegisterType.Integer;
                            break;
                        case "bool":
                            _register_type = ShaderRegister.RegisterType.Boolean;
                            break;
                    }
                    if (typename.StartsWith("sampler"))
                    {
                        _register_type = ShaderRegister.RegisterType.Sampler;
                    }

                    register_type = _register_type ?? ShaderRegister.RegisterType.Vector;
                }

                foreach (var register in registers)
                {
                    if (register.Name == name && register.registerType == register_type)
                    {
                        // Integers have an associated Vector, find and add it
                        if (register_type == ShaderRegister.RegisterType.Integer)
                        {
                            foreach (var register2 in registers)
                            {
                                if (register2.Name == name && register2.registerType == ShaderRegister.RegisterType.Vector)
                                {
                                    Registers.Add(register2);
                                    break;
                                }
                            }
                        }

                        Registers.Add(register);
                        break;
                    }

                    // for bool compiled as float
                    else if (register.Name == name && 
                        register.registerType == ShaderRegister.RegisterType.Vector &&
                        register_type == ShaderRegister.RegisterType.Boolean)
                    {
                        Registers.Add(register);
                        break;
                    }

                    // slight hack, for int's that get compiled only as vector
                    else if (register.Name == name && register_type == ShaderRegister.RegisterType.Integer)
                    {
                        foreach (var register2 in registers)
                        {
                            if (register2.Name == name && register2.registerType == ShaderRegister.RegisterType.Vector)
                            {
                                Registers.Add(register2);
                                break;
                            }
                        }
                    }
                }
            }
            */
        }
    }

}
