#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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

using System;
using System.Globalization;
using mshtml;
using SHDocVw;
using WatiN.Core.Constraints;
using StringComparer = WatiN.Core.Comparers.StringComparer;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a Frame or IFrame.
	/// </summary>
	public class Frame : Document, IAttributeBag
	{
		private IHTMLDocument3 _frameSetParent;
		private string _frameElementUniqueId;
		private Element frameElement;

		/// <summary>
		/// This constructor will mainly be used by the constructor of FrameCollection
		/// to create an instance of a Frame.
		/// </summary>
		/// <param name="domContainer">The domContainer.</param>
		/// <param name="htmlDocument">The HTML document.</param>
		/// <param name="frameSetParent">The frame set parent.</param>
		/// <param name="frameElementUniqueId">The frame element unique id.</param>
		public Frame(DomContainer domContainer, IHTMLDocument2 htmlDocument, IHTMLDocument3 frameSetParent, string frameElementUniqueId) : base(domContainer, htmlDocument)
		{
			_frameSetParent = frameSetParent;
			_frameElementUniqueId = frameElementUniqueId;
		}

		/// <summary>
		/// This constructor will mainly be used by Document.Frame to find
		/// a Frame. A FrameNotFoundException will be thrown if the Frame isn't found.
		/// </summary>
		/// <param name="frames">Collection of frames to find the frame in</param>
		/// <param name="findBy">The <see cref="AttributeConstraint"/> of the Frame to find (Find.ByUrl, Find.ByName and Find.ById are supported)</param>
		public static Frame Find(FrameCollection frames, BaseConstraint findBy)
		{
			return findFrame(frames, findBy);
		}

		private static Frame findFrame(FrameCollection frames, BaseConstraint findBy)
		{
			foreach (Frame frame in frames)
			{
				if (findBy.Compare(frame))
				{
					// Return
					return frame;
				}
			}

			throw new FrameNotFoundException(findBy.ConstraintToString());
		}

		public string Name
		{
			get { return GetAttributeValue("name"); }
		}

		public string Id
		{
			get { return GetAttributeValue("id"); }
		}

		public string GetAttributeValue(string attributename)
		{
			if(frameElement == null)
			{
				IHTMLElement2 element = GetFrameElement("FRAME");
				if(element == null)			
				{
					element = GetFrameElement("IFRAME");
				}

				if (element == null)
				{
					throw new WatiNException("element shouldn't be null");
				}

				frameElement = new Element(DomContainer, DomContainer.NativeBrowser.CreateElement(element));
			}	
			return frameElement.GetAttributeValue(attributename);
		}

		private IHTMLElement2 GetFrameElement(string tagname) 
		{
			IHTMLElementCollection elements = _frameSetParent.getElementsByTagName(tagname);

			foreach (DispHTMLBaseElement element in elements)
			{
				if (StringComparer.AreEqual(element.uniqueID, _frameElementUniqueId, true))
				{
					return (IHTMLElement2)element;
				}
			}
			return null;
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

		#region IAttributeBag Members

		public string GetValue(string attributename)
		{
			switch (attributename.ToLower(CultureInfo.InvariantCulture))
			{
				case "url":
					return Url;
				
				case "href":
					return Url;

				default:
					return GetAttributeValue(attributename);
			}
		}

		#endregion
	}
}
