namespace WatiN.Core.UnitTests
{
  using System;
  using NUnit.Framework;

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