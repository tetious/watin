#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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

namespace WatiN.Core.Native
{
    using System;
    using Mozilla;

    /// <summary>
    /// Defines behaviour common to most javascript controlled browsers.
    /// </summary>
    public abstract class JSBrowserBase : INativeBrowser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JSBrowserBase"/> class.
        /// </summary>
        /// <param name="clientPort">The client port.</param>
        protected JSBrowserBase(ClientPortBase clientPort)
        {
            ClientPort = clientPort;
        }

        /// <summary>
        /// Gets the client port used to communicate with the instance of FireFox.
        /// </summary>
        /// <value>The client port.</value>
        public ClientPortBase ClientPort { get; private set; }

        /// <summary>
        /// Gets the name of the browser variable.
        /// </summary>
        /// <value>The name of the browser variable.</value>
        public string BrowserVariableName
        {
            get { return ClientPort.BrowserVariableName;  }
        }

        public string PromptName
        {
          get { return ClientPort.PromptName; }
        }

        /// <inheritdoc />
        public IntPtr hWnd
        {
            get { return ClientPort.Process.MainWindowHandle; }
        }

        public abstract INativeDocument NativeDocument { get; }

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
            Reopen(null);
        }


        /// <inheritdoc />
        public void NavigateTo(Uri url)
        {
            LoadUri(url, true);
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
        private void Reload(bool forceGet)
        {
            ClientPort.Write("{0}.location.reload({1});", FireFoxClientPort.WindowVariableName, forceGet.ToString().ToLower());
        }

        /// <inheritdoc />
        public void Refresh()
        {
            Reload(false);
        }

        /// <summary>
        /// Closes the browser.
        /// </summary>
        public void Close()
        {
            ClientPort.Write("{0}.close()", FireFoxClientPort.WindowVariableName);
        }

        public bool IsLoading()
        {
            bool loading;
            switch (ClientPort.JavaScriptEngine)
            {
                case JavaScriptEngineType.WebKit:
                    loading = ClientPort.WriteAndReadAsBool("{0}.readyState != 'complete';", ClientPort.DocumentVariableName);
                    ClientPort.WriteAndRead("{0}.readyState;", ClientPort.DocumentVariableName);
                    ClientPort.WriteAndRead("window.location.href");
                    break;
                case JavaScriptEngineType.Mozilla:
                    ClientPort.WriteAndRead(string.Format("{0}.home();true;", PromptName));
                    loading = ClientPort.WriteAndReadAsBool("{0}.webProgress.busyFlags!=0;", BrowserVariableName);
                    ClientPort.WriteAndRead(string.Format("if(typeof(w0)!=='undefined'){0}.enter(w0.content);true;", PromptName));
                    break;
                default:
                    throw new NotImplementedException();
            }

            return loading;
        }

        protected void Reopen(Uri url)
        {
            ClientPort.Dispose();
            ClientPort.Connect(url == null ? string.Empty : url.ToString());
        }

        /// <summary>
        /// Load a URL into the document. see: http://developer.mozilla.org/en/docs/XUL:browser#m-loadURI
        /// </summary>
        /// <param name="url">The URL to laod.</param>
        /// <param name="waitForComplete">If false, makes to execution of LoadUri asynchronous.</param>
        protected abstract void LoadUri(Uri url, bool waitForComplete);

        public int WindowCount
        {
            get 
            { 
              ClientPort.WriteAndRead(string.Format("{0}.home();true;", ClientPort.PromptName));
              var windowCount = ClientPort.WriteAndReadAsInt(string.Format("{0}.getWindows().length", ClientPort.PromptName));
              ClientPort.WriteAndRead(string.Format("if(typeof(w0)!=='undefined'){0}.enter(w0.content);true;", ClientPort.PromptName));              

              return windowCount;
            }
        }

        private bool Navigate(string action)
        {
            var ticks = Guid.NewGuid().ToString();
            ClientPort.Write("{0}.WatiNGoBackCheck='{1}';",ClientPort.DocumentVariableName, ticks);
            ClientPort.WriteAndRead(string.Format("{0}.home();true;", ClientPort.PromptName));
            ClientPort.Write("{0}.{1}();", BrowserVariableName, action);

            ClientPort.InitializeDocument();

            return ClientPort.WriteAndReadAsBool("{0}.WatiNGoBackCheck!='{1}';",ClientPort.DocumentVariableName, ticks);
        }
    }
}