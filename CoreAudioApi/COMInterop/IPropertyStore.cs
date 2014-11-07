using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace CoreAudioApi.COMInterop
{
    [Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyStore
    {
        void GetCount([Out] out uint cProps);
        void GetAt([In] uint iProp, [Out] out PROPERTYKEY pkey);
        void GetValue([In] ref PROPERTYKEY key, [Out] out PROPVARIANT pv);
        void SetValue([In] ref PROPERTYKEY key, [In] ref PROPVARIANT propvar);
        void Commit();
        
    }
}
