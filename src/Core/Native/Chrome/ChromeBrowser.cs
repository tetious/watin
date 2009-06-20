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

namespace WatiN.Core.Native.Chrome
{
    using System;
    using System.Threading;

    /// <summary>
    /// Native driver the communicates with the Chrome browser using a
    /// telnet session <see cref="ClientPortBase"/>.
    /// </summary>
    public class ChromeBrowser : JSBrowserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeBrowser"/> class.
        /// </summary>
        /// <param name="clientPort">The client port.</param>
        public ChromeBrowser(ClientPortBase clientPort) : base(clientPort)
        {
        }

        public override INativeDocument NativeDocument
        {
            get { return new ChromeDocument(ClientPort); }
        }

        /// <summary>
        /// Load a URL into the document.
        /// </summary>
        /// <param name="url">The URL to laod.</param>
        /// <param name="waitForComplete">If false, makes to execution of LoadUri asynchronous.</param>
        protected override void LoadUri(Uri url, bool waitForComplete)
        {
            if (!url.IsFile)
            {
                var command = string.Format("window.location.href='{0}\';", url.AbsoluteUri);
                if (!waitForComplete)
                {
                    command = JSUtils.WrapCommandInTimer(command);
                }

                this.ClientPort.Write(command);
                this.ReAttachToTab(url);
            }
            else
            {
                // Need to reopen Chrome to go to a file based url.
                // #TODO if the current url is a file url do we still need to??
                this.Reopen(url);
            }
        }
        
        /// <summary>
        /// Reattaches to the first tab. This is required every time the document
        /// </summary>
        private void ReAttachToTab(Uri url)
        {            
            do
            {
                this.ClientPort.WriteAndRead("exit", true, true);

                // #Hack instead of sleeping create function which exit's and debugs until the current page 
                // is not about:blank.             
                Thread.Sleep(100);
                this.ClientPort.WriteAndRead("debug()", true, true);
            } 
            while (this.ClientPort.LastResponseRaw.Contains("attached to about:blank") && url.AbsoluteUri != "about:blank");
        }
    }
}