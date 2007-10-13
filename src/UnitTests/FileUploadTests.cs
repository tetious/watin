namespace WatiN.Core.UnitTests
{
  using System.Collections;
  using System.Text.RegularExpressions;
  using NUnit.Framework;

  [TestFixture]
  public class FileUploadTests : BaseElementsTests
  {
    [Test]
    public void FileUploadElementTags()
    {
      Assert.AreEqual(1, FileUpload.ElementTags.Count, "1 elementtags expected");
      Assert.AreEqual("input", ((ElementTag) FileUpload.ElementTags[0]).TagName);
      Assert.AreEqual("file", ((ElementTag) FileUpload.ElementTags[0]).InputTypes);
    }

    [Test]
    public void CreateFileUploadFromElement()
    {
      Element element = ie.Element("upload");
      FileUpload fileUpload = new FileUpload(element);
      Assert.AreEqual("upload", fileUpload.Id);
    }

    [Test]
    public void FileUploadExists()
    {
      Assert.IsTrue(ie.FileUpload("upload").Exists);
      Assert.IsTrue(ie.FileUpload(new Regex("upload")).Exists);
      Assert.IsFalse(ie.FileUpload("noneexistingupload").Exists);
    }

    [Test]
    public void FileUploadTest()
    {
      FileUpload fileUpload = ie.FileUpload("upload");

      Assert.IsNotNull(fileUpload);
      Assert.IsNull(fileUpload.FileName);

      fileUpload.Set(MainURI.LocalPath);

      Assert.AreEqual(MainURI.LocalPath, fileUpload.FileName);
    }

    [Test, ExpectedException(typeof (System.IO.FileNotFoundException))]
    public void FileUploadFileNotFoundException()
    {
      FileUpload fileUpload = ie.FileUpload("upload");
      fileUpload.Set("nonexistingfile.nef");
    }

    [Test]
    public void FileUploads()
    {
      const int expectedFileUploadsCount = 1;
      Assert.AreEqual(expectedFileUploadsCount, ie.FileUploads.Length, "Unexpected number of FileUploads");

      // Collection.Length
      FileUploadCollection formFileUploads = ie.FileUploads;

      // Collection items by index
      Assert.AreEqual("upload", ie.FileUploads[0].Id);

      IEnumerable FileUploadEnumerable = formFileUploads;
      IEnumerator FileUploadEnumerator = FileUploadEnumerable.GetEnumerator();

      // Collection iteration and comparing the result with Enumerator
      int count = 0;
      foreach (FileUpload inputFileUpload in formFileUploads)
      {
        FileUploadEnumerator.MoveNext();
        object enumFileUpload = FileUploadEnumerator.Current;

        Assert.IsInstanceOfType(inputFileUpload.GetType(), enumFileUpload, "Types are not the same");
        Assert.AreEqual(inputFileUpload.OuterHtml, ((FileUpload) enumFileUpload).OuterHtml, "foreach and IEnumator don't act the same.");
        ++count;
      }

      Assert.IsFalse(FileUploadEnumerator.MoveNext(), "Expected last item");
      Assert.AreEqual(expectedFileUploadsCount, count);
    }
  }
}