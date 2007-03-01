#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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

    public bool Exists(Attribute findBy)
    {
      foreach (Frame frame in elements)
      {
        if (findBy.Compare(frame))
        {
          // Return
          return true;
        }
      }
      
      return false;
    }
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
