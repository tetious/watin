using System.Collections;
using System.IO;
using mshtml;
using WatiN.Core;
using WatiN.Core.DialogHandlers;

namespace WatiN.Core
{
  public class FileUpload : Element
  {
    private static ArrayList elementTags;

    public static ArrayList ElementTags
    {
      get
      {
        if (elementTags == null)
        {
          elementTags = new ArrayList();
          elementTags.Add(new ElementTag("input", "file"));
        }

        return elementTags;
      }
    }

    public FileUpload(DomContainer domContainer, IHTMLInputFileElement inputFileElement): base(domContainer, inputFileElement)
    {}
    
    public FileUpload(DomContainer domContainer, ElementFinder finder): base(domContainer, finder)
    {}

    /// <summary>
    /// Initialises a new instance of the <see cref="FileUpload"/> class based on <paramref name="element"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    public FileUpload(Element element) : base(element, ElementTags)
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
