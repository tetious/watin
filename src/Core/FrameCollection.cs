#region WatiN Copyright (C) 2006 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006 Jeroen van Menen
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

using mshtml;
using SHDocVw;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
  /// <summary>
  /// A typed collection of <see cref="Frame" /> instances within a <see cref="Document"/>.
  /// </summary>
  public class FrameCollection : IEnumerable
  {
    ArrayList elements;
		
    public FrameCollection(DomContainer ie, IHTMLDocument2 htmlDocument) 
    {
      AllFramesProcessor processor = new AllFramesProcessor(ie, (HTMLDocument)htmlDocument);
      
      NativeMethods.EnumIWebBrowser2Interfaces(processor);
      
      elements = processor.elements;
    }

    public int Length { get { return elements.Count; } }

    public Frame this[int index] { get { return (Frame)elements[index]; } }

    /// <exclude />
    public Enumerator GetEnumerator() 
    {
      return new Enumerator(elements);
    }

    IEnumerator IEnumerable.GetEnumerator() 
    {
      return GetEnumerator();
    }

    /// <exclude />
    public class Enumerator: IEnumerator 
    {
      ArrayList children;
      int index;
      public Enumerator(ArrayList children) 
      {
        this.children = children;
        Reset();
      }

      public void Reset() 
      {
        index = -1;
      }

      public bool MoveNext() 
      {
        ++index;
        return index < children.Count;
      }

      public Frame Current 
      {
        get 
        {
          return (Frame)children[index];
        }
      }

      object IEnumerator.Current { get { return Current; } }
    }
  }
  
  internal class AllFramesProcessor : IWebBrowser2Processor
  {
    public ArrayList elements;
    
    private HTMLDocument htmlDocument;
    private IHTMLElementCollection frameElements;
    private int index = 0;
    private DomContainer ie;
    
    public AllFramesProcessor(DomContainer ie, HTMLDocument htmlDocument)
    {
      elements = new ArrayList();

      frameElements = (IHTMLElementCollection)htmlDocument.all.tags(ElementsSupport.FrameTagName);
      
      // If the current document doesn't contain FRAME elements, it then
      // might contain IFRAME elements.
      if (frameElements.length == 0)
      {
        frameElements = (IHTMLElementCollection)htmlDocument.all.tags("IFRAME");
      }

      this.ie = ie;
      this.htmlDocument = htmlDocument;  
    }

    public HTMLDocument HTMLDocument()
    {
      return htmlDocument;
    }

    public void Process(IWebBrowser2 webBrowser2)
    {
      // Get the frame element from the parent document
      IHTMLElement frameElement = (IHTMLElement)frameElements.item(index, null);
            
      string frameName = null;
      string frameId = null;

      if (frameElement != null)
      {
        frameId = frameElement.id;
        frameName = frameElement.getAttribute("name", 0) as string;
      }

      Frame frame = new Frame(ie, webBrowser2.Document as IHTMLDocument2, frameName, frameId);
      elements.Add(frame);
                
      index++;
    }

    public bool Continue()
    {
      return true;
    }
  }
}
