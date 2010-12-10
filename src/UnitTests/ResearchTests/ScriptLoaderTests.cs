using System;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Properties;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.ResearchTests
{
    [TestFixture]
    public class ScriptLoaderTests : BaseWithBrowserTests
    {
        [Test]
        public void Should_return_sizzlejs_install_script()
        {
            // GIVEN
            var scriptLoader = new ScriptLoader();

            // WHEN
            var script = scriptLoader.GetInstallScript();

            // THEN
            Console.WriteLine(script);
            Assert.That(script, Is.Not.Null, "expected script");
            Assert.That(script, Text.Contains("Sizzle"), "expected Sizzle function");
        }

        [Test]
        public void Should_inject_script_sucessfully()
        {
            ExecuteTest(browser =>
                            {
                                // GIVEN
                                var scriptLoader = new ScriptLoader();

                                // THEN
                                browser.RunScript(scriptLoader.GetInstallScript());

                                // WHEN
                                var eval = browser.Eval("window.Sizzle('#popupid').length;");
                                Assert.That(eval, Is.EqualTo("1"));
                            });
        }

        public override Uri TestPageUri
        {
            get { return MainURI; }
        }
    }

    public class ScriptLoader
    {
        public string GetInstallScript()
        {
            var result = new StringBuilder();

            result.Append("with(window) { if (typeof Sizzle == 'undefined') {");
            result.Append(Resources.sizzle); 
            result.Append("};};"); 
            
            return result.ToString();
        } 
    }
}
