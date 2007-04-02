#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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
using System.Threading;
using System.Runtime.InteropServices;

using mshtml;
using SHDocVw;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
  using WatiN.Core.Exceptions;

  /// <summary>
  /// This class hosts functionality for classes which are an entry point
  /// to a document and its elements and/or frames. Currently implemented
  /// by IE and HTMLDialog.
  /// </summary>
  public abstract class DomContainer : Document
  {
    private IHTMLDocument2 htmlDocument;
    private SimpleTimer waitForCompleteTimeout;
    private DialogWatcher dialogWatcher;
    private bool disposed = false;

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
    internal abstract IHTMLDocument2 OnGetHtmlDocument();

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
    /// Call this function (from a subclass) as soon as the process is started.
    /// </summary>
    protected void StartDialogWatcher()
    {
      if (dialogWatcher == null)
      {
        dialogWatcher = DialogWatcher.GetDialogWatcherForProcess(ProcessID);
        dialogWatcher.IncreaseReferenceCount();
      }
    }

    /// <summary>
    /// Gets the dialog watcher.
    /// </summary>
    /// <value>The dialog watcher.</value>
    public DialogWatcher DialogWatcher
    {
      get
      {
        return dialogWatcher;
      }
    }

    /// <summary>
    /// Adds the dialog handler.
    /// </summary>
    /// <param name="handler">The dialog handler.</param>
    public void AddDialogHandler(IDialogHandler handler)
    {
      DialogWatcher.Add(handler);
    }

    /// <summary>
    /// Removes the dialog handler.
    /// </summary>
    /// <param name="handler">The dialog handler.</param>
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
      // TODO: Passing the eventarguments in a new param of type array. This array
      //       holds 0 or more name/value pairs where the name is a property of the event object
      //       and the value is the value that's assigned to the property.

      // Execute the JScript to fire the event inside the Browser.
      string scriptCode = "var newEvt = document.createEventObject();";
      scriptCode += "newEvt.button = 1;";
      scriptCode += "document.getElementById('" + element.uniqueID + "').fireEvent('" + eventName + "', newEvt);";


      try
      {
        RunScript(scriptCode);
      }
      catch (RunScriptException)
      {
        // In a cross domain automation scenario a System.UnauthorizedAccessException 
        // is thrown. The following code works, but setting the event properties
        // has no effect so that is left out.
        object dummyEvt = null;
        //      IHTMLEventObj2 mouseDownEvent = (IHTMLEventObj2)parentEvt;
        //      mouseDownEvent.button = 1;
        object parentEvt = ((IHTMLDocument4)element.document).CreateEventObject(ref dummyEvt);
        element.FireEvent(eventName, ref parentEvt);
      }
    }

    /// <summary>
    /// Runs the javascript code in IE.
    /// </summary>
    /// <param name="scriptCode">The javascript code.</param>
    public void RunScript(string scriptCode)
    {
      RunScript(scriptCode, "javascript");
    }

    /// <summary>
    /// Runs the script code in IE.
    /// </summary>
    /// <param name="scriptCode">The script code.</param>
    /// <param name="language">The language.</param>
    public void RunScript(string scriptCode, string language)
    {
      try
      {
        IHTMLWindow2 window = htmlDocument.parentWindow;
        window.execScript(scriptCode, language);
      }
      catch (Exception ex)
      {
        throw new WatiN.Core.Exceptions.RunScriptException(ex);
      }
    }

    /// <summary>
    /// This method must be called by its inheritor to dispose references
    /// to internal resources.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      if (!disposed)
      {
        htmlDocument = null;
        if (dialogWatcher != null)
        {
          DialogWatcher.DecreaseReferenceCount();
          dialogWatcher = null;
        }        
        disposed = true;

        base.Dispose(true);        
      }
    }

    /// <summary>
    /// This method calls InitTimeOut and waits till IE is ready
    /// processing or the timeout period (30 seconds) has expired.
    /// To change the default time out, set <see cref="P:WatiN.Core.IE.Settings.WaitForCompleteTimeOut"/>
    /// </summary>
    public virtual void WaitForComplete()
    {
      InitTimeout();
      WaitForCompleteOrTimeout();
    }

    /// <summary>
    /// This method waits till IE is ready processing 
    /// or the timeout period has expired. You should
    /// call InitTimeout prior to calling this method.
    /// </summary>
    protected internal void WaitForCompleteOrTimeout()
    {
      WaitWhileMainDocumentNotAvailable(this);
      WaitWhileDocumentStateNotComplete(HtmlDocument);
      WaitForFramesToComplete(HtmlDocument);
    }

    private void WaitForFramesToComplete(IHTMLDocument2 maindocument)
    {
      HTMLDocument mainHtmlDocument = (HTMLDocument) maindocument;
      
      int framesCount = WatiN.Core.Frame.GetFrameCountFromHTMLDocument(mainHtmlDocument);
      
      for (int i = 0; i != framesCount; ++i)
      {
        IWebBrowser2 frame = WatiN.Core.Frame.GetFrameFromHTMLDocument(i, mainHtmlDocument);
        
        if (frame != null)
        {
          IHTMLDocument2 document;

          try
          {
            waitWhileIEBusy(frame);
            waitWhileIEStateNotComplete(frame);
            document = WaitWhileFrameDocumentNotAvailable(frame);
          }
          finally
          {
            // free frame
            Marshal.ReleaseComObject(frame);
          }           
         
          WaitWhileDocumentStateNotComplete(document);
          WaitForFramesToComplete(document);
        }
      }
    }

    /// <summary>
    /// This method is called to initialise the start time for
    /// determining a time out. It's set to the current time.
    /// </summary>
    /// <returns></returns>
    protected internal SimpleTimer InitTimeout()
    {
      waitForCompleteTimeout = new SimpleTimer(IE.Settings.WaitForCompleteTimeOut);
      return waitForCompleteTimeout;
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
      return waitForCompleteTimeout.Elapsed;
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

        document = IsDocumentAvailable(document, "maindocument");
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
        {}
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
        Thread.Sleep(500);
      }
      return document;
    }

    /// <summary>
    /// Gets the process ID the Internet Explorer or HTMLDialog is running in.
    /// </summary>
    /// <value>The process ID.</value>
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
      while (IsIEReadyStateComplete(ie))
      {
        ThrowExceptionWhenTimeout("Internet Explorer state not complete");

        Thread.Sleep(100);        
      }
    }

    private static bool IsIEReadyStateComplete(IWebBrowser2 ie)
    {
      try
      {
        return ie.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE;
      }
      catch
      {
        return false;
      }
    }

    protected void waitWhileIEBusy(IWebBrowser2 ie)
    {
      while (IsIEBusy(ie))
      {
        ThrowExceptionWhenTimeout("Internet Explorer busy");

        Thread.Sleep(100);
      }
    }

    private static bool IsIEBusy(IWebBrowser2 ie)
    {
      try
      {
        return ie.Busy;
      }
      catch
      {
        return false;
      }
    }
  }  
}
