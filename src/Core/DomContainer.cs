#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006-2007 Jeroen van Menen
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
using System.Threading;
using System.Runtime.InteropServices;

using mshtml;
using SHDocVw;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Interfaces;

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

      bool removeIdAttribute = false;
      
      // If the element has no Id, assign a temporary and unique Id so we can find 
      // the element within the java code (I know, it's a bit dirty hack)
      if (element.id == null)
      {
        element.id = Guid.NewGuid().ToString();
        removeIdAttribute = true;
      }

      // Execute the JScript to fire the event inside the Browser.
      FireEventOnElementWithJScript(element, eventName);

      // Remove Id attribute if temporary Id was assigned.
      if (removeIdAttribute)
      {
        element.removeAttribute("id", 0);
      }
    }

    private void FireEventOnElementWithJScript(DispHTMLBaseElement element, string eventName)
    {
      string scriptCode = "var newEvt = document.createEventObject();";
      scriptCode += "newEvt.button = 1;";
      scriptCode += "document.getElementById('" + element.id + "').fireEvent('" + eventName + "', newEvt);";

      IHTMLWindow2 window = ((HTMLDocument) element.document).parentWindow;

      try
      {
        window.execScript(scriptCode, "javascript");
      }
      catch (UnauthorizedAccessException)
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
        return ie.ReadyState !=  tagREADYSTATE.READYSTATE_COMPLETE;
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
  
  internal class WaitForFrameCompleteProcessor : IWebBrowser2Processor
  {
    public ArrayList elements;
    
    private HTMLDocument htmlDocument;
    private IHTMLElementCollection frameElements;
    private int index = 0;
    private DomContainer ie;
    
    public WaitForFrameCompleteProcessor(DomContainer ie, HTMLDocument htmlDocument)
    {
      elements = new ArrayList();

      frameElements = (IHTMLElementCollection)htmlDocument.all.tags(ElementsSupport.FrameTagName);
      
      // If the current document doesn't contain FRAME elements, it then
      // might contain IFRAME elements.
      if (frameElements.length == 0)
      {
        frameElements = (IHTMLElementCollection)htmlDocument.all.tags("IFRAME");
      }

      this.ie = ie;
      this.htmlDocument = htmlDocument;  
    }

    public HTMLDocument HTMLDocument()
    {
      return htmlDocument;
    }

    public void Process(IWebBrowser2 webBrowser2)
    {
      // Get the frame element from the parent document
      IHTMLElement frameElement = (IHTMLElement)frameElements.item(index, null);
            
      string frameName = null;
      string frameId = null;

      if (frameElement != null)
      {
        frameId = frameElement.id;
        frameName = frameElement.getAttribute("name", 0) as string;
      }

      Frame frame = new Frame(ie, webBrowser2.Document as IHTMLDocument2, frameName, frameId);
      elements.Add(frame);
                
      index++;
    }

    public bool Continue()
    {
      return true;
    }
  }

}
