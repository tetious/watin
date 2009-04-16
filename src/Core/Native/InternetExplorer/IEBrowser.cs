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
using System.Runtime.InteropServices;
using System.Threading;
using mshtml;
using SHDocVw;

namespace WatiN.Core.Native.InternetExplorer
{
	public class IEBrowser : INativeBrowser 
	{
	    private readonly IWebBrowser2 _webBrowser2;

	    public IEBrowser(IWebBrowser2 webBrowser2)
	    {
	        _webBrowser2 = webBrowser2;
	    }

	    public object AsIWebBrowser2
	    {
            get { return _webBrowser2; }
	    }

	    /// <inheritdoc />
        public void NavigateTo(Uri url)
	    {
            object nil = null;
            object absoluteUri = url.AbsoluteUri;
            _webBrowser2.Navigate2(ref absoluteUri, ref nil, ref nil, ref nil, ref nil);
        }

        /// <inheritdoc />
        public void NavigateToNoWait(Uri url)
	    {
            var thread = new Thread(GoToNoWaitInternal);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(url);
            thread.Join(500);
        }

        [STAThread]
        private void GoToNoWaitInternal(object uriIn)
        {
            var uri = (Uri)uriIn;
            NavigateTo(uri);
        }

        /// <inheritdoc />
        public bool GoBack()
	    {
            try
            {
                _webBrowser2.GoBack();
                return true;
            }
            catch (COMException)
            {
                return false;
            }
        }

        /// <inheritdoc />
        public bool GoForward()
	    {
            try
            {
                _webBrowser2.GoForward();
                return true;
            }
            catch (COMException)
            {
                return false;
            }
        }

        /// <inheritdoc />
        public void Reopen()
	    {
	        throw new System.NotImplementedException();
	    }

        /// <inheritdoc />
        public void Refresh()
	    {
            object REFRESH_COMPLETELY = 3;
            _webBrowser2.Refresh2(ref REFRESH_COMPLETELY);
        }

        /// <inheritdoc />
        public IntPtr hWnd
	    {
            get { return new IntPtr(_webBrowser2.HWND); }
	    }

	    public INativeDocument NativeDocument
	    {
            get { return new IEDocument((IHTMLDocument2) _webBrowser2.Document); }
	    }

	    public bool Visible
	    {
            get { return _webBrowser2.Visible; }
            set { _webBrowser2.Visible = value; }
	    }

        public void Quit()
        {
            _webBrowser2.Quit();
        }
	}
}
