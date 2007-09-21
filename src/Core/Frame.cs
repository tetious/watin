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

using mshtml;
using SHDocVw;

using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
  /// <summary>
  /// This class provides specialized functionality for a Frame or IFrame.
  /// </summary>
  public class Frame : Document, IAttributeBag
  {
    private string frameName = string.Empty;
    private string frameId = string.Empty;

    /// <summary>
    /// This constructor will mainly be used by the constructor of FrameCollection
    /// to create an instance of a Frame.
    /// </summary>
    /// <param name="ie"></param>
    /// <param name="htmlDocument"></param>
    /// <param name="name"></param>
    /// <param name="id"></param>
    public Frame(DomContainer ie, IHTMLDocument2 htmlDocument, string name, string id) : base(ie, htmlDocument)
    {
      Init(name, id);
    }

    /// <summary>
    /// This constructor will mainly be used by Document.Frame to find
    /// a Frame. A FrameNotFoundException will be thrown if the Frame isn't found.
    /// </summary>
    /// <param name="frames">Collection of frames to find the frame in</param>
    /// <param name="findBy">The <see cref="AttributeConstraint"/> of the Frame to find (Find.ByUrl, Find.ByName and Find.ById are supported)</param>
    public static Frame Find(FrameCollection frames, AttributeConstraint findBy)
    {
      return findFrame(frames, findBy);
    }

    private static Frame findFrame(FrameCollection frames, AttributeConstraint findBy)
    {
      foreach (Frame frame in frames)
      {
        if (findBy.Compare(frame))
        {
          // Return
          return frame;
        }
      }

      throw new FrameNotFoundException(findBy.AttributeName, findBy.Value);
    }

    public string Name
    {
      get { return frameName; }
    }

    public string Id
    {
      get { return frameId; }
    }

    internal static int GetFrameCountFromHTMLDocument(HTMLDocument htmlDocument)
    {
      FrameCountProcessor processor = new FrameCountProcessor(htmlDocument);
      
      NativeMethods.EnumIWebBrowser2Interfaces(processor);
      
      return processor.FramesCount;
    }

    internal static IWebBrowser2 GetFrameFromHTMLDocument(int frameIndex, HTMLDocument htmlDocument)
    {
      FrameByIndexProcessor processor = new FrameByIndexProcessor(frameIndex, htmlDocument);
      
      NativeMethods.EnumIWebBrowser2Interfaces(processor);
      
      return processor.IWebBrowser2();
    }

    private void Init(string name, string id)
    {
      frameName = name;
      frameId = id;
    }
    #region IAttributeBag Members

    public string GetValue(string attributename)
    {
      switch(attributename.ToLower())
      {
        case "name":
          return Name;
        case "url":
          return Url;
        case "href":
          return Url;
        case "id":
          return Id;
        default:
          throw new InvalidAttributException(attributename, "Frame or IFrame");
      }
    }

    
    #endregion
  }
    
  internal class FrameByIndexProcessor :IWebBrowser2Processor
  {
    private HTMLDocument htmlDocument;
    private int index;
    private int counter = 0;
    private IWebBrowser2 iWebBrowser2 = null;
    
    public FrameByIndexProcessor(int index, HTMLDocument htmlDocument)
    {
      this.index = index;
      this.htmlDocument = htmlDocument;  
    }

    public HTMLDocument HTMLDocument()
    {
      return htmlDocument;
    }

    public void Process(IWebBrowser2 webBrowser2)
    {
      if (counter == index)
      {
        iWebBrowser2 = webBrowser2;
      }
      counter++;
    }

    public bool Continue()
    {
      return (iWebBrowser2 == null);
    }
    
    public IWebBrowser2 IWebBrowser2()
    {
      return iWebBrowser2;
    }
  }
  
  internal class FrameCountProcessor :IWebBrowser2Processor
  {
    private HTMLDocument htmlDocument;
    private int counter = 0;
    
    public FrameCountProcessor(HTMLDocument htmlDocument)
    {
      this.htmlDocument = htmlDocument;  
    }

    public int FramesCount
    {
      get { return counter; }
    }

    public HTMLDocument HTMLDocument()
    {
      return htmlDocument;
    }

    public void Process(IWebBrowser2 webBrowser2)
    {
      counter++;
    }

    public bool Continue()
    {
      return true;
    }
  }
}