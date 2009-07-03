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
	/// This class represents an HTML "body" element.
	/// </summary>
    [ElementTag("body", Index = 0)]
    public class Body : ElementContainer<Body>
    {
		/// <summary>
		/// Initialises a new instance of the <see cref="Body"/> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The <see cref="DomContainer" /> the element is in.</param>
		/// <param name="element">The input button or button element.</param>
		public Body(DomContainer domContainer, INativeElement element) : base(domContainer, element) { }

		/// <summary>
		/// Initialises a new instance of the <see cref="Body"/> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The <see cref="DomContainer" /> the element is in.</param>
		/// <param name="finder">The input button or button element.</param>
        public Body(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }
	}
}
