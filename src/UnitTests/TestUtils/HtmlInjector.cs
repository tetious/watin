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

namespace WatiN.Core.UnitTests.TestUtils
{
    public class HtmlInjector
    {
        private readonly string _html;
        private readonly int _numberOfSecondsToWaitBeforeInjection;
        private readonly Document _document;

        public HtmlInjector(Document document, string html, int numberOfSecondsToWaitBeforeInjection)
        {
            _document = document;
            _html = html;
            _numberOfSecondsToWaitBeforeInjection = numberOfSecondsToWaitBeforeInjection;
        }

        public void Inject()
        {
            var sleepTime = _numberOfSecondsToWaitBeforeInjection*1000;
            var documentVariableName = _document.NativeDocument.JavaScriptVariableName;

            var script = "window.setTimeout(function()" +
                         "{" +
                         documentVariableName +".body.innerHTML = '" + _html +"';" +
                         "}, " + sleepTime +");";

            _document.RunScript(script);
        }

        /// <summary>
        /// Starts a new thread and injects the html into the document after numberOfSecondsToWaitBeforeInjection.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="html"></param>
        /// <param name="numberOfSecondsToWaitBeforeInjection"></param>
        public static void Start(Document document, string html, int numberOfSecondsToWaitBeforeInjection)
        {
            var htmlInjector = new HtmlInjector(document, html, numberOfSecondsToWaitBeforeInjection);
            htmlInjector.Inject();
        }
    }
}