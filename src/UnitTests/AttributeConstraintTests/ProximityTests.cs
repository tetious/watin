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

// This class is kindly donated by Seven Simple Machines

using System;
using mshtml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;


namespace WatiN.Core.UnitTests.AttributeConstraintTests
{
    /// <summary>
    /// Tests for identifying text-field correlation by proximity
    /// </summary>
    [TestFixture]
    public class ProximityTests : BaseWithIETests
    {
        private string _startingHtml;

        public override Uri TestPageUri
        {
            get { return (ProximityURI); }
        }

        [SetUp]
        public override void TestSetUp()
        {
            base.TestSetUp();

            // Store the HTML for testing on TestTearDown (see note below)
            _startingHtml = ie.Html;
        }

        [TearDown]
        // This not strictly a tear-down but used to verify that all tested procedures don't alter the HTML
        public void TestTearDown()
        {
            // For debugging
//            using( StreamWriter inp = File.CreateText( @"C:\temp\input.html" ) ) {
//                inp.Write( _startingHtml );
//            }
//            using( StreamWriter outp = File.CreateText( @"C:\temp\ouput.html" )) {
//                outp.Write( ie.Html );
//            }
			
            string endingHtml = ie.Html;
			
            // If these don't match, reset the browser so further tests are not affected by this one
            if( endingHtml != _startingHtml ) ie = new IE(ProximityURI);

            // Now record the test result
            Assert.That( endingHtml, Is.EqualTo( _startingHtml ), "HTML in the page changed while the test executed." );
        }

        [Test]
        // Verify absolute left/right positioning of text and field
        public void ShouldWorkWithAbsolutePositionedFields() 
        {
            TextField inputSearch = ie.TextField(new ProximityTextConstraint("Search"));
            Assert.AreEqual("inputSearch", inputSearch.Id, "Absoloute positioned proximity for text did not find correct field.");
        }
		
        [Test]
        // Verify left/right juxtaposition of text and field
        public void ShouldWorkWithLabelLeftAndFieldRight() 
        {
			
            TextField inputUsername = ie.TextField(new ProximityTextConstraint("User Name"));
            Assert.AreEqual("inputUserName", inputUsername.Id, "Left/right proximity for text did not find correct field.");

            TextField inputPassword = ie.TextField(new ProximityTextConstraint("Password"));
            Assert.AreEqual("inputPassword", inputPassword.Id, "Left/right proximity for text did not find correct field.");
			
        }
		
        [Test]
        // Verify top/bottom juxtaposition of text and field
        public void ShouldWorkWithLabelTopAndFieldRight() 
        {
			
            TextField inputFirstName = ie.TextField(new ProximityTextConstraint("First Name"));
            Assert.AreEqual("inputFirstName", inputFirstName.Id, "Top/bottom proximity for text did not find correct field.");

            TextField inputLastName = ie.TextField(new ProximityTextConstraint("Last Name"));
            Assert.AreEqual("inputLastName", inputLastName.Id, "Top/bottom proximity for text did not find correct field.");

            TextField inputEmail = ie.TextField(new ProximityTextConstraint("Email"));
            Assert.AreEqual("inputEmail", inputEmail.Id, "Top/bottom proximity for text did not find correct field.");
			
        }

        [Test]
        public void ShouldWorkWithTableLayout() 
        {
			
            TextField inputConfirmCode = ie.TextField(new ProximityTextConstraint("Confirm Code"));
            Assert.AreEqual("inputConfirmCode", inputConfirmCode.Id, "Proximity for table layout of text and field did not find correct field.");
			
        }
		
        [Test]
        public void ShouldWorkWithValidationMessage() 
        {
			
            TextField inputCode = ie.TextField(new ProximityTextConstraint("Code"));
            Assert.AreEqual("inputCode", inputCode.Id, "Proximity for table layout of text and field did not find correct field.");
			
        }

        [Test, ExpectedException(typeof(ElementNotFoundException))]
        public void ShouldFailWhenTextIsNotOnPage() {
			
            Settings.WaitUntilExistsTimeOut = 1;
            TextField inputDoesNotExist = ie.TextField( new ProximityTextConstraint(  "Aldm4em9dj54bnsvk49sk4ndlkDKj 4KVj FDS" ) );
            inputDoesNotExist.TypeText( "" );	// To activate the lookup
        }
		
        [Test, ExpectedException(typeof(ElementNotFoundException))]
        public void ShouldFailWhenTextIsPartOfAnHtmlElement() 
        {
			
            Settings.WaitUntilExistsTimeOut = 1;
            TextField inputDoesNotExist = ie.TextField( new ProximityTextConstraint( "submitTable" ) );
            inputDoesNotExist.TypeText( "" );	// To activate the lookup
        }

        [Test]
        // I am not certain that this test really works. I can't ever cause it to fail and I 
        // don't think that the inserted style is being applied.
        public void ShouldNotBeAffectedByStyles() {
            // Create a style that drastically affects the span tags
            var body = ((IHTMLDocument2)ie.NativeDocument.Object).body;
            body.insertAdjacentHTML( "afterBegin", "<style type=\"text/css\">span {display:block; position:absolute; left:0; top:0;}</style>" );
			
            // Test that text near an element is not then confused with another, closer element
            // The search text should be moved to the top right, nearer to many other elements than
            // the search field.
            var inputSearch = ie.TextField(new ProximityTextConstraint("Search"));
            Assert.AreEqual("inputSearch", inputSearch.Id, "Absoloute positioned proximity for text did not find correct field.");
			
            // Cleanup by re-loading the page
            ie.GoTo(ProximityURI);
        }

        [Test]
        // This bug has to do with a problem in the element.outerHTML assignment:
        // For some reason when this is applied to text preceded by a span tag then the 
        // entire span element is duplicated as well.
        // For example this:
        //		<SPAN id=spanValidateCode>Code and Confirm Code must match!</SPAN>
        // Becomes this after attempting comparison form elements:
        //		<SPAN id=spanValidateCode><SPAN id=spanValidateCode>Code</SPAN> and Confirm Code must match!</SPAN>
        // Note that this test would also be caught be the more general test that makes sure the HTML isn't changed.
        public void ShouldNotDuplicateSpanElementsPrecedingTheText() {
            var document = (IHTMLDocument2)ie.NativeDocument.Object;
            var span = (IHTMLElement)document.all.item("spanValidateCode", null);
            var originalContent = span.outerHTML;
			
            var inputCode = ie.TextField(new ProximityTextConstraint("Code"));
            Assert.AreEqual("inputCode", inputCode.Id, "Proximity for table layout of text and field did not find correct field.");

            document = (IHTMLDocument2)ie.NativeDocument.Object;
            var obj = document.all.item( "spanValidateCode", null );
            Assert.IsNotNull( obj, "Enclosing span element no longer exists or is not unique.");
            Assert.That(obj, Is.TypeOf(typeof(mshtml.HTMLSpanElementClass)), "Enclosing span element no longer exists or is not unique.");
            span = (IHTMLElement)obj;
            Assert.AreEqual( originalContent, span.outerHTML, "Content of enclosing span changed during testing." );
        }
		
        [Test, ExpectedException(typeof(ElementNotFoundException))]
        public void ShouldFailWhenLabelContainsHtml() {
			
            Settings.WaitUntilExistsTimeOut = 1;
            var inputFirstName = ie.TextField(new ProximityTextConstraint("First Name:<br />"));
            inputFirstName.TypeText( "" );	// To activate the lookup
			
        }
		
        // TODO: More tests
        // - Pass when label is diagonal from field
		
		
    }
}