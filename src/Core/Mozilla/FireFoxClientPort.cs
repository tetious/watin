#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using WatiN.Core.Logging;

namespace WatiN.Core.Mozilla
{
    public class FireFoxClientPort : IDisposable
    {
        #region Private fields

        /// <summary>
        /// Used by CreateElementVariableName
        /// </summary>
        private static long elementCounter;

        /// <summary>
        /// <c>true</c> if we have successfully connected to FireFox
        /// </summary>
        private bool connected;

        /// <summary>
        /// Underlying socket used to create a <see cref="NetworkStream"/>.
        /// </summary>
        private Socket telnetSocket;

        /// <summary>
        /// The last reponse recieved from the jssh server
        /// </summary>
        private string lastResponse;

        /// <summary>
        /// The entire response from the jssh server so far.
        /// </summary>
        private StringBuilder response;

        /// <summary>
        /// <c>true</c> if the <see cref="Dispose()"/> method has been called to release resources.
        /// </summary>
        private bool disposed;

        #endregion

        #region Public constants

        /// <summary>
        /// Name of the javascript variable that references the XUL:browser object.
        /// </summary>
        public const string BrowserVariableName = "browser";

        /// <summary>
        /// Name of the javascript variable that references the DOM:document object.
        /// </summary>
        public const string DocumentVariableName = "doc";

        /// <summary>
        /// Name of the javascript variable that references the DOM:window object.
        /// </summary>
        public const string WindowVariableName = "window";

        #endregion

        #region Constructors / destructors

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="FireFox"/> is reclaimed by garbage collection.
        /// </summary>
        ~FireFoxClientPort()
        {
            Dispose(false);
        }

        #endregion

        #region Public instance properties

        /// <summary>
        /// <c>true</c> if we have successfully connected to FireFox
        /// </summary>
        public bool Connected
        {
            get { return connected; }
        }

        /// <summary>
        /// The last reponse recieved from the jssh server
        /// </summary>
        public string LastResponse
        {
            get
            {
                return LastResponseIsNull ? null : lastResponse;
            }
        }

        /// <summary>
        /// The entire response from the jssh server so far.
        /// </summary>
        public string Response
        {
            get { return response.ToString(); }
        }

