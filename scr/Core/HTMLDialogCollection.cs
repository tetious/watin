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
using System.Collections;
using System.Diagnostics;

namespace WatiN
{
  public class HTMLDialogCollection : IEnumerable
  {
    private ArrayList htmlDialogs;

    public HTMLDialogCollection(Process ieProcess) 
    {
      htmlDialogs = new ArrayList();

      IntPtr hWnd = IntPtr.Zero;

      foreach (ProcessThread t in ieProcess.Threads)
      {
        int threadId = t.Id;

        Win32.EnumThreadProc callbackProc = new Win32.EnumThreadProc(EnumChildForTridentDialogFrame);
        Win32.EnumThreadWindows(threadId, callbackProc, hWnd);
      }
    }

    private bool EnumChildForTridentDialogFrame(IntPtr hWnd, IntPtr lParam)
    {
      if (HTMLDialog.IsIETridenDlgFrame(hWnd))
      {
        HTMLDialog htmlDialog = new HTMLDialog(hWnd);
        htmlDialogs.Add(htmlDialog);
      }

      return true;
    }


    public int length { get { return htmlDialogs.Count; } }

    public HTMLDialog this[int index] { get { return (HTMLDialog)htmlDialogs[index]; } }

    public Enumerator GetEnumerator() 
    {
      return new Enumerator(htmlDialogs);
    }

    IEnumerator IEnumerable.GetEnumerator() 
    {
      return GetEnumerator();
    }

    public class Enumerator: IEnumerator 
    {
      ArrayList children;
      int index;
      public Enumerator(ArrayList children) 
      {
        this.children = children;
        Reset();
      }

      public void Reset() 
      {
        index = -1;
      }

      public bool MoveNext() 
      {
        ++index;
        return index < children.Count;
      }

      public HTMLDialog Current 
      {
        get 
        {
          return (HTMLDialog)children[index];
        }
      }

      object IEnumerator.Current { get { return Current; } }
    }
  }
}