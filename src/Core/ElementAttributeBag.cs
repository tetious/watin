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

using WatiN.Core.Constraints;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// Wrapper around the <see cref="INativeElement"/> object. Used by <see cref="BaseConstraint.Compare"/>.
	/// </summary>
	public class ElementAttributeBag : IAttributeBag
	{
		private INativeElement _nativeElement;
		private Element _element;
		private Element _elementTyped;

	    public ElementAttributeBag(DomContainer domContainer) : this (domContainer, null)
        {}

		public ElementAttributeBag(DomContainer domContainer, INativeElement element)
		{
            DomContainer = domContainer;
            INativeElement = element;
		}

		/// <summary>
		/// Gets or sets the IHTMLelement from which the attribute values are read.
		/// </summary>
		/// <value>The IHTMLelement.</value>
		public INativeElement INativeElement
		{
			get { return _nativeElement; }
			set 
            { 
                _nativeElement = value;
                _element = null;
                _elementTyped = null;
            }
		}

	    public DomContainer DomContainer { get; set; }

	    /// <summary>
		/// Returns a typed Element instance that can be cast to an ElementsContainer.
		/// </summary>
		/// <value>The element.</value>
		public Element Element
		{
			get
			{
				if (_element == null)
				{
				    _element = TypedElementFactory.GetDefaultReturnElement(DomContainer, INativeElement);
				}

				return _element;
			}
		}

		/// <summary>
		/// Returns a typed Element instance that can be cast to the specific WatiN type.
		/// </summary>
		/// <value>The element typed.</value>
		public Element ElementTyped
		{
			get
			{
				if (_elementTyped == null)
				{
                    _elementTyped = TypedElementFactory.CreateTypedElement(DomContainer, INativeElement);
				    _element = _elementTyped;
				}

				return _elementTyped;
			}
		}

	    /// <summary>
		/// Gets the value for the given <paramref name="attributename" />
		/// </summary>
		/// <param name="attributename">The attributename.</param>
		/// <returns>The value of the attribute</returns>
		public string GetValue(string attributename)
		{
	        return Element.GetAttributeValue(attributename, _nativeElement);
        }
	}
}
