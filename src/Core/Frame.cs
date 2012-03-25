#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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
using WatiN.Core.Native;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a Frame or IFrame.
	/// </summary>
    [ElementTag("frame", Index = 0)]
    [ElementTag("iframe", Index = 1)]
    public class Frame : Document
	{
	    public Element FrameElement { get; private set; }
	    private readonly INativeDocument _frameDocument;

	    /// <summary>
	    /// This constructor will mainly be used by the constructor of FrameCollection
	    /// to create an instance of a Frame.
	    /// </summary>
	    /// <param name="domContainer">The domContainer</param>
	    /// <param name="frameDocument">The document within the frame</param>
	    /// <param name="parentDocument"> </param>
	    public Frame(DomContainer domContainer, INativeDocument frameDocument, Frame parentDocument)
            : base(domContainer)
		{
	        if (frameDocument == null)
                throw new ArgumentNullException("frameDocument");

            _frameDocument = frameDocument;
            FrameElement = CreateFrameElement(domContainer, frameDocument);

            SetFrameHierarchy(parentDocument);
		}

	    /// <summary>
	    /// This is done to facilitate CSS selector look up in IEElementCollection
	    /// </summary>
	    /// <param name="parentDocument"> </param>
	    private void SetFrameHierarchy(Frame parentDocument)
	    {
            if (parentDocument == null) return;

	        var frameElement = parentDocument.FrameElement;
	        var nameOrId = GetFrameElementNameOrId(frameElement);
	        var hierarchy = frameElement.GetAttributeValue("data-watinFrameHierarchy") + nameOrId + ".";
            
            FrameElement.SetAttributeValue("data-watinFrameHierarchy", hierarchy);
	    }

	    private static string GetFrameElementNameOrId(Element frameElement)
	    {
	        var frameRef = frameElement.Name;
	        
            if (string.IsNullOrEmpty(frameRef))
                frameRef = frameElement.Id;

	        if (string.IsNullOrEmpty(frameRef))
	        {
	            frameRef = frameElement.GetJavascriptElementReference();
	            frameElement.SetAttributeValue("Id", "frame_" + frameRef);
	        }
	        
            return frameRef;
	    }

	    private static Element CreateFrameElement(DomContainer domContainer, INativeDocument frameDocument)
	    {
	        return new Element(domContainer, frameDocument.ContainingFrameElement);
	    }

	    /// <inheritdoc />
        public override INativeDocument NativeDocument
        {
            get { return _frameDocument; }
        }

        public virtual string Name
		{
			get { return GetAttributeValue("name"); }
		}

        public virtual string Id
		{
			get { return GetAttributeValue("id"); }
		}

	    /// <inheritdoc />
        protected override string GetAttributeValueImpl(string attributeName)
        {
            switch (attributeName.ToLowerInvariant())
            {
                case "url":
                    return Url;

                case "href":
                    return Url;

                default:
                    return FrameElement.GetAttributeValue(attributeName);
            }
        }

        public virtual void SetAttributeValue(string attributeName, string value)
        {
            FrameElement.SetAttributeValue(attributeName, value);
        }

	}
}
