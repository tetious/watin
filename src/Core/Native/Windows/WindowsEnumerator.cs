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
using System.Collections.Generic;
using WatiN.Core.Native.Windows;

namespace WatiN.Core.Native.InternetExplorer
{
	/// <summary> 
	/// Enumerate top-level and child windows 
	/// </summary> 
	public class WindowsEnumerator
	{
	    public delegate bool WindowEnumConstraint(Window window);
	
	    /// <summary> 
	    /// Get all top-level window information 
	    /// </summary> 
	    /// <returns>List of window information objects</returns> 
	    public IList<Window> GetTopLevelWindows()
	    {
	        return GetTopLevelWindows(null);
	    }
	
	    public IList<Window> GetTopLevelWindows(string className)
	    {
	        return GetWindows(window => !window.HasParentWindow && NativeMethods.CompareClassNames(window.Hwnd, className));
	    }
	
	    public IList<Window> GetWindows(WindowEnumConstraint constraint)
	    {
	        var windows = new List<Window>();
	
            NativeMethods.EnumWindows((hwnd, lParam) =>
                {
                    var window = new Window(hwnd);
                    if (constraint == null || constraint(window))
                        windows.Add(window);
                    
                    return true;
                }, IntPtr.Zero);

	        return windows;
	    }
	
	    /// <summary> 
	    /// Get all child windows for the specific windows handle (hwnd). 
	    /// </summary> 
	    /// <returns>List of child windows for parent window</returns> 
	    public IList<Window> GetChildWindows(IntPtr hwnd)
	    {
	        return GetChildWindows(hwnd, (string) null);
	    }
	
	    public IList<Window> GetChildWindows(IntPtr hwnd, string childClass)
	    {
            return GetChildWindows(hwnd, window => NativeMethods.CompareClassNames(window.Hwnd, childClass));
	    }

        public IList<Window> GetChildWindows(IntPtr hwnd, WindowEnumConstraint constraint)
        {
            var childWindows = new List<Window>();

            NativeMethods.EnumChildWindows(hwnd, (childHwnd, lParam) => 
            {
                var childWindow = new Window(childHwnd);
                if (constraint == null || constraint(childWindow))
                    childWindows.Add(childWindow);

                return true;
            }, IntPtr.Zero);

            return childWindows;
        }
	}
}
