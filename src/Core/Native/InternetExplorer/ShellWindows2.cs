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
using System.Collections;
using System.Collections.Generic;
using mshtml;
using SHDocVw;
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;
using IServiceProvider = WatiN.Core.Native.Windows.IServiceProvider;

namespace WatiN.Core.Native.InternetExplorer
{
    public class ShellWindows2 : IEnumerable<IWebBrowser2>
    {
        private List<IWebBrowser2> _browsers;

        private Guid SID_STopLevelBrowser = new Guid(0x4C96BE40, 0x915C, 0x11CF, 0x99, 0xD3, 0x00, 0xAA, 0x00, 0x4A, 0xE8, 0x37);
        private Guid SID_SWebBrowserApp = new Guid(0x0002DF05, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        public ShellWindows2()
        {
            CollectInternetExplorerInstances();
        }

        public int Count
        {
            get { return _browsers.Count; }
        }

        private void CollectInternetExplorerInstances()
        {
            var enumerator = new WindowsEnumerator();
            _browsers = new List<IWebBrowser2>();

            var topLevelWindows = enumerator.GetTopLevelWindows("IEFrame");
            foreach (var mainBrowserWindow in topLevelWindows)
            {
                var windows = enumerator.GetChildWindows(mainBrowserWindow.Hwnd, "TabWindowClass");

                // IE6 has no TabWindowClass so use the IEFrame as starting point
                if (windows.Count == 0)
                {
                    windows.Add(mainBrowserWindow);
                }

                foreach (var window in windows)
                {
                    var hwnd = window.Hwnd;
                    var document2 = UtilityClass.TryFuncIgnoreException(() => IEUtils.IEDOMFromhWnd(hwnd));
                    if (document2 == null) continue;

                    var parentWindow = UtilityClass.TryFuncIgnoreException(() => document2.parentWindow);
                    if (parentWindow == null) continue;

                    var webBrowser2 = UtilityClass.TryFuncIgnoreException(() => RetrieveIWebBrowser2FromIHtmlWindw2Instance(parentWindow));
                    if (webBrowser2 == null) continue;

                    _browsers.Add(webBrowser2);
                }
            }
        }

        /// <exclude />
        public IEnumerator GetEnumerator()
        {
            foreach (var browser in _browsers)
            {
                yield return browser;
            }
        }

        IEnumerator<IWebBrowser2> IEnumerable<IWebBrowser2>.GetEnumerator()
        {
            foreach (var browser in _browsers)
            {
                yield return browser;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IWebBrowser2 RetrieveIWebBrowser2FromIHtmlWindw2Instance(IHTMLWindow2 ihtmlWindow2)
        {
            var guidIServiceProvider = typeof(IServiceProvider).GUID;

            var serviceProvider = ihtmlWindow2 as IServiceProvider;
            if (serviceProvider == null) return null;

            object objIServiceProvider;
            serviceProvider.QueryService(ref SID_STopLevelBrowser, ref guidIServiceProvider, out objIServiceProvider);

            serviceProvider = objIServiceProvider as IServiceProvider;
            if (serviceProvider == null) return null;

            object objIWebBrowser;
            var guidIWebBrowser = typeof(IWebBrowser2).GUID;
            serviceProvider.QueryService(ref SID_SWebBrowserApp, ref guidIWebBrowser, out objIWebBrowser);
            var webBrowser = objIWebBrowser as IWebBrowser2;

            return webBrowser;
        }
    }


}
