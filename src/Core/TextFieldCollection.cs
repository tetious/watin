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
using System.Collections;
using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// A typed collection of <see cref="TextField" /> instances within a <see cref="Document"/> or <see cref="Element"/>.
	/// </summary>
#if NET11
	public class TextFieldCollection : BaseElementCollection
#else
    public class TextFieldCollection : BaseElementCollection<TextField>
#endif
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TextFieldCollection"/> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The DOM container.</param>
		/// <param name="finder">The finder.</param>
		public TextFieldCollection(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder, new CreateElementInstance(TextField.New)) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextFieldCollection"/> class.
		/// Mainly used by WatiN internally.
		/// </summary>
		/// <param name="domContainer">The DOM container.</param>
		/// <param name="elements">The elements.</param>
		public TextFieldCollection(DomContainer domContainer, ArrayList elements) : base(domContainer, elements, new CreateElementInstance(TextField.New)) {}

		/// <summary>
		/// Gets the <see cref="TextField"/> at the specified index.
		/// </summary>
		/// <value></value>
		public TextField this[int index]
		{
			get { return (TextField) ElementsTyped(index); }
		}

		public TextFieldCollection Filter(BaseConstraint findBy)
		{
			return new TextFieldCollection(domContainer, DoFilter(findBy));
		}

#if !NET11
        public TextFieldCollection Filter(Predicate<TextField> predicate)
        {
            return new TextFieldCollection(domContainer, DoFilter(Find.ByElement(predicate)));
        }
#endif

	}
}