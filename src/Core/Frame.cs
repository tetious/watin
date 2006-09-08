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

using System.Runtime.InteropServices;

using mshtml;
using SHDocVw;

using WatiN.Core.Exceptions;

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
      IWebBrowser2 result = null;
      
      NativeMethods.IEnumUnknown eu;
      NativeMethods.IOleContainer oc = htmlDocument as NativeMethods.IOleContainer;
      int hr = oc.EnumObjects(NativeMethods.tagOLECONTF.OLECONTF_EMBEDDINGS, out eu);
      Marshal.ThrowExceptionForHR(hr);

      try
      {
        object pUnk;
        int fetched;
        const int MAX_FETCH_COUNT = 1;

        // get the first embedded object
        // pUnk alloc
        hr = eu.Next(MAX_FETCH_COUNT, out pUnk, out fetched);
        Marshal.ThrowExceptionForHR(hr);

        int index = 0;
        while (hr == 0)
        {
          // Query Interface pUnk for the IWebBrowser2 interface
          IWebBrowser2 brow = pUnk as IWebBrowser2;

          if (brow != null)
          {
            if (index++ == frameIndex)
            {
              result = brow;
              break;
            }
          }

          // pUnk free
          Marshal.ReleaseComObject(brow);

          // get the next embedded object
          // pUnk alloc
          hr = eu.Next(MAX_FETCH_COUNT, out pUnk, out fetched);
          Marshal.ThrowExceptionForHR(hr);
        }
      }
      finally
      {
        // eu free
        Marshal.ReleaseComObject(eu);
      }

      return result;
    }

    private void Init(string name, string id)
    {
      frameName = name;
      frameId = id;
    }
  }
}