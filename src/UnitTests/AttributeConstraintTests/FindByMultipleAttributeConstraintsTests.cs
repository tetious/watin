#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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
using NUnit.Framework;
using Rhino.Mocks;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class FindByMultipleAttributeConstraintsTests
	{
		private MockRepository mocks;
		private IAttributeBag mockAttributeBag;

		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
			mockAttributeBag = (IAttributeBag) mocks.CreateMock(typeof (IAttributeBag));
		}

		[Test]
		public void AndTrue()
		{
			AttributeConstraint findBy = Find.ByName("X").And(Find.ByValue("Cancel"));

			Expect.Call(mockAttributeBag.GetValue("name")).Return("X");
			Expect.Call(mockAttributeBag.GetValue("value")).Return("Cancel");

			mocks.ReplayAll();

			Assert.IsTrue(findBy.Compare(mockAttributeBag));

			mocks.VerifyAll();
		}

		[Test]
		public void AndFalseFirstSoSecondPartShouldNotBeEvaluated()
		{
			AttributeConstraint findBy = Find.ByName("X").And(Find.ByValue("Cancel"));

			Expect.Call(mockAttributeBag.GetValue("name")).Return("Y");

			mocks.ReplayAll();

			Assert.IsFalse(findBy.Compare(mockAttributeBag));

			mocks.VerifyAll();
		}

		[Test]
		public void AndFalseSecond()
		{
			AttributeConstraint findBy = Find.ByName("X").And(Find.ByValue("Cancel"));

			MockAttributeBag attributeBag = new MockAttributeBag("name", "X");
			attributeBag.Add("value", "OK");
			Assert.IsFalse(findBy.Compare(attributeBag));
		}

		[Test]
		public void OrFirstTrue()
		{
			AttributeConstraint findBy = Find.ByName("X").Or(Find.ByName("Y"));
			MockAttributeBag attributeBag = new MockAttributeBag("name", "X");
			Assert.IsTrue(findBy.Compare(attributeBag));
		}

		[Test]
		public void OrSecondTrue()
		{
			AttributeConstraint findBy = Find.ByName("X").Or(Find.ByName("Y"));
			MockAttributeBag attributeBag = new MockAttributeBag("name", "Y");
			Assert.IsTrue(findBy.Compare(attributeBag));
		}

		[Test]
		public void OrFalse()
		{
			AttributeConstraint findBy = Find.ByName("X").Or(Find.ByName("Y"));
			MockAttributeBag attributeBag = new MockAttributeBag("name", "Z");
			Assert.IsFalse(findBy.Compare(attributeBag));
		}

		[Test]
		public void AndOr()
		{
			AttributeConstraint findByNames = Find.ByName("X").Or(Find.ByName("Y"));
			AttributeConstraint findBy = Find.ByValue("Cancel").And(findByNames);

			MockAttributeBag attributeBag = new MockAttributeBag("name", "X");
			attributeBag.Add("value", "Cancel");
			Assert.IsTrue(findBy.Compare(attributeBag));
		}

		[Test]
		public void AndOrThroughOperatorOverloads()
		{
			AttributeConstraint findBy = Find.ByName("X") & Find.ByValue("Cancel") | (Find.ByName("Z") & Find.ByValue("Cancel"));

			MockAttributeBag attributeBag = new MockAttributeBag("name", "Z");
			attributeBag.Add("value", "OK");
			Assert.IsFalse(findBy.Compare(attributeBag));
		}

		[Test, ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void OccurrenceShouldNotAcceptNegativeValue()
		{
			new IndexAttributeConstraint(-1);
		}

		[Test]
		public void Occurence0()
		{
			AttributeConstraint findBy = new IndexAttributeConstraint(0);

			MockAttributeBag attributeBag = new MockAttributeBag("name", "Z");
			Assert.IsTrue(findBy.Compare(attributeBag));
			Assert.IsFalse(findBy.Compare(attributeBag));
		}

		[Test]
		public void Occurence2()
		{
			AttributeConstraint findBy = new IndexAttributeConstraint(2);

			MockAttributeBag attributeBag = new MockAttributeBag("name", "Z");
			Assert.IsFalse(findBy.Compare(attributeBag));
			Assert.IsFalse(findBy.Compare(attributeBag));
			Assert.IsTrue(findBy.Compare(attributeBag));
			Assert.IsFalse(findBy.Compare(attributeBag));
		}

		[Test]
		public void OccurenceAndTrue()
		{
			AttributeConstraint findBy = new IndexAttributeConstraint(1).And(Find.ByName("Z"));

			MockAttributeBag attributeBag = new MockAttributeBag("name", "Z");
			Assert.IsFalse(findBy.Compare(attributeBag));
			Assert.IsTrue(findBy.Compare(attributeBag));
		}

		[Test]
		public void OccurenceOr()
		{
			AttributeConstraint findBy = new IndexAttributeConstraint(2).Or(Find.ByName("Z"));

			MockAttributeBag attributeBag = new MockAttributeBag("name", "Z");
			Assert.IsTrue(findBy.Compare(attributeBag));

			attributeBag = new MockAttributeBag("name", "y");
			Assert.IsFalse(findBy.Compare(attributeBag));
			Assert.IsTrue(findBy.Compare(attributeBag));
		}

		[Test]
		public void OccurenceAndFalse()
		{
			AttributeConstraint findBy = new IndexAttributeConstraint(1).And(Find.ByName("Y"));

			MockAttributeBag attributeBag = new MockAttributeBag("name", "Z");
			Assert.IsFalse(findBy.Compare(attributeBag));
			Assert.IsFalse(findBy.Compare(attributeBag));
		}

		[Test]
		public void TrueAndOccurence()
		{
			AttributeConstraint findBy = Find.ByName("Z").And(new IndexAttributeConstraint(1));

			MockAttributeBag attributeBag = new MockAttributeBag("name", "Z");
			Assert.IsFalse(findBy.Compare(attributeBag));
			Assert.IsTrue(findBy.Compare(attributeBag));
		}

		[Test]
		public void FalseAndOccurence()
		{
			AttributeConstraint findBy = Find.ByName("Y").And(new IndexAttributeConstraint(1));

			MockAttributeBag attributeBag = new MockAttributeBag("name", "Z");
			Assert.IsFalse(findBy.Compare(attributeBag));
			Assert.IsFalse(findBy.Compare(attributeBag));
		}

		[Test]
		public void TrueAndOccurenceAndTrue()
		{
			AttributeConstraint findBy = Find.ByName("Z").And(new IndexAttributeConstraint(1)).And(Find.ByValue("text"));

			Expect.Call(mockAttributeBag.GetValue("name")).Return("Z");
			Expect.Call(mockAttributeBag.GetValue("value")).Return("text");

			Expect.Call(mockAttributeBag.GetValue("name")).Return("Z");
			Expect.Call(mockAttributeBag.GetValue("value")).Return("some other text");

			Expect.Call(mockAttributeBag.GetValue("name")).Return("Y");

			Expect.Call(mockAttributeBag.GetValue("name")).Return("Z");
			Expect.Call(mockAttributeBag.GetValue("value")).Return("text");

			mocks.ReplayAll();

			Assert.IsFalse(findBy.Compare(mockAttributeBag));
			Assert.IsFalse(findBy.Compare(mockAttributeBag));
			Assert.IsFalse(findBy.Compare(mockAttributeBag));
			Assert.IsTrue(findBy.Compare(mockAttributeBag));

			mocks.VerifyAll();
		}

		[Test]
		public void OccurenceAndOrWithOrTrue()
		{
			AttributeConstraint findBy = new IndexAttributeConstraint(2).And(Find.ByName("Y")).Or(Find.ByName("Z"));

			MockAttributeBag attributeBag = new MockAttributeBag("name", "Z");
			Assert.IsTrue(findBy.Compare(attributeBag));
		}

		[Test]
		public void OccurenceAndOrWithAndTrue()
		{
			AttributeConstraint findBy = new IndexAttributeConstraint(2).And(Find.ByName("Y")).Or(Find.ByName("Z"));

			MockAttributeBag attributeBag = new MockAttributeBag("name", "Y");
			Assert.IsFalse(findBy.Compare(attributeBag));
			Assert.IsFalse(findBy.Compare(attributeBag));
			Assert.IsTrue(findBy.Compare(attributeBag));
			Assert.IsFalse(findBy.Compare(attributeBag));
		}

		[Test, ExpectedException(typeof (ReEntryException))]
		public void RecusiveCallExceptionExpected()
		{
			AttributeConstraint findBy = Find.By("tag", "value");
			findBy.Or(findBy);

			Expect.Call(mockAttributeBag.GetValue("tag")).Return("val").Repeat.AtLeastOnce();

			mocks.ReplayAll();
			findBy.Compare(mockAttributeBag);
			mocks.VerifyAll();
		}
	}
}