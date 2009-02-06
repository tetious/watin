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

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests
{
    [TestFixture]
    public class DefaultSettingsTests
    {
        [Test]
        public void Properties()
        {
            ISettings settings = new DefaultSettings {AttachToIETimeOut = 111};

            var autoCloseDialogs = !settings.AutoCloseDialogs;
            settings.AutoCloseDialogs = autoCloseDialogs;
            settings.HighLightColor = "strange color";
            var highLightElement = !settings.HighLightElement;
            settings.HighLightElement = highLightElement;
            settings.WaitForCompleteTimeOut = 222;
            settings.WaitUntilExistsTimeOut = 333;
            settings.SleepTime = 444;

            Assert.AreEqual(111, settings.AttachToIETimeOut, "Unexpected AttachToIETimeOut");
            Assert.AreEqual(autoCloseDialogs, settings.AutoCloseDialogs, "Unexpected AutoCloseDialogs");
            Assert.AreEqual("strange color", settings.HighLightColor, "Unexpected HighLightColor");
            Assert.AreEqual(highLightElement, settings.HighLightElement, "Unexpected HighLightElement");
            Assert.AreEqual(222, settings.WaitForCompleteTimeOut, "Unexpected WaitForCompleteTimeOut");
            Assert.AreEqual(333, settings.WaitUntilExistsTimeOut, "Unexpected WaitUntilExistsTimeOut");
            Assert.AreEqual(444, settings.SleepTime, "Unexpected SleepTime");
        }

        [Test]
        public void Clone()
        {
            ISettings settings = new DefaultSettings();

            settings.AttachToIETimeOut = 111;
            bool autoCloseDialogs = !settings.AutoCloseDialogs;
            settings.AutoCloseDialogs = autoCloseDialogs;
            settings.HighLightColor = "strange color";
            bool highLightElement = !settings.HighLightElement;
            settings.HighLightElement = highLightElement;
            settings.WaitForCompleteTimeOut = 222;
            settings.WaitUntilExistsTimeOut = 333;
            settings.SleepTime = 444;

            ISettings settingsClone = settings.Clone();
            Assert.AreEqual(111, settingsClone.AttachToIETimeOut, "Unexpected AttachToIETimeOut");
            Assert.AreEqual(autoCloseDialogs, settingsClone.AutoCloseDialogs, "Unexpected AutoCloseDialogs");
            Assert.AreEqual("strange color", settingsClone.HighLightColor, "Unexpected HighLightColor");
            Assert.AreEqual(highLightElement, settingsClone.HighLightElement, "Unexpected HighLightElement");
            Assert.AreEqual(222, settingsClone.WaitForCompleteTimeOut, "Unexpected WaitForCompleteTimeOut");
            Assert.AreEqual(333, settingsClone.WaitUntilExistsTimeOut, "Unexpected WaitUntilExistsTimeOut");
            Assert.AreEqual(444, settingsClone.SleepTime, "Unexpected SleepTime");
        }

        [Test]
        public void Defaults()
        {
            ISettings settings = new DefaultSettings();

            AssertDefaults(settings);
        }

        [Test]
        public void Reset()
        {
            ISettings settings = new DefaultSettings();

            settings.AttachToIETimeOut = 111;
            bool autoCloseDialogs = !settings.AutoCloseDialogs;
            settings.AutoCloseDialogs = autoCloseDialogs;
            settings.HighLightColor = "strange color";
            bool highLightElement = !settings.HighLightElement;
            settings.HighLightElement = highLightElement;
            settings.WaitForCompleteTimeOut = 222;
            settings.WaitUntilExistsTimeOut = 333;
            settings.SleepTime = 444;

            ISettings settingsClone = settings.Clone();
            Assert.AreEqual(111, settingsClone.AttachToIETimeOut, "Unexpected AttachToIETimeOut");
            Assert.AreEqual(autoCloseDialogs, settingsClone.AutoCloseDialogs, "Unexpected AutoCloseDialogs");
            Assert.AreEqual("strange color", settingsClone.HighLightColor, "Unexpected HighLightColor");
            Assert.AreEqual(highLightElement, settingsClone.HighLightElement, "Unexpected HighLightElement");
            Assert.AreEqual(222, settingsClone.WaitForCompleteTimeOut, "Unexpected WaitForCompleteTimeOut");
            Assert.AreEqual(333, settingsClone.WaitUntilExistsTimeOut, "Unexpected WaitUntilExistsTimeOut");
            Assert.AreEqual(444, settingsClone.SleepTime, "Unexpected SleepTime");

            settingsClone.Reset();
            AssertDefaults(settingsClone);
        }

        [Test]
        public void ChangeSettingInCloneShouldNotChangeOriginalSetting()
        {
            ISettings settings = new DefaultSettings();

            settings.AttachToIETimeOut = 111;

            ISettings settingsClone = settings.Clone();
            Assert.AreEqual(111, settingsClone.AttachToIETimeOut, "Unexpected clone 1");

            settingsClone.AttachToIETimeOut = 222;

            Assert.AreEqual(111, settings.AttachToIETimeOut, "Unexpected original");
            Assert.AreEqual(222, settingsClone.AttachToIETimeOut, "Unexpected clone 2");
        }

        private static void AssertDefaults(ISettings settings)
        {
            Assert.AreEqual(30, settings.AttachToIETimeOut, "Unexpected AttachToIETimeOut");
            Assert.AreEqual(true, settings.AutoCloseDialogs, "Unexpected AutoCloseDialogs");
            Assert.AreEqual("yellow", settings.HighLightColor, "Unexpected HighLightColor");
            Assert.AreEqual(true, settings.HighLightElement, "Unexpected HighLightElement");
            Assert.AreEqual(30, settings.WaitForCompleteTimeOut, "Unexpected WaitForCompleteTimeOut");
            Assert.AreEqual(30, settings.WaitUntilExistsTimeOut, "Unexpected WaitUntilExistsTimeOut");
            Assert.AreEqual(100, settings.SleepTime, "Unexpected SleepTime");
        }
    }
}