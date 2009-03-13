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
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.Native.InternetExplorer
{
    public class WaitForComplete : WaitForCompleteBase
    {
        protected DomContainer _domContainer;

        /// <summary>
        /// Waits until the given <paramref name="domContainer"/> is ready loading the webpage. It will timeout after
        /// <seealso cref="Settings.WaitForCompleteTimeOut"/> seconds.
        /// </summary>
        /// <param name="domContainer">The page to wait for in this domcontainer</param>
        public WaitForComplete(DomContainer domContainer) : this(domContainer, Settings.WaitForCompleteTimeOut) { }

        /// Waits until the given <paramref name="domContainer"/> is ready loading the webpage. It will timeout after
        /// <paramref name="waitForCompleteTimeOut"> seconds.</paramref>
        /// <param name="domContainer">The page to wait for in this domcontainer</param>
        /// <param name="waitForCompleteTimeOut">Time to wait in seconds</param>
        public WaitForComplete(DomContainer domContainer, int waitForCompleteTimeOut) : base(waitForCompleteTimeOut)
        {
            _domContainer = domContainer;
        }

        protected virtual void WaitForFramesToComplete(IHTMLDocument2 maindocument)
        {
            var mainHtmlDocument = (HTMLDocument) maindocument;

            var framesCount = FrameCountProcessor.GetFrameCountFromHTMLDocument(mainHtmlDocument);

            for (var i = 0; i != framesCount; ++i)
            {
                var frame = FrameByIndexProcessor.GetFrameFromHTMLDocument(i, mainHtmlDocument);

                if (frame == null) continue;
			    
                IHTMLDocument2 document;

                try
                {
                    WaitWhileIEBusy(frame);
                    WaitWhileIEReadyStateNotComplete(frame);
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

        protected virtual void WaitWhileDocumentStateNotComplete(IHTMLDocument2 htmlDocument)
        {
            WaitUntil(() => DocumentReadyStateIsAvailable(htmlDocument),
                      () => "waiting for document ready state available.");

            WaitUntil(() => htmlDocument.readyState == "complete",
                      () => "waiting for document state complete. Last state was '" + htmlDocument.readyState + "'");
        }

        protected virtual void WaitWhileMainDocumentNotAvailable(DomContainer domContainer)
        {
            WaitUntil(() => DocumentReadyStateIsAvailable(GetDomContainerDocument(domContainer)),
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

        protected virtual IHTMLDocument2 GetDomContainerDocument(DomContainer domContainer)
        {
            return UtilityClass.TryFuncIgnoreException(() => ((IEDocument)domContainer.NativeDocument).HtmlDocument);
        }

        protected virtual bool DocumentReadyStateIsAvailable(IHTMLDocument2 document)
        {
            if (document == null) return false;

            // Sometimes an OutOfMemoryException or ComException occurs while accessing
            // the readystate property of IHTMLDocument2. Giving MSHTML some time
            // to do further processing seems to solve this problem.
            return UtilityClass.TryFuncIgnoreException(() =>
                                                           {
                                                               var readyState = document.readyState;
                                                               return true;
                                                           });
        }

        protected virtual void WaitWhileIEReadyStateNotComplete(IWebBrowser2 ie)
        {
            WaitUntil(() => IEReadyStateIsComplete(ie),
                      () => "Internet Explorer state not complete");
        }

        protected virtual bool IEReadyStateIsComplete(IWebBrowser2 ie)
        {
            return UtilityClass.TryFuncIgnoreException(() => ie.ReadyState == tagREADYSTATE.READYSTATE_COMPLETE);
        }

        protected virtual void WaitWhileIEBusy(IWebBrowser2 ie)
        {
            WaitUntil(() => IENotBusy(ie),
                      () => "Internet Explorer busy");
        }

        protected virtual bool IENotBusy(IWebBrowser2 ie)
        {
            return UtilityClass.TryFuncIgnoreException(() => !ie.Busy);
        }

        protected override void WaitForCompleteOrTimeout()
        {
            WaitWhileMainDocumentNotAvailable(_domContainer);
            WaitWhileDocumentStateNotComplete(GetDomContainerDocument(_domContainer));
            WaitForFramesToComplete(GetDomContainerDocument(_domContainer));
        }
    }
}