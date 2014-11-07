using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
//using System.Runtime.InteropServices.ComTypes;

namespace CoreAudioApi.COMInterop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct PROPVARIANT
    {
        ushort vt;
        ushort wReserved1;
        ushort wReserved2;
        ushort wReserved3;
        IntPtr ptr;
        uint v;


        public sbyte cVal
        {
            get
            {
                ExpectsType(VarEnum.VT_I1);
                return (sbyte)getDataBytes()[0];
            }
        }

        public byte bVal
        {
            get
            {
                ExpectsType(VarEnum.VT_UI1);
                return (byte)getDataBytes()[0];
            }
        }

        public short iVal
        {
            get
            {
                ExpectsType(VarEnum.VT_I2);
                return BitConverter.ToInt16(getDataBytes(), 0);
            }
        }

        public ushort uiVal
        {
            get
            {
                ExpectsType(VarEnum.VT_UI2);
                return BitConverter.ToUInt16(getDataBytes(), 0);
            }
        }

        public long lVal
        {
            get
            {
                ExpectsType(VarEnum.VT_I4);
                return BitConverter.ToInt32(getDataBytes(), 0);
            }
        }

        public ulong ulVal
        {
            get
            {
                ExpectsType(VarEnum.VT_UI4);
                return BitConverter.ToUInt32(getDataBytes(), 0);
            }
        }

        public int intVal
        {
            get
            {
                ExpectsType(VarEnum.VT_INT);
                return BitConverter.ToInt32(getDataBytes(), 0);
            }
        }

        public uint uintVal
        {
            get
            {
                ExpectsType(VarEnum.VT_UINT);
                return BitConverter.ToUInt32(getDataBytes(), 0);
            }
        }

        public long hVal
        {
            get
            {
                ExpectsType(VarEnum.VT_I8);
                return BitConverter.ToInt64(getDataBytes(), 0);
            }
        }

        public ulong uhVal
        {
            get
            {
                ExpectsType(VarEnum.VT_UI8);
                return BitConverter.ToUInt64(getDataBytes(), 0);
            }
        }

        public float fltVal
        {
            get
            {
                ExpectsType(VarEnum.VT_R4);
                return BitConverter.ToSingle(getDataBytes(), 0);
            }
        }

        public double dblVal
        {
            get
            {
                ExpectsType(VarEnum.VT_R8);
                return BitConverter.ToDouble(getDataBytes(), 0);
            }
        }

        public bool boolVal
        {
            get
            {
                ExpectsType(VarEnum.VT_BOOL);
                return getDataBytes()[0] == 0;
            }
        }

        public object Value
        {
            get 
            {
                switch ((VarEnum)vt)
                {
                    case VarEnum.VT_I1:
                        return cVal;
                    case VarEnum.VT_UI1:
                        return bVal;
                    case VarEnum.VT_I2:
                        return iVal;
                    case VarEnum.VT_UI2:
                        return uiVal;
                    case VarEnum.VT_I4:
                        return lVal;
                    case VarEnum.VT_UI4:
                        return ulVal;
                    case VarEnum.VT_INT:
                        return intVal;
                    case VarEnum.VT_UINT:
                        return uintVal;
                    case VarEnum.VT_I8:
                        return hVal;
                    case VarEnum.VT_UI8:
                        return uhVal;
                    case VarEnum.VT_R4:
                        return fltVal;
                    case VarEnum.VT_R8:
                        return dblVal;
                    case VarEnum.VT_BOOL:
                        return boolVal;
                    case VarEnum.VT_LPSTR:
                        return Marshal.PtrToStringAnsi(ptr);
                    case VarEnum.VT_LPWSTR:
                        return Marshal.PtrToStringUni(ptr);
                    case VarEnum.VT_BLOB:
                    {
                        byte[] propBytes = getDataBytes();
                        byte[] blob = new byte[BitConverter.ToInt32(propBytes, 0)];

                        IntPtr dataPtr;
                        if (IntPtr.Size == 4)
                            dataPtr = new IntPtr(v);
                        else if (IntPtr.Size == 8)
                            dataPtr = new IntPtr(BitConverter.ToInt64(propBytes, sizeof(int)));
                        else
                            throw new NotSupportedException();

                        Marshal.Copy(dataPtr, blob, 0, blob.Length);

                        return blob;
                    }
                    case VarEnum.VT_CLSID:
                    {
                        byte[] guidBuffer = new byte[16];
                        Marshal.Copy(ptr, guidBuffer, 0, guidBuffer.Length);

                        return new Guid(guidBuffer);
                    }
                    default:
                        throw new NotSupportedException("Type of variable " + ((VarEnum)vt) + " not supported.");
                }
            }
        }

        public VarEnum Type
        {
            get
            {
                return (VarEnum)vt;
            }
        }

        [DllImport("ole32.dll")]
        private extern static int PropVariantClear(ref PROPVARIANT pvar);

        public void Clear()
        {
            PROPVARIANT theProp = this;
            PropVariantClear(ref theProp);

            vt = (ushort)VarEnum.VT_EMPTY;
            wReserved1 = wReserved2 = wReserved3 = 0;
            ptr = IntPtr.Zero;
            v = 0;
        }

        private byte[] getDataBytes()
        {
            byte[] buffer = new byte[IntPtr.Size + sizeof(int)];

            if (IntPtr.Size == 4)
                BitConverter.GetBytes(ptr.ToInt32()).CopyTo(buffer, 0);
            else if(IntPtr.Size == 8)
                BitConverter.GetBytes(ptr.ToInt64()).CopyTo(buffer, 0);
            BitConverter.GetBytes(v).CopyTo(buffer, IntPtr.Size);

            return buffer;
        }

        private void ExpectsType(VarEnum ve)
        {
            if ((VarEnum)vt != ve)
                throw new InvalidCastException("The PROPVARIANT is not a " + ve + ". (Is a " + ((VarEnum)vt) + ")");
        }
    }
}
