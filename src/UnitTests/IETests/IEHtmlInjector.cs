using System.Threading;
using mshtml;

namespace WatiN.Core.UnitTests.IETests
{
    internal class IEHtmlInjector
    {
        private readonly string _html;
        private readonly int _numberOfSecondsToWaitBeforeInjection;
        private readonly Document _document;

        public IEHtmlInjector(Document document, string html, int numberOfSecondsToWaitBeforeInjection)
        {
            _document = document;
            _html = html;
            _numberOfSecondsToWaitBeforeInjection = numberOfSecondsToWaitBeforeInjection;
        }

        public void Inject()
        {
            Thread.Sleep(_numberOfSecondsToWaitBeforeInjection * 1000);

            try
            {
                ((IHTMLDocument2)_document.NativeDocument.Object).writeln(_html);
            }
            catch { }
        }

        /// <summary>
        /// Starts a new thread and injects the html into the document after numberOfSecondsToWaitBeforeInjection.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="html"></param>
        /// <param name="numberOfSecondsToWaitBeforeInjection"></param>
        public static void Start(Document document, string html, int numberOfSecondsToWaitBeforeInjection)
        {
            var htmlInjector = new IEHtmlInjector(document, html, numberOfSecondsToWaitBeforeInjection);

            ThreadStart start = htmlInjector.Inject;
            var thread = new Thread(start);
            thread.Start();
        }

    }
}