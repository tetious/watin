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
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests
{
    /// <summary>
    ///This is a NUnit TestFixture class
    ///</summary>
    [TestFixture]
    public class WatiNSanityTests : BaseWatiNTest
    {
        [Test]
        public void ShouldEnsureThatEachHtmlFileHasAProperMarkOfTheWebComment()
        {
            var htmlFiles = Directory.GetFiles(HtmlTestBaseURI.AbsolutePath, "*.htm*", SearchOption.AllDirectories);
            var fail = false;
            foreach (var htmlFile in htmlFiles)
            {
                // skip svn files
                if (!htmlFile.EndsWith("html",true,CultureInfo.InvariantCulture) &&
                   (!htmlFile.EndsWith("htm",true,CultureInfo.InvariantCulture)))
                {
                    continue;
                }

                var fileFailed = false;
                Console.WriteLine();

                var textfile = File.OpenText(htmlFile);
                try
                {
                    var firstline = textfile.ReadLine();
                    if (!Equals(firstline, "<!-- saved from url=(0029)http://watin.sourceforge.net/ -->"))
                    {
                        fail = true;
                        fileFailed = true;
                        Console.WriteLine(htmlFile);
                        Console.WriteLine(firstline);
                    }
                }
                finally
                {
                    textfile.Close();
                }

                if (fileFailed) continue;

                var bytes = File.ReadAllBytes(htmlFile);

                string crlf = null;
                if (bytes.Length >= 60)
                {
                    crlf = ((char)bytes[59]).ToString() + ((char)bytes[60]);
                }
                if (Equals(crlf, "\r\n")) continue;
                
                fail = true;
                Console.WriteLine(htmlFile);
                Console.WriteLine(@"Not properly terminated with \r\n (cr\lf) but was " + crlf);
            }

            Assert.That(fail, Is.False, "Not all test html files are correctly Marked Of The Web");
        }

        [Test]
        public void ShouldEnsureThatEachCodeFileHasACopyRightHeader()
        {
            var copyRightHeader =
                "#region WatiN Copyright (C) 2006-2009 Jeroen van Menen" + Environment.NewLine +
                "" + Environment.NewLine +
                "//Copyright 2006-2009 Jeroen van Menen" + Environment.NewLine +
                "//" + Environment.NewLine +
                "//   Licensed under the Apache License, Version 2.0 (the \"License\");" + Environment.NewLine +
                "//   you may not use this file except in compliance with the License." + Environment.NewLine +
                "//   You may obtain a copy of the License at" + Environment.NewLine +
                "//" + Environment.NewLine +
                "//       http://www.apache.org/licenses/LICENSE-2.0" + Environment.NewLine +
                "//" + Environment.NewLine +
                "//   Unless required by applicable law or agreed to in writing, software" + Environment.NewLine +
                "//   distributed under the License is distributed on an \"AS IS\" BASIS," + Environment.NewLine +
                "//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied." + Environment.NewLine +
                "//   See the License for the specific language governing permissions and" + Environment.NewLine +
                "//   limitations under the License." + Environment.NewLine +
                "" + Environment.NewLine +
                "#endregion Copyright" + Environment.NewLine;

            Console.WriteLine(copyRightHeader);
            Console.WriteLine();

            var baseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            var codeFiles = Directory.GetFiles(baseDirectory.Parent.Parent.FullName, "*.cs", SearchOption.AllDirectories);
            var fail = false;

            foreach (var codeFile in codeFiles)
            {
                // skip svn files
                if (!codeFile.EndsWith("cs",true,CultureInfo.InvariantCulture) ||
                    codeFile.EndsWith("Resources.Designer.cs", true, CultureInfo.InvariantCulture)) 
                {
                    continue;
                }

                var textfile = File.ReadAllText(codeFile);
                if (textfile.Contains(copyRightHeader)) continue;
                
                fail = true;
                Console.WriteLine(codeFile);
                Console.WriteLine();
            }

            Assert.That(fail, Is.False, "Not all code files have the correct copyright header");
        }

        [Test]
        public void ShouldBePossibleToOverridePublicProperties()
        {
            // GIVEN
            var excludedProperties = new List<string> 
            { 
                "TextField.get_TypeTextAction",
                "TextField.set_TypeTextAction",
                "FileUpload.get_FileName",
                "Control.get_Element",
                "IE.get_InternetExplorer",
                "IE.get_AutoClose",
                "IE.set_AutoClose",
                "IE.get_HtmlDialogs",
                "IE.get_HtmlDialogsNoWait",
                "Page.get_Metadata",
                "Page.get_Document",
                "IE.get_Visible",
                "IE.set_Visible"
            };

            string missedProperties = null;
            
            // WHEN
            foreach (var type in GetTypesToCheck())
            {
                foreach (var propertyInfo in type.GetProperties())
                {
                    var accessors = propertyInfo.GetAccessors(false);
                    if (accessors == null) continue;
                    
                    foreach (var accessor in accessors)
                    {
                        missedProperties = Check(type, accessor, excludedProperties, missedProperties);
                    }
                }
            }

            // THEN
            Assert.That(missedProperties, Is.Null, "Found properties which aren't virtual or excluded");
        }

        [Test]
        public void ShouldBePossibleToOverridePublicMethods()
        {
            // GIVEN
            var excludedMethods = new List<string> 
            { 
                "TextField.TypeText",
                "TextField.AppendText",
                "TextField.Clear",
                "TextField.get_TypeTextAction",
                "TextField.set_TypeTextAction",
                "ElementContainer`1.TableCell",
                "ElementContainer`1.TableCell",
                "DomContainer.AddDialogHandler",
                "DomContainer.RemoveDialogHandler",
                "DomContainer.WaitForComplete",
                "DomContainer.WaitForComplete",
                "Control.get_Element",
                "IE.get_InternetExplorer",
                "IE.Close",
                "IE.Reopen",
                "IE.ClearCookies",
                "IE.ClearCookies",
                "IE.ClearCache",
                "IE.GetCookie",
                "IE.GetCookieContainerForUrl",
                "IE.GetCookiesForUrl",
                "IE.SetCookie",
                "IE.get_AutoClose",
                "IE.set_AutoClose",
                "IE.get_HtmlDialogs",
                "IE.get_HtmlDialogsNoWait",
                "IE.HtmlDialog",
                "IE.HtmlDialog",
                "Page.get_Metadata",
                "Page.get_Document",
                "IE.get_Visible",
                "IE.set_Visible"
            };

            string missedMethods = null;

            // WHEN
            foreach (var type in GetTypesToCheck())
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    missedMethods = Check(type, methodInfo, excludedMethods, missedMethods);
                }
            }

            // THEN
            Assert.That(missedMethods, Is.Null, "Found methods which aren't virtual or excluded");
        }

        private static IEnumerable<Type> GetTypesToCheck()
        {
            var assembly = Assembly.GetAssembly(typeof(Browser));

            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsSubclassOf(typeof (Component)) && !type.IsSubclassOf(typeof (BaseComponentCollection<,>)))
                    continue;
                if (type.IsNotPublic) continue;

                yield return type;
            }
        }

        private static string Check(Type type, MethodBase method, ICollection<string> excludedProperties, string missedProperties)
        {
            if (method.IsVirtual) return missedProperties;
            if (!method.IsPublic) return missedProperties;
            if (method.DeclaringType != type && !method.DeclaringType.IsInterface) return missedProperties;
            if (method.IsAbstract) return missedProperties;
            if (method.IsStatic) return missedProperties;

            var fullName = type.Name + "." + method.Name;
            if (excludedProperties.Contains(fullName)) return missedProperties;

            missedProperties += fullName + Environment.NewLine;
            return missedProperties;
        }

    }
}