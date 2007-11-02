#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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
using System.Globalization;
using mshtml;
using StringComparer = WatiN.Core.Comparers.StringComparer;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// Wrapper around the <see cref="mshtml.IHTMLElement"/> object. Used by <see cref="BaseConstraint.Compare"/>.
	/// </summary>
	public class ElementAttributeBag : IAttributeBag
	{
		private IHTMLElement element = null;

		public ElementAttributeBag() {}

		public ElementAttributeBag(IHTMLElement element)
		{
			IHTMLElement = element;
		}

		public IHTMLElement IHTMLElement
		{
			get { return element; }
			set { element = value; }
		}

		public string GetValue(string attributename)
		{
			if (StringComparer.AreEqual(attributename, "style", true))
			{
				return element.style.cssText;
			}

			object attributeValue;

			if (attributename.ToLower(CultureInfo.InvariantCulture).StartsWith("style."))
			{
				attributeValue = Style.GetAttributeValue(attributename.Substring(6), element.style);
			}
			else
			{
				attributeValue = element.getAttribute(attributename, 0);
			}

			if (attributeValue == DBNull.Value)
			{
				return null;
			}

			if (attributeValue == null)
			{
				return null;
			}

			return attributeValue.ToString();
		}
	}
}