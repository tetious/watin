namespace WatiN.Core.UnitTests.DialogHandlerTests
{
  using System;
  using NUnit.Framework;
  using WatiN.Core.DialogHandlers;

  [TestFixture]
  public class LogonDialogHandlerTests
  {
    [Test, Ignore()]
    public void LogonDialogTest()
    {
      using (IE ie = new IE())
      {
        LogonDialogHandler logonDialogHandler = new LogonDialogHandler(@"username", "password");
        using (new UseDialogOnce(ie.DialogWatcher, logonDialogHandler))
        {
          ie.GoTo("https://www.somesecuresite.com");
        }
      }
    }

    [Test, ExpectedException(typeof (ArgumentNullException))]
    public void LogonDialogWithUserNameNullShouldThrowArgumentNullException()
    {
      new LogonDialogHandler(null, "pwd");
    }

    [Test, ExpectedException(typeof (ArgumentNullException))]
    public void LogonDialogWithUserNameStringEmptyShouldThrowArgumentNullException()
    {
      new LogonDialogHandler(String.Empty, "pwd");
    }

    [Test]
    public void LogonDialogValidConstructorArguments()
    {
      new LogonDialogHandler("username", "pwd");
      new LogonDialogHandler("username", "");
      new LogonDialogHandler("username", null);
    }
  }
}