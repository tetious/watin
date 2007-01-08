#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006-2007 Jeroen van Menen
//
// This library is free software; you can redistribute it and/or modify it under the terms of the GNU 
// Lesser General Public License as published by the Free Software Foundation; either version 2.1 of 
// the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with this library; 
// if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 
// 02111-1307 USA 

#endregion Copyright

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
