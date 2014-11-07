using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CoreAudioApi.COMInterop
{
    internal enum StgmAccess : uint
    {
        STGM_READ      =  0x00000000,
        STGM_WRITE     =  0x00000001,
        STGM_READWRITE =  0x00000002
    }

    [Flags]
    internal enum CLSCTX : uint
    { 
        CLSCTX_INPROC_SERVER           = 0x1,
        CLSCTX_INPROC_HANDLER          = 0x2,
        CLSCTX_LOCAL_SERVER            = 0x4,
        CLSCTX_INPROC_SERVER16         = 0x8,
        CLSCTX_REMOTE_SERVER           = 0x10,
        CLSCTX_INPROC_HANDLER16        = 0x20,
        CLSCTX_RESERVED1               = 0x40,
        CLSCTX_RESERVED2               = 0x80,
        CLSCTX_RESERVED3               = 0x100,
        CLSCTX_RESERVED4               = 0x200,
        CLSCTX_NO_CODE_DOWNLOAD        = 0x400,
        CLSCTX_RESERVED5               = 0x800,
        CLSCTX_NO_CUSTOM_MARSHAL       = 0x1000,
        CLSCTX_ENABLE_CODE_DOWNLOAD    = 0x2000,
        CLSCTX_NO_FAILURE_LOG          = 0x4000,
        CLSCTX_DISABLE_AAA             = 0x8000,
        CLSCTX_ENABLE_AAA              = 0x10000,
        CLSCTX_FROM_DEFAULT_CONTEXT    = 0x20000,
        CLSCTX_ACTIVATE_32_BIT_SERVER  = 0x40000,
        CLSCTX_ACTIVATE_64_BIT_SERVER  = 0x80000,
        CLSCTX_ENABLE_CLOAKING         = 0x100000,
        CLSCTX_APPCONTAINER            = 0x400000,
        CLSCTX_ACTIVATE_AAA_AS_IU      = 0x800000,
        CLSCTX_PS_DLL                  = 0x80000000,
        CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
        CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
        CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
    };

    [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDevice
    {
        void Activate([In] ref Guid iid, [In] CLSCTX dwClsCtx, [In] IntPtr pActivationParams, [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
        void OpenPropertyStore([In] StgmAccess stgmAccess, [Out] out IPropertyStore ppProperties);
        void GetId([Out, MarshalAs(UnmanagedType.LPWStr)] out string ppstrId);
        void GetState([Out] out uint pdwState);
        
    }
}
