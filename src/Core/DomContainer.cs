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
using System.Threading;
using System.Runtime.InteropServices;

using mshtml;
using SHDocVw;
using WatiN.Core.DialogHandlers;

namespace WatiN.Core
{
  /// <summary>
  /// This class hosts functionality for classes which are an entry point
  /// to a document and its elements and/or frames. Currently implemented
  /// by IE and HTMLDialog.
  /// </summary>
  public abstract class DomContainer : Document
  {
    private IHTMLDocument2 htmlDocument;
    private DateTime startWaitForComplete;
    private DialogWatcher dialogWatcher;

    public DomContainer()
    {
      DomContainer = this;
    }
    
    public abstract IntPtr hWnd
    {
      get;
    }
    
    /// <summary>
    /// This method must be overriden by all sub classes
    /// </summary>
    public virtual IHTMLDocument2 OnGetHtmlDocument()
    {
      throw new NotImplementedException("This method must be overridden by all sub classes");
    }

    /// <summary>
    /// Returns the 'raw' html document for the internet explorer DOM.
    /// </summary>
    public override IHTMLDocument2 HtmlDocument
    {
      get
      {
        if (htmlDocument == null)
        {
          htmlDocument = OnGetHtmlDocument();
        }

        return htmlDocument;
      }
    }

    /// <summary>
    /// The IE.MainDocument.xxx syntax is no longer supported, use IE.xxx instead. 
    /// </summary>
    /// <value>The main document.</value>
    [Obsolete("The IE.MainDocument.xxx syntax is no longer supported, use IE.xxx instead.")]
    public Document MainDocument
    {
      get
      {
        return this;
      }
    }

    protected void StartDialogWatcher()
    {
      dialogWatcher = DialogWatcher.GetDialogWatcherForProcess(ProcessID);
    }

    public DialogWatcher DialogWatcher
    {
      get
      {
        return dialogWatcher;
      }
    }

    public void AddDialogHandler(IDialogHandler handler)
    {
      DialogWatcher.Add(handler);
    }

    public void RemoveDialogHandler(IDialogHandler handler)
    {
      DialogWatcher.Remove(handler);
    }
    
    /// <summary>
    /// Fires the given event on the given element.
    /// </summary>
    /// <param name="element">Element to fire the event on</param>
    /// <param name="eventName">Name of the event to fire</param>
    public virtual void FireEvent(DispHTMLBaseElement element, string eventName)
    {
      object dummyEvt = null;
      object parentEvt = ((IHTMLDocument4)element.document).CreateEventObject(ref dummyEvt);
      //      IHTMLEventObj2 mouseDownEvent = (IHTMLEventObj2)parentEvt;
      //      mouseDownEvent.button = 1;
      element.FireEvent(eventName, ref parentEvt);
    }

    /// <summary>
    /// This method must be called by its inheritor to dispose references
    /// to internal resources.
    /// </summary>
    internal override void Dispose()
    {
      base.Dispose();
      htmlDocument = null;
      dialogWatcher = null;
    }

    /// <summary>
    /// This method calls InitTimeOut and waits till IE is ready
    /// processing or the timeout periode has expired.
    /// </summary>
    public virtual void WaitForComplete()
    {
      InitTimeout();
      WaitForCompleteOrTimeout();
    }

    /// <summary>
    /// This method waits till IE is ready
    /// processing or the timeout periode has expired. You should
    /// call InitTimeout prior to calling this method.
    /// </summary>
    protected internal void WaitForCompleteOrTimeout()
    {
      WaitWhileMainDocumentNotAvailable(this);
      WaitWhileDocumentStateNotComplete(HtmlDocument);

      int framesCount = HtmlDocument.frames.length;
      for (int i = 0; i != framesCount; ++i)
      {
        IWebBrowser2 frame = WatiN.Core.Frame.GetFrameFromHTMLDocument(i, (HTMLDocument) HtmlDocument);
        
        if (frame != null)
        {
          waitWhileIEBusy(frame);
          waitWhileIEStateNotComplete(frame);
          
          IHTMLDocument2 document = WaitWhileFrameDocumentNotAvailable(frame);
          WaitWhileDocumentStateNotComplete(document);

          // free frame
          Marshal.ReleaseComObject(frame);
        }
      }
    }

