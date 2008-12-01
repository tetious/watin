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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;
using WatiN.Core.InternetExplorer;
using WatiN.Core.UnitTests.AttributeConstraintTests;

namespace WatiN.Core.UnitTests.IETests
{
	[TestFixture]
	public class IEElementFinderTests : BaseWithIETests
	{
		private Mock<IElementCollection> stubElementCollection;
		private Mock<DomContainer> domContainerMock;

		public void SetUp()
		{
			stubElementCollection = new Mock<IElementCollection>();
			domContainerMock = new Mock<DomContainer>();

			stubElementCollection.Expect(elements => elements.Elements).Returns(null);
		}

		[TearDown]
		public void TearDown()
		{
			domContainerMock.VerifyAll();
		}

		[Test]
		public void FindFirstShouldReturnNullIfIElementCollectionIsNull()
		{
			SetUp();

			INativeElementFinder finder = new IEElementFinder("input", "text", stubElementCollection.Object, domContainerMock.Object);

			Assert.IsNull(finder.FindFirst());
		}

		[Test]
		public void FindAllShouldReturnEmptyArrayListIfIElementCollectionIsNull()
		{
			SetUp();
			
			INativeElementFinder finder = new IEElementFinder("input", "text", stubElementCollection.Object, domContainerMock.Object);

		    var all = new List<INativeElement>(finder.FindAll());
		    Assert.AreEqual(0, all.Count);
		}

		[Test]
		public void ElementFinderShouldCallConstraintResetBeforeCompare()
		{
			SetUp();
			
			var constraint = new MyTestConstraint();
			INativeElementFinder finder = new IEElementFinder("input", "text", constraint, stubElementCollection.Object, domContainerMock.Object);
			
			finder.FindFirst();

			Assert.That(constraint.CallsToReset, Is.EqualTo(1), "Unexpected number of calls to reset");
			Assert.That(constraint.CallsToCompare, Is.EqualTo(0), "Unexpected number of calls to compare");
		}

		// TODO: More tests to cover positive find results		[Test]

		public void ShouldNotFindElementWithIdofWrongElementType()
		{
			SetUp();
			
			Assert.That(ie.Span("divid").Exists, Is.False);
			Assert.That(ie.Div("divid").Exists, Is.True);
		}

		[Test]
		public void ShouldNotFindElementWithByNameWhenSearchingForId()
		{
			SetUp();
			
			Assert.That(ie.TextField("textinput1").Exists, Is.False);
		}

		[Test]
		public void FindingElementByExactIdShouldBeFasterThenUsingAnyOtherConstraint()
		{
			SetUp();
			
			// Kick this code off to exclude initialization time during measurement
			Assert.IsTrue(ie.Div("divid").Exists);

			var ticksByExactId = GetTicks(Find.ById("divid"));
            var ticksByRegExId = GetTicks(Find.ById(new Regex("divid")));

            Console.WriteLine("Find.By exact id: " + ticksByExactId);
            Console.WriteLine("Find.By regex id: " + ticksByRegExId);
			
            Assert.That(ticksByExactId, Is.LessThan(ticksByRegExId), "Lost performance gain");
		}

	    private long GetTicks(BaseConstraint findBy)
	    {
	        var ticks = DateTime.Now.Ticks;
	        for (var index = 0; index < 100; index++ ) Assert.IsTrue(ie.Div(findBy).Exists);
	        ticks = DateTime.Now.Ticks - ticks;
	        return ticks;
	    }

	    public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}
