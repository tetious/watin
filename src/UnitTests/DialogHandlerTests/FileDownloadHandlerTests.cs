namespace WatiN.Core.UnitTests.DialogHandlerTests
{
  using System.IO;
  using NUnit.Framework;
  using WatiN.Core.DialogHandlers;

  [TestFixture]
  public class FileDownloadHandlerTests
  {
    [Test, Ignore("Because of timeout issues, run this test manually and not automated"), Category("InternetConnectionNeeded")]
    public void DownloadOpen()
    {
      WatiN.Core.DialogHandlers.FileDownloadHandler dhdl = new WatiN.Core.DialogHandlers.FileDownloadHandler(WatiN.Core.DialogHandlers.FileDownloadOptionEnum.Open);

      IE ie = new IE();
      ie.AddDialogHandler(dhdl);
      ie.WaitForComplete();
      ie.GoTo("http://watin.sourceforge.net/WatiNRecorder.zip");

      dhdl.WaitUntilFileDownloadDialogIsHandled(5);
      dhdl.WaitUntilDownloadCompleted(20);
      ie.Close();
    }

    [Test, Ignore("Because of timeout issues, run this test manually and not automated"), Category("InternetConnectionNeeded")]
    public void DownloadSave()
    {
      FileInfo file = new FileInfo(@"c:\temp\test.zip");
      file.Directory.Create();
      file.Delete();

      FileDownloadHandler fileDownloadHandler = new FileDownloadHandler(file.FullName);

      using (IE ie = new IE())
      {
        ie.AddDialogHandler(fileDownloadHandler);

        ie.GoTo("http://watin.sourceforge.net/WatiN-1.0.0.4000-net-1.1.msi");
        //        ie.GoTo("http://watin.sourceforge.net/WatiNRecorder.zip");

        fileDownloadHandler.WaitUntilFileDownloadDialogIsHandled(15);
        fileDownloadHandler.WaitUntilDownloadCompleted(200);
      }

      Assert.IsTrue(file.Exists, file.FullName + " file does not exist after download");
    }

    [Test, Ignore("Because of timeout issues, run this test manually and not automated"), Category("InternetConnectionNeeded")]
    public void DownloadRun()
    {
      WatiN.Core.DialogHandlers.FileDownloadHandler dhdl = new WatiN.Core.DialogHandlers.FileDownloadHandler(WatiN.Core.DialogHandlers.FileDownloadOptionEnum.Run);
      IE ie = new IE();
      ie.AddDialogHandler(dhdl);
      ie.WaitForComplete();
      ie.GoTo("http://watin.sourceforge.net/WatiN-1.0.0.4000-net-1.1.msi");

      dhdl.WaitUntilFileDownloadDialogIsHandled(5);
      dhdl.WaitUntilDownloadCompleted(20);
      ie.Close();
    }
  }
}