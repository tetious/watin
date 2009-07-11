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

using System;
using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class AreaTests : BaseWithBrowserTests
    {
        [Test]
        public void AreaElementTags()
        {
            var elementTags = ElementFactory.GetElementTags<Area>();
            Assert.AreEqual(1, elementTags.Count, "1 elementtags expected");
            Assert.AreEqual("area", elementTags[0].TagName);
            Assert.AreEqual(null, elementTags[0].InputType);
        }

        [Test]
        public void AreaExists()
        {
            ExecuteTest(browser =>
                            {
                                Assert.IsTrue(browser.Area("Area1").Exists);
                                Assert.IsTrue(browser.Area(new Regex("Area1")).Exists);
                                Assert.IsFalse(browser.Area("noneexistingArea1id").Exists);
                            });
        }

        [Test]
        public void AreaTest()
        {
            ExecuteTest(browser =>
                            {
                                var area1 = browser.Area("Area1");

                                Assert.AreEqual("Area1", area1.Id, "Found wrong area");

                                Assert.AreEqual("WatiN", area1.Alt, "Alt text was incorrect");
                                Assert.IsTrue(area1.Url.EndsWith("main.html"), "Url was incorrect");
                                Assert.AreEqual("0,0,110,45", area1.Coords, "Coords was incorrect");
                                Assert.AreEqual("rect", area1.Shape.ToLower(), "Shape was incorrect");
                            });
        }

        [Test]
        public void Areas()
        {
            ExecuteTest(browser =>
                            {
                                // Collection items by index
                                var areas = browser.Areas;
                                Assert.AreEqual(2, areas.Count, "Unexpected number of areas");
                                Assert.AreEqual("Area1", areas[0].Id);
                                Assert.AreEqual("Area2", areas[1].Id);

                                // Collection iteration and comparing the result with Enumerator
                                IEnumerable areaEnumerable = areas;
                                var areaEnumerator = areaEnumerable.GetEnumerator();

                                var count = 0;
                                foreach (Area area in areaEnumerable)
                                {
                                    areaEnumerator.MoveNext();
                                    var enumArea = areaEnumerator.Current;

                                    Assert.IsInstanceOfType(area.GetType(), enumArea, "Types are not the same");
                                    Assert.AreEqual(area.OuterHtml, ((Area) enumArea).OuterHtml, "foreach and IEnumator don't act the same.");
                                    ++count;
                                }

                                Assert.IsFalse(areaEnumerator.MoveNext(), "Expected last item");
                                Assert.AreEqual(2, count);
                            });
        }

        public override Uri TestPageUri
        {
            get { return (ImagesURI); }
        }
    }
}