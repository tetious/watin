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

using System;
using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.IETests
{
    public class IEBrowserTestManager : IBrowserTestManager
    {
        private IE ie;

        public Browser CreateBrowser(Uri uri)
        {
            return new IE(uri);
        }

        public Browser GetBrowser(Uri uri)
        {
            if (ie == null)
            {
                ie = (IE) CreateBrowser(uri);
            }

            return ie;
        }

        public void CloseBrowser()
        {
            if (ie == null) return;
            ie.Close();
            ie = null;
            if (IE.InternetExplorers().Count == 0) return;

            foreach (var explorer in IE.InternetExplorersNoWait())
            {
                Console.WriteLine(explorer.Title + " (" + explorer.Url + ")");
                explorer.Close();
            }
            throw new Exception("Expected no open IE instances.");
        }
    }
}