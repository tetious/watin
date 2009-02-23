#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
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

using System.Collections.Generic;
using WatiN.Core.Interfaces;
using WatiN.Core.Native;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a Frame or IFrame.
	/// </summary>
    [ElementTag("frame", Index = 0)]
    [ElementTag("iframe", Index = 1)]
    public sealed class Frame : Document, IAttributeBag
	{
		private readonly Element _frameElement;

		/// <summary>
		/// This constructor will mainly be used by the constructor of FrameCollection
		/// to create an instance of a Frame.
		/// </summary>
		/// <param name="domContainer">The domContainer.</param>
		/// <param name="frameDocument">The HTML document.</param>
		/// <param name="frameElement"></param>
		public Frame(DomContainer domContainer, INativeDocument frameDocument, Element frameElement) : base(domContainer, frameDocument)
		{
		    _frameElement = frameElement;
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
            switch (attributename.ToLowerInvariant())
            {
                case "url":
                    return Url;

                case "href":
                    return Url;

                default:
                    return _frameElement.GetAttributeValue(attributename);
            }
		}

	    public string GetValue(string attributename)
		{
			return GetAttributeValue(attributename);
		}
	}
}
