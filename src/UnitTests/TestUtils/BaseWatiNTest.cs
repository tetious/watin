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
using System.IO;
using NUnit.Framework;
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests.TestUtils
{
    public abstract class BaseWatiNTest
    {
        private static Uri HtmlTestBaseUriInternal;
        private ISettings _backupSettings;

        public static Uri MainURI = new Uri(HtmlTestBaseURI, "main.html");
        public static Uri IndexURI = new Uri(HtmlTestBaseURI, "Index.html");
        public static Uri PopUpURI = new Uri(HtmlTestBaseURI, "popup.html");
        public static Uri FramesetURI = new Uri(HtmlTestBaseURI, "Frameset.html");
        public static Uri FramesetWithinFramesetURI = new Uri(HtmlTestBaseURI, "FramesetWithinFrameset.html");
        public static Uri CrossDomainFramesetURI = new Uri(HtmlTestBaseURI, "CrossDomainFrameset.html");
        public static Uri TestEventsURI = new Uri(HtmlTestBaseURI, "TestEvents.html");
        public static Uri FormSubmitURI = new Uri(HtmlTestBaseURI, "formsubmit.html");
        public static Uri WatiNURI = new Uri("http://watin.sourceforge.net");
        public static Uri ImagesURI = new Uri(HtmlTestBaseURI, "images.html");
        public static Uri IFramesMainURI = new Uri(HtmlTestBaseURI, "iframes\\main.html");
        public static Uri IFramesLeftURI = new Uri(HtmlTestBaseURI, "iframes\\leftpage.html");
        public static Uri IFramesMiddleURI = new Uri(HtmlTestBaseURI, "iframes\\middlepage.html");
        public static Uri IFramesRightURI = new Uri(HtmlTestBaseURI, "iframes\\rightpage.html");
        public static Uri OnBeforeUnloadJavaDialogURI = new Uri(HtmlTestBaseURI, "OnBeforeUnloadJavaDialog.html");
        public static Uri TablesUri = new Uri(HtmlTestBaseURI, "Tables.html");
        public static Uri ProximityURI = new Uri(HtmlTestBaseURI, "ProximityTests.html");
        public static Uri AboutBlank = new Uri("about:blank");
        public static Uri ButtonTestsUri = new Uri(HtmlTestBaseUriInternal, "ButtonTests.html");
        public static Uri StyleTestUri = new Uri(HtmlTestBaseUriInternal, "styletests\\Style.html");
        public static Uri TheAppUri = new Uri(HtmlTestBaseUriInternal, "theApp.html");
        public static Uri NewWindowUri = new Uri(HtmlTestBaseURI, "openNewWindow.html");
        public static Uri NewWindowTargetUri = new Uri(HtmlTestBaseURI, "openNewWindowTarget.html");
        public static string GoogleUrl = "http://www.google.com";
        public static string EbayUrl = "http://www.ebay.com";

        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            _backupSettings = Settings.Clone();
            Settings.Instance = new StealthSettings();
        }

        [TestFixtureTearDown]
        public virtual void FixtureTearDown()
        {
            Settings.Instance = _backupSettings;
        }

        public static Uri HtmlTestBaseURI
        {
            get
            {
                if (HtmlTestBaseUriInternal == null)
                {
                    HtmlTestBaseUriInternal = new Uri(GetHtmlTestFilesLocation());
                }
                return HtmlTestBaseUriInternal;
            }
        }

        private static string GetHtmlTestFilesLocation()
        {
            var baseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            // Search for the html directory in the current domains base directory
            // Valid when executing WatiN UnitTests in a deployed situation. 
            var htmlTestFilesLocation = baseDirectory.FullName + @"\html\";

            if (!Directory.Exists(htmlTestFilesLocation))
            {
                // If html directory not found, search one dir up in the directory tree
                // Valid when executing WatiN UnitTests from within Visual Studio
                htmlTestFilesLocation = baseDirectory.Parent.FullName + @"\html\";
            }

            return htmlTestFilesLocation;
        }
    }
}