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

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML para element.
	/// </summary>
    public class Para : ElementsContainer<Para>
	{
        private static List<ElementTag> elementTags;

        public static List<ElementTag> ElementTags
		{
			get
			{
				if (elementTags == null)
				{
                    elementTags = new List<ElementTag> { new ElementTag("p") };
				}

				return elementTags;
			}
		}

		public Para(DomContainer domContainer, INativeElement htmlParaElement) : base(domContainer, htmlParaElement) {}

		public Para(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder) {}

		/// <summary>
		/// Initialises a new instance of the <see cref="Para"/> class based on <paramref name="element"/>.
		/// </summary>
		/// <param name="element">The element.</param>
		public Para(Element element) : base(element, ElementTags) {}

		internal new static Element New(DomContainer domContainer, INativeElement element)
		{
			return new Para(domContainer, element);
		}
	}
}
