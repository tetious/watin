#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class SettingsTests
	{
		[Test]
		public void Properties()
		{
			Settings settings = new Settings();

			settings.AttachToIETimeOut = 111;
			bool autoCloseDialogs = !settings.AutoCloseDialogs;
			settings.AutoCloseDialogs = autoCloseDialogs;
			settings.HighLightColor = "strange color";
			bool highLightElement = !settings.HighLightElement;
			settings.HighLightElement = highLightElement;
			settings.WaitForCompleteTimeOut = 222;
			settings.WaitUntilExistsTimeOut = 333;

			Assert.AreEqual(111, settings.AttachToIETimeOut, "Unexpected AttachToIETimeOut");
			Assert.AreEqual(autoCloseDialogs, settings.AutoCloseDialogs, "Unexpected AutoCloseDialogs");
			Assert.AreEqual("strange color", settings.HighLightColor, "Unexpected HighLightColor");
			Assert.AreEqual(highLightElement, settings.HighLightElement, "Unexpected HighLightElement");
			Assert.AreEqual(222, settings.WaitForCompleteTimeOut, "Unexpected WaitForCompleteTimeOut");
			Assert.AreEqual(333, settings.WaitUntilExistsTimeOut, "Unexpected WaitUntilExistsTimeOut");
		}

		[Test]
		public void Clone()
		{
			Settings settings = new Settings();

			settings.AttachToIETimeOut = 111;
			bool autoCloseDialogs = !settings.AutoCloseDialogs;
			settings.AutoCloseDialogs = autoCloseDialogs;
			settings.HighLightColor = "strange color";
			bool highLightElement = !settings.HighLightElement;
			settings.HighLightElement = highLightElement;
			settings.WaitForCompleteTimeOut = 222;
			settings.WaitUntilExistsTimeOut = 333;

			Settings settingsClone = settings.Clone();
			Assert.AreEqual(111, settingsClone.AttachToIETimeOut, "Unexpected AttachToIETimeOut");
			Assert.AreEqual(autoCloseDialogs, settingsClone.AutoCloseDialogs, "Unexpected AutoCloseDialogs");
			Assert.AreEqual("strange color", settingsClone.HighLightColor, "Unexpected HighLightColor");
			Assert.AreEqual(highLightElement, settingsClone.HighLightElement, "Unexpected HighLightElement");
			Assert.AreEqual(222, settingsClone.WaitForCompleteTimeOut, "Unexpected WaitForCompleteTimeOut");
			Assert.AreEqual(333, settingsClone.WaitUntilExistsTimeOut, "Unexpected WaitUntilExistsTimeOut");
		}

		[Test]
		public void Defaults()
		{
			Settings settings = new Settings();

			AssertDefaults(settings);
		}

		[Test]
		public void Reset()
		{
			Settings settings = new Settings();

			settings.AttachToIETimeOut = 111;
			bool autoCloseDialogs = !settings.AutoCloseDialogs;
			settings.AutoCloseDialogs = autoCloseDialogs;
			settings.HighLightColor = "strange color";
			bool highLightElement = !settings.HighLightElement;
			settings.HighLightElement = highLightElement;
			settings.WaitForCompleteTimeOut = 222;
			settings.WaitUntilExistsTimeOut = 333;

			Settings settingsClone = settings.Clone();
			Assert.AreEqual(111, settingsClone.AttachToIETimeOut, "Unexpected AttachToIETimeOut");
			Assert.AreEqual(autoCloseDialogs, settingsClone.AutoCloseDialogs, "Unexpected AutoCloseDialogs");
			Assert.AreEqual("strange color", settingsClone.HighLightColor, "Unexpected HighLightColor");
			Assert.AreEqual(highLightElement, settingsClone.HighLightElement, "Unexpected HighLightElement");
			Assert.AreEqual(222, settingsClone.WaitForCompleteTimeOut, "Unexpected WaitForCompleteTimeOut");
			Assert.AreEqual(333, settingsClone.WaitUntilExistsTimeOut, "Unexpected WaitUntilExistsTimeOut");

			settingsClone.Reset();
			AssertDefaults(settingsClone);
		}

		[Test]
		public void ChangeSettingInCloneShouldNotChangeOriginalSetting()
		{
			Settings settings = new Settings();

			settings.AttachToIETimeOut = 111;

			Settings settingsClone = settings.Clone();
			Assert.AreEqual(111, settingsClone.AttachToIETimeOut, "Unexpected clone 1");

			settingsClone.AttachToIETimeOut = 222;

			Assert.AreEqual(111, settings.AttachToIETimeOut, "Unexpected original");
			Assert.AreEqual(222, settingsClone.AttachToIETimeOut, "Unexpected clone 2");
		}

		private static void AssertDefaults(Settings settings)
		{
			Assert.AreEqual(30, settings.AttachToIETimeOut, "Unexpected AttachToIETimeOut");
			Assert.AreEqual(true, settings.AutoCloseDialogs, "Unexpected AutoCloseDialogs");
			Assert.AreEqual("yellow", settings.HighLightColor, "Unexpected HighLightColor");
			Assert.AreEqual(true, settings.HighLightElement, "Unexpected HighLightElement");
			Assert.AreEqual(30, settings.WaitForCompleteTimeOut, "Unexpected WaitForCompleteTimeOut");
			Assert.AreEqual(30, settings.WaitUntilExistsTimeOut, "Unexpected WaitUntilExistsTimeOut");
		}

		[Test, ExpectedException(typeof (ArgumentNullException))]
		public void IESettingsSetToNullShouldThrowArgumentNullException()
		{
			IE.Settings = null;
		}

		[Test]
		public void SetIESettings()
		{
			Settings settings = new Settings();
			Assert.AreNotEqual(111, IE.Settings.AttachToIETimeOut);
			settings.AttachToIETimeOut = 111;

			IE.Settings = settings;
			Assert.AreEqual(111, IE.Settings.AttachToIETimeOut);
		}
	}
}