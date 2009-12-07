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

using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.DialogHandlers;

namespace WatiN.Core.UnitTests.DialogHandlerTests
{
    [TestFixture]
    public class PromptDialogHandlerTests
    {
        /// <summary>
        /// Because of popup blockers this test will almost always fail. 
        /// To run the test succesfully: 1 - Open the TestEvents.html page in IE.
        /// 2 - Click on the Show prompt button
        /// 3 - Handle the warning on top of the IE window, select to temporarily allow the scripts to run on this page
        /// 4 - Now you can start this unit test (remove Ignore attribute first)
        /// </summary>
        [Test, Ignore("IE popup blocker will cause this test to fail. Read method documentation to run this test manually")]
        public void PromptDialogHandler()
        {
            var ie = Browser.AttachTo<IE>(Find.ByUrl(new Regex("TestEvents.html$")));

            var handler = new PromptDialogHandler("Hello");
            using (new UseDialogOnce(ie.DialogWatcher, handler))
            {
                ie.Button("showPrompt").Click();
            }
            Assert.That(ie.TextField("promptResult").Value, Is.EqualTo("Hello"), "input did not work. IE might be blocking the prompt dialog");
        }
    }
}
