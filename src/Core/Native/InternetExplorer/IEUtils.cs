#region WatiN Copyright (C) 2006-2011 Jeroen van Menen

//Copyright 2006-2011 Jeroen van Menen
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
using System.Text;
using mshtml;
using SHDocVw;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;
using IEnumUnknown=WatiN.Core.Native.Windows.IEnumUnknown;

namespace WatiN.Core.Native.InternetExplorer
{
    public static class IEUtils
    {
        public static readonly VariableNameHelper IEVariableNameHelper = new VariableNameHelper();

        [DllImport("oleacc", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern Int32 ObjectFromLresult(Int32 lResult, ref Guid riid, Int32 wParam, ref IHTMLDocument2 ppvObject);

        /// <summary>
        /// Runs the javascript code in IE.
        /// </summary>
        /// <param name="scriptCode">The javascript code.</param>
        /// <param name="window">The parent window of the document.</param>
        public static void RunScript(StringBuilder scriptCode, IHTMLWindow2 window)
        {
            RunScript(scriptCode.ToString(), window);
        }

        /// <summary>
        /// Runs the javascript code in IE.
        /// </summary>
        /// <param name="scriptCode">The javascript code.</param>
        /// <param name="window">The parent window of the document.</param>
        public static void RunScript(string scriptCode, IHTMLWindow2 window)
        {
            RunScript(scriptCode, "javascript", window);
        }

        /// <summary>
        /// Runs the script code in IE.
        /// </summary>
        /// <param name="scriptCode">The script code.</param>
        /// <param name="language">The language.</param>
        /// <param name="window">The parent window of the document.</param>
        public static void RunScript(string scriptCode, string language, IHTMLWindow2 window)
        {
            try
            {
                Logger.LogDebug("[script] {0}", scriptCode);
                window.execScript(scriptCode, language);
            }
            catch (Exception ex)
            {
                throw new RunScriptException(ex);
            }
        }


        public static IHTMLDocument2 IEDOMFromhWnd(IntPtr hWnd)
        {
            var IID_IHTMLDocument2 = new Guid("626FC520-A41E-11CF-A731-00A0C9082637");

            var lRes = 0;

            if (!IsIEServerWindow(hWnd))
            {
                // Get 1st child IE server window
                hWnd = NativeMethods.GetChildWindowHwnd(hWnd, "Internet Explorer_Server");
            }

            if (IsIEServerWindow(hWnd))
            {
                // Register the message
                var lMsg = NativeMethods.RegisterWindowMessage("WM_HTML_GETOBJECT");
                // Get the object
                NativeMethods.SendMessageTimeout(hWnd, lMsg, 0, 0, NativeMethods.SMTO_ABORTIFHUNG, 1000, ref lRes);
                if (lRes != 0)
                {
                    // Get the object from lRes
                    IHTMLDocument2 ieDOMFromhWnd = null;
                    var hr = ObjectFromLresult(lRes, ref IID_IHTMLDocument2, 0, ref ieDOMFromhWnd);
                    if (hr != 0)
                    {
                        throw new COMException("ObjectFromLresult has thrown an exception", hr);
                    }
                    return ieDOMFromhWnd;
                }
            }
            return null;
        }

        public static bool IsIEServerWindow(IntPtr hWnd)
        {
            return NativeMethods.CompareClassNames(hWnd, "Internet Explorer_Server");
        }

        internal static void EnumIWebBrowser2Interfaces(IWebBrowser2Processor processor)
        {
            var oc = processor.HTMLDocument() as IOleContainer;

            if (oc == null) return;

            IEnumUnknown eu;
            var hr = oc.EnumObjects(NativeMethods.tagOLECONTF.OLECONTF_EMBEDDINGS, out eu);
            Marshal.ThrowExceptionForHR(hr);

            if (eu == null) return;

            try
            {
                object pUnk;
                int fetched;
                const int MAX_FETCH_COUNT = 1;

                // get the first embedded object
                // pUnk alloc
                hr = eu.Next(MAX_FETCH_COUNT, out pUnk, out fetched);
                Marshal.ThrowExceptionForHR(hr);

                while (hr == 0)
                {
                    // Query Interface pUnk for the IWebBrowser2 interface
                    var brow = pUnk as IWebBrowser2;

                    try
                    {
                        if (brow != null)
                        {
                            processor.Process(brow);
                            if (!processor.Continue())
                            {
                                break;
                            }
                            // free brow
                            ReleaseComObjectButIgnoreNull(brow);
                        }
                    }
                    catch
                    {
                        // free brow
                        ReleaseComObjectButIgnoreNull(brow);
                        ReleaseComObjectButIgnoreNull(pUnk);
                    }

                    // pUnk free
                    ReleaseComObjectButIgnoreNull(pUnk);

                    // get the next embedded object
                    // pUnk alloc
                    hr = eu.Next(MAX_FETCH_COUNT, out pUnk, out fetched);
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
            finally
            {
                // eu free
                ReleaseComObjectButIgnoreNull(eu);
            }
        }

        private static void ReleaseComObjectButIgnoreNull(object comObject)
        {
            if (comObject != null)
            {
                Marshal.ReleaseComObject(comObject);
            }
        }
    }
}
