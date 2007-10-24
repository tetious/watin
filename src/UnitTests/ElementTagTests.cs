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

using System.Collections;
using System.Globalization;
using System.Threading;
using mshtml;
using NUnit.Framework;
using Rhino.Mocks;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class ElementTagTests
	{
		[Test]
		public void CompareNullShouldReturnFalse()
		{
			ElementTag elementTag = new ElementTag("tagname", "");
			Assert.IsFalse(elementTag.Compare(null));
		}

		[Test]
		public void CompareObjectNotImplementingIHTMLElementShouldReturnFalse()
		{
			ElementTag elementTag = new ElementTag("tagname", "");
			Assert.IsFalse(elementTag.Compare(new object()));
		}

		[Test]
		public void IsValidElementWithNullElementShouldReturnFalse()
		{
			Assert.IsFalse(ElementTag.IsValidElement(null, new ArrayList()));
		}

		[Test]
		public void IsValidElementWithObjectNotImplementingIHTMLElementShouldReturnFalse()
		{
			Assert.IsFalse(ElementTag.IsValidElement(new object(), new ArrayList()));
		}

		[Test]
		public void ToStringShouldBeEmptyIfTagNameIsNull()
		{
			ElementTag elementTag = new ElementTag((string) null);
			Assert.That(elementTag.ToString(), NUnit.Framework.SyntaxHelpers.Is.EqualTo(""));
		}

		[Test]
		public void CompareShouldBeCultureInvariant()
		{
			// Get the tr-TR (Turkish-Turkey) culture.
			CultureInfo turkish = new CultureInfo("tr-TR");

			// Get the culture that is associated with the current thread.
			CultureInfo thisCulture = Thread.CurrentThread.CurrentCulture;

			try
			{
				// Set the culture to Turkish
				Thread.CurrentThread.CurrentCulture = turkish;

				MockRepository mockRepository = new MockRepository();
				IHTMLElement element = (IHTMLElement) mockRepository.DynamicMultiMock(typeof (IHTMLElement), typeof(IHTMLInputElement));

				AssertUpperCaseLowerCase(element, mockRepository);
				AssertUpperCaseUpperCase(element, mockRepository);
				AssertLowerCaseUpperCase(element, mockRepository);
			}
			finally
			{
				// Set the culture back to the original
				Thread.CurrentThread.CurrentCulture = thisCulture;
			}
		}

		private static void AssertLowerCaseUpperCase(IHTMLElement element, MockRepository mockRepository) 
		{
			mockRepository.BackToRecordAll();

			// LowerCase
			SetupResult.For(element.tagName).Return("input");
			SetupResult.For(((IHTMLInputElement) element).type).Return("image");

			mockRepository.ReplayAll();
				
			// UpperCase
			ElementTag elementTag = new ElementTag("INPUT", "IMAGE");
			Assert.IsTrue(elementTag.Compare(element), "Compare should compare using CultureInvariant");
				
			mockRepository.VerifyAll();
		}

		private static void AssertUpperCaseUpperCase(IHTMLElement element, MockRepository mockRepository) 
		{
			mockRepository.BackToRecordAll();

			// UpperCase
			SetupResult.For(element.tagName).Return("INPUT");
			SetupResult.For(((IHTMLInputElement) element).type).Return("IMAGE");

			mockRepository.ReplayAll();
				
			// UpperCase
			ElementTag elementTag = new ElementTag("INPUT", "IMAGE");
			Assert.IsTrue(elementTag.Compare(element), "Compare should compare using CultureInvariant");
				
			mockRepository.VerifyAll();
		}

		private static void AssertUpperCaseLowerCase(IHTMLElement element, MockRepository mockRepository) {
			
			// UpperCase
			SetupResult.For(element.tagName).Return("INPUT");
			SetupResult.For(((IHTMLInputElement) element).type).Return("IMAGE");
			
			mockRepository.ReplayAll();

			// LowerCase
			ElementTag elementTag = new ElementTag("input", "image");
			Assert.IsTrue(elementTag.Compare(element), "Compare should compare using CultureInvariant");
			Assert.AreEqual("INPUT (image)", elementTag.ToString(), "ToString problem");

			mockRepository.VerifyAll();
		}
	}
}