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
                Assert.That(Browser.Exists<IE>(Find.By("hwnd", hwnd)), Is.False, "Expected no IE");
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
                Assert.That(Browser.Exists<IE>(Find.By("hwnd", hwnd)), Is.True, "Expected IE");
            }
        }
    }
}