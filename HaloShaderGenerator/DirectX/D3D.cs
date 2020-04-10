

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



namespace HaloShaderGenerator.DirectX
{
    public static class D3D
    {
        const string d3dx9_43 = "D3DCompiler_43.dll";
        const string d3dx9_47 = "D3DCompiler_47.dll";

        public enum INCLUDE_TYPE : int
        {
            D3D_INCLUDE_LOCAL = 0,
            D3D_INCLUDE_SYSTEM = 1,
            D3D10_INCLUDE_LOCAL = D3D_INCLUDE_LOCAL,
            D3D10_INCLUDE_SYSTEM = D3D_INCLUDE_SYSTEM,
            D3D_INCLUDE_FORCE_DWORD = 0x7fffffff
        }

        /// <summary>
        /// C# wrapper for D3D9 shader macro
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct SHADER_MACRO
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string Name;
            [MarshalAs(UnmanagedType.LPStr)]
            public string Definition;
        }

        /// <summary>
        /// C# wrapper for the interface ID3DBlob used to store data in a buffer
        /// </summary>
        [Guid("8BA5FB08-5195-40e2-AC58-0D989C3A0102")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ID3DBlob
        {
            [PreserveSig]
            IntPtr GetBufferPointer();
            [PreserveSig]
            int GetBufferSize();
        }

        [PreserveSig]
        [DllImport(d3dx9_43)]
        public extern static int D3DCompileFromFile(
            [MarshalAs(UnmanagedType.LPTStr)] string pFilename,
            [In, Out] SHADER_MACRO[] pDefines,
            IntPtr pInclude,
            [MarshalAs(UnmanagedType.LPStr)] string pEntrypoint,
            [MarshalAs(UnmanagedType.LPStr)] string pTarget,
            uint flags1,
            uint flags2,
            ref ID3DBlob ppCode,
            ref ID3DBlob ppErrorMsgs);


        [PreserveSig]
        [DllImport(d3dx9_43)]
        public extern static int D3DAssemble(
            [In] byte[] pSrcData,
            [In] UIntPtr SrcDataSize,
            [MarshalAs(UnmanagedType.LPStr)] string pSourceName,
            [In, Out] SHADER_MACRO[] pDefines,
            IntPtr pInclude,
            uint Flags,
            ref ID3DBlob ppCode,
            ref ID3DBlob ppErrorMsgs);

        [PreserveSig]
        [DllImport(d3dx9_43)]
        public extern static int D3DCompile(
            [In] byte[] pSrcData,
            [In] UIntPtr SrcDataSize,
            [MarshalAs(UnmanagedType.LPStr)] string pSourceName,
            [In, Out] SHADER_MACRO[] pDefines,
            IntPtr pInclude,
            [MarshalAs(UnmanagedType.LPStr)] string pEntrypoint,
            [MarshalAs(UnmanagedType.LPStr)] string pTarget,
            uint flags1,
            uint flags2,
            ref ID3DBlob ppCode,
            ref ID3DBlob ppErrorMsgs);

        [PreserveSig]
        [DllImport(d3dx9_43)]
        public extern static int D3DDisassemble(
            [In] byte[] pSrcData,
            [In] UIntPtr SrcDataSize,
            uint flags,
            [MarshalAs(UnmanagedType.LPStr)] string szComments,
            ref ID3DBlob ppDisassembly);
    }
}
