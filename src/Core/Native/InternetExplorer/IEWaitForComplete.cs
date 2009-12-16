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

using System.Runtime.InteropServices;
using mshtml;
using SHDocVw;
using WatiN.Core.Exceptions;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Native.InternetExplorer
{
    public class IEWaitForComplete : WaitForCompleteBase
    {
        private readonly IEBrowser ieBrowser;
        private IEDocument ieDocument;

        public IEWaitForComplete(IEBrowser ieBrowser)
            : this(ieBrowser, Settings.WaitForCompleteTimeOut)
        {
        }

        public IEWaitForComplete(IEBrowser ieBrowser, int waitForCompleteTimeout)
            : base(waitForCompleteTimeout)
        {
            this.ieBrowser = ieBrowser;
        }

        public IEWaitForComplete(IEDocument ieDocument)
            : this(ieDocument, Settings.WaitForCompleteTimeOut)
        {
        }

        public IEWaitForComplete(IEDocument ieDocument, int waitForCompleteTimeout)
            : base (waitForCompleteTimeout)
        {
            this.ieDocument = ieDocument;
        }

        protected override void WaitForCompleteOrTimeout()
        {
            if (ieBrowser != null)
            {
                if (!WaitWhileIEBusy(ieBrowser.WebBrowser))
                    return;
                if (!WaitWhileIEReadyStateNotComplete(ieBrowser.WebBrowser))
                    return;

                ieDocument = (IEDocument)ieBrowser.NativeDocument;
            }

            if (ieDocument == null) return;
            
            WaitWhileMainDocumentNotAvailable(ieDocument.HtmlDocument);
            WaitWhileDocumentStateNotComplete(ieDocument.HtmlDocument);
            WaitForFramesToComplete(ieDocument.HtmlDocument);
        }

        protected virtual void WaitForFramesToComplete(IHTMLDocument2 maindocument)
        {
            var mainHtmlDocument = (HTMLDocument)maindocument;

            var framesCount = FrameCountProcessor.GetFrameCountFromHTMLDocument(mainHtmlDocument);

            for (var i = 0; i != framesCount; ++i)
            {
                var frame = FrameByIndexProcessor.GetFrameFromHTMLDocument(i, mainHtmlDocument);

                if (frame == null) continue;

                IHTMLDocument2 frameDocument;
                try
                {
                    if (!WaitWhileIEBusy(frame))
                        continue;

                    if (!WaitWhileIEReadyStateNotComplete(frame))
                        continue;

                    WaitWhileFrameDocumentNotAvailable(frame);

                    frameDocument = (IHTMLDocument2)frame.Document;
                }
                finally
                {
                    // free frame
                    Marshal.ReleaseComObject(frame);
                }

                WaitWhileDocumentStateNotComplete(frameDocument);
                WaitForFramesToComplete(frameDocument);
            }
        }

        protected virtual void WaitWhileDocumentStateNotComplete(IHTMLDocument2 htmlDocument)
        {
            WaitUntil(() => DocumentReadyStateIsAvailable(htmlDocument),
                      () => "waiting for document ready state available.");

            WaitUntil(() => htmlDocument.readyState == "complete",
                      () => "waiting for document state complete. Last state was '" + htmlDocument.readyState + "'");
        }

        protected virtual void WaitWhileMainDocumentNotAvailable(IHTMLDocument2 htmlDocument)
        {
            WaitUntil(() => DocumentReadyStateIsAvailable(htmlDocument),
                      () => "waiting for main document becoming available");
        }

        protected virtual void WaitWhileFrameDocumentNotAvailable(IWebBrowser2 frame)
        {
            WaitUntil(() => DocumentReadyStateIsAvailable(GetFrameDocument(frame)),
                      () => "waiting for frame document becoming available");
        }

        protected virtual IHTMLDocument2 GetFrameDocument(IWebBrowser2 frame)
        {
            return UtilityClass.TryFuncIgnoreException(() => frame.Document as IHTMLDocument2);
        }

        protected virtual bool DocumentReadyStateIsAvailable(IHTMLDocument2 document)
        {
            // Sometimes an OutOfMemoryException or ComException occurs while accessing
            // the readystate property of IHTMLDocument2. Giving MSHTML some time
            // to do further processing seems to solve this problem.
            return UtilityClass.TryFuncIgnoreException(() =>
            {
                var readyState = document.readyState;
                return true;
            });
        }

        protected virtual bool WaitWhileIEReadyStateNotComplete(IWebBrowser2 ie)
        {
            return WaitUntilNotNull(() =>
                {
                    try
                    {
                        return ie.ReadyState == tagREADYSTATE.READYSTATE_COMPLETE ? true : (bool?)null;
                    }
                    catch
                    {
                        // If an exception occurs, it's possible that IE has been closed
                        // so we stop waiting.
                        return false;
                    }
                },
                () => "Internet Explorer state not complete");
        }

        protected virtual bool WaitWhileIEBusy(IWebBrowser2 ie)
        {
            return WaitUntilNotNull(() =>
                {
                    try
                    {
                        return ! ie.Busy ? true : (bool?)null;
                    }
                    catch
                    {
                        // If an exception occurs, it's possible that IE has been closed
                        // so we stop waiting.
                        return false;
                    }
                },
                () => "Internet Explorer busy");
        }

        /// <summary>
        /// Waits until the method returns true or false.
        /// </summary>
        /// <param name="func">The function to evaluate.</param>
        /// <param name="exceptionMessage">A function to build an exception message.</param>
        /// <returns>The last function result.</returns>
        /// <exception cref="TimeoutException">Thrown if a timeout occurs.</exception>
        protected bool WaitUntilNotNull(DoFunc<bool?> func, BuildTimeOutExceptionMessage exceptionMessage)
        {
            var result = false;
            WaitUntil(() =>
                {
                    var currentResult = func();
                    if (currentResult.HasValue)
                    {
                        result = currentResult.Value;
                        return true;
                    }
                    return false;
                }, exceptionMessage);
            return result;
        }
    }
}