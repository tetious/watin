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

namespace WatiN.Core.Native.Mozilla
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Windows.Forms;

    using Logging;
    using Windows;
    using UtilityClasses;
    using System.Net;

    /// <summary>
    /// The firefox client port used to communicate with the remote automation server.
    /// </summary>
    public class FireFoxClientPort : ClientPortBase
    {
        public const string LOCAL_IP_ADRESS = "127.0.0.1";
        public static int DEFAULT_PORT = 4242;

        public string IpAdress { get; set; }
        public int Port { get; set; }

        /// <summary>
        /// Name of the javascript variable that references the DOM:window object.
        /// </summary>
        public const string WindowVariableName = "window";

        /// <summary>
        /// Name of the javascript function to retrieve only child elements (skip text nodes).
        /// </summary>
        public const string GetChildElementsFunctionName = "WATINgetChildElements";

        /// <summary>
        /// <c>true</c> if the <see cref="Dispose()"/> method has been called to release resources.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Underlying socket used to create a <see cref="NetworkStream"/>.
        /// </summary>
        private Socket _telnetSocket;

        /// <summary>
        /// mozrepl prompt name
        /// </summary>
        private string _prompt;

        private const string _promptSuffix = "> ";

        private bool _emulateActiveElement;
        private bool _emulateActiveElementChecked;

        public FireFoxClientPort(string ipAdress, int port)
        {
            IpAdress = ipAdress;
            Port = port;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FireFoxClientPort"/> class. 
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="FireFox"/> is reclaimed by garbage collection.
        /// </summary>
        ~FireFoxClientPort()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the javascript prompt name
        /// </summary>
        public override string PromptName { get { return _prompt;  } }

        /// <summary>
        /// Gets a value indicating whether this <see cref="FireFoxClientPort"/> is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public bool Connected { get; private set; }

        /// <summary>
        /// Gets the name of the javascript variable that references the DOM:document object.
        /// </summary>
        public override string DocumentVariableName
        {
            get { return "document"; }   
        }

        /// <summary>
        /// Gets the type of java script engine.
        /// </summary>
        /// <value>The type of java script engine.</value>
        public override JavaScriptEngineType JavaScriptEngine
        {
            get
            {
                return JavaScriptEngineType.Mozilla;
            }
        }

        /// <summary>
        /// Gets the name of the browser variable.
        /// </summary>
        /// <value>The name of the browser variable.</value>
        public override string BrowserVariableName
        {
            get { return "browser"; }
        }

        /// <summary>
        /// Gets a value indicating whether the main FireFox window is visible, it's possible that the
        /// main FireFox window is not visible if a previous shutdown didn't complete correctly
        /// in which case the restore / resume previous session dialog may be visible.
        /// </summary>
        private bool IsMainWindowVisible
        {
            get
            {
                var result = NativeMethods.GetWindowText(Process.MainWindowHandle).Contains("Mozilla Firefox");
                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsRestoreSessionDialogVisible.
        /// </summary>
        /// <value>
        /// The is restore session dialog visible.
        /// </value>
        private bool IsRestoreSessionDialogVisible
        {
            get
            {
                var result = NativeMethods.GetWindowText(Process.MainWindowHandle).Contains("Firefox - ");
                return result;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <exception cref="FireFoxException">
        /// </exception>
        public override void Connect(string url)
        {
            Connect(url, true);
        }

        /// <summary>
        /// </summary>
        /// <exception cref="FireFoxException">
        /// </exception>
        public virtual void ConnectToExisting()
        {
            Connect(null, false);
        }

        private void Connect(string url, bool createNewFireFoxInstance)
        {
            ThrowExceptionIfConnected();

            // Init
            _disposed = false;
            LastResponse = string.Empty;
            Response = new StringBuilder();

            if (createNewFireFoxInstance) CreateNewFireFoxInstance(url);

            Logger.LogDebug("Attempting to connect to mozrepl server on {0} port {1}.", IpAdress, Port);

            ConnectToMozReplServer();
            WaitForConnectionEstablished();

            Logger.LogDebug("Successfully connected to FireFox using mozrepl.");

            DefineDefaultJSVariablesForWindow(0);
        }

        private void ConnectToMozReplServer()
        {
            _telnetSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {Blocking = true};

            try
            {
                _telnetSocket.Connect(IpAdress, Port);
                Connected = true;
            }
            catch (SocketException sockException)
            {
                Logger.LogDebug(string.Format("Failed connecting to mozrepl.\nError code:{0}\nError message:{1}", sockException.ErrorCode, sockException.Message));
                throw new FireFoxException("Unable to connect to mozrepl, please make sure you have correctly installed the mozrepl plugin", sockException);
            }
        }

        internal override System.Diagnostics.Process Process
        {
            get
            {
                return FireFox.CurrentProcess;
            }
            set
            {
                // not possible;
            }
        }

        private void CreateNewFireFoxInstance(string url)
        {
            Logger.LogDebug("Starting a new Firefox instance.");

            CloseExistingFireFoxInstances();

            if (string.IsNullOrEmpty(url)) url = "about:blank";
            FireFox.CreateProcess(url, true);

            if (IsMainWindowVisible) return;
            if (!IsRestoreSessionDialogVisible) return;
            
            NativeMethods.SetForegroundWindow(Process.MainWindowHandle);
            // TODO replace SendKeys cause they will hang the test when running test in a service or on
            //      a remote desktop session or on a locked system
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{ENTER}");
        }

        private void ThrowExceptionIfConnected()
        {
            if (Connected)
            {
                throw new FireFoxException("Already connected to mozrepl server.");
            }
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public override void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Reloads the javascript variables that are scoped at the document level.
        /// </summary>
        public override void InitializeDocument()
        {
            WriteAndRead("if(typeof(w0)!=='undefined'){0}.enter(w0.content);true", PromptName);

            if (!EmulateActiveElement()) return;

            // Javascript to implement document.activeElement if not supported by browser (FireFox 2.x)
            Write(DocumentVariableName + ".activeElement = " + DocumentVariableName + ".body;" +
                       "var allElements = " + DocumentVariableName + ".getElementsByTagName(\"*\");" +
                       "for (i = 0; i < allElements.length; i++){" +
                       "allElements[i].addEventListener(\"focus\", function (event) {" +
                       DocumentVariableName + ".activeElement = event.target;}, false);}");
        }

        private bool EmulateActiveElement()
        {
            if (!_emulateActiveElementChecked)
            {
                _emulateActiveElement = WriteAndReadAsBool(DocumentVariableName + ".activeElement == null");
                _emulateActiveElementChecked = true;
            }
            return _emulateActiveElement;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_telnetSocket != null && _telnetSocket.Connected && (Process == null || !Process.HasExited))
                    {
                        try
                        {
                            WriteAndRead("{0}.home();", PromptName);
                            var windowCount = WriteAndReadAsInt("{0}.getWindows().length", PromptName);
                            Logger.LogDebug(string.Format("Closing window. {0} total windows found", windowCount));
                            SendCommand(string.Format("{0}.close();", WindowVariableName));
                            if (windowCount == 1)
                            {
                                Logger.LogDebug("No further windows remain open.");
                                CloseConnection();
                                CloseFireFoxProcess();
                            }
                            else
                            {
                                TryFuncUntilTimeOut waiter = new TryFuncUntilTimeOut(TimeSpan.FromMilliseconds(2000));
                                bool windowClosed = waiter.Try<bool>(() => { return WriteAndReadAsInt("{0}.getWindows().length", PromptName) == windowCount - 1; });
                            }
                        }
                        catch (IOException ex)
                        {
                            Logger.LogDebug("Error communicating with mozrepl server to initiate shut down, message: {0}", ex.Message);
                        }
                    }
                }
            }

            _disposed = true;
            Connected = false;
        }

        private void CloseFireFoxProcess()
        {
            //if (Process == null) return;
            
            //Process.WaitForExit(5000);
            
            //if (Process == null || Process.HasExited) return;

            System.Diagnostics.Process firefoxProcess = FireFox.CurrentProcess;
            if (firefoxProcess == null)
            {
                return;
            }

            firefoxProcess.WaitForExit(5000);

            firefoxProcess = FireFox.CurrentProcess;
            if (firefoxProcess == null)
            {
                return;
            }
            else if (firefoxProcess.HasExited)
            {
                TryFuncUntilTimeOut waiter = new TryFuncUntilTimeOut(TimeSpan.FromMilliseconds(5000));
                bool procIsNull = waiter.Try<bool>(() => { firefoxProcess = FireFox.CurrentProcess; return firefoxProcess == null; });
                if (procIsNull)
                {
                    if (!waiter.DidTimeOut && firefoxProcess == null)
                    {
                        return;
                    }
                }
            }

            Logger.LogDebug("Killing FireFox process");
            UtilityClass.TryActionIgnoreException(() => Process.Kill());
        }

        public void CloseConnection()
        {
            Logger.LogDebug("Closing connection to mozrepl.");
            _telnetSocket.Close();
            Connected = false;
        }

        /// <summary>
        /// Writes the specified data to the mozrepl server.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="resultExpected">
        /// </param>
        /// <param name="checkForErrors">
        /// </param>
        /// <param name="args">
        /// </param>
        protected override void SendAndRead(string data, bool resultExpected, bool checkForErrors, params object[] args)
        {
            var command = UtilityClass.StringFormat(data, args);

            SendCommand(command);
            ReadResponse(checkForErrors);
        }

        /// <summary>
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <exception cref="FireFoxException">
        /// </exception>
        private static void CheckForError(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                return;
            }

            if (response.StartsWith("!!! Error", StringComparison.InvariantCultureIgnoreCase) ||
                response.StartsWith("TypeError", StringComparison.InvariantCultureIgnoreCase) ||
                response.StartsWith("uncaught exception", StringComparison.InvariantCultureIgnoreCase) ||
                response.StartsWith("!!! ReferenceError:", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new FireFoxException(string.Format("Error sending last message to mozrepl server: {0}", response));
            }
        }

        /// <summary>
        /// Cleans the response.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <returns>
        /// Response from FireFox with out any of the telnet UI characters
        /// </returns>
        private string CleanTelnetResponse(string response)
        {
            // HACK refactor in the future, should find a cleaner way of doing 
            if (!string.IsNullOrEmpty(response))
            {
                response = response.Replace(string.Format("\r\n{0}> ", PromptName), string.Empty);
                response = response.Replace(string.Format("\r\n{0}>", PromptName), string.Empty);
                response = response.Replace(string.Format("\n{0}> ", PromptName), string.Empty);
                response = response.Replace(string.Format("\n{0}>", PromptName), string.Empty);
                response = response.Replace(string.Format("{0}>", PromptName), string.Empty);
                response = response.Trim();

                if (response.StartsWith("\""))
                {
                  response = response.Substring(1);
                }
                
                if(response.EndsWith("\""))
                {
                  response = response.Substring(0, response.Length - 1);
                }

                if (response.EndsWith(string.Format("{0}>", "\n")))
                {
                    response = response.Substring(0, response.Length - 2);
                }
                else if (response.EndsWith(string.Format("?{0}> ", "\n")))
                {
                    response = response.Substring(0, response.Length - 4);
                }
                else if (response.EndsWith(string.Format("{0}> ", "\n")))
                {
                    response = response.Substring(0, response.Length - 3);
                }
                else if (response.EndsWith(string.Format("{0}> {0}", "\n")))
                {
                    response = response.Substring(0, response.Length - 4);
                }
                else if (response.EndsWith(string.Format("{0}> {0}{0}", "\n")))
                {
                    response = response.Substring(0, response.Length - 5);
                }
                else if (response.EndsWith(string.Format("{0}>", Environment.NewLine)))
                {
                    response = response.Substring(0, response.Length - 3);
                }
                else if (response.EndsWith(string.Format("{0}> ", Environment.NewLine)))
                {
                    response = response.Substring(0, response.Length - 4);
                }
                else if (response.EndsWith(string.Format("{0}> {0}", Environment.NewLine)))
                {
                    response = response.Substring(0, response.Length - 6);
                }
                else if (response.EndsWith(string.Format("{0}> {0}{0}", Environment.NewLine)))
                {
                    response = response.Substring(0, response.Length - 8);
                }

                if (response.StartsWith("> "))
                {
                    response = response.Substring(2);
                }
                else if (response.StartsWith(string.Format("{0}> ", "\n")))
                {
                    response = response.Substring(3);
                }
            }

            return response;
        }

        /// <summary>
        /// Defines the default JS variables used to automate this FireFox window.
        /// </summary>
        /// <param name="windowIndex">Index of the window.</param>
        internal void DefineDefaultJSVariablesForWindow(int windowIndex)
        {
            Write("{0}.home();", PromptName);
            
            Write("{0}.getWindows = {1}", PromptName, GetWindowsFunction());
            Write("{0}.loadURI = function(url){{ content.location.href = url; }};", PromptName);
            Write("{0}.{1} = {2}", PromptName, GetChildElementsFunctionName, GetChildElementsFunction());

            Write("var w0 = {0}.getWindows()[{1}];", PromptName, windowIndex.ToString());
            Write("var {0} = w0.content;", WindowVariableName);
            Write("var {0} = w0.getBrowser();", BrowserVariableName);
        }

        private string GetWindowsFunction()
        {
            return "function() { var windowEnum = Cc['@mozilla.org/appshell/window-mediator;1'].getService(Ci.nsIWindowMediator).getEnumerator(''); var windows = []; while(windowEnum.hasMoreElements()) windows.push(windowEnum.getNext()); return windows; }";
        }

        private static string GetChildElementsFunction()
        {
            return "function(node){var a=[];var tags=node.childNodes;for (var i=0;i<tags.length;++i){if (tags[i].nodeType!=3) a.push(tags[i]);} return a;}";
        }

        /// <summary>
        /// Reads the response from the mozrepl server.
        /// </summary>
        /// <param name="checkForErrors">
        /// The check For Errors.
        /// </param>
        private void ReadResponse(bool checkForErrors)
        {
            var stream = new NetworkStream(_telnetSocket);
            
            LastResponse = string.Empty;
            LastResponseRaw = string.Empty;

            const int bufferSize = 4096;
            var buffer = new byte[bufferSize];

            while (!stream.CanRead)
            {
                // TODO: need to work out a better way for this
                System.Threading.Thread.Sleep(10);
            }

            var readData = String.Empty;
            var EOR = string.Format("{0}{1}", PromptName, _promptSuffix);
            const string emergency = "Host context unloading! Going back to creation context.";

            do
            {
                var read = stream.Read(buffer, 0, bufferSize);
                readData += Encoding.UTF7.GetString(buffer, 0, read);
                Logger.LogDebug("readdata on {1}: {0}", readData, _prompt);
            
                // Recover 
                if (readData.Contains(emergency))
                {
                    readData = string.Empty;
                    SendCommand(";");
                }
            }
            while (!readData.EndsWith(EOR));
            LastResponseRaw += readData;
            AddToLastResponse(CleanTelnetResponse(readData));

            // Convert \n to newline
            if (LastResponse != null)
            {
                LastResponse = LastResponse.Replace("\n", Environment.NewLine);
            }
            
            Response.Append(LastResponse);

            if (checkForErrors)
            {
                CheckForError(LastResponse);
            }
        }

        /// <summary>
        /// Sends a command to the FireFox remote server.
        /// </summary>
        /// <param name="data">
        /// The data to send.
        /// </param>
        /// <exception cref="FireFoxException">When not connected.</exception>
        private void SendCommand(string data)
        {
            if (!Connected)
            {
                throw new FireFoxException("You must connect before writing to the server.");
            }

            var bytes = Encoding.ASCII.GetBytes(data + "\r\n");

            Logger.LogDebug("sending on {1}: {0}", data, _prompt);
            using (var networkStream = new NetworkStream(_telnetSocket))
            {
                networkStream.Write(bytes, 0, bytes.Length);
                networkStream.Flush();
            }
        }

        /// <summary>
        /// </summary>
        /// <exception cref="FireFoxException">
        /// </exception>
        private void CloseExistingFireFoxInstances()
        {
            System.Diagnostics.Process firefoxProcess = FireFox.CurrentProcess;
            if (firefoxProcess != null && !Settings.CloseExistingFireFoxInstances)
            {
                throw new FireFoxException("Existing instances of FireFox detected.");
            }

            var currentProcess = FireFox.CurrentProcess;
            if (currentProcess != null && !currentProcess.HasExited)
            {
                firefoxProcess.Kill();
            }
        }

        /// <summary>
        /// Writes a line to the mozrepl server.
        /// </summary>
        private void WaitForConnectionEstablished()
        {
            var rawResponse = string.Empty;
            var responseToWaitFor = "Welcome to MozRepl"; // .Replace("\n", Environment.NewLine);

            while (!rawResponse.Contains(responseToWaitFor))
            {
                ReadResponse(true);
                rawResponse += LastResponseRaw;
            }

            SendCommand(";");
            rawResponse = string.Empty;
            while (!rawResponse.Contains(_promptSuffix))
            {
              ReadResponse(true);
              rawResponse += LastResponseRaw;
            }

            _prompt = string.Format("repl{0}", ((IPEndPoint)_telnetSocket.LocalEndPoint).Port);
            SendCommand(string.Format("repl.rename(\"{0}\");", _prompt));
            ReadResponse(true);
            Logger.LogDebug("mozrepl connected: '{0}'", LastResponseRaw);
        }
    }
}
