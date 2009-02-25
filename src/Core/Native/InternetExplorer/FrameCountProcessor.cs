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

using mshtml;
using SHDocVw;
using WatiN.Core.Native.Windows;

namespace WatiN.Core.Native.InternetExplorer
{
    internal class FrameCountProcessor : IWebBrowser2Processor
    {
        private readonly HTMLDocument htmlDocument;

        public FrameCountProcessor(HTMLDocument htmlDocument)
        {
            this.htmlDocument = htmlDocument;
        }

        public int FramesCount { get; private set; }

        public HTMLDocument HTMLDocument()
        {
            return htmlDocument;
        }

        public void Process(IWebBrowser2 webBrowser2)
        {
            FramesCount++;
        }

        public bool Continue()
        {
            return true;
        }

        internal static int GetFrameCountFromHTMLDocument(HTMLDocument htmlDocument)
        {
            var processor = new FrameCountProcessor(htmlDocument);

            IEUtils.EnumIWebBrowser2Interfaces(processor);

            return processor.FramesCount;
        }

    }
}