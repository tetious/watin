#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ParaTests : BaseElementsTests
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
			Element element = ie.Element("links");
			Para para = new Para(element);
			Assert.AreEqual("links", para.Id);
		}

		[Test]
		public void ParaExists()
		{
			Assert.IsTrue(ie.Para("links").Exists);
			Assert.IsTrue(ie.Para(new Regex("links")).Exists);
			Assert.IsFalse(ie.Para("nonexistinglinks").Exists);
		}

		[Test]
		public void ParaTest()
		{
			Para para = ie.Para("links");

			Assert.IsInstanceOfType(typeof (ElementsContainer), para);

			Assert.IsNotNull(para);
			Assert.AreEqual("links", para.Id);
		}

		[Test]
		public void Paras()
		{
			const int expectedParasCount = 4;
			Assert.AreEqual(expectedParasCount, ie.Paras.Length, "Unexpected number of Paras");

			// Collection.Length
			ParaCollection formParas = ie.Paras;

			// Collection items by index
			Assert.IsNull(ie.Paras[0].Id);
			Assert.AreEqual("links", ie.Paras[1].Id);
			Assert.IsNull(ie.Paras[2].Id);
			Assert.IsNull(ie.Paras[3].Id);

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