    /// <summary>
    /// This method is called to initialise the start time for
    /// determining a time out. It's set to the current time.
    /// </summary>
    /// <returns></returns>
    protected internal DateTime InitTimeout()
    {
      return startWaitForComplete = DateTime.Now;
    }

    /// <summary>
    /// This method checks the return value of IsTimedOut. When true, it will
    /// throw a TimeoutException with the timeoutMessage param as message.
    /// </summary>
    /// <param name="timeoutMessage">The message to present when the TimeoutException is thrown</param>
    protected internal void ThrowExceptionWhenTimeout(string timeoutMessage)
    {
      if (IsTimedOut())
      {
        throw new Exceptions.TimeoutException(timeoutMessage);
      }
    }

    /// <summary>
    /// This method evaluates the time between the last call to InitTimeOut
    /// and the current time. If the timespan is more than 30 seconds, the
    /// return value will be true.
    /// </summary>
    /// <returns>If the timespan is more than 30 seconds, the
    /// return value will be true</returns>
    protected internal bool IsTimedOut()
    {
      return UtilityClass.IsTimedOut(startWaitForComplete, 30);
    }

    private void WaitWhileDocumentStateNotComplete(IHTMLDocument2 htmlDocument)
    {
      HTMLDocument document = (HTMLDocument)htmlDocument;
      while (document.readyState != "complete")
      {
        ThrowExceptionWhenTimeout("waiting for document state complete. Last state was '" + document.readyState + "'");
        Thread.Sleep(100);
      }
    }

    private void WaitWhileMainDocumentNotAvailable(DomContainer domContainer)
    {
      IHTMLDocument2 document = null;

      while(document == null)
      {
        try
        {
          document = domContainer.HtmlDocument;
        }
        catch{}

        document = IsDocumentAvailable(document, "frame");
      }
    }

    private IHTMLDocument2 WaitWhileFrameDocumentNotAvailable(IWebBrowser2 frame)
    {
      IHTMLDocument2 document = null;

      while (document == null)
      {
        try
        {
          document = frame.Document as IHTMLDocument2;
        }
        catch{}

        document = IsDocumentAvailable(document, "frame");
      }

      return document;
    }

    private static bool IsDocumentReadyStateAvailable(IHTMLDocument2 document)
    {
      if (document != null)
      {
        // Sometimes an OutOfMemoryException or ComException occurs while accessing
        // the readystate property of IHTMLDocument2. Giving MSHTML some time
        // to do further processing seems to solve this problem.
        try
        {
          string readyState = ((HTMLDocument)document).readyState;
          return true;
        }
        catch
        {
          Thread.Sleep(500);
        }
      }

      return false;
    }

    private IHTMLDocument2 IsDocumentAvailable(IHTMLDocument2 document, string documentType)
    {
      if (document == null)
      {
        ThrowExceptionWhenTimeout(String.Format("waiting for {0} document becoming available", documentType));
          
        Thread.Sleep(100);
      }
      else if (!IsDocumentReadyStateAvailable(document))
      {
        document = null;
      }
      return document;
    }

    public int ProcessID
    {
      get
      {
        int iePid;

        NativeMethods.GetWindowThreadProcessId(hWnd, out iePid);

        return iePid;
      }
    }

    protected void waitWhileIEStateNotComplete(IWebBrowser2 ie)
    {
      while (ie.ReadyState !=  tagREADYSTATE.READYSTATE_COMPLETE)
      {
        ThrowExceptionWhenTimeout("Internet Explorer state not complete");

        Thread.Sleep(100);        
      }
    }

    protected void waitWhileIEBusy(IWebBrowser2 ie)
    {
      while (ie.Busy)
      {
        ThrowExceptionWhenTimeout("Internet Explorer busy");

        Thread.Sleep(100);
      }
    }
  }
}
