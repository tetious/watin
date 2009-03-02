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
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using WatiN.Core.Logging;
using WatiN.Core.Native.Mozilla;
using WatiN.Core.Native;
using WatiN.Core.UtilityClasses;

// https://developer.mozilla.org/en/Gecko_DOM_Reference

namespace WatiN.Core
{
    public class FireFox : Browser 
    {
        private FFBrowser nativeBrowser;

        #region Public constructors / destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FireFox"/> class.
        /// </summary>
        public FireFox()
        {
            CreateFireFoxInstance("about:blank");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FireFox"/> class.
        /// </summary>
        /// <param name="url">The url to go to</param>
        public FireFox(string url)
        {
            CreateFireFoxInstance(url);
            WaitForComplete();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FireFox"/> class.
        /// </summary>
        /// <param name="uri">The url to go to</param>
        public FireFox(Uri uri)
        {
            CreateFireFoxInstance(uri.AbsoluteUri);
            WaitForComplete();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="FireFox"/> is reclaimed by garbage collection.
        /// </summary>
        ~FireFox()
        {
            Dispose(false);
        }

        #endregion

        #region Public instance properties

//        public BrowserType BrowserType
//        {
//            get { return Core.BrowserType.FireFox; }
//        }

        public new FFBrowser NativeBrowser
        {
            get { return nativeBrowser; }
            private set { nativeBrowser = value; }
        }

        protected override INativeBrowser GetNativeBrowser()
        {
            return NativeBrowser;
        }

        #endregion Public instance properties

        #region Public instance methods

        protected override bool GoBack()
        {
            return NativeBrowser.Back();
        }

        protected override bool GoForward()
        {
            return NativeBrowser.Forward();
        }

        protected override void NavigateTo(Uri url)
        {
            NativeBrowser.LoadUri(url);
        }

        protected override void NavigateToNoWait(Uri url)
        {
            NativeBrowser.LoadUriNoWait(url);
        }

        public override IntPtr hWnd
        {
            get { return NativeBrowser.hWnd; }
        }

        public override INativeDocument OnGetNativeDocument()
        {
            return new FFDocument(NativeBrowser.ClientPort);
        }

        /// <summary>
        /// Gets the number of running FireFox processes.
        /// </summary>
        /// <value>The number of running FireFox processes.</value>
        public static int CurrentProcessCount
        {
            get
            {
                var ffCount = 0;

                foreach (var process in Process.GetProcesses())
                {
                    if (process.ProcessName.Equals("firefox", StringComparison.OrdinalIgnoreCase))
                    {
                        ffCount++;
                    }
                }

                return ffCount;
            }
        }

        /// <summary>
        /// Gets the current FireFox process.
        /// </summary>
        /// <value>The current FireFox process or null if none is found.</value>
        internal static Process CurrentProcess
        {
            get
            {
                Process ffProcess = null;

                foreach (var process in Process.GetProcesses())
                {
                    if (process.ProcessName.Equals("firefox", StringComparison.OrdinalIgnoreCase))
                    {
                        ffProcess = process;
                    }
                }

                return ffProcess;
            }
        }

        private static string pathToExe;
        /// <summary>
        /// Gets the path to FireFox executable.
        /// </summary>
        /// <value>The path to exe.</value>
        public static string PathToExe
        {
            get
            {
                if (pathToExe == null)
                {
                    pathToExe = GetExecutablePath();
                }

                return pathToExe;
            }
        }

        internal static Process CreateProcess(string arguments, bool waitForMainWindow)
        {
            var ffProcess = new Process {StartInfo = {FileName = PathToExe, Arguments = arguments}};
            
            ffProcess.Start();
            ffProcess.WaitForInputIdle(5000);
            ffProcess.Refresh();

            if (waitForMainWindow)
            {
                var action = new TryFuncUntilTimeOut(Settings.WaitForCompleteTimeOut) { SleepTime = 200};
                var result = action.Try(() =>
                                            {
                                                ffProcess.Refresh();
                                                if (!ffProcess.HasExited && ffProcess.MainWindowHandle != IntPtr.Zero)
                                                {
                                                    Logger.LogAction("Waited for FireFox, main window handle found.");
                                                    return true;
                                                }
                                                return false;
                                            });

                if (!result)
                {
                    Debug.WriteLine("Timer elapsed waiting for FireFox to start.");
                }
            }

            return ffProcess;
        }


        /// <summary>
        /// Initalizes the executable path.
        /// </summary>
        private static string GetExecutablePath()
        {
            string path;
            var mozillaKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Mozilla\Mozilla Firefox");
            if (mozillaKey != null)
            {
                path = GetExecutablePathUsingRegistry(mozillaKey);
            }
            else
            {
                // We try and guess common locations where FireFox might be installed
                var tempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Mozilla FireFox\FireFox.exe");
                if (File.Exists(tempPath))
                {
                    path = tempPath;
                }
                else
                {
                    tempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + " (x86)", @"Mozilla FireFox\FireFox.exe");
                    if (File.Exists(tempPath))
                    {
                        path = tempPath;
                    }
                    else
                    {
                        throw new FireFoxException("Unable to determine the current version of FireFox tried looking in the registry and the common locations on disk, please make sure you have installed FireFox and Jssh correctly");
                    }
                }
            }

            return path;
        }

        /// <summary>
        /// Initializes the executable path to FireFox using the registry.
        /// </summary>
        /// <param name="mozillaKey">The mozilla key.</param>
        private static string GetExecutablePathUsingRegistry(RegistryKey mozillaKey)
        {
            var currentVersion = (string)mozillaKey.GetValue("CurrentVersion");
            if (string.IsNullOrEmpty(currentVersion))
            {
                throw new FireFoxException("Unable to determine the current version of FireFox using the registry, please make sure you have installed FireFox and Jssh correctly");
            }

            var currentMain = mozillaKey.OpenSubKey(string.Format(@"{0}\Main", currentVersion));
            if (currentMain == null)
            {
                throw new FireFoxException(
                    "Unable to determine the current version of FireFox using the registry, please make sure you have installed FireFox and Jssh correctly");
            }

            var path = (string)currentMain.GetValue("PathToExe");
            if (!File.Exists(path))
            {
                throw new FireFoxException(
                    "FireFox executable listed in the registry does not exist, please make sure you have installed FireFox and Jssh correctly");
            }

            return path;
        }

        /// <summary>
        /// Reloads the currently displayed webpage.
        /// </summary>
        protected override void DoRefresh()
        {
            NativeBrowser.Reload();
        }

        /// <summary>
        /// Closes then reopens the browser navigating to a blank page.
        /// </summary>
        /// <remarks>
        /// Useful when clearing the cookie cache and continuing execution to a test.
        /// </remarks>
        protected override void DoReopen()
        {
            NativeBrowser.ClientPort.Dispose();
            NativeBrowser.ClientPort.Connect(string.Empty);
            WaitForComplete();
        }

        /// <summary>
        /// Waits until the page is completely loaded
        /// </summary>
        public override void WaitForComplete(int waitForCompleteTimeOut)
        {
            WaitForComplete(new FFWaitForComplete(NativeBrowser, waitForCompleteTimeOut));
        }
        
        #endregion Public instance methods

        #region Protected instance methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (IsDisposed) return;
            
            // If disposing equals true, dispose all managed 
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
                NativeBrowser.ClientPort.Dispose();
            }

            // Call the appropriate methods to clean up 
            // unmanaged resources here.
            base.Dispose(disposing);
        }

        #endregion Protected instance methods

        #region Private instance methods



        #endregion Private instance methods

        #region Private static methods

        private void CreateFireFoxInstance(string url)
        {
            Logger.LogAction("Creating FireFox instance");

            UtilityClass.MoveMousePoinerToTopLeft(Settings.AutoMoveMousePointerToTopLeft);

            var clientPort = new FireFoxClientPort();
            clientPort.Connect(url);

            NativeBrowser = new FFBrowser(clientPort);
            WaitForComplete();
        }

        #endregion

        protected override string GetAttributeValueImpl(string attributeName)
        {
            return null;
        }
    }
}