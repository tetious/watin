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

using System;

namespace WatiN.Core.Native.Mozilla
{
    /// <summary>
    /// Wrapper around the XUL:browser class, see: http://developer.mozilla.org/en/docs/XUL:browser
    /// </summary>
    public class FFBrowser : INativeBrowser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FFBrowser"/> class.
        /// </summary>
        /// <param name="clientPort">The client port.</param>
        public FFBrowser(FireFoxClientPort clientPort)
        {
            ClientPort = clientPort;
        }

        /// <summary>
        /// Gets the client port used to communicate with the instance of FireFox.
        /// </summary>
        /// <value>The client port.</value>
        public FireFoxClientPort ClientPort { get; private set; }

        public string BrowserVariableName
        {
            get { return FireFoxClientPort.BrowserVariableName;  }
        }

        public IntPtr hWnd
        {
            get { return ClientPort.Process.MainWindowHandle; }
        }

        /// <inheritdoc />
        public bool GoBack()
        {
            return Navigate("goBack");
        }

        /// <inheritdoc />
        public bool GoForward()
        {
            return Navigate("goForward");
        }

        /// <inheritdoc />
        public void Reopen()
        {
            ClientPort.Dispose();
            ClientPort.Connect(string.Empty);
        }

        private bool Navigate(string action)
        {
            var ticks = DateTime.Now.Ticks;
            ClientPort.Write("window.document.WatiNGoBackCheck={0};", ticks);
            ClientPort.Write("{0}.{1}();", BrowserVariableName, action);
            return ClientPort.WriteAndReadAsBool("window.document.WatiNGoBackCheck!={0};", ticks);
        }

        /// <inheritdoc />
        public void NavigateTo(Uri url)
        {
            LoadUri(url, true);
        }

        /// <summary>
        /// Load a URL into the document. see: http://developer.mozilla.org/en/docs/XUL:browser#m-loadURI
        /// </summary>
        /// <param name="url">The URL to laod.</param>
        /// <param name="waitForComplete">If false, makes to execution of LoadUri asynchronous.</param>
        private void LoadUri(Uri url, bool waitForComplete)
        {
            var command = string.Format("{0}.loadURI(\"{1}\");", BrowserVariableName, url.AbsoluteUri);
            if (!waitForComplete)
            {
                command = FFUtils.WrapCommandInTimer(command);
            }

            ClientPort.Write(command);
        }

        /// <inheritdoc />
        public void NavigateToNoWait(Uri url)
        {
            LoadUri(url, false);
        }

        /// <summary>
        /// Reloads this instance.
        /// </summary>
        /// <param name="forceGet">When it is <c>true</c>, causes the page to always be reloaded from the server. 
        /// If it is <c>false</c>, the browser may reload the page from its cache.</param>
        public void Reload(bool forceGet)
        {
            ClientPort.Write("{0}.location.reload({1});", FireFoxClientPort.WindowVariableName, forceGet.ToString().ToLower());
        }

        /// <inheritdoc />
        public void Refresh()
        {
            Reload(false);
        }

        public bool IsLoading()
        {
            return ClientPort.WriteAndReadAsBool("{0}.webProgress.isLoadingDocument;", BrowserVariableName);
        }
    }
}