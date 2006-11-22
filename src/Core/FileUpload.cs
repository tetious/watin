using System.IO;
using mshtml;
using WatiN.Core;
using WatiN.Core.DialogHandlers;

namespace WatiN.Core
{
  public class FileUpload : Element
  {
    public FileUpload(DomContainer domContainer, IHTMLInputFileElement inputFileElement): base(domContainer, inputFileElement)
    {}
    
    public FileUpload(DomContainer domContainer, ElementFinder finder): base(domContainer, finder)
    {}

    public string FileName
    {
      get
      {
        return IHTMLInputFileElement.value;
      }
    }

    public void Set(string fileName)
    { 
      
      FileInfo info = new FileInfo(fileName);
      if (!info.Exists)
      {
        throw new FileNotFoundException("File does not exist", fileName);
      }

      FileUploadDialogHandler uploadDialogHandler = new FileUploadDialogHandler(fileName);
      DomContainer.AddDialogHandler(uploadDialogHandler);

      try
      {
        Click();
      }
      finally
      {
        DomContainer.RemoveDialogHandler(uploadDialogHandler);
      }      
    }

    private IHTMLInputFileElement IHTMLInputFileElement
    {
      get { return ((IHTMLInputFileElement) HTMLElement); }
    }
  }
}
