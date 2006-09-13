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

using mshtml;
using SHDocVw;

using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
  /// <summary>
  /// This class provides specialized functionality for a Frame or IFrame.
  /// </summary>
  public class Frame : Document
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
    /// <param name="findBy">The name of the frame</param>
    public static Frame Find(FrameCollection frames, Name findBy)
    {
      return findFrame(frames, findBy);
    }
    /// <summary>
    /// This constructor will mainly be used by Document.Frame to find
    /// a Frame. A FrameNotFoundException will be thrown if the Frame isn't found.
    /// </summary>
    /// <param name="frames">Collection of frames to find the frame in</param>
    /// <param name="findBy">The Url of the Frame html page</param>
    public static Frame Find(FrameCollection frames, Url findBy)
    {
      return findFrame(frames, findBy);
    }
    /// <summary>
    /// This constructor will mainly be used by Document.Frame to find
    /// a Frame. A FrameNotFoundException will be thrown if the Frame isn't found.
    /// </summary>
    /// <param name="frames">Collection of frames to find the frame in</param>
    /// <param name="findBy">The Id of the Frame</param>
    public static Frame Find(FrameCollection frames, Id findBy)
    {
      return findFrame(frames, findBy);
    }

    private static Frame findFrame(FrameCollection frames, Attribute findBy)
    {
      foreach (Frame frame in frames)
      {
        string compareValue = string.Empty;

        if (findBy is Name)
        {
          compareValue = frame.Name;
        }

        else if(findBy is Url)
        {
          compareValue = frame.Url;
        }

        else if(findBy is Id)
        {
          compareValue = frame.Id;
        }

        if (findBy.Compare(compareValue))
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
  }
    
  internal class FrameByIndexProcessor :IProcessIWebBrowser2
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
}