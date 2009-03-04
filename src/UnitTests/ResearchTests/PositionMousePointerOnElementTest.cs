using System;
using System.Threading;
using mshtml;
using NUnit.Framework;
using WatiN.Core.Native.InternetExplorer;

namespace WatiN.Core.UnitTests.ResearchTests
{
    [TestFixture]
    public class PositionMousePointerOnElementTest : BaseWithBrowserTests
    {
        [Test, Ignore("Work in progress")] // Category("InternetConnectionNeeded")]
        public void PositionMousePointerInMiddleOfElement()
        {
            Ie.GoTo(GoogleUrl);

            var button = Ie.Button(Find.ByName("btnG"));
            PositionMousePointerInMiddleOfElement(button, Ie);
            button.Flash();
            MouseMove(50, 50, true);
            Thread.Sleep(2000);
        }

        [Test, Ignore("Doesn't work yet")]
        public void PositionMousePointerInMiddleOfElementInFrame()
        {
            Settings.MakeNewIeInstanceVisible = true;
            Settings.HighLightElement = true;

            using (var ie = new IE(FramesetURI))
            {
                var button = ie.Frames[1].Links[0];
                PositionMousePointerInMiddleOfElement(button, ie);
                button.Flash();
                MouseMove(50, 50, true);
                Thread.Sleep(2000);
            }
        }

        private static void PositionMousePointerInMiddleOfElement(Element button, Document ie)
        {
            var left = position(button, "Left");
            var width = int.Parse(button.GetAttributeValue("clientWidth"));
            var top = position(button, "Top");
            var height = int.Parse(button.GetAttributeValue("clientHeight"));

            var window = (IHTMLWindow3)((IEDocument)ie.NativeDocument).HtmlDocument.parentWindow;

            left = left + window.screenLeft;
            top = top + window.screenTop;

            var currentPt = new System.Drawing.Point(left + (width / 2), top + (height / 2));
            System.Windows.Forms.Cursor.Position = currentPt;
        }

        private static int position(Element element, string attributename)
        {
            var ieElement = (IEElement)element.NativeElement;

            var pos = 0;
            var offsetParent = ieElement.HtmlElement.offsetParent;
            if (offsetParent != null)
            {
                var domContainer = element.DomContainer;
                pos = position(new Element(domContainer, new IEElement(offsetParent)), attributename);
            }

            if (Comparers.StringComparer.AreEqual(element.TagName, "table", true))
            {
                pos = pos + int.Parse(element.GetAttributeValue("client" + attributename));
            }
            return pos + int.Parse(element.GetAttributeValue("offset" + attributename));
        }

        private static void MouseMove(int X, int Y, bool Relative)
        {
            var currentPt = System.Windows.Forms.Cursor.Position;
            if (Relative)
            {
                currentPt.X += X;
                currentPt.Y += Y;
            }
            else
            {
                currentPt.X = X;
                currentPt.Y = Y;
            }

            System.Windows.Forms.Cursor.Position = currentPt;
        }

        public override Uri TestPageUri
        {
            get { return AboutBlank; }
        }
    }
}
