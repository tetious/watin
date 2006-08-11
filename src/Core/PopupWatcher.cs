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
using System.Text;
using System.Threading;

using WatiN.Core.Exceptions;

namespace WatiN.Core
{
  public class PopupWatcher
  {
    private int ieProcessId;
    private bool keepRunning;

    private System.Collections.Queue alertQueue;

    public PopupWatcher(int ieProcessId)
    {
      this.ieProcessId = ieProcessId;
      keepRunning = true;
      alertQueue = new System.Collections.Queue();
    }

    public int AlertCount()
    {
      return alertQueue.Count;
    }

    public string PopAlert()
    {
      if (alertQueue.Count == 0)
      {
        throw new MissingAlertException();
      }

      return (string) alertQueue.Dequeue();
    }

    public string[] Alerts
    {
      get
      {
        string[] result = new string[alertQueue.Count];
        Array.Copy(alertQueue.ToArray(), result, alertQueue.Count);
        return result;
      }
    }

    public void FlushAlerts()
    {
      alertQueue.Clear();
    }

    public void Run()
    {
      while (keepRunning)
      {
        Thread.Sleep(1000);

        if (keepRunning)
        {
        	System.Diagnostics.Process p;

        	try
        	{
      			p = System.Diagnostics.Process.GetProcessById(ieProcessId);
        	}
        	catch(ArgumentException)
      		{
        		// Thrown when the ieProcessId is not running (anymore)
        		p = null;
        	}
      		
      		if (p != null)
      		{
	    			foreach (System.Diagnostics.ProcessThread t in p.Threads)
		        {
		          int threadId = t.Id;
		
		          NativeMethods.EnumThreadProc callbackProc = new NativeMethods.EnumThreadProc(MyEnumThreadWindowsProc);
		          NativeMethods.EnumThreadWindows(threadId, callbackProc, IntPtr.Zero);
		        }
      		}
        }
      }
    }

    public void Stop()
    {
      keepRunning = false;
    }

    private bool MyEnumThreadWindowsProc(IntPtr hwnd, IntPtr lParam)
    {
      if (IsDialog(hwnd))
      {        
        IntPtr handleToDialogText = NativeMethods.GetDlgItem(hwnd, 0xFFFF);
        string alertMessage = GetText(handleToDialogText);
        alertQueue.Enqueue(alertMessage);

        NativeMethods.SendMessage(hwnd, NativeMethods.WM_CLOSE, 0, 0);
      }

      return true;
    }

    private bool IsDialog( IntPtr wParam )
    {
      StringBuilder className = new StringBuilder(255);
      NativeMethods.GetClassName(wParam, className, className.Capacity);

      return (className.ToString() == "#32770");
    }

    private static string GetText( IntPtr handle )
    {
      int length = NativeMethods.GetWindowTextLength(handle) + 1;
      StringBuilder buffer = new StringBuilder(length);
      NativeMethods.GetWindowText(handle, buffer, length);
			
      return buffer.ToString();
    }
  }
}