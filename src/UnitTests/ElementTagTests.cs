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
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Moq;
using NUnit.Framework;
using WatiN.Core.Native;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ElementTagTests
	{
		[Test]
        [ExpectedException(typeof(ArgumentNullException))]
		public void IsMatchNullShouldThrow()
		{
			var elementTag = new ElementTag("tagname");
			elementTag.IsMatch(null);
		}

		[Test]
		public void ToStringShouldBeEmptyIfTagNameIsNull()
		{
			var elementTag = new ElementTag((string) null);
			Assert.That(elementTag.ToString(), NUnit.Framework.SyntaxHelpers.Is.EqualTo(""));
		}

		[Test]
		public void CompareShouldBeCultureInvariant()
		{
			// Get the tr-TR (Turkish-Turkey) culture.
			var turkish = new CultureInfo("tr-TR");

			// Get the culture that is associated with the current thread.
			var thisCulture = Thread.CurrentThread.CurrentCulture;

			try
			{
				// Set the culture to Turkish
				Thread.CurrentThread.CurrentCulture = turkish;

				var elementMock = new Mock<INativeElement>();

				AssertUpperCaseLowerCase(elementMock);
				AssertUpperCaseUpperCase(elementMock);
				AssertLowerCaseUpperCase(elementMock);
			}
			finally
			{
				// Set the culture back to the original
				Thread.CurrentThread.CurrentCulture = thisCulture;
			}
		}

		private static void AssertLowerCaseUpperCase(Mock<INativeElement> elementMock) 
		{
			// LowerCase
			elementMock.Expect(element => element.TagName).Returns("input");
            elementMock.Expect(element => element.GetAttributeValue("type")).Returns("image");

			// UpperCase
			var elementTag = new ElementTag("INPUT", "IMAGE");
			Assert.IsTrue(elementTag.IsMatch(elementMock.Object), "Compare should compare using CultureInvariant");

            elementMock.VerifyAll();
		}

		private static void AssertUpperCaseUpperCase(Mock<INativeElement> elementMock) 
		{
			// UpperCase
            elementMock.Expect(element => element.TagName).Returns("INPUT");
            elementMock.Expect(element => element.GetAttributeValue("type")).Returns("IMAGE");

			// UpperCase
			var elementTag = new ElementTag("INPUT", "IMAGE");
			Assert.IsTrue(elementTag.IsMatch(elementMock.Object), "Compare should compare using CultureInvariant");

            elementMock.VerifyAll();
		}

		private static void AssertUpperCaseLowerCase(Mock<INativeElement> elementMock) {
			
			// UpperCase
            elementMock.Expect(element => element.TagName).Returns("INPUT");
            elementMock.Expect(element => element.GetAttributeValue("type")).Returns("IMAGE");
			
			// LowerCase
			var elementTag = new ElementTag("input", "image");
			Assert.IsTrue(elementTag.IsMatch(elementMock.Object), "Compare should compare using CultureInvariant");
			Assert.AreEqual("INPUT (image)", elementTag.ToString(), "ToString problem");

            elementMock.VerifyAll();
		}
	}
}