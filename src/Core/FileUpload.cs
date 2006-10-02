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
      
      DomContainer.DialogWatcher.Add(new FileUploadDialogHandler(fileName));
     
      Click();
      
      DomContainer.DialogWatcher.Remove(new FileUploadDialogHandler(fileName));
    }

    private IHTMLInputFileElement IHTMLInputFileElement
    {
      get { return ((IHTMLInputFileElement) DomElement); }
    }
  }
}