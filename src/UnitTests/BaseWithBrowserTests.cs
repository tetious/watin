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
using NUnit.Framework;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;

namespace WatiN.Core.UnitTests
{
	public abstract class BaseWithBrowserTests : BaseWatiNTest
	{
        /// <summary>
        /// The test method to execute.
        /// </summary>
        protected delegate void BrowserTest(Browser browser);

	    private IE ie;
	    private FireFox firefox;

		[TestFixtureSetUp]
		public override void FixtureSetup()
		{
		    base.FixtureSetup();
//		    Logger.LogWriter = new ConsoleLogWriter();
		    CreateNewIeInstance();
		}

	    public void CreateNewIeInstance()
	    {
	        ie = new IE(TestPageUri);
	    }

	    [TestFixtureTearDown]
		public override void FixtureTearDown()
	    {
	        CloseBrowsers();

	        base.FixtureTearDown();
	    }

	    private void CloseBrowsers()
	    {
	        if (ie != null)
	        {
	            ie.Close();
	            ie = null;
	        }
	        
            if (firefox == null) return;
	        firefox.Dispose();
	        firefox = null;
	    }

	    [SetUp]
		public virtual void TestSetUp()
	    {
	        Settings.Reset();
	    }

	    private void GoToTestPage(Browser browser)
	    {
	        if ( browser != null && !browser.Uri.Equals(TestPageUri))
	        {
	            browser.GoTo(TestPageUri);
	        }
	    }

	    public abstract Uri TestPageUri { get; }

	    public IE Ie
	    {
	        get
	        {
                if (ie == null)
                {
                    ie = new IE();
                    GoToTestPage(ie);
                }

                return ie;
	        }
	    }

	    public FireFox Firefox
	    {
	        get
	        {
                if (firefox == null)
                {
                    firefox = new FireFox();
                    GoToTestPage(firefox);
                }

                return firefox;
	        }
	    }

        /// <summary>
        /// Executes the test using both FireFox and Internet Explorer.
        /// </summary>
        /// <param name="testMethod">The test method.</param>
        protected void ExecuteTest(BrowserTest testMethod)
        {
            ExecuteTest(testMethod, Firefox);
//            ExecuteTest(testMethod, Ie);
        }

        private static void ExecuteTest(BrowserTest testMethod, Browser browser)
        {
            try
            {
                testMethod.Invoke(browser);
            }
            catch (WatiNException e)
            {
                Logger.LogAction(browser.GetType() + " exception: " + e.Message);
                throw;
            }
            catch(Exception e)
            {
                throw new WatiNException(browser.GetType() + " exception", e);
            }
        }

	}

	public class StealthSettings : DefaultSettings
	{
		public StealthSettings()
		{
			SetDefaults();
		}

		public override void Reset()
		{
			SetDefaults();
		}

		private void SetDefaults()
		{
			base.Reset();
			AutoMoveMousePointerToTopLeft = false;
			HighLightElement = false;
			MakeNewIeInstanceVisible = false;
		}
	}
}