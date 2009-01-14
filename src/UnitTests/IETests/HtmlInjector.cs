using System.Threading;
using mshtml;

namespace WatiN.Core.UnitTests.IETests
{
    internal class HtmlInjector
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