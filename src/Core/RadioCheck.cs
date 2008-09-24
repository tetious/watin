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
using WatiN.Core.Logging;

namespace WatiN.Core
{
	/// <summary>
	/// Base class for <see cref="CheckBox"/> and <see cref="RadioButton"/> provides
	/// support for common functionality.
	/// </summary>
#if NET11
    public class RadioCheck : Element
#else
    public class RadioCheck<E> : Element<E> where E : Element
#endif
	{
        public RadioCheck(DomContainer domContainer, IHTMLInputElement element) :
            base(domContainer, domContainer.NativeBrowser.CreateElement(element)) { }

		public RadioCheck(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder) {}

		/// <summary>
		/// Initialises a new instance of this class based on <paramref name="element"/>.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="elementTags">The element tags the element should match with.</param>
		public RadioCheck(Element element, ArrayList elementTags) : base(element, elementTags) {}

		public bool Checked
		{
			get { return bool.Parse(GetAttributeValue("checked")); }
			set
			{
				Logger.LogAction("Selecting " + GetType().Name + " '" + ToString() + "'");

				Highlight(true);
				inputElement.@checked = value;
				FireEvent("onClick");
				Highlight(false);
			}
		}

		public override string ToString()
		{
			return Id;
		}

		private IHTMLInputElement inputElement
		{
			get { return ((IHTMLInputElement) HTMLElement); }
		}
	}
}
