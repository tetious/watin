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

using System.Collections;
using mshtml;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML link element.
	/// </summary>
#if NET11
	public class Link : Element
#else
    public class Link : Element<Link>
#endif 
	{
		private static ArrayList elementTags;

		public static ArrayList ElementTags
		{
			get
			{
				if (elementTags == null)
				{
					elementTags = new ArrayList();
					elementTags.Add(new ElementTag("a"));
				}

				return elementTags;
			}
		}

        public Link(DomContainer domContainer, IHTMLAnchorElement element) :
            base(domContainer, domContainer.NativeBrowser.CreateElement(element)) { }

		public Link(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder) {}

		/// <summary>
		/// Initialises a new instance of the <see cref="Link"/> class based on <paramref name="element"/>.
		/// </summary>
		/// <param name="element">The element.</param>
		public Link(Element element) : base(element, ElementTags) {}

		public string Url
		{
			get { return GetAttributeValue("href"); }
		}

		internal new static Element New(DomContainer domContainer, IHTMLElement element)
		{
			return new Link(domContainer, (IHTMLAnchorElement) element);
		}
	}
}
