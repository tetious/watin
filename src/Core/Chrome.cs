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

namespace WatiN.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using Logging;

    using Microsoft.Win32;

    using Native.Chrome;

    using UtilityClasses;

    using WatiN.Core.Constraints;
    using WatiN.Core.Native;

    /// <summary>
    /// Main class used to access web pages in Google Chrome.
    /// </summary>
    public class Chrome : Browser
    {
        /// <summary>
        /// Path to the chrome executable.
        /// </summary>
        private static string pathToExe;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chrome"/> class.
        /// </summary>
        public Chrome() : this(new Uri("about:blank"))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chrome"/> class.
        /// </summary>
        /// <param name="url">The initail URL to load.</param>
        public Chrome(string url)
        {
            // We don't call this(new Uri), because we don't want an escaped url.
            this.CreateChromeInstance(url);
            this.WaitForComplete();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chrome"/> class.
        /// </summary>
        /// <param name="url">The initial URL to load.</param>
        public Chrome(Uri url)
        {
            this.CreateChromeInstance(url.ToString());
            this.WaitForComplete();
        }

        /// <summary>
        /// Gets the number of running FireFox processes.
        /// </summary>
        /// <value>The number of running FireFox processes.</value>
        public static int CurrentProcessCount
        {
            get
            {
                int chromeCount = 0;

                foreach (var process in Process.GetProcesses())
                {
                    if (process.ProcessName.Equals("chrome", StringComparison.OrdinalIgnoreCase))
                    {
                        chromeCount++;
                    }
                }

                return chromeCount;
            }
        }

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

        /// <summary>
        /// Gets window pointer to the current browser.
        /// </summary>
        /// <value>
        /// The windows pointer to the current browser.
        /// </value>
        public override IntPtr hWnd
        {
            get
            {
                return ChromeBrowser.hWnd;
            }
        }

        /// <inheritdoc />
        public override bool ContainsText(string text)
        {
            return this.ChromeBrowser.ClientPort.WriteAndReadAsInt("document.body.innerText.lastIndexOf('{0}');", text) > -1;
        }

        /// <summary>
        /// Gets the native browser.
        /// </summary>
        /// <value>The native browser.</value>
        public override INativeBrowser NativeBrowser
        {
            get { return ChromeBrowser; }
        }

        private ChromeBrowser ChromeBrowser { get; set; }

        /// <summary>
        /// Gets the current Chrome process.
        /// </summary>
        /// <value>The current Chrome process or null if none is found.</value>
        internal static Process CurrentProcess
        {
            get
            {
                Process chromeProcess = null;

                foreach (var process in Process.GetProcesses())
                {
                    if (process.ProcessName.Equals("chrome", StringComparison.OrdinalIgnoreCase))
                    {
                        chromeProcess = process;
                    }
                }

                return chromeProcess;
            }
        }

        /// <summary>
        /// Waits for the page to be completely loaded.
        /// </summary>
        /// <param name="waitForCompleteTimeOut">
        /// The number of seconds to wait before timing out
        /// </param>
        public override void WaitForComplete(int waitForCompleteTimeOut)
        {
            this.WaitForComplete(new JSWaitForComplete(this.ChromeBrowser, waitForCompleteTimeOut));
        }

        /// <summary>
        /// Closes the browser.
        /// </summary>
        public override void Close()
        {
            this.ChromeBrowser.Close();
        }

        /// <summary>
        /// Creates the Chrome process.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="waitForMainWindow">if set to <c>true</c> [wait for main window].</param>
        /// <returns>Chrome process.</returns>
        internal static Process CreateProcess(string arguments, bool waitForMainWindow)
        {
            var chromeProcess = new Process { StartInfo = { FileName = PathToExe, Arguments = arguments } };

            chromeProcess.Start();
            chromeProcess.WaitForInputIdle(5000);
            chromeProcess.Refresh();

            if (waitForMainWindow)
            {
                var action = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(Settings.WaitForCompleteTimeOut))
                {
                    SleepTime = TimeSpan.FromMilliseconds(200)
                };

                var result = action.Try(() =>
                                        {
                                            chromeProcess.Refresh();
                                            if (!chromeProcess.HasExited && chromeProcess.MainWindowHandle != IntPtr.Zero)
                                            {
                                                Logger.LogAction("Waited for Chrome, main window handle found.");
                                                return true;
                                            }

                                            return false;
                                        });

                if (!result)
                {
                    Debug.WriteLine("Timer elapsed waiting for Chrome to start.");
                }
            }

            return chromeProcess;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (IsDisposed)
            {
                return;
            }

            // If disposing equals true, dispose all managed 
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
                ChromeBrowser.ClientPort.Dispose();
            }

            // Call the appropriate methods to clean up 
            // unmanaged resources here.
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initalizes the chrome executable path.
        /// </summary>
        /// <returns>The chrome executable path</returns>
        private static string GetExecutablePath()
        {
            string path;
            var chromeKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\Google Chrome\");
            if (chromeKey != null)
            {
                path = Path.Combine((string)chromeKey.GetValue("InstallLocation"), "chrome.exe");

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException("Error locating chrome executable", path);
                }
            }
            else
            {
                throw new ChromeException(@"Unable to determine the location of Chrome, please make sure you have installed it on your computer, tried looking for the registry key HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\Google Chrome\InstallLocation");                
            }

            return path;
        }

        /// <summary>
        /// Creates the chrome instance.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        private void CreateChromeInstance(string url)
        {
            Logger.LogAction("Creating Chrome instance");

            UtilityClass.MoveMousePoinerToTopLeft(Settings.AutoMoveMousePointerToTopLeft);

            var clientPort = new ChromeClientPort();
            clientPort.Connect(url);

            ChromeBrowser = new ChromeBrowser(clientPort);
        }

        public static Chrome AttachToChrome(Constraint findBy)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif