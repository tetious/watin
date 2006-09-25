#region WatiN Copyright (C) 2006 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006 Jeroen van Menen
//
// This library is free software; you can redistribute it and/or modify it under the terms of the GNU 
// Lesser General Public License as published by the Free Software Foundation; either version 2.1 of 
// the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with this library; 
// if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 
// 02111-1307 USA 

#endregion Copyright

using System;
using System.Runtime.InteropServices;
using System.Text;

using mshtml;

namespace WatiN.Core
{
  /// <summary>
  /// This is the main class to access a webpage within a modal or modeles
  /// HTML dialog.
  /// </summary>
  public class HtmlDialog : DomContainer
	{
    private IntPtr hWnd = IntPtr.Zero;

    public static bool IsIETridentDlgFrame(IntPtr windowHandle)
    {
      return UtilityClass.CompareClassNames(windowHandle, "Internet Explorer_TridentDlgFrame");
    }

    public HtmlDialog(IntPtr windowHandle)
    {
      hWnd = windowHandle;
    }

    public void Close()
    {
      Dispose();
      NativeMethods.SendMessage(hWnd, NativeMethods.WM_CLOSE, 0, 0);
    }

    public bool HasFocus()
    {
      // TODO: Find Win32 API to determine if window has the focus
      return true;
    }

    public override IHTMLDocument2 OnGetHtmlDocument()
    {
      return IEDOMFromhWnd(hWnd);
    }

    private IHTMLDocument2 IEDOMFromhWnd(IntPtr hWnd)
    {
      Guid IID_IHTMLDocument2 = new Guid("626FC520-A41E-11CF-A731-00A0C9082637");

      Int32 lRes = 0;
      Int32 lMsg;
      Int32 hr;

      if (IsIETridentDlgFrame(hWnd))
      {
        if (!IsIEServerWindow(hWnd))
        {
          // Get 1st child IE server window
          NativeMethods.EnumChildProc childProc = new NativeMethods.EnumChildProc(EnumChildForServerWindow);
          NativeMethods.EnumChildWindows(hWnd, childProc, ref hWnd);
        }

        if (IsIEServerWindow(hWnd))
        {
          // Register the message
          lMsg = NativeMethods.RegisterWindowMessage("WM_HTML_GETOBJECT");
          // Get the object
          NativeMethods.SendMessageTimeout(hWnd, lMsg, 0, 0, NativeMethods.SMTO_ABORTIFHUNG, 1000, ref lRes);
          if (lRes != 0)
          {
            // Get the object from lRes
            IHTMLDocument2 ieDOMFromhWnd = null;
            hr = NativeMethods.ObjectFromLresult(lRes, ref IID_IHTMLDocument2, 0, ref ieDOMFromhWnd);
            if (hr != 0)
            {
              throw new COMException("ObjectFromLresult has thrown an exception", hr);
            }
            return ieDOMFromhWnd;
          }
        }
      }
      return null;
    }

    private bool EnumChildForServerWindow(IntPtr hWnd, ref IntPtr lParam)
    {
      if (IsIEServerWindow(hWnd))
      {
        lParam = hWnd;
        return false;
      }
      else
      {
        return true;
      }
    }

    private static bool IsIEServerWindow(IntPtr hWnd)
    {
      return UtilityClass.CompareClassNames(hWnd, "Internet Explorer_Server");
    }
	}
}
