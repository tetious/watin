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

namespace WatiN.Core
{
	/// <summary>
	/// A typed collection of <see cref="Image" /> instances within a <see cref="Document"/> or <see cref="Element"/>.
	/// </summary>
    public class ImageCollection : BaseElementCollection<Image, ImageCollection>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageCollection"/> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The DOM container.</param>
		/// <param name="finder">The finder.</param>
        public ImageCollection(DomContainer domContainer, ElementFinder finder) : base(domContainer, finder) { }

        /// <inheritdoc />
        protected override ImageCollection CreateFilteredCollection(ElementFinder elementFinder)
        {
            return new ImageCollection(DomContainer, elementFinder);
        }
    }
}