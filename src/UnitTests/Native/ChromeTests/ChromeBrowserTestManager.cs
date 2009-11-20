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

# if INCLUDE_CHROME

using WatiN.Core.UnitTests.TestUtils;

namespace WatiN.Core.UnitTests.Native.ChromeTests
{
    using System;

    public class ChromeBrowserTestManager : IBrowserTestManager
    {
        private Chrome chrome;

        public Browser CreateBrowser(Uri uri)
        {
            return new Chrome(uri);
        }

        public Browser GetBrowser(Uri uri)
        {
            if (this.chrome == null)
            {
                this.chrome = (Chrome)CreateBrowser(uri);
            }

            return this.chrome;
        }

        public void CloseBrowser()
        {
            if (this.chrome == null)
            {
                return;
            }

            this.chrome.Dispose();
            this.chrome = null;
        }
    }
}
#endif