// --------------------------------------------------------------------------------------------------------------------- 
// <copyright file="FireFoxClientPort.cs">
//   Copyright 2006-2009 Jeroen van Menen
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
// </copyright>
// <summary>
//   The firefox client port used to communicate with the remote automation server jssh.
// </summary>
// ---------------------------------------------------------------------------------------------------------------------

namespace WatiN.Core.Native.Mozilla
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Windows.Forms;

    using WatiN.Core.Logging;
    using WatiN.Core.Native.Windows;
    using WatiN.Core.UtilityClasses;

    /// <summary>
    /// The firefox client port used to communicate with the remote automation server jssh.
    /// </summary>
    public class FireFoxClientPort : ClientPortBase, IDisposable
    {
        /// <summary>
        /// Name of the javascript variable that references the XUL:browser object.
        /// </summary>
        public const string BrowserVariableName = "browser";

        /// <summary>
        /// Name of the javascript variable that references the DOM:window object.
        /// </summary>
        public const string WindowVariableName = "window";

        /// <summary>
        /// <c>true</c> if the <see cref="Dispose()"/> method has been called to release resources.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Underlying socket used to create a <see cref="NetworkStream"/>.
        /// </summary>
        private Socket telnetSocket;

        /// <summary>
        /// Finalizes an instance of the <see cref="FireFoxClientPort"/> class. 
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="FireFox"/> is reclaimed by garbage collection.
        /// </summary>
        ~FireFoxClientPort()
        {
            this.Dispose(false);
        }

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
            get
            {
                return "doc";
            }   
        }

        /// <summary>
        /// Gets a value indicating whether the main FireFox window is visible, it's possible that the
        /// main FireFox window is not visible if a previous shutdown didn't complete correctly
        /// in which case the restore / resume previous session dialog may be visible.
        /// </summary>
        internal bool IsMainWindowVisible
        {
            get
            {
                var result = NativeMethods.GetWindowText(this.Process.MainWindowHandle).Contains("Mozilla Firefox");
                return result;
            }
        }

        /// <summary>
        /// Gets Process.
        /// </summary>
        /// <value>
        /// The process.
        /// </value>
        internal Process Process { get; private set; }

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
                var result = NativeMethods.GetWindowText(this.Process.MainWindowHandle).Contains("Firefox - ");
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
        public void Connect(string url)
        {
            if (string.IsNullOrEmpty(url)) url = "about:blank";

            this.ValidateCanConnect();
            this.disposed = false;
            
            Logger.LogDebug("Attempting to connect to jssh server on localhost port 9997.");
            
            this.LastResponse = string.Empty;
            this.Response = new StringBuilder();

            this.Process = Core.FireFox.CreateProcess(url + " -jssh", true);

            if (!this.IsMainWindowVisible)
            {
                if (this.IsRestoreSessionDialogVisible)
                {
                    NativeMethods.SetForegroundWindow(this.Process.MainWindowHandle);
                    SendKeys.SendWait("{TAB}");
                    SendKeys.SendWait("{ENTER}");
                }
            }

            this.telnetSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {Blocking = true};

            try
            {
                this.telnetSocket.Connect(IPAddress.Parse("127.0.0.1"), 9997);
            }
            catch (SocketException sockException)
            {
                Logger.LogDebug(string.Format("Failed connecting to jssh server.\nError code:{0}\nError message:{1}", sockException.ErrorCode, sockException.Message));
                throw new FireFoxException("Unable to connect to jssh server, please make sure you have correctly installed the jssh.xpi plugin", sockException);
            }

            this.Connected = true;
            this.WaitForConnectionEstablished();
            Logger.LogDebug("Successfully connected to FireFox using jssh.");
            this.DefineDefaultJSVariables();
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);

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
            // Check to see if setting the reference (and doing the loop for activeElement which is time consuming)
            // is needed.
            var needToUpdateDocumentReference = this.WriteAndReadAsBool("{0} != {1}.document", DocumentVariableName, WindowVariableName);
            if (!needToUpdateDocumentReference) return;

            // Sets up the document variable
            this.Write("var {0} = {1}.document;", DocumentVariableName, WindowVariableName);

            // Javascript to implement document.activeElement if not supported by browser (FireFox 2.x)
            this.Write("if (" + DocumentVariableName + ".activeElement == null){" + DocumentVariableName + ".activeElement = " + DocumentVariableName + ".body;" +
                       "var allElements = " + DocumentVariableName + ".getElementsByTagName(\"*\");" +
                       "for (i = 0; i < allElements.length; i++){" +
                       "allElements[i].addEventListener(\"focus\", function (event) {" +
                       DocumentVariableName + ".activeElement = event.target;}, false);}}");
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
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (this.telnetSocket != null && this.telnetSocket.Connected && !this.Process.HasExited)
                    {
                        try
                        {
                            Logger.LogDebug("Closing connection to jssh");
                            this.Write(string.Format("{0}.close();", WindowVariableName));
                            this.telnetSocket.Close();
                            this.Process.WaitForExit(5000);
                        }
                        catch (IOException ex)
                        {
                            Logger.LogDebug("Error communicating with jssh server to innitiate shut down, message: " + ex.Message);
                        }
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                if (this.Process != null)
                {
                    if (!this.Process.HasExited)
                    {
                        Logger.LogDebug("Closing FireFox");
                        this.Process.Kill();
                    }
                }
            }

            this.disposed = true;
            this.Connected = false;
        }

        /// <summary>
        /// Writes the specified data to the jssh server.
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

            this.SendCommand(command);
            this.ReadResponse(resultExpected, checkForErrors);
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

            if (response.StartsWith("SyntaxError", StringComparison.InvariantCultureIgnoreCase) ||
                response.StartsWith("TypeError", StringComparison.InvariantCultureIgnoreCase) ||
                response.StartsWith("uncaught exception", StringComparison.InvariantCultureIgnoreCase) ||
                response.StartsWith("ReferenceError:", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new FireFoxException(string.Format("Error sending last message to jssh server: {0}", response));
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
        private static string CleanTelnetResponse(string response)
        {
            // HACK refactor in the future, should find a cleaner way of doing this.
            if (!string.IsNullOrEmpty(response))
            {
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
        /// Defines the default JS variables used to automate FireFox.
        /// </summary>
        private void DefineDefaultJSVariables()
        {
            this.Write("var w0 = getWindows()[0];");
            this.Write("var {0} = w0.content;", WindowVariableName);
            this.Write("var {0} = w0.document;", DocumentVariableName);
            this.Write("var {0} = w0.getBrowser();", BrowserVariableName);
        }

        /// <summary>
        /// Reads the response from the jssh server.
        /// </summary>
        /// <param name="resultExpected">
        /// The result Expected.
        /// </param>
        /// <param name="checkForErrors">
        /// The check For Errors.
        /// </param>
        private void ReadResponse(bool resultExpected, bool checkForErrors)
        {
            var stream = new NetworkStream(this.telnetSocket);
            
            this.LastResponse = string.Empty;
            this.LastResponseRaw = string.Empty;

            var bufferSize = 4096;
            var buffer = new byte[bufferSize];

            while (!stream.CanRead)
            {
                // TODO: need to work out a better way for this
                System.Threading.Thread.Sleep(10);
            }

            string readData;
            do
            {
                var read = stream.Read(buffer, 0, bufferSize);
                readData = Encoding.UTF8.GetString(buffer, 0, read);

                Logger.LogDebug("jssh says: '" + readData.Replace("\n", "[newline]") + "'");
                this.LastResponseRaw += readData;
                AddToLastResponse(CleanTelnetResponse(readData));
            }
            while (!readData.EndsWith("> ") || stream.DataAvailable || (resultExpected && string.IsNullOrEmpty(this.LastResponse)));

            // Convert \n to newline
            if (this.LastResponse != null)
            {
                this.LastResponse = this.LastResponse.Replace("\n", Environment.NewLine);
            }
            
            this.Response.Append(this.LastResponse);

            if (checkForErrors)
            {
                CheckForError(this.LastResponse);
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
            if (!this.Connected)
            {
                throw new FireFoxException("You must connect before writing to the server.");
            }

            var bytes = Encoding.ASCII.GetBytes(data + "\n");

            Logger.LogDebug("sending: {0}", data);
            using (var networkStream = new NetworkStream(this.telnetSocket))
            {
                networkStream.Write(bytes, 0, bytes.Length);
                networkStream.Flush();
            }
        }

        /// <summary>
        /// </summary>
        /// <exception cref="FireFoxException">
        /// </exception>
        private void ValidateCanConnect()
        {
            if (this.Connected)
            {
                throw new FireFoxException("Already connected to jssh server.");
            }

            if (Core.FireFox.CurrentProcessCount > 0 && !Settings.CloseExistingBrowserInstances)
            {
                throw new FireFoxException("Existing instances of FireFox detected.");
            }
            
            if (Core.FireFox.CurrentProcessCount > 0)
            {
                Core.FireFox.CurrentProcess.Kill();
            }
        }

        /// <summary>
        /// Writes a line to the jssh server.
        /// </summary>
        private void WaitForConnectionEstablished()
        {
            this.SendCommand("\n");
            
            var rawResponse = string.Empty;
            var responseToWaitFor = "Welcome to the Mozilla JavaScript Shell!\n\n> \n> \n> "; // .Replace("\n", Environment.NewLine);

            while (rawResponse != responseToWaitFor)
            {
                this.ReadResponse(false, true);
                rawResponse += this.LastResponseRaw;
            }
        }
    }
}