#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

using System.Runtime.InteropServices;

namespace WatiN.Core.Native.Windows
{
    [ComImport, Guid("0000011B-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOleContainer
    {
        [PreserveSig]
        int ParseDisplayName(
            [In, MarshalAs(UnmanagedType.Interface)] object pbc,
            [In, MarshalAs(UnmanagedType.BStr)] string pszDisplayName,
            [Out, MarshalAs(UnmanagedType.LPArray)] int[] pchEaten,
            [Out, MarshalAs(UnmanagedType.LPArray)] object[] ppmkOut
            );

        [PreserveSig]
        int EnumObjects(
            [In, MarshalAs(UnmanagedType.U4)] NativeMethods.tagOLECONTF grfFlags,
            out IEnumUnknown ppenum
            );

        [PreserveSig]
        int LockContainer(bool fLock);
    }
}