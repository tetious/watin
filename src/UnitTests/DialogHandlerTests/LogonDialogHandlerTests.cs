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
using NUnit.Framework;
using WatiN.Core.DialogHandlers;

namespace WatiN.Core.UnitTests.DialogHandlerTests
{
	[TestFixture]
	public class LogonDialogHandlerTests
	{
		[Test, Category("InternetConnectionNeeded")]
		public void LogonDialogTest()
		{
			using (var ie = new IE())
			{
				var logonDialogHandler = new LogonDialogHandler("test", "this");
				using (new UseDialogOnce(ie.DialogWatcher, logonDialogHandler))
				{
					
					ie.GoTo("http://irisresearch.library.cornell.edu/control/authBasic/authTest");
				}
				ie.WaitUntilContainsText("Basic Authentication test passed successfully",5);
			}
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void LogonDialogWithUserNameNullShouldThrowArgumentNullException()
		{
			new LogonDialogHandler(null, "pwd");
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void LogonDialogWithUserNameStringEmptyShouldThrowArgumentNullException()
		{
			new LogonDialogHandler(String.Empty, "pwd");
		}

		[Test]
		public void LogonDialogValidConstructorArguments()
		{
			new LogonDialogHandler("username", "pwd");
			new LogonDialogHandler("username", "");
			new LogonDialogHandler("username", null);
		}
	}
}