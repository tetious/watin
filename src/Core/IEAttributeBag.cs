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

using System.Globalization;
using mshtml;
using SHDocVw;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core
{
	/// <summary>
	/// Wrapper around the <see cref="SHDocVw.InternetExplorer"/> object. Used by <see cref="BaseConstraint.Compare"/>.
	/// </summary>
	public class IEAttributeBag : IAttributeBag
	{
	    public IWebBrowser2 InternetExplorer { get; set; }

	    public string GetValue(string attributename)
		{
			var name = attributename.ToLower(CultureInfo.InvariantCulture);
			string value = null;

			if (name.Equals("href"))
			{
                UtilityClass.TryActionIgnoreException(() => value = InternetExplorer.LocationURL);
			}
			else if (name.Equals("title"))
			{
                UtilityClass.TryActionIgnoreException(() => value = ((HTMLDocument)InternetExplorer.Document).title);
			}
			else if (name.Equals("hwnd"))
			{
                UtilityClass.TryActionIgnoreException(() => value = InternetExplorer.HWND.ToString());
			}
			else
			{
				throw new InvalidAttributeException(attributename, "IE");
			}

			return value;
		}
	}
}