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

using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.DialogHandlers;

namespace WatiN.Core.UnitTests.DialogHandlerTests
{
	[TestFixture]
	public class FileDownloadHandlerTests
	{
		[Test, Ignore("Because of timeout issues, run this test manually and not automated"), Category("InternetConnectionNeeded")]
		public void DownloadOpen()
		{
			var dhdl = new FileDownloadHandler(FileDownloadOptionEnum.Open);

			var ie = new IE();
			ie.AddDialogHandler(dhdl);
			ie.WaitForComplete();
			ie.GoTo("http://watin.sourceforge.net/WatiNRecorder.zip");

			dhdl.WaitUntilFileDownloadDialogIsHandled(5);
			dhdl.WaitUntilDownloadCompleted(20);
			ie.Close();
		}

        [Test, Category("InternetConnectionNeeded"), Ignore("Because of timeout issues, run this test manually and not automated")]
		public void DownloadSave()
		{
			var file = new FileInfo(@"c:\temp\test.zip");
			file.Directory.Create();
			file.Delete();
            Assert.That(file.Exists, Is.False, file.FullName + " file should not exist before download");

			var fileDownloadHandler = new FileDownloadHandler(file.FullName);

			using (var ie = new IE())
			{
				ie.AddDialogHandler(fileDownloadHandler);

//				ie.GoTo("http://watin.sourceforge.net/WatiN-1.0.0.4000-net-1.1.msi");
				        ie.GoTo("http://watin.sourceforge.net/WatiNRecorder.zip");

				fileDownloadHandler.WaitUntilFileDownloadDialogIsHandled(15);
				fileDownloadHandler.WaitUntilDownloadCompleted(200);
			}

            file = new FileInfo(@"c:\temp\test.zip");
			Assert.IsTrue(file.Exists, file.FullName + " file does not exist after download");
		}

		[Test, Ignore("Because of timeout issues, run this test manually and not automated"), Category("InternetConnectionNeeded")]
		public void DownloadRun()
		{
			var dhdl = new FileDownloadHandler(FileDownloadOptionEnum.Run);
			var ie = new IE();
			ie.AddDialogHandler(dhdl);
			ie.WaitForComplete();
			ie.GoTo("http://watin.sourceforge.net/WatiN-1.0.0.4000-net-1.1.msi");

			dhdl.WaitUntilFileDownloadDialogIsHandled(5);
			dhdl.WaitUntilDownloadCompleted(20);
			ie.Close();
		}
	}
}