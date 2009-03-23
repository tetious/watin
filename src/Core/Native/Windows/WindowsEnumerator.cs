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
using System.Collections;
using System.Collections.Generic;
using mshtml;
using SHDocVw;
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;
using IServiceProvider = WatiN.Core.Native.Windows.IServiceProvider;

namespace WatiN.Core.Native.InternetExplorer
{
	/// <summary> 
	/// Enumerate top-level and child windows 
	/// </summary> 
	public class WindowsEnumerator
	{
	    public delegate bool WindowEnumConstraint(Window window);
	
	    private List<Window> _windows;
	    private WindowEnumConstraint _constraint;
	
	    /// <summary> 
	    /// Get all top-level window information 
	    /// </summary> 
	    /// <returns>List of window information objects</returns> 
	    public List<Window> GetTopLevelWindows()
	    {
	        return GetTopLevelWindows(null);
	    }
	
	    public List<Window> GetTopLevelWindows(string className)
	    {
	        _constraint = window => !window.HasParentWindow && NativeMethods.CompareClassNames(window.Hwnd, className);
	        return GetWindows(_constraint);
	    }
	
	    public List<Window> GetWindows(WindowEnumConstraint constraint)
	    {
	        _constraint = constraint;
	        _windows = new List<Window>();
	
	        var hwnd = IntPtr.Zero;
	        NativeMethods.EnumWindows(EnumWindowProc, hwnd);
	        return _windows;
	    }
	
	    /// <summary> 
	    /// Get all child windows for the specific windows handle (hwnd). 
	    /// </summary> 
	    /// <returns>List of child windows for parent window</returns> 
	    public List<Window> GetChildWindows(IntPtr hwnd)
	    {
	        return GetChildWindows(hwnd, null);
	    }
	
	    public List<Window> GetChildWindows(IntPtr hwnd, string childClass)
	    {
	        _constraint = window => NativeMethods.CompareClassNames(window.Hwnd, childClass);
	
	        _windows = new List<Window>();
	
	        var hWnd = IntPtr.Zero;
	        NativeMethods.EnumChildWindows(hwnd, EnumWindowProc, ref hWnd);
	
	        return _windows;
	    }
	
	    /// <summary> 
	    /// Callback function that does the work of enumerating top-level windows. 
	    /// </summary> 
	    /// <param name="hwnd">Discovered Window handle</param> 
	    /// <param name="lParam"></param>
	    /// <returns>1=keep going, 0=stop</returns> 
	    private bool EnumWindowProc(IntPtr hwnd, ref IntPtr lParam)
	    {
	        MatchWindow(new Window(hwnd));
	        return true;
	    }
	
	    private void MatchWindow(Window window)
	    {
	        // Match the class name if searching for a specific window class. 
	        if (_constraint == null || _constraint.Invoke(window))
	        {
	            _windows.Add(window);
	        }
	    }
	}
}
