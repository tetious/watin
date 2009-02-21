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

using System;

namespace WatiN.Core.Native
{
    public interface IHwnd
    {
        IntPtr hwnd { get; }

        string WindowText { get; }

        bool IsWindow { get; }
        
        bool IsWindowEnabled { get; }

        bool IsWindowVisible { get; }

        string ClassName { get; }

        void SendMessage(int msg, int wParam, int lParam);
        
        IntPtr GetDlgItem(int controlid);
        
        void SendString(string s);
        
        void SetFocus();
    }
}