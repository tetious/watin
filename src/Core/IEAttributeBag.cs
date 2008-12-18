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

using System.Globalization;
using mshtml;
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
	    public SHDocVw.InternetExplorer InternetExplorer { get; set; }

	    public string GetValue(string attributename)
		{
			var name = attributename.ToLower(CultureInfo.InvariantCulture);
			string value;

			if (name.Equals("href"))
			{
                value = TryOtherwiseReturnNull(() => InternetExplorer.LocationURL);
			}
			else if (name.Equals("title"))
			{
                value = TryOtherwiseReturnNull(() => ((HTMLDocument)InternetExplorer.Document).title);
			}
			else if (name.Equals("hwnd"))
			{
			    value = TryOtherwiseReturnNull(() => InternetExplorer.HWND.ToString());
			}
			else
			{
				throw new InvalidAttributException(attributename, "IE");
			}

			return value;
		}

	    private static string TryOtherwiseReturnNull(TryAction<string> action )
	    {
	        try
	        {
	            return action.Invoke();
	        }
	        catch {}
	        return null;
	    }
	}
}