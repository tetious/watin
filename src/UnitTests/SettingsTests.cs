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

using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Interfaces;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class SettingsTests
	{
        [TearDown]
        public void TearDown()
        {
            Settings.Instance = new DefaultSettings();
        }

		[Test]
		public void Properties()
		{
			Settings.AttachToBrowserTimeOut = 111;
			var autoCloseDialogs = !Settings.AutoCloseDialogs;
			Settings.AutoCloseDialogs = autoCloseDialogs;
			Settings.HighLightColor = "strange color";
			var highLightElement = !Settings.HighLightElement;
			Settings.HighLightElement = highLightElement;
			Settings.WaitForCompleteTimeOut = 222;
			Settings.WaitUntilExistsTimeOut = 333;
		    Settings.SleepTime = 444;

			Assert.AreEqual(111, Settings.AttachToBrowserTimeOut, "Unexpected AttachToBrowserTimeOut");
			Assert.AreEqual(autoCloseDialogs, Settings.AutoCloseDialogs, "Unexpected AutoCloseDialogs");
			Assert.AreEqual("strange color", Settings.HighLightColor, "Unexpected HighLightColor");
			Assert.AreEqual(highLightElement, Settings.HighLightElement, "Unexpected HighLightElement");
			Assert.AreEqual(222, Settings.WaitForCompleteTimeOut, "Unexpected WaitForCompleteTimeOut");
			Assert.AreEqual(333, Settings.WaitUntilExistsTimeOut, "Unexpected WaitUntilExistsTimeOut");
            Assert.AreEqual(444, Settings.SleepTime, "Unexpected SleepTime");
		}

		[Test]
		public void CloneShouldDelegateToInstance()
		{
            // GIVEN
            var settingsMock = new Mock<ISettings>();

            settingsMock.Expect(settings => settings.Clone()).Returns((ISettings) null);
		    Settings.Instance = settingsMock.Object;

            // WHEN
		    Settings.Clone();

            // THEN
            settingsMock.VerifyAll();
        }

	    [Test]
		public void Reset()
		{
            // GIVEN
            var settingsMock = new Mock<ISettings>();

	        settingsMock.Expect(settings => settings.Reset());
            Settings.Instance = settingsMock.Object;

            // WHEN
            Settings.Reset();

            // THEN
            settingsMock.VerifyAll();
        }

		[Test]
		public void IESettingsSetToNullShouldThrowArgumentNullException()
		{
            // GIVEN 
		    var original = Settings.Instance;
		    Assert.That(original, Is.Not.Null, "Expected instance of Settings");
            
            // WHEN
			Settings.Instance = null;

            // THEN
            Assert.That(Settings.Instance, Is.Not.Null, "Expected new instance of settings (1)");
            Assert.That(ReferenceEquals(Settings.Instance, original), Is.False, "Expected new instance of settings (2)");
		}

		[Test]
		public void SetIESettings()
		{
            // GIVEN
			Assert.AreNotEqual(111, Settings.AttachToBrowserTimeOut, "Pre condition failed");

            ISettings settings = new DefaultSettings {AttachToBrowserTimeOut = 111};

            // WHEN
		    Settings.Instance = settings;
			
            // THEN
            Assert.AreEqual(111, Settings.AttachToBrowserTimeOut);
		}
	}
}