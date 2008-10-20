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
using NUnit.Framework;
using Rhino.Mocks;
using WatiN.Core.Interfaces;
using WatiN.Core.InternetExplorer;
using WatiN.Core.UnitTests.AttributeConstraintTests;
using Iz=NUnit.Framework.SyntaxHelpers.Is;

namespace WatiN.Core.UnitTests.IETests
{
	[TestFixture]
	public class IEElementFinderTests : BaseWithIETests
	{
		private MockRepository mocks;
		private IElementCollection stubElementCollection;
		private DomContainer domContainer;

		public void SetUp()
		{
			mocks = new MockRepository();
			stubElementCollection = (IElementCollection) mocks.CreateMock(typeof (IElementCollection));
			domContainer = (DomContainer)mocks.DynamicMock(typeof(DomContainer));

			SetupResult.For(stubElementCollection.Elements).Return(null);

			mocks.ReplayAll();
		}

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAll();
		}

		[Test]
		public void FindFirstShouldReturnNullIfIElementCollectionIsNull()
		{
			SetUp();

			INativeElementFinder finder = new IEElementFinder("input", "text", stubElementCollection, domContainer);

			Assert.IsNull(finder.FindFirst());
		}

		[Test]
		public void FindAllShouldReturnEmptyArrayListIfIElementCollectionIsNull()
		{
			SetUp();
			
			INativeElementFinder finder = new IEElementFinder("input", "text", stubElementCollection, domContainer);

		    var all = new List<INativeElement>(finder.FindAll());
		    Assert.AreEqual(0, all.Count);
		}

		[Test]
		public void ElementFinderShouldCallConstraintResetBeforeCompare()
		{
			SetUp();
			
			var constraint = new MyTestConstraint();
			INativeElementFinder finder = new IEElementFinder("input", "text", constraint, stubElementCollection, domContainer);
			
			finder.FindFirst();

			Assert.That(constraint.CallsToReset, Iz.EqualTo(1), "Unexpected number of calls to reset");
			Assert.That(constraint.CallsToCompare, Iz.EqualTo(0), "Unexpected number of calls to compare");
		}

		// TODO: More tests to cover positive find results		[Test]

		public void ShouldNotFindElementWithIdofWrongElementType()
		{
			SetUp();
			
			Assert.That(ie.Span("divid").Exists, NUnit.Framework.SyntaxHelpers.Is.False);
			Assert.That(ie.Div("divid").Exists, NUnit.Framework.SyntaxHelpers.Is.True);
		}

		[Test]
		public void ShouldNotFindElementWithByNameWhenSearchingForId()
		{
			SetUp();
			
			Assert.That(ie.TextField("textinput1").Exists, NUnit.Framework.SyntaxHelpers.Is.False);
		}

		[Test]
		public void FindingElementByExactIdShouldBeFasterThenUsingAnyOtherConstraint()
		{
			SetUp();
			
			// Kick this code off to prevent initialization issues during measurement
			Assert.IsTrue(ie.Div("divid").Exists);

			long ticks = DateTime.Now.Ticks;
			for (int index = 0; index < 100; index++ )
				Assert.IsTrue(ie.Div("divid").Exists);
			ticks = DateTime.Now.Ticks - ticks;

			long ticksWithRegEx = DateTime.Now.Ticks;
			for (int index = 0; index < 100; index++)
				Assert.IsTrue(ie.Div(new Regex("divid")).Exists);
			ticksWithRegEx = DateTime.Now.Ticks - ticksWithRegEx;

			Console.WriteLine("Find.By exact id: " + ticks);
			Console.WriteLine("Find.By regex id: " + ticksWithRegEx);
			Assert.That(ticks, NUnit.Framework.SyntaxHelpers.Is.LessThan(ticksWithRegEx), "Lost performance gain");
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}
