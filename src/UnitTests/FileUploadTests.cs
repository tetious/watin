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
using NUnit.Framework.SyntaxHelpers;
using System.Collections.Generic;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class FileUploadTests : BaseWithBrowserTests
	{
		[Test]
		public void FileUploadElementTags()
		{
            IList<ElementTag> elementTags = ElementFactory.GetElementTags<FileUpload>();
            Assert.AreEqual(1, elementTags.Count, "1 elementtags expected");
			Assert.AreEqual("input", elementTags[0].TagName);
			Assert.AreEqual("file", elementTags[0].InputType);
		}

		[Test]
		public void FileUploadExists()
		{
		    ExecuteTest(browser =>
		                    {
		                        Assert.IsTrue(browser.FileUpload("upload").Exists);
		                        Assert.IsTrue(browser.FileUpload(new Regex("upload")).Exists);
		                        Assert.IsFalse(browser.FileUpload("noneexistingupload").Exists);
		                    });
		}

	    [Test]
	    public void FileUploadOfFileWithSpecialCharactersInFilenameShouldBeHandledOK()
	    {
	        ExecuteTest(browser =>
	                        {
	                            // GIVEN
	                            const string fileName = @"~^+{}[]().txt";
	                            
                                browser.GoTo(TestPageUri);

	                            var fileUpload = browser.FileUpload("upload");

	                            Assert.That(fileUpload.Exists, "Pre-Condition: Expected file upload element");
	                            Assert.That(fileUpload.FileName,Text.DoesNotEndWith(fileName), "pre-Condition: Filename already set as default");

	                            var file = new Uri(HtmlTestBaseURI, fileName).LocalPath;

	                            // WHEN
	                            fileUpload.Set(file);

	                            // THEN
	                            Assert.That(fileUpload.FileName, Text.EndsWith(fileName), "Unexpected filename");
	                        });
	    }

	    [Test]
		public void FileUploadTest()
		{
		    ExecuteTest(browser =>
		                    {
                                // GIVEN
		                        const string fileName = "main.html";
		                        var fileUpload = browser.FileUpload("upload");

		                        Assert.That(fileUpload.Exists);
                                Assert.That(fileUpload.FileName, Text.DoesNotEndWith(fileName), "pre-Condition: Filename already set as default");

                                // WHEN
		                        fileUpload.Set(MainURI.LocalPath);

                                // THEN
		                        Assert.That(fileUpload.FileName, Text.EndsWith(fileName));
		                    });
		}

        // TODO: Should be mocked cause this WatiN behaviour not browser related
		[Test, ExpectedException(typeof (System.IO.FileNotFoundException))]
		public void FileUploadFileNotFoundException()
		{
			var fileUpload = Ie.FileUpload("upload");
			fileUpload.Set("nonexistingfile.nef");
		}

		[Test]
		public void FileUploads()
		{
		    ExecuteTest(browser =>
		                    {
		                        const int expectedFileUploadsCount = 1;
		                        Assert.AreEqual(expectedFileUploadsCount, browser.FileUploads.Count, "Unexpected number of FileUploads");

		                        // Collection.Length
		                        var formFileUploads = browser.FileUploads;

		                        // Collection items by index
		                        Assert.AreEqual("upload", browser.FileUploads[0].Id);

		                        IEnumerable FileUploadEnumerable = formFileUploads;
		                        var FileUploadEnumerator = FileUploadEnumerable.GetEnumerator();

		                        // Collection iteration and comparing the result with Enumerator
		                        var count = 0;
		                        foreach (FileUpload inputFileUpload in formFileUploads)
		                        {
		                            FileUploadEnumerator.MoveNext();
		                            var enumFileUpload = FileUploadEnumerator.Current;

		                            Assert.IsInstanceOfType(inputFileUpload.GetType(), enumFileUpload, "Types are not the same");
		                            Assert.AreEqual(inputFileUpload.OuterHtml, ((FileUpload) enumFileUpload).OuterHtml, "foreach and IEnumator don't act the same.");
		                            ++count;
		                        }

		                        Assert.IsFalse(FileUploadEnumerator.MoveNext(), "Expected last item");
		                        Assert.AreEqual(expectedFileUploadsCount, count);
		                    });
		}

	    public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}