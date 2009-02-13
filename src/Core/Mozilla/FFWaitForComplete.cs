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

namespace WatiN.Core.Mozilla
{
    public class FFWaitForComplete : WaitForCompleteBase
    {
        private readonly FFBrowser _browser;

        public FFWaitForComplete(FFBrowser browser, int waitForCompleteTimeOut): base(waitForCompleteTimeOut)
        {
            _browser = browser;
        }

        protected override void InitialSleep()
        {
            // Seems like this is not needed for FireFox
        }

        protected override void WaitForCompleteOrTimeout()
        {
            WaitWhileDocumentNotAvailable();
        }

        protected virtual void WaitWhileDocumentNotAvailable()
        {
            while (_browser.IsLoading())
            {
                ThrowExceptionWhenTimeout("waiting for main document becoming available");

                Sleep("FFWaitForComplete.WaitWhileDocumentNotAvailable");
            }

            _browser.ClientPort.InitializeDocument();
        }
    }
}
