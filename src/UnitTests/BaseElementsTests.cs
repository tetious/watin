namespace WatiN.Core.UnitTests
{
  using NUnit.Framework;

  public class BaseElementsTests : WatiNTest
  {
    protected IE ie;
    private Settings backupSettings;

    [TestFixtureSetUp]
    public void FixtureSetup()
    {
      backupSettings = IE.Settings.Clone();
      IE.Settings = new StealthSettings();
      
      ie = new IE(MainURI);
    }

    [TestFixtureTearDown]
    public void FixtureTearDown()
    {
      IE.Settings = backupSettings;
      ie.Close();
    }

    [SetUp]
    public void TestSetUp()
    {
      IE.Settings.Reset();
      if (!ie.Uri.Equals(MainURI))
      {
        ie.GoTo(MainURI);
      }
    }
  }

  public class StealthSettings : Settings
  {
    public StealthSettings() : base()
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