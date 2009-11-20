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
using NUnit.Framework;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;
using WatiN.Core.UnitTests.FireFoxTests;
using WatiN.Core.UnitTests.IETests;
# if INCLUDE_CHROME
using WatiN.Core.UnitTests.Native.ChromeTests;
#endif

namespace WatiN.Core.UnitTests.TestUtils
{
    public abstract class BaseWithBrowserTests : BaseWatiNTest
    {
        /// <summary>
        /// The test method to execute.
        /// </summary>
        public delegate void BrowserTest(Browser browser);

        private readonly IBrowserTestManager ieManager = new IEBrowserTestManager();
        private readonly IBrowserTestManager ffManager = new FFBrowserTestManager();
# if INCLUDE_CHROME
        private readonly IBrowserTestManager chromeManager = new ChromeBrowserTestManager();
#endif

        public readonly List<IBrowserTestManager> BrowsersToTestWith = new List<IBrowserTestManager>();

        // TODO: remove this property in time
        public IE Ie
        {
            get
            {
                if (InsideExecuteTest) throw new WatiNException("Specific test for IE detected inside call to ExecuteTest");
                return (IE)ieManager.GetBrowser(TestPageUri);
            }
        }

        // TODO: remove this property in time
        public FireFox Firefox
        {
            get
            {
                if (InsideExecuteTest) throw new WatiNException("Specific test for Firefox detected inside call to ExecuteTest");
                return (FireFox)ffManager.GetBrowser(TestPageUri);
            }
        }

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();
#if !IncludeChromeInUnitTesting
            BrowsersToTestWith.Add(ieManager);
            BrowsersToTestWith.Add(ffManager);
#else
		    BrowsersToTestWith.Add(chromeManager);
#endif
            Logger.LogWriter = new ConsoleLogWriter {IgnoreLogDebug = true};
        }

        [TestFixtureTearDown]
        public override void FixtureTearDown()
        {
            var exceptions = new List<Exception>();
            BrowsersToTestWith.ForEach(browserTestManager =>
                {
                    try
                    {
                        browserTestManager.CloseBrowser();
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
                });
            base.FixtureTearDown();

            foreach (var exception in exceptions)
                Logger.LogAction( exception.Message + Environment.NewLine + exception.StackTrace);

            foreach (var exception in exceptions)
                throw exception;
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

        /// <summary>
        /// Executes the test using both FireFox and Internet Explorer.
        /// </summary>
        /// <param name="testMethod">The test method.</param>
        public void ExecuteTest(BrowserTest testMethod)
        {
            InsideExecuteTest = true;
            try
            {
                BrowsersToTestWith.ForEach(browserTestManager =>
                                               {
                                                   GoToTestPage(browserTestManager.GetBrowser(TestPageUri));
                                                   ExecuteTest(testMethod, browserTestManager.GetBrowser(TestPageUri));
                                               }
                    );
            }
            finally
            {
                InsideExecuteTest = false;
            }
        }

        public void ExecuteTest(BrowserTest testMethod, Browser browser)
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

        public void ExecuteTestWithAnyBrowser(BrowserTest testMethod)
        {
            GoToTestPage(Ie);
            ExecuteTest(testMethod, Ie);
        }

        private static bool InsideExecuteTest { get; set; }
    }
}