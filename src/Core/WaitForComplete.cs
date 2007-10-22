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

using System.Runtime.InteropServices;
using System.Threading;
using mshtml;
using SHDocVw;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	public class WaitForComplete : IWait
	{
		protected DomContainer _domContainer;
		protected SimpleTimer _waitForCompleteTimeout;

		public WaitForComplete(DomContainer _domContainer)
		{
			this._domContainer = _domContainer;
		}

		/// <summary>
		/// This method calls InitTimeOut and waits till IE is ready
		/// processing or the timeout period (30 seconds) has expired.
		/// To change the default time out, set <see cref="P:WatiN.Core.IE.Settings.WaitForCompleteTimeOut"/>
		/// </summary>
		public virtual void DoWait()
		{
			Thread.Sleep(100);

			InitTimeout();
			WaitForCompleteOrTimeout();
		}

		/// <summary>
		/// This method waits till IE is ready processing 
		/// or the timeout period has expired. You should
		/// call InitTimeout prior to calling this method.
		/// </summary>
		protected virtual void WaitForCompleteOrTimeout()
		{
			WaitWhileMainDocumentNotAvailable(_domContainer);
			WaitWhileDocumentStateNotComplete(_domContainer.HtmlDocument);
			WaitForFramesToComplete(_domContainer.HtmlDocument);
		}

		protected virtual void WaitForFramesToComplete(IHTMLDocument2 maindocument)
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
						WaitWhileIEBusy(frame);
						waitWhileIEStateNotComplete(frame);
						WaitWhileFrameDocumentNotAvailable(frame);

						document = (IHTMLDocument2) frame.Document;
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
		protected virtual SimpleTimer InitTimeout()
		{
			_waitForCompleteTimeout = new SimpleTimer(IE.Settings.WaitForCompleteTimeOut);
			return _waitForCompleteTimeout;
		}

		protected virtual void WaitWhileDocumentStateNotComplete(IHTMLDocument2 htmlDocument)
		{
			HTMLDocument document = (HTMLDocument) htmlDocument;
			while (document.readyState != "complete")
			{
				ThrowExceptionWhenTimeout("waiting for document state complete. Last state was '" + document.readyState + "'");
				Thread.Sleep(100);
			}
		}

		/// <summary>
		/// This method evaluates the time between the last call to InitTimeOut
		/// and the current time. If the timespan is more than 30 seconds, the
		/// return value will be true.
		/// </summary>
		/// <returns>If the timespan is more than 30 seconds, the
		/// return value will be true</returns>
		protected virtual bool IsTimedOut()
		{
			return _waitForCompleteTimeout.Elapsed;
		}

		/// <summary>
		/// This method checks the return value of IsTimedOut. When true, it will
		/// throw a TimeoutException with the timeoutMessage param as message.
		/// </summary>
		/// <param name="timeoutMessage">The message to present when the TimeoutException is thrown</param>
		protected virtual void ThrowExceptionWhenTimeout(string timeoutMessage)
		{
			if (IsTimedOut())
			{
				throw new Exceptions.TimeoutException(timeoutMessage);
			}
		}

		protected virtual void WaitWhileMainDocumentNotAvailable(DomContainer domContainer)
		{
			while (!IsDocumentReadyStateAvailable(GetDomContainerDocument(domContainer)))
			{
				ThrowExceptionWhenTimeout("waiting for main document becoming available");

				Thread.Sleep(100);
			}
		}

		protected virtual void WaitWhileFrameDocumentNotAvailable(IWebBrowser2 frame)
		{
			while (!IsDocumentReadyStateAvailable(GetFrameDocument(frame)))
			{
				ThrowExceptionWhenTimeout("waiting for frame document becoming available");

				Thread.Sleep(100);
			}
		}

		protected virtual IHTMLDocument2 GetFrameDocument(IWebBrowser2 frame)
		{
			try
			{
				return frame.Document as IHTMLDocument2;
			}
			catch
			{
				return null;
			}
		}

		protected virtual IHTMLDocument2 GetDomContainerDocument(DomContainer domContainer)
		{
			try
			{
				return domContainer.HtmlDocument;
			}
			catch
			{
				return null;
			}
		}

		protected virtual bool IsDocumentReadyStateAvailable(IHTMLDocument2 document)
		{
			if (document != null)
			{
				// Sometimes an OutOfMemoryException or ComException occurs while accessing
				// the readystate property of IHTMLDocument2. Giving MSHTML some time
				// to do further processing seems to solve this problem.
				try
				{
					string readyState = document.readyState;
					return true;
				}
				catch {}
			}

			return false;
		}

		protected virtual void waitWhileIEStateNotComplete(IWebBrowser2 ie)
		{
			while (IsIEReadyStateComplete(ie))
			{
				ThrowExceptionWhenTimeout("Internet Explorer state not complete");

				Thread.Sleep(100);
			}
		}

		protected virtual bool IsIEReadyStateComplete(IWebBrowser2 ie)
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

		protected virtual void WaitWhileIEBusy(IWebBrowser2 ie)
		{
			Thread.Sleep(100);

			while (IsIEBusy(ie))
			{
				ThrowExceptionWhenTimeout("Internet Explorer busy");

				Thread.Sleep(100);
			}
		}

		protected virtual bool IsIEBusy(IWebBrowser2 ie)
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