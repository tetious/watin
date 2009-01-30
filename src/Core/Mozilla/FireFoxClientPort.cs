#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

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
using WatiN.Core.UtilityClasses;

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
        /// Underlying socket used to create a <see cref="NetworkStream"/>.
        /// </summary>
        private Socket telnetSocket;

        /// <summary>
        /// The last reponse recieved from the jssh server
        /// </summary>
        private string lastResponse;
        private string lastResponseRaw;

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
        public bool Connected { get; private set; }

        /// <summary>
        /// The last reponse recieved from the jssh server
        /// </summary>
        private string LastResponse
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
        private bool LastResponseIsNull
        {
            get
            {
                return lastResponse.Equals("null", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Retruns the last reponse as a Boolen, default to false if converting <see cref="LastResponse"/> fails.
        /// </summary>
        private bool LastResponseAsBool
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
            if (elementCounter == long.MaxValue) elementCounter = 0;
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
            // Check to see if setting the reference (and doing the loop for activeElement which is time consuming)
            // is needed.
            var needToUpdateDocumentReference = WriteAndReadAsBool("{0} != {1}.document", DocumentVariableName, WindowVariableName);
            if (!needToUpdateDocumentReference) return;

            // Sets up the document variable
            Write("var {0} = {1}.document;", DocumentVariableName, WindowVariableName);

            // Javascript to implement document.activeElement if not supported by browser (FireFox 2.x)
            Write("if (" + DocumentVariableName + ".activeElement == null){" + DocumentVariableName + ".activeElement = " + DocumentVariableName + ".body;" +
                  "var allElements = " + DocumentVariableName + ".getElementsByTagName(\"*\");" +
                  "for (i = 0; i < allElements.length; i++){" +
                  "allElements[i].addEventListener(\"focus\", function (event) {" +
                  DocumentVariableName + ".activeElement = event.target;}, false);}}");
        }

        public void Connect(string arguments)
        {
            ValidateCanConnect();
            disposed = false;
            Logger.LogDebug("Attempting to connect to jssh server on localhost port 9997.");
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
                Logger.LogDebug(string.Format("Failed connecting to jssh server.\nError code:{0}\nError message:{1}", sockException.ErrorCode, sockException.Message));
                throw new FireFoxException("Unable to connect to jssh server, please make sure you have correctly installed the jssh.xpi plugin", sockException);
            }

            Connected = true;
            WaitForConnectionEstablished();
            Logger.LogDebug("Successfully connected to FireFox using jssh.");
            DefineDefaultJSVariables();
        }

        private void ValidateCanConnect()
        {
            if (Connected)
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

        private int LastResponseAsInt
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
                            Logger.LogDebug("Closing connection to jssh");
                            Write(string.Format("{0}.close();", WindowVariableName));
                            telnetSocket.Close();
                            Process.WaitForExit(5000);
                        }
                        catch (IOException ex)
                        {
                            Logger.LogDebug("Error communicating with jssh server to innitiate shut down, message: " + ex.Message);
                        }
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                if (Process != null)
                {
                    if (!Process.HasExited)
                    {
                        Logger.LogDebug("Closing FireFox");
                        Process.Kill();
                    }
                }
            }

            disposed = true;
            Connected = false;
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
        private void WaitForConnectionEstablished()
        {
            SendCommand("\n");
            
            var rawResponse = string.Empty;
            var responseToWaitFor = "Welcome to the Mozilla JavaScript Shell!\n\n> \n> \n> "; //.Replace("\n", Environment.NewLine);

            while (rawResponse != responseToWaitFor)
            {
                ReadResponse(false, true);
                rawResponse += lastResponseRaw;
            }
        }

        /// <summary>
        /// Writes the specified data to the jssh server.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="resultExpected"></param>
        /// <param name="checkForErrors"></param>
        /// <param name="args"></param>
        private void SendAndRead(string data, bool resultExpected, bool checkForErrors, params object[] args)
        {
            var command = UtilityClass.StringFormat(data, args);

            SendCommand(command);
            ReadResponse(resultExpected, checkForErrors);
        }

        private void SendCommand(string data)
        {
            if (!Connected)
            {
                throw new FireFoxException("You must connect before writing to the server.");
            }

            var bytes = Encoding.ASCII.GetBytes(data + "\n");

            Logger.LogDebug("sending: {0}", data);
            using (var networkStream = new NetworkStream(telnetSocket))
            {
                networkStream.Write(bytes, 0, bytes.Length);
                networkStream.Flush();
            }
        }

        /// <summary>
        /// Writes the specified data to the jssh server.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="args">Arguments to be passed to <see cref="string.Format(string,object[])"/></param>
        public void Write(string data, params object[] args)
        {
            var command = data.EndsWith(";") == false ? data + ";" : data;
            SendAndRead(command + " true;", true, true, args);
        }

        public string WriteAndReadIgnoreError(string data, params object[] args)
        {
            SendAndRead(data, false, false, args);
            return LastResponse;
        }

        public string WriteAndRead(string data, params object[] args)
        {
            SendAndRead(data, false, true, args);
            return LastResponse;
        }

        public bool WriteAndReadAsBool(string data, params object[] args)
        {
            SendAndRead(data, true, true, args);
            return LastResponseAsBool;
        }

        public int WriteAndReadAsInt(string data, params object[] args)
        {
            SendAndRead(data, true, true, args);
            return LastResponseAsInt;
        }

        /// <summary>
        /// Reads the response from the jssh server.
        /// </summary>
        private void ReadResponse(bool resultExpected, bool checkForErrors)
        {
            var stream = new NetworkStream(telnetSocket);
            
            lastResponse = string.Empty;
            lastResponseRaw = string.Empty;

            var bufferSize = 4096;
            var buffer = new byte[bufferSize];

            while (!stream.CanRead)
            {
                // Hack: need to work out a better way for this
                System.Threading.Thread.Sleep(10);
            }

            string readData;
            do
            {
                var read = stream.Read(buffer, 0, bufferSize);
                readData = Encoding.UTF8.GetString(buffer, 0, read);

                Logger.LogDebug("jssh says: '" + readData.Replace("\n", "[newline]") + "'");
                lastResponseRaw += readData;
                lastResponse += CleanTelnetResponse(readData);
            } while (!readData.EndsWith("> ") || stream.DataAvailable || (resultExpected && string.IsNullOrEmpty(lastResponse)));

            response.Append(lastResponse);

            if (checkForErrors)
            {
                CheckForError(lastResponse);
            }

        }

        private static void CheckForError(string response)
        {
            if (response.StartsWith("SyntaxError", StringComparison.InvariantCultureIgnoreCase) ||
                response.StartsWith("TypeError", StringComparison.InvariantCultureIgnoreCase) ||
                response.StartsWith("uncaught exception", StringComparison.InvariantCultureIgnoreCase) ||
                response.StartsWith("ReferenceError:", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new FireFoxException(string.Format("Error sending last message to jssh server: {0}", response));
            }
        }

        #endregion private instance methods
    }
}