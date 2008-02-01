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
using SHDocVw;
using WatiN.Core.Constraints;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// Wrapper around the <see cref="SHDocVw.InternetExplorer"/> object. Used by <see cref="BaseConstraint.Compare"/>.
	/// </summary>
	public class IEAttributeBag : IAttributeBag
	{
		private SHDocVw.InternetExplorer internetExplorer = null;

		public SHDocVw.InternetExplorer	 InternetExplorer
		{
			get { return internetExplorer; }
			set { internetExplorer = value; }
		}

		public string GetValue(string attributename)
		{
			string name = attributename.ToLower(CultureInfo.InvariantCulture);
			string value;

			if (name.Equals("href"))
			{
				value = GetUrl();
			}
			else if (name.Equals("title"))
			{
				value = GetTitle();
			}
			else if (name.Equals("hwnd"))
			{
				value = GetHwnd();
			}
			else
			{
				throw new InvalidAttributException(attributename, "IE");
			}

			return value;
		}

		private string GetHwnd()
		{
			try
			{
				return InternetExplorer.HWND.ToString();
			}
			catch {}
			return null;
		}

		private string GetTitle()
		{
			try
			{
				return ((HTMLDocument) InternetExplorer.Document).title;
			}
			catch {}
			return null;
		}

		private string GetUrl()
		{
			try
			{
				return InternetExplorer.LocationURL;
			}
			catch {}
			return null;
		}
	}
}