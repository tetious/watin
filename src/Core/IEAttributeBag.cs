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

namespace WatiN.Core
{
  using mshtml;
  using SHDocVw;
  using WatiN.Core.Exceptions;
  using WatiN.Core.Interfaces;

  /// <summary>
  /// Wrapper around the <see cref="SHDocVw.InternetExplorer"/> object. Used by <see cref="AttributeConstraint.Compare"/>.
  /// </summary>
  public class IEAttributeBag: IAttributeBag
  {
    private InternetExplorer internetExplorer = null;

    public InternetExplorer InternetExplorer
    {
      get
      {
        return internetExplorer;
      }
      set
      {
        internetExplorer = value;
      }
    }

    public string GetValue(string attributename)
    {
      string name = attributename.ToLower();
      string value = null;
      
      if (name.Equals("href"))
      {
        try
        {
          value = InternetExplorer.LocationURL;
        }
        catch{}
      }
      else if (name.Equals("title"))
      {
        try
        {
          value = ((HTMLDocument) InternetExplorer.Document).title;
        }
        catch{}
      }
      else if (name.Equals("hwnd"))
      {
        try
        {
          value = InternetExplorer.HWND.ToString();
        }
        catch{}
      }
      else
      {
        throw new InvalidAttributException(attributename, "IE");
      }
      
      return value;
    }
  }
}