        /// <summary>
        /// Gets a value indicating whether last response is null.
        /// </summary>
        /// <value><c>true</c> if last response is null; otherwise, <c>false</c>.</value>
        public bool LastResponseIsNull
        {
            get
            {
                return lastResponse.Equals("null", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Retruns the last reponse as a Boolen, default to false if converting <see cref="LastResponse"/> fails.
        /// </summary>
        public bool LastResponseAsBool
        {
            get
            {
                bool lastBoolResponse;
                Boolean.TryParse(LastResponse, out lastBoolResponse);
                return lastBoolResponse;
            }
        }

        #endregion

        #region Internal instance properties

        internal Process Process { get; private set; }

        #endregion

        #region Public static methods

        /// <summary>
        /// Creates a unique variable name
        /// </summary>
        /// <returns></returns>
        public static string CreateVariableName()
        {
            elementCounter++;
            return string.Format("{0}.watin{1}", DocumentVariableName, elementCounter);
        }

        #endregion

        #region Public instance methods

        /// <summary>
        /// Reloads the javascript variables that are scoped at the document level.
        /// </summary>
        public void InitializeDocument()
        {
            // Sets up the document variable
            Write("var {0} = {1}.document;", DocumentVariableName, WindowVariableName);

            // Javascript to implement document.activeElement currently not support by Mozilla
            Write("if (" + DocumentVariableName + ".activeElement == null){" + DocumentVariableName + ".activeElement = " + DocumentVariableName + ".body;\n" +
                       "var allElements = " + DocumentVariableName + ".getElementsByTagName(\"*\");\n" +
                       "for (i = 0; i < allElements.length; i++)\n{\n" +
                       "allElements[i].addEventListener(\"focus\", function (event) {\n" +
                       DocumentVariableName + ".activeElement = event.target;}, false);\n}}");
        }

        public void Connect(string arguments)
        {
            ValidateCanConnect();
            disposed = false;
            Logger.LogAction("Attempting to connect to jssh server on localhost port 9997.");
            lastResponse = string.Empty;
            response = new StringBuilder();

            Process = FireFox.CreateProcess(arguments + " -jssh", true);

            if (!IsMainWindowVisible)
            {
                if (IsRestoreSessionDialogVisible)
                {
                    NativeMethods.SetForegroundWindow(Process.MainWindowHandle);
                    SendKeys.SendWait("{TAB}");
                    SendKeys.SendWait("{ENTER}");
                }
            }

            telnetSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {Blocking = true};

            try
            {
                telnetSocket.Connect(IPAddress.Parse("127.0.0.1"), 9997);
            }
            catch (SocketException sockException)
            {
                Logger.LogAction(string.Format("Failed connecting to jssh server.\nError code:{0}\nError message:{1}", sockException.ErrorCode, sockException.Message));
                throw new FireFoxException("Unable to connect to jssh server, please make sure you have correctly installed the jssh.xpi plugin", sockException);
            }

            connected = true;
            WriteLine();
            Logger.LogAction("Successfully connected to FireFox using jssh.");
//            WriteAndRead("setProtocol('synchronous')");
            DefineDefaultJSVariables();

        }

        private void ValidateCanConnect()
        {
            if (connected)
            {
                throw new FireFoxException("Already connected to jssh server.");
            }

            if (FireFox.CurrentProcessCount > 0 && !Settings.CloseExistingBrowserInstances)
            {
                throw new FireFoxException("Existing instances of FireFox detected.");
            }
            
            if (FireFox.CurrentProcessCount > 0)
            {
                FireFox.CurrentProcess.Kill();
            }
        }

        private bool IsRestoreSessionDialogVisible
        {
            get
            {
                var result = NativeMethods.GetWindowText(Process.MainWindowHandle).Contains("Firefox - Restore Previous Session");
                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating if the main FireFox window is visible, it's possible that the
        /// main FireFox window is not visible if a previous shutdown didn't complete correctly
        /// in which case the restore / resume previous session dialog may be visible.
        /// </summary>
        internal bool IsMainWindowVisible
        {
            get
            {
                var result = NativeMethods.GetWindowText(Process.MainWindowHandle).Contains("Mozilla Firefox");
                return result;
            }
        }

        public int LastResponseAsInt
        {
            get { return string.IsNullOrEmpty(LastResponse) ? 0 : int.Parse(lastResponse); }
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        #endregion Public instance methods

        #region Protected instance methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (telnetSocket != null && telnetSocket.Connected && !Process.HasExited)
                    {
                        try
                        {
                            Logger.LogAction("Closing connection to jssh");
                            Write(string.Format("{0}.close()", WindowVariableName));
                            telnetSocket.Close();
                            Process.WaitForExit(5000);
                        }
                        catch (IOException ex)
                        {
                            Logger.LogAction("Error communicating with jssh server to innitiate shut down, message: " + ex.Message);
                        }
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                if (Process != null)
                {
                    if (!Process.HasExited)
                    {
                        Logger.LogAction("Closing FireFox");
                        Process.Kill();
                    }
                }
            }

            disposed = true;
            connected = false;
        }

        #endregion Protected instance methods

        #region Private static methods

        /// <summary>
        /// Cleans the response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>Response from FireFox with out any of the telnet UI characters</returns>
        private static string CleanTelnetResponse(string response)
        {
            //HACK refactor in the future, should find a cleaner way of doing this.
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

                response = response.Trim();
            }
            return response;
        }

        #endregion

        #region Private instance methods

        /// <summary>
        /// Defines the default JS variables used to automate FireFox.
        /// </summary>
        private void DefineDefaultJSVariables()
        {
            Write("var w0 = getWindows()[0];", WindowVariableName);
            Write("var {0} = w0.content;", WindowVariableName);
            Write("var {0} = {1}.document;", DocumentVariableName, WindowVariableName);
            Write("var {0} = w0.getBrowser();", BrowserVariableName);
        }

        /// <summary>
        /// Writes a line to the jssh server.
        /// </summary>
        private void WriteLine()
        {
            Write("\n");
        }

        /// <summary>
        /// Writes the specified data to the jssh server.
        /// </summary>
        /// <param name="data">The data.</param>
        internal void Write(string data)
        {
            if (!connected)
            {
                throw new FireFoxException("You must connect before writing to the server.");
            }

            var bytes = Encoding.ASCII.GetBytes(data + "\n");

            Logger.LogAction("sending: {0}", data);
            using (var networkStream = new NetworkStream(telnetSocket))
            {
                networkStream.Write(bytes, 0, bytes.Length);
            }

            ReadResponse();
        }

        /// <summary>
        /// Writes the specified data to the jssh server.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="args">Arguments to be passed to <see cref="string.Format(string,object[])"/></param>
        internal void Write(string data, params object[] args)
        {
            Write(string.Format(data, args));
        }

        internal string WriteAndRead(string data, params object[] args)
        {
            Write(data, args);
            return LastResponse;
        }

        /// <summary>
        /// Reads the response from the jssh server.
        /// </summary>
        private void ReadResponse()
        {
            var stream = new NetworkStream(telnetSocket);
            lastResponse = string.Empty;

            var bufferSize = 4096;
            var buffer = new byte[bufferSize];
            while (!stream.CanRead)
//            while (!stream.DataAvailable)
            {
                // Hack: need to work out a better way for this
                System.Threading.Thread.Sleep(200);
            }

            string readData;
            do
            {
                var read = stream.Read(buffer, 0, bufferSize);
//                readData = UnicodeEncoding.ASCII.GetString(buffer, 0, read);
                readData = Encoding.UTF8.GetString(buffer, 0, read);

                Logger.LogAction("jssh says: '" + readData.Replace("\n", "[newline]") + "'");
                lastResponse += CleanTelnetResponse(readData);
            } while (!readData.EndsWith("> ") || stream.DataAvailable);
//            } while (read == 1024);

            lastResponse = lastResponse.Trim();
            if (lastResponse.StartsWith("SyntaxError", StringComparison.InvariantCultureIgnoreCase) ||
                lastResponse.StartsWith("TypeError", StringComparison.InvariantCultureIgnoreCase) ||
                lastResponse.StartsWith("uncaught exception", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new FireFoxException(string.Format("Error sending last message to jssh server: {0}", lastResponse));
            }

            response.Append(lastResponse);

        }

        #endregion private instance methods
    }
}