using System;
using System.Collections.Generic;
using System.Threading;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core.Mozilla
{
    /// <summary>
    /// Wrapper around the XUL:browser class, see: http://developer.mozilla.org/en/docs/XUL:browser
    /// </summary>
    public class FFBrowser : INativeBrowser
    {
        private readonly DomContainer _domContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FFBrowser"/> class.
        /// </summary>
        /// <param name="clientPort">The client port.</param>
        public FFBrowser(FireFoxClientPort clientPort, DomContainer domContainer)
        {
            _domContainer = domContainer;
            ClientPort = clientPort;
        }

        /// <summary>
        /// Gets the client port used to communicate with the instance of FireFox.
        /// </summary>
        /// <value>The client port.</value>
        public FireFoxClientPort ClientPort { get; private set; }

        public IntPtr Handle
        {
            get { return ClientPort.Process.MainWindowHandle; }
        }

        /// <summary>
        /// Navigates to the previous page in the browser history.
        /// </summary>
        public void Back()
        {
            ClientPort.Write("{0}.goBack();", FireFoxClientPort.BrowserVariableName);
        }

        /// <summary>
        /// Navigates to the next back in the browser history.
        /// </summary>
        public void Forward()
        {
            ClientPort.Write("{0}.goForward();", FireFoxClientPort.BrowserVariableName);
        }

        /// <summary>
        /// Load a URL into the document. see: http://developer.mozilla.org/en/docs/XUL:browser#m-loadURI
        /// </summary>
        /// <param name="url">The URL to laod.</param>
        public void LoadUri(Uri url)
        {
            ClientPort.Write("{0}.loadURI(\"{1}\");", FireFoxClientPort.BrowserVariableName, url);
        }

        /// <summary>
        /// Reloads this instance.
        /// </summary>
        public void Reload()
        {
            ClientPort.Write("{0}.reload();", FireFoxClientPort.BrowserVariableName);
        }

        /// <summary>
        /// Waits until the document is loaded
        /// </summary>
        public void WaitForComplete()
        {
            WaitForComplete(ClientPort);
        }

        /// <summary>
        /// Waits until the document, associated with the <param name="clientPort" /> is loaded.
        /// </summary>
        internal static void WaitForComplete(FireFoxClientPort clientPort)
        {
            clientPort.Write("{0}.webProgress.isLoadingDocument;", FireFoxClientPort.BrowserVariableName);

            while (clientPort.LastResponse == "true")
            {
                Thread.Sleep(200);
                clientPort.Write("{0}.webProgress.isLoadingDocument;", FireFoxClientPort.BrowserVariableName);
            }

            clientPort.InitializeDocument();
        }

        public INativeElementFinder CreateElementFinder(List<ElementTag> tags, BaseConstraint baseConstraint, IElementCollection elements)
        {
            return new FFElementFinder(tags, baseConstraint, elements, _domContainer, ClientPort);
        }

        public INativeElementFinder CreateElementFinder(List<ElementTag> tags, IElementCollection elements)
        {
            return new FFElementFinder(tags, elements, _domContainer, ClientPort);
        }

        public INativeElementFinder CreateElementFinder(string tagname, string inputtypesString, BaseConstraint baseConstraint, IElementCollection elements)
        {
            return new FFElementFinder(tagname, inputtypesString, baseConstraint, elements, _domContainer, ClientPort);
        }

        public INativeElement CreateElement(object element)
        {
            return new FFElement(element, ClientPort);
        }

        public INativeDocument CreateDocument(object document)
        {
            throw new System.NotImplementedException();
        }
    }
}