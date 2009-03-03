using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.DialogHandlers;

namespace WatiN.Core.UnitTests.DialogHandlerTests
{
    /// <summary>
    ///This is a NUnit TestFixture class
    ///</summary>
    [TestFixture]
    public class CloseIEDialogHandlerTests
    {
        [Test]
        public void ShouldCloseBrowser()
        {
            using (var ie = new IE())
            {
                var hwnd = ie.hWnd.ToString();

                // GIVEN
                var command = "window.close();";
                using (new UseDialogOnce(ie.DialogWatcher, new CloseIEDialogHandler(true)))
                {
                    // WHEN
                    ie.Eval(command);
                }

                // THEN
                Assert.That(IE.Exists(Find.By("hwnd", hwnd)), Is.False, "Expected no IE");
            }
        }

        [Test]
        public void ShouldCancelCloseBrowser()
        {
            using (var ie = new IE())
            {
                var hwnd = ie.hWnd.ToString();

                // GIVEN
                var command = "window.close();";
                using (new UseDialogOnce(ie.DialogWatcher, new CloseIEDialogHandler(false)))
                {
                    // WHEN
                    ie.Eval(command);
                }

                // THEN
                Assert.That(IE.Exists(Find.By("hwnd", hwnd)), Is.True, "Expected IE");
            }
        }
    }
}