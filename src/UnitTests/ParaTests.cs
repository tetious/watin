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
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ParaTests : BaseWithBrowserTests
	{
		[Test]
		public void ParaElementTags()
		{
			Assert.AreEqual(1, Para.ElementTags.Count, "1 elementtags expected");
			Assert.AreEqual("p", ((ElementTag) Para.ElementTags[0]).TagName);
		}

		[Test]
		public void CreateParaFromElement()
		{
			Element element = Ie.Element("links");
			Para para = new Para(element);
			Assert.AreEqual("links", para.Id);
		}

		[Test]
		public void ParaExists()
		{
			Assert.IsTrue(Ie.Para("links").Exists);
			Assert.IsTrue(Ie.Para(new Regex("links")).Exists);
			Assert.IsFalse(Ie.Para("nonexistinglinks").Exists);
		}

		[Test]
		public void ParaTest()
		{
			Para para = Ie.Para("links");

			Assert.IsInstanceOfType(typeof (IElementsContainer), para);
            Assert.IsInstanceOfType(typeof (ElementsContainer<Para>), para);

			Assert.IsNotNull(para);
			Assert.AreEqual("links", para.Id);
		}

		[Test]
		public void Paras()
		{
			const int expectedParasCount = 4;
			Assert.AreEqual(expectedParasCount, Ie.Paras.Length, "Unexpected number of Paras");

			// Collection.Length
			ParaCollection formParas = Ie.Paras;

			// Collection items by index
			Assert.IsNull(Ie.Paras[0].Id);
			Assert.AreEqual("links", Ie.Paras[1].Id);
			Assert.IsNull(Ie.Paras[2].Id);
			Assert.IsNull(Ie.Paras[3].Id);

			IEnumerable ParaEnumerable = formParas;
			IEnumerator ParaEnumerator = ParaEnumerable.GetEnumerator();

			// Collection iteration and comparing the result with Enumerator
			int count = 0;
			foreach (Para inputPara in formParas)
			{
				ParaEnumerator.MoveNext();
				object enumPara = ParaEnumerator.Current;

				Assert.IsInstanceOfType(inputPara.GetType(), enumPara, "Types are not the same");
				Assert.AreEqual(inputPara.OuterHtml, ((Para) enumPara).OuterHtml, "foreach and IEnumator don't act the same.");
				++count;
			}

			Assert.IsFalse(ParaEnumerator.MoveNext(), "Expected last item");
			Assert.AreEqual(expectedParasCount, count);
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}