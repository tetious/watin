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

using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.ResearchTests
{
    [TestFixture]
    public class JScriptElementArrayEnumaratorTests : BaseWithBrowserTests
    {
        [Test]
        public void Should_itterate_elements()
        {
            Ie.RunScript("document.result = toArray();");

            var elements = new JScriptElementArrayEnumerator((IEDocument)Ie.NativeDocument, "result");

            Assert.That(elements.Count(), Is.EqualTo(2));
            Assert.That(elements.First().GetAttributeValue("Id"), Is.EqualTo("popupid"));
            Assert.That(elements.Last().GetAttributeValue("Id"), Is.EqualTo("Select1"));
        }

        [Test]
        public void Should_not_crash_if_field_doesnt_exist()
        {
            // GIVEN
            const string iDontExist = "I_dont_exist";

            // WHEN
            var elements = new JScriptElementArrayEnumerator((IEDocument) Ie.NativeDocument, iDontExist);

            // THEN
            Assert.That(elements.Count(), Is.EqualTo(0));
        }

        public override Uri TestPageUri
        {
            get { return MainURI; }
        }
    }
}
