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

using WatiN.Core.Native;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML input element of type 
	/// button, submit, image and reset.
	/// </summary>
    [ElementTag("input", InputType = "button", Index = 0)]
    [ElementTag("input", InputType = "submit", Index = 1)]
    [ElementTag("input", InputType = "reset", Index = 2)]
    [ElementTag("button", Index = 3)]
    public class Button : Element<Button>
    {
		/// <summary>
		/// Initialises a new instance of the <see cref="Button"/> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The <see cref="DomContainer" /> the element is in.</param>
		/// <param name="element">The input button or button element.</param>
		public Button(DomContainer domContainer, INativeElement element) : base(domContainer, element) { }

		/// <summary>
		/// Initialises a new instance of the <see cref="Button"/> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The <see cref="DomContainer" /> the element is in.</param>
		/// <param name="finder">The input button or button element.</param>
        public Button(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }

		/// <summary>
		/// The text displayed at the button.
		/// </summary>
		/// <value>The displayed text.</value>
        public virtual string Value
		{
			get { return GetAttributeValue("value"); }
		}

		/// <summary>
		/// The text displayed at the button (alias for the Value property).
		/// </summary>
		/// <value>The displayed text.</value>
		public override string Text
		{
			get { return Value; }
		}

        /// <inheritdoc />
		protected override string DefaultToString()
		{
			return Value;
		}
	}
